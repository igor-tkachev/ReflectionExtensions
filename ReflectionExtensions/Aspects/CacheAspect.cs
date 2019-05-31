﻿

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace ReflectionExtensions.Aspects
{
	using Common;

	public delegate bool IsCacheableParameterType(Type parameterType);

	public class CacheAspect : Interceptor
	{
		#region Init

		static CacheAspect()
		{
			MaxCacheTime = int.MaxValue;
			IsEnabled    = true;

			CleanupThread.Init();
		}

		public CacheAspect()
		{
			RegisteredAspects.Add(this);
		}

		MethodInfo? _methodInfo;
		int?        _instanceMaxCacheTime;
		bool?       _instanceIsWeak;
		int         _isLocked;

		public override void Init(CallMethodInfo info, string configString)
		{
			base.Init(info, configString);

			info.CacheAspect = this;

			_methodInfo = info.MethodInfo;

			CacheSyncRoot = info.SyncRoot;

			Cache = CreateCache();

			var ps = configString.Split(';');

			foreach (var p in ps)
			{
				var vs = p.Split('=');

				if (vs.Length == 2)
				{
					switch (vs[0].ToLower().Trim())
					{
						case "maxcachetime": _instanceMaxCacheTime = int. Parse(vs[1].Trim()); break;
						case "isweak":       _instanceIsWeak       = bool.Parse(vs[1].Trim()); break;
					}
				}
			}
		}

		protected static IList RegisteredAspects { get; } = ArrayList.Synchronized(new ArrayList());

		public static CacheAspect? GetAspect(MethodInfo methodInfo)
		{
			lock (RegisteredAspects.SyncRoot)
				foreach (CacheAspect aspect in RegisteredAspects)
					if (aspect._methodInfo == methodInfo)
						return aspect;

			return null;
		}

		#endregion

		#region Overrides

		// to enable unlock in BeforeCall or in OnFinally

		private void Lock()
		{
			Monitor.Enter(CacheSyncRoot);
			_isLocked = Thread.CurrentThread.ManagedThreadId;
		}

		private void EndLock()
		{
			if (_isLocked != Thread.CurrentThread.ManagedThreadId)
				return;
			_isLocked = -1;

			Monitor.Exit(CacheSyncRoot);
		}

		protected override void BeforeCall(InterceptCallInfo info)
		{
			if (!IsEnabled)
				return;

			var cache = Cache;
			var key   = GetKey(info);

			Debug.Assert(cache != null, nameof(cache) + " != null");

			var item  = GetItem(cache, key);

			if (item != null && !item.IsExpired)
			{
				SetReturnFromCache(info, item);
				return;
			}

			try
			{
				Lock();

				item = GetItem(cache, key);

				if (item != null && !item.IsExpired)
				{
					SetReturnFromCache(info, item);
					EndLock();
				}
				else
				{
					info.Items["CacheKey"] = key;
				}
			}
			catch
			{
				EndLock();
				throw;
			}
		}

		static void SetReturnFromCache(InterceptCallInfo info, CacheAspectItem item)
		{
			Debug.Assert(item.ReturnValue != null, "item.ReturnValue != null");

			info.InterceptResult = InterceptResult.Return;
			info.ReturnValue     = item.ReturnValue;

			if (item.RefValues != null)
			{
				Debug.Assert(info.CallMethodInfo != null, "info.CallMethodInfo != null");
				Debug.Assert(info.ParameterValues != null, "info.ParameterValues != null");

				var pis = info.CallMethodInfo.Parameters;
				var n = 0;

				for (var i = 0; i < pis.Length; i++)
					if (pis[i].ParameterType.IsByRef)
						info.ParameterValues[i] = item.RefValues[n++];
			}

			info.Cached = true;
		}

		protected override void AfterCall(InterceptCallInfo info)
		{
			if (!IsEnabled)
				return;

			var cache = Cache;
			var key   = (CompoundValue) info.Items["CacheKey"];

			if (key == null)
				return;

			var maxCacheTime = _instanceMaxCacheTime ?? MaxCacheTime;
			var isWeak       = _instanceIsWeak       ?? IsWeak;

			var item = new CacheAspectItem
			{
				ReturnValue  = info.ReturnValue,
				MaxCacheTime = maxCacheTime == int.MaxValue || maxCacheTime < 0
					? DateTime.MaxValue
					: DateTime.Now.AddMilliseconds(maxCacheTime),
			};

			var pis = info.CallMethodInfo.Parameters;
			var n = 0;

			for (int i = 0; i < pis.Length; i++)
				if ( pis[i].ParameterType.IsByRef)
					n++;

			if (n > 0)
			{
				item.RefValues = new object[n];

				n = 0;

				for (var i = 0; i < pis.Length; i++)
					if (pis[i].ParameterType.IsByRef)
						item.RefValues[n++] = info.ParameterValues[i];
			}

			Debug.Assert(cache != null, nameof(cache) + " != null");
			cache[key] = isWeak ? (object) new WeakReference(item) : item;
		}

		protected override void OnFinally(InterceptCallInfo info)
		{
			EndLock();
			base.OnFinally(info);
		}

		public bool HasCache => CacheSyncRoot != null && Cache != null;

		#endregion

		#region Global Parameters

		public static bool IsEnabled    { get; set; }
		public static int  MaxCacheTime { get; set; }
		public static bool IsWeak       { get; set; }

		#endregion

		#region IsCacheableParameterType

		private static IsCacheableParameterType _isCacheableParameterType =
			IsCacheableParameterTypeInternal;

		public  static IsCacheableParameterType  IsCacheableParameterType
		{
			get { return _isCacheableParameterType; }
			set { _isCacheableParameterType = value ?? IsCacheableParameterTypeInternal; }
		}

		private static bool IsCacheableParameterTypeInternal(Type parameterType)
		{
			return parameterType.IsValueType || parameterType == typeof(string);
		}

		#endregion

		#region Cache

		internal object? CacheSyncRoot;

		public  IDictionary? Cache { get; private set; }

		protected virtual CacheAspectItem CreateCacheItem(InterceptCallInfo info)
		{
			return new CacheAspectItem();
		}

		protected virtual IDictionary CreateCache()
		{
#if FW4
			return new System.Collections.Concurrent.ConcurrentDictionary<CompoundValue, object>();
#else
			return Hashtable.Synchronized(new Hashtable());
#endif
		}

		protected static CompoundValue GetKey(InterceptCallInfo info)
		{
			var parInfo     = info.CallMethodInfo.Parameters;
			var parValues   = info.ParameterValues;
			var keyValues   = new object[parValues.Length];
			var cacheParams = info.CallMethodInfo.CacheableParameters;

			if (cacheParams == null)
			{
				info.CallMethodInfo.CacheableParameters = cacheParams = new bool[parInfo.Length];

				for (var i = 0; i < parInfo.Length; i++)
					cacheParams[i] = IsCacheableParameterType(parInfo[i].ParameterType);
			}

			for (var i = 0; i < parValues.Length; i++)
				keyValues[i] = cacheParams[i] ? parValues[i] : null;

			return new CompoundValue(keyValues);
		}

		protected static CacheAspectItem GetItem(IDictionary cache, CompoundValue key)
		{
			var obj = cache[key];

			if (obj == null)
				return null;

			var wr = obj as WeakReference;

			if (wr == null)
				return (CacheAspectItem)obj;

			obj = wr.Target;

			if (obj != null)
				return (CacheAspectItem)obj;

			cache.Remove(key);

			return null;
		}

		/// <summary>
		/// Clear a method call cache.
		/// </summary>
		/// <param name="methodInfo">The <see cref="MethodInfo"/> representing cached method.</param>
		public static void ClearCache(MethodInfo methodInfo)
		{
			if (methodInfo == null)
				throw new ArgumentNullException("methodInfo");

			var aspect = GetAspect(methodInfo);

			if (aspect != null)
				CleanupThread.ClearCache(aspect);
		}

		/// <summary>
		/// Clear a method call cache.
		/// </summary>
		/// <param name="declaringType">The method declaring type.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="types">An array of <see cref="System.Type"/> objects representing
		/// the number, order, and type of the parameters for the method to get.-or-
		/// An empty array of the type <see cref="System.Type"/> (for example, <see cref="System.Type.EmptyTypes"/>)
		/// to get a method that takes no parameters.</param>
		public static void ClearCache(Type declaringType, string methodName, params Type[] types)
		{
			ClearCache(GetMethodInfo(declaringType, methodName, types));
		}

		/// <summary>
		/// Clear a method call cache.
		/// </summary>
		/// <param name="declaringType">The method declaring type.</param>
		/// <param name="methodName">The method name.</param>
		/// <param name="types">An array of <see cref="System.Type"/> objects representing
		/// the number, order, and type of the parameters for the method to get.-or-
		/// An empty array of the type <see cref="System.Type"/> (for example, <see cref="System.Type.EmptyTypes"/>)
		/// to get a method that takes no parameters.</param>
		/// <param name="values">An array of values of the parameters for the method to get</param>
		public static void ClearCache(Type declaringType, string methodName, Type[] types, object[] values)
		{
			var methodInfo = GetMethodInfo(declaringType, methodName, types);

			if (methodInfo == null)
				throw new ArgumentNullException("methodInfo");

			var aspect = GetAspect(methodInfo);

			if (aspect != null)
				CleanupThread.ClearCache(aspect, new CompoundValue(values));
		}

		public static void ClearCache(Type declaringType)
		{
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");

			if (declaringType.IsAbstract)
				declaringType = TypeBuilder.TypeFactory.GetType(declaringType);

			lock (RegisteredAspects.SyncRoot)
				foreach (CacheAspect aspect in RegisteredAspects)
					if (aspect._methodInfo.DeclaringType == declaringType)
						CleanupThread.ClearCache(aspect);
		}

		public static MethodInfo GetMethodInfo(Type declaringType, string methodName, params Type[] parameterTypes)
		{
			if (declaringType == null)
				throw new ArgumentNullException("declaringType");

			if (declaringType.IsAbstract)
				declaringType = TypeBuilder.TypeFactory.GetType(declaringType);

			if (parameterTypes == null)
				parameterTypes = Type.EmptyTypes;

			var methodInfo = declaringType.GetMethod(
				methodName,
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null,
				parameterTypes,
				null);

			if (methodInfo == null)
			{
				methodInfo = declaringType.GetMethod(
					methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

				if (methodInfo == null)
					throw new ArgumentException(string.Format("Method '{0}.{1}' not found.",
						declaringType.FullName, methodName));
			}

			return methodInfo;
		}

		/// <summary>
		/// Clear all cached method calls.
		/// </summary>
		public static void ClearCache()
		{
			CleanupThread.ClearCache();
		}

		#endregion

		#region Statistics

		public static int WorkTimes
		{
			get { return CleanupThread.WorkTimes; }
		}

		public static TimeSpan WorkTime
		{
			get { return CleanupThread.WorkTime; }
		}

		public static int ObjectsExpired
		{
			get { return CleanupThread.ObjectsExpired; }
		}

		public static int ObjectsInCache
		{
			get { return CleanupThread.ObjectsInCache; }
		}

		#endregion

		#region Cleanup Thread

		private class CleanupThread
		{
			private CleanupThread() {}

			internal static void Init()
			{
				AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
				Start();
			}

			static void CurrentDomain_DomainUnload(object sender, EventArgs e)
			{
				Stop();
			}

			static volatile Timer  _timer;
			static readonly object _syncTimer = new object();

			private static void Start()
			{
				if (_timer == null)
					lock (_syncTimer)
						if (_timer == null)
						{
							var interval = TimeSpan.FromSeconds(10);
							_timer = new Timer(Cleanup, null, new TimeSpan(0), interval);
						}
			}

			private static void Stop()
			{
				if (_timer != null)
					lock (_syncTimer)
						if (_timer != null)
						{
							_timer.Dispose();
							_timer = null;
						}
			}

			private static void Cleanup(object state)
			{
				if (!Monitor.TryEnter(RegisteredAspects.SyncRoot, 10))
				{
					// The Cache is busy, skip this turn.
					//
					return;
				}

				var start          = DateTime.Now;
				var objectsInCache = 0;

				try
				{
					_workTimes++;

					var list = new List<DictionaryEntry>();

					foreach (CacheAspect aspect in RegisteredAspects)
					{
						if (!aspect.HasCache)
							continue;

						var cache = aspect.Cache;

						// cache can be in process now
						if(!Monitor.TryEnter(aspect.CacheSyncRoot, 10))
							continue;
						try
						{

							foreach (DictionaryEntry de in cache)
							{
								var wr = de.Value as WeakReference;

								bool isExpired;

								if (wr != null)
								{
									var ca = wr.Target as CacheAspectItem;

									isExpired = ca == null || ca.IsExpired;
								}
								else
								{
									isExpired = ((CacheAspectItem) de.Value).IsExpired;
								}

								if (isExpired)
									list.Add(de);
							}

							foreach (var de in list)
							{
								cache.Remove(de.Key);
								_objectsExpired++;
							}

							list.Clear();

							objectsInCache += cache.Count;
						}
						finally
						{
							Monitor.Exit(aspect.CacheSyncRoot);
						}
					}

					_objectsInCache = objectsInCache;
				}
				finally
				{
					_workTime += DateTime.Now - start;

					Monitor.Exit(RegisteredAspects.SyncRoot);
				}
			}

			private static int _workTimes;
			public  static int  WorkTimes
			{
				get { return _workTimes; }
			}

			private static TimeSpan _workTime;
			public  static TimeSpan  WorkTime
			{
				get { return _workTime; }
			}

			private static int _objectsExpired;
			public  static int  ObjectsExpired
			{
				get { return _objectsExpired; }
			}

			private static int _objectsInCache;
			public  static int  ObjectsInCache
			{
				get { return _objectsInCache; }
			}

			//public static void UnregisterCache(IDictionary cache)
			//{
			//	lock (RegisteredAspects.SyncRoot)
			//		RegisteredAspects.Remove(cache);
			//}

			public static void ClearCache(CacheAspect aspect)
			{
				lock (RegisteredAspects.SyncRoot)
				{
					if (!aspect.HasCache)
						return;

					if (!Monitor.TryEnter(aspect.CacheSyncRoot, 10))
						return;
					try
					{
						_objectsExpired += aspect.Cache.Count;
						aspect.Cache.Clear();
					}
					finally
					{
						Monitor.Exit(aspect.CacheSyncRoot);
					}
				}
			}

			public static void ClearCache(CacheAspect aspect, CompoundValue key)
			{
				lock (RegisteredAspects.SyncRoot)
				{
					if (!aspect.HasCache)
						return;

					if (!Monitor.TryEnter(aspect.CacheSyncRoot, 10))
						return;
					try
					{
						_objectsExpired += 1;
						aspect.Cache.Remove(key);
					}
					finally
					{
						Monitor.Exit(aspect.CacheSyncRoot);
					}
				}
			}

			public static void ClearCache()
			{
				lock (RegisteredAspects.SyncRoot)
				{
					foreach (CacheAspect aspect in RegisteredAspects)
					{
						if (!aspect.HasCache)
							continue;

						_objectsExpired += aspect.Cache.Count;

						if (!Monitor.TryEnter(aspect.CacheSyncRoot, 10))
							continue;
						try
						{
							aspect.Cache.Clear();
						}
						finally
						{
							Monitor.Exit(aspect.CacheSyncRoot);
						}
					}
				}
			}
		}

		#endregion
	}
}

#endif
