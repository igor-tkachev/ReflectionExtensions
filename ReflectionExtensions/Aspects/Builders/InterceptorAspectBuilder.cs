#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace ReflectionExtensions.Aspects.Builders
{
	using TypeBuilder.Builders;

	public class InterceptorAspectBuilder : AbstractTypeBuilderBase
	{
		public InterceptorAspectBuilder(
			Type? interceptorType, InterceptType interceptType, string? configString, int priority, bool localInterceptor)
		{
			_interceptorType  = interceptorType;
			_interceptType    = interceptType;
			_configString     = configString;
			_priority         = priority;
			_localInterceptor = localInterceptor;
		}

		readonly Type?         _interceptorType;
		readonly InterceptType _interceptType;
		readonly string?       _configString;
		readonly int           _priority;
		readonly bool          _localInterceptor;

		         FieldBuilder? _interceptorField;
		         LocalBuilder? _infoField;

		public override int GetPriority(BuildContext context)
		{
			return _priority;
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			if (_interceptorType == null && _interceptType == 0)
				return false;

			foreach (var builder in builders)
			{
				if (builder is InterceptorAspectBuilder interceptor)
				{
					if (interceptor._interceptorType == null && interceptor._interceptType == 0)
						return false;

					if (builder == this)
						break;
				}
			}

			if (context.IsMethodOrProperty) switch (context.Step)
			{
				case BuildStep.Begin:   return true;
				case BuildStep.Before:  return (_interceptType & InterceptType.BeforeCall) != 0;
				case BuildStep.After:   return (_interceptType & InterceptType.AfterCall)  != 0;
				case BuildStep.Catch:   return (_interceptType & InterceptType.OnCatch)    != 0;
				case BuildStep.Finally: return (_interceptType & InterceptType.OnFinally)  != 0;
				case BuildStep.End:     return true;
			}

			return false;
		}

		public override bool IsCompatible(BuildContext context, IAbstractTypeBuilder typeBuilder)
		{
			return !(typeBuilder is InterceptorAspectBuilder builder) || _interceptorType != builder._interceptorType;
		}

		public override void Build(BuildContext context)
		{
			if (context.Step == BuildStep.Begin || context.Step == BuildStep.End)
			{
				base.Build(context);
				return;
			}

			Context = context;

			var emit = Context.MethodBuilder.Emitter;

			// Push ref & out parameters.
			//
			var parameters = Context.CurrentMethod.GetParameters();

			Debug.Assert(_infoField != null, nameof(_infoField) + " != null");

			for (var i = 0; i < parameters.Length; i++)
			{
				var p = parameters[i];

				if (!p.ParameterType.IsByRef)
					continue;

				emit
					.ldloc      (_infoField)
					.callvirt   (typeof(InterceptCallInfo).GetProperty("ParameterValues").GetGetMethod())
					.ldc_i4     (i)
					.ldargEx    (p, true)
					.stelem_ref
					.end()
					;
			}

			// Push return value.
			//
			if (Context.ReturnValue != null)
			{
				emit
					.ldloc          (_infoField)
					.ldloc          (Context.ReturnValue)
					.boxIfValueType (Context.CurrentMethod.ReturnType)
					.callvirt       (typeof(InterceptCallInfo).GetProperty("ReturnValue").GetSetMethod())
					;
			}

			// Set Exception.
			//
			if (Context.Step == BuildStep.Catch)
			{
				Debug.Assert(Context.Exception != null, "Context.Exception != null");
				emit
					.ldloc(_infoField)
					.ldloc(Context.Exception)
					.callvirt(typeof(InterceptCallInfo).GetProperty("Exception").GetSetMethod())
					;
			}

			// Set intercept result.
			//
			emit
				.ldloc    (_infoField)
				.ldc_i4   ((int)InterceptResult.Continue)
				.callvirt (typeof(InterceptCallInfo).GetProperty("InterceptResult").GetSetMethod())
				;

			// Set intercept type.
			//
			InterceptType interceptType;

			switch (Context.Step)
			{
				case BuildStep.Before:  interceptType = InterceptType.BeforeCall; break;
				case BuildStep.After:   interceptType = InterceptType.AfterCall;  break;
				case BuildStep.Catch:   interceptType = InterceptType.OnCatch;    break;
				case BuildStep.Finally: interceptType = InterceptType.OnFinally;  break;
				default:
					throw new InvalidOperationException();
			}

			Debug.Assert(_interceptorField != null, nameof(_interceptorField) + " != null");

			emit
				.ldloc    (_infoField)
				.ldc_i4   ((int)interceptType)
				.callvirt (typeof(InterceptCallInfo).GetProperty("InterceptType").GetSetMethod())

			// Call interceptor.
			//
				.LoadField(_interceptorField)
				.ldloc    (_infoField)
				.callvirt (typeof(IInterceptor), "Intercept", typeof(InterceptCallInfo))
				;

			// Pop return value.
			//
			if (Context.ReturnValue != null)
			{
				emit
					.ldloc          (_infoField)
					.callvirt       (typeof(InterceptCallInfo).GetProperty("ReturnValue").GetGetMethod())
					.CastFromObject (Context.CurrentMethod.ReturnType)
					.stloc          (Context.ReturnValue)
					;
			}

			// Pop ref & out parameters.
			//
			for (var i = 0; i < parameters.Length; i++)
			{
				var p = parameters[i];

				if (!p.ParameterType.IsByRef)
					continue;

				var type = p.ParameterType.GetElementType();

				emit
					.ldarg          (p)
					.ldloc          (_infoField)
					.callvirt       (typeof(InterceptCallInfo).GetProperty("ParameterValues").GetGetMethod())
					.ldc_i4         (i)
					.ldelem_ref
					.CastFromObject (type)
					.stind          (type)
					;
			}

			// Check InterceptResult
			emit
				.ldloc    (_infoField)
				.callvirt (typeof(InterceptCallInfo).GetProperty("InterceptResult").GetGetMethod())
				.ldc_i4   ((int)InterceptResult.Return)
				.beq      (Context.ReturnLabel)
				;
		}

		static int _methodCounter;

		LocalBuilder GetInfoField()
		{
			var field = Context.GetItem<LocalBuilder>("$ReflectionExtensions.InfoField");

			if (field == null)
			{
				_methodCounter++;

				// Create MethodInfoLock field.
				//
				var typeEmit = Context.TypeBuilder.TypeInitializer.Emitter;
				var syncRoot = Context.CreatePrivateStaticField(
					Context.CurrentMethod.Name + "$Lock" + _methodCounter, typeof (object));

				typeEmit
					.newobj(typeof(object))
					.stsfld(syncRoot)
					;


				// Create MethodInfo field.
				//
				var methodInfo = Context.CreatePrivateStaticField(
					"_methodInfo$" + Context.CurrentMethod.Name + _methodCounter, typeof(CallMethodInfo));

				var emit = Context.MethodBuilder.Emitter;


				emit.BeginExceptionBlock();
				emit
					.LoadField(syncRoot)
					.call(typeof(Monitor), "Enter", typeof(object));

				var checkMethodInfo = emit.DefineLabel();
				var constructorInfo = typeof(CallMethodInfo).GetConstructorEx(typeof(MethodInfo));

				Debug.Assert(constructorInfo != null, nameof(constructorInfo) + " != null");

				emit
					.LoadField (methodInfo)
					.brtrue_s  (checkMethodInfo)
					.call      (typeof(MethodBase), "GetCurrentMethod")
					.castclass (typeof(MethodInfo))
					.newobj    (constructorInfo)
					.stsfld    (methodInfo)
					.LoadField (methodInfo)
					.LoadField (syncRoot)
					.call      (typeof (CallMethodInfo), "set_SyncRoot", typeof (object))
					.MarkLabel (checkMethodInfo)
					;

				emit.BeginFinallyBlock();
				emit
					.LoadField(syncRoot)
					.call(typeof(Monitor), "Exit", typeof(object));
				emit.ILGenerator.EndExceptionBlock();

				// Create & initialize the field.
				//
				field = emit.DeclareLocal(typeof(InterceptCallInfo));

				var constructor        = typeof(InterceptCallInfo).GetConstructorEx();
				var objectProperty     = typeof(InterceptCallInfo).GetProperty(nameof(InterceptCallInfo.Object));
				var methodInfoProperty = typeof(InterceptCallInfo).GetProperty(nameof(InterceptCallInfo.CallMethodInfo));

				Debug.Assert(constructor        != null, nameof(constructor) + " != null");
				Debug.Assert(objectProperty     != null, nameof(objectProperty) + " != null");
				Debug.Assert(methodInfoProperty != null, nameof(methodInfoProperty) + " != null");

				emit
					.newobj   (constructor)
					.dup
					.ldarg_0
					.callvirt (objectProperty.GetSetMethod())

					.dup
					.LoadField(methodInfo)
					.callvirt (methodInfoProperty.GetSetMethod())
					;

				var parameters = Context.CurrentMethod.GetParameters();

				for (var i = 0; i < parameters.Length; i++)
				{
					var p = parameters[i];

					if (p.ParameterType.IsByRef)
						continue;

					emit
						.dup
						.callvirt   (typeof(InterceptCallInfo).GetProperty("ParameterValues").GetGetMethod())
						.ldc_i4     (i)
						.ldargEx    (p, true)
						.stelem_ref
						.end()
						;
				}

				emit.stloc(field);

				Context.Items.Add("$ReflectionExtensions.MethodInfo", methodInfo);
				Context.Items.Add("$ReflectionExtensions.InfoField",  field);
				Context.Items.Add("$ReflectionExtensions.SyncRoot",   syncRoot);
			}

			return field;
		}

		FieldBuilder GetInterceptorField()
		{
			var fieldName = $"_interceptor${_interceptorType?.FullName}$_{Context.CurrentMethod.Name}{_methodCounter}";
			var field     = Context.GetField(fieldName);

			if (field == null)
			{
				// Create MethodInfo field.
				//
				field = _localInterceptor?
					Context.CreatePrivateField      (fieldName, typeof(IInterceptor)):
					Context.CreatePrivateStaticField(fieldName, typeof(IInterceptor));

				var emit = Context.MethodBuilder.Emitter;

				var checkInterceptor = emit.DefineLabel();
				var methodInfo       = Context.GetItem<FieldBuilder>("$ReflectionExtensions.MethodInfo");
				var syncRoot         = Context.GetItem<FieldBuilder>("$ReflectionExtensions.SyncRoot");

				emit.BeginExceptionBlock();
				emit
					.LoadField(syncRoot)
					.call(typeof(Monitor), "Enter", typeof(object));

				emit
					.LoadField (field)
					.brtrue_s  (checkInterceptor)
					;

				if (!field.IsStatic)
					emit.ldarg_0.end();

				var ctor = _interceptorType?.GetConstructorEx();

				Debug.Assert(ctor != null, nameof(ctor) + " != null");

				emit
					.newobj    (ctor)
					.castclass (typeof(IInterceptor))
					;

				if (field.IsStatic)
					emit.stsfld(field);
				else
					emit.stfld(field);

				emit
					.LoadField (field)
					.LoadField (methodInfo)
					.ldstrEx   (_configString ?? "")
					.callvirt  (typeof(IInterceptor), "Init", typeof(CallMethodInfo), typeof(string))

					.MarkLabel (checkInterceptor)
					;


				emit.BeginFinallyBlock();
				emit
					.LoadField(syncRoot)
					.call(typeof(Monitor), "Exit", typeof(object));
				emit.ILGenerator.EndExceptionBlock();

			}

			return field;
		}

		protected override void BeginMethodBuild()
		{
			_infoField        = GetInfoField();
			_interceptorField = GetInterceptorField();
		}

		protected override void EndMethodBuild()
		{
			Context.Items.Remove("$ReflectionExtensions.MethodInfo");
			Context.Items.Remove("$ReflectionExtensions.InfoField");
			Context.Items.Remove("$ReflectionExtensions.SyncRoot");
		}
	}
}

#endif
