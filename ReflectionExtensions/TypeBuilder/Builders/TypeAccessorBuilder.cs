

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Reflection;
	using Reflection.Emit;

	class TypeAccessorBuilder : ITypeBuilder
	{
		public TypeAccessorBuilder(Type type, Type originalType)
		{
			_type         = type         ?? throw new ArgumentNullException(nameof(type));
			_originalType = originalType ?? throw new ArgumentNullException(nameof(originalType));
		}

		readonly Type                    _type;
		readonly Type                    _originalType;
		readonly Type                    _accessorType   = typeof(TypeAccessor);
		readonly Type                    _memberAccessor = typeof(MemberAccessor);
		readonly List<TypeBuilderHelper> _nestedTypes    = new List<TypeBuilderHelper>();
		         TypeBuilderHelper?      _typeBuilder;
		         bool                    _friendlyAssembly;

		public string AssemblyNameSuffix => "TypeAccessor";

		public string GetTypeName()
		{
			// It's a bad idea to use '.TypeAccessor' here since we got
			// a class and a namespace with the same full name.
			// The sgen utility fill fail in such case.
			//
			return _type.FullName?.Replace('+', '.') + "$TypeAccessor";
		}

		public Type GetBuildingType()
		{
			return _type;
		}

		public Type? Build(AssemblyBuilderHelper assemblyBuilder)
		{
			if (assemblyBuilder == null) throw new ArgumentNullException(nameof(assemblyBuilder));

			// Check InternalsVisibleToAttributes of the source type's assembly.
			// Even if the sourceType is public, it may have internal fields and props.
			//
			_friendlyAssembly = false;

			// Usually, there is no such attribute in the source assembly.
			// Therefore we do not cache the result.
			//
			var attributes = _originalType.Assembly.GetCustomAttributes(typeof(InternalsVisibleToAttribute), true);

			foreach (InternalsVisibleToAttribute visibleToAttribute in attributes)
			{
				var an = new AssemblyName(visibleToAttribute.AssemblyName);

				if (AssemblyName.ReferenceMatchesDefinition(assemblyBuilder.AssemblyName, an))
				{
					_friendlyAssembly = true;
					break;
				}
			}

			var typeName = GetTypeName();

			_typeBuilder = assemblyBuilder.DefineType(typeName, _accessorType);

			var ctor = _accessorType.GetDefaultConstructor();

			Debug.Assert(ctor != null, nameof(ctor) + " != null");

			_typeBuilder.DefaultConstructor.Emitter
				.ldarg_0
				.call    (ctor)
				;

			BuildCreateInstanceMethods();
			BuildTypeProperties();
			BuildMembers();
			BuildObjectFactory();

			_typeBuilder.DefaultConstructor.Emitter
				.ret()
				;

			var result = _typeBuilder.Create();

			foreach (TypeBuilderHelper tb in _nestedTypes)
				tb.Create();

			return result;
		}

		void BuildCreateInstanceMethods()
		{
			var isValueType  = _type.IsValueType;
			var baseDefCtor  = isValueType? null: _type.GetDefaultConstructor();
			var baseInitCtor = _type.GetConstructorEx(typeof(InitContext));

			if (baseDefCtor == null && baseInitCtor == null && !isValueType)
				return;

			Debug.Assert(_typeBuilder != null, nameof(_typeBuilder) + " != null");

			// CreateInstance.
			//
			var method = _typeBuilder.DefineMethod(_accessorType.GetMethodEx(nameof(TypeAccessor.CreateInstance), Type.EmptyTypes));

			if (baseDefCtor != null)
			{
				method.Emitter
					.newobj (baseDefCtor)
					.ret()
					;
			}
			else if (isValueType)
			{
				var locObj = method.Emitter.DeclareLocal(_type);

				method.Emitter
					.ldloca  (locObj)
					.initobj (_type)
					.ldloc   (locObj)
					.box     (_type)
					.ret()
					;
			}
			else
			{
				Debug.Assert(baseInitCtor != null, nameof(baseInitCtor) + " != null");

				method.Emitter
					.ldnull
					.newobj (baseInitCtor)
					.ret()
					;
			}

			// CreateInstance(IniContext).
			//
			method = _typeBuilder.DefineMethod(
				_accessorType.GetMethodEx(nameof(TypeAccessor.CreateInstance), typeof(InitContext)));

			if (baseInitCtor != null)
			{
				method.Emitter
					.ldarg_1
					.newobj (baseInitCtor)
					.ret()
					;
			}
			else if (isValueType)
			{
				var locObj = method.Emitter.DeclareLocal(_type);

				method.Emitter
					.ldloca  (locObj)
					.initobj (_type)
					.ldloc   (locObj)
					.box     (_type)
					.ret()
					;
			}
			else
			{
				Debug.Assert(baseDefCtor != null, nameof(baseDefCtor) + " != null");

				method.Emitter
					.newobj (baseDefCtor)
					.ret()
					;
			}
		}

		void BuildTypeProperties()
		{
			Debug.Assert(_typeBuilder != null, nameof(_typeBuilder) + " != null");

			// Type.
			//
			var method = _typeBuilder.DefineMethod(_accessorType.GetProperty(nameof(TypeAccessor.Type)).GetGetMethod());

			method.Emitter
				.LoadType(_type)
				.ret()
				;

			// OriginalType.
			//
			method =
				_typeBuilder.DefineMethod(_accessorType.GetProperty(nameof(TypeAccessor.InstanceType)).GetGetMethod());

			method.Emitter
				.LoadType(_originalType)
				.ret()
				;
		}

		void BuildMembers()
		{
			var members = new Dictionary<string,MemberInfo>();

			foreach (var fi in _originalType.GetFields(BindingFlags.Instance | BindingFlags.Public))
				AddMemberToDictionary(members, fi);

			if (_friendlyAssembly)
			{
				foreach (var fi in _originalType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
					if (fi.IsAssembly || fi.IsFamilyOrAssembly)
						AddMemberToDictionary(members, fi);
			}

			foreach (var pi in _originalType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				if (pi.GetIndexParameters().Length == 0)
					AddMemberToDictionary(members, pi);

			var interfaceMethods = _originalType.IsClassEx() && !_originalType.IsArrayEx()
				? _originalType.GetInterfacesEx().SelectMany(ti => _originalType.GetInterfaceMap(ti).TargetMethods).ToList()
				: new List<MethodInfo>();

			foreach (var pi in _originalType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				if (pi.GetIndexParameters().Length == 0)
				{
					var getter = pi.GetGetMethod(true);
					var setter = pi.GetSetMethod(true);

					if (getter != null && (getter.IsAbstract || interfaceMethods.Contains(getter))
						|| setter != null && (setter.IsAbstract || interfaceMethods.Contains(setter)))
						AddMemberToDictionary(members, pi);
				}
			}

			foreach (var mi in members.Values)
				BuildMember(mi);
		}

		static void AddMemberToDictionary(IDictionary<string,MemberInfo> members, MemberInfo mi)
		{
			MemberInfo existing;

			var name = mi.Name;

			if (!members.TryGetValue(name, out existing))
			{
				members.Add(name, mi);
			}
			else if (mi.DeclaringType.IsSubclassOf(existing.DeclaringType))
			{
				// mi is a member of the most descendant type.
				//
				members[name] = mi;
			}
		}

		void BuildMember(MemberInfo mi)
		{
			Debug.Assert(_typeBuilder != null, nameof(_typeBuilder) + " != null");

			var isValueType = _originalType.IsValueType;
			var name        = "Accessor$" + mi.Name.Replace('.', '_').Replace('<', '_').Replace('>', '_');
			var nestedType  = _typeBuilder.DefineNestedType(name, TypeAttributes.NestedPrivate, typeof(MemberAccessor));
			var ctorBuilder = BuildNestedTypeConstructor(nestedType);

			BuildGetter    (mi, nestedType);
			if (!isValueType)
				BuildSetter(mi, nestedType);
			BuildInitMember(mi, ctorBuilder);

			var type = mi is FieldInfo ? ((FieldInfo)mi).FieldType : ((PropertyInfo)mi).PropertyType;

			BuildIsNull    (mi, nestedType, type);

			if (type.IsEnum)
				type = Enum.GetUnderlyingType(type);

			string? typedPropertyName = type.Name;

			if (type.IsGenericType)
			{
				var underlyingType = Nullable.GetUnderlyingType(type);

				if (underlyingType != null)
				{
					BuildTypedGetterForNullable    (mi, nestedType, underlyingType);
					if (!isValueType)
						BuildTypedSetterForNullable(mi, nestedType, underlyingType);

					if (underlyingType.IsEnum)
					{
						// Note that PEVerify will complain on using Nullable<SomeEnum> as Nullable<Int32>.
						// It works in the current CLR implementation, bu may not work in future releases.
						//
						underlyingType = Enum.GetUnderlyingType(underlyingType);
						type = typeof(Nullable<>).MakeGenericType(underlyingType);
					}

					typedPropertyName = "Nullable" + underlyingType.Name;
				}
				else
				{
					typedPropertyName = null;
				}
			}

			if (typedPropertyName != null)
			{
				BuildTypedGetter    (mi, nestedType, typedPropertyName);
				if (!isValueType)
					BuildTypedSetter(mi, nestedType, type, typedPropertyName);
			}

			if (!isValueType)
				BuildCloneValueMethod(mi, nestedType, type);

			// FW 1.1 wants nested types to be created before parent.
			//
			_nestedTypes.Add(nestedType);
		}

		void BuildInitMember(MemberInfo mi, ConstructorBuilderHelper ctorBuilder)
		{
			Debug.Assert(_typeBuilder != null, nameof(_typeBuilder) + " != null");

			_typeBuilder.DefaultConstructor.Emitter
				.ldarg_0
				.ldarg_0
				.ldarg_0
				.ldc_i4  (mi is FieldInfo? 1: 2)
				.ldstr   (mi.Name)
				.call    (_accessorType.GetMethodEx("GetMember", typeof(int), typeof(string)))
				.newobj  (ctorBuilder)
				.call    (_accessorType.GetMethodEx("AddMember", typeof(MemberAccessor)))
				;
		}

		/// <summary>
		/// Figure out is base type method is accessible by extension type method.
		/// </summary>
		/// <param name="method">A <see cref="MethodInfo"/> instance.</param>
		/// <returns>True if the method access is Public or Family and it's assembly is friendly.</returns>
		bool IsMethodAccessible(MethodInfo method)
		{
			if (method == null) throw new ArgumentNullException(nameof(method));

			return method.IsPublic || (_friendlyAssembly && (method.IsAssembly || method.IsFamilyOrAssembly));
		}

		void BuildGetter(MemberInfo mi, TypeBuilderHelper nestedType)
		{
			var methodType = mi.DeclaringType;
			var getMethod  = null as MethodInfo;

			if (mi is PropertyInfo)
			{
				getMethod = ((PropertyInfo)mi).GetGetMethod();

				if (getMethod == null)
				{
					if (_type != _originalType)
					{
						getMethod  = _type.GetMethod("get_" + mi.Name);
						methodType = _type;
					}

					if (getMethod == null || !IsMethodAccessible(getMethod))
						return;
				}
			}

			var method = nestedType.DefineMethod(_memberAccessor.GetMethodEx("GetValue", typeof(object)));
			var emit   = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.end();

			if (mi is FieldInfo fi)
			{
				emit
					.ldfld          (fi)
					.boxIfValueType (fi.FieldType)
					;
			}
			else
			{
				if (methodType.IsValueType)
				{
					var loc = emit.DeclareLocal(methodType);

					emit
						.stloc      ((byte)loc.LocalIndex)
						.ldloca_s   ((byte)loc.LocalIndex);
				}

				var pi = (PropertyInfo)mi;

				Debug.Assert(getMethod != null, nameof(getMethod) + " != null");

				emit
					.callvirt       (getMethod)
					.boxIfValueType (pi.PropertyType)
					;
			}

			emit
				.ret()
				;

			nestedType.DefineMethod(_memberAccessor.GetProperty("HasGetter").GetGetMethod()).Emitter
				.ldc_i4_1
				.ret()
				;
		}

		void BuildSetter(MemberInfo mi, TypeBuilderHelper nestedType)
		{
			var methodType = mi.DeclaringType;
			var setMethod  = null as MethodInfo;

			if (mi is PropertyInfo)
			{
				setMethod = ((PropertyInfo)mi).GetSetMethod();

				if (setMethod == null)
				{
					if (_type != _originalType)
					{
						setMethod  = _type.GetMethod("set_" + mi.Name);
						methodType = _type;
					}

					if (setMethod == null || !IsMethodAccessible(setMethod))
						return;
				}
			}
			//else if (((FieldInfo)mi).IsLiteral)
			//	return;

			var method = nestedType.DefineMethod(_memberAccessor.GetMethodEx("SetValue", typeof(object), typeof(object)));
			var emit   = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.ldarg_2
				.end();

			if (mi is FieldInfo fi)
			{
				emit
					.CastFromObject (fi.FieldType)
					.stfld          (fi)
					;
			}
			else
			{
				Debug.Assert(setMethod != null, nameof(setMethod) + " != null");

				var pi = (PropertyInfo)mi;

				emit
					.CastFromObject (pi.PropertyType)
					.callvirt       (setMethod)
					;
			}

			emit
				.ret()
				;

			nestedType.DefineMethod(_memberAccessor.GetProperty("HasSetter").GetGetMethod()).Emitter
				.ldc_i4_1
				.ret()
				;
		}

		void BuildIsNull(
			MemberInfo        mi,
			TypeBuilderHelper nestedType,
			Type              memberType)
		{
			var methodType  = mi.DeclaringType;
			var getMethod   = null as MethodInfo;
			var isNullable  = memberType.IsNullableType();
			var isValueType = !isNullable && memberType.IsValueType;

			if (!isValueType && mi is PropertyInfo pi)
			{
				getMethod = pi.GetGetMethod();

				if (getMethod == null)
				{
					if (_type != _originalType)
					{
						getMethod  = _type.GetMethod("get_" + pi.Name);
						methodType = _type;
					}

					if (getMethod == null)
						return;
				}
			}

			var methodInfo = _memberAccessor.GetMethod("IsNull");

			if (methodInfo == null)
				return;

			var method = nestedType.DefineMethod(methodInfo);
			var emit   = method.Emitter;

			if (isValueType)
			{
				emit
					.ldc_i4_0
					.end()
					;
			}
			else
			{
				LocalBuilder? locObj = null;

				if (isNullable)
					locObj = method.Emitter.DeclareLocal(memberType);

				emit
					.ldarg_1
					.castType (methodType)
					.end();

				if (mi is FieldInfo fi) emit.ldfld (fi);
				else
				{
					Debug.Assert(getMethod != null, nameof(getMethod) + " != null");
					emit.callvirt(getMethod);
				}

				if (isNullable)
				{
					Debug.Assert(locObj != null, nameof(locObj) + " != null");

					emit
						.stloc  (locObj)
						.ldloca (locObj)
						.call   (memberType, "get_HasValue")
						.ldc_i4_0
						.ceq
						.end();
				}
				else
				{
					emit
						.ldnull
						.ceq
						.end();
				}
			}

			emit
				.ret()
				;
		}

		void BuildTypedGetter(MemberInfo mi, TypeBuilderHelper nestedType, string typedPropertyName)
		{
			var methodType = mi.DeclaringType;
			var getMethod  = null as MethodInfo;

			if (mi is PropertyInfo pi)
			{
				getMethod = pi.GetGetMethod();

				if (getMethod == null)
				{
					if (_type != _originalType)
					{
						getMethod  = _type.GetMethod("get_" + pi.Name);
						methodType = _type;
					}

					if (getMethod == null || !IsMethodAccessible(getMethod))
						return;
				}
			}

			var methodInfo = _memberAccessor.GetMethodEx("Get" + typedPropertyName, typeof(object));

			if (methodInfo == null)
				return;

			var method = nestedType.DefineMethod(methodInfo);
			var emit   = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.end();

			if (mi is FieldInfo fi) emit.ldfld   (fi);
			else
			{
				Debug.Assert(getMethod != null, nameof(getMethod) + " != null");
				emit.callvirt(getMethod);
			}

			emit
				.ret()
				;
		}

		void BuildTypedSetter(
			MemberInfo        mi,
			TypeBuilderHelper nestedType,
			Type              memberType,
			string            typedPropertyName)
		{
			var methodType = mi.DeclaringType;
			var setMethod  = null as MethodInfo;

			if (mi is PropertyInfo propertyInfo)
			{
				setMethod = propertyInfo.GetSetMethod();

				if (setMethod == null)
				{
					if (_type != _originalType)
					{
						setMethod  = _type.GetMethod("set_" + propertyInfo.Name);
						methodType = _type;
					}

					if (setMethod == null || !IsMethodAccessible(setMethod))
						return;
				}
			}

			var methodInfo = _memberAccessor.GetMethodEx("Set" + typedPropertyName, typeof(object), memberType);

			if (methodInfo == null)
				return;

			var method = nestedType.DefineMethod(methodInfo);
			var emit   = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.ldarg_2
				.end();

			if (mi is FieldInfo fieldInfo) emit.stfld(fieldInfo);
			else
			{
				Debug.Assert(setMethod != null, nameof(setMethod) + " != null");
				emit.callvirt(setMethod);
			}

			emit
				.ret()
				;
		}

		void BuildCloneValueMethod(
			MemberInfo        mi,
			TypeBuilderHelper nestedType,
			Type              memberType
			)
		{
			var methodType = mi.DeclaringType;
			var getMethod  = null as MethodInfo;
			var setMethod  = null as MethodInfo;

			if (mi is PropertyInfo)
			{
				getMethod = ((PropertyInfo)mi).GetGetMethod();

				if (getMethod == null)
				{
					if (_type != _originalType)
					{
						getMethod  = _type.GetMethod("get_" + mi.Name);
						methodType = _type;
					}

					if (getMethod == null || !IsMethodAccessible(getMethod))
						return;
				}

				setMethod = ((PropertyInfo)mi).GetSetMethod();

				if (setMethod == null)
				{
					if (_type != _originalType)
					{
						setMethod  = _type.GetMethod("set_" + mi.Name);
						methodType = _type;
					}

					if (setMethod == null || !IsMethodAccessible(setMethod))
						return;
				}
			}

			var method = nestedType.DefineMethod(_memberAccessor.GetMethodEx("CloneValue", typeof(object), typeof(object)));
			var emit   = method.Emitter;

			emit
				.ldarg_2
				.castType (methodType)
				.ldarg_1
				.castType (methodType)
				.end();

			if (mi is FieldInfo fieldInfo)
				emit.ldfld   (fieldInfo);
			else
			{
				Debug.Assert(getMethod != null, nameof(getMethod) + " != null");
				emit.callvirt(getMethod);
			}

			if (typeof(string) != memberType && typeof(ICloneable).IsSameOrParentOf(memberType))
			{
				if (memberType.IsValueType)
					emit
						.box       (memberType)
						.callvirt  (typeof(ICloneable), "Clone")
						.unbox_any (memberType)
						;
				else
				{
					var valueIsNull = emit.DefineLabel();

					emit
						.dup
						.brfalse_s (valueIsNull)
						.callvirt  (typeof(ICloneable), "Clone")
						.castclass (memberType)
						.MarkLabel (valueIsNull)
						;
				}
			}

			if (mi is FieldInfo fi)
				emit.stfld(fi);
			else
			{
				Debug.Assert(setMethod != null, nameof(setMethod) + " != null");
				emit.callvirt(setMethod);
			}

			emit
				.ret()
				;
		}

		void BuildTypedGetterForNullable(MemberInfo mi, TypeBuilderHelper nestedType, Type memberType)
		{
			var methodType = mi.DeclaringType;
			var getMethod  = null as MethodInfo;

			if (mi is PropertyInfo)
			{
				getMethod = ((PropertyInfo)mi).GetGetMethod();

				if (getMethod == null)
				{
					if (_type != _originalType)
					{
						getMethod  = _type.GetMethod("get_" + mi.Name);
						methodType = _type;
					}

					if (getMethod == null || !IsMethodAccessible(getMethod))
						return;
				}
			}

			var setterType = (memberType.IsEnum ? Enum.GetUnderlyingType(memberType) : memberType);
			var methodInfo = _memberAccessor.GetMethodEx("Get" + setterType.Name, typeof(object));

			if (methodInfo == null)
				return;

			var method       = nestedType.DefineMethod(methodInfo);
			var nullableType = typeof(Nullable<>).MakeGenericType(memberType);
			var emit         = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.end();

			if (mi is FieldInfo fieldInfo)
			{
				emit.ldflda(fieldInfo);
			}
			else
			{
				var locNullable = emit.DeclareLocal(nullableType);

				Debug.Assert(getMethod != null, nameof(getMethod) + " != null");

				emit
					.callvirt (getMethod)
					.stloc    (locNullable)
					.ldloca   (locNullable)
					;
			}

			emit
				.call(nullableType, "get_Value")
				.ret()
				;
		}

		void BuildTypedSetterForNullable(
			MemberInfo        mi,
			TypeBuilderHelper nestedType,
			Type              memberType)
		{
			var methodType = mi.DeclaringType;
			var setMethod  = null as MethodInfo;

			if (mi is PropertyInfo)
			{
				setMethod = ((PropertyInfo)mi).GetSetMethod();

				if (setMethod == null)
				{
					if (_type != _originalType)
					{
						setMethod  = _type.GetMethod("set_" + mi.Name);
						methodType = _type;
					}

					if (setMethod == null || !IsMethodAccessible(setMethod))
						return;
				}
			}

			var setterType = (memberType.IsEnum ? Enum.GetUnderlyingType(memberType) : memberType);
			var methodInfo = _memberAccessor.GetMethodEx("Set" + setterType.Name, typeof(object), setterType);

			if (methodInfo == null)
				return;

			var method = nestedType.DefineMethod(methodInfo);
			var emit   = method.Emitter;

			emit
				.ldarg_1
				.castType (methodType)
				.ldarg_2
				.newobj   (typeof(Nullable<>).MakeGenericType(memberType), memberType)
				.end();

			if (mi is FieldInfo fieldInfo)
				emit.stfld(fieldInfo);
			else
			{
				Debug.Assert(setMethod != null, nameof(setMethod) + " != null");
				emit.callvirt(setMethod);
			}

			emit
				.ret()
				;
		}

		static ConstructorBuilderHelper BuildNestedTypeConstructor(TypeBuilderHelper nestedType)
		{
			Type[] parameters = { typeof(TypeAccessor), typeof(MemberInfo) };

			var ctorBuilder = nestedType.DefinePublicConstructor(parameters);

			ctorBuilder.Emitter
				.ldarg_0
				.ldarg_1
				.ldarg_2
				.call    (typeof(MemberAccessor).GetConstructor(parameters))
				.ret()
				;

			return ctorBuilder;
		}

		void BuildObjectFactory()
		{
			var attr = _type.GetCustomAttributeEx(typeof(ObjectFactoryAttribute));

			if (attr != null)
			{
				Debug.Assert(_typeBuilder != null, nameof(_typeBuilder) + " != null");

				_typeBuilder.DefaultConstructor.Emitter
					.ldarg_0
					.LoadType  (_type)
					.LoadType  (typeof(ObjectFactoryAttribute))
					.call      (typeof(TypeExtensions), nameof(TypeExtensions.GetCustomAttributeEx), typeof(Type), typeof(Type))
					.castclass (typeof(ObjectFactoryAttribute))
					.call      (typeof(ObjectFactoryAttribute).GetProperty("ObjectFactory").GetGetMethod())
					.call      (typeof(TypeAccessor).          GetProperty("ObjectFactory").GetSetMethod())
					;
			}
		}
	}
}

#endif
