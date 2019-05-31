#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;
using System.Security.Principal;
using System.Threading;

namespace ReflectionExtensions.Aspects
{
	public sealed class InterceptCallInfo
	{
		public InterceptCallInfo()
		{
			CurrentPrincipal = Thread.CurrentPrincipal;
			CurrentThread    = Thread.CurrentThread;
		}

		private CallMethodInfo? _callMethodInfo;
		public  CallMethodInfo?  CallMethodInfo
		{
			get => _callMethodInfo;
			set
			{
				if (_callMethodInfo == value)
				{
					// A race condition.
					//
					return;
				}

				if (_callMethodInfo != null)
					throw new InvalidOperationException("MethodInfo can not be changed.");

				_callMethodInfo = value;

				var len = value?.MethodInfo.GetParameters().Length ?? 0;

				ParameterValues = len == 0? _emptyValues: new object[len];
			}
		}

		readonly object[] _emptyValues = new object[0];

		public object[]?       ParameterValues  { get; set; }
		public object?         ReturnValue      { get; set; }
		public InterceptResult InterceptResult  { get; set; } = InterceptResult.Continue;
		public InterceptType   InterceptType    { get; set; }
		public Exception?      Exception        { get; set; }

		private Hashtable?    _items;
		public IDictionary     Items => _items ??= new Hashtable();

		public DateTime        BeginCallTime    { get; } = DateTime.Now;
		public IPrincipal      CurrentPrincipal { get; }
		public Thread          CurrentThread    { get; }
		public bool            Cached           { get; set; }
		public object?         Object           { get; set; }
	}
}

#endif
