#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

using System;
using System.Globalization;
using System.Reflection;

namespace ReflectionExtensions
{
	/// <Summary>
	/// Selects a member from a list of candidates, and performs type conversion
	/// from actual argument type to formal argument type.
	/// </Summary>
	[Serializable]
	class GenericBinder : Binder
	{
		readonly bool _genericMethodDefinition;

		public GenericBinder(bool genericMethodDefinition)
		{
			_genericMethodDefinition = genericMethodDefinition;
		}

		#region System.Reflection.Binder methods

		public override MethodBase BindToMethod(BindingFlags bindingAttr, MethodBase[] match, ref object[] args,
			ParameterModifier[] modifiers, CultureInfo culture, string[] names, out object state)
		{
			throw new InvalidOperationException("GenericBinder.BindToMethod");
		}

		public override FieldInfo BindToField(BindingFlags bindingAttr, FieldInfo[] match, object value, CultureInfo culture)
		{
			throw new InvalidOperationException("GenericBinder.BindToField");
		}

		public override MethodBase? SelectMethod(
			BindingFlags        bindingAttr,
			MethodBase[]        matchMethods,
			Type[]              parameterTypes,
			ParameterModifier[] modifiers)
		{
			foreach (var m in matchMethods)
			{
				if (m.IsGenericMethodDefinition != _genericMethodDefinition)
					continue;

				var pis   = m.GetParameters();
				var match = pis.Length == parameterTypes.Length;

				for (var j = 0; match && j < pis.Length; ++j)
				{
					match = CompareParameterTypes(pis[j].ParameterType, parameterTypes[j]);
				}

				if (match)
					return m;
			}

			return null;
		}

		static bool CompareParameterTypes(Type goal, Type probe)
		{
			if (goal == probe)
				return true;

			if (goal.IsGenericParameter)
				return CheckConstraints(goal, probe);
			if (goal.IsGenericType && probe.IsGenericType)
				return CompareGenericTypes(goal, probe);

			return false;
		}

		static bool CompareGenericTypes(Type goal, Type probe)
		{
			var  genArgs =  goal.GetGenericArguments();
			var specArgs = probe.GetGenericArguments();
			var match    = (genArgs.Length == specArgs.Length);

			for (var i = 0; match && i < genArgs.Length; i++)
			{
				if (genArgs[i] == specArgs[i])
					continue;

				if (genArgs[i].IsGenericParameter)
					match = CheckConstraints(genArgs[i], specArgs[i]);
				else if (genArgs[i].IsGenericType && specArgs[i].IsGenericType)
					match = CompareGenericTypes(genArgs[i], specArgs[i]);
				else
					match = false;
			}

			return match;
		}

		public static bool CheckConstraints(Type goal, Type probe)
		{
			var constraints = goal.GetGenericParameterConstraints();

			foreach (var c in constraints)
				if (!c.IsAssignableFrom(probe))
					return false;

			return true;
		}

		public override PropertyInfo SelectProperty(BindingFlags bindingAttr, PropertyInfo[] match, Type returnType,
			Type[] indexes, ParameterModifier[] modifiers)
		{
			throw new InvalidOperationException("GenericBinder.SelectProperty");
		}

		public override object ChangeType(object value, Type type, CultureInfo culture)
		{
			throw new InvalidOperationException("GenericBinder.ChangeType");
		}

		public override void ReorderArgumentArray(ref object[] args, object state)
		{
			throw new InvalidOperationException("GenericBinder.ReorderArgumentArray");
		}

		#endregion

		private static GenericBinder _generic;
		public  static GenericBinder  Generic => _generic ??= new GenericBinder(true);

		private static GenericBinder _nonGeneric;
		public  static GenericBinder  NonGeneric => _nonGeneric ??= new GenericBinder(false);
	}
}

#endif
