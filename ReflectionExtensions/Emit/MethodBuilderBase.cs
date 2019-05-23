#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using JetBrains.Annotations;

namespace ReflectionExtensions.Emit
{
	/// <summary>
	/// Base class for wrappers around methods and constructors.
	/// </summary>
	[PublicAPI]
	public abstract class MethodBuilderBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MethodBuilderHelper"/> class
		/// with the specified parameters.
		/// </summary>
		/// <param name="typeBuilder">Associated <see cref="TypeBuilderHelper"/>.</param>
		protected MethodBuilderBase([NotNull] TypeBuilderHelper typeBuilder)
		{
			Type = typeBuilder ?? throw new ArgumentNullException(nameof(typeBuilder));
		}

		/// <summary>
		/// Gets associated <see cref="TypeBuilderHelper"/>.
		/// </summary>
		public TypeBuilderHelper Type { get; }

		/// <summary>
		/// Gets <see cref="EmitHelper"/>.
		/// </summary>
		public abstract EmitHelper Emitter { get; }
	}
}

#endif
