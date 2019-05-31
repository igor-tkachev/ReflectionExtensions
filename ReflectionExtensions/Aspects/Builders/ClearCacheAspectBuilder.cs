

using System.Diagnostics;
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionExtensions.Aspects.Builders
{
	using ReflectionExtensions.Reflection.Emit;
	using ReflectionExtensions.TypeBuilder.Builders;

	public class ClearCacheAspectBuilder : AbstractTypeBuilderBase
	{
		#region Init

		public ClearCacheAspectBuilder(Type? declaringType, string? methodName, Type[]? parameterTypes)
		{
			_declaringType  = declaringType;
			_methodName     = methodName;
			_parameterTypes = parameterTypes;
		}

		readonly Type?   _declaringType;
		readonly string? _methodName;
		readonly Type[]? _parameterTypes;

		#endregion

		#region Overrides

		public override int GetPriority(BuildContext context)
		{
			return TypeBuilderConsts.Priority.ClearCacheAspect;
		}

		public override bool IsCompatible(BuildContext context, IAbstractTypeBuilder typeBuilder)
		{
			return true;
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return context.IsFinallyStep && context.IsMethodOrProperty;
		}

		#endregion

		#region Build

		static int _methodCounter;

		public override void Build(BuildContext context)
		{
			Context = context;

			if (string.IsNullOrEmpty(_methodName))
			{
				var type      = Context.CreatePrivateStaticField($"_type$ClearCacheAspect${++_methodCounter}", typeof(Type));
				var emit      = Context.MethodBuilder.Emitter;
				var checkType = emit.DefineLabel();

				emit
					.ldsfld    (type)
					.brtrue_s  (checkType)
					.ldarg_0
					.LoadType  (_declaringType)
					.call      (typeof(ClearCacheAspect), "GetType", typeof(object), typeof(Type))
					.stsfld    (type)
					.MarkLabel (checkType)
					.ldsfld    (type)
					.call      (typeof(CacheAspect), "ClearCache", typeof(Type))
					;
			}
			else
			{
				var methodInfo      = Context.CreatePrivateStaticField($"_methodInfo$ClearCacheAspect${++_methodCounter}", typeof(MethodInfo));
				var emit            = Context.MethodBuilder.Emitter;
				var checkMethodInfo = emit.DefineLabel();

				emit
					.ldsfld   (methodInfo)
					.brtrue_s (checkMethodInfo)
					.ldarg_0
					.LoadType (_declaringType)
					.ldstrEx  (_methodName)
					;

				if (_parameterTypes == null || _parameterTypes.Length == 0)
				{
					emit.ldnull.end();
				}
				else
				{
					var field = emit.DeclareLocal(typeof(Type[]));

					emit
						.ldc_i4_ (_parameterTypes.Length)
						.newarr  (typeof(Type))
						.stloc   (field)
						;

					for (var i = 0; i < _parameterTypes.Length; i++)
					{
						emit
							.ldloc      (field)
							.ldc_i4     (i)
							.LoadType   (_parameterTypes[i])
							.stelem_ref
							.end()
							;
					}

					emit.ldloc(field);
				}

				emit
					.call      (typeof(ClearCacheAspect), "GetMethodInfo", typeof(object), typeof(Type), typeof(string), typeof(Type[]))
					.stsfld    (methodInfo)
					.MarkLabel (checkMethodInfo)
					.ldsfld    (methodInfo)
					.call      (typeof(CacheAspect), "ClearCache", typeof(MethodInfo))
					;
			}
		}

		#endregion
	}
}

#endif
