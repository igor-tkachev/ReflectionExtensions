#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
#define NETSTANDARDLESS1_4
#endif
#if NETSTANDARDLESS1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
#define NETSTANDARDLESS1_6
#endif

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

namespace TypeExtensions
{
	[PublicAPI]
	public static partial class Extensions
	{
		#region Helpers

		internal const MethodImplOptions AggressiveInlining =
#if NET20 || NET30 || NET35 || NET40
			(MethodImplOptions)256;
#else
			MethodImplOptions.AggressiveInlining;
#endif


#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARDLESS1_6
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

		#endregion

		#region Common

		/// <summary>Gets a <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</summary>
		/// <returns>A <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MemberTypes MemberTypeEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return MemberTypes.TypeInfo;
#else
			return type.TypeInfo().MemberType;
#endif
		}

		/// <summary>Gets the namespace of the <see cref="T:System.Type" />.</summary>
		/// <returns>The namespace of the <see cref="T:System.Type" />; <see langword="null" /> if the current instance has no namespace or represents a generic parameter.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static string NamespaceEx([NotNull] this Type type)
		{
			return type.Namespace;
		}

		/// <summary>Gets the assembly-qualified name of the type, which includes the name of the assembly from which this <see cref="T:System.Type" /> object was loaded.</summary>
		/// <returns>The assembly-qualified name of the <see cref="T:System.Type" />, which includes the name of the assembly from which the <see cref="T:System.Type" /> was loaded, or <see langword="null" /> if the current instance represents a generic type parameter.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static string AssemblyQualifiedNameEx([NotNull] this Type type)
		{
			return type.AssemblyQualifiedName;
		}

		/// <summary>Gets the fully qualified name of the type, including its namespace but not its assembly. </summary>
		/// <returns>The fully qualified name of the type, including its namespace but not its assembly; or <see langword="null" /> if the current instance represents a generic type parameter, an array type, pointer type, or <see langword="byref" /> type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static string FullNameEx([NotNull] this Type type)
		{
			return type.FullName;
		}

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

		/// <summary>Gets the module (the DLL) in which the current <see cref="T:System.Type" /> is defined.</summary>
		/// <returns>The module in which the current <see cref="T:System.Type" /> is defined.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Module ModuleEx([NotNull] this Type type)
		{
			return type.TypeInfo().Module;
		}

		/// <summary>
		/// Gets a <see cref="T:System.Reflection.MethodBase" /> that represents the declaring method, if the current <see cref="T:System.Type" /> represents a type parameter of a generic method.</summary>
		/// <returns>If the current <see cref="T:System.Type" /> represents a type parameter of a generic method, a <see cref="T:System.Reflection.MethodBase" /> that represents declaring method; otherwise, <see langword="null" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodBase? DeclaringMethodEx([NotNull] this Type type)
		{
			return type.TypeInfo().DeclaringMethod;
		}

		/// <summary>Gets the attributes associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>A <see cref="T:System.Reflection.TypeAttributes" /> object representing the attribute set of the <see cref="T:System.Type" />, unless the <see cref="T:System.Type" /> represents a generic type parameter, in which case the value is unspecified. </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static TypeAttributes AttributesEx([NotNull] this Type type)
		{
			return type.TypeInfo().Attributes;
		}

		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static StructLayoutAttribute StructLayoutAttributeEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			throw new NotSupportedException();
#else
			return type.TypeInfo().StructLayoutAttribute;
#endif
		}

		#endregion

		#region Type

		/// <summary>Gets the type that declares the current nested type or generic type parameter.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing the enclosing type, if the current type is a nested type; or the generic type definition, if the current type is a type parameter of a generic type; or the type that declares the generic method, if the current type is a type parameter of a generic method; otherwise, <see langword="null" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type? DeclaringTypeEx([NotNull] this Type type)
		{
			return type.DeclaringType;
		}

		/// <summary>Gets the class object that was used to obtain this member. </summary>
		/// <returns>The <see langword="Type" /> object through which this <see cref="T:System.Type" /> object was obtained. </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type? ReflectedTypeEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_6 || NETCOREAPP1_0 || NETCOREAPP1_1
			return null;
#else
			return type.TypeInfo().ReflectedType;
