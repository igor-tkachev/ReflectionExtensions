#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4
#define NETSTANDARDLESS1_4
#endif
#if NETSTANDARDLESS1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
#define NETSTANDARDLESS1_6
#endif

//#nullable disable

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ReSharper disable CommentTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace ReflectionExtensions
{
	public static partial class ReflectionExtensions
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

		#region Type.Common

		/// <summary>Gets a <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</summary>
		/// <returns>A <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MemberTypes MemberTypeEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return MemberTypes.TypeInfo;
#else
			return type.TypeInfo().MemberType;
#endif
		}

		/// <summary>Gets the namespace of the <see cref="T:System.Type" />.</summary>
		/// <returns>The namespace of the <see cref="T:System.Type" />; <see langword="null" /> if the current instance has no namespace or represents a generic parameter.</returns>
		[MethodImpl(AggressiveInlining)]
		public static string NamespaceEx(this Type type)
		{
			return type.Namespace;
		}

		/// <summary>Gets the assembly-qualified name of the type, which includes the name of the assembly from which this <see cref="T:System.Type" /> object was loaded.</summary>
		/// <returns>The assembly-qualified name of the <see cref="T:System.Type" />, which includes the name of the assembly from which the <see cref="T:System.Type" /> was loaded, or <see langword="null" /> if the current instance represents a generic type parameter.</returns>
		[MethodImpl(AggressiveInlining)]
		public static string AssemblyQualifiedNameEx(this Type type)
		{
			return type.AssemblyQualifiedName;
		}

		/// <summary>Gets the fully qualified name of the type, including its namespace but not its assembly. </summary>
		/// <returns>The fully qualified name of the type, including its namespace but not its assembly; or <see langword="null" /> if the current instance represents a generic type parameter, an array type, pointer type, or <see langword="byref" /> type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.</returns>
		[MethodImpl(AggressiveInlining)]
		public static string FullNameEx(this Type type)
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
		[MethodImpl(AggressiveInlining)]
		public static Assembly AssemblyEx(this Type type)
		{
			return type.TypeInfo().Assembly;
		}

		/// <summary>Gets the module (the DLL) in which the current <see cref="T:System.Type" /> is defined.</summary>
		/// <returns>The module in which the current <see cref="T:System.Type" /> is defined.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Module ModuleEx(this Type type)
		{
			return type.TypeInfo().Module;
		}

		/// <summary>
		/// Gets a <see cref="T:System.Reflection.MethodBase" /> that represents the declaring method, if the current <see cref="T:System.Type" /> represents a type parameter of a generic method.</summary>
		/// <returns>If the current <see cref="T:System.Type" /> represents a type parameter of a generic method, a <see cref="T:System.Reflection.MethodBase" /> that represents declaring method; otherwise, <see langword="null" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MethodBase DeclaringMethodEx(this Type type)
		{
			return type.TypeInfo().DeclaringMethod;
		}

		/// <summary>Gets the attributes associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>A <see cref="T:System.Reflection.TypeAttributes" /> object representing the attribute set of the <see cref="T:System.Type" />, unless the <see cref="T:System.Type" /> represents a generic type parameter, in which case the value is unspecified. </returns>
		[MethodImpl(AggressiveInlining)]
		public static TypeAttributes AttributesEx(this Type type)
		{
			return type.TypeInfo().Attributes;
		}

		[MethodImpl(AggressiveInlining)]
		public static StructLayoutAttribute StructLayoutAttributeEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			throw new NotSupportedException();
#else
			return type.TypeInfo().StructLayoutAttribute;
