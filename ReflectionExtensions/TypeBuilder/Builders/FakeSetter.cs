#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	class FakeSetter : FakeMethodInfo
	{
		public FakeSetter(PropertyInfo propertyInfo)
			: base(propertyInfo, propertyInfo.GetGetMethodEx(true))
		{
		}

		public override ParameterInfo[] GetParameters()
		{
			var index = Property.GetIndexParameters();
			var pi    = new ParameterInfo[index.Length + 1];

			index.CopyTo(pi, 0);
			pi[index.Length] = new FakeParameterInfo("value", Property.PropertyType, Property, new object[0]);

			return pi;
		}

		public override string Name       => $"set_{Property.Name}";
		public override Type   ReturnType => typeof(void);
	}
}

#endif
