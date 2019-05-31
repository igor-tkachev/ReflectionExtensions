#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;
using System.Reflection;

namespace ReflectionExtensions.Aspects
{
	[System.Diagnostics.DebuggerStepThrough]
	public class MethodCallCounter
	{
		public MethodCallCounter(CallMethodInfo methodInfo)
		{
			CallMethodInfo = methodInfo;
			MethodInfo     = methodInfo.MethodInfo;
		}

		int      _exceptionCount;
		int      _cachedCount;
		TimeSpan _maxTime;

		#region Public Members

		public MethodInfo     MethodInfo     { get; set; }
		public CallMethodInfo CallMethodInfo { get; set; }
		public int            TotalCount     { get; set; }
		public TimeSpan       TotalTime      { get; set; }
		public TimeSpan       MinTime        { get; set; } = TimeSpan.MaxValue;
		public ArrayList      CurrentCalls   { get; } = ArrayList.Synchronized(new ArrayList());
		public TimeSpan       AverageTime => TotalCount == 0 ? TimeSpan.MinValue : new TimeSpan(TotalTime.Ticks / TotalCount);

		#endregion

		#region Protected Members

		public virtual void RegisterCall(InterceptCallInfo info)
		{
			lock (CurrentCalls.SyncRoot)
				CurrentCalls.Add(info);
		}

		public virtual void UnregisterCall(InterceptCallInfo info)
		{
			AddCall(DateTime.Now - info.BeginCallTime, info.Exception != null, info.Cached);

			lock (CurrentCalls.SyncRoot)
				CurrentCalls.Remove(info);
		}

		protected void AddCall(TimeSpan time, bool withException, bool cached)
		{
			if (cached)
			{
				_cachedCount++;
			}
			else
			{
				TotalTime += time;
				TotalCount++;

				if (MinTime > time) MinTime = time;
				if (_maxTime < time) _maxTime = time;
			}

			if (withException) _exceptionCount++;
		}

		#endregion
	}
}

#endif
