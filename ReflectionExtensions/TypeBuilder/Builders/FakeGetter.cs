#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	class FakeGetter : FakeMethodInfo
	{
		public FakeGetter(PropertyInfo propertyInfo)
			: base(propertyInfo, propertyInfo.GetSetMethod(true))
		{
		}

		public override ParameterInfo[] GetParameters()
		{
			return Property.GetIndexParameters();
		}

		public override string Name       => $"get_{Property.Name}";
		public override Type   ReturnType => Property.PropertyType;
	}
}

#endif
