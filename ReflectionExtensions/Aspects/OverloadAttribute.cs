#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OverloadAttribute : AbstractTypeBuilderAttribute
	{
		readonly string? _overloadedMethodName;
		readonly Type[]? _parameterTypes;

		public OverloadAttribute()
		{
		}

		public OverloadAttribute(string overloadedMethodName)
			: this(overloadedMethodName, null)
		{
		}

		public OverloadAttribute(params Type[] parameterTypes)
			: this(null, parameterTypes)
		{
		}

		public OverloadAttribute(string? overloadedMethodName, params Type[]? parameterTypes)
		{
			_overloadedMethodName = overloadedMethodName;
			_parameterTypes       = parameterTypes;
		}

		public override IAbstractTypeBuilder TypeBuilder =>
			new Builders.OverloadAspectBuilder(_overloadedMethodName, _parameterTypes);
	}
}

#endif
