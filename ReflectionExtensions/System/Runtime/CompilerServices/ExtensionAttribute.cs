using System;

#if NET20 || NET30

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
	sealed class ExtensionAttribute : Attribute
	{
	}
}

#endif