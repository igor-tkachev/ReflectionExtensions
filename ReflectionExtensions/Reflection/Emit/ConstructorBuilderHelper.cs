#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;
using System.Reflection.Emit;

using JetBrains.Annotations;

namespace ReflectionExtensions.Reflection.Emit
{
	/// <summary>
	/// A wrapper around the <see cref="ConstructorBuilder"/> class.
	/// </summary>
	[PublicAPI]
	public class ConstructorBuilderHelper : MethodBuilderBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConstructorBuilder"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="typeBuilder">Associated <see cref="TypeBuilderHelper"/>.</param>
		/// <param name="constructorBuilder">A <see cref="ConstructorBuilder"/></param>
		public ConstructorBuilderHelper(TypeBuilderHelper typeBuilder, ConstructorBuilder constructorBuilder)
			: base(typeBuilder)
		{
			ConstructorBuilder = constructorBuilder;
			ConstructorBuilder.SetCustomAttribute(Type.Assembly.BLToolkitAttribute);
		}

		/// <summary>
		/// Gets ConstructorBuilder.
		/// </summary>
		public ConstructorBuilder ConstructorBuilder { get; }

		/// <summary>
		/// Converts the supplied <see cref="ConstructorBuilderHelper"/> to a <see cref="MethodBuilder"/>.
		/// </summary>
		/// <param name="constructorBuilder">The <see cref="ConstructorBuilder"/>.</param>
		/// <returns>A <see cref="ConstructorBuilder"/>.</returns>
		public static implicit operator ConstructorBuilder(ConstructorBuilderHelper constructorBuilder)
		{
			return constructorBuilder.ConstructorBuilder;
		}

		EmitHelper? _emitter;
		/// <summary>
		/// Gets <see cref="EmitHelper"/>.
		/// </summary>
		public override EmitHelper Emitter => _emitter ??= new EmitHelper(this, ConstructorBuilder.GetILGenerator());
	}
}

#endif
