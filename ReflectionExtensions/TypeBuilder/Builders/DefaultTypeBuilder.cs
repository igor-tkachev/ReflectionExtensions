

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Reflection;
	using Reflection.Emit;

	public class DefaultTypeBuilder : AbstractTypeBuilderBase
	{
		#region Interface Overrides

		public override bool IsCompatible(BuildContext context, IAbstractTypeBuilder typeBuilder)
		{
			return IsRelative(typeBuilder) == false;
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (context.IsAbstractProperty && context.IsBeforeOrBuildStep)
			{
				return context.CurrentProperty?.GetIndexParameters().Length <= 1;
			}

			return context.BuildElement == BuildElement.Type && context.IsAfterStep;
		}

		#endregion

		#region Get/Set Implementation

		protected override void BuildAbstractGetter()
		{
			Debug.Assert(Context.ReturnValue     != null, "Context.ReturnValue != null");
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var field = GetField();
			var index = Context.CurrentProperty.GetIndexParameters();

			switch (index.Length)
			{
				case 0:
					Context.MethodBuilder.Emitter
						.ldarg_0
						.ldfld   (field)
						.stloc   (Context.ReturnValue)
						;
					break;

				case 1:
					Context.MethodBuilder.Emitter
						.ldarg_0
						.ldfld          (field)
						.ldarg_1
						.boxIfValueType (index[0].ParameterType)
						.callvirt       (typeof(Dictionary<object,object>), "get_Item", typeof(object))
						.castType       (Context.CurrentProperty.PropertyType)
						.stloc          (Context.ReturnValue)
						;
					break;
			}
		}

		protected override void BuildAbstractSetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var field = GetField();
			var index = Context.CurrentProperty.GetIndexParameters();

			switch (index.Length)
			{
				case 0:
					Context.MethodBuilder.Emitter
						.ldarg_0
						.ldarg_1
						.stfld   (field)
						;
					//Context.MethodBuilder.Emitter.AddMaxStackSize(6);
					break;

				case 1:
					Context.MethodBuilder.Emitter
						.ldarg_0
						.ldfld          (field)
						.ldarg_1
						.boxIfValueType (index[0].ParameterType)
						.ldarg_2
						.boxIfValueType (Context.CurrentProperty.PropertyType)
						.callvirt       (typeof(Dictionary<object,object>), "set_Item", typeof(object), typeof(object))
					;
					break;
			}
		}

		#endregion

		#region Call Base Method

		protected override void BuildVirtualGetter()
		{
			CallBaseMethod();
		}

		protected override void BuildVirtualSetter()
		{
			CallBaseMethod();
		}

		protected override void BuildVirtualMethod()
		{
			CallBaseMethod();
		}

		void CallBaseMethod()
		{
			var emit   = Context.MethodBuilder.Emitter;
			var method = Context.MethodBuilder.OverriddenMethod;
			var ps     = method.GetParameters();

			emit.ldarg_0.end();

			for (var i = 0; i < ps.Length; i++)
				emit.ldarg(i + 1);

			emit.call(method);

			if (Context.ReturnValue != null)
				emit.stloc(Context.ReturnValue);
		}

		#endregion

		#region Properties

		private   static Type _initContextType;
		protected static Type  InitContextType => _initContextType ??= typeof(InitContext);

		#endregion

		#region Field Initialization

		#region Overrides

		protected override void BeforeBuildAbstractGetter()
		{
			CallLazyInstanceInsurer(GetField());
		}

		protected override void BeforeBuildAbstractSetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var field = GetField();

			if (field.FieldType != Context.CurrentProperty.PropertyType)
				CallLazyInstanceInsurer(field);
		}

		#endregion

		#region Common

		protected FieldBuilder GetField()
		{
			var propertyInfo = Context.CurrentProperty;
			var fieldName    = GetFieldName();
			var fieldType    = GetFieldType();
			var field        = Context.GetField(fieldName);

			if (field == null)
			{
				Debug.Assert(propertyInfo != null, nameof(propertyInfo) + " != null");

				field = Context.CreatePrivateField(propertyInfo, fieldName, fieldType);

				if (TypeAccessor.IsInstanceBuildable(fieldType))
				{
					var noInstance = propertyInfo.GetCustomAttributes(typeof(NoInstanceAttribute), true).Length > 0;

					if (IsObjectHolder && noInstance)
					{
						BuildHolderInstance(Context.TypeBuilder.DefaultConstructor.Emitter);
						BuildHolderInstance(Context.TypeBuilder.InitConstructor.Emitter);
					}
					else if (!noInstance)
					{
						if (fieldType.IsClass && IsLazyInstance(fieldType))
						{
							BuildLazyInstanceEnsurer();
						}
						else
						{
							BuildDefaultInstance();
							BuildInitContextInstance();
						}
					}
				}
			}

			return field;
		}

		#endregion

		#region Build

		void BuildHolderInstance(EmitHelper emit)
		{
			var fieldName  = GetFieldName();
			var field      = Context.GetField(fieldName);
			var fieldType  = field.FieldType;
			var objectType = GetObjectType();

			var ci = fieldType.GetDefaultConstructor();

			if (ci != null)
			{
				emit
					.ldarg_0
					.newobj (ci)
					.stfld  (field)
					;
			}
			else
			{
				if (!CheckObjectHolderCtor(fieldType, objectType))
					return;

				emit
					.ldarg_0
					.ldnull
					.newobj (fieldType, objectType)
					.stfld  (field)
					;
			}
		}

		void CreateDefaultInstance(FieldBuilder field, Type fieldType, Type objectType, EmitHelper emit)
		{
			if (!CheckObjectHolderCtor(fieldType, objectType))
				return;

			if (objectType == typeof(string))
			{
				emit
					.ldarg_0
					.LoadInitValue (objectType)
					;
			}
			else if (objectType.IsArray)
			{
				var initializer = GetArrayInitializer(objectType);

				emit
					.ldarg_0
					.ldsfld  (initializer)
					;
			}
			else
			{
				var ci = objectType.GetDefaultConstructor();

				if (ci == null)
				{
					if (objectType.IsValueTypeEx())
						return;

					Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

					var message = string.Format(
						"Could not build the '{0}' property of the '{1}' type: type '{2}' has to have public default constructor.",
						Context.CurrentProperty.Name,
						Context.Type.FullName,
						objectType.FullName);

					emit
						.ldstr  (message)
						.newobj (typeof(TypeBuilderException), typeof(string))
						.@throw
						.end()
						;

					return;
				}

				emit
					.ldarg_0
					.newobj  (ci)
					;
			}

			if (IsObjectHolder)
			{
				emit
					.newobj (fieldType, objectType)
					;
			}

			emit
				.stfld (field)
				;
		}

		void CreateParametrizedInstance(
			FieldBuilder field, Type fieldType, Type objectType, EmitHelper emit, object?[] parameters)
		{
			if (!CheckObjectHolderCtor(fieldType, objectType))
				return;

			Stack<ConstructorInfo>? genericNestedConstructors;

			if (parameters.Length == 1)
			{
				var o = parameters[0];

				if (o == null)
				{
					if (objectType.IsValueType == false)
						emit
							.ldarg_0
							.ldnull
							.end()
							;

					if (IsObjectHolder)
					{
						emit
							.newobj (fieldType, objectType)
							;
					}
					else
					{
						if (objectType.IsGenericTypeEx())
						{
							Type? nullableType = null;

							genericNestedConstructors = GetGenericNestedConstructors(
								objectType,
								type =>
									type.IsValueType == false ||
									(type.IsGenericTypeEx() && type.GetGenericTypeDefinitionEx() == typeof (Nullable<>)),
								typeHelper => nullableType = typeHelper,
								() => nullableType != null);

							if (nullableType == null)
								throw new Exception("Cannot find nullable type in generic types chain");

							if (nullableType.IsValueType == false)
							{
								emit
									.ldarg_0
									.ldnull
									.end()
									;
							}
							else
							{
								var nullable = emit.DeclareLocal(nullableType);

								emit
									.ldloca(nullable)
									.initobj(nullableType)
									.ldarg_0
									.ldloc(nullable)
									;

								if (genericNestedConstructors != null)
								{
									while (genericNestedConstructors.Count != 0)
									{
										emit
											.newobj(genericNestedConstructors.Pop())
											;
									}
								}
							}
						}
					}

					emit
						.stfld (field)
						;

					return;
				}

				if (objectType == o.GetType())
				{
					if (objectType == typeof(string))
					{
						emit
							.ldarg_0
							.ldstr   ((string)o)
							.stfld   (field)
							;

						return;
					}

					if (objectType.IsValueType)
					{
						emit.ldarg_0.end();

						if (emit.LoadWellKnownValue(o) == false)
						{
							emit
								.ldsfld     (GetParameterField())
								.ldc_i4_0
								.ldelem_ref
								.end()
								;
						}

						emit.stfld(field);

						return;
					}
				}
			}

			var types = new Type[parameters.Length];

			for (var i = 0; i < parameters.Length; i++)
			{
				var p = parameters[i];

				if (p != null)
				{
					var t = p.GetType();

					types[i] = (t.IsEnum) ? Enum.GetUnderlyingType(t) : t;
				}
				else
					types[i] = typeof(object);
			}

			// Do some heuristics for Nullable<DateTime> and EditableValue<Decimal>
			//
			ConstructorInfo? objectCtor = null;

			genericNestedConstructors = GetGenericNestedConstructors(
				objectType,
				type => true,
				type => objectCtor = type.GetConstructorEx(types),
				() => objectCtor != null);

			if (objectCtor == null)
			{
				if (objectType.IsValueType)
					return;

				throw new TypeBuilderException(
					string.Format(types.Length == 0?
							"Could not build the '{0}' property of the '{1}' type: type '{2}' has to have public default constructor." :
							"Could not build the '{0}' property of the '{1}' type: type '{2}' has to have public constructor.",
						Context.CurrentProperty?.Name,
						Context.Type.FullName,
						objectType.FullName));
			}

			var pi = objectCtor.GetParameters();

			emit.ldarg_0.end();

			for (var i = 0; i < parameters.Length; i++)
			{
				var o     = parameters[i];

				Debug.Assert(o != null, nameof(o) + " != null");

				var oType = o.GetType();

				if (emit.LoadWellKnownValue(o))
				{
					if (oType.IsValueType)
					{
						if (!pi[i].ParameterType.IsValueType)
							emit.box(oType);
						else if (Type.GetTypeCode(oType) != Type.GetTypeCode(pi[i].ParameterType))
							emit.conv(pi[i].ParameterType);
					}
				}
				else
				{
					emit
						.ldsfld         (GetParameterField())
						.ldc_i4         (i)
						.ldelem_ref
						.CastFromObject (types[i])
						;

					if (oType.IsValueType && !pi[i].ParameterType.IsValueType)
						emit.box(oType);
				}
			}

			emit
				.newobj (objectCtor)
				;

			if (genericNestedConstructors != null)
			{
				while (genericNestedConstructors.Count != 0)
				{
					emit
						.newobj(genericNestedConstructors.Pop())
						;
				}
			}

			if (IsObjectHolder)
			{
				emit
					.newobj (fieldType, objectType)
					;
			}

			emit
				.stfld  (field)
				;
		}

		Stack<ConstructorInfo>? GetGenericNestedConstructors(Type objectType,
			Predicate<Type> isActionable,
			Action<Type>    action,
			Func<bool>      isBreakCondition)
		{
			Stack<ConstructorInfo>? genericNestedConstructors = null;

			if (isActionable(objectType))
				action(objectType);

			while (objectType.IsGenericTypeEx() && !isBreakCondition())
			{
				var typeArgs = objectType.GetGenericArgumentsEx();

				if (typeArgs.Length == 1)
				{
					var genericCtor = objectType.GetConstructorEx(typeArgs[0]);

					if (genericCtor != null)
					{
						if (genericNestedConstructors == null)
							genericNestedConstructors = new Stack<ConstructorInfo>();

						genericNestedConstructors.Push(genericCtor);
						objectType = typeArgs[0];

						if (isActionable(objectType))
							action(objectType);
					}
					else
					{
						break;
					}
				}
				else
				{
					throw new TypeBuilderException(
						string.Format("Could not build the '{0}' property of the '{1}' type: generic type '{2}' and it's generic parameter types should have only one parameter type.",
							Context.CurrentProperty?.Name,
							Context.Type.FullName,
							objectType.FullName));
				}
			}

			return genericNestedConstructors;
		}

		#endregion

		#region Build InitContext Instance

		void BuildInitContextInstance()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var fieldName  = GetFieldName();
			var field      = Context.GetField(fieldName);
			var fieldType  = field.FieldType;
			var objectType = GetObjectType();

			var emit = Context.TypeBuilder.InitConstructor.Emitter;

			var parameters = GetPropertyParameters(Context.CurrentProperty);
			var ci = objectType.GetConstructorEx(typeof(InitContext));

			if (ci != null && ci.GetParameters()[0].ParameterType != typeof(InitContext))
				ci = null;

			if (ci != null || objectType.IsAbstract)
				CreateAbstractInitContextInstance(field, fieldType, objectType, emit, parameters);
			else if (parameters == null)
				CreateDefaultInstance(field, fieldType, objectType, emit);
			else
				CreateParametrizedInstance(field, fieldType, objectType, emit, parameters);
		}

		public new static object?[]? GetPropertyParameters(PropertyInfo propertyInfo)
		{
			var attrs = propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true);

			if (attrs.Length > 0)
				return ((ParameterAttribute)attrs[0]).Parameters;

			attrs = propertyInfo.GetCustomAttributes(typeof(InstanceTypeAttribute), true);

			if (attrs.Length > 0)
				return ((InstanceTypeAttribute)attrs[0]).Parameters;

			attrs = propertyInfo.DeclaringType.GetCustomAttributesEx<GlobalInstanceTypeAttribute>(true);

			foreach (GlobalInstanceTypeAttribute attr in attrs)
				if (attr.PropertyType.IsSameOrParentOf(propertyInfo.PropertyType))