#endif
		}

		/// <summary>Indicates the type provided by the common language runtime that represents this type.</summary>
		/// <returns>The underlying system type for the <see cref="T:System.Type" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type? UnderlyingSystemTypeEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return null;
#else
			return type.TypeInfo().UnderlyingSystemType;
#endif
		}

		#endregion

		#region Is Flags

		/// <summary>Gets a value that indicates whether the type is an array.</summary>
		/// <returns><see langword="true" /> if the current type is an array; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsArrayEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsArray;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is passed by reference.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is passed by reference; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsByRefEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsByRef;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a pointer.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is a pointer; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsPointerEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsPointer;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is abstract and must be overridden.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is abstract; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAbstractEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsAbstract;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" /> attribute applied, indicating that it was imported from a COM type library.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsImportEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsImport;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared sealed.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared sealed; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsSealedEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsSealed;
		}

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns><see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsSpecialNameEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsSpecialName;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a class or a delegate; that is, not a value type or interface.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a class; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsClassEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsClass;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is not declared public.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is not declared public and is not a nested type; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNotPublicEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNotPublic;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared public.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared public and is not a nested type; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsPublicEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsPublic;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out automatically by the common language runtime.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.AutoLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAutoLayoutEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsAutoLayout;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out at explicitly specified offsets.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.ExplicitLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsExplicitLayoutEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsExplicitLayout;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out sequentially, in the order that they were defined or emitted to the metadata.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.SequentialLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsLayoutSequentialEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsLayoutSequential;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAnsiClassEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsAnsiClass;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsAutoClassEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsAutoClass;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsUnicodeClassEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsUnicodeClass;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a COM object.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a COM object; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsCOMObjectEx([NotNull] this Type type)
		{
#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
			return false;
#else
			return type.TypeInfo().IsCOMObject;
#endif
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> can be hosted in a context.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> can be hosted in a context; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsContextfulEx([NotNull] this Type type)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return false;
#else
			return type.TypeInfo().IsContextful;
#endif
		}

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns><see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsEnumEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsEnum;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is marshaled by reference.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is marshaled by reference; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsMarshalByRefEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsMarshalByRef;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is one of the primitive types.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is one of the primitive types; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsPrimitiveEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsPrimitive;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a value type.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a value type; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsValueTypeEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsValueType;
		}

		#endregion

		#region Array

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> encompasses or refers to another type; that is, whether the current <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool HasElementTypeEx([NotNull] this Type type)
		{
			return type.HasElementType;
		}

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer, or reference type, or <see langword="null" /> if the current <see cref="T:System.Type" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type GetElementTypeEx([NotNull] this Type type)
		{
			return type.GetElementType();
		}

		/// <summary>Gets the number of dimensions in an array. </summary>
		/// <returns>An integer that contains the number of dimensions in the current type. </returns>
		/// <exception cref="T:System.NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead. </exception>
		/// <exception cref="T:System.ArgumentException">The current type is not an array. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static int GetArrayRankEx([NotNull] this Type type)
		{
			return type.GetArrayRank();
		}

		#endregion

		#region Generic

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or method.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericParameterEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsGenericParameter;
		}

		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericTypeParameterEx([NotNull] this Type type)
		{
#if NET20 || NET30 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462  || NET47 || NET471 || NET472 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARDLESS1_6 || NETSTANDARD2_0

			return type.IsGenericParameter && type.DeclaringMethodEx() == null;
#else
			return type.TypeInfo().IsGenericTypeParameter;
#endif
		}

		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericMethodParameterEx([NotNull] this Type type)
		{
#if NET20 || NET30 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462  || NET47 || NET471 || NET472 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARDLESS1_6 || NETSTANDARD2_0

			return type.IsGenericParameter && type.DeclaringMethodEx() != null;
#else
			return type.TypeInfo().IsGenericMethodParameter;
#endif
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

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents a generic type definition from which the current generic type can be constructed.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing a generic type from which the current type can be constructed.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type is not a generic type.  That is, <see cref="P:System.Type.IsGenericType" /> returns <see langword="false" />. </exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type GetGenericTypeDefinitionEx([NotNull] this Type type)
		{
			return type.GetGenericTypeDefinition();
		}

		/// <summary>Gets an array of the generic type arguments for this type.</summary>
		/// <returns>An array of the generic type arguments for this type.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type[] GenericTypeArgumentsEx([NotNull] this Type type)
		{
#if NET20 || NET30 || NET35 || NET40
			return (type.IsGenericType && !type.IsGenericTypeDefinition) ? type.GetGenericArguments() : new Type[0];
#else
			return type.GenericTypeArguments;
#endif
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the type arguments of a closed generic type or the type parameters of a generic type definition.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetGenericArgumentsEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			throw new NotSupportedException();
#else
			return type.TypeInfo().GetGenericArguments();
#endif
		}

		/// <summary>Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, when the <see cref="T:System.Type" /> object represents a type parameter of a generic type or a generic method.</summary>
		/// <returns>The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type does not represent a type parameter. That is, <see cref="P:System.Type.IsGenericParameter" /> returns <see langword="false" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static int GenericParameterPositionEx([NotNull] this Type type)
		{
			return type.GenericParameterPosition;
		}

		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static GenericParameterAttributes GenericParameterAttributesEx([NotNull] this Type type)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			throw new NotSupportedException();
#else
			return type.GenericParameterAttributes;
#endif
		}

		#endregion

		#region Constructor

		/// <summary>Gets the initializer for the type.</summary>
		/// <returns>An object that contains the name of the class constructor for the <see cref="T:System.Type" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo TypeInitializerEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredConstructors.FirstOrDefault(c => c.IsStatic);
