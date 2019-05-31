#if !NETCOREAPP1_0

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.Reflection
{
	using ReflectionExtensions.Reflection;

	[TestFixture]
	public class TypeAccessorTest
	{
		public class TestObject1
		{
			public int    IntField = 10;
			public string StrField = "10";

			public int    IntProperty
			{
				get => IntField * 2;
				set => IntField = value / 2;
			}
			public string StrProperty { get { return StrField + "2"; } }

			public int    SetProperty { set {} }

			protected int ProtectedProperty
			{
				get => IntField * 2;
				set => IntField = value / 2;
			}

			public int    ProtectedSetter
			{
				          get => IntField;
				protected set => IntField = value;
			}
		}

		[Test]
		public void HasGetter()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject1));

			Assert.IsTrue (ta["IntField"].   HasGetter);
			Assert.IsTrue (ta["IntProperty"].HasGetter);
			Assert.IsTrue (ta["StrField"].   HasGetter);
			Assert.IsTrue (ta["StrProperty"].HasGetter);

			Assert.IsFalse(ta["SetProperty"].HasGetter);
			Assert.IsFalse(ta.HasMember("ProtectedProperty"));
			Assert.IsTrue (ta["ProtectedSetter"].HasGetter);
		}

		[Test]
		public void HasSetter()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject1));

			Assert.IsTrue (ta["IntField"].   HasSetter);
			Assert.IsTrue (ta["IntProperty"].HasSetter);
			Assert.IsTrue (ta["StrField"].   HasSetter);
			Assert.IsFalse(ta["StrProperty"].HasSetter);

			Assert.IsTrue (ta["SetProperty"].HasSetter);
			Assert.IsFalse(ta.HasMember("ProtectedProperty"));
			Assert.IsTrue (ta["ProtectedSetter"].HasSetter);
		}

		[Test]
		public void GetValue()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject1));
			var o  = (TestObject1)ta.CreateInstance();

			Assert.AreEqual(10,    ta["IntField"].   GetValue(o));
			Assert.AreEqual(20,    ta["IntProperty"].GetValue(o));
			Assert.AreEqual("10",  ta["StrField"].   GetValue(o));
			Assert.AreEqual("102", ta["StrProperty"].GetValue(o));
		}

		public abstract class TestObject2
		{
			public int IntField = 10;

			public abstract int    SetProperty {      set; }
			public abstract int    IntProperty { get; set; }
			public abstract string StrProperty { get; set; }
			public abstract int    GetProperty { get;      }

			protected int          ProtField;
			protected int          ProtProperty1 { get { return 0; } }
			protected abstract int ProtProperty2 { get; set; }
		}

		[Test]
		public void HasAbstractGetter()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject2));

			Assert.IsTrue(ta["IntField"].     HasGetter);
			Assert.IsTrue(ta["IntProperty"].  HasGetter);
			Assert.IsTrue(ta["StrProperty"].  HasGetter);
			Assert.IsTrue(ta["SetProperty"].  HasGetter);
			Assert.IsTrue(ta["ProtProperty2"].HasGetter);
		}

		[Test]
		public void GetAbstractValue()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject2));
			var o  = (TestObject2)ta.CreateInstance();

			o.IntProperty = 20;
			o.StrProperty = "10";
			o.SetProperty = 30;

			Assert.AreEqual(30,   ta["SetProperty"].GetValue(o));
			Assert.AreEqual(20,   ta["IntProperty"].GetValue(o));
			Assert.AreEqual("10", ta["StrProperty"].GetValue(o));
		}

		[Test]
		public void HasAbstractSetter()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject2));

			Assert.IsTrue(ta["IntField"].     HasSetter);
			Assert.IsTrue(ta["IntProperty"].  HasSetter);
			Assert.IsTrue(ta["StrProperty"].  HasSetter);
			Assert.IsTrue(ta["GetProperty"].  HasSetter);
			Assert.IsTrue(ta["ProtProperty2"].HasSetter);
		}

		[Test]
		public void SetAbstractValue()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject2));
			var o  = (TestObject2)ta.CreateInstance();

			ta["IntField"].     SetValue(o, 10);
			ta["IntProperty"].  SetValue(o, 20);
			ta["StrProperty"].  SetValue(o, "30");
			ta["GetProperty"].  SetValue(o, 40);
			ta["ProtProperty2"].SetValue(o, 50);

			Assert.AreEqual(10,   ta["IntField"].     GetValue(o));
			Assert.AreEqual(20,   ta["IntProperty"].  GetValue(o));
			Assert.AreEqual("30", ta["StrProperty"].  GetValue(o));
			Assert.AreEqual(40,   ta["GetProperty"].  GetValue(o));
			Assert.AreEqual(50,   ta["ProtProperty2"].GetValue(o));
		}

		[Test]
		public void ProtectedMembers()
		{
			var ta = TypeAccessor.GetAccessor(typeof(TestObject2));

			Assert.IsFalse  (ta.HasMember("ProtField"));
			Assert.IsFalse  (ta.HasMember("ProtProperty1"));
			Assert.IsNotNull(ta["ProtProperty2"]);
		}

		public class TestObject3
		{
			public int        IntField    = 10;
			public string     StringField = "256";
			public ArrayList  ListField   = new ArrayList();
			public ArrayList? NullField   = null;
		}

		[Test]
		public void Write()
		{
			var o = new TestObject3();

			Console.WriteLine(o);
			Debug.  WriteLine(o);
		}

		[Test]
		public void AccessMember()
		{
			var ta  = TypeAccessor.GetAccessor<TestObject3>();
			var ma  = ta["IntField"];
			var obj = new TestObject3();
			var val = (int)(ma.GetValue(obj) ?? 0);

			Assert.AreEqual(obj.IntField, val);
		}

		static class MemberAccessors<TObject,TValue>
		{
			public static readonly Dictionary<string,Func<TObject,TValue>> Accessor = new Dictionary<string,Func<TObject,TValue>>();
		}

#if !NET20 && !NET30

		static Func<TObject,TValue> GetAccessor<TObject,TValue>(string name)
		{
			if (!MemberAccessors<TObject,TValue>.Accessor.TryGetValue(name, out var func))
			{
				var param = System.Linq.Expressions.Expression.Parameter(typeof(TObject), "obj");

				func = System.Linq.Expressions.Expression.Lambda<Func<TObject,TValue>>(
					System.Linq.Expressions.Expression.PropertyOrField(param, name), param).Compile();

				MemberAccessors<TObject,TValue>.Accessor.Add(name, func);
			}

			return func;
		}

		[Test]
		public void AccessMember2()
		{
			var accessor = GetAccessor<TestObject3, int>("IntField");

			var obj = new TestObject3();

			Assert.AreEqual(obj.IntField, accessor(obj));
		}

#endif

		[Test]
		public void PointTest()
		{
			var p = new Point();

			foreach (var memberAccessor in TypeAccessor.GetAccessor(typeof(Point)).Members)
			{
				memberAccessor.GetValue(p);
			}
		}

		public object GetValue(object o)
		{
			return ((Point)o).X;
		}
	}
}

#endif
