using System;

using NUnit.Framework;

namespace TypeExtensions.Tests
{
	[TestFixture]
	public class TypeExtensionsTests
	{
		[Test]
		public void AssemblyExTest()
		{
			Assert.IsNotNull(GetType().AssemblyEx());
		}

		sealed   class SealedTest {}
		abstract class AbstractTest {}

		[Test]
		public void IsSealedExTest()
		{
			Assert.IsTrue (typeof(SealedTest).  IsSealedEx());
			Assert.IsFalse(typeof(AbstractTest).IsSealedEx());
		}

		[Test]
		public void IsAbstractExTest()
		{
			Assert.IsTrue (typeof(AbstractTest).IsAbstractEx());
			Assert.IsFalse(typeof(SealedTest).  IsAbstractEx());
		}
	}
}
