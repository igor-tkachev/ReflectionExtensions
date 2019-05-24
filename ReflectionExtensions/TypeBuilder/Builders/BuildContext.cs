#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;

using JetBrains.Annotations;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Reflection.Emit;

	[PublicAPI]
	[DebuggerStepThrough]
	public class BuildContext
	{
		public BuildContext(Type type)
		{
			Type  = type;
			Items = new Dictionary<object,object>(10);
		}

		public Type                      Type            { get; }
		public AssemblyBuilderHelper?    AssemblyBuilder { get; set; }
		public Dictionary<object,object> Items           { get; }

		private Dictionary<PropertyInfo,FieldBuilder>?   _fields;
		public  Dictionary<PropertyInfo,FieldBuilder>     Fields => _fields ??= new Dictionary<PropertyInfo, FieldBuilder>(10);

		private IDictionary<Type,IAbstractTypeBuilder?>? _interfaceMap;
		public  IDictionary<Type,IAbstractTypeBuilder?>   InterfaceMap => _interfaceMap ??= new Dictionary<Type,IAbstractTypeBuilder?>();

		public Type?                  CurrentInterface { get; set; }

		private MethodBuilderHelper? _methodBuilder;
		public  MethodBuilderHelper   MethodBuilder
		{
			get => _methodBuilder ?? throw new InvalidOperationException();
			set => _methodBuilder = value;
		}

		private TypeBuilderHelper? _typeBuilder;
		public  TypeBuilderHelper   TypeBuilder
		{
			get => _typeBuilder ?? throw new InvalidOperationException();
			set => _typeBuilder = value;
		}

		[CanBeNull]
		public T GetItem<T>(string key)
		{
			return Items.TryGetValue(key, out var value) ? (T)value : default;
		}

		public LocalBuilder? ReturnValue { get; set; }
		public LocalBuilder? Exception   { get; set; }
		public Label         ReturnLabel { get; set; }

		#region BuildElement

		public BuildElement BuildElement { get; set; }

		public bool IsAbstractGetter   => BuildElement == BuildElement.AbstractGetter;
		public bool IsAbstractSetter   => BuildElement == BuildElement.AbstractSetter;
		public bool IsAbstractProperty => IsAbstractGetter || IsAbstractSetter;
		public bool IsAbstractMethod   => BuildElement == BuildElement.AbstractMethod;
		public bool IsVirtualGetter    => BuildElement == BuildElement.VirtualGetter;
		public bool IsVirtualSetter    => BuildElement == BuildElement.VirtualSetter;
		public bool IsVirtualProperty  => IsVirtualGetter  || IsVirtualSetter;
		public bool IsVirtualMethod    => BuildElement == BuildElement.VirtualMethod;
		public bool IsGetter           => IsAbstractGetter || IsVirtualGetter;
		public bool IsSetter           => IsAbstractSetter || IsVirtualSetter;
		public bool IsProperty         => IsGetter         || IsSetter;
		public bool IsMethod           => IsAbstractMethod || IsVirtualMethod;
		public bool IsMethodOrProperty => IsMethod         || IsProperty;

		#endregion

		#region BuildStep

		public BuildStep Step { get; set; }

		public bool IsBeginStep   => Step == BuildStep.Begin;
		public bool IsBeforeStep  => Step == BuildStep.Before;
		public bool IsBuildStep   => Step == BuildStep.Build;
		public bool IsAfterStep   => Step == BuildStep.After;
		public bool IsCatchStep   => Step == BuildStep.Catch;
		public bool IsFinallyStep => Step == BuildStep.Finally;
		public bool IsEndStep     => Step == BuildStep.End;

		public bool IsBeforeOrBuildStep => Step == BuildStep.Before || Step == BuildStep.Build;

		#endregion

		public List<IAbstractTypeBuilder>? TypeBuilders    { get; set; }
		public MethodInfo?                 CurrentMethod   { get; set; }
		public PropertyInfo?               CurrentProperty { get; set; }

		#region Internal Methods

		public FieldBuilder GetField(string fieldName)
		{
			return GetItem<FieldBuilder>("$BLToolkit.Field." + fieldName);
		}

		public FieldBuilder CreateField(string fieldName, Type type, FieldAttributes attributes)
		{
			var field = TypeBuilder.DefineField(fieldName, type, attributes);

			field.SetCustomAttribute(MethodBuilder.Type.Assembly.BLToolkitAttribute);

			Items.Add("$BLToolkit.Field." + fieldName, field);

			return field;
		}

		public FieldBuilder CreatePrivateField(string fieldName, Type type)
		{
			return CreateField(fieldName, type, FieldAttributes.Private);
		}

		public FieldBuilder CreatePrivateField(PropertyInfo propertyInfo, string fieldName, Type type)
		{
			var field = CreateField(fieldName, type, FieldAttributes.Private);

			if (propertyInfo != null)
				Fields.Add(propertyInfo, field);

			return field;
		}

		public FieldBuilder CreatePrivateStaticField(string fieldName, Type type)
		{
			return CreateField(fieldName, type, FieldAttributes.Private | FieldAttributes.Static);
		}

		public MethodBuilderHelper GetFieldInstanceEnsurer(string fieldName)
		{
			return GetItem<MethodBuilderHelper>($"$BLToolkit.FieldInstanceEnsurer.{fieldName}");
		}

		#endregion
	}
}

#endif
