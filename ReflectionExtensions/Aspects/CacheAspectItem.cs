#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	public class CacheAspectItem
	{
		private        DateTime _maxCacheTime;
		public virtual DateTime  MaxCacheTime
		{
			get => _maxCacheTime;
			set => _maxCacheTime = value;
		}

		public  object?   ReturnValue { get; set; }

		public  object[]? RefValues { get; set; }

		public virtual bool IsExpired => _maxCacheTime <= DateTime.Now;
	}
}

#endif
