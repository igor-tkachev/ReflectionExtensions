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
	public class InterceptorAttribute : AbstractTypeBuilderAttribute
	{
		public InterceptorAttribute(Type? interceptorType, InterceptType interceptType)
			: this(interceptorType, interceptType, null, TypeBuilderConsts.Priority.Normal)
		{
		}

		public InterceptorAttribute(Type? interceptorType, InterceptType interceptType, int priority)
			: this(interceptorType, interceptType, null, priority)
		{
		}

		public InterceptorAttribute(Type? interceptorType, InterceptType interceptType, string parameters)
			: this(interceptorType, interceptType, parameters, TypeBuilderConsts.Priority.Normal)
		{
		}

		public InterceptorAttribute(
			Type? interceptorType, InterceptType interceptType, string? configString, int priority)
			: this(interceptorType, interceptType, configString, priority, false)
		{
		}

		public InterceptorAttribute(
			Type? interceptorType, InterceptType interceptType, string? configString, int priority, bool localInterceptor)
		{
			if (interceptorType == null && interceptType != 0)
				throw new ArgumentNullException(nameof(interceptorType));

			InterceptorType  = interceptorType;
			InterceptType    = interceptType;
			ConfigString     = configString;
			Priority         = priority;
			LocalInterceptor = localInterceptor;
		}

		public  virtual  Type?         InterceptorType  { get; }
		public  virtual  InterceptType InterceptType    { get; }
		public  virtual  int           Priority         { get; }
		public  virtual  string?       ConfigString     { get; }
		public  virtual  bool          LocalInterceptor { get; }

		public override IAbstractTypeBuilder TypeBuilder =>
			new Builders.InterceptorAspectBuilder(InterceptorType, InterceptType, ConfigString, Priority, LocalInterceptor);
	}
}

#endif
