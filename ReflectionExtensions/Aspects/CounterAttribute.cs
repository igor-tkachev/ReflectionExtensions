#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	[AttributeUsage(
		AttributeTargets.Class |
		AttributeTargets.Interface |
		AttributeTargets.Property |
		AttributeTargets.Method,
		AllowMultiple=true)]
	public class CounterAttribute : InterceptorAttribute
	{
		public CounterAttribute()
			: this(typeof(CounterAspect), null)
		{
		}

		public CounterAttribute(string configString)
			: this(typeof(CounterAspect), configString)
		{
		}

		protected CounterAttribute(Type interceptorType, string? configString)
			: base(
				interceptorType,
				InterceptType.BeforeCall | InterceptType.OnCatch | InterceptType.OnFinally,
				configString,
				TypeBuilderConsts.Priority.Normal)
		{
		}
	}
}

#endif
