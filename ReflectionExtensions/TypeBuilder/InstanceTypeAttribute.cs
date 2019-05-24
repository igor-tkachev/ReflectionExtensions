#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

namespace ReflectionExtensions.TypeBuilder
{
	using Builders;
	using Reflection;

	///<summary>
	/// Specifies a value holder type.
	///</summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class InstanceTypeAttribute : AbstractTypeBuilderAttribute
	{
		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		public InstanceTypeAttribute(Type instanceType)
		{
			InstanceType = instanceType;
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType, object parameter1)
		{
			InstanceType = instanceType;
			SetParameters(parameter1);
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<param name="parameter2">An additional parameter.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType,
			object parameter1,
			object parameter2)
		{
			InstanceType = instanceType;
			SetParameters(parameter1, parameter2);
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<param name="parameter2">An additional parameter.</param>
		///<param name="parameter3">An additional parameter.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType,
			object parameter1,
			object parameter2,
			object parameter3)
		{
			InstanceType = instanceType;
			SetParameters(parameter1, parameter2, parameter3);
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<param name="parameter2">An additional parameter.</param>
		///<param name="parameter3">An additional parameter.</param>
		///<param name="parameter4">An additional parameter.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType,
			object parameter1,
			object parameter2,
			object parameter3,
			object parameter4)
		{
			InstanceType = instanceType;
			SetParameters(parameter1, parameter2, parameter3, parameter4);
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<param name="parameter2">An additional parameter.</param>
		///<param name="parameter3">An additional parameter.</param>
		///<param name="parameter4">An additional parameter.</param>
		///<param name="parameter5">An additional parameter.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType,
			object parameter1,
			object parameter2,
			object parameter3,
			object parameter4,
			object parameter5)
		{
			InstanceType = instanceType;
			SetParameters(parameter1, parameter2, parameter3, parameter4, parameter5);
		}

		///<summary>
		/// Initializes a new instance of the InstanceTypeAttribute class.
		///</summary>
		///<param name="instanceType">The <see cref="System.Type"/> of an instance.</param>
		///<param name="parameter1">An additional parameter.</param>
		///<param name="parameters">More additional parameters.</param>
		///<seealso cref="Parameters"/>
		public InstanceTypeAttribute(Type instanceType, object parameter1, params object[] parameters)
		{
			InstanceType = instanceType;

			// Note: we can not use something like
			// public InstanceTypeAttribute(Type instanceType, params object[] parameters)
			// because [InstanceType(typeof(Foo), new object[] {1,2,3})] will be treated as
			// [InstanceType(typeof(Foo), 1, 2, 3)] so it will be not possible to specify
			// an instance type with array as the type of the one and only parameter.
			// An extra parameter of type object made it successful.

			var len = parameters.Length;

			Array.Resize(ref parameters, len + 1);
			Array.ConstrainedCopy(parameters, 0, parameters, 1, len);
			parameters[0] = parameter1;

			SetParameters(parameters);
		}

		protected void SetParameters(params object[] parameters)
		{
			Parameters = parameters;
		}

		///<summary>
		/// Any additional parameters passed to a value holder constructor
		/// with <see cref="InitContext"/> parameter.
		///</summary>
		public object[] Parameters { get; set; }

		protected Type InstanceType { get; }

		///<summary>
		/// False (default value) for holders for scalar types,
		/// true for holders for complex objects.
		///</summary>
		public bool IsObjectHolder { get; set; }

		IAbstractTypeBuilder? _typeBuilder;
		///<summary>
		/// An <see cref="IAbstractTypeBuilder"/> required for this attribute
		/// to build an abstract type inheritor.
		///</summary>
		public override IAbstractTypeBuilder TypeBuilder
			=> _typeBuilder ??= new InstanceTypeBuilder(InstanceType, IsObjectHolder);
	}
}

#endif
