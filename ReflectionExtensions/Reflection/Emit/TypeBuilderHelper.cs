#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

using JetBrains.Annotations;

namespace ReflectionExtensions.Reflection.Emit
{
	using TypeBuilder.Builders;

	/// <summary>
	/// A wrapper around the <see cref="TypeBuilder"/> class.
	/// </summary>
	/// <include file="Examples.CS.xml" path='examples/emit[@name="Emit"]/*' />
	/// <include file="Examples.VB.xml" path='examples/emit[@name="Emit"]/*' />
	/// <seealso cref="System.Reflection.Emit.TypeBuilder">TypeBuilder Class</seealso>
	[PublicAPI]
	public class TypeBuilderHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TypeBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="assemblyBuilder">Associated <see cref="AssemblyBuilderHelper"/>.</param>
		/// <param name="typeBuilder">A <see cref="TypeBuilder"/></param>
		public TypeBuilderHelper(AssemblyBuilderHelper assemblyBuilder, System.Reflection.Emit.TypeBuilder typeBuilder)
		{
			Assembly    = assemblyBuilder ?? throw new ArgumentNullException(nameof(assemblyBuilder));
			TypeBuilder = typeBuilder     ?? throw new ArgumentNullException(nameof(typeBuilder));

			TypeBuilder.SetCustomAttribute(Assembly.ReflectionExtensionsAttribute);
		}

		/// <summary>
		/// Gets associated <see cref="AssemblyBuilderHelper"/>.
		/// </summary>
		public AssemblyBuilderHelper Assembly { get; }

		/// <summary>
		/// Gets <see cref="System.Reflection.Emit.TypeBuilder"/>.
		/// </summary>
		public System.Reflection.Emit.TypeBuilder TypeBuilder { get; }

		/// <summary>
		/// Converts the supplied <see cref="TypeBuilderHelper"/> to a <see cref="TypeBuilder"/>.
		/// </summary>
		/// <param name="typeBuilder">The <see cref="TypeBuilderHelper"/>.</param>
		/// <returns>A <see cref="TypeBuilder"/>.</returns>
		public static implicit operator System.Reflection.Emit.TypeBuilder(TypeBuilderHelper typeBuilder)
		{
			if (typeBuilder == null) throw new ArgumentNullException(nameof(typeBuilder));

			return typeBuilder.TypeBuilder;
		}

