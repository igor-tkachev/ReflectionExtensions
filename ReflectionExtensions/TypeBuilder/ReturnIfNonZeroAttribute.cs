using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public class ReturnIfNonZeroAttribute : Attribute
	{
	}
}
