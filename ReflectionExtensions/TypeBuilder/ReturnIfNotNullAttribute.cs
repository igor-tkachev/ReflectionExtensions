using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public sealed class ReturnIfNotNullAttribute : ReturnIfNonZeroAttribute
	{
	}
}
