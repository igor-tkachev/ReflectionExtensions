#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionExtensions.Aspects.Builders
{
	using ReflectionExtensions.Reflection.Emit;
	using ReflectionExtensions.TypeBuilder.Builders;

	public class NotNullAspectBuilder : AbstractTypeBuilderBase
	{
		public NotNullAspectBuilder(string message)
		{
			_message = message;
		}

		private readonly string _message;

		public override int GetPriority(BuildContext context)
		{
			return TypeBuilderConsts.Priority.NotNullAspect;
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return context.IsBeforeStep && context.BuildElement != BuildElement.Type;
		}

		public override void Build(BuildContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			Debug.Assert(TargetElement != null, nameof(TargetElement) + " != null");

			var pi = (ParameterInfo)TargetElement;

			if (pi.ParameterType.IsValueType == false)
			{
				var emit    = context.MethodBuilder.Emitter;
				var label   = emit.DefineLabel();
				var message = _message != null? string.Format(_message, pi.Name): string.Empty;

				emit
					.ldarg    (pi)
					.brtrue_s (label)
					;

				if (string.IsNullOrEmpty(message))
				{
					emit
						.ldstr  (pi.Name)
						.newobj (typeof(ArgumentNullException), typeof(string))
						;
				}
				else
				{
					emit
						.ldnull
						.ldstr  (message)
						.newobj (typeof(ArgumentNullException), typeof(string), typeof(string))
						;
				}

				emit
					.@throw
					.MarkLabel (label)
					;
			}
		}
	}
}

#endif
