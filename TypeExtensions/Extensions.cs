using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;

using JetBrains.Annotations;

namespace TypeExtensions
{
	/// <summary>
	/// Extension methods for <see cref="System.Type"/>
	/// </summary>
	[PublicAPI]
	public static partial class Extensions
	{
		[MethodImpl(AggressiveInlining)]
		public static bool GetIsFromLocalAssembly([NotNull] this Type type)
		{
#if SILVERLIGHT
			string assemblyName = type.GetAssembly().FullName;
#else
			var assemblyName = type.AssemblyEx().GetName().Name;
#endif

			try
			{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
				Assembly.Load(new AssemblyName { Name = assemblyName });
#else
				Assembly.Load(assemblyName);
#endif
				return true;
			}
			catch
			{
				return false;
			}
		}

		[MethodImpl(AggressiveInlining)]
		public static void InvokeMethod<T>([NotNull] this Type type, [NotNull] string methodName, object target, T value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
			method.Invoke(target,
#nullable disable
			new object[] { value });
#nullable restore
#else
			type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target,
#nullable disable
				new object[] { value });
#nullable restore
#endif
		}

#if !NETCOREAPP1_0 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
		const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		[MethodImpl(AggressiveInlining)]
		public static MethodInfo? GetMethod(
			[NotNull] this Type type,
			[NotNull] string name,
			BindingFlags bindingAttr,
			[CanBeNull] object binder,
			[NotNull, ItemNotNull] Type[] types)
		{
			if (name == null) throw new ArgumentNullException(name, nameof(name));
			if (binder != null) throw new NotImplementedException();
			if (types.Length != 0) throw new NotImplementedException();

			return type.GetMethod(name, bindingAttr, binder, types, null);
		}

		[MethodImpl(AggressiveInlining)]
		public static MethodInfo? GetMethod(
			[NotNull]         this Type                 type,
			[NotNull]              string               name,
			                       BindingFlags         bindingAttr,
			[CanBeNull]            object               binder,
			[NotNull, ItemNotNull] Type[]               types,
			                       ParameterModifier[]? modifiers)
		{
			if (name == null) throw new ArgumentNullException(nameof(name));
			if (binder != null) throw new NotImplementedException(nameof(binder));
			if (types == null) throw new ArgumentNullException(nameof(types));
			if (types.Any(t => t == null)) throw new ArgumentNullException(nameof(types));
			if (modifiers != null) throw new NotImplementedException(nameof(modifiers));

			if (bindingAttr == BindingFlags.Default)
				bindingAttr = DefaultLookup;

			return type.GetMethods(bindingAttr).Where(m => m.Name == name).TryFindParametersTypesMatch(types);
		}

		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo? GetConstructor(
			this Type            type,
			BindingFlags         bindingAttr,
			object?              binder,
			Type[]               types,
			ParameterModifier[]? modifiers)
		{
			if (binder != null) throw new NotImplementedException(nameof(binder));
			if (types == null) throw new ArgumentNullException(nameof(types));
			if (types.Any(t => t == null)) throw new ArgumentNullException(nameof(types));
			if (modifiers != null) throw new NotImplementedException(nameof(modifiers));

			if (bindingAttr == BindingFlags.Default) bindingAttr = DefaultLookup;

			return type.GetConstructors(bindingAttr).TryFindParametersTypesMatch(types);
		}

		static T? TryFindParametersTypesMatch<T>(
			[NotNull, ItemNotNull] this IEnumerable<T> methods,
			[NotNull, ItemNotNull]      Type[]         types)
			where T : MethodBase
		{
			return methods.Where(m =>
				{
					var parameters = m.GetParameters();
					if (parameters.Length != types.Length) return false;

					for (var i = 0; i < parameters.Length; i++)
					{
						if (!parameters[i].ParameterType.Equals(types[i])) return false;
					}

					return true;
				})
				.FirstOrDefault();
		}

#endif
	}
}
