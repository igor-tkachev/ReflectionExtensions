#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.TypeBuilder
{
	using ReflectionExtensions.Aspects;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class GenericMethodTest
	{
		public abstract class TestObject
		{
			public virtual T? GetValue<T>([NotNull] T? value) where T : class, ICloneable
			{
				return value;
			}

			public abstract T Abstract<T>(T value) where T : struct, IFormattable;
			public abstract T Abstract2<T>(T value) where T : new();
			public abstract T Abstract3<T>(T value);
		}

		[Test]
		public void Test()
		{
			// If you got an 'Invalid executable format' exception here
			// you need to install .Net Framework 2.0 SP1 or later.
			//
			var t = TypeAccessor.CreateInstance<TestObject>();

			Assert.AreEqual("123", t.GetValue("123"));
			Assert.AreEqual(0, t.Abstract(123));
			Assert.AreEqual(0, t.Abstract2(123));
			Assert.AreEqual(0, t.Abstract3(123));

			// Throws ArgumentNullException
			//
			Assert.Throws<ArgumentNullException>(() => t.GetValue<string>(null));
		}

		public abstract class TestClass<T>
		{
			public abstract TL SelectAll<TL>() where TL : IList<T>, new();
		}

		// Works only with Mono.
		// See https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=282829
		//
		//[Test]
		public void GenericMixTest()
		{
			var t = TypeAccessor.CreateInstance<TestClass<int>>();
			Assert.That(t, Is.Not.Null);
		}
	}
}

#endif
