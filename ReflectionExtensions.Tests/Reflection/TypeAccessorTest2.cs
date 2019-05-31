#if !NETCOREAPP1_0

using System;
using System.Collections.Generic;
using System.Diagnostics;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Reflection
{
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class TypeAccessorTest2
	{
		public class TestObject
		{
			public int Field;
		}

		[Test]
		public void Test()
		{
			var o1 = TypeAccessor.CreateInstance<TestObject>();
			var o2 = TypeAccessor.CreateInstance<TestObject>();
		}

		public class TestObject1
		{
			public int?                             IntField;
			public Dictionary<int?, List<decimal?>> ListField = new Dictionary<int?, List<decimal?>>();
		}

		[Test]
		public void Write()
		{
			var o = new TestObject1();

			Console.WriteLine(o);
			Debug.WriteLine(o);
		}
	}
}

#endif
