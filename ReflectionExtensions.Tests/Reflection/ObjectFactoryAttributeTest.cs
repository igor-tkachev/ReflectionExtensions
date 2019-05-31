using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Reflection
{
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class ObjectFactoryAttributeTest
	{
		[ObjectFactory(typeof(TestObject.Factory))]
		public class TestObject
		{
			public TestObject()
			{
				throw new InvalidOperationException();
			}

			private TestObject(int n)
			{
				Number = n;
			}

			public int Number;

			public class Factory : IObjectFactory
			{
				object IObjectFactory.CreateInstance(TypeAccessor typeAccessor, InitContext? context)
				{
					return new TestObject(53);
				}
			}
		}

		[Test]
		public void Test()
		{
			var o = (TestObject)TypeAccessor.CreateInstanceEx(typeof(TestObject));

			Assert.AreEqual(53, o.Number);
		}
	}
}
