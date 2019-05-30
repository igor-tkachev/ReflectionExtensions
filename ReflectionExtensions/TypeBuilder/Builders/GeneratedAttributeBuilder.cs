#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	class GeneratedAttributeBuilder : AbstractTypeBuilderBase
	{
		CustomAttributeBuilder? _attributeBuilder;

		public GeneratedAttributeBuilder(Type attributeType, object?[]? arguments, string[] names, object[] values)
		{
			if (attributeType == null)
				throw new ArgumentNullException(nameof(attributeType));

			ConstructorInfo? constructor = null;

			if (arguments == null || arguments.Length == 0)
			{
				constructor = attributeType.GetConstructor(Type.EmptyTypes);
				arguments   = Type.EmptyTypes;
			}
			else
			{
				// Some arguments may be null. We can not infer a type from the null reference.
				// So we must iterate all of them and got a suitable one.
				//
				foreach (ConstructorInfo ci in attributeType.GetConstructors())
				{
					if (CheckParameters(ci.GetParameters(), arguments))
					{
						constructor = ci;
						break;
					}
				}
			}

			if (constructor == null)
				throw new TypeBuilderException($"No suitable constructors found for the type '{attributeType.FullName}'.");

			if (names == null || names.Length == 0)
			{
				_attributeBuilder = new CustomAttributeBuilder(constructor, arguments);
			}
			else if (values == null || names.Length != values.Length)
			{
				throw new TypeBuilderException(string.Format("Named argument names count should match named argument values count."));
			}
			else
			{
				var namedProperties = new List<PropertyInfo>();
				var propertyValues  = new List<object>();
				var namedFields     = new List<FieldInfo>();
				var fieldValues     = new List<object>();

				for (var i = 0; i < names.Length; i++)
				{
					var name = names[i];
					var mi   = attributeType.GetMember(name);

					if (mi.Length == 0)
						throw new TypeBuilderException($"The type '{attributeType.FullName}' does not have a public member '{name}'.");

					if (mi[0].MemberType == MemberTypes.Property)
					{
						namedProperties.Add((PropertyInfo)mi[0]);
						propertyValues.Add(values[i]);
					}
					else if (mi[0].MemberType == MemberTypes.Field)
					{
						namedFields.Add((FieldInfo)mi[0]);
						fieldValues.Add(values[i]);
					}
					else
						throw new TypeBuilderException($"The member '{attributeType.FullName}' of the type '{name}' is not a filed nor a property.");
				}

				_attributeBuilder = new CustomAttributeBuilder(constructor, arguments,
					namedProperties.ToArray(), propertyValues.ToArray(), namedFields.ToArray(), fieldValues.ToArray());
			}
		}

		static bool CheckParameters(ParameterInfo[] argumentTypes, object?[]? arguments)
		{
			if (arguments == null)
				return argumentTypes.Length == 0;

			if (argumentTypes.Length != arguments.Length)
				return false;

			for (var i = 0; i < arguments.Length; i++)
			{
				var arg = arguments[i];

				if (arg == null && argumentTypes[i].ParameterType.IsClass)
					continue;

				if (arg != null && argumentTypes[i].ParameterType.IsAssignableFrom(arg.GetType()))
					continue;

				// Bad match
				//
				return false;
			}

			return true;
		}

		public override bool IsApplied(BuildContext context, List<IAbstractTypeBuilder> builders)
		{
			return context.IsAfterStep && context.BuildElement == BuildElement.Type == TargetElement is Type;
		}

		public override void Build(BuildContext context)
		{
			if (context.BuildElement == BuildElement.Type)
			{
				context.TypeBuilder.TypeBuilder.SetCustomAttribute(_attributeBuilder);
			}
			else if (TargetElement is MethodInfo)
			{
				context.MethodBuilder.MethodBuilder.SetCustomAttribute(_attributeBuilder);
			}
			else if (TargetElement is PropertyInfo && context.IsAbstractProperty)
			{
				if (_attributeBuilder != null)
				{
					var field = context.Fields[(PropertyInfo)TargetElement];

					field.SetCustomAttribute(_attributeBuilder);

					// Suppress multiple instances when the property has both getter and setter.
					//
					_attributeBuilder = null;
				}
			}
		}
	}
}

#endif
