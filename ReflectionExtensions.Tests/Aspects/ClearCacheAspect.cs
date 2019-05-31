#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;
	using ReflectionExtensions.TypeBuilder;

	[TestFixture]
	public class ClearCacheAspect
	{
		public abstract class TestClass
		{
			int _value;

			[Cache]
			public virtual int Test1()
			{
				return _value++;
			}

			[ClearCache("Test1")]
			public abstract void ClearTest1();

			[Cache]
			public virtual int Test2()
			{
				return _value++;
			}

			[Cache]
			public virtual int Test2(int i)
			{
				return _value++;
			}

			[ClearCache("Test2")]
			public abstract void ClearTest2();

			[ClearCache("Test2", typeof(int))]
			public abstract void ClearTest2a();

			[ClearCache("Test2"), ClearCache("Test2", typeof(int))]
			public abstract void ClearTest2b();
		}

		public abstract class TestClass1
		{
			[ClearCache(typeof(TestClass), "Test2")]
			public abstract void ClearTest();

			[ClearCache(typeof(TestClass), "Test2", typeof(int))]
			public abstract void ClearTest1();

			protected abstract int Test();

			[ClearCache("Test")]
			public abstract void ClearTest3();

			[ClearCache(typeof(TestClass))]
			public abstract void ClearTest4();

			[ClearCache]
			public abstract void ClearTest5();
		}

		[Test]
		public void Test1()
		{
			TestClass tc = TypeFactory.CreateInstance<TestClass>();

			int value1 = tc.Test1();
			int value2 = tc.Test1();

			Assert.AreEqual(value1, value2);

			tc.ClearTest1();

			Assert.AreNotEqual(value1, tc.Test1());
		}

		[Test]
		public void Test2()
		{
			TestClass tc = TypeFactory.CreateInstance<TestClass>();

			tc.ClearTest2();

			int value1 = tc.Test2();
			int value2 = tc.Test2();

			Assert.AreEqual(value1, value2);

			tc.ClearTest2();

			Assert.AreNotEqual(value1, tc.Test2());
		}

		[Test]
		public void Test2a()
		{
			TestClass tc = TypeFactory.CreateInstance<TestClass>();

			tc.ClearTest2a();

			int value1 = tc.Test2(1);
			int value2 = tc.Test2(1);

			Assert.AreEqual(value1, value2);

			tc.ClearTest2a();

			Assert.AreNotEqual(value1, tc.Test2(1));
		}

		[Test]
		public void Test2b()
		{
			TestClass tc = TypeFactory.CreateInstance<TestClass>();

			tc.ClearTest2b();

			int value1 = tc.Test2();
			int value2 = tc.Test2();
			int value3 = tc.Test2(1);
			int value4 = tc.Test2(1);

			Assert.AreEqual(value1, value2);
			Assert.AreEqual(value3, value4);

			tc.ClearTest2b();

			Assert.AreNotEqual(value1, tc.Test2());
			Assert.AreNotEqual(value3, tc.Test2(1));
		}

		[Test]
		public void Test3()
		{
			TestClass  tc1 = TypeAccessor.CreateInstance<TestClass> ();
			TestClass1 tc2 = TypeAccessor.CreateInstance<TestClass1>();

			tc1.ClearTest2b();

			int value1 = tc1.Test2();
			int value2 = tc1.Test2();
			int value3 = tc1.Test2(1);
			int value4 = tc1.Test2(1);

			Assert.AreEqual(value1, value2);
			Assert.AreEqual(value3, value4);

			tc2.ClearTest();
			tc2.ClearTest1();

			Assert.AreNotEqual(value1, tc1.Test2());
			Assert.AreNotEqual(value3, tc1.Test2(1));
		}

		[Test]
		public void Test4()
		{
			TestClass1 tc = TypeAccessor.CreateInstance<TestClass1>();

			tc.ClearTest3();
		}

		[Test]
		public void Test5()
		{
			TestClass  tc1 = TypeAccessor.CreateInstance<TestClass> ();
			TestClass1 tc2 = TypeAccessor.CreateInstance<TestClass1>();

			tc1.ClearTest2b();

			int value1 = tc1.Test2();
			int value2 = tc1.Test2();
			int value3 = tc1.Test2(1);
			int value4 = tc1.Test2(1);

			Assert.AreEqual(value1, value2);
			Assert.AreEqual(value3, value4);

			tc2.ClearTest();
			tc2.ClearTest4();

			Assert.AreNotEqual(value1, tc1.Test2());
			Assert.AreNotEqual(value3, tc1.Test2(1));
		}

		[Test]
		public void Test6()
		{
			TestClass1 tc = TypeAccessor.CreateInstance<TestClass1>();

			tc.ClearTest5();
		}
	}
}

#endif
