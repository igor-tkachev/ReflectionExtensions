#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using ReflectionExtensions.TypeBuilder.Builders;

	[AttributeUsage(AttributeTargets.Method)]
	public sealed class AsyncAttribute : AbstractTypeBuilderAttribute
	{
		readonly string? _targetMethodName;
		readonly Type[]? _parameterTypes;

		public AsyncAttribute()
		{
		}

		public AsyncAttribute(string targetMethodName)
			: this(targetMethodName, null)
		{
		}

		public AsyncAttribute(params Type[] parameterTypes)
			: this(null, parameterTypes)
		{
		}

		public AsyncAttribute(string? targetMethodName, params Type[]? parameterTypes)
		{
			_targetMethodName = targetMethodName;
			_parameterTypes   = parameterTypes;
		}

		public override IAbstractTypeBuilder TypeBuilder =>
			new Builders.AsyncAspectBuilder(_targetMethodName, _parameterTypes);
	}
}

#endif