#else
			return type.TypeInfo().TypeInitializer;
#endif
		}

		/// <summary>Searches for a public instance constructor whose parameters match the types in the specified array.</summary>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the desired constructor.-or- An empty array of <see cref="T:System.Type" /> objects, to get a constructor that takes no parameters. Such an empty array is provided by the <see langword="static" /> field <see cref="F:System.Type.EmptyTypes" />.</param>
		/// <returns>An object representing the public instance constructor whose parameters match the types in the parameter type array, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx([NotNull] this Type type, Type[] types)
		{
#if NETSTANDARDLESS1_4
			throw new NotImplementedException();
#else
			return type.TypeInfo().GetConstructor(types);
#endif
		}

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the constructor to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.-or-
		/// <see cref="F:System.Type.EmptyTypes" />. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the parameter type array. The default binder does not process this parameter. </param>
		/// <returns>A <see cref="T:System.Reflection.ConstructorInfo" /> object representing the constructor that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.-or-
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx(
			[NotNull] this Type type, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetConstructor(bindingAttr, binder, types, modifiers);
		}

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. </param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and the stack is cleaned up. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the constructor to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter. </param>
		/// <returns>An object representing the constructor that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.-or-
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx(
			[NotNull] this Type type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
		}

#endif

		/// <summary>Returns all the public constructors defined for the current <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// An array of <see cref="T:System.Reflection.ConstructorInfo" /> objects representing all the public instance constructors defined for the current <see cref="T:System.Type" />,
		/// but not including the type initializer (static constructor). If no public instance constructors are defined for the current <see cref="T:System.Type" />,
		/// or if the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or generic method,
		/// an empty array of type <see cref="T:System.Reflection.ConstructorInfo" /> is returned.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo[] GetConstructorsEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredConstructors.ToArray();
#else
			return type.TypeInfo().GetConstructors();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the constructors defined for the current <see cref="T:System.Type" />, using the specified <see langword="BindingFlags" />.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>
		/// An array of <see cref="T:System.Reflection.ConstructorInfo" /> objects representing all constructors defined for the current <see cref="T:System.Type" />
		/// that match the specified binding constraints, including the type initializer if it is defined. Returns an empty array of type <see cref="T:System.Reflection.ConstructorInfo" />
		/// if no constructors are defined for the current <see cref="T:System.Type" />, if none of the defined constructors match the binding constraints,
		/// or if the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or generic method.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo[] GetConstructorsEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetConstructors(bindingAttr);
		}

