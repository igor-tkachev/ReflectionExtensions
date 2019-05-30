using System;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.TypeBuilder.Builders
{
	using ReflectionExtensions.Reflection;
	using ReflectionExtensions.TypeBuilder;

	[TestFixture]
	public class AutoImplementInterfaceTest
	{
		#region TestException

		public interface Test2
		{
			string Name { get; }
		}

		[Test]
		public void TestException()
		{
			Assert.Throws<TypeBuilderException>(() =>
			{
				var ta = TypeAccessor.GetAccessor(typeof(Test2));
				var t  = (Test2)ta.CreateInstance();
			});
		}

		#endregion

		#region TestMemberImpl

		[AutoImplementInterface]
		public interface Test3
		{
			string Name { get; set; }
		}

		[AutoImplementInterface]
		public interface Test4
		{
			Test3 Test { get; }
		}

#if !NETCOREAPP1_0

		[Test]
		public void TestMemberImpl()
		{
			var t = TypeAccessor<Test4>.Instance.Create();

			t.Test.Name = "John";

			Assert.AreEqual("John", t.Test.Name);
		}

#endif

		#endregion

		#region Inheritance

		public interface InterfaceBase
		{
			string? Name { get; set; }
		}

		public interface Interface1 : InterfaceBase
		{
			void Foo();
		}

		[AutoImplementInterface]
		public interface Interface2 : Interface1, InterfaceBase
		{
			void Bar();
		}

#if !NETCOREAPP1_0

		[Test]
		public void TestInheritance()
		{
			Interface2 i2 = TypeAccessor<Interface2>.Instance.Create();
			Interface1 i1 = i2;

			i1.Foo();
			i2.Foo();
			i2.Bar();

			i1.Name = "John";

			Assert.AreEqual("John", i1.Name);
			Assert.AreEqual("John", i2.Name);
		}

#endif

		#endregion

		#region AssociateTypeTest

		public class MyClass : Interface1
		{
			public void Foo() {}

			public string? Name { get; set; }

			public string? Address;
		}

		[Test]
		public void AssociateTypeTest()
		{
			TypeAccessor.AssociateType(typeof(Interface1), typeof(MyClass));

			Interface1 i1 = TypeAccessor<Interface1>.Instance.Create();

			i1.Name = "John";

			Assert.AreEqual("John", i1.Name);
			Assert.AreEqual("John", TypeAccessor<Interface1>.Instance[nameof(Interface1.Name)].GetValue(i1)?.ToString());
		}

		#endregion

		#region AssociateTypeHandlerTest

		public interface IMy
		{
			string? Name { get; set; }
		}

		public class MyImpl : IMy
		{
			public string? Name { get; set; }
		}


		[Test]
		public void AssociateTypeHandlerTest()
		{
			TypeAccessor.AssociatedTypeHandler += parent => parent == typeof(IMy) ? typeof(MyImpl) : null;

			var i = TypeAccessor<IMy>.Instance.Create();

			i.Name = "John";

			Assert.AreEqual("John", i.Name);
			Assert.AreEqual("John", TypeAccessor<IMy>.Instance[nameof(IMy.Name)].GetValue(i)?.ToString());
		}

		#endregion
	}
}