		#region DefineMethod Overrides

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(
			string name, MethodAttributes attributes, Type returnType, params Type[] parameterTypes)
		{
			return new MethodBuilderHelper(this, TypeBuilder.DefineMethod(name, attributes, returnType, parameterTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(
			string             name,
			MethodAttributes   attributes,
			CallingConventions callingConvention,
			Type               returnType,
			Type[]             parameterTypes)
		{
			return new MethodBuilderHelper(this, TypeBuilder.DefineMethod(
				name, attributes, callingConvention, returnType, parameterTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="returnType">The return type of the method.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodAttributes attributes, Type returnType)
		{
			return new MethodBuilderHelper(
				this,
				TypeBuilder.DefineMethod(name, attributes, returnType, Type.EmptyTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodAttributes attributes)
		{
			return new MethodBuilderHelper(
				this,
				TypeBuilder.DefineMethod(name, attributes, typeof(void), Type.EmptyTypes));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		/// <param name="callingConvention">The calling convention of the method.</param>
		public MethodBuilderHelper DefineMethod(
			string             name,
			MethodAttributes   attributes,
			CallingConventions callingConvention)
		{
			return new MethodBuilderHelper(
				this,
				TypeBuilder.DefineMethod(name, attributes, callingConvention));
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
		/// <param name="genericArguments">Generic arguments of the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined generic method.</returns>
		public MethodBuilderHelper DefineGenericMethod(
			string             name,
			MethodAttributes   attributes,
			CallingConventions callingConvention,
			Type[]             genericArguments,
			Type               returnType,
			Type[]             parameterTypes)
		{
			return new MethodBuilderHelper(
				this,
				TypeBuilder.DefineMethod(name, attributes, callingConvention), genericArguments, returnType, parameterTypes);
		}

		Dictionary<MethodInfo,MethodBuilder>? _overriddenMethods;

		/// <summary>
		/// Retrieves the map of base type methods overridden by this type.
		/// </summary>
		public  Dictionary<MethodInfo,MethodBuilder>  OverriddenMethods => _overriddenMethods ??= new Dictionary<MethodInfo,MethodBuilder>();

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <param name="attributes">The attributes of the method. </param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(
			string           name,
			MethodInfo       methodInfoDeclaration,
			MethodAttributes attributes)
		{
			if (methodInfoDeclaration == null) throw new ArgumentNullException(nameof(methodInfoDeclaration));

			MethodBuilderHelper method;

			var pi         = methodInfoDeclaration.GetParameters();
			var parameters = new Type[pi.Length];

			for (var i = 0; i < pi.Length; i++)
				parameters[i] = pi[i].ParameterType;

			if (methodInfoDeclaration.ContainsGenericParameters)
			{
				method = DefineGenericMethod(
					name,
					attributes,
					methodInfoDeclaration.CallingConvention,
					methodInfoDeclaration.GetGenericArguments(),
					methodInfoDeclaration.ReturnType,
					parameters);
			}
			else
			{
				method = DefineMethod(
					name,
					attributes,
					methodInfoDeclaration.CallingConvention,
					methodInfoDeclaration.ReturnType,
					parameters);
			}

			// Compiler overrides methods only for interfaces. We do the same.
			// If we wanted to override virtual methods, then methods should have had
			// MethodAttributes.VtableLayoutMask attribute
			// and the following condition should have been used below:
			// if ((methodInfoDeclaration is FakeMethodInfo) == false)
			//
			if (methodInfoDeclaration.DeclaringType?.IsInterface == true && !(methodInfoDeclaration is FakeMethodInfo))
			{
				OverriddenMethods.Add(methodInfoDeclaration, method.MethodBuilder);
				TypeBuilder.DefineMethodOverride(method.MethodBuilder, methodInfoDeclaration);
			}

			method.OverriddenMethod = methodInfoDeclaration;

			for (var i = 0; i < pi.Length; i++)
				method.MethodBuilder.DefineParameter(i + 1, pi[i].Attributes, pi[i].Name);

			return method;
		}

		/// <summary>
		/// Adds a new method to the class, with the given name and method signature.
		/// </summary>
		/// <param name="name">The name of the method. name cannot contain embedded nulls. </param>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(string name, MethodInfo methodInfoDeclaration)
		{
			return DefineMethod(name, methodInfoDeclaration, MethodAttributes.Virtual);
		}

		/// <summary>
		/// Adds a new private method to the class.
		/// </summary>
		/// <param name="methodInfoDeclaration">The method whose declaration is to be used.</param>
		/// <returns>The defined method.</returns>
		public MethodBuilderHelper DefineMethod(MethodInfo methodInfoDeclaration)
		{
			if (methodInfoDeclaration == null) throw new ArgumentNullException(nameof(methodInfoDeclaration));

			var isInterface = methodInfoDeclaration.DeclaringType?.IsInterface == true;
			var isFake      = methodInfoDeclaration is FakeMethodInfo;

			var name = isInterface && !isFake?
				methodInfoDeclaration.DeclaringType?.FullName + "." + methodInfoDeclaration.Name:
				methodInfoDeclaration.Name;

			var attributes =
				MethodAttributes.Virtual |
				MethodAttributes.HideBySig |
				MethodAttributes.PrivateScope |
				methodInfoDeclaration.Attributes & MethodAttributes.SpecialName;

			if (isInterface && !isFake)
				attributes |= MethodAttributes.Private;
			else if ((attributes & MethodAttributes.SpecialName) != 0)
				attributes |= MethodAttributes.Public;
			else
				attributes |= methodInfoDeclaration.Attributes &
					(MethodAttributes.Public | MethodAttributes.Private);

			return DefineMethod(name, methodInfoDeclaration, attributes);
		}

		#endregion

		/// <summary>
		/// Creates a Type object for the class.
		/// </summary>
		/// <returns>Returns the new Type object for this class.</returns>
		public Type Create()
		{
			return TypeBuilder.CreateType();
		}

		/// <summary>
		/// Sets a custom attribute using a custom attribute type.
		/// </summary>
		/// <param name="attributeType">Attribute type.</param>
		public void SetCustomAttribute(Type attributeType)
		{
			var ci        = attributeType.GetConstructor(Type.EmptyTypes);
			var caBuilder = new CustomAttributeBuilder(ci, new object[0]);

			TypeBuilder.SetCustomAttribute(caBuilder);
		}

		/// <summary>
		/// Sets a custom attribute using a custom attribute type
		/// and named properties.
		/// </summary>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="properties">Named properties of the custom attribute.</param>
		/// <param name="propertyValues">Values for the named properties of the custom attribute.</param>
		public void SetCustomAttribute(
			Type           attributeType,
			PropertyInfo[] properties,
			object[]       propertyValues)
		{
			var ci        = attributeType.GetConstructor(Type.EmptyTypes);
			var caBuilder = new CustomAttributeBuilder(ci, new object[0], properties, propertyValues);

			TypeBuilder.SetCustomAttribute(caBuilder);
		}

		/// <summary>
		/// Sets a custom attribute using a custom attribute type
		/// and named property.
		/// </summary>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="propertyName">A named property of the custom attribute.</param>
		/// <param name="propertyValue">Value for the named property of the custom attribute.</param>
		public void SetCustomAttribute(Type attributeType, string propertyName, object propertyValue)
		{
			SetCustomAttribute(
				attributeType,
				new [] { attributeType.GetProperty(propertyName) },
				new [] { propertyValue } );
		}

		ConstructorBuilderHelper? _typeInitializer;
		/// <summary>
		/// Gets the initializer for this type.
		/// </summary>
		public ConstructorBuilderHelper TypeInitializer =>
			_typeInitializer ??= new ConstructorBuilderHelper(this, TypeBuilder.DefineTypeInitializer());

		/// <summary>
		/// Returns true if the initializer for this type has a body.
		/// </summary>
		public bool IsTypeInitializerDefined => _typeInitializer != null;

		ConstructorBuilderHelper? _defaultConstructor;
		/// <summary>
		/// Gets the default constructor for this type.
		/// </summary>
		public  ConstructorBuilderHelper  DefaultConstructor
		{
			get
			{
				if (_defaultConstructor == null)
				{
					var builder = TypeBuilder.DefineConstructor(
						MethodAttributes.Public,
						CallingConventions.Standard,
						Type.EmptyTypes);

					_defaultConstructor = new ConstructorBuilderHelper(this, builder);
				}

				return _defaultConstructor;
			}
		}

		/// <summary>
		/// Returns true if the default constructor for this type has a body.
		/// </summary>
		public bool IsDefaultConstructorDefined => _defaultConstructor != null;

		ConstructorBuilderHelper? _initConstructor;
		/// <summary>
		/// Gets the init context constructor for this type.
		/// </summary>
		public ConstructorBuilderHelper InitConstructor
		{
			get
			{
				if (_initConstructor == null)
				{
					var builder = TypeBuilder.DefineConstructor(
						MethodAttributes.Public,
						CallingConventions.Standard,
						new[] { typeof(InitContext) });

					_initConstructor = new ConstructorBuilderHelper(this, builder);
				}

				return _initConstructor;
			}
		}

		/// <summary>
		/// Returns true if a constructor with parameter of <see cref="InitContext"/> for this type has a body.
		/// </summary>
		public bool IsInitConstructorDefined => _initConstructor != null;

		/// <summary>
		/// Adds a new field to the class, with the given name, attributes and field type.
		/// </summary>
		/// <param name="fieldName">The name of the field. <paramref name="fieldName"/> cannot contain embedded nulls.</param>
		/// <param name="type">The type of the field.</param>
		/// <param name="attributes">The attributes of the field.</param>
		/// <returns>The defined field.</returns>
		public FieldBuilder DefineField(
			string          fieldName,
			Type            type,
			FieldAttributes attributes)
		{
			return TypeBuilder.DefineField(fieldName, type, attributes);
		}

		#region DefineConstructor Overrides

		/// <summary>
		/// Adds a new public constructor to the class, with the given parameters.
		/// </summary>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined constructor.</returns>
		public ConstructorBuilderHelper DefinePublicConstructor(params Type[] parameterTypes)
		{
			return new ConstructorBuilderHelper(
				this,
				TypeBuilder.DefineConstructor(
					MethodAttributes.Public, CallingConventions.Standard, parameterTypes));
		}

		/// <summary>
		/// Adds a new constructor to the class, with the given attributes and parameters.
		/// </summary>
		/// <param name="attributes">The attributes of the field.</param>
		/// <param name="callingConvention">The <see cref="CallingConventions">calling convention</see> of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		/// <returns>The defined constructor.</returns>
		public ConstructorBuilderHelper DefineConstructor(
			MethodAttributes   attributes,
			CallingConventions callingConvention,
			params Type[]      parameterTypes)
		{
			return new ConstructorBuilderHelper(
				this,
				TypeBuilder.DefineConstructor(attributes, callingConvention, parameterTypes));
		}

		#endregion

		#region DefineNestedType Overrides

		/// <summary>
		/// Defines a nested type given its name..
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name)
		{
			return new TypeBuilderHelper(Assembly, TypeBuilder.DefineNestedType(name));
		}

		/// <summary>
		/// Defines a public nested type given its name and the type that it extends.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(string name, Type parent)
		{
			return new TypeBuilderHelper(
				Assembly,
				TypeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent));
		}

		/// <summary>
		/// Defines a nested type given its name, attributes, and the type that it extends.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="attributes">The attributes of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type)">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(
			string         name,
			TypeAttributes attributes,
			Type           parent)
		{
			return new TypeBuilderHelper(
				Assembly,
				TypeBuilder.DefineNestedType(name, attributes, parent));
		}

		/// <summary>
		/// Defines a public nested type given its name, the type that it extends, and the interfaces that it implements.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <param name="interfaces">The interfaces that the nested type implements.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.TypeBuilder.DefineNestedType(string,TypeAttributes,Type,Type[])">
		/// TypeBuilder.DefineNestedType Method</seealso>
		public TypeBuilderHelper DefineNestedType(
			string        name,
			Type          parent,
			params Type[] interfaces)
		{
			return new TypeBuilderHelper(
				Assembly,
				TypeBuilder.DefineNestedType(name, TypeAttributes.NestedPublic, parent, interfaces));
		}

		/// <summary>
		/// Defines a nested type given its name, attributes, the type that it extends, and the interfaces that it implements.
		/// </summary>
		/// <param name="name">The short name of the type.</param>
		/// <param name="attributes">The attributes of the type.</param>
		/// <param name="parent">The type that the nested type extends.</param>
		/// <param name="interfaces">The interfaces that the nested type implements.</param>
		/// <returns>Returns the created <see cref="TypeBuilderHelper"/>.</returns>
		/// <seealso cref="System.Reflection.Emit.ModuleBuilder.DefineType(string,TypeAttributes,Type,Type[])">ModuleBuilder.DefineType Method</seealso>
		public TypeBuilderHelper DefineNestedType(
			string         name,
			TypeAttributes attributes,
			Type           parent,
			params         Type[] interfaces)
		{
			return new TypeBuilderHelper(
				Assembly,
				TypeBuilder.DefineNestedType(name, attributes, parent, interfaces));
		}

		#endregion
	}
}

#endif
