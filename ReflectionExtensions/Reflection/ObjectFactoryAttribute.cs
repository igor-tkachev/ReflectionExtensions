using System;

namespace ReflectionExtensions.Reflection
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class ObjectFactoryAttribute : Attribute
	{
		public ObjectFactoryAttribute(Type type)
		{
			if (type == null) throw new ArgumentNullException("type");

			ObjectFactory = Activator.CreateInstance(type) as IObjectFactory ??
				throw new ArgumentException($"Type '{type}' does not implement IObjectFactory interface.");
		}

		public IObjectFactory ObjectFactory { get; }
	}
}
