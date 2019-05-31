#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.Aspects
{
	[System.Diagnostics.DebuggerStepThrough]
	public abstract class Interceptor : IInterceptor
	{
		public virtual void Init(CallMethodInfo info, string configString)
		{
		}

		public virtual void Intercept(InterceptCallInfo info)
		{
			switch (info.InterceptType)
			{
				case InterceptType.BeforeCall: BeforeCall(info); break;
				case InterceptType.AfterCall:  AfterCall (info); break;
				case InterceptType.OnCatch:    OnCatch   (info); break;
				case InterceptType.OnFinally:  OnFinally (info); break;
			}
		}

		protected virtual void BeforeCall(InterceptCallInfo info) {}
		protected virtual void AfterCall (InterceptCallInfo info) {}
		protected virtual void OnCatch   (InterceptCallInfo info) {}
		protected virtual void OnFinally (InterceptCallInfo info) {}
	}
}

#endif
