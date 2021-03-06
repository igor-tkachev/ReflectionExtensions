﻿#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Security.Principal;
using System.Threading;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class CounterAspectTest
	{
		[Log]
		public abstract class TestClass
		{
			[Counter]
			public virtual void Test()
			{
			}

			[Counter]
			public virtual void LongTest()
			{
				Thread.Sleep(100);
			}
		}

		[Test]
		public void Test()
		{
			var t = (TestClass)TypeAccessor.CreateInstance(typeof(TestClass));

			for (var i = 0; i < 10; i++)
				t.Test();

			var counter = CounterAspect.GetCounter(typeof(TestClass).GetMethod("Test"));

			Assert.AreEqual(10, counter?.TotalCount);

			Console.WriteLine(counter?.TotalTime);

			new Thread(t.LongTest).Start();
			Thread.Sleep(20);

			lock (CounterAspect.Counters.SyncRoot) foreach (MethodCallCounter c in CounterAspect.Counters)
			{
				Console.WriteLine("{0}.{1,-10} | {2,2} | {3,2} | {4}",
					c.MethodInfo.DeclaringType?.Name,
					c.MethodInfo.Name,
					c.TotalCount,
					c.CurrentCalls.Count,
					c.TotalTime);

				lock (c.CurrentCalls.SyncRoot) for (int i = 0; i < c.CurrentCalls.Count; i++)
				{
					InterceptCallInfo ci = (InterceptCallInfo)c.CurrentCalls[i];
					IPrincipal        pr = ci.CurrentPrincipal;

					Console.WriteLine("{0,15} | {1}",
						pr == null? "***" : pr.Identity.Name,
						DateTime.Now - ci.BeginCallTime);
				}
			}
		}

		public abstract class TestClass2
		{
			[Counter]
			public virtual void Test()
			{
			}
		}

		[Test]
		public void Test2()
		{
			// custom create counter delegate returns null
			CounterAspect.CreateCounter = mi => null;

			var t = (TestClass2)TypeAccessor.CreateInstance(typeof(TestClass2));

			// interceptor should fallback to default counter implementation
			t.Test();
		}

	}
}

#endif
