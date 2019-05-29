#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.TypeBuilder
{
	using Builders;

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true)]
	public class ImplementInterfaceAttribute : AbstractTypeBuilderAttribute
	{
		public ImplementInterfaceAttribute(Type type)
		{
			Type = type;
		}

		public Type Type { get; }

		public override IAbstractTypeBuilder TypeBuilder => new ImplementInterfaceBuilder(Type);
	}
}

#endif
