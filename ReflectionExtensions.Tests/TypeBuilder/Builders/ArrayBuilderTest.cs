#if !NETCOREAPP1_0

using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.TypeBuilder.Builders
{
	using ReflectionExtensions.TypeBuilder;
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class ArrayBuilderTest
	{
		public abstract class TestObject
		{
			public abstract int[]    IntArray1 { get; set; }
			[LazyInstance]
			public abstract int[]    IntArray2 { get; set; }
			public abstract byte[][] ByteArray { get; set; }
			public int[,]            DimArray  { get; set; } = new int[1,1];
		}

		[Test]
		public void AbstractProperties()
		{
			var o = TypeAccessor.CreateInstance<TestObject>();

			Assert.IsNotNull(o.IntArray1);
			Assert.AreSame(o.IntArray1, o.IntArray2);

			Assert.IsNotNull(o.ByteArray);
		}
	}
}

#endif
