#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Patterns;
	using Reflection.Emit;

	class DuckTypeBuilder : ITypeBuilder
	{
		public DuckTypeBuilder(MustImplementAttribute defaultAttribute, Type interfaceType, Type[] objectTypes)
		{
			_interfaceType    = interfaceType;
			_objectTypes      = objectTypes;
			_defaultAttribute = defaultAttribute;
		}

		readonly Type                   _interfaceType;
		readonly Type[]                 _objectTypes;
		         TypeBuilderHelper?     _typeBuilder;
		readonly MustImplementAttribute _defaultAttribute;

		#region ITypeBuilder Members

		public string AssemblyNameSuffix => "DuckType." + AbstractClassBuilder.GetTypeFullName(_interfaceType).Replace('+', '.');

		public Type? Build(AssemblyBuilderHelper assemblyBuilder)
		{
			_typeBuilder = assemblyBuilder.DefineType(GetTypeName(), typeof(DuckType), _interfaceType);

			if (!BuildMembers(_interfaceType))
				return null;

			foreach (Type t in _interfaceType.GetInterfaces())
				if (!BuildMembers(t))
					return null;

			return _typeBuilder.Create();
		}

		public string GetTypeName()
		{
			var name = string.Empty;

			foreach (var t in _objectTypes)
			{
				if (t != null)
					name += AbstractClassBuilder.GetTypeFullName(t).Replace('+', '.');
				name += "$";
			}

			return name + AssemblyNameSuffix;
		}

		public Type GetBuildingType()
		{
			return _interfaceType;
		}

		#endregion

		static bool CompareMethodSignature(MethodInfo m1, MethodInfo m2)
		{
			if (m1 == m2)
				return true;

			if (m1.Name != m2.Name)
				return false;

			if (m1.ReturnType != m2.ReturnType)
				return false;

			var ps1 = m1.GetParameters();
			var ps2 = m2.GetParameters();

			if (ps1.Length != ps2.Length)
				return false;

			for (int i = 0; i < ps1.Length; i++)
			{
				var p1 = ps1[i];
				var p2 = ps2[i];

				if (p1.ParameterType != p2.ParameterType || p1.IsIn != p2.IsIn || p1.IsOut != p2.IsOut)
					return false;
			}

			return true;
		}

		bool BuildMembers(Type interfaceType)
		{
			var objectsField = typeof(DuckType).GetField("_objects", BindingFlags.NonPublic | BindingFlags.Instance);
			var flags        = BindingFlags.Public | BindingFlags.Instance |
				(DuckTyping.AllowStaticMembers ? BindingFlags.Static | BindingFlags.FlattenHierarchy : 0);

			foreach (var interfaceMethod in interfaceType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
			{
				MethodInfo? targetMethod = null;
				var         typeIndex    = 0;

				for (; typeIndex < _objectTypes.Length; typeIndex++)
				{
					if (_objectTypes[typeIndex] == null)
						continue;

					foreach (var mi in _objectTypes[typeIndex].GetMethods(flags))
					{
						if (CompareMethodSignature(interfaceMethod, mi))
						{
							targetMethod = mi;
							break;
						}
					}

					if (targetMethod == null)
					{
						foreach (var intf in _objectTypes[typeIndex].GetInterfaces())
						{
							if (intf.IsPublicEx() || intf.IsNestedPublicEx())
							{
								foreach (var mi in intf.GetMethods(flags))
								{
									if (CompareMethodSignature(interfaceMethod, mi))
									{
										targetMethod = mi;
										break;
									}
								}

								if (targetMethod != null)
									break;
							}
						}
					}

					if (targetMethod != null)
						break;
				}

				var ips     = interfaceMethod.GetParameters();
				var builder = _typeBuilder.DefineMethod(interfaceMethod);
				var emit    = builder.Emitter;

				if (targetMethod != null)
				{
					var targetType = targetMethod.DeclaringType;

					if (!targetMethod.IsStatic)
					{
						emit
							.ldarg_0
							.ldfld         (objectsField)
							.ldc_i4        (typeIndex)
							.ldelem_ref
							.end()
							;

						if (targetType.IsValueType)
						{
							// For value types we have to use stack.
							//
							var obj = emit.DeclareLocal(targetType);

							emit
								.unbox_any (targetType)
								.stloc     (obj)
								.ldloca    (obj)
								;
						}
						else
							emit
								.castclass (targetType)
								;
					}

					foreach (var p in ips)
						emit.ldarg(p);

					if (targetMethod.IsStatic || targetMethod.IsFinal || targetMethod.DeclaringType.IsSealed)
						emit
							.call     (targetMethod)
							.ret();
					else
						emit
							.callvirt (targetMethod)
							.ret();
				}
				else
				{
					// Method or property was not found.
					// Insert an empty stub or stub that throws the NotImplementedException.
					//
					var attr = (MustImplementAttribute)
						Attribute.GetCustomAttribute(interfaceMethod, typeof(MustImplementAttribute));

					if (attr == null)
					{
						attr = (MustImplementAttribute)Attribute.GetCustomAttribute(
							interfaceMethod.DeclaringType, typeof (MustImplementAttribute));
						if (attr == null)
							attr = _defaultAttribute;
					}

					// When the member is marked as 'Required' throw a build-time exception.
					//
					if (attr.Implement)
					{
						if (attr.ThrowException)
						{
							throw new TypeBuilderException(string.Format(
								"Type '{0}' must implement required public method '{1}'",
								_objectTypes.Length > 0 && _objectTypes[0] != null ? _objectTypes[0].FullName : "???",
								interfaceMethod));
						}
						else
						{
							// Implement == true, but ThrowException == false.
							// In this case the null pointer will be returned.
							// This mimics the 'as' operator behaviour.
							//
							return false;
						}
					}

					if (attr.ThrowException)
					{
						var message = attr.ExceptionMessage;

						if (message == null)
						{
							message = string.Format("Type '{0}' does not implement public method '{1}'",
								_objectTypes.Length > 0 && _objectTypes[0] != null ? _objectTypes[0].FullName : "???",
								interfaceMethod);
						}

						emit
							.ldstr  (message)
							.newobj (typeof(InvalidOperationException), typeof(string))
							.@throw
							.end();
					}
					else
					{
						// Emit a 'do nothing' stub.
						//
						LocalBuilder? returnValue = null;

						if (interfaceMethod.ReturnType != typeof(void))
						{
							returnValue = emit.DeclareLocal(interfaceMethod.ReturnType);
							emit.Init(returnValue);
						}

						// Initialize out parameters.
						//
						var parameters = ips;

						if (parameters != null)
							emit.InitOutParameters(parameters);

						if (returnValue != null)
							emit.ldloc(returnValue);

						emit.ret();
					}
				}
			}

			return true;
		}
	}
}

#endif
