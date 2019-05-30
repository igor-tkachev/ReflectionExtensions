#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
	public class GenerateAttributeAttribute: Builders.AbstractTypeBuilderAttribute
	{
		public GenerateAttributeAttribute(Type attributeType)
		{
			AttributeType = attributeType;
		}

		public GenerateAttributeAttribute(Type attributeType, params object?[] arguments)
		{
			AttributeType = attributeType;
			Arguments     = arguments;
		}

		public Type       AttributeType { get; }
		public object?[]? Arguments     { get; }

#nullable disable

		private string[] _namedArgumentNames;
		public  string[] NamedArgumentNames
		{
			get => _namedArgumentNames;
			set => _namedArgumentNames = value;
		}

		private object[] _namedArgumentValues;
		public  object[] NamedArgumentValues
		{
			get => _namedArgumentValues;
			set => _namedArgumentValues = value;
		}

#nullable restore

		public object? this[string name]
		{
			get
			{
				if (_namedArgumentNames == null)
					return null;

				var idx = Array.IndexOf(_namedArgumentNames, name);

				return idx < 0 ? null : _namedArgumentValues[idx];
			}

			set
			{
				if (_namedArgumentNames == null)
				{
					_namedArgumentNames  = new[] { name  };
					_namedArgumentValues = new[] { value };

					return;
				}

				var idx = Array.IndexOf(_namedArgumentNames, name);

				if (idx < 0)
				{
					idx = _namedArgumentNames.Length;

					Array.Resize(ref _namedArgumentNames,  idx + 1);
					Array.Resize(ref _namedArgumentValues, idx + 1);

					_namedArgumentNames [idx] = name;
					_namedArgumentValues[idx] = value;
				}
				else
				{
					_namedArgumentValues[idx] = value;
				}
			}
		}

#nullable disable

		public T GetValue<T>(string name)
		{
			var value = this[name];
			return value == null? default : (T)value;
		}

#nullable restore

		public T GetValue<T>(string name, T defaultValue)
		{
			return _namedArgumentNames == null || Array.IndexOf(_namedArgumentNames, name) < 0?
				defaultValue : GetValue<T>(name);
		}

		public void SetValue<T>(string name, T value)
		{
			this[name] = value;
		}

		public override Builders.IAbstractTypeBuilder  TypeBuilder =>
			new Builders.GeneratedAttributeBuilder(
				AttributeType, Arguments, _namedArgumentNames, _namedArgumentValues);
	}
}

#endif