//				if (attr.PropertyType == propertyInfo.PropertyType)
					return attr.Parameters;

			return null;
		}

		void CreateAbstractInitContextInstance(FieldBuilder field, Type fieldType, Type objectType, EmitHelper emit, object?[]? parameters)
		{
			if (!CheckObjectHolderCtor(fieldType, objectType))
				return;

			var memberParams = InitContextType.GetProperty("MemberParameters").GetSetMethod();
			var parentField  = Context.GetItem<LocalBuilder>("$BLToolkit.InitContext.Parent");

			if (parentField == null)
			{
				Context.Items.Add("$BLToolkit.InitContext.Parent", parentField = emit.DeclareLocal(typeof(object)));

				var label = emit.DefineLabel();
				var ctor  = InitContextType.GetConstructorEx();

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.ldarg_1
					.brtrue_s  (label)

					.newobj    (ctor)
					.starg     (1)

					.ldarg_1
					.ldc_i4_1
					.callvirt  (InitContextType.GetProperty("IsInternal").GetSetMethod())

					.MarkLabel (label)

					.ldarg_1
					.callvirt  (InitContextType.GetProperty("Parent").GetGetMethod())
					.stloc     (parentField)
					;
			}

			emit
				.ldarg_1
				.ldarg_0
				.callvirt (InitContextType.GetProperty("Parent").GetSetMethod())
				;

			var isDirty = Context.GetItem<bool?>("$BLToolkit.InitContext.DirtyParameters");

			if (parameters != null)
			{
				emit
					.ldarg_1
					.ldsfld   (GetParameterField())
					.callvirt (memberParams)
					;
			}
			else if (isDirty != null && (bool)isDirty)
			{
				emit
					.ldarg_1
					.ldnull
					.callvirt (memberParams)
					;
			}

			if (Context.Items.ContainsKey("$BLToolkit.InitContext.DirtyParameters"))
				Context.Items["$BLToolkit.InitContext.DirtyParameters"] = (bool?)(parameters != null);
			else
				Context.Items.Add("$BLToolkit.InitContext.DirtyParameters", (bool?)(parameters != null));

			if (objectType.IsAbstract)
			{
				emit
					.ldarg_0
					.ldsfld             (GetTypeAccessorField())
					.ldarg_1
					.callvirtNoGenerics (typeof(TypeAccessor), "CreateInstanceEx", _initContextType)
					.isinst             (objectType)
					;
			}
			else
			{
				var ctor = objectType.GetConstructorEx(typeof(InitContext));

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.ldarg_0
					.ldarg_1
					.newobj  (ctor)
					;
			}

			if (IsObjectHolder)
			{
				emit
					.newobj (fieldType, objectType)
					;
			}

			emit
				.stfld (field)
				;
		}

		#endregion

		#region Build Default Instance

		void BuildDefaultInstance()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var fieldName  = GetFieldName();
			var field      = Context.GetField(fieldName);
			var fieldType  = field.FieldType;
			var objectType = GetObjectType();

			var emit = Context.TypeBuilder.DefaultConstructor.Emitter;
			var ps   = GetPropertyParameters(Context.CurrentProperty);
			var ci   = objectType.GetConstructorEx(typeof(InitContext));

			if (ci != null && ci.GetParameters()[0].ParameterType != typeof(InitContext))
				ci = null;

			if (ci != null || objectType.IsAbstract)
				CreateInitContextDefaultInstance(
					"$BLToolkit.DefaultInitContext.", field, fieldType, objectType, emit, ps);
			else if (ps == null)
				CreateDefaultInstance(field, fieldType, objectType, emit);
			else
				CreateParametrizedInstance(field, fieldType, objectType, emit, ps);
		}

		bool CheckObjectHolderCtor(Type fieldType, Type objectType)
		{
			if (IsObjectHolder)
			{
				var holderCi = fieldType.GetConstructorEx(objectType);

				if (holderCi == null)
				{
					var message = string.Format(
						"Could not build the '{0}' property of the '{1}' type: type '{2}' has to have constructor taking type '{3}'.",
						Context.CurrentProperty?.Name,
						Context.Type.FullName,
						fieldType.FullName,
						objectType.FullName);

					Context.TypeBuilder.DefaultConstructor.Emitter
						.ldstr  (message)
						.newobj (typeof(TypeBuilderException), typeof(string))
						.@throw
						.end()
						;

					return false;
				}
			}

			return true;
		}

		void CreateInitContextDefaultInstance(
			string       initContextName,
			FieldBuilder field,
			Type         fieldType,
			Type         objectType,
			EmitHelper   emit,
			object?[]?   parameters)
		{
			if (!CheckObjectHolderCtor(fieldType, objectType))
				return;

			var initField    = GetInitContextBuilder(initContextName, emit);
			var memberParams = InitContextType.GetProperty(nameof(InitContext.MemberParameters)).GetSetMethod();

			if (parameters != null)
			{
				emit
					.ldloc    (initField)
					.ldsfld   (GetParameterField())
					.callvirt (memberParams)
					;
			}
			else if ((bool)Context.Items["$BLToolkit.Default.DirtyParameters"])
			{
				emit
					.ldloc    (initField)
					.ldnull
					.callvirt (memberParams)
					;
			}

			Context.Items["$BLToolkit.Default.DirtyParameters"] = parameters != null;

			if (objectType.IsAbstract)
			{
				emit
					.ldarg_0
					.ldsfld             (GetTypeAccessorField())
					.ldloc              (initField)
					.callvirtNoGenerics (typeof(TypeAccessor), "CreateInstanceEx", _initContextType)
					.isinst             (objectType)
					;
			}
			else
			{
				var ctor = objectType.GetConstructorEx(typeof(InitContext));

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.ldarg_0
					.ldloc   (initField)
					.newobj  (ctor)
					;
			}

			if (IsObjectHolder)
			{
				emit
					.newobj (fieldType, objectType)
					;
			}

			emit
				.stfld (field)
				;
		}

		LocalBuilder GetInitContextBuilder(
			string initContextName, EmitHelper emit)
		{
			var initField = Context.GetItem<LocalBuilder>(initContextName);

			if (initField == null)
			{
				Context.Items.Add(initContextName, initField = emit.DeclareLocal(InitContextType));

				var ctor = InitContextType.GetConstructorEx();

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.newobj   (ctor)

					.dup
					.ldarg_0
					.callvirt (InitContextType.GetProperty(nameof(InitContext.Parent)).GetSetMethod())

					.dup
					.ldc_i4_1
					.callvirt (InitContextType.GetProperty(nameof(InitContext.IsInternal)).GetSetMethod())

					.stloc    (initField)
					;

				Context.Items.Add("$BLToolkit.Default.DirtyParameters", false);
			}

			return initField;
		}

		#endregion

		#region Build Lazy Instance

		bool IsLazyInstance(Type type)
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var attrs = Context.CurrentProperty.GetCustomAttributes(typeof(LazyInstanceAttribute), true);

			if (attrs.Length > 0)
				return ((LazyInstanceAttribute)attrs[0]).IsLazy;

			attrs = Context.Type.GetCustomAttributesEx<LazyInstancesAttribute>(true);

			foreach (LazyInstancesAttribute a in attrs)
				if (a.Type == typeof(object) || type == a.Type || type.IsSubclassOf(a.Type))
					return a.IsLazy;

			return false;
		}

		void BuildLazyInstanceEnsurer()
		{
			var fieldName  = GetFieldName();
			var field      = Context.GetField(fieldName);
			var fieldType  = field.FieldType;
			var objectType = GetObjectType();
			var ensurer    = Context.TypeBuilder.DefineMethod($"$EnsureInstance{fieldName}", MethodAttributes.Private | MethodAttributes.HideBySig);

			var emit = ensurer.Emitter;
			var end  = emit.DefineLabel();

			emit
				.ldarg_0
				.ldfld    (field)
				.brtrue_s (end)
				;

			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var parameters = GetPropertyParameters(Context.CurrentProperty);
			var ci         = objectType.GetConstructorEx(typeof(InitContext));

			if (ci != null || objectType.IsAbstract)
				CreateInitContextLazyInstance(field, fieldType, objectType, emit, parameters);
			else if (parameters == null)
				CreateDefaultInstance(field, fieldType, objectType, emit);
			else
				CreateParametrizedInstance(field, fieldType, objectType, emit, parameters);

			emit
				.MarkLabel(end)
				.ret()
				;

			Context.Items.Add("$BLToolkit.FieldInstanceEnsurer." + fieldName, ensurer);
		}

		void CreateInitContextLazyInstance(
			FieldBuilder field,
			Type         fieldType,
			Type         objectType,
			EmitHelper   emit,
			object?[]?   parameters)
		{
			if (!CheckObjectHolderCtor(fieldType, objectType))
				return;

			var initField = emit.DeclareLocal(InitContextType);
			var ctor      = InitContextType.GetConstructorEx();

			Debug.Assert(ctor != null, nameof(ctor) + " != null");

			emit
				.newobj   (ctor)

				.dup
				.ldarg_0
				.callvirt (InitContextType.GetProperty("Parent").GetSetMethod())

				.dup
				.ldc_i4_1
				.callvirt (InitContextType.GetProperty("IsInternal").GetSetMethod())

				.dup
				.ldc_i4_1
				.callvirt (InitContextType.GetProperty("IsLazyInstance").GetSetMethod())

				;

			if (parameters != null)
			{
				emit
					.dup
					.ldsfld   (GetParameterField())
					.callvirt (InitContextType.GetProperty("MemberParameters").GetSetMethod())
					;
			}

			emit
				.stloc    (initField);

			if (objectType.IsAbstractEx())
			{
				emit
					.ldarg_0
					.ldsfld             (GetTypeAccessorField())
					.ldloc              (initField)
					.callvirtNoGenerics (typeof(TypeAccessor), "CreateInstanceEx", _initContextType)
					.isinst             (objectType)
					;
			}
			else
			{
				ctor = objectType.GetConstructorEx(typeof(InitContext));

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.ldarg_0
					.ldloc   (initField)
					.newobj  (ctor)
					;
			}

			if (IsObjectHolder)
			{
				emit
					.newobj (fieldType, objectType)
					;
			}

			emit
				.stfld (field)
				;
		}

		#endregion

		#region Finalize Type

		protected override void AfterBuildType()
		{
			var isDirty = Context.GetItem<bool?>("$BLToolkit.InitContext.DirtyParameters");

			if (isDirty != null && isDirty.Value)
			{
				Context.TypeBuilder.InitConstructor.Emitter
					.ldarg_1
					.ldnull
					.callvirt (InitContextType.GetProperty("MemberParameters").GetSetMethod())
					;
			}

			var localBuilder = Context.GetItem<LocalBuilder>("$BLToolkit.InitContext.Parent");

			if (localBuilder != null)
			{
				Context.TypeBuilder.InitConstructor.Emitter
					.ldarg_1
					.ldloc    (localBuilder)
					.callvirt (InitContextType.GetProperty("Parent").GetSetMethod())
					;
			}

			FinalizeDefaultConstructors();
			FinalizeInitContextConstructors();
		}

		void FinalizeDefaultConstructors()
		{
			var ci = Context.Type.GetDefaultConstructor();

			if (ci == null || Context.TypeBuilder.IsDefaultConstructorDefined)
			{
				var emit = Context.TypeBuilder.DefaultConstructor.Emitter;

				if (ci != null)
				{
					emit.ldarg_0.call(ci);
				}
				else
				{
					ci = Context.Type.GetConstructorEx(typeof(InitContext));

					if (ci != null)
					{
						var initField = GetInitContextBuilder("$BLToolkit.DefaultInitContext.", emit);

						emit
							.ldarg_0
							.ldloc   (initField)
							.call    (ci);
					}
					else
					{
						if (Context.Type.GetConstructors().Length > 0)
							throw new TypeBuilderException($"Could not build the '{Context.Type.FullName}' type: default constructor not found.");
					}
				}
			}
		}

		void FinalizeInitContextConstructors()
		{
			var ci = Context.Type.GetConstructorEx(typeof(InitContext));

			if (ci != null || Context.TypeBuilder.IsInitConstructorDefined)
			{
				var emit = Context.TypeBuilder.InitConstructor.Emitter;

				if (ci != null)
				{
					emit
						.ldarg_0
						.ldarg_1
						.call    (ci);
				}
				else
				{
					ci = Context.Type.GetDefaultConstructor();

					if (ci != null)
					{
						emit.ldarg_0.call(ci);
					}
					else
					{
						if (Context.Type.GetConstructors().Length > 0)
							throw new TypeBuilderException($"Could not build the '{Context.Type.FullName}' type: default constructor not found.");
					}
				}
			}
		}

		#endregion

		#endregion
	}
}

#endif
