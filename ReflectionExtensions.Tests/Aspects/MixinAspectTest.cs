#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Aspects
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class MixinAspectTest
	{
		public interface ITestInterface1
		{
			int TestMethod(int value);
		}

		public class TestInterface1Impl : ITestInterface1
		{
			public int TestMethod(int value) { return value; }
		}

		public interface ITestInterface2
		{
			int TestMethod1(int value);
			int TestMethod2(int value);
		}

		public class TestInterface2Impl : ITestInterface2
		{
			public int TestMethod1(int value) { return value; }
			public int TestMethod2(int value) { return value; }
		}

		[Mixin(typeof(ITestInterface1), "_testInterface1")]
		[Mixin(typeof(ITestInterface2), "TestInterface2", "'{0}.{1}' is null.")]
		public abstract class TestClass
		{
			public TestClass()
			{
				_testInterface1 = new TestInterface1Impl();
			}

			protected object?          _testInterface1;

			private   ITestInterface2? _testInterface2;
			public    ITestInterface2   TestInterface2 => _testInterface2 ??= new TestInterface2Impl();

			[MixinOverride(typeof(ITestInterface2))]
			protected int TestMethod1(int value) { return 15; }
		}

		[Test]
		public void Test()
		{
			var tc = TypeAccessor.CreateInstance<TestClass>();
			var i1 = (ITestInterface1)tc;
			var i2 = (ITestInterface2)tc;

			Assert.AreEqual(10, i1.TestMethod (10));
			Assert.AreEqual(15, i2.TestMethod1(20));
			Assert.AreEqual(30, i2.TestMethod2(30));
		}
	}
}

#endif
