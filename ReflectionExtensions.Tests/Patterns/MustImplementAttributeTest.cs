﻿#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Patterns
{
	using ReflectionExtensions.Patterns;
	using ReflectionExtensions.TypeBuilder;

	[TestFixture]
	public class MustImplementAttributeTest
	{
		[MustImplement]
		public interface IRequiredInterface
		{
			int RequiredMethod();
			[MustImplement(false)]
			int SameMethodName();
			[MustImplement(false)]
			int OptionalMethod();
		}

		public interface ISameMethodNameInterface
		{
			[MustImplement]
			int SameMethodName();
		}

		public interface IIntermediateInterface : IRequiredInterface, ISameMethodNameInterface
		{
			[MustImplement(false, ThrowException = true)]
			new int OptionalMethod();
		}

		public interface IOptionalInterface : IIntermediateInterface
		{
		}

		[MustImplement(false, ThrowException = false)]
		public interface IOptionalInterfaceNoException : IRequiredInterface
		{
			int OtherOptionalMethod();
		}

		[MustImplement(true, ThrowException = false)]
		public interface IOtherOptionalInterface
		{
			int SameMethodName();
		}

		public struct TestClass
		{
			public int RequiredMethod()
			{
				return 1;
			}

			public int SameMethodName()
			{
				return 2;
			}
		}

		public class EmptyClass
		{
		}

		[Test]
		public void Test()
		{
			var duck = DuckTyping.Implement<IOptionalInterfaceNoException> (new TestClass());

			Assert.AreEqual(1, duck.RequiredMethod());
			Assert.AreEqual(0, duck.OtherOptionalMethod());
			Assert.AreEqual(2, duck.SameMethodName());

			duck = DuckTyping.Aggregate<IOptionalInterfaceNoException>(new TestClass(), string.Empty, Guid.Empty);

			Assert.AreEqual(1, duck.RequiredMethod());
			Assert.AreEqual(0, duck.OtherOptionalMethod());
			Assert.AreEqual(2, duck.SameMethodName());
		}

		[Test]
		public void RuntimeExceptionTest()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var duck = DuckTyping.Implement<IOptionalInterface>(new TestClass());

				Assert.AreEqual(1, duck.RequiredMethod());

				// Exception here.
				//
				duck.OptionalMethod();
			});
		}

		[Test]
		public void RuntimeAggregateExceptionTest()
		{
			Assert.Throws<InvalidOperationException>(() =>
			{
				var duck = DuckTyping.Aggregate<IOptionalInterface>(new TestClass(), new EmptyClass());

				Assert.AreEqual(1, duck.RequiredMethod());

				// Exception here.
				//
				duck.OptionalMethod();
			});
		}

		[Test]
		public void BuildTimeExceptionTest()
		{
			Assert.Throws<TypeBuilderException>(() =>
			{
				// Exception here.
				//
				_ = DuckTyping.Implement<IOptionalInterface>(string.Empty);
			});
		}

		[Test]
		public void BuildTimeAggregateExceptionTest()
		{
			Assert.Throws<TypeBuilderException>(() =>
			{
				// Exception here.
				//
				_ = DuckTyping.Aggregate<IOptionalInterface>(string.Empty, Guid.Empty);
			});
		}

		[Test]
		public void AsLikeBehaviourTest()
		{
			var duck = DuckTyping.Implement<IOtherOptionalInterface>(new TestClass(), false);

			Assert.IsNotNull(duck);

			duck = DuckTyping.Implement<IOtherOptionalInterface>(new EmptyClass(), false);
			Assert.IsNull(duck);

			duck = DuckTyping.Implement<IOtherOptionalInterface>(new EmptyClass(), false);
			Assert.IsNull(duck);

			duck = DuckTyping.Aggregate<IOtherOptionalInterface>(false, new EmptyClass(), string.Empty);
			Assert.IsNull(duck);
		}
	}
}

#endif
