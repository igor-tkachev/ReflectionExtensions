#if NET20 || NET30 || NET35 || NET40 || NET45 || NET451 || NET452 || NET46 || NET461 || NET462 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETCOREAPP1_0 || NETCOREAPP1_1
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace System
{
	interface ITupleInternal
	{
		int    Size { get; }
		int    GetHashCode(IEqualityComparer comparer);
		string ToStringEnd();
	}

	/// <summary>
	/// The ValueTuple types (from arity 0 to 8) comprise the runtime implementation that underlies tuples in C# and struct tuples in F#.
	/// Aside from created via language syntax, they are most easily created via the ValueTuple.Create factory methods.
	/// The System.ValueTuple types differ from the System.Tuple types in that:
	/// - they are structs rather than classes,
	/// - they are mutable rather than readonly, and
	/// - their members (such as Item1, Item2, etc) are fields rather than properties.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	readonly struct ValueTuple : IEquatable<ValueTuple>,
#if !NET20 && !NET30 && !NET35
		IStructuralEquatable, IStructuralComparable,
#endif
		IComparable, IComparable<ValueTuple>, ITupleInternal
	{
		int ITupleInternal.Size => 0;

		/// <summary>
		/// Returns a value that indicates whether the current <see cref="T:System.ValueTuple`0" /> instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><see langword="true" /> if the current instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// The <paramref name="obj" /> parameter is considered to be equal to the current instance under the following conditions:
		/// <list type="bullet">
		///     <item><description>It is a <see cref="T:System.ValueTuple`0" /> value type.</description></item>
		///     <item><description>Its components are of the same types as those of the current instance.</description></item>
		///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
		/// </list>
		/// </remarks>
		public override bool Equals(object? obj)
		{
			return obj is ValueTuple v && Equals(v);
		}

		public bool Equals(ValueTuple other)
		{
			return true;
		}

#if !NET20 && !NET30 && !NET35

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (other == null || !(other is ValueTuple v)) return false;
			return true;
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null) return 1;

			if (!(other is ValueTuple v))
				throw new ArgumentException("Incorrect type of 'ValueTuple'", nameof(other));

			return 0;
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			return 0;
		}

