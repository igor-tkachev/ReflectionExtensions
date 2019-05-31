#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETCOREAPP2_0 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Diagnostics;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class AsyncAspectTest
	{
		private const int ExecutionTime = 200;

		public abstract class TestObject
		{
			public int Test(int intVal, string? strVal)
			{
				System.Threading.Thread.Sleep(ExecutionTime + 30);
				return intVal;
			}

			[Async] public abstract IAsyncResult BeginTest(int intVal, string? strVal);
			[Async] public abstract IAsyncResult BeginTest(int intVal, string? strVal, AsyncCallback? callback);
			[Async] public abstract IAsyncResult BeginTest(int intVal, string? strVal, AsyncCallback? callback, object state);
			[Async] public abstract int EndTest(IAsyncResult asyncResult);

			[Async("Test")]
			public abstract IAsyncResult AnyName(int intVal, string? strVal, AsyncCallback? callback, object? state);
			[Async("Test", typeof(int), typeof(string))]
			public abstract int AnyName(IAsyncResult asyncResult);
		}

		public abstract class TestObject<T>
		{
			public T Test(T intVal)
			{
				System.Threading.Thread.Sleep(ExecutionTime + 30);
				return intVal;
			}

			[Async] public abstract IAsyncResult BeginTest(T intVal);
			[Async] public abstract T EndTest(IAsyncResult asyncResult);
		}

		[Test]
		public void AsyncTest()
		{
			var o = TypeAccessor.CreateInstanceEx<TestObject>();
			var sw = Stopwatch.StartNew();

			Assert.AreEqual(1, o.Test(1, null));
			var mss = sw.ElapsedMilliseconds;
			Assert.IsTrue(mss >= ExecutionTime);

			sw.Reset();
			sw.Start();

			var ar = o.BeginTest(2, "12");
			mss = sw.ElapsedMilliseconds;
			Assert.IsTrue(mss <= ExecutionTime);

			Assert.AreEqual(2, o.EndTest(ar));
			mss = sw.ElapsedMilliseconds;
			Assert.IsTrue(mss >= ExecutionTime);
		}

		[Test]
		public void GenericTest()
		{
			var o   = TypeAccessor.CreateInstanceEx<TestObject<DateTime>>();
			var now = DateTime.Now;
			var ar  = o.BeginTest(now);

			Assert.AreEqual(now, o.EndTest(ar));
		}

		private static void CallBack(IAsyncResult ar)
		{
			var o = (TestObject) ar.AsyncState;
			Console.WriteLine("Callback");
			o.EndTest(ar);
		}

		[Test]
		public void CallbackTest()
		{
			var o = TypeAccessor.CreateInstanceEx<TestObject>();

			o.BeginTest(2, null, CallBack, o);
		}

		[Test]
		public void NoStateTest()
		{
			var o = TypeAccessor.CreateInstanceEx<TestObject>();

			Assert.AreEqual(1, o.Test(1, null));

			var ar = o.BeginTest(2, null, null);
			Assert.AreEqual(2, o.EndTest(ar));
		}

		[Test]
		public void NoCallbackTest()
		{
			var o = TypeAccessor.CreateInstanceEx<TestObject>();

			Assert.AreEqual(1, o.Test(1, null));

			var ar = o.BeginTest(2, "1234");
			Assert.AreEqual(2, o.EndTest(ar));
		}

		[Test]
		public void AnyNameTest()
		{
			var o = TypeAccessor.CreateInstanceEx<TestObject>();

			var ar = o.AnyName(2, null, null, null);
			Assert.AreEqual(2, o.AnyName(ar));
		}
	}
}

#endif