#endif

		#endregion

		#region Event

		/// <summary>Returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified public event.</summary>
		/// <param name="name">The string containing the name of an event that is declared or inherited by the current <see cref="T:System.Type" />.</param>
		/// <returns>The object representing the specified public event that is declared or inherited by the current <see cref="T:System.Type" />,
		/// if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static EventInfo? GetEventEx([NotNull] this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().GetDeclaredEvent(name);
#else
			return type.TypeInfo().GetEvent(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified event, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of an event which is declared or inherited by the current <see cref="T:System.Type" />. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>The object representing the specified event that is declared or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static EventInfo GetEventEx([NotNull] this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetEvent(name, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for events that are declared or inherited by the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all events that are declared or inherited by the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have events, or if none of the events match the binding constraints.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static EventInfo[] GetEventsEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetEvents(bindingAttr);
		}

#endif

		/// <summary>Returns all the public events that are declared or inherited by the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all the public events which are declared or inherited by the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have public events.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static EventInfo[] GetEventsEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredEvents.ToArray();
#else
			return type.TypeInfo().GetEvents();
#endif
		}

		#endregion

		#region Field

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified field, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the data field to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the field that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo GetGetFieldEx([NotNull] this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetField(name, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for the fields defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all fields defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no fields are defined for the current <see cref="T:System.Type" />, or if none of the defined fields match the binding constraints.</returns>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo[] GetFieldsEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetFields(bindingAttr);
		}

#endif

		/// <summary>Searches for the public field with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public field to get. </param>
		/// <returns>An object representing the public field with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one field is found with the specified name. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo? GetFieldEx([NotNull] this Type type, [NotNull] string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredField(name);
#else
			return type.TypeInfo().GetField(name);
#endif
		}

		/// <summary>Returns all the public fields of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all the public fields defined for the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no public fields are defined for the current <see cref="T:System.Type" />.</returns>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo[] GetFieldsEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredFields.ToArray();
#else
			return type.TypeInfo().GetFields();
#endif
		}

		#endregion

		#region Member

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified members, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the members to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx([NotNull] this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMember(name, bindingAttr);
		}

		/// <summary>Searches for the specified members of the specified member type, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the members to get. </param>
		/// <param name="memberTypes">The value to search for. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">A derived class must provide an implementation. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx([NotNull] this Type type, string name, MemberTypes memberTypes, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMember(name, memberTypes, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for the members defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero (<see cref="F:System.Reflection.BindingFlags.Default" />), to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all members defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if no members are defined for the current <see cref="T:System.Type" />, or if none of the defined members match the binding constraints.</returns>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMembersEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMembers(bindingAttr);
		}

#endif

		/// <summary>Searches for the public members with the specified name.</summary>
		/// <param name="name">The string containing the name of the public members to get. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx([NotNull] this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMembers.Where(m => m.Name == name).ToArray();
#else
			return type.TypeInfo().GetMember(name);
#endif
		}

		/// <summary>Returns all the public members of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all the public members of the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if the current <see cref="T:System.Type" /> does not have public members.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMembersEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMembers.ToArray();
#else
			return type.TypeInfo().GetMembers();
#endif
		}

		#endregion

		#region Method

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified method, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the method to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo GetMethodEx([NotNull] this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr);
		}

#endif

		/// <summary>Searches for the public method with the specified name.</summary>
		/// <param name="name">The string containing the name of the public method to get. </param>
		/// <returns>An object that represents the public method with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx([NotNull] this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredMethod(name);
#else
			return type.TypeInfo().GetMethod(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified public method whose parameters match the specified argument types.</summary>
		/// <param name="name">The string containing the name of the public method to get. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.-or- An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters. </param>
		/// <returns>An object representing the public method whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and specified parameters. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx([NotNull] this Type type, string name, Type[] types)
		{
			return type.TypeInfo().GetMethod(name, types);
		}

		/// <summary>Searches for the specified public method whose parameters match the specified argument types and modifiers.</summary>
		/// <param name="name">The string containing the name of the public method to get. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.-or- An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the public method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and specified parameters. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx([NotNull] this Type type, string name, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, types, modifiers);
		}

