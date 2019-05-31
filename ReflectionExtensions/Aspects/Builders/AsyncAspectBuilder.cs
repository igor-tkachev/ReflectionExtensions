﻿#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace ReflectionExtensions.Aspects.Builders
{
	using TypeBuilder;
	using TypeBuilder.Builders;

	/// <summary>
	/// This aspect simplifies asynchronous operations.
	/// </summary>
	public class AsyncAspectBuilder : AbstractTypeBuilderBase
	{
		readonly string? _targetMethodName;
		readonly Type[]? _parameterTypes;

		public AsyncAspectBuilder(string? targetMethodName, Type[]? parameterTypes)
		{
			_targetMethodName = targetMethodName;
			_parameterTypes   = parameterTypes;
		}

		public override int GetPriority(BuildContext context)
		{
			return TypeBuilderConsts.Priority.AsyncAspect;
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
			var mi = Context.CurrentMethod;

			if (mi.ReturnType == typeof(IAsyncResult))
				BuildBeginMethod();
			else
			{
				var parameters = mi.GetParameters();

				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IAsyncResult))
					BuildEndMethod();
				else
					throw new TypeBuilderException(
						$"Method '{mi.DeclaringType?.FullName}.{mi.Name}' is not a 'Begin' nor an 'End' method.");
			}
		}

		void BuildBeginMethod()
		{
			var mi           = Context.CurrentMethod;
			var method       = GetTargetMethod(Context, "Begin");
			var delegateType = EnsureDelegateType(Context, method);
			var emit         = Context.MethodBuilder.Emitter;
			var type         = typeof(InternalAsyncResult);
			var arLocal      = emit.DeclareLocal(type);
			var dLocal       = emit.DeclareLocal(delegateType);

			emit
				.newobj (type)
				.dup
				.dup
				.dup
				.stloc  (arLocal)
				.ldarg_0
				.ldftn  (method)
				.newobj (delegateType, typeof(object), typeof(IntPtr))
				.stloc  (dLocal)
				.ldloc  (dLocal)
				.stfld  (type.GetField("Delegate"))
				.ldloc  (dLocal)
				;

			var callbackIndex = -1;
			var parameters    = mi.GetParameters();

			for (var i = 0; i < parameters.Length; ++i)
			{
				if (parameters[i].ParameterType == typeof(AsyncCallback))
				{
					callbackIndex = i;
					emit
						.ldloc  (arLocal)
						.dup
						.ldarg  (parameters[i])
						.stfld  (type.GetField("AsyncCallback"))
						.ldftn  (type.GetMethod("CallBack"))
						.newobj (typeof(AsyncCallback), typeof(object), typeof(IntPtr))
						;
				}
				else
					emit.ldarg(parameters[i]);
			}

			if (callbackIndex < 0)
			{
				// Callback omitted
				//
				emit
					.ldnull
					.ldnull
					.end();
			}
			else if (callbackIndex == parameters.Length - 1)
			{
				// State omitted
				//
				emit
					.ldnull
					.end();
			}

			Debug.Assert(Context.ReturnValue != null, "Context.ReturnValue != null");

			emit
				.callvirt (delegateType.GetMethod("BeginInvoke"))
				.stfld    (type.GetField("InnerResult"))
				.stloc    (Context.ReturnValue)
				;
		}

		void BuildEndMethod()
		{
			var method       = GetTargetMethod(Context, "End");
			var delegateType = EnsureDelegateType(Context, method);
			var emit         = Context.MethodBuilder.Emitter;
			var type         = typeof(InternalAsyncResult);
			var arLocal      = emit.DeclareLocal(type);

			emit
				.ldarg_1
				.castclass (type)
				.dup
				.stloc     (arLocal)
				.ldfld     (type.GetField("Delegate"))
				.castclass (delegateType)
				.ldloc     (arLocal)
				.ldfld     (type.GetField("InnerResult"))
				.callvirt  (delegateType, "EndInvoke", typeof(IAsyncResult));

			if (Context.ReturnValue != null)
				emit.stloc(Context.ReturnValue);
		}

		MethodInfo GetTargetMethod(BuildContext context, string prefix)
		{
			var targetMethodName = _targetMethodName;

			if (targetMethodName == null)
			{
				var mi   = context.CurrentMethod;
				var name = mi.Name;

				if (name.StartsWith(prefix))
					targetMethodName = name.Substring(prefix.Length);
				else
					throw new TypeBuilderException(string.Format(
						"Can not figure out the target method for the method '{0}.{1}'.",
							mi.DeclaringType?.FullName, mi.Name));
			}

			return _parameterTypes == null?
				context.Type.GetMethod(targetMethodName):
				context.Type.GetMethod(targetMethodName, _parameterTypes);
		}

		static Type EnsureDelegateType(BuildContext context, MethodInfo method)
		{
			// The delegate should be defined as inner type of context.TypeBuilder.
			// It's possible, but we can not define and use newly defined type as Emit target in its owner type.
			// To solve this problem, we should create a top level delegate and make sure its name is unique.
			//
			var delegateName = context.TypeBuilder.TypeBuilder.FullName + "$" + method.Name + "$Delegate";
			var delegateType = context.GetItem<Type>(delegateName);

			if (delegateType == null)
			{
				var pi         = method.GetParameters();
				var parameters = new Type[pi.Length];

				for (var i = 0; i < pi.Length; i++)
					parameters[i] = pi[i].ParameterType;

				const MethodImplAttributes mia = MethodImplAttributes.Runtime | MethodImplAttributes.Managed;
				const MethodAttributes     ma  = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

				Debug.Assert(context.AssemblyBuilder != null, "context.AssemblyBuilder != null");

				var delegateBuilder = context.AssemblyBuilder.DefineType(delegateName,
					TypeAttributes.Class | TypeAttributes.NotPublic | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass,
					typeof(MulticastDelegate));

				// Create constructor
				//
				var ctorBuilder = delegateBuilder.DefineConstructor(
					MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName, CallingConventions.Standard,
					typeof(object), typeof(IntPtr));
				ctorBuilder.ConstructorBuilder.SetImplementationFlags(mia);

				// Define the BeginInvoke method for the delegate
				//
				var beginParameters = new Type[parameters.Length + 2];

				Array.Copy(parameters, 0, beginParameters, 0, parameters.Length);
				beginParameters[parameters.Length]   = typeof(AsyncCallback);
				beginParameters[parameters.Length+1] = typeof(object);

				var methodBuilder = delegateBuilder.DefineMethod("BeginInvoke", ma, typeof(IAsyncResult), beginParameters);

				methodBuilder.MethodBuilder.SetImplementationFlags(mia);

				// Define the EndInvoke method for the delegate
				//
				methodBuilder = delegateBuilder.DefineMethod("EndInvoke", ma, method.ReturnType, typeof(IAsyncResult));
				methodBuilder.MethodBuilder.SetImplementationFlags(mia);

				// Define the Invoke method for the delegate
				//
				methodBuilder = delegateBuilder.DefineMethod("Invoke", ma, method.ReturnType, parameters);
				methodBuilder.MethodBuilder.SetImplementationFlags(mia);

				context.Items.Add(delegateName, delegateType = delegateBuilder.Create());
			}

			return delegateType;
		}

		#region Helper

		/// <summary>
		/// Reserved for internal ReflectionExtensions use.
		/// </summary>
		public class InternalAsyncResult : IAsyncResult
		{
			public IAsyncResult?  InnerResult;
			public Delegate?      Delegate;
			public AsyncCallback? AsyncCallback;

			public void CallBack(IAsyncResult ar)
			{
				AsyncCallback?.Invoke(this);
			}

			public bool        IsCompleted            => InnerResult?.IsCompleted ?? false;
			public WaitHandle? AsyncWaitHandle        => InnerResult?.AsyncWaitHandle;
			public object?     AsyncState             => InnerResult?.AsyncState;
			public bool        CompletedSynchronously => InnerResult?.CompletedSynchronously ?? false;
		}

		#endregion
	}
}

#endif
