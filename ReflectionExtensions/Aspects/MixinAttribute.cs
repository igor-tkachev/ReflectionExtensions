#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using TypeBuilder.Builders;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public sealed class MixinAttribute : AbstractTypeBuilderAttribute
	{
		public MixinAttribute(
			Type targetInterface, string memberName, bool throwExceptionIfNull, string? exceptionMessage)
		{
			TargetInterface      = targetInterface;
			MemberName           = memberName;
			ThrowExceptionIfNull = throwExceptionIfNull;
			ExceptionMessage     = exceptionMessage;
		}

		public MixinAttribute(Type targetInterface, string memberName, bool throwExceptionIfNull)
			: this(targetInterface, memberName, throwExceptionIfNull, null)
		{
		}

		public MixinAttribute(Type targetInterface, string memberName, string exceptionMessage)
			: this(targetInterface, memberName, true, exceptionMessage)
		{
		}

		public MixinAttribute(Type targetInterface, string memberName)
			: this(targetInterface, memberName, true, null)
		{
		}

		public Type    TargetInterface      { get; }
		public string  MemberName           { get; }
		public bool    ThrowExceptionIfNull { get; set; }
		public string? ExceptionMessage     { get; set; }

		public override IAbstractTypeBuilder TypeBuilder =>
			new Builders.MixinAspectBuilder(TargetInterface, MemberName, ThrowExceptionIfNull, ExceptionMessage);
	}
}

#endif