#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the method to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.-or- An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(
			[NotNull] this Type type, string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr, binder, types, modifiers);
		}

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="name">The string containing the name of the method to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. </param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and how the stack is cleaned up. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.-or- An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(
			[NotNull] this Type type, string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

#endif

		/// <summary>Returns all the public methods of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all the public methods defined for the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no public methods are defined for the current <see cref="T:System.Type" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo[] GetMethodsEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMethods.ToArray();
#else
			return type.TypeInfo().GetMethods();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the methods defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all methods defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no methods are defined for the current <see cref="T:System.Type" />, or if none of the defined methods match the binding constraints.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo[] GetMethodsEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMethods(bindingAttr);
		}

#endif

		#endregion

		#region Nested

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object represents a type whose definition is nested inside the definition of another type.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested inside another type; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedEx([NotNull] this Type type)
		{
			return type.IsNested;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own assembly.</summary>
		/// <returns> <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own assembly; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedAssemblyEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedAssembly;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamANDAssemEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedFamANDAssem;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own family.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own family; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamilyEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedFamily;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to either its own family or to its own assembly.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to its own family or to its own assembly; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamORAssemEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedFamORAssem;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and declared private.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and declared private; otherwise, <see langword="false" />.
		/// </returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedPrivateEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedPrivate;
		}

		/// <summary>Gets a value indicating whether a class is nested and declared public.</summary>
		/// <returns><see langword="true" /> if the class is nested and declared public; otherwise, <see langword="false" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedPublicEx([NotNull] this Type type)
		{
			return type.TypeInfo().IsNestedPublic;
		}

		/// <summary>Searches for the public nested type with the specified name.</summary>
		/// <param name="name">The string containing the name of the nested type to get. </param>
		/// <returns>An object representing the public nested type with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type GetNestedTypeEx([NotNull] this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredNestedTypes.FirstOrDefault(t => t.Name == name)?.AsType();