#endif
		}

		/// <summary>Gets the GUID associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>The GUID associated with the <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Guid GUIDEx(this Type type)
		{
			return type.TypeInfo().GUID;
		}

		/// <summary>Gets the type from which the current <see cref="T:System.Type" /> directly inherits.</summary>
		/// <returns>The <see cref="T:System.Type" /> from which the current <see cref="T:System.Type" /> directly inherits, or <see langword="null" /> if the current <see langword="Type" /> represents the <see cref="T:System.Object" /> class or an interface.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type BaseTypeEx(this Type type)
		{
			return type.TypeInfo().BaseType;
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
		[MethodImpl(AggressiveInlining)]
		public static bool IsAssignableFromEx(this Type type, Type c)
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
		[MethodImpl(AggressiveInlining)]
		public static bool IsSubclassOfEx(this Type type, Type c)
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
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx(this Type type, Type attributeType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
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
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx(this Type type, Type attributeType, bool inherit)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return type.GetTypeInfo().IsDefined(attributeType, inherit);
#else
			return Attribute.IsDefined(type, attributeType, inherit);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static bool IsInstanceOfTypeEx(this Type type, object o)
		{
#if NETSTANDARDLESS1_4
			return o != null && type.IsAssignableFromEx(o.GetType());
#else
			return type.TypeInfo().IsInstanceOfType(o);
#endif
		}

		#endregion

		#region Type.Type

		/// <summary>Gets the type that declares the current nested type or generic type parameter.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing the enclosing type, if the current type is a nested type; or the generic type definition, if the current type is a type parameter of a generic type; or the type that declares the generic method, if the current type is a type parameter of a generic method; otherwise, <see langword="null" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type DeclaringTypeEx(this Type type)
		{
			return type.DeclaringType;
		}

		/// <summary>Gets the class object that was used to obtain this member. </summary>
		/// <returns>The <see langword="Type" /> object through which this <see cref="T:System.Type" /> object was obtained. </returns>
		[MethodImpl(AggressiveInlining)]
		public static Type? ReflectedTypeEx(this Type type)
		{
#if NETSTANDARDLESS1_6 || NETCOREAPP1_0 || NETCOREAPP1_1
			return null;
#else
			return type.TypeInfo().ReflectedType;
#endif
		}

		/// <summary>Indicates the type provided by the common language runtime that represents this type.</summary>
		/// <returns>The underlying system type for the <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type? UnderlyingSystemTypeEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return null;
#else
			return type.TypeInfo().UnderlyingSystemType;
#endif
		}

		#endregion

		#region Type.IsFlags

		/// <summary>Gets a value that indicates whether the type is an array.</summary>
		/// <returns><see langword="true" /> if the current type is an array; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsArrayEx(this Type type)
		{
			return type.TypeInfo().IsArray;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is passed by reference.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is passed by reference; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsByRefEx(this Type type)
		{
			return type.TypeInfo().IsByRef;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a pointer.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is a pointer; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsPointerEx(this Type type)
		{
			return type.TypeInfo().IsPointer;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is abstract and must be overridden.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is abstract; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsAbstractEx(this Type type)
		{
			return type.TypeInfo().IsAbstract;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" /> attribute applied, indicating that it was imported from a COM type library.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsImportEx(this Type type)
		{
			return type.TypeInfo().IsImport;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared sealed.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared sealed; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsSealedEx(this Type type)
		{
			return type.TypeInfo().IsSealed;
		}

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns><see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsSpecialNameEx(this Type type)
		{
			return type.TypeInfo().IsSpecialName;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a class or a delegate; that is, not a value type or interface.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a class; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsClassEx(this Type type)
		{
			return type.TypeInfo().IsClass;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is not declared public.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is not declared public and is not a nested type; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNotPublicEx(this Type type)
		{
			return type.TypeInfo().IsNotPublic;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared public.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is declared public and is not a nested type; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsPublicEx(this Type type)
		{
			return type.TypeInfo().IsPublic;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out automatically by the common language runtime.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.AutoLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsAutoLayoutEx(this Type type)
		{
			return type.TypeInfo().IsAutoLayout;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out at explicitly specified offsets.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.ExplicitLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsExplicitLayoutEx(this Type type)
		{
			return type.TypeInfo().IsExplicitLayout;
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out sequentially, in the order that they were defined or emitted to the metadata.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.SequentialLayout" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsLayoutSequentialEx(this Type type)
		{
			return type.TypeInfo().IsLayoutSequential;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsAnsiClassEx(this Type type)
		{
			return type.TypeInfo().IsAnsiClass;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsAutoClassEx(this Type type)
		{
			return type.TypeInfo().IsAutoClass;
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		/// <see langword="true" /> if the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsUnicodeClassEx(this Type type)
		{
			return type.TypeInfo().IsUnicodeClass;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a COM object.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a COM object; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsCOMObjectEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return false;
#else
			return type.TypeInfo().IsCOMObject;
#endif
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> can be hosted in a context.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> can be hosted in a context; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsContextfulEx(this Type type)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return false;
#else
			return type.TypeInfo().IsContextful;
#endif
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is marshaled by reference.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is marshaled by reference; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsMarshalByRefEx(this Type type)
		{
			return type.TypeInfo().IsMarshalByRef;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is one of the primitive types.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is one of the primitive types; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsPrimitiveEx(this Type type)
		{
			return type.TypeInfo().IsPrimitive;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a value type.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is a value type; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsValueTypeEx(this Type type)
		{
			return type.TypeInfo().IsValueType;
		}

		#endregion

		#region Type.Array

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> encompasses or refers to another type; that is, whether the current <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool HasElementTypeEx(this Type type)
		{
			return type.HasElementType;
		}

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer, or reference type, or <see langword="null" /> if the current <see cref="T:System.Type" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type GetElementTypeEx(this Type type)
		{
			return type.GetElementType();
		}

		/// <summary>Gets the number of dimensions in an array. </summary>
		/// <returns>An integer that contains the number of dimensions in the current type. </returns>
		/// <exception cref="T:System.NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead. </exception>
		/// <exception cref="T:System.ArgumentException">The current type is not an array. </exception>
		[MethodImpl(AggressiveInlining)]
		public static int GetArrayRankEx(this Type type)
		{
			return type.GetArrayRank();
		}

		#endregion

		#region Type.Generic

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or method.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericParameterEx(this Type type)
		{
			return type.TypeInfo().IsGenericParameter;
		}

		/// <summary>Gets a value indicating whether the current type is a generic type.</summary>
		/// <returns><see langword="true" /> if the current type is a generic type; otherwise,<see langword=" false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericTypeEx(this Type type)
		{
			return type.TypeInfo().IsGenericType;
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a generic type definition, from which other generic types can be constructed.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> object represents a generic type definition; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsGenericTypeDefinitionEx(this Type type)
		{
			return type.TypeInfo().IsGenericTypeDefinition;
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents a generic type definition from which the current generic type can be constructed.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing a generic type from which the current type can be constructed.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type is not a generic type.  That is, <see cref="P:System.Type.IsGenericType" /> returns <see langword="false" />. </exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[MethodImpl(AggressiveInlining)]
		public static Type GetGenericTypeDefinitionEx(this Type type)
		{
			return type.GetGenericTypeDefinition();
		}

		/// <summary>Gets an array of the generic type arguments for this type.</summary>
		/// <returns>An array of the generic type arguments for this type.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type[] GenericTypeArgumentsEx(this Type type)
		{
#if NET20 || NET30 || NET35 || NET40
			return type.IsGenericType && !type.IsGenericTypeDefinition ? type.GetGenericArguments() : new Type[0];
#else
			return type.GenericTypeArguments;
#endif
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the type arguments of a closed generic type or the type parameters of a generic type definition.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetGenericArgumentsEx(this Type type)
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
		[MethodImpl(AggressiveInlining)]
		public static int GenericParameterPositionEx(this Type type)
		{
			return type.GenericParameterPosition;
		}

		/// <summary>Gets a combination of <see cref="T:System.Reflection.GenericParameterAttributes" /> flags that describe the covariance and special constraints of the current generic type parameter. </summary>
		/// <returns>A bitwise combination of <see cref="T:System.Reflection.GenericParameterAttributes" /> values that describes the covariance and special constraints of the current generic type parameter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type" /> object is not a generic type parameter. That is, the <see cref="P:System.Type.IsGenericParameter" /> property returns <see langword="false" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		[MethodImpl(AggressiveInlining)]
		public static GenericParameterAttributes GenericParameterAttributesEx(this Type type)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			throw new NotSupportedException();
#else
			return type.GenericParameterAttributes;
#endif
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object has type parameters that have not been replaced by specific types.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> object is itself a generic type parameter or has type parameters
		/// for which specific types have not been supplied; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool ContainsGenericParametersEx(this Type type)
		{
			return type.TypeInfo().ContainsGenericParameters;
		}

		#endregion

		#region Type.Constructor

		/// <summary>Gets the initializer for the type.</summary>
		/// <returns>An object that contains the name of the class constructor for the <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo TypeInitializerEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredConstructors.FirstOrDefault(c => c.IsStatic);
#else
			return type.TypeInfo().TypeInitializer;
#endif
		}

		/// <summary>Searches for a public instance constructor whose parameters match the types in the specified array.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the desired constructor.-or- An empty array of <see cref="T:System.Type" /> objects, to get a constructor that takes no parameters. Such an empty array is provided by the <see langword="static" /> field <see cref="F:System.Type.EmptyTypes" />.</param>
		/// <returns>An object representing the public instance constructor whose parameters match the types in the parameter type array, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx(this Type type, Type[] types)
		{
#if NETSTANDARDLESS1_4
			throw new NotImplementedException();
#else
			return type.TypeInfo().GetConstructor(types);
#endif
		}

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
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
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx(
			this Type type, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetConstructor(bindingAttr, binder, types, modifiers);
		}

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
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
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo GetConstructorEx(
			this Type type, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
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
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo[] GetConstructorsEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredConstructors.ToArray();
#else
			return type.TypeInfo().GetConstructors();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the constructors defined for the current <see cref="T:System.Type" />, using the specified <see langword="BindingFlags" />.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>
		/// An array of <see cref="T:System.Reflection.ConstructorInfo" /> objects representing all constructors defined for the current <see cref="T:System.Type" />
		/// that match the specified binding constraints, including the type initializer if it is defined. Returns an empty array of type <see cref="T:System.Reflection.ConstructorInfo" />
		/// if no constructors are defined for the current <see cref="T:System.Type" />, if none of the defined constructors match the binding constraints,
		/// or if the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or generic method.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static ConstructorInfo[] GetConstructorsEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetConstructors(bindingAttr);
		}

#endif

		#endregion

		#region Type.Event

		/// <summary>Returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified public event.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of an event that is declared or inherited by the current <see cref="T:System.Type" />.</param>
		/// <returns>The object representing the specified public event that is declared or inherited by the current <see cref="T:System.Type" />,
		/// if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static EventInfo GetEventEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().GetDeclaredEvent(name);
#else
			return type.TypeInfo().GetEvent(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified event, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of an event which is declared or inherited by the current <see cref="T:System.Type" />. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>The object representing the specified event that is declared or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static EventInfo GetEventEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetEvent(name, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for events that are declared or inherited by the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all events that are declared or inherited by the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have events, or if none of the events match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static EventInfo[] GetEventsEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetEvents(bindingAttr);
		}

#endif

		/// <summary>Returns all the public events that are declared or inherited by the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all the public events which are declared or inherited by the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have public events.</returns>
		[MethodImpl(AggressiveInlining)]
		public static EventInfo[] GetEventsEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().DeclaredEvents.ToArray();
#else
			return type.TypeInfo().GetEvents();
#endif
		}

		#endregion

		#region Type.Field

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified field, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the data field to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the field that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo GetGetFieldEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetField(name, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for the fields defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all fields defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no fields are defined for the current <see cref="T:System.Type" />, or if none of the defined fields match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo[] GetFieldsEx(this Type type, BindingFlags bindingAttr)
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
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo GetFieldEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredField(name);
#else
			return type.TypeInfo().GetField(name);
#endif
		}

		/// <summary>Returns all the public fields of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all the public fields defined for the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no public fields are defined for the current <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static FieldInfo[] GetFieldsEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredFields.ToArray();
#else
			return type.TypeInfo().GetFields();
#endif
		}

		#endregion

		#region Type.Member

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified members, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the members to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMember(name, bindingAttr);
		}

		/// <summary>Searches for the specified members of the specified member type, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the members to get. </param>
		/// <param name="memberTypes">The value to search for. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">A derived class must provide an implementation. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx(this Type type, string name, MemberTypes memberTypes, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMember(name, memberTypes, bindingAttr);
		}

		/// <summary>When overridden in a derived class, searches for the members defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero (<see cref="F:System.Reflection.BindingFlags.Default" />), to return an empty array. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all members defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if no members are defined for the current <see cref="T:System.Type" />, or if none of the defined members match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMembersEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMembers(bindingAttr);
		}

#endif

		/// <summary>Searches for the public members with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public members to get. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMemberEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMembers.Where(m => m.Name == name).ToArray();
#else
			return type.TypeInfo().GetMember(name);
#endif
		}

		/// <summary>Returns all the public members of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all the public members of the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if the current <see cref="T:System.Type" /> does not have public members.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetMembersEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMembers.ToArray();
#else
			return type.TypeInfo().GetMembers();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the members defined for the current <see cref="T:System.Type" /> whose
		/// <see cref="T:System.Reflection.DefaultMemberAttribute" /> is set.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all default members of the current
		/// <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.MemberInfo" />,
		/// if the current <see cref="T:System.Type" /> does not have default members.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo[] GetDefaultMembersEx(this Type type)
		{
			return type.TypeInfo().GetDefaultMembers();
		}

#endif

		#endregion

		#region Type.Method

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified method, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the method to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MemberInfo GetMethodEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr);
		}

#endif

		/// <summary>Searches for the public method with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public method to get. </param>
		/// <returns>An object that represents the public method with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredMethod(name);
#else
			return type.TypeInfo().GetMethod(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified public method whose parameters match the specified argument types.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public method to get. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.-or- An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters. </param>
		/// <returns>An object representing the public method whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and specified parameters. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.-or- One of the elements in <paramref name="types" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(this Type type, string name, Type[] types)
		{
			return type.TypeInfo().GetMethod(name, types);
		}

		/// <summary>Searches for the specified public method whose parameters match the specified argument types and modifiers.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
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
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(this Type type, string name, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, types, modifiers);
		}

#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
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
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(
			this Type type, string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr, binder, types, modifiers);
		}

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
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
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetMethodEx(
			this Type type, string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
		}

#endif

		/// <summary>Returns all the public methods of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all the public methods defined for the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no public methods are defined for the current <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo[] GetMethodsEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredMethods.ToArray();
#else
			return type.TypeInfo().GetMethods();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the methods defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all methods defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no methods are defined for the current <see cref="T:System.Type" />, or if none of the defined methods match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo[] GetMethodsEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetMethods(bindingAttr);
		}

#endif

		#endregion

		#region Type.Nested

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object represents a type whose definition is nested inside the definition of another type.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested inside another type; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedEx(this Type type)
		{
			return type.IsNested;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own assembly.</summary>
		/// <returns> <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own assembly; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedAssemblyEx(this Type type)
		{
			return type.TypeInfo().IsNestedAssembly;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamANDAssemEx(this Type type)
		{
			return type.TypeInfo().IsNestedFamANDAssem;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own family.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own family; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamilyEx(this Type type)
		{
			return type.TypeInfo().IsNestedFamily;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to either its own family or to its own assembly.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to its own family or to its own assembly; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedFamORAssemEx(this Type type)
		{
			return type.TypeInfo().IsNestedFamORAssem;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and declared private.</summary>
		/// <returns>
		/// <see langword="true" /> if the <see cref="T:System.Type" /> is nested and declared private; otherwise, <see langword="false" />.
		/// </returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedPrivateEx(this Type type)
		{
			return type.TypeInfo().IsNestedPrivate;
		}

		/// <summary>Gets a value indicating whether a class is nested and declared public.</summary>
		/// <returns><see langword="true" /> if the class is nested and declared public; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsNestedPublicEx(this Type type)
		{
			return type.TypeInfo().IsNestedPublic;
		}

		/// <summary>Searches for the public nested type with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the nested type to get. </param>
		/// <returns>An object representing the public nested type with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static Type? GetNestedTypeEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredNestedTypes.FirstOrDefault(t => t.Name == name)?.AsType();
#else
			return type.TypeInfo().GetNestedType(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the specified nested type, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the nested type to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the nested type that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static Type GetNestedTypeEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetNestedType(name, bindingAttr);
		}

#endif

		/// <summary>Returns the public types nested in the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing the public types nested in the current <see cref="T:System.Type" /> (the search is not recursive), or an empty array of type <see cref="T:System.Type" /> if no public types are nested in the current <see cref="T:System.Type" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetNestedTypesEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredNestedTypes.Select(t => t.AsType()).ToArray();
#else
			return type.TypeInfo().GetNestedTypes();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the types nested in the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing all the types nested in the current <see cref="T:System.Type" /> that match the specified binding constraints (the search is not recursive), or an empty array of type <see cref="T:System.Type" />, if no nested types are found that match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static Type[] GetNestedTypesEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetNestedTypes(bindingAttr);
		}

#endif

		#endregion

		#region Type.Property

		/// <summary>Searches for the public property with the specified name.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().GetDeclaredProperty(name);
#else
			return type.TypeInfo().GetProperty(name);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified property, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the property to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints. See Remarks.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetProperty(name, bindingAttr);
		}

#endif

		/// <summary>Searches for the public property with the specified name and return type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <param name="returnType">The return type of the property. </param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />, or <paramref name="returnType" /> is <see langword="null" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, Type returnType)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredProperties.FirstOrDefault(p => p.Name == name && p.PropertyType == returnType);
#else
			return type.TypeInfo().GetProperty(name, returnType);
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified public property whose parameters match the specified argument types.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed. </param>
		/// <returns>An object representing the public property whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, Type[] types)
		{
			return type.TypeInfo().GetProperty(name, types);
		}

#endif

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified public property whose parameters match the specified argument types.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <param name="returnType">The return type of the property. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed. </param>
		/// <returns>An object representing the public property whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional. </exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, Type returnType, Type[] types)
		{
			return type.TypeInfo().GetProperty(name, returnType, types);
		}

#endif

#if !NETSTANDARDLESS1_4

		/// <summary>Searches for the specified public property whose parameters match the specified argument types and modifiers.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the public property to get. </param>
		/// <param name="returnType">The return type of the property. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter. </param>
		/// <returns>An object representing the public property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types and modifiers. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.-or-
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length. </exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetProperty(name, returnType, types, modifiers);
		}

#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Searches for the specified property whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the property to get. </param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. </param>
		/// <param name="returnType">The return type of the property. </param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.-or- An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter. </param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="name" /> is <see langword="null" />.-or-
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="types" /> is multidimensional.-or-
		/// <paramref name="modifiers" /> is multidimensional.-or-
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length. </exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo GetPropertyEx(this Type type, string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return type.TypeInfo().GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
		}

#endif

		/// <summary>Returns all the public properties of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.PropertyInfo" /> objects representing all public properties of the current <see cref="T:System.Type" />.-or- An empty array of type <see cref="T:System.Reflection.PropertyInfo" />, if the current <see cref="T:System.Type" /> does not have public properties.</returns>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo[] GetPropertiesEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.GetTypeInfo().DeclaredProperties.ToArray();
#else
			return type.TypeInfo().GetProperties();
#endif
		}

#if !NETSTANDARDLESS1_4

		/// <summary>When overridden in a derived class, searches for the properties of the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.-or- Zero, to return <see langword="null" />. </param>
		/// <returns>An array of <see cref="T:System.Reflection.PropertyInfo" /> objects representing all properties of the current <see cref="T:System.Type" /> that match the specified binding constraints.-or- An empty array of type <see cref="T:System.Reflection.PropertyInfo" />, if the current <see cref="T:System.Type" /> does not have properties, or if none of the properties match the binding constraints.</returns>
		[MethodImpl(AggressiveInlining)]
		public static PropertyInfo[] GetPropertiesEx(this Type type, BindingFlags bindingAttr)
		{
			return type.TypeInfo().GetProperties(bindingAttr);
		}

#endif

		#endregion

		#region Type.InvokeMember

		[MethodImpl(AggressiveInlining)]
		public static object InvokeMethodEx(this Type type, string methodName, object target, params object[] parameters)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			var method = type.GetTypeInfo().GetDeclaredMethod(methodName);
			return method.Invoke(target, parameters);
#else
			return type.InvokeMember(methodName, BindingFlags.InvokeMethod, null, target, parameters);
#endif
		}

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Invokes the specified member, using the specified binding constraints and matching the specified argument list.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.-or- An empty string ("") to invoke the default member. -or-For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member. </param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke. </param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="invokeAttr" /> does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute. -or-
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.-or- This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.-or- One of the named parameter arrays contains a string that is <see langword="null" />. </exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer. </exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found. </exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.-or- The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />. </exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria. </exception>
		/// <exception cref="T:System.NotSupportedException">The .NET Compact Framework does not currently support this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static object InvokeMemberEx(this Type type, string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
		{
			return type.TypeInfo().InvokeMember(name, invokeAttr, binder, target, args);
		}

#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>Invokes the specified member, using the specified binding constraints and matching the specified argument list and culture.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.-or- An empty string ("") to invoke the default member. -or-For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member. </param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke. </param>
		/// <param name="culture">The object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric <see cref="T:System.String" /> to a <see cref="T:System.Double" />.-or- A null reference (<see langword="Nothing" /> in Visual Basic) to use the current thread's <see cref="T:System.Globalization.CultureInfo" />. </param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="invokeAttr" />
		///  does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute. -or-
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.-or- This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.-or- One of the named parameter arrays contains a string that is <see langword="null" />. </exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer. </exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found. </exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.-or- The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />. </exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria. </exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static object InvokeMemberEx(this Type type, string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
		{
			return type.TypeInfo().InvokeMember(name, invokeAttr, binder, target, args, culture);
		}

#endif

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARDLESS1_6

		/// <summary>When overridden in a derived class, invokes the specified member, using the specified binding constraints and matching the specified argument list, modifiers and culture.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.-or- An empty string ("") to invoke the default member. -or-For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used. </param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.-or- A null reference (Nothing in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member. </param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke. </param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="args" /> array. A parameter's associated attributes are stored in the member's signature. The default binder processes this parameter only when calling a COM component. </param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric String to a Double.-or- A null reference (<see langword="Nothing" /> in Visual Basic) to use the current thread's <see cref="T:System.Globalization.CultureInfo" />. </param>
		/// <param name="namedParameters">An array containing the names of the parameters to which the values in the <paramref name="args" /> array are passed. </param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="invokeAttr" />
		///  does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="args" /> and <paramref name="modifiers" /> do not have the same length.-or-
		/// <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute.-or-
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.-or-
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.-or-
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.-or- The named parameter array is larger than the argument array.-or- This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.-or- One of the named parameter arrays contains a string that is <see langword="null" />. </exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer. </exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found. </exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.-or- No member can be found that has the argument names supplied in <paramref name="namedParameters" />.-or- The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />. </exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria. </exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		[MethodImpl(AggressiveInlining)]
		public static object InvokeMemberEx(this Type type,
			string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			return type.TypeInfo().InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
		}

#endif

		[MethodImpl(AggressiveInlining)]
		public static object GetPropertyValueEx(this Type type, string propertyName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
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
		public static T GetPropertyValueEx<T>(this Type type, string propertyName, object target)
		{
			return (T)GetPropertyValueEx(type, propertyName, target);
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetPropertyValueEx(
			this Type type, string propertyName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
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
		public static object GetFieldValueEx(this Type type, string fieldName, object target)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
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
		public static T GetFieldValueEx<T>(this Type type, string fieldName, object target)
		{
			return (T)GetFieldValueEx(type, fieldName, target);
		}

		[MethodImpl(AggressiveInlining)]
		public static void SetFieldValueEx(this Type type, string fieldName, object target, object value)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
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

		#endregion

		#region Type.Interface

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is an interface; that is, not a class or a value type.</summary>
		/// <returns><see langword="true" /> if the <see cref="T:System.Type" /> is an interface; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsInterfaceEx(this Type type)
		{
			return type.TypeInfo().IsInterface;
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
		[MethodImpl(AggressiveInlining)]
		public static InterfaceMapping GetInterfaceMapEx(this Type type, Type interfaceType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return type.TypeInfo().GetRuntimeInterfaceMap(interfaceType);
#else
			return type.GetInterfaceMap(interfaceType);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static Type GetInterfaceEx(this Type type, string name)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().ImplementedInterfaces.FirstOrDefault(i => i.Name == name);
#else
			return type.TypeInfo().GetInterface(name);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static Type GetInterfaceEx(this Type type, string name, bool ignoreCase)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().ImplementedInterfaces.FirstOrDefault(i => string.Compare(i.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
#else
			return type.TypeInfo().GetInterface(name, ignoreCase);
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static Type[] GetInterfacesEx(this Type type)
		{
#if NETSTANDARDLESS1_4
			return type.TypeInfo().ImplementedInterfaces.ToArray();
#else
			return type.TypeInfo().GetInterfaces();
#endif
		}

		#endregion

		#region Type.Enum

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns><see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		[MethodImpl(AggressiveInlining)]
		public static bool IsEnumEx(this Type type)
		{
			return type.TypeInfo().IsEnum;
		}

		[MethodImpl(AggressiveInlining)]
		public static Type GetEnumUnderlyingTypeEx(this Type type)
		{
#if NET20 || NET30 || NET35 || NETSTANDARDLESS1_4
			return Enum.GetUnderlyingType(type);
#else
			return type.TypeInfo().GetEnumUnderlyingType();
#endif
		}

		[MethodImpl(AggressiveInlining)]
		public static Array GetEnumValuesEx(this Type type)
		{
#if NET20 || NET30 || NET35 || NETSTANDARDLESS1_4
			return Enum.GetValues(type);
#else
			return type.TypeInfo().GetEnumValues();
#endif
		}

		#endregion

		#region Type.Make

		/// <summary>Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="typeArguments">An array of types to be substituted for the type parameters of the current generic type.</param>
		/// <returns>A <see cref="T:System.Type" /> representing the constructed type formed by substituting the elements of <paramref name="typeArguments" /> for the type parameters of the current generic type.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type does not represent a generic type definition. That is, <see cref="P:System.Type.IsGenericTypeDefinition" /> returns <see langword="false" />. </exception>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="typeArguments" /> is <see langword="null" />.-or- Any element of <paramref name="typeArguments" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in <paramref name="typeArguments" /> is not the same as the number of type parameters in the current generic type definition.-or- Any element of <paramref name="typeArguments" /> does not satisfy the constraints specified for the corresponding type parameter of the current generic type. -or-
		/// <paramref name="typeArguments" /> contains an element that is a pointer type (<see cref="P:System.Type.IsPointer" /> returns <see langword="true" />), a by-ref type (<see cref="P:System.Type.IsByRef" /> returns <see langword="true" />), or <see cref="T:System.Void" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[MethodImpl(AggressiveInlining)]
		public static Type MakeGenericTypeEx(this Type type, params Type[] typeArguments)
		{
			return type.TypeInfo().MakeGenericType(typeArguments);
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object representing a one-dimensional array of the current type, with a lower bound of zero.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing a one-dimensional array of the current type, with a lower bound of zero.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.-or-The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static Type MakeArrayTypeEx(this Type type)
		{
			return type.TypeInfo().MakeArrayType();
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object representing an array of the current type, with the specified number of dimensions.</summary>
		/// <param name="type">A <see cref="T:System.Type" /> object.</param>
		/// <param name="rank">The number of dimensions for the array. This number must be less than or equal to 32.</param>
		/// <returns>An object representing an array of the current type, with the specified number of dimensions.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		/// <paramref name="rank" /> is invalid. For example, 0 or negative.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.-or-The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />. -or-
		/// <paramref name="rank" /> is greater than 32.</exception>
		[MethodImpl(AggressiveInlining)]
		public static Type MakeArrayTypeEx(this Type type, int rank)
		{
			return type.TypeInfo().MakeArrayType(rank);
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents the current type when passed as a <see langword="ref" /> parameter (<see langword="ByRef" /> parameter in Visual Basic).</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents the current type when passed as a <see langword="ref" /> parameter (<see langword="ByRef" /> parameter in Visual Basic).</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.-or-The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static Type MakeByRefTypeEx(this Type type)
		{
			return type.TypeInfo().MakeByRefType();
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents a pointer to the current type.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents a pointer to the current type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.-or-The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />. </exception>
		[MethodImpl(AggressiveInlining)]
		public static Type MakePointerTypeEx(this Type type)
		{
			return type.TypeInfo().MakePointerType();
		}

		#endregion

		#region Type/MemberInfo.GetCustomAttribute

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <typeparamref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>(this Type element)
			where T : Attribute
		{
			return element.GetTypeInfo().GetCustomAttribute<T>();
		}

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member, and optionally inspects the ancestors of that member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <param name="inherit">
		/// <see langword="true" /> to inspect the ancestors of <paramref name="element" />; otherwise, <see langword="false" />. </param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <typeparamref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>(this Type element, bool inherit)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx(this Type element, Type attributeType)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx(this Type element, Type attributeType, bool inherit)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx(this Type element)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx(this Type element, Type attributeType, bool inherit)
		{
			return element.GetTypeInfo().GetCustomAttributes(attributeType, inherit).ToArray();
		}
#endif

		/// <summary>Retrieves a custom attribute of a specified type that is applied to a specified member.</summary>
		/// <param name="element">The member to inspect.</param>
		/// <typeparam name="T">The type of attribute to search for.</typeparam>
		/// <returns>A custom attribute that matches <typeparamref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>(this MemberInfo element)
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
		/// <returns>A custom attribute that matches <typeparamref name="T" />, or <see langword="null" /> if no such attribute is found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> is <see langword="null" />. </exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field. </exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one of the requested attributes was found. </exception>
		/// <exception cref="T:System.TypeLoadException">A custom attribute type cannot be loaded. </exception>
		[MethodImpl(AggressiveInlining)]
		public static T GetCustomAttributeEx<T>(this MemberInfo element, bool inherit)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx(this MemberInfo element, Type attributeType)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute GetCustomAttributeEx(this MemberInfo element, Type attributeType, bool inherit)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx(this MemberInfo element)
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
		[MethodImpl(AggressiveInlining)]
		public static Attribute[] GetCustomAttributesEx(this MemberInfo element, Type attributeType, bool inherit)
		{
			return element.GetCustomAttributes(attributeType, inherit).Cast<Attribute>().ToArray();
		}

		#endregion

		#region MemberInfo

		/// <summary>Determines whether any custom attributes are applied to a member of a type. Parameters specify the member, and the type of the custom attribute to search for.</summary>
		/// <param name="element">An object derived from the <see cref="T:System.Reflection.MemberInfo" /> class that describes a constructor, event, field, method, type, or property member of a class.</param>
		/// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
		/// <returns>
		/// <see langword="true" /> if a custom attribute of type <paramref name="attributeType" /> is applied to <paramref name="element" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field.</exception>
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx(this MemberInfo element, Type attributeType)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return element.IsDefined(attributeType);
#else
			return Attribute.IsDefined(element, attributeType);
#endif
		}

		/// <summary>Determines whether any custom attributes are applied to a member of a type. Parameters specify the member, the type of the custom attribute to search for, and whether to search ancestors of the member.</summary>
		/// <param name="element">An object derived from the <see cref="T:System.Reflection.MemberInfo" /> class that describes a constructor, event, field, method, type, or property member of a class.</param>
		/// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
		/// <param name="inherit">If <see langword="true" />, specifies to also search the ancestors of <paramref name="element" /> for custom attributes.</param>
		/// <returns>
		/// <see langword="true" /> if a custom attribute of type <paramref name="attributeType" /> is applied to <paramref name="element" />; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="element" /> or <paramref name="attributeType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="attributeType" /> is not derived from <see cref="T:System.Attribute" />.</exception>
		/// <exception cref="T:System.NotSupportedException">
		/// <paramref name="element" /> is not a constructor, method, property, event, type, or field.</exception>
		[MethodImpl(AggressiveInlining)]
		public static bool IsDefinedEx(this MemberInfo element, Type attributeType, bool inherit)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return element.IsDefined(attributeType, inherit);
#else
			return Attribute.IsDefined(element, attributeType, inherit);
#endif
		}

		#endregion

		#region PropertyInfo

		/// <summary>When overridden in a derived class, returns the public or non-public <see langword="get" /> accessor for this property.</summary>
		/// <param name="propertyInfo"></param>
		/// <param name="nonPublic">Indicates whether a non-public <see langword="get" /> accessor should be returned. <see langword="true" /> if a non-public accessor is to be returned; otherwise, <see langword="false" />.</param>
		/// <returns>A <see langword="MethodInfo" /> object representing the <see langword="get" /> accessor for this property, if <paramref name="nonPublic" /> is <see langword="true" />. Returns <see langword="null" /> if <paramref name="nonPublic" /> is <see langword="false" /> and the <see langword="get" /> accessor is non-public, or if <paramref name="nonPublic" /> is <see langword="true" /> but no <see langword="get" /> accessors exist.</returns>
		/// <exception cref="T:System.Security.SecurityException">The requested method is non-public and the caller does not have <see cref="T:System.Security.Permissions.ReflectionPermission" /> to reflect on this non-public method.</exception>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetGetMethodEx(this PropertyInfo propertyInfo, bool nonPublic)
		{
#if NETSTANDARDLESS1_4
			return propertyInfo.GetMethod;
#else
			return propertyInfo.GetGetMethod(nonPublic);
#endif
		}

		/// <summary>When overridden in a derived class, returns the <see langword="set" /> accessor for this property.</summary>
		/// <param name="propertyInfo"></param>
		/// <param name="nonPublic">Indicates whether the accessor should be returned if it is non-public. <see langword="true" /> if a non-public accessor is to be returned; otherwise, <see langword="false" />.</param>
		/// <returns>This property's <see langword="Set" /> method, or <see langword="null" />, as shown in the following table.
		///  Value
		///
		///  Condition
		///
		///  The <see langword="Set" /> method for this property.
		///
		///  The <see langword="set" /> accessor is public.
		///
		/// -or-
		///
		/// <paramref name="nonPublic" /> is <see langword="true" /> and the <see langword="set" /> accessor is non-public.
		///
		/// <see langword="null" /><paramref name="nonPublic" /> is <see langword="true" />, but the property is read-only.
		///
		/// -or-
		///
		/// <paramref name="nonPublic" /> is <see langword="false" /> and the <see langword="set" /> accessor is non-public.
		///
		/// -or-
		///
		/// There is no <see langword="set" /> accessor.</returns>
		/// <exception cref="T:System.Security.SecurityException">The requested method is non-public and the caller does not have <see cref="T:System.Security.Permissions.ReflectionPermission" /> to reflect on this non-public method.</exception>
		[MethodImpl(AggressiveInlining)]
		public static MethodInfo GetSetMethodEx(this PropertyInfo propertyInfo, bool nonPublic)
		{
#if NETSTANDARDLESS1_4
			return propertyInfo.SetMethod;
#else
			return propertyInfo.GetSetMethod(nonPublic);
#endif
		}

		#endregion

		#region Extensions

		/// <summary>
		/// Determines whether the specified types are considered equal.
		/// </summary>
		/// <param name="parent">A <see cref="System.Type"/> instance. </param>
		/// <param name="child">A type possible derived from the <c>parent</c> type</param>
		/// <returns>True, when an object instance of the type <c>child</c>
		/// can be used as an object of the type <c>parent</c>; otherwise, false.</returns>
		/// <remarks>Note that nullable types does not have a parent-child relation to it's underlying type.
		/// For example, the 'int?' type (nullable int) and the 'int' type
		/// aren't a parent and it's child.</remarks>
		public static bool IsSameOrParentOf(this Type parent, Type child)
		{
			if (parent == child ||
				child.IsEnumEx() && Enum.GetUnderlyingType(child) == parent ||
				child.IsSubclassOfEx(parent))
			{
				return true;
			}

			if (parent.IsGenericTypeDefinitionEx())
				for (var t = child; t != typeof(object) && t != null; t = t.BaseTypeEx())
					if (t.IsGenericTypeEx() && t.GetGenericTypeDefinition() == parent)
						return true;

			if (parent.IsInterfaceEx())
			{
				var interfaces = child.GetInterfacesEx();

				foreach (var t in interfaces)
				{
					if (parent.IsGenericTypeDefinitionEx())
					{
						if (t.IsGenericTypeEx() && t.GetGenericTypeDefinition() == parent)
							return true;
					}
					else if (t == parent)
						return true;
				}
			}

			return false;
		}

		public static bool IsNullableType(Type type)
		{
			return type.IsGenericTypeEx() && type.GetGenericTypeDefinitionEx() == typeof(Nullable<>);
		}

		/// <summary>
		/// Returns the underlying type argument of the specified type.
		/// </summary>
		/// <param name="type">A <see cref="System.Type"/> instance. </param>
		/// <returns><list>
		/// <item>The type argument of the type parameter,
		/// if the type parameter is a closed generic nullable type.</item>
		/// <item>The underlying Type if the type parameter is an enum type.</item>
		/// <item>Otherwise, the type itself.</item>
		/// </list>
		/// </returns>
		public static Type GetUnderlyingType(this Type type)
		{
			if (IsNullableType(type))
				type = Nullable.GetUnderlyingType(type) ?? type;

			if (type.IsEnumEx())
				type = Enum.GetUnderlyingType(type);

			return type;
		}

		#endregion
	}

	public static partial class AssemblyExtensions
	{
		#region Assembly

#if !NETSTANDARDLESS1_4

		/// <summary>Loads the contents of an assembly file on the specified path.</summary>
		/// <param name="path">The fully qualified path of the file to load. </param>
		/// <returns>The loaded assembly.</returns>
		/// <exception cref="T:System.ArgumentException">The <paramref name="path" /> argument is not an absolute path. </exception>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="path" /> parameter is <see langword="null" />. </exception>
		/// <exception cref="T:System.IO.FileLoadException">A file that was found could not be loaded. </exception>
		/// <exception cref="T:System.IO.FileNotFoundException">The <paramref name="path" /> parameter is an empty string ("") or does not exist. </exception>
		/// <exception cref="T:System.BadImageFormatException">
		/// <paramref name="path" /> is not a valid assembly. -or-Version 2.0 or later of the common language runtime is currently loaded and <paramref name="path" /> was compiled with a later version.</exception>
		public static Assembly LoadFileEx(string path)
		{
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARDLESS1_6
			return System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
#else
			return Assembly.LoadFile(path);
#endif
		}

#endif

		#endregion
	}
}
