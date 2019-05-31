#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class LoggingAspectTest
	{
		[Log]
		public abstract class TestClass
		{
			public abstract int Test();

			[Log("LogParameters=true")]
			public virtual void Test(ArrayList list, int i, string s, char c)
			{
			}

			[Log("LogParameters=true")]
			public virtual void Test(int i)
			{
				throw new ApplicationException("test exception");
			}
		}

		[Test]
		public void Test1()
		{
			TestClass t = (TestClass)TypeAccessor.CreateInstance(typeof(TestClass));

			t.Test();
			t.Test(new ArrayList(), 567, "876", 'X');
		}

		[Test]
		public void Test2()
		{
			Assert.Throws<ApplicationException>(() =>
			{
				var t = (TestClass)TypeAccessor.CreateInstance(typeof(TestClass));
				t.Test(123);
			});
		}
	}
}

#endif
