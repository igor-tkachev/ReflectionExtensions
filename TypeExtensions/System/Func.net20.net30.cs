namespace System
{
	delegate TResult Func<in T, out TResult>(T arg);
}
