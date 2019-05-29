#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.TypeBuilder
{
	using Builders;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class GlobalInstanceTypeAttribute : InstanceTypeAttribute
	{
		public GlobalInstanceTypeAttribute(Type propertyType, Type instanceType, params object[] parameters)
			: base(instanceType, parameters)
		{
			PropertyType = propertyType;
		}

		public Type  PropertyType { get; }

		private         IAbstractTypeBuilder? _typeBuilder;
		public override IAbstractTypeBuilder   TypeBuilder =>
			_typeBuilder ??= new InstanceTypeBuilder(PropertyType, InstanceType, IsObjectHolder);
	}
}

#endif
