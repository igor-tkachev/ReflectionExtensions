#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;
	using ReflectionExtensions.TypeBuilder;

	[TestFixture]
	public class NotNullAspectTest
	{
		public abstract class TestObject1
		{
			public virtual void Foo1(string str1, [NotNull] string str2, string str3) {}
			public virtual void Foo2(string str1, [NotNull("Null")] string str2, string str3) { }
			public virtual void Foo3(string str1, [NotNull("Null: {0}")] string str2, string str3) { }
		}

		[Test]
		public void Test1()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestObject1 o = (TestObject1)TypeAccessor.CreateInstance(typeof(TestObject1));
				o.Foo1("str1", null, "str3");
			});
		}

		[Test]
		public void Test2()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestObject1 o = (TestObject1)TypeAccessor.CreateInstance(typeof(TestObject1));
				o.Foo2("str1", null, "str3");
			},
			"Null");
		}

		[Test]
		public void Test3()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestObject1 o = (TestObject1)TypeAccessor.CreateInstance(typeof(TestObject1));
				o.Foo3("str1", null, "str3");
			},
			"Null: str2");
		}
	}
}

#endif
