using System;

using NUnit.Framework;

namespace TypeExtensions.Tests
{
	[TestFixture]
	public class TypeExtensionsTests
	{
		[Test]
		public void AssemblyTest()
		{
			Assert.IsNotNull(GetType().AssemblyEx());
		}
	}
}
