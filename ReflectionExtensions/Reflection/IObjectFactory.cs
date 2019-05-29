using System;

namespace ReflectionExtensions.Reflection
{
	public interface IObjectFactory
	{
		object CreateInstance(TypeAccessor typeAccessor, InitContext? context);
	}
}
