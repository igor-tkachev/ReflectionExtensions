using System;

namespace ReflectionExtensions.TypeBuilder
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ParameterAttribute : Attribute
	{
		protected ParameterAttribute()
		{
			Parameters = new object[0];
		}

		public ParameterAttribute(object parameter1)
		{
			Parameters = new[] { parameter1 };
		}

		public ParameterAttribute(
			object parameter1,
			object parameter2)
		{
			Parameters = new[] { parameter1, parameter2 };
		}

		public ParameterAttribute(
			object parameter1,
			object parameter2,
			object parameter3)
		{
			Parameters = new[] { parameter1, parameter2, parameter3 };
		}

		public ParameterAttribute(
			object parameter1,
			object parameter2,
			object parameter3,
			object parameter4)
		{
			Parameters = new[] { parameter1, parameter2, parameter3, parameter4 };
		}

		public ParameterAttribute(
			object parameter1,
			object parameter2,
			object parameter3,
			object parameter4,
			object parameter5)
		{
			Parameters = new[] { parameter1, parameter2, parameter3, parameter4, parameter5 };
		}

		protected void SetParameters(params object[] parameters)
		{
			Parameters = parameters;
		}

		public object[] Parameters { get; set; }
	}
}
