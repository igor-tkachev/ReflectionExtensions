#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Globalization;
using System.Reflection;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	abstract class FakeMethodInfo : MethodInfo
	{
		protected FakeMethodInfo(PropertyInfo propertyInfo, MethodInfo pair)
		{
			Property      = propertyInfo;
			Pair          = pair;
			ReflectedType = propertyInfo.ReflectedType;
		}

		protected MethodInfo   Pair;
		protected PropertyInfo Property;

		public override MethodAttributes   Attributes        => Pair.Attributes;
		public override CallingConventions CallingConvention => Pair.CallingConvention;
		public override Type               DeclaringType     => Property.DeclaringType;
		public override MemberTypes        MemberType        => Property.MemberType;

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return Pair.GetMethodImplementationFlags();
		}

		class CustomAttributeProvider : ICustomAttributeProvider
		{
			static readonly object[] _object = new object[0];

			public object[] GetCustomAttributes(bool inherit)
			{
				return _object;
			}

			public object[] GetCustomAttributes(Type attributeType, bool inherit)
			{
				return _object;
			}

			public bool IsDefined(Type attributeType, bool inherit)
			{
				return false;
			}
		}

		static readonly CustomAttributeProvider _customAttributeProvider = new CustomAttributeProvider();

		public override ICustomAttributeProvider ReturnTypeCustomAttributes => _customAttributeProvider;

		public override ParameterInfo ReturnParameter => new FakeParameterInfo("ret", ReturnType, this, new object[0]);

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public override MethodInfo GetBaseDefinition()
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotImplementedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotImplementedException();
		}

		public override Type                ReflectedType { get; }
		public override RuntimeMethodHandle MethodHandle  { get; } = default;
	}
}

#endif
