using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class LazyInstanceAttribute : Attribute
	{
		public LazyInstanceAttribute()
		{
			IsLazy = true;
		}

		public LazyInstanceAttribute(bool isLazy)
		{
			IsLazy = isLazy;
		}

		public bool IsLazy { get; set; }
	}
}
