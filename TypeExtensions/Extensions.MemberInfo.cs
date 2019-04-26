using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace TypeExtensions
{
	static partial class Extensions
	{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this Type element)
			where T : Attribute
		{
			return element.GetTypeInfo().GetCustomAttribute<T>();
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="inherit">
		/// <see langword="true" /> to inspect the ancestors of <paramref name="element" />; otherwise, <see langword="false" />. </param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this Type element, bool inherit)
			where T : Attribute
		{
			return element.GetTypeInfo().GetCustomAttribute<T>(inherit);
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <returns>A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx([NotNull] this Type element, Type attributeType)
		{
			return element.GetTypeInfo().GetCustomAttribute(attributeType);
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <param name="inherit">
		/// <see langword="true" /> to inspect the ancestors of <paramref name="element" />; otherwise, <see langword="false" />. </param>
		/// <returns>A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx([NotNull] this Type element, Type attributeType, bool inherit)
		{
			return element.GetTypeInfo().GetCustomAttribute(attributeType, inherit);
		}

		/// <summary>Retrieves a collection of custom attributes that are applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <returns>A collection of the custom attributes that are applied to <paramref name="element" />, or an empty collection if no such attributes exist. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this Type element)
		{
			return element.GetTypeInfo().GetCustomAttributes().ToArray();
		}

		/// <summary>When overridden in a derived class, returns an array of custom attributes applied to this member and identified by <see cref="T:System.Type" />.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for. Only attributes that are assignable to this type are returned. </param>
		/// <param name="inherit">
		/// <see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />. This parameter is ignored for properties and events; see Remarks. </param>
		/// <returns>An array of custom attributes applied to this member, or an array with zero elements if no attributes assignable to <paramref name="attributeType" /> have been applied.</returns>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		/// <exception cref="T:System.ArgumentNullException">If <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.</exception>
		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this Type element, Type attributeType, bool inherit)
		{
			return element.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
		}
#endif

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this MemberInfo element)
			where T : Attribute
		{
#if NET20 || NET30 || NET35 || NET40
			return element.GetCustomAttributes(true).OfType<T>().FirstOrDefault();
#else
			return element.GetCustomAttribute<T>();
#endif
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="inherit">
		/// <see langword="true" /> to inspect the ancestors of <paramref name="element" />; otherwise, <see langword="false" />. </param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <paramref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>([NotNull] this MemberInfo element, bool inherit)
			where T : Attribute
		{
#if NET20 || NET30 || NET35 || NET40
			return element.GetCustomAttributes(inherit).OfType<T>().FirstOrDefault();
#else
			return element.GetCustomAttribute<T>(inherit);
#endif
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <returns>A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx([NotNull] this MemberInfo element, Type attributeType)
		{
#if NET20 || NET30 || NET35 || NET40
			return (Attribute)element.GetCustomAttributes(true).Where(a => a.GetType() == attributeType).FirstOrDefault();
#else
			return element.GetCustomAttribute(attributeType);
#endif
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for.</param>
		/// <param name="inherit">
		/// <see langword="true" /> to inspect the ancestors of <paramref name="element" />; otherwise, <see langword="false" />. </param>
		/// <returns>A custom attribute that matches <paramref name="attributeType" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx([NotNull] this MemberInfo element, Type attributeType, bool inherit)
		{
#if NET20 || NET30 || NET35 || NET40
			return (Attribute)element.GetCustomAttributes(inherit).Where(a => a.GetType() == attributeType).FirstOrDefault();
#else
			return element.GetCustomAttribute(attributeType, inherit);
#endif
		}

		/// <summary>Retrieves a collection of custom attributes that are applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <returns>A collection of the custom attributes that are applied to <paramref name="element" />, or an empty collection if no such attributes exist. </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this MemberInfo element)
		{
#if NET20 || NET30 || NET35 || NET40
			return element.GetCustomAttributes(true).Cast<Attribute>().ToArray();
#else
			return element.GetCustomAttributes().ToArray();
#endif
		}

		/// <summary>When overridden in a derived class, returns an array of custom attributes applied to this member and identified by <see cref="T:System.Type" />.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="attributeType">The type of attribute to search for. Only attributes that are assignable to this type are returned. </param>
		/// <param name="inherit">
		/// <see langword="true" /> to search this member's inheritance chain to find the attributes; otherwise, <see langword="false" />. This parameter is ignored for properties and events; see Remarks. </param>
		/// <returns>An array of custom attributes applied to this member, or an array with zero elements if no attributes assignable to <paramref name="attributeType" /> have been applied.</returns>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		/// <exception cref="T:System.ArgumentNullException">If <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.</exception>
		[NotNull, ItemNotNull]
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx([NotNull] this MemberInfo element, Type attributeType, bool inherit)
		{
			return element.GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
		}
	}
}
