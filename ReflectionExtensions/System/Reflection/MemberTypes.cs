#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4

using System;

namespace System.Reflection
{
	/// <summary>Marks each type of member that is defined as a derived class of <see cref="T:System.Reflection.MemberInfo" />.</summary>
	[Flags]
	public enum MemberTypes
	{
		/// <summary>Specifies that the member is a constructor</summary>
		Constructor = 1,
		/// <summary>Specifies that the member is an event.</summary>
		Event = 2,
		/// <summary>Specifies that the member is a field.</summary>
		Field = 4,
		/// <summary>Specifies that the member is a method.</summary>
		Method = 8,
		/// <summary>Specifies that the member is a property.</summary>
		Property = 16, // 0x00000010
		/// <summary>Specifies that the member is a type.</summary>
		TypeInfo = 32, // 0x00000020
		/// <summary>Specifies that the member is a custom member type. </summary>
		Custom = 64, // 0x00000040
		/// <summary>Specifies that the member is a nested type.</summary>
		NestedType = 128, // 0x00000080
		/// <summary>Specifies all member types.</summary>
		All = NestedType | TypeInfo | Property | Method | Field | Event | Constructor, // 0x000000BF
	}
}

#endif
