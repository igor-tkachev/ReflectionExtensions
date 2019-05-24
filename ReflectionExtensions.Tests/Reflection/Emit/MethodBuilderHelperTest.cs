#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Reflection.Emit
{
	using ReflectionExtensions.Reflection.Emit;

	[TestFixture]
	public class MethodBuilderHelperTest
	{
		public abstract class TestObject
		{
			public    abstract int Property { get; }
			protected abstract int Method1(float f);
			public    abstract int Method2(float f);

			public int Method3(float f) { return Method1(f); }
		}

		[Test]
		public void Test()
		{
			var typeBuilder = new AssemblyBuilderHelper("HelloWorld.dll").DefineType("Test", typeof(TestObject));

			// Property
			//
			var propertyInfo  = typeof(TestObject).GetProperty("Property");
			var methodBuilder = typeBuilder.DefineMethod(propertyInfo.GetGetMethod());
			var emit          = methodBuilder.Emitter;

			emit
				.ldc_i4(10)
				.ret()
				;

			// Method1
			//
			var methodInfo = typeof(TestObject).GetMethod(
				"Method1", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			methodBuilder = typeBuilder.DefineMethod(methodInfo);
			emit          = methodBuilder.Emitter;

			emit
				.ldc_i4(10)
				.ret()
				;

			// Method2
			//
			methodInfo = typeof(TestObject).GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

			methodBuilder = typeBuilder.DefineMethod(
				"Method2",
				MethodAttributes.Virtual |
				MethodAttributes.Public |
				MethodAttributes.HideBySig	|
				MethodAttributes.PrivateScope |
				MethodAttributes.VtableLayoutMask,
				typeof(int),
				new Type[] { typeof(float) });

			typeBuilder.TypeBuilder.DefineMethodOverride(methodBuilder, methodInfo);

			emit = methodBuilder.Emitter;

			emit
				.ldc_i4(10)
				.ret()
				;

			// Create type.
			//
			var type = typeBuilder.Create();
			var obj  = (TestObject)Activator.CreateInstance(type);

			Assert.AreEqual(10, obj.Property);
			Assert.AreEqual(10, obj.Method3(0));
			Assert.AreEqual(10, obj.Method2(0));
		}
	}
}

#endif
