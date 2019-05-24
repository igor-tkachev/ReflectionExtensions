#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Reflection;
	using Reflection.Emit;

	public abstract class AbstractTypeBuilderBase : IAbstractTypeBuilder
	{
		public virtual Type[] GetInterfaces()
		{
			return new Type[0];
		}

		public  int           ID            { get; set; }
		public  object        TargetElement { get; set; }
		public  BuildContext  Context       { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

		public virtual bool IsCompatible(BuildContext context, IAbstractTypeBuilder typeBuilder)
		{
			return true;
		}

		protected bool IsRelative(IAbstractTypeBuilder typeBuilder)
		{
			if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));

			return GetType().IsInstanceOfType(typeBuilder) || typeBuilder.GetType().IsInstanceOfType(this);
		}

		public virtual bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			return false;
		}

		public virtual int GetPriority(BuildContext context)
		{
			return TypeBuilderConsts.Priority.Normal;
		}

		public virtual void Build(BuildContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));

			switch (context.Step)
			{
				case BuildStep.Begin: BeginMethodBuild(); return;
				case BuildStep.End:   EndMethodBuild();   return;
			}

			switch (context.BuildElement)
			{
				case BuildElement.Type:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildType(); break;
						case BuildStep.Build:          BuildType(); break;
						case BuildStep.After:     AfterBuildType(); break;
						case BuildStep.Catch:     CatchBuildType(); break;
						case BuildStep.Finally: FinallyBuildType(); break;
					}

					break;

				case BuildElement.AbstractGetter:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildAbstractGetter(); break;
						case BuildStep.Build:          BuildAbstractGetter(); break;
						case BuildStep.After:     AfterBuildAbstractGetter(); break;
						case BuildStep.Catch:     CatchBuildAbstractGetter(); break;
						case BuildStep.Finally: FinallyBuildAbstractGetter(); break;
					}

					break;

				case BuildElement.AbstractSetter:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildAbstractSetter(); break;
						case BuildStep.Build:          BuildAbstractSetter(); break;
						case BuildStep.After:     AfterBuildAbstractSetter(); break;
						case BuildStep.Catch:     CatchBuildAbstractSetter(); break;
						case BuildStep.Finally: FinallyBuildAbstractSetter(); break;
					}

					break;

				case BuildElement.AbstractMethod:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildAbstractMethod(); break;
						case BuildStep.Build:          BuildAbstractMethod(); break;
						case BuildStep.After:     AfterBuildAbstractMethod(); break;
						case BuildStep.Catch:     CatchBuildAbstractMethod(); break;
						case BuildStep.Finally: FinallyBuildAbstractMethod(); break;
					}

					break;

				case BuildElement.VirtualGetter:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildVirtualGetter(); break;
						case BuildStep.Build:          BuildVirtualGetter(); break;
						case BuildStep.After:     AfterBuildVirtualGetter(); break;
						case BuildStep.Catch:     CatchBuildVirtualGetter(); break;
						case BuildStep.Finally: FinallyBuildVirtualGetter(); break;
					}

					break;

				case BuildElement.VirtualSetter:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildVirtualSetter(); break;
						case BuildStep.Build:          BuildVirtualSetter(); break;
						case BuildStep.After:     AfterBuildVirtualSetter(); break;
						case BuildStep.Catch:     CatchBuildVirtualSetter(); break;
						case BuildStep.Finally: FinallyBuildVirtualSetter(); break;
					}

					break;

				case BuildElement.VirtualMethod:
					switch (context.Step)
					{
						case BuildStep.Before:   BeforeBuildVirtualMethod(); break;
						case BuildStep.Build:          BuildVirtualMethod(); break;
						case BuildStep.After:     AfterBuildVirtualMethod(); break;
						case BuildStep.Catch:     CatchBuildVirtualMethod(); break;
						case BuildStep.Finally: FinallyBuildVirtualMethod(); break;
					}

					break;

				case BuildElement.InterfaceMethod:
					BuildInterfaceMethod();
					break;
			}
		}

		protected virtual void  BeforeBuildType          () {}
		protected virtual void        BuildType          () {}
		protected virtual void   AfterBuildType          () {}
		protected virtual void   CatchBuildType          () {}
		protected virtual void FinallyBuildType          () {}

		protected virtual void  BeforeBuildAbstractGetter() {}
		protected virtual void        BuildAbstractGetter() {}
		protected virtual void   AfterBuildAbstractGetter() {}
		protected virtual void   CatchBuildAbstractGetter() {}
		protected virtual void FinallyBuildAbstractGetter() {}

		protected virtual void  BeforeBuildAbstractSetter() {}
		protected virtual void        BuildAbstractSetter() {}
		protected virtual void   AfterBuildAbstractSetter() {}
		protected virtual void   CatchBuildAbstractSetter() {}
		protected virtual void FinallyBuildAbstractSetter() {}

		protected virtual void  BeforeBuildAbstractMethod() {}
		protected virtual void        BuildAbstractMethod() {}
		protected virtual void   AfterBuildAbstractMethod() {}
		protected virtual void   CatchBuildAbstractMethod() {}
		protected virtual void FinallyBuildAbstractMethod() {}

		protected virtual void  BeforeBuildVirtualGetter () {}
		protected virtual void        BuildVirtualGetter () {}
		protected virtual void   AfterBuildVirtualGetter () {}
		protected virtual void   CatchBuildVirtualGetter () {}
		protected virtual void FinallyBuildVirtualGetter () {}

		protected virtual void  BeforeBuildVirtualSetter () {}
		protected virtual void        BuildVirtualSetter () {}
		protected virtual void   AfterBuildVirtualSetter () {}
		protected virtual void   CatchBuildVirtualSetter () {}
		protected virtual void FinallyBuildVirtualSetter () {}

		protected virtual void  BeforeBuildVirtualMethod () {}
		protected virtual void        BuildVirtualMethod () {}
		protected virtual void   AfterBuildVirtualMethod () {}
		protected virtual void   CatchBuildVirtualMethod () {}
		protected virtual void FinallyBuildVirtualMethod () {}

		protected virtual void BuildInterfaceMethod      () {}

		protected virtual void BeginMethodBuild          () {}
		protected virtual void   EndMethodBuild          () {}

		#region Helpers

		[SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
		protected bool CallLazyInstanceInsurer(FieldBuilder field)
		{
			if (field == null) throw new ArgumentNullException(nameof(field));

			var ensurer = Context.GetFieldInstanceEnsurer(field.Name);

			if (ensurer != null)
			{
				Context.MethodBuilder.Emitter
					.ldarg_0
					.call    (ensurer);
			}

			return ensurer != null;
		}

		[SuppressMessage("Microsoft.Performance", "CA1818:DoNotConcatenateStringsInsideLoops")]
		protected virtual string GetFieldName(PropertyInfo propertyInfo)
		{
			var name = propertyInfo.Name;

			if (char.IsUpper(name[0]) && name.Length > 1 && char.IsLower(name[1]))
				name = char.ToLower(name[0]) + name.Substring(1, name.Length - 1);

			name = "_" + name;

			foreach (var p in propertyInfo.GetIndexParameters())
				name += "." + p.ParameterType.FullName;//.Replace(".", "_").Replace("+", "_");

			return name;
		}

		protected string GetFieldName()
		{
			return GetFieldName(Context.CurrentProperty);
		}

		protected FieldBuilder GetPropertyInfoField(PropertyInfo property)
		{
			var fieldName = GetFieldName(property) + "_$propertyInfo";
			var field     = Context.GetField(fieldName);

			if (field == null)
			{
				field = Context.CreatePrivateStaticField(fieldName, typeof(PropertyInfo));

				var emit  = Context.TypeBuilder.TypeInitializer.Emitter;
				var index = property.GetIndexParameters();

				emit
					.LoadType (Context.Type)
					.ldstr    (property.Name)
					.LoadType (property.PropertyType)
					;

				if (index.Length == 0)
				{
					emit
						.ldsfld (GetType().GetField(nameof(EmptyTypes)))
						;
				}
				else
				{
					emit
						.ldc_i4 (index.Length)
						.newarr (typeof(Type))
						;

					for (int i = 0; i < index.Length; i++)
						emit
							.dup
							.ldc_i4     (i)
							.LoadType   (index[i].ParameterType)
							.stelem_ref
							.end()
							;
				}

				emit
					.call   (GetType().GetMethodEx(nameof(GetPropertyInfo)))
					.stsfld (field)
					;
			}

			return field;
		}

		static PropertyInfo GetPropertyInfo(Type type, string propertyName, Type returnType, Type[] types)
		{
			return type.GetPropertyEx(
				propertyName,
				BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
				null,
				returnType,
				types,
				null);
		}

		static Type[] EmptyTypes => Type.EmptyTypes;

		protected FieldBuilder GetPropertyInfoField()
		{
			return GetPropertyInfoField(Context.CurrentProperty);
		}

		protected FieldBuilder GetParameterField()
		{
			string       fieldName = GetFieldName() + "_$parameters";
			FieldBuilder field     = Context.GetField(fieldName);

			if (field == null)
			{
				field = Context.CreatePrivateStaticField(fieldName, typeof(object[]));

				FieldBuilder piField = GetPropertyInfoField();
				EmitHelper   emit    = Context.TypeBuilder.TypeInitializer.Emitter;

				emit
					.ldsfld (piField)
					.call   (GetType().GetMethod(nameof(GetPropertyParameters)))
					.stsfld (field)
					;
			}

			return field;
		}

		static object[]? GetPropertyParameters(PropertyInfo propertyInfo)
		{
			if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");

			var attrs = propertyInfo.GetCustomAttributes(typeof(ParameterAttribute), true);

			if (attrs.Length > 0)
				return ((ParameterAttribute)attrs[0]).Parameters;

			attrs = propertyInfo.GetCustomAttributes(typeof(InstanceTypeAttribute), true);

			if (attrs.Length > 0)
				return ((InstanceTypeAttribute)attrs[0]).Parameters;

			var gAttrs = propertyInfo.DeclaringType.GetCustomAttributesEx<GlobalInstanceTypeAttribute>(true);

			foreach (GlobalInstanceTypeAttribute attr in gAttrs)
				if (attr.PropertyType.IsSameOrParentOf(propertyInfo.PropertyType))
					return attr.Parameters;

			return null;
		}

		protected FieldBuilder GetTypeAccessorField()
		{
			var fieldName = "_" + GetObjectType().FullName + "_$typeAccessor";
			var field     = Context.GetField(fieldName);

			if (field == null)
			{
				field = Context.CreatePrivateStaticField(fieldName, typeof(TypeAccessor));

				EmitHelper emit = Context.TypeBuilder.TypeInitializer.Emitter;

				emit
					.LoadType (GetObjectType())
					.call     (typeof(TypeAccessor), "GetAccessor", typeof(Type))
					.stsfld   (field)
					;
			}

			return field;
		}

		protected FieldBuilder GetArrayInitializer(Type arrayType)
		{
			var fieldName = $"_array_of_$_{arrayType.FullName}";
			var field     = Context.GetField(fieldName);

			if (field == null)
			{
				field = Context.CreatePrivateStaticField(fieldName, arrayType);

				var emit = Context.TypeBuilder.TypeInitializer.Emitter;
				var rank = arrayType.GetArrayRank();

				if (rank > 1)
				{
					var parameters = new Type[rank];

					for (var i = 0; i < parameters.Length; i++)
					{
						parameters[i] = typeof(int);
						emit.ldc_i4_0.end();
					}

					var ci = arrayType.GetConstructorEx(parameters);

					emit
						.newobj (ci)
						.stsfld (field)
						;
				}
				else
				{
					emit
						.ldc_i4_0
						.newarr   (arrayType.GetElementType())
						.stsfld   (field)
						;
				}
			}

			return field;
		}

		protected FieldBuilder GetArrayInitializer()
		{
			return GetArrayInitializer(Context.CurrentProperty.PropertyType);
		}

		protected virtual Type GetFieldType()
		{
			var pi    = Context.CurrentProperty;
			var index = pi.GetIndexParameters();

			switch (index.Length)
			{
				case 0: return pi.PropertyType;
				case 1: return typeof(Dictionary<object,object>);
				default:
					throw new InvalidOperationException();
			}
		}

		protected virtual Type GetObjectType()
		{
			return GetFieldType();
		}

		protected virtual bool IsObjectHolder => false;

		#endregion
	}
}

#endif
