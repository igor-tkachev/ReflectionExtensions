#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ReflectionExtensions.Aspects.Builders
{
	using Reflection;
	using TypeBuilder;
	using TypeBuilder.Builders;

	public class OverloadAspectBuilder: AbstractTypeBuilderBase
	{
		readonly string? _overloadedMethodName;
		readonly Type[]? _parameterTypes;

		public OverloadAspectBuilder(string? overloadedMethodName, Type[]? parameterTypes)
		{
			_overloadedMethodName = overloadedMethodName;
			_parameterTypes       = parameterTypes;
		}

		public override int GetPriority(BuildContext context)
		{
			return TypeBuilderConsts.Priority.OverloadAspect;
		}

		public override bool IsCompatible(BuildContext context, IAbstractTypeBuilder typeBuilder)
		{
			if (context.IsBuildStep)
				return false;

			var list = new List<IAbstractTypeBuilder>(2) { this, typeBuilder };
			var step = context.Step;

			try
			{
				context.Step = BuildStep.Build;

				return typeBuilder.IsApplied(context, list);
			}
			finally
			{
				context.Step = step;
			}
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return context.IsBuildStep && context.BuildElement == BuildElement.AbstractMethod;
		}

		protected override void BuildAbstractMethod()
		{
			var currentMethod    = Context.CurrentMethod;
			var methodName       = _overloadedMethodName ?? currentMethod.Name;
			var overloadedMethod = GetOverloadedMethod(methodName);

			if (overloadedMethod == null)
			{
				throw new TypeBuilderException(
					$"Can not figure out the overloaded method for the method '{Context.Type.FullName}.{methodName}'.");
			}

			var emit       = Context.MethodBuilder.Emitter;
			var parameters = new List<ParameterInfo>(currentMethod.GetParameters());

			if (!overloadedMethod.IsStatic)
				emit.ldarg_0.end();

			foreach (var param in overloadedMethod.GetParameters())
			{
				ParameterInfo? currentMethodParameter = null;

				foreach (var p in parameters)
				{
					if (p.Name != param.Name)
						continue;

					currentMethodParameter = p;
					parameters.Remove(p);
					break;
				}

				if (currentMethodParameter != null)
				{
					emit.ldarg(currentMethodParameter);
				}
				else
				{
					var type  = param.ParameterType;
					var isRef = false;

					if (type.IsByRef)
					{
						type  = type.GetElementType();
						isRef = true;
					}

					if (type.IsValueType && !type.IsPrimitive)
					{
						var localBuilder = emit.DeclareLocal(type);

						emit
							.ldloca      (localBuilder)
							.initobj     (type)
							;

						if (isRef)
							emit.ldloca  (localBuilder);
						else
							emit.ldloc   (localBuilder);

					}
					else
					{
						if ((param.Attributes & ParameterAttributes.HasDefault) == 0 ||
							!emit.LoadWellKnownValue(param.DefaultValue))
						{
							emit.LoadInitValue(type);
						}

						if (isRef)
						{
							var localBuilder = emit.DeclareLocal(type);

							emit
								.stloc   (localBuilder)
								.ldloca  (localBuilder)
								;
						}
					}
				}
			}

			// Finally, call the method we override.
			//
			if (overloadedMethod.IsStatic || overloadedMethod.IsFinal)
				emit.call      (overloadedMethod);
			else
				emit.callvirt  (overloadedMethod);

			if (currentMethod.ReturnType != typeof(void))
			{
				Debug.Assert(Context.ReturnValue != null, "Context.ReturnValue != null");
				emit.stloc(Context.ReturnValue);
			}
		}

		MethodInfo? GetOverloadedMethod(string methodName)
		{
			var         currentMethod            = Context.CurrentMethod;
			MethodInfo? bestMatch                = null;
			var         bestMatchParametersCount = -1;
			var         currentMethodParameters  = currentMethod.GetParameters();

			if (_parameterTypes != null)
			{
				bestMatch = Context.Type.GetMethod(methodName, _parameterTypes);
				return bestMatch != null && MatchParameters(currentMethodParameters, bestMatch.GetParameters()) >= 0? bestMatch: null;
			}

			const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Instance| BindingFlags.Public | BindingFlags.NonPublic;

			foreach (var m in Context.Type.GetMethods(bindingFlags))
			{
				if (m.IsPrivate || m.Name != methodName || m.IsGenericMethod != currentMethod.IsGenericMethod)
					continue;

				if (!GenericBinder.CompareParameterTypes(m.ReturnType, currentMethod.ReturnType))
					continue;

				if (m.IsDefined(typeof(OverloadAttribute), true))
					continue;

				var overloadedMethodParameters = m.GetParameters();
				if (overloadedMethodParameters.Length <= bestMatchParametersCount)
					continue;

				var matchedParameters = MatchParameters(overloadedMethodParameters, currentMethodParameters);
				if (matchedParameters <= bestMatchParametersCount)
					continue;

				bestMatchParametersCount = matchedParameters;
				bestMatch                = m;
			}

			return bestMatch;
		}

		static int MatchParameters(ParameterInfo[] parametersToMatch, ParameterInfo[] existingParameters)
		{
			var matchedParameters      = 0;
			var existingParametersList = new List<ParameterInfo>(existingParameters);

			foreach (var param in parametersToMatch)
			foreach (var existing in existingParametersList)
			{
				if (existing.Name != param.Name)
					continue;

				if (!GenericBinder.CompareParameterTypes(param.ParameterType, existing.ParameterType))
					return -1;

				++matchedParameters;
				existingParametersList.Remove(existing);

				break;
			}

			return matchedParameters;
		}
	}
}

#endif
