using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

namespace TypeExtensions
{
	partial class Extensions
	{
		internal const MethodImplOptions AggressiveInlining =
#if NET20 || NET30 || NET35 || NET40
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

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is abstract and must be overridden.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is abstract; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAbstractEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsAbstract;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared sealed.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared sealed; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsSealedEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsSealed;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a class or a delegate; that is, not a value type or interface.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a class; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsClassEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsClass;
		}

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns><see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsEnumEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsEnum;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is one of the primitive types.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is one of the primitive types; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsPrimitiveEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsPrimitive;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared public.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared public and is not a nested type; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsPublicEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsPublic;
		}

		/// <summary>Gets a value indicating whether a class is nested and declared public.</summary>
		/// <returns><see langword="true" /> if the class is nested and declared public; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedPublicEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedPublic;
		}

		/// <summary>Gets a value indicating whether the current type is a generic type.</summary>
		/// <returns><see langword="true" /> if the current type is a generic type; otherwise,<see langword=" false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericTypeEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsGenericType;
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a generic type definition, from which other generic types can be constructed.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> object represents a generic type definition; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericTypeDefinitionEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsGenericTypeDefinition;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is an interface; that is, not a class or a value type.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is an interface; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsInterfaceEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsInterface;
		}

		/// <summary>Gets the type from which the current <see cref="T:System.Type" /> directly inherits.</summary>
		/// <returns>The <see cref="T:System.Type" /> from which the current <see cref="T:System.Type" /> directly inherits, or <see langword="null" /> if the current <see langword="Type" /> represents the <see cref="T:System.Object" /> class or an interface.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type? BaseTypeEx([NotNull] this Type type)
		{
			return type.TypeInfo().BaseType;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a value type.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a value type; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsValueTypeEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsValueType;
		}

		/// <summary>Gets a value that indicates whether the type is an array.</summary>
		/// <returns><see langword="true" /> if the current type is an array; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsArrayEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsArray;
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object has type parameters that have not been replaced by specific types.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> object is itself a generic type parameter or has type parameters
		/// for which specific types have not been supplied; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool ContainsGenericParametersEx([NotNull] this Type type)
		{
			return type.TypeInfo().ContainsGenericParameters;
		}

		/// <summary>Determines whether an instance of a specified type can be assigned to an instance of the current type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="c">The type to compare with the current type. </param>
		/// <returns>
		/// <see langword="true" /> if any of the following conditions is true:
		///     <paramref name="c" /> and the current instance represent the same type.
		///     <paramref name="c" /> is derived either directly or indirectly from the current instance. <paramref name="c" /> is derived directly from the current instance if it inherits from the current instance; <paramref name="c" /> is derived indirectly from the current instance if it inherits from a succession of one or more classes that inherit from the current instance.  The current instance is an interface that <paramref name="c" /> implements.
		///     <paramref name="c" /> is a generic type parameter, and the current instance represents one of the constraints of <paramref name="c" />. In the following example, the current instance is a <see cref="T:System.Type" /> object that represents the <see cref="T:System.IO.Stream" /> class. GenericWithConstraint is a generic type whose generic type parameter must be of type    <see cref="T:System.IO.Stream" />. Passing its generic type parameter to the <see cref="M:System.Type.IsAssignableFrom(System.Type)" /> indicates that  an instance of the generic type parameter can be assigned to an <see cref="T:System.IO.Stream" /> object. System.Type.IsAssignableFrom#2
		///     <paramref name="c" /> represents a value type, and the current instance represents Nullable&lt;c&gt; (Nullable(Of c) in Visual Basic).
		/// <see langword="false" /> if none of these conditions are true, or if <paramref name="c" /> is <see langword="null" />. </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAssignableFromEx([NotNull] this Type type, [NotNull] Type c)
		{
			return type.TypeInfo().IsAssignableFrom(c.TypeInfo());
		}

		/// <summary>Determines whether the current <see cref="T:System.Type" /> derives from the specified <see cref="T:System.Type" />.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="c">The type to compare with the current type. </param>
		/// <returns>
		/// <see langword="true" /> if the current <see langword="Type" /> derives from <paramref name="c" />; otherwise, <see langword="false" />.
		/// This method also returns <see langword="false" /> if <paramref name="c" /> and the current <see langword="Type" /> are equal.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="c" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsSubclassOfEx([NotNull] this Type type, [NotNull] Type c)
		{
			return type.TypeInfo().IsSubclassOf(c);
		}

		/// <summary>Determines whether any custom attributes are applied to a member of a type. Parameters specify the member, and the type of the custom attribute to search for.</summary>
		/// <param name="type">An object derived from the <see cref="T:System.Reflection.MemberInfo" /> class that describes a constructor, event, field, method, type, or property member of a class. </param>
		/// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
		/// <returns>
		/// <see langword="true" /> if a custom attribute of type <paramref name="attributeType" /> is applied to <paramref name="type" />; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="type" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="type" /> is not a constructor, method, property, event, type, or field. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx([NotNull] this Type type, [NotNull] Type attributeType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			return type.TypeInfo().IsDefined(attributeType);
#else
			return Attribute.IsDefined(type, attributeType);
#endif
		}

		/// <summary>Determines whether any custom attributes are applied to a member of a type. Parameters specify the member, the type of the custom attribute to search for, and whether to search ancestors of the member.</summary>
		/// <param name="type">An object derived from the <see cref="T:System.Reflection.MemberInfo" /> class that describes a constructor, event, field, method, type, or property member of a class. </param>
		/// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
		/// <param name="inherit">If <see langword="true" />, specifies to also search the ancestors of <paramref name="type" /> for custom attributes. </param>
		/// <returns>
		/// <see langword="true" /> if a custom attribute of type <paramref name="attributeType" /> is applied to <paramref name="type" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="type" /> or <paramref name="attributeType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="type" /> is not a constructor, method, property, event, type, or field. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx([NotNull] this Type type, [NotNull] Type attributeType, bool inherit)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			return type.GetTypeInfo().IsDefined(attributeType, inherit);
#else
			return Attribute.IsDefined(type, attributeType, inherit);
#endif
		}

		/// <summary>Returns an interface mapping for the specified interface type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="interfaceType">The interface type to retrieve a mapping for. </param>
		/// <returns>An object that represents the interface mapping for <paramref name="interfaceType" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="interfaceType" /> is not implemented by the current type. -or-The <paramref name="interfaceType" /> parameter does not refer to an interface. -or-
		/// <paramref name="interfaceType" /> is a generic interface, and the current type is an array type. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="interfaceType" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type" /> represents a generic type parameter; that is, <see cref="P:System.Type.IsGenericParameter" /> is <see langword="true" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static InterfaceMapping GetInterfaceMapEx([NotNull] this Type type, Type interfaceType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			return type.TypeInfo().GetRuntimeInterfaceMap(interfaceType);
#else
			return type.GetInterfaceMap(interfaceType);
#endif
		}

		/// <summary>Searches for the public property with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx([NotNull] this Type type, [NotNull] string name)
		{
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
			return type.GetTypeInfo().GetDeclaredProperty(name);
#else
			return type.TypeInfo().GetProperty(name);
#endif
		}
	}
}
