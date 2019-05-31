#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	public class CacheAspectItem
	{
		public virtual DateTime MaxCacheTime { get; set; }
		public virtual bool     IsExpired => MaxCacheTime <= DateTime.Now;

		public object?       ReturnValue { get; set; }
		public object[]?     RefValues   { get; set; }
	}
}

#endif
