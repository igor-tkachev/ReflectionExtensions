using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

namespace TypeExtensions
{
	/// <summary>
	/// Extension methods for <see cref="System.Type"/>
	/// </summary>
	[PublicAPI]
	public static class TypeExtensions
	{
		const MethodImplOptions AggressiveInlining =
#if NET20 || NET30 || NET40
			(MethodImplOptions)256;
#else
			MethodImplOptions.AggressiveInlining;
#endif

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
		[MethodImpl(AggressiveInlining)]
		static TypeInfo TypeInfo(this Type type)
		{
			return type.GetTypeInfo();
		}
#else
		[MethodImpl(AggressiveInlining)]
		static Type TypeInfo(this Type type)
		{
			return type;
		}
#endif

		/// <summary>
		/// Gets the <see cref="T:System.Reflection.Assembly" /> in which the type is declared. For generic types,
		/// gets the <see cref="T:System.Reflection.Assembly" /> in which the generic type is defined.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Reflection.Assembly" /> instance that describes the assembly containing the current type.
		/// For generic types, the instance describes the assembly that contains the generic type definition,
		/// not the assembly that creates and uses a particular constructed type.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Assembly AssemblyEx([NotNull] this Type type)
		{
			return type.TypeInfo().Assembly;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsSealed([NotNull] this Type type)
		{
			return type.TypeInfo().IsSealed;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsAbstract([NotNull] this Type type)
		{
			return type.TypeInfo().IsAbstract;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsEnum([NotNull] this Type type)
		{
			return type.TypeInfo().IsEnum;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsClass([NotNull] this Type type)
		{
			return type.TypeInfo().IsClass;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsPrimitive([NotNull] this Type type)
		{
			return type.TypeInfo().IsPrimitive;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsPublic([NotNull] this Type type)
		{
			return type.TypeInfo().IsPublic;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsNestedPublic([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedPublic;
		}

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
		public static bool GetIsGenericType([NotNull] this Type type)
		{
			return type.TypeInfo().IsGenericType;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsGenericTypeDefinition([NotNull] this Type type)
		{
			return type.TypeInfo().IsGenericTypeDefinition;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsInterface([NotNull] this Type type)
		{
			return type.TypeInfo().IsInterface;
		}

		[MethodImpl(AggressiveInlining)]
		public static Type? GetBaseType([NotNull] this Type type)
		{
			return type.TypeInfo().BaseType;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsValueType([NotNull] this Type type)
		{
			return type.TypeInfo().IsValueType;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetContainsGenericParameters([NotNull] this Type type)
		{
			return type.TypeInfo().ContainsGenericParameters;
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsDefined([NotNull] this Type type, [NotNull] Type attributeType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			return type.TypeInfo().IsDefined(attributeType);
#else
			return Attribute.IsDefined(type, attributeType);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsDefined([NotNull] this Type type, [NotNull] Type attributeType, bool inherit)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			return type.GetTypeInfo().IsDefined(attributeType, inherit);
#else
			return Attribute.IsDefined(type, attributeType, inherit);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsArray([NotNull] this Type type)
		{
			return type.TypeInfo().IsArray;
		}

		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx([NotNull] this Type type, [NotNull] string propertyName)
		{
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
			return type.GetTypeInfo().GetDeclaredProperty(propertyName);
#else
			return type.TypeInfo().GetProperty(propertyName);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetPropertyValue<T>([NotNull] this Type type, [NotNull] string propertyName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
			return (T)property.GetValue(target);
#else
			return (T)type.InvokeMember(propertyName, BindingFlags.GetProperty, null, target, null);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetPropertyValue(
			[NotNull] this Type type, [NotNull] string propertyName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
			property.SetValue(target, value);
#else
			type.InvokeMember(propertyName, BindingFlags.SetProperty, null, target, new[] { value });
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetFieldValue<T>([NotNull] this Type type, [NotNull] string fieldName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var field = type.GetTypeInfo().GetDeclaredField(fieldName);
			return (T)field.GetValue(target);
#else
			return (T)type.InvokeMember(fieldName, BindingFlags.GetField | BindingFlags.GetProperty, null, target, null);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetFieldValue([NotNull] this Type type, [NotNull] string fieldName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var field = type.GetTypeInfo().GetDeclaredField(fieldName);
			if (field != null)
			{
				field.SetValue(target, value);
			}
			else
			{
				type.SetPropertyValue(fieldName, target, value);
			}
#else
			type.InvokeMember(fieldName, BindingFlags.SetField | BindingFlags.SetProperty, null, target, new[] { value });
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static void InvokeMethod<T>([NotNull] this Type type, [NotNull] string methodName, object target, T value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
			method.Invoke(target, new object[] { value });
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

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsAssignableFrom([NotNull] this Type type, [NotNull] Type otherType)
		{
			return type.TypeInfo().IsAssignableFrom(otherType.TypeInfo());
		}

		[MethodImpl(AggressiveInlining)]
		public static bool GetIsSubclassOf([NotNull] this Type type, [NotNull] Type c)
		{
			return type.TypeInfo().IsSubclassOf(c);
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this Type type)
			where T : Attribute
		{
#if NET20 || NET30 || NET40
			return type.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
#else
			return type.TypeInfo().GetCustomAttribute<T>();
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this Type type, bool inherit)
			where T : Attribute
		{
#if NET20 || NET30 || NET40
			return type.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
#else
			return type.TypeInfo().GetCustomAttribute<T>(inherit);
#endif
		}

		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this Type type)
		{
#if NET20 || NET30 || NET40
			return type.GetCustomAttributes(true).Cast<Attribute>().ToArray();
#else
			return type.TypeInfo().GetCustomAttributes().ToArray();
#endif
		}

		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this Type type, Type attributeType, bool inherit)
		{
#if NE_T20
			return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
#else
			return type.TypeInfo().GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
#endif
		}

		[NotNull]
		[MethodImpl(AggressiveInlining)]
		public static InterfaceMapping GetInterfaceMap([NotNull] this Type type, Type interfaceType)
		{
#if NETCOREAPP1_0
			return type.TypeInfo().GetRuntimeInterfaceMap(interfaceType);
#else
			return type.GetInterfaceMap(interfaceType);
#endif
		}
	}
}
