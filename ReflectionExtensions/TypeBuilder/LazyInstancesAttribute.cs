using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class LazyInstancesAttribute : Attribute
	{
		public LazyInstancesAttribute()
		{
			IsLazy = true;
			Type   = typeof(object);
		}

		public LazyInstancesAttribute(Type type)
		{
			IsLazy = true;
			Type   = type;
		}

		public LazyInstancesAttribute(bool isLazy)
		{
			IsLazy = isLazy;
			Type   = typeof(object);
		}

		public LazyInstancesAttribute(Type type, bool isLazy)
		{
			IsLazy = isLazy;
			Type   = type;
		}

		public bool IsLazy { get; set; }
		public Type Type   { get; set; }
	}
}
