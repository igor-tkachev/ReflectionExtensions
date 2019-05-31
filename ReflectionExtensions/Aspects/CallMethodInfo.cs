#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;
using System.Reflection;

namespace ReflectionExtensions.Aspects
{
	public class CallMethodInfo
	{
		#region Init

		readonly object _sync = new object();

		#endregion

		#region Public Members

		public CallMethodInfo(MethodInfo methodInfo)
		{
			MethodInfo = methodInfo;
			Parameters = methodInfo.GetParameters();
		}

		public MethodInfo       MethodInfo { get; }
		public ParameterInfo[]  Parameters { get; }
		public CacheAspect?     CacheAspect { get; internal set; }
		public object?          SyncRoot    { get; set; }

		volatile Hashtable?  _items;
		public   IDictionary  Items
		{
			get
			{
				if (_items == null) lock (_sync) if (_items == null)
					_items = Hashtable.Synchronized(new Hashtable());

				return _items;
			}
		}

		#endregion

		#region Proptected Members

		internal bool[]? CacheableParameters { get; set; }

		#endregion
	}
}

#endif
