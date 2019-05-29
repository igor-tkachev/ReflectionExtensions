

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	class InstanceTypeBuilder : DefaultTypeBuilder
	{
		public InstanceTypeBuilder(Type instanceType, bool isObjectHolder)
		{
			InstanceType   = instanceType;
			_isObjectHolder = isObjectHolder;
		}

		public InstanceTypeBuilder(Type propertyType, Type instanceType, bool isObjectHolder)
		{
			PropertyType    = propertyType;
			InstanceType    = instanceType;
			_isObjectHolder = isObjectHolder;
		}

		readonly bool _isObjectHolder;

		public Type? PropertyType { get; }
		public Type  InstanceType { get; }

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			return
				base.IsApplied(context, builders) &&
				context.CurrentProperty != null &&
				context.CurrentProperty.GetIndexParameters().Length == 0 &&
				(PropertyType == null || PropertyType.IsSameOrParentOf(context.CurrentProperty.PropertyType));
		}

		protected override Type GetFieldType()
		{
			return InstanceType;
		}

		protected override Type GetObjectType()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");
			return IsObjectHolder? Context.CurrentProperty.PropertyType: base.GetObjectType();
		}

		protected override bool IsObjectHolder
		{
			get
			{
				Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");
				return _isObjectHolder && Context.CurrentProperty.PropertyType.IsClass;
			}
		}

		protected override void BuildAbstractGetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var field        = GetField();
			var emit         = Context.MethodBuilder.Emitter;
			var propertyType = Context.CurrentProperty.PropertyType;
			var getter       = GetGetter();

			if (InstanceType.IsValueType) emit.ldarg_0.ldflda(field);
			else                          emit.ldarg_0.ldfld (field);

			Type memberType;

			if (getter is PropertyInfo pri)
			{
				if (InstanceType.IsValueType) emit.call    (pri.GetGetMethod());
				else                          emit.callvirt(pri.GetGetMethod());

				memberType = pri.PropertyType;
			}
			else if (getter is FieldInfo fi)
			{
				emit.ldfld(fi);

				memberType = fi.FieldType;
			}
			else
			{
				var mi = (MethodInfo)getter;
				var pi = mi.GetParameters();

				foreach (var p in pi)
				{
					if (p.IsDefined(typeof(ParentAttribute), true))
					{
						// Parent - set this.
						//
						emit.ldarg_0.end();

						if (!p.ParameterType.IsSameOrParentOf(Context.Type))
							emit.castclass(p.ParameterType);
					}
					else if (p.IsDefined(typeof(PropertyInfoAttribute), true))
					{
						// PropertyInfo.
						//
						emit.ldsfld(GetPropertyInfoField()).end();
					}
					else
						throw new TypeBuilderException(string.Format(
							"The method '{0}' of '{1}' has parameter '{2}' which can't be handled. Please specify attrbutes [Parent] or [PropertyInfo] to get access to them.",
							mi.Name, mi.DeclaringType?.FullName, p.Name));
				}

				if (InstanceType.IsValueType) emit.call    (mi);
				else                          emit.callvirt(mi);

				memberType = mi.ReturnType;
			}

			if (propertyType.IsValueType)
			{
				if (memberType.IsValueType == false)
					emit.CastFromObject(propertyType);
			}
			else
			{
				if (memberType != propertyType)
					emit.castclass(propertyType);
			}

			Debug.Assert(Context.ReturnValue != null, "Context.ReturnValue != null");

			emit.stloc(Context.ReturnValue);
		}

		protected override void BuildAbstractSetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			var field        = GetField();
			var emit         = Context.MethodBuilder.Emitter;
			var propertyType = Context.CurrentProperty.PropertyType;
			var setter       = GetSetter();

			if (InstanceType.IsValueType) emit.ldarg_0.ldflda(field);
			else                          emit.ldarg_0.ldfld (field);

			if (setter is PropertyInfo pri)
			{
				emit.ldarg_1.end();

				if (propertyType.IsValueType && !pri.PropertyType.IsValueType)
					emit.box(propertyType);

				if (InstanceType.IsValueType) emit.call    (pri.GetSetMethod());
				else                          emit.callvirt(pri.GetSetMethod());
			}
			else if (setter is FieldInfo fi)
			{
				emit.ldarg_1.end();

				if (propertyType.IsValueType && !fi.FieldType.IsValueType)
					emit.box(propertyType);

				emit.stfld(fi);
			}
			else
			{
				var mi            = (MethodInfo)setter;
				var pi            = mi.GetParameters();
				var gotValueParam = false;

				foreach (var p in pi)
				{
					if (p.IsDefined(typeof (ParentAttribute), true))
					{
						// Parent - set this.
						//
						emit.ldarg_0.end();

						if (!p.ParameterType.IsSameOrParentOf(Context.Type))
							emit.castclass(p.ParameterType);
					}
					else if (p.IsDefined(typeof (PropertyInfoAttribute), true))
					{
						// PropertyInfo.
						//
						emit.ldsfld(GetPropertyInfoField()).end();
					}
					else if (!gotValueParam)
					{
						// This must be a value.
						//
						emit.ldarg_1.end();

						if (propertyType.IsValueType && !p.ParameterType.IsValueType)
							emit.box(propertyType);

						gotValueParam = true;
					}
					else
						throw new TypeBuilderException(string.Format(
							"The method '{0}' of '{1}' has parameter '{2}' which can't be handled. Please specify attrbutes [Parent] or [PropertyInfo] to get access to them.",
							mi.Name, mi.DeclaringType?.FullName, p.Name));
				}

				if (InstanceType.IsValueType) emit.call(mi);
				else                          emit.callvirt(mi);
			}
		}

		MemberInfo GetGetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

			var propertyType = Context.CurrentProperty.PropertyType;
			var fields       = InstanceType.GetFields(bindingFlags);

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(typeof(GetValueAttribute), true);

				if (attrs.Length > 0 && field.FieldType.IsSameOrParentOf(propertyType))
					return field;
			}

			var props = InstanceType.GetProperties(bindingFlags);

			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(typeof(GetValueAttribute), true);

				if (attrs.Length > 0 && prop.PropertyType.IsSameOrParentOf(propertyType))
					return prop;
			}

			foreach (var field in fields)
				if (field.Name == "Value" && field.FieldType.IsSameOrParentOf(propertyType))
					return field;

			foreach (var prop in props)
				if (prop.Name == "Value" && prop.PropertyType.IsSameOrParentOf(propertyType))
					return prop;

			var method = InstanceType.GetMethod(false, "GetValue", bindingFlags);

			if (method != null && propertyType.IsSameOrParentOf(method.ReturnType))
				return method;

			throw new TypeBuilderException(string.Format(
				"The '{0}' type does not have appropriate getter. See '{1}' member '{2}' of '{3}' type.",
				InstanceType.FullName, propertyType.FullName, Context.CurrentProperty.Name, Context.Type.FullName));
		}

		MemberInfo GetSetter()
		{
			Debug.Assert(Context.CurrentProperty != null, "Context.CurrentProperty != null");

			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;

			var propertyType = Context.CurrentProperty.PropertyType;
			var fields       = InstanceType.GetFields(bindingFlags);

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(typeof(SetValueAttribute), true);

				if (attrs.Length > 0 && field.FieldType.IsSameOrParentOf(propertyType))
					return field;
			}

			var props = InstanceType.GetProperties(bindingFlags);

			foreach (var prop in props)
			{
				var attrs = prop.GetCustomAttributes(typeof(SetValueAttribute), true);

				if (attrs.Length > 0 && prop.PropertyType.IsSameOrParentOf(propertyType))
					return prop;
			}

			foreach (var field in fields)
				if (field.Name == "Value" && field.FieldType.IsSameOrParentOf(propertyType))
					return field;

			foreach (var prop in props)
				if (prop.Name == "Value" && prop.PropertyType.IsSameOrParentOf(propertyType))
					return prop;

			var method = InstanceType.GetMethod(false, "SetValue", bindingFlags);

			if (method != null && method.ReturnType == typeof(void))
				return method;

			throw new TypeBuilderException(string.Format(
				"The '{0}' type does not have appropriate setter. See '{1}' member '{2}' of '{3}' type.",
				InstanceType.FullName, propertyType.FullName, Context.CurrentProperty.Name, Context.Type.FullName));
		}
	}
}

#endif