#endif

		int ITupleInternal.GetHashCode(IEqualityComparer comparer)
		{
			return 0;
		}


		int IComparable.CompareTo(object other)
		{
			if (other == null) return 1;
			if (other is ValueTuple v) return CompareTo(v);

			throw new ArgumentException("Incorrect type of 'ValueTuple'", "other");
		}

		public int CompareTo(ValueTuple other)
		{
			return 0;
		}

		/// <summary>
		/// Returns the hash code for the current <see cref="T:System.ValueTuple`0" /> instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			return 0;
		}

		string ITupleInternal.ToStringEnd()
		{
			return "";
		}

		public override string ToString()
		{
			return "()";
		}

		/// <summary>Creates a new struct 0-tuple.</summary>
		/// <returns>A 0-tuple.</returns>
		public static ValueTuple Create()
		{
			return default;
		}

		/// <summary>Creates a new struct 1-tuple, or singleton.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <returns>A 1-tuple (singleton) whose value is (item1).</returns>
		public static ValueTuple<T1> Create<T1>(T1 item1)
		{
			return new ValueTuple<T1>(item1);
		}

		/// <summary>Creates a new struct 2-tuple, or pair.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <returns>A 2-tuple (pair) whose value is (item1, item2).</returns>
		public static ValueTuple<T1,T2> Create<T1,T2>(T1 item1, T2 item2)
		{
			return (item1, item2);
		}

		/// <summary>Creates a new struct 3-tuple, or triple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <returns>A 3-tuple (triple) whose value is (item1, item2, item3).</returns>
		public static ValueTuple<T1,T2,T3> Create<T1,T2,T3>(T1 item1, T2 item2, T3 item3)
		{
			return (item1, item2, item3);
		}

		/// <summary>Creates a new struct 4-tuple, or quadruple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <param name="item4">The value of the fourth component of the tuple.</param>
		/// <returns>A 4-tuple (quadruple) whose value is (item1, item2, item3, item4).</returns>
		public static ValueTuple<T1,T2,T3,T4> Create<T1,T2,T3,T4>(T1 item1, T2 item2, T3 item3, T4 item4)
		{
			return (item1, item2, item3, item4);
		}

		/// <summary>Creates a new struct 5-tuple, or quintuple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
		/// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <param name="item4">The value of the fourth component of the tuple.</param>
		/// <param name="item5">The value of the fifth component of the tuple.</param>
		/// <returns>A 5-tuple (quintuple) whose value is (item1, item2, item3, item4, item5).</returns>
		public static ValueTuple<T1,T2,T3,T4,T5> Create<T1,T2,T3,T4,T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
		{
			return (item1, item2, item3, item4, item5);
		}

		/// <summary>Creates a new struct 6-tuple, or sextuple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
		/// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
		/// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <param name="item4">The value of the fourth component of the tuple.</param>
		/// <param name="item5">The value of the fifth component of the tuple.</param>
		/// <param name="item6">The value of the sixth component of the tuple.</param>
		/// <returns>A 6-tuple (sextuple) whose value is (item1, item2, item3, item4, item5, item6).</returns>
		public static ValueTuple<T1,T2,T3,T4,T5,T6> Create<T1,T2,T3,T4,T5,T6>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
		{
			return (item1, item2, item3, item4, item5, item6);
		}

		/// <summary>Creates a new struct 7-tuple, or septuple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
		/// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
		/// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
		/// <typeparam name="T7">The type of the seventh component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <param name="item4">The value of the fourth component of the tuple.</param>
		/// <param name="item5">The value of the fifth component of the tuple.</param>
		/// <param name="item6">The value of the sixth component of the tuple.</param>
		/// <param name="item7">The value of the seventh component of the tuple.</param>
		/// <returns>A 7-tuple (septuple) whose value is (item1, item2, item3, item4, item5, item6, item7).</returns>
		public static ValueTuple<T1,T2,T3,T4,T5,T6,T7> Create<T1,T2,T3,T4,T5,T6,T7>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
		{
			return (item1, item2, item3, item4, item5, item6, item7);
		}

		/// <summary>Creates a new struct 8-tuple, or octuple.</summary>
		/// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
		/// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
		/// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
		/// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
		/// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
		/// <typeparam name="T6">The type of the sixth component of the tuple.</typeparam>
		/// <typeparam name="T7">The type of the seventh component of the tuple.</typeparam>
		/// <typeparam name="T8">The type of the eighth component of the tuple.</typeparam>
		/// <param name="item1">The value of the first component of the tuple.</param>
		/// <param name="item2">The value of the second component of the tuple.</param>
		/// <param name="item3">The value of the third component of the tuple.</param>
		/// <param name="item4">The value of the fourth component of the tuple.</param>
		/// <param name="item5">The value of the fifth component of the tuple.</param>
		/// <param name="item6">The value of the sixth component of the tuple.</param>
		/// <param name="item7">The value of the seventh component of the tuple.</param>
		/// <param name="item8">The value of the eighth component of the tuple.</param>
		/// <returns>An 8-tuple (octuple) whose value is (item1, item2, item3, item4, item5, item6, item7, item8).</returns>
		public static ValueTuple<T1,T2,T3,T4,T5,T6,T7,ValueTuple<T8>> Create<T1,T2,T3,T4,T5,T6,T7,T8>(
			T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, T8 item8)
		{
			return new ValueTuple<T1,T2,T3,T4,T5,T6,T7,ValueTuple<T8>>(item1, item2, item3, item4, item5, item6, item7, ValueTuple.Create<T8>(item8));
		}

		static int Combine(int h1, int h2)
		{
			var num = (uint)((h1 << 5) | (int)((uint)h1 >> 27));
			return ((int)num + h1) ^ h2;
		}

		static readonly int RandomSeed = Guid.NewGuid().GetHashCode();

		internal static int CombineHashCodes(int h1, int h2)
		{
			return Combine(Combine(RandomSeed, h1), h2);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3)
		{
			return Combine(CombineHashCodes(h1, h2), h3);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
		{
			return Combine(CombineHashCodes(h1, h2, h3), h4);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
		{
			return Combine(CombineHashCodes(h1, h2, h3, h4), h5);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
		{
			return Combine(CombineHashCodes(h1, h2, h3, h4, h5), h6);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
		{
			return Combine(CombineHashCodes(h1, h2, h3, h4, h5, h6), h7);
		}

		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
		{
			return Combine(CombineHashCodes(h1, h2, h3, h4, h5, h6, h7), h8);
		}
	}

	/// <summary>
	/// Represents an 8-tuple, or octuple, as a value type.
	/// </summary>
	/// <typeparam name="T1">The type of the tuple's first component.</typeparam>
	/// <typeparam name="T2">The type of the tuple's second component.</typeparam>
	/// <typeparam name="T3">The type of the tuple's third component.</typeparam>
	/// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
	/// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
	/// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
	/// <typeparam name="T7">The type of the tuple's seventh component.</typeparam>
	/// <typeparam name="TRest">The type of the tuple's eighth component.</typeparam>
	[StructLayout(LayoutKind.Auto)]
	struct ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> : IEquatable<ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest>>,
#if !NET20 && !NET30 && !NET35
		IStructuralEquatable, IStructuralComparable,
#endif
		IComparable, IComparable<ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest>>, ITupleInternal
		where TRest : struct
	{
		static readonly EqualityComparer<T1>    s_t1Comparer    = EqualityComparer<T1>.Default;
		static readonly EqualityComparer<T2>    s_t2Comparer    = EqualityComparer<T2>.Default;
		static readonly EqualityComparer<T3>    s_t3Comparer    = EqualityComparer<T3>.Default;
		static readonly EqualityComparer<T4>    s_t4Comparer    = EqualityComparer<T4>.Default;
		static readonly EqualityComparer<T5>    s_t5Comparer    = EqualityComparer<T5>.Default;
		static readonly EqualityComparer<T6>    s_t6Comparer    = EqualityComparer<T6>.Default;
		static readonly EqualityComparer<T7>    s_t7Comparer    = EqualityComparer<T7>.Default;
		static readonly EqualityComparer<TRest> s_tRestComparer = EqualityComparer<TRest>.Default;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's first component.
		/// </summary>
		public T1 Item1;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's second component.
		/// </summary>
		public readonly T2 Item2;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's third component.
		/// </summary>
		public readonly T3 Item3;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's fourth component.
		/// </summary>
		public readonly T4 Item4;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's fifth component.
		/// </summary>
		public readonly T5 Item5;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's sixth component.
		/// </summary>
		public readonly T6 Item6;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's seventh component.
		/// </summary>
		public readonly T7 Item7;

		/// <summary>
		/// The current <see cref="T:System.ValueTuple`8" /> instance's eighth component.
		/// </summary>
		public readonly TRest Rest;

		int ITupleInternal.Size => Rest is ITupleInternal tuple ? 7 + tuple.Size : 8;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.ValueTuple`8" /> value type.
		/// </summary>
		/// <param name="item1">The value of the tuple's first component.</param>
		/// <param name="item2">The value of the tuple's second component.</param>
		/// <param name="item3">The value of the tuple's third component.</param>
		/// <param name="item4">The value of the tuple's fourth component.</param>
		/// <param name="item5">The value of the tuple's fifth component.</param>
		/// <param name="item6">The value of the tuple's sixth component.</param>
		/// <param name="item7">The value of the tuple's seventh component.</param>
		/// <param name="rest">The value of the tuple's eight component.</param>
		public ValueTuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
		{
			if (!(rest is System.ITupleInternal))
				throw new ArgumentException("ValueTuple last argument is not a ValueTuple");

			Item1 = item1;
			Item2 = item2;
			Item3 = item3;
			Item4 = item4;
			Item5 = item5;
			Item6 = item6;
			Item7 = item7;
			Rest  = rest;
		}

		/// <summary>
		/// Returns a value that indicates whether the current <see cref="T:System.ValueTuple`8" /> instance is equal to a specified object.
		/// </summary>
		/// <param name="obj">The object to compare with this instance.</param>
		/// <returns><see langword="true" /> if the current instance is equal to the specified object; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// The <paramref name="obj" /> parameter is considered to be equal to the current instance under the following conditions:
		/// <list type="bullet">
		///     <item><description>It is a <see cref="T:System.ValueTuple`8" /> value type.</description></item>
		///     <item><description>Its components are of the same types as those of the current instance.</description></item>
		///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
		/// </list>
		/// </remarks>
		public override bool Equals(object obj)
		{
			return obj is ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> v && Equals(v);
		}

		/// <summary>
		/// Returns a value that indicates whether the current <see cref="T:System.ValueTuple`8" />
		/// instance is equal to a specified <see cref="T:System.ValueTuple`8" />.
		/// </summary>
		/// <param name="other">The tuple to compare with this instance.</param>
		/// <returns><see langword="true" /> if the current instance is equal to the specified tuple; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// The <paramref name="other" /> parameter is considered to be equal to the current instance if each of its fields
		/// are equal to that of the current instance, using the default comparer for that field's type.
		/// </remarks>
		public bool Equals(ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> other)
		{
			return
				s_t1Comparer.Equals(Item1, other.Item1) &&
				s_t2Comparer.Equals(Item2, other.Item2) &&
				s_t3Comparer.Equals(Item3, other.Item3) &&
				s_t4Comparer.Equals(Item4, other.Item4) &&
				s_t5Comparer.Equals(Item5, other.Item5) &&
				s_t6Comparer.Equals(Item6, other.Item6) &&
				s_t7Comparer.Equals(Item7, other.Item7) &&
				s_tRestComparer.Equals(Rest, other.Rest);
		}

#if !NET20 && !NET30 && !NET35

		bool IStructuralEquatable.Equals(object other, IEqualityComparer comparer)
		{
			if (other == null || !(other is ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> v)) return false;

			return
				comparer.Equals(Item1, v.Item1) &&
				comparer.Equals(Item2, v.Item2) &&
				comparer.Equals(Item3, v.Item3) &&
				comparer.Equals(Item4, v.Item4) &&
				comparer.Equals(Item5, v.Item5) &&
				comparer.Equals(Item6, v.Item6) &&
				comparer.Equals(Item7, v.Item7) &&
				comparer.Equals(Rest,  v.Rest);
		}

		int IStructuralComparable.CompareTo(object other, IComparer comparer)
		{
			if (other == null) return 1;
			if (!(other is ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> v))
				throw new ArgumentException("ValueTuple has incorrect type", nameof(other));

			int num;

			num = comparer.Compare(Item1, v.Item1); if (num != 0) return num;
			num = comparer.Compare(Item2, v.Item2); if (num != 0) return num;
			num = comparer.Compare(Item3, v.Item3); if (num != 0) return num;
			num = comparer.Compare(Item4, v.Item4); if (num != 0) return num;
			num = comparer.Compare(Item5, v.Item5); if (num != 0) return num;
			num = comparer.Compare(Item6, v.Item6); if (num != 0) return num;
			num = comparer.Compare(Item7, v.Item7); if (num != 0) return num;
			num = comparer.Compare(Item1, v.Item1); if (num != 0) return num;

			return comparer.Compare(Rest, v.Rest);
		}

		int IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
		{
			if (!(Rest is ITupleInternal tuple))
				return ValueTuple.CombineHashCodes(
					comparer.GetHashCode(Item1),
					comparer.GetHashCode(Item2),
					comparer.GetHashCode(Item3),
					comparer.GetHashCode(Item4),
					comparer.GetHashCode(Item5),
					comparer.GetHashCode(Item6),
					comparer.GetHashCode(Item7));

			var size = tuple.Size;

			if (size >= 8)
				return tuple.GetHashCode(comparer);

			switch (8 - size)
			{
				case 1 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 2 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 3 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 4 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 5 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 6 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				case 7 :
				case 8 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tuple.GetHashCode(comparer));
				default: return -1;
			}
		}

#endif

		int IComparable.CompareTo(object other)
		{
			if (other == null) return 1;
			if (other is ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> tuple)
				return CompareTo(tuple);

			throw new ArgumentException("ValueTuple has incorrect type", nameof(other));
		}

		/// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
		/// <param name="other">An instance to compare.</param>
		/// <returns>
		/// A signed number indicating the relative values of this instance and <paramref name="other" />.
		/// Returns less than zero if this instance is less than <paramref name="other" />, zero if this
		/// instance is equal to <paramref name="other" />, and greater than zero if this instance is greater
		/// than <paramref name="other" />.
		/// </returns>
		public int CompareTo(ValueTuple<T1,T2,T3,T4,T5,T6,T7,TRest> other)
		{
			int num;

			num = Comparer<T1>.Default.Compare(Item1, other.Item1); if (num != 0) return num;
			num = Comparer<T2>.Default.Compare(Item2, other.Item2); if (num != 0) return num;
			num = Comparer<T3>.Default.Compare(Item3, other.Item3); if (num != 0) return num;
			num = Comparer<T4>.Default.Compare(Item4, other.Item4); if (num != 0) return num;
			num = Comparer<T5>.Default.Compare(Item5, other.Item5); if (num != 0) return num;
			num = Comparer<T6>.Default.Compare(Item6, other.Item6); if (num != 0) return num;
			num = Comparer<T7>.Default.Compare(Item7, other.Item7); if (num != 0) return num;

			return Comparer<TRest>.Default.Compare(Rest, other.Rest);
		}

		/// <summary>
		/// Returns the hash code for the current <see cref="T:System.ValueTuple`8" /> instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code.</returns>
		public override int GetHashCode()
		{
			if (!(Rest is ITupleInternal tupleInternal))
			{
				return ValueTuple.CombineHashCodes(
					s_t1Comparer.GetHashCode(Item1),
					s_t2Comparer.GetHashCode(Item2),
					s_t3Comparer.GetHashCode(Item3),
					s_t4Comparer.GetHashCode(Item4),
					s_t5Comparer.GetHashCode(Item5),
					s_t6Comparer.GetHashCode(Item6),
					s_t7Comparer.GetHashCode(Item7));
			}

			var size = tupleInternal.Size;

			if (size >= 8)
				return tupleInternal.GetHashCode();

			switch (8 - size)
			{
				case 1 : return ValueTuple.CombineHashCodes(s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 2 : return ValueTuple.CombineHashCodes(s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 3 : return ValueTuple.CombineHashCodes(s_t5Comparer.GetHashCode(Item5), s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 4 : return ValueTuple.CombineHashCodes(s_t4Comparer.GetHashCode(Item4), s_t5Comparer.GetHashCode(Item5), s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 5 : return ValueTuple.CombineHashCodes(s_t3Comparer.GetHashCode(Item3), s_t4Comparer.GetHashCode(Item4), s_t5Comparer.GetHashCode(Item5), s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 6 : return ValueTuple.CombineHashCodes(s_t2Comparer.GetHashCode(Item2), s_t3Comparer.GetHashCode(Item3), s_t4Comparer.GetHashCode(Item4), s_t5Comparer.GetHashCode(Item5), s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				case 7 :
				case 8 : return ValueTuple.CombineHashCodes(s_t1Comparer.GetHashCode(Item1), s_t2Comparer.GetHashCode(Item2), s_t3Comparer.GetHashCode(Item3), s_t4Comparer.GetHashCode(Item4), s_t5Comparer.GetHashCode(Item5), s_t6Comparer.GetHashCode(Item6), s_t7Comparer.GetHashCode(Item7), tupleInternal.GetHashCode());
				default: return -1;
			}
		}

		int GetHashCodeCore(IEqualityComparer comparer)
		{
			if (!(Rest is ITupleInternal tupleInternal))
			{
				return ValueTuple.CombineHashCodes(
					comparer.GetHashCode(Item1),
					comparer.GetHashCode(Item2),
					comparer.GetHashCode(Item3),
					comparer.GetHashCode(Item4),
					comparer.GetHashCode(Item5),
					comparer.GetHashCode(Item6),
					comparer.GetHashCode(Item7));
			}

			var size = tupleInternal.Size;

			if (size >= 8)
				return tupleInternal.GetHashCode(comparer);

			switch (8 - size)
			{
				case 1 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 2 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 3 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 4 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 5 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 6 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				case 7 :
				case 8 : return ValueTuple.CombineHashCodes(comparer.GetHashCode(Item1), comparer.GetHashCode(Item2), comparer.GetHashCode(Item3), comparer.GetHashCode(Item4), comparer.GetHashCode(Item5), comparer.GetHashCode(Item6), comparer.GetHashCode(Item7), tupleInternal.GetHashCode(comparer));
				default: return -1;
			}
		}

		int ITupleInternal.GetHashCode(IEqualityComparer comparer)
		{
			return GetHashCodeCore(comparer);
		}

#nullable disable

		/// <summary>
		/// Returns a string that represents the value of this <see cref="T:System.ValueTuple`8" /> instance.
		/// </summary>
		/// <returns>The string representation of this <see cref="T:System.ValueTuple`8" /> instance.</returns>
		/// <remarks>
		/// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5, Item6, Item7, Rest)</c>.
		/// If any field value is <see langword="null" />, it is represented as <see cref="F:System.String.Empty" />.
		/// </remarks>
		public override string ToString()
		{
			return "(" + ((ITupleInternal)this).ToStringEnd() + ")";
		}

		string ITupleInternal.ToStringEnd()
		{
			var str =
				Item1 + ", " +
				Item2 + ", " +
				Item3 + ", " +
				Item4 + ", " +
				Item5 + ", " +
				Item6 + ", " +
				Item7 + ", ";

			return Rest is ITupleInternal t ? str + t.ToStringEnd() : str + Rest;
		}

#nullable restore

	}
}

#endif