#else
			return type.TypeInfo().GetNestedType(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the specified nested type, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the nested type to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the nested type that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type GetNestedTypeEx([NotNull] this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetNestedType(name, bindingAttr);
		}

#endif

		/// <summary>Returns the public types nested in the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing the public types nested in the current <see cref="T:System.Type" /> (the search is not recursive), or an empty array of type <see cref="T:System.Type" /> if no public types are nested in the current <see cref="T:System.Type" />.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetNestedTypesEx([NotNull] this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredNestedTypes.Select(t => t.AsType()).ToArray();
#else
			return type.TypeInfo().GetNestedTypes();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the types nested in the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing all the types nested in the current <see cref="T:System.Type" /> that match the specified binding constraints (the search is not recursive), or an empty array of type <see cref="T:System.Type" />, if no nested types are found that match the binding constraints.</returns>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetNestedTypesEx([NotNull] this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetNestedTypes(bindingAttr);
		}

#endif

		#endregion

		#region Property

		/// <summary>Searches for the public property with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure, CanBeNull]
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo? GetPropertyEx([NotNull] this Type type, [NotNull] string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredProperty(name);
#else
			return type.TypeInfo().GetProperty(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified property, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the property to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx([NotNull] this Type type, [NotNull] string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetProperty(name, bindingAttr);
		}

#endif

		/// <summary>Searches for the public property with the specified name and return type.</summary>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <param name="returnType">The return type of the property. </param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />, or <paramref name="returnType" /> is <see langword="null" />. </exception>
		[Pure]
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx([NotNull] this Type type, [NotNull] string name, Type returnType)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredProperties.FirstOrDefault(p => p.Name == name && p.PropertyType == returnType);

#else
			return type.TypeInfo().GetProperty(name, returnType);
#endif
		}

		#endregion

/*
		public PropertyInfo GetProperty(string name, Type[] types) => GetProperty(name, null, types);
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types) => GetProperty(name, returnType, types, null);
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers) => GetProperty(name, Type.DefaultLookup, null, returnType, types, modifiers);
		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (types == null)
				throw new ArgumentNullException(nameof(types));
			return GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		public PropertyInfo[] GetProperties() => GetProperties(Type.DefaultLookup);
		public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

		public virtual MemberInfo[] GetDefaultMembers() { throw NotImplemented.ByDesign; }

		public virtual RuntimeTypeHandle TypeHandle { get { throw new NotSupportedException(); } }
		public static RuntimeTypeHandle GetTypeHandle(object o)
		{
			if (o == null)
				throw new ArgumentNullException(null, SR.Arg_InvalidHandle);
			Type type = o.GetType();
			return type.TypeHandle;
		}

		public static Type[] GetTypeArray(object[] args)
		{
			if (args == null)
				throw new ArgumentNullException(nameof(args));

			Type[] cls = new Type[args.Length];
			for (int i = 0; i < cls.Length; i++)
			{
				if (args[i] == null)
					throw new ArgumentNullException();
				cls[i] = args[i].GetType();
			}
			return cls;
		}

		public static TypeCode GetTypeCode(Type type)
		{
			if (type == null)
				return TypeCode.Empty;
			return type.GetTypeCodeImpl();
		}
		protected virtual TypeCode GetTypeCodeImpl()
		{
			Type systemType = UnderlyingSystemType;
			if (this != systemType && systemType != null)
				return Type.GetTypeCode(systemType);

			return TypeCode.Object;
		}

		public abstract Guid GUID { get; }

		public static Type GetTypeFromCLSID(Guid clsid) => GetTypeFromCLSID(clsid, null, throwOnError: false);
		public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError) => GetTypeFromCLSID(clsid, null, throwOnError: throwOnError);
		public static Type GetTypeFromCLSID(Guid clsid, string server) => GetTypeFromCLSID(clsid, server, throwOnError: false);

		public static Type GetTypeFromProgID(string progID) => GetTypeFromProgID(progID, null, throwOnError: false);
		public static Type GetTypeFromProgID(string progID, bool throwOnError) => GetTypeFromProgID(progID, null, throwOnError: throwOnError);
		public static Type GetTypeFromProgID(string progID, string server) => GetTypeFromProgID(progID, server, throwOnError: false);

		public abstract Type BaseType { get; }

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args) => InvokeMember(name, invokeAttr, binder, target, args, null, null, null);

		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture) => InvokeMember(name, invokeAttr, binder, target, args, null, culture, null);
		public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		public Type GetInterface(string name) => GetInterface(name, ignoreCase: false);
		public abstract Type GetInterface(string name, bool ignoreCase);
		public abstract Type[] GetInterfaces();

		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType) { throw new NotSupportedException(SR.NotSupported_SubclassOverride); }

		public virtual bool IsInstanceOfType(object o) => o == null ? false : IsAssignableFrom(o.GetType());
		public virtual bool IsEquivalentTo(Type other) => this == other;

		public virtual Type GetEnumUnderlyingType()
		{
			if (!IsEnum)
				throw new ArgumentException(SR.Arg_MustBeEnum, "enumType");

			FieldInfo[] fields = GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			if (fields == null || fields.Length != 1)
				throw new ArgumentException(SR.Argument_InvalidEnum, "enumType");

			return fields[0].FieldType;
		}
		public virtual Array GetEnumValues()
		{
			if (!IsEnum)
				throw new ArgumentException(SR.Arg_MustBeEnum, "enumType");

			// We don't support GetEnumValues in the default implementation because we cannot create an array of
			// a non-runtime type. If there is strong need we can consider returning an object or int64 array.
			throw NotImplemented.ByDesign;
		}

		public virtual Type MakeArrayType() { throw new NotSupportedException(); }
		public virtual Type MakeArrayType(int rank) { throw new NotSupportedException(); }
		public virtual Type MakeByRefType() { throw new NotSupportedException(); }
		public virtual Type MakeGenericType(params Type[] typeArguments) { throw new NotSupportedException(SR.NotSupported_SubclassOverride); }
		public virtual Type MakePointerType() { throw new NotSupportedException(); }

		public static Type MakeGenericSignatureType(Type genericTypeDefinition, params Type[] typeArguments) => new SignatureConstructedGenericType(genericTypeDefinition, typeArguments);

		public static Type MakeGenericMethodParameter(int position)
		{
			if (position < 0)
				throw new ArgumentException(SR.ArgumentOutOfRange_NeedNonNegNum, nameof(position));
			return new SignatureGenericMethodParameterType(position);
		}

		public override string ToString() => "Type: " + Name;  // Why do we add the "Type: " prefix?

		public override bool Equals(object o) => o == null ? false : Equals(o as Type);
		public override int GetHashCode()
		{
			Type systemType = UnderlyingSystemType;
			if (!object.ReferenceEquals(systemType, this))
				return systemType.GetHashCode();
			return base.GetHashCode();
		}
		public virtual bool Equals(Type o) => o == null ? false : object.ReferenceEquals(this.UnderlyingSystemType, o.UnderlyingSystemType);

		public static Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase) { throw new PlatformNotSupportedException(SR.PlatformNotSupported_ReflectionOnly); }

		public static Binder DefaultBinder
		{
			get
			{
				if (s_defaultBinder == null)
				{
					DefaultBinder binder = new DefaultBinder();
					Interlocked.CompareExchange<Binder>(ref s_defaultBinder, binder, null);
				}
				return s_defaultBinder;
			}
		}

		public static readonly char Delimiter = '.';
		public static readonly Type[] EmptyTypes = Array.Empty<Type>();
		public static readonly object Missing = System.Reflection.Missing.Value;

		public static readonly MemberFilter FilterAttribute = FilterAttributeImpl;
		public static readonly MemberFilter FilterName = (m, c) => FilterNameImpl(m, c, StringComparison.Ordinal);
		public static readonly MemberFilter FilterNameIgnoreCase = (m, c) => FilterNameImpl(m, c, StringComparison.OrdinalIgnoreCase);
*/

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

		[MethodImpl(AggressiveInlining)]
		public static object? GetPropertyValueEx([NotNull] this Type type, [NotNull] string propertyName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
#if NETCOREAPP1_0 || NETCOREAPP1_1
			if (property == null) throw new MissingFieldException($"The field {propertyName} cannot be found.");
#else
			if (property == null) throw new Exception($"The property {propertyName} cannot be found.");
#endif
			return property.GetValue(target);
#else
			return type.InvokeMember(propertyName, BindingFlags.GetProperty, null, target, null);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetPropertyValueEx<T>([NotNull] this Type type, [NotNull] string propertyName, object target)
		{
#nullable disable
			return (T)GetPropertyValueEx(type, propertyName, target);
#nullable restore
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetPropertyValueEx(
			[NotNull] this Type type, [NotNull] string propertyName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var property = type.GetTypeInfo().GetDeclaredProperty(propertyName);
#if NETCOREAPP1_0 || NETCOREAPP1_1
			if (property == null) throw new MissingFieldException($"The field {propertyName} cannot be found.");
#else
			if (property == null) throw new Exception($"The property {propertyName} cannot be found.");
#endif
			property.SetValue(target, value);
#else
			type.InvokeMember(propertyName, BindingFlags.SetProperty, null, target, new[] { value });
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static object? GetFieldValueEx([NotNull] this Type type, [NotNull] string fieldName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var field = type.GetTypeInfo().GetDeclaredField(fieldName);
#if NETCOREAPP1_0 || NETCOREAPP1_1
			if (field == null) throw new MissingFieldException($"The field {fieldName} cannot be found.");
#else
			if (field == null) throw new Exception($"The field {fieldName} cannot be found.");
#endif
			return field.GetValue(target);
#else
			return type.InvokeMember(fieldName, BindingFlags.GetField, null, target, null);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static T GetFieldValueEx<T>([NotNull] this Type type, [NotNull] string fieldName, object target)
		{
#nullable disable
			return (T)GetFieldValueEx(type, fieldName, target);
#nullable restore
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetFieldValueEx([NotNull] this Type type, [NotNull] string fieldName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var field = type.GetTypeInfo().GetDeclaredField(fieldName);
#if NETCOREAPP1_0 || NETCOREAPP1_1
			if (field == null) throw new MissingFieldException($"The field {fieldName} cannot be found.");
#else
			if (field == null) throw new Exception($"The field {fieldName} cannot be found.");
#endif
			field.SetValue(target, value);
#else
			type.InvokeMember(fieldName, BindingFlags.SetField | BindingFlags.SetProperty, null, target, new[] { value });
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static object? InvokeMethodEx([NotNull] this Type type, [NotNull] string methodName, object target, params object[] parameters)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
#nullable disable
			return method.Invoke(target, parameters);
#nullable restore
#else
#nullable disable
			return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, parameters);
#nullable restore
#endif
		}
	}
}
