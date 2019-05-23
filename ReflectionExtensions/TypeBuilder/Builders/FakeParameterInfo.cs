#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections;
using System.Reflection;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	class FakeParameterInfo : ParameterInfo
	{
		public FakeParameterInfo(string name, Type type, MemberInfo memberInfo, object[] attributes)
		{
			Name       = name;
			ParameterType       = type;
			Member = memberInfo;
			_attributes = attributes ?? new object[0];
		}

		public FakeParameterInfo(MethodInfo method) : this(
			"ret",
			method.ReturnType,
			method,
			method.ReturnTypeCustomAttributes.GetCustomAttributes(true))
		{
		}

		public override ParameterAttributes Attributes   => ParameterAttributes.Retval;
		public override object              DefaultValue => DBNull.Value;

		readonly object[] _attributes;

		public override object[] GetCustomAttributes(bool inherit)
		{
			return _attributes;
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));

			if (_attributes.Length == 0)
				return (object[]) Array.CreateInstance(attributeType, 0);

			var list = new ArrayList();

			foreach (var o in _attributes)
				if (o.GetType() == attributeType || attributeType.IsInstanceOfType(o))
					list.Add(o);

			return (object[]) list.ToArray(attributeType);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null) throw new ArgumentNullException(nameof(attributeType));

			foreach (object o in _attributes)
				if (o.GetType() == attributeType || attributeType.IsInstanceOfType(o))
					return true;

			return false;
		}

		public override MemberInfo Member        { get; }
		public override string     Name          { get; }
		public override Type       ParameterType { get; }
		public override int        Position => 0;
	}
}

#endif
