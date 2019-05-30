#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class NotNullAttribute : AbstractTypeBuilderAttribute
	{
		public NotNullAttribute()
		{
			Message = "";
		}

		public NotNullAttribute(string message)
		{
			Message = message;
		}

		public  string  Message { get; set; }

		public override IAbstractTypeBuilder TypeBuilder => new Builders.NotNullAspectBuilder(Message);
	}
}

#endif
