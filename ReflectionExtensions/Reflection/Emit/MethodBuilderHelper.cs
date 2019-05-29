#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;

using JetBrains.Annotations;

namespace ReflectionExtensions.Reflection.Emit
{
	/// <summary>
	/// A wrapper around the <see cref="MethodBuilder"/> class.
	/// </summary>
	/// <include file="Examples.CS.xml" path='examples/emit[@name="Emit"]/*' />
	/// <include file="Examples.VB.xml" path='examples/emit[@name="Emit"]/*' />
	/// <seealso cref="System.Reflection.Emit.MethodBuilder">MethodBuilder Class</seealso>
	[PublicAPI]
	public class MethodBuilderHelper : MethodBuilderBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="typeBuilder">Associated <see cref="TypeBuilderHelper"/>.</param>
		/// <param name="methodBuilder">A <see cref="MethodBuilder"/></param>
		public MethodBuilderHelper(TypeBuilderHelper typeBuilder, MethodBuilder methodBuilder)
			: base(typeBuilder)
		{
			MethodBuilder = methodBuilder;
			methodBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
		}

		/// <summary>
		/// Sets a custom attribute using a custom attribute type.
		/// </summary>
		/// <param name="attributeType">Attribute type.</param>
		public void SetCustomAttribute(Type attributeType)
		{
			var ci        = attributeType.GetConstructor(System.Type.EmptyTypes);
			var caBuilder = new CustomAttributeBuilder(ci, new object[0]);

			MethodBuilder.SetCustomAttribute(caBuilder);
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
			var ci        = attributeType.GetConstructor(System.Type.EmptyTypes);
			var caBuilder = new CustomAttributeBuilder(ci, new object[0], properties, propertyValues);

			MethodBuilder.SetCustomAttribute(caBuilder);
		}

		/// <summary>
		/// Sets a custom attribute using a custom attribute type
		/// and named property.
		/// </summary>
		/// <param name="attributeType">Attribute type.</param>
		/// <param name="propertyName">A named property of the custom attribute.</param>
		/// <param name="propertyValue">Value for the named property of the custom attribute.</param>
		public void SetCustomAttribute(
			Type   attributeType,
			string propertyName,
			object propertyValue)
		{
			SetCustomAttribute(
				attributeType,
				new[] { attributeType.GetProperty(propertyName) },
				new[] { propertyValue });
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodBuilderHelper"/> class with the specified parameters.
		/// </summary>
		/// <param name="typeBuilder">Associated <see cref="TypeBuilderHelper"/>.</param>
		/// <param name="methodBuilder">A <see cref="MethodBuilder"/></param>
		/// <param name="genericArguments">Generic arguments of the method.</param>
		/// <param name="returnType">The return type of the method.</param>
		/// <param name="parameterTypes">The types of the parameters of the method.</param>
		internal MethodBuilderHelper(
			TypeBuilderHelper typeBuilder,
			MethodBuilder     methodBuilder,
			Type[]            genericArguments,
			Type              returnType,
			Type[]            parameterTypes
			)
			: base(typeBuilder)
		{
			MethodBuilder = methodBuilder;

			var genArgNames = genericArguments.Select(t => t.Name).ToArray();
			var genParams   = methodBuilder.DefineGenericParameters(genArgNames);

			// Copy parameter constraints.
			//
			List<Type>? interfaceConstraints = null;

			for (var i = 0; i < genParams.Length; i++)
			{
				genParams[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

				foreach (var constraint in genericArguments[i].GetGenericParameterConstraints())
				{
					if (constraint.IsClass)
						genParams[i].SetBaseTypeConstraint(constraint);
					else
					{
						if (interfaceConstraints == null)
							interfaceConstraints = new List<Type>();
						interfaceConstraints.Add(constraint);
					}
				}

				if (interfaceConstraints != null && interfaceConstraints.Count != 0)
				{
					genParams[i].SetInterfaceConstraints(interfaceConstraints.ToArray());
					interfaceConstraints.Clear();
				}
			}

			// When a method contains a generic parameter we need to replace all
			// generic types from methodInfoDeclaration with local ones.
			//
			for (var i = 0; i < parameterTypes.Length; i++)
				parameterTypes[i] = TranslateGenericParameters(parameterTypes[i], genParams);

			methodBuilder.SetParameters(parameterTypes);
			methodBuilder.SetReturnType(TranslateGenericParameters(returnType, genParams));

			// Once all generic stuff is done is it is safe to call SetCustomAttribute
			//
			methodBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
		}

		static Type TranslateGenericParameters(Type type, GenericTypeParameterBuilder[] typeArguments)
		{
			// 'T paramName' case
			//
			if (type.IsGenericParameter)
				return typeArguments[type.GenericParameterPosition];

			// 'List<T> paramName' or something like that.
			//
			if (type.IsGenericType && type.ContainsGenericParameters)
			{
				var genArgs = type.GetGenericArguments();

				for (var i = 0; i < genArgs.Length; ++i)
					genArgs[i] = TranslateGenericParameters(genArgs[i], typeArguments);

				return type.GetGenericTypeDefinition().MakeGenericType(genArgs);
			}

			// Non-generic type.
			//
			return type;
		}

		/// <summary>
		/// Gets MethodBuilder.
		/// </summary>
		public MethodBuilder MethodBuilder { get; }

		/// <summary>
		/// Converts the supplied <see cref="MethodBuilderHelper"/> to a <see cref="MethodBuilder"/>.
		/// </summary>
		/// <param name="methodBuilder">The <see cref="MethodBuilderHelper"/>.</param>
		/// <returns>A <see cref="MethodBuilder"/>.</returns>
		public static implicit operator MethodBuilder(MethodBuilderHelper methodBuilder)
		{
			return methodBuilder.MethodBuilder;
		}

		EmitHelper? _emitter;

		/// <summary>
		/// Gets <see cref="EmitHelper"/>.
		/// </summary>
		public override EmitHelper Emitter => _emitter ??= new EmitHelper(this, MethodBuilder.GetILGenerator());

		MethodInfo? _overriddenMethod;
		/// <summary>
		/// Gets or sets the base type method overridden by this method, if any.
		/// </summary>
		public MethodInfo OverriddenMethod
		{
			get => _overriddenMethod         ?? throw new InvalidOperationException();
			set => _overriddenMethod = value ?? throw new InvalidOperationException();
		}

		/// <summary>
		/// Returns the type that declares this method.
		/// </summary>
		public Type DeclaringType => MethodBuilder.DeclaringType;
	}
}

#endif
