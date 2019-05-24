#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6 && !NETSTANDARD2_0

using System;

using JetBrains.Annotations;

namespace ReflectionExtensions.TypeBuilder.Builders
{
	using Reflection.Emit;

	[PublicAPI]
	public interface ITypeBuilder
	{
		string AssemblyNameSuffix { get; }
		Type?  Build          (AssemblyBuilderHelper assemblyBuilder);
		string GetTypeName    ();
		Type   GetBuildingType();
	}
}

#endif
