#if NET20 || NET30

namespace System
{
	delegate TResult Func<out TResult>();
	delegate TResult Func<in T, out TResult>(T arg);
}

#endif
