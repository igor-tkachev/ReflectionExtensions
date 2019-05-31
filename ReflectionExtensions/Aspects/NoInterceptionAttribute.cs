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
	public class NoInterceptionAttribute : InterceptorAttribute
	{
		public NoInterceptionAttribute()
			: base(null, 0)
		{
		}

		public NoInterceptionAttribute(Type interceptorType, InterceptType interceptType)
			: base(interceptorType, interceptType)
		{
		}

		public override IAbstractTypeBuilder TypeBuilder => new NoInterceptionAspectBuilder(InterceptorType, InterceptType);

		private class NoInterceptionAspectBuilder : Builders.InterceptorAspectBuilder
		{
			public NoInterceptionAspectBuilder(Type? interceptorType, InterceptType interceptType)
				: base(interceptorType, interceptType, null, TypeBuilderConsts.Priority.Normal, false)
			{
			}

			public override void Build(BuildContext context)
			{
				if (context.Step == BuildStep.Begin || context.Step == BuildStep.End)
					base.Build(context);
			}
		}
	}
}

#endif
