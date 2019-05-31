#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionExtensions.Aspects.Builders
{
	using Reflection.Emit;
	using TypeBuilder;
	using TypeBuilder.Builders;

	public class MixinAspectBuilder : AbstractTypeBuilderBase
	{
		public MixinAspectBuilder(
			Type targetInterface, string memberName, bool throwExceptionIfNull, string? exceptionMessage)
		{
			_targetInterface      = targetInterface;
			_memberName           = memberName;
			_throwExceptionIfNull = throwExceptionIfNull;
			_exceptionMessage     = exceptionMessage;
		}

		readonly Type    _targetInterface;
		readonly string  _memberName;
		readonly bool    _throwExceptionIfNull;
		readonly string? _exceptionMessage;

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			return context.BuildElement == BuildElement.InterfaceMethod;
		}

		public override Type[] GetInterfaces()
		{
			return new Type[] { _targetInterface };
		}

		public override void Build(BuildContext context)
		{
			Context = context;

			if (CheckOverrideAttribute())
				return;

			var emit   = Context.MethodBuilder.Emitter;
			var method = Context.MethodBuilder.OverriddenMethod;
			var ps     = method.GetParameters();
			var field  = Context.Type.GetField(_memberName);


			Type memberType;

			if (field != null)
			{
				if (field.IsPrivate)
					throw new TypeBuilderException($"Field '{Context.Type.Name}.{_memberName}' must be protected or public.");

				memberType = field.FieldType;

				emit
					.ldarg_0
					.ldfld   (field)
					;

				CheckNull(emit);

				emit
					.ldarg_0
					.ldfld   (field)
					;
			}
			else
			{
				var prop = Context.Type.GetProperty(_memberName);

				if (prop != null)
				{
					MethodInfo mi = prop.GetGetMethod(true);

					if (mi == null)
						throw new TypeBuilderException(string.Format(
							"Property '{0}.{1}' getter not found.",
							Context.Type.Name, _memberName));

					memberType = prop.PropertyType;

					if (mi.IsPrivate)
						throw new TypeBuilderException(string.Format(
							"Property '{0}.{1}' getter must be protected or public.",
							Context.Type.Name, _memberName));

					emit
						.ldarg_0
						.callvirt (mi)
						;

					CheckNull(emit);

					emit
						.ldarg_0
						.callvirt (mi)
						;
				}
				else
				{
					throw new TypeBuilderException(string.Format(
						"Member '{0}.{1}' not found.",
						Context.Type.Name, _memberName));
				}
			}

			emit.CastIfNecessary(_targetInterface, memberType);

			for (int i = 0; i < ps.Length; i++)
				emit.ldarg(i + 1);

			emit.callvirt(method);

			if (Context.ReturnValue != null)
				emit.stloc(Context.ReturnValue);
		}

		private void CheckNull(EmitHelper emit)
		{
			if (_throwExceptionIfNull == false && string.IsNullOrEmpty(_exceptionMessage))
			{
				emit
					.brfalse (Context.ReturnLabel)
					;
			}
			else
			{
				string message = string.Format(
					string.IsNullOrEmpty(_exceptionMessage)?
						"'{0}.{1}' is not initialized." : _exceptionMessage,
					_targetInterface.Name, _memberName, _targetInterface.FullName);

				Label label = emit.DefineLabel();

				emit
					.brtrue    (label)
					.ldstr     (message)
					.newobj    (typeof(InvalidOperationException), typeof(string))
					.@throw
					.MarkLabel (label)
					;
			}
		}

		bool CheckOverrideAttribute()
		{
			var method  = Context.MethodBuilder.OverriddenMethod;
			var ps      = method.GetParameters();
			var methods = Context.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

			foreach (var mi in methods)
			{
				if (mi.IsPrivate)
					continue;

				var attrs = mi.GetCustomAttributes(typeof(MixinOverrideAttribute), true);

				if (attrs == null || attrs.Length == 0)
					continue;

				foreach (MixinOverrideAttribute attr in attrs)
				{
					if (attr.TargetInterface != null &&
						attr.TargetInterface != Context.CurrentInterface)
						continue;

					string name = string.IsNullOrEmpty(attr.MethodName)?
						mi.Name: attr.MethodName;

					if (name != method.Name || mi.ReturnType != method.ReturnType)
						continue;

					var mips = mi.GetParameters();

					if (mips.Length != ps.Length)
						continue;

					var equal = true;

					for (var i = 0; equal && i < ps.Length; i++)
						equal = ps[i].ParameterType == mips[i].ParameterType;

					if (equal)
					{
						var emit = Context.MethodBuilder.Emitter;

						for (int i = -1; i < ps.Length; i++)
							emit.ldarg(i + 1);

						emit.callvirt(mi);

						if (Context.ReturnValue != null)
							emit.stloc(Context.ReturnValue);

						return true;
					}
				}
			}

			return false;
		}
	}
}

#endif
