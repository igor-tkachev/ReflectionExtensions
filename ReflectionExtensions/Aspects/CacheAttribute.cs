#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	using ReflectionExtensions.TypeBuilder.Builders;

	/// <summary>
	/// http://www.bltoolkit.net/Doc/Aspects/index.htm
	/// </summary>
	[AttributeUsage(
		AttributeTargets.Class |
		AttributeTargets.Interface |
		AttributeTargets.Property |
		AttributeTargets.Method,
		AllowMultiple=true)]
	public class CacheAttribute : InterceptorAttribute
	{
		#region Constructors

		public CacheAttribute()
			: this(typeof(CacheAspect), null)
		{
		}

		public CacheAttribute(Type cacheAspectType, string? configString)
			: base(
				cacheAspectType,
				InterceptType.BeforeCall | InterceptType.AfterCall | InterceptType.OnFinally,
				configString,
				TypeBuilderConsts.Priority.CacheAspect)
		{
			if (!typeof(CacheAspect).IsSameOrParentOf(cacheAspectType))
				throw new ArgumentException("Parameter 'cacheAspectType' must be of CacheAspect type.");
		}

		public CacheAttribute(Type interceptorType)
			: this(interceptorType, null)
		{
		}

		public CacheAttribute(Type interceptorType, int maxCacheTime)
			: this(interceptorType, null)
		{
			MaxCacheTime = maxCacheTime;
		}

		public CacheAttribute(Type interceptorType, bool isWeak)
			: this(interceptorType, null)
		{
			IsWeak = isWeak;
		}

		public CacheAttribute(Type interceptorType, int maxCacheTime, bool isWeak)
			: this(interceptorType, null)
		{
			MaxCacheTime = maxCacheTime;
			IsWeak       = isWeak;
		}

		public CacheAttribute(string configString)
			: this(typeof(CacheAspect), configString)
		{
		}

		public CacheAttribute(int maxCacheTime)
			: this(typeof(CacheAspect), maxCacheTime)
		{
		}

		public CacheAttribute(bool isWeak)
			: this(typeof(CacheAspect), isWeak)
		{
		}

		public CacheAttribute(int maxCacheTime, bool isWeak)
			: this(typeof(CacheAspect), maxCacheTime, isWeak)
		{
		}

		#endregion

		#region Properties

		private bool _hasMaxCacheTime;
		private int  _maxCacheTime;
		public  int   MaxCacheTime
		{
			get => _maxCacheTime;
			set { _maxCacheTime = value; _hasMaxCacheTime = true; }
		}

		public int MaxSeconds
		{
			get => MaxCacheTime / 1000;
			set => MaxCacheTime = value * 1000;
		}

		public int MaxMinutes
		{
			get => MaxCacheTime / 60 / 1000;
			set => MaxCacheTime = value * 60 * 1000;
		}

		private bool _hasIsWeak;
		private bool _isWeak;
		public  bool  IsWeak
		{
			get => _isWeak;
			set { _isWeak = value; _hasIsWeak = true; }
		}

		public override string? ConfigString
		{
			get
			{
				var s = base.ConfigString;

				if (_hasMaxCacheTime) s += ";MaxCacheTime=" + MaxCacheTime;
				if (_hasIsWeak)       s += ";IsWeak="       + IsWeak;

				if (!string.IsNullOrEmpty(s) && s[0] == ';')
					s = s.Substring(1);

				return s;
			}
		}

		#endregion
	}
}

#endif
