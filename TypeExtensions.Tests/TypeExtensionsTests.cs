using System;
using System.Collections;
using System.Collections.Generic;

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

		public sealed class SealedTest {}
		abstract class AbstractTest {}
		struct StructTest {}
		enum EnumTest {}
		class NonAbstractTest : AbstractTest {}

		[Test]
		public void IsAbstractExTest()
		{
			Assert.IsTrue (typeof(AbstractTest).IsAbstractEx());
			Assert.IsFalse(typeof(SealedTest).  IsAbstractEx());
		}

		[Test]
		public void IsSealedExTest()
		{
			Assert.IsTrue (typeof(SealedTest).  IsSealedEx());
			Assert.IsFalse(typeof(AbstractTest).IsSealedEx());
		}

		[Test]
		public void IsClassExTest()
		{
			Assert.IsTrue (typeof(SealedTest).IsClassEx());
			Assert.IsFalse(typeof(StructTest).IsClassEx());
		}

		[Test]
		public void IsEnumExTest()
		{
			Assert.IsTrue (typeof(EnumTest).  IsEnumEx());
			Assert.IsFalse(typeof(StructTest).IsEnumEx());
		}

		[Test]
		public void IsPrimitiveExTest()
		{
			Assert.IsTrue (typeof(int).     IsPrimitiveEx());
			Assert.IsFalse(typeof(EnumTest).IsPrimitiveEx());
		}

		[Test]
		public void IsPublicExTest()
		{
			Assert.IsTrue (typeof(int).     IsPublicEx());
			Assert.IsFalse(typeof(EnumTest).IsPublicEx());
		}

		[Test]
		public void IsNestedPublicExTest()
		{
			Assert.IsTrue (typeof(SealedTest).IsNestedPublicEx());
			Assert.IsFalse(typeof(EnumTest).  IsNestedPublicEx());
			Assert.IsFalse(typeof(int).       IsNestedPublicEx());
		}

		[Test]
		public void IsGenericTypeExTest()
		{
			Assert.IsTrue (typeof(List<>).   IsGenericTypeEx());
			Assert.IsTrue (typeof(List<int>).IsGenericTypeEx());
			Assert.IsFalse(typeof(EnumTest). IsGenericTypeEx());
		}

		[Test]
		public void IsGenericTypeDefinitionExTest()
		{
			Assert.IsTrue (typeof(List<>).   IsGenericTypeDefinitionEx());
			Assert.IsFalse(typeof(List<int>).IsGenericTypeDefinitionEx());
			Assert.IsFalse(typeof(EnumTest). IsGenericTypeDefinitionEx());
		}

		[Test]
		public void IsInterfaceExTest()
		{
			Assert.IsTrue (typeof(IList<>).  IsInterfaceEx());
			Assert.IsFalse(typeof(List<int>).IsInterfaceEx());
		}

		[Test]
		public void BaseTypeExTest()
		{
			Assert.That(typeof(NonAbstractTest).BaseTypeEx(), Is.    EqualTo(typeof(AbstractTest)));
			Assert.That(typeof(AbstractTest).   BaseTypeEx(), Is.Not.EqualTo(typeof(NonAbstractTest)));
		}

		[Test]
		public void IsValueTypeExTest()
		{
			Assert.IsTrue (typeof(DateTime).IsValueTypeEx());
			Assert.IsFalse(typeof(string).  IsValueTypeEx());
		}

		[Test]
		public void IsArrayExTest()
		{
			Assert.IsTrue (typeof(int[]).IsArrayEx());
			Assert.IsFalse(typeof(int).  IsArrayEx());
		}

		[Test]
		public void ContainsGenericParametersExTest()
		{
			Assert.IsTrue (typeof(List<>).   ContainsGenericParametersEx());
			Assert.IsFalse(typeof(List<int>).ContainsGenericParametersEx());
			Assert.IsFalse(typeof(int).      ContainsGenericParametersEx());
		}

		[Test]
		public void IsAssignableFromExTest()
		{
			Assert.IsTrue (typeof(List<int>).IsAssignableFromEx(typeof(List<int>)));
			Assert.IsFalse(typeof(List<int>).IsAssignableFromEx(typeof(IList<>)));
		}

		[Test]
		public void IsSubclassOfExTest()
		{
			Assert.IsTrue (typeof(NonAbstractTest).IsSubclassOfEx(typeof(AbstractTest)));
			Assert.IsFalse(typeof(AbstractTest).   IsSubclassOfEx(typeof(NonAbstractTest)));
		}

		[Test]
		public void IsDefinedExTest()
		{
			Assert.IsTrue (typeof(TypeExtensionsTests).IsDefinedEx(typeof(TestFixtureAttribute)));
			Assert.IsFalse(typeof(TypeExtensionsTests).IsDefinedEx(typeof(TestAttribute)));

			Assert.IsTrue (typeof(TypeExtensionsTests).IsDefinedEx(typeof(TestFixtureAttribute), true));
			Assert.IsFalse(typeof(TypeExtensionsTests).IsDefinedEx(typeof(TestAttribute),        true));
		}

		[Test]
		public void GetInterfaceMapExTest()
		{
			var map = typeof(List<int>).GetInterfaceMapEx(typeof(IList));
			Assert.That(map.InterfaceMethods, Is.Not.Empty);
		}

		[Test]
		public void GetPropertyExTest()
		{
			var info = typeof(List<int>).GetPropertyEx(nameof(List<int>.Count));
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributeExTest1()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributeEx<TestFixtureAttribute>();
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributeExTest2()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributeEx<TestFixtureAttribute>(true);
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributeExTest3()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributeEx(typeof(TestFixtureAttribute));
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributeExTest4()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributeEx(typeof(TestFixtureAttribute), true);
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributesExTest1()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributesEx();
			Assert.That(info, Is.Not.Null);
		}

		[Test]
		public void GetCustomAttributesExTest2()
		{
			var info = typeof(TypeExtensionsTests).GetCustomAttributesEx(typeof(TestFixtureAttribute), true);
			Assert.That(info, Is.Not.Null);
		}
	}
}
