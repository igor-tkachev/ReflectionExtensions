#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

namespace ReflectionExtensions.Tests.TypeBuilder.Builders
{
	using ReflectionExtensions.Reflection;
	using ReflectionExtensions.Reflection.Emit;
	using ReflectionExtensions.TypeBuilder;
	using ReflectionExtensions.TypeBuilder.Builders;

	[TestFixture]
	public class DefaultTypeBuilderTest
	{
		public abstract class TestObject
		{
			public    abstract int       Int       { get; set; }
			public    abstract double    Double    { get; set; }
			public    abstract DateTime  DateTime  { get; set; }
			public    abstract ArrayList ArrayList { get; set; }

			protected abstract string this[int i]    { get; set; }
			protected abstract int    this[string i] { get; set; }

			protected abstract int    Method1(float f);
			public    abstract int    Method2(float f);
		}

		[Test]
		public void AbstractProperties()
		{
			TestObject o = (TestObject)TypeAccessor.CreateInstance(typeof(TestObject));

			o.Int    = 100; Assert.AreEqual(100, o.Int);
			o.Double = 200; Assert.AreEqual(200, o.Double);

			ArrayList list = new ArrayList();

			o.ArrayList = list;
			Assert.AreSame(list, o.ArrayList);
		}

		public class AbstractTypeBuilder : AbstractTypeBuilderBase
		{
			public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
			{
				return context.IsVirtualMethod && context.Step == BuildStep.After;
			}

			protected override void AfterBuildVirtualMethod()
			{
				Debug.Assert(Context.ReturnValue != null, "Context.ReturnValue != null");

				Context.MethodBuilder.Emitter
					.ldc_i4 (25)
					.stloc  (Context.ReturnValue);
			}

			protected override void BuildType()
			{
				ConstructorBuilderHelper cb = Context.TypeBuilder.DefaultConstructor;

				cb = Context.TypeBuilder.TypeInitializer;
				cb = Context.TypeBuilder.InitConstructor;
			}
		}

		public class TestTypeBuilderAttribute : AbstractTypeBuilderAttribute
		{
			public override IAbstractTypeBuilder TypeBuilder
			{
				get { return new AbstractTypeBuilder(); }
			}
		}

		[TestTypeBuilder]
		public abstract class VirtObject
		{
			public virtual int Foo(int i, ref int ii, DateTime d, string s)
			{
				ii = i;
				return 50;
			}
		}

		[Test]
		public void AbstractMethod()
		{
			VirtObject o = (VirtObject)TypeAccessor.CreateInstance(typeof(VirtObject));

			int i = 0;
			int r = o.Foo(10, ref i, DateTime.Now, "");

			Assert.AreEqual(10, i);
			Assert.AreEqual(25, r);
		}

		public abstract class DefCtorObject1
		{
			protected DefCtorObject1()
			{
				Value = 10;
			}

			public int Value;
		}

		[Test]
		public void DefCtorTest1()
		{
			DefCtorObject1 o = (DefCtorObject1)TypeAccessor.CreateInstance(typeof(DefCtorObject1));

			Assert.AreEqual(10, o.Value);
		}

		public abstract class DefCtorObject2
		{
			protected DefCtorObject2()
			{
				Value = 10;
			}

			[Parameter(20)]
			public abstract int Value { get; set; }

		}

		[Test]
		public void DefCtorTest2()
		{
			DefCtorObject2 o = (DefCtorObject2)TypeAccessor.CreateInstance(typeof(DefCtorObject2));

			Assert.AreEqual(10, o.Value);
		}

		public abstract class InitCtorObject1
		{
			protected InitCtorObject1(InitContext init)
			{
				Value = 10;
			}

			public int Value;
		}

		[Test]
		public void InitCtorTest1()
		{
			InitCtorObject1 o = (InitCtorObject1)TypeAccessor.CreateInstance(typeof(InitCtorObject1));

			Assert.AreEqual(10, o.Value);
		}

		public abstract class InitCtorObject2
		{
			protected InitCtorObject2(InitContext init)
			{
				Value = 10;
			}

			[Parameter(20)]
			public abstract int Value { get; set; }

			public abstract InitCtorObject1 InitCtor { get; set; }

		}

		[Test]
		public void InitCtorTest2()
		{
			InitCtorObject2 o = (InitCtorObject2)TypeAccessor.CreateInstance(typeof(InitCtorObject2));

			Assert.AreEqual(10, o.Value);
		}

		public abstract class TestObject2
		{
			public abstract int  Int1 { get; set; }
			public abstract int? Int2 { get; set; }

			private int _Int3;
			public  virtual int  Int3 { get { return _Int3; } set { _Int3 = value; } }

			private int? _Int4;
			public virtual int? Int4
			{
				get { return _Int4;  }
				set { _Int4 = value; }
			}
		}

		[Test]
		public void Test()
		{
			TestObject2 o = TypeAccessor<TestObject2>.Instance.Create();

			int  i  = o.Int1;
			int? i2 = o.Int2;

			o.Int1 = 5;
			o.Int2 = 6;
		}
	}
}

#endif
