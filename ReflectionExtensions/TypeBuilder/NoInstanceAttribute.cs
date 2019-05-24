using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class NoInstanceAttribute : Attribute
	{
	}
}
