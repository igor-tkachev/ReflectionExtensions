using System;
using System.Collections;

namespace ReflectionExtensions.Common
{
	public class CompoundValue : IComparable, IEquatable<CompoundValue>
	{
		public CompoundValue(params object?[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			// Note that the compound hash is precalculated.
			// This means that CompoundValue can be used only with immutable values.
			// Otherwise the behaviour is undefined.
			//
			_hash   = CalcHashCode(values);
			_values = values;
		}

		readonly object?[] _values;
		readonly int       _hash;

		public int Count => _values.Length;

		public object? this[int index] => _values[index];

		static int CalcHashCode(object?[] values)
		{
			if (values.Length == 0)
				return 0;

			var o    = values[0];
			var hash = o?.GetHashCode() ?? 0;

			for (var i = 1; i < values.Length; i++)
			{
				o = values[i];
				hash = ((hash << 5) + hash) ^ (o?.GetHashCode() ?? 0);
			}

			return hash;
		}

		#region IComparable Members

		public int CompareTo(object obj)
		{
			var objValues = ((CompoundValue)obj)._values;

			if (_values.Length != objValues.Length)
				return _values.Length - objValues.Length;

			for (var i = 0; i < _values.Length; i++)
			{
				var n = Compare(_values[i], objValues[i]);

				if (n != 0)
					return n;
			}

			return 0;
		}

#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD2_0

		public int Compare(object? a, object? b)
		{
			if (a == b)    return 0;
			if (a == null) return -1;
			if (b == null) return 1;

			if (a is string s1 && b is string s2)
				return string.CompareOrdinal(s1, s2);

			if (a is IComparable comparable1) return comparable1.CompareTo(b);
			if (b is IComparable comparable2) return -comparable2.CompareTo(a);

			throw new ArgumentException($"Object of type {a.GetType()} does not implement IComparable.");
		}

#else

		static int Compare(object? a, object? b)
		{
			return Comparer.Default.Compare(a, b);
		}

#endif
		#endregion

		#region Object Overrides

		public override int GetHashCode()
		{
			return _hash;
		}

		public override bool Equals(object obj)
		{
			return obj is CompoundValue value && Equals(value);
		}

		#endregion

		#region IEquatable<CompoundValue> Members

		public bool Equals(CompoundValue other)
		{
			if (_hash != other._hash)
				return false;

			var values = other._values;

			if (_values.Length != values.Length)
				return false;

			for (var i = 0; i < _values.Length; i++)
			{
				var x = _values[i];
				var y =  values[i];

				if (x == null && y == null)
					continue;

				if (x == null || y == null)
					return false;

				if (x.Equals(y) == false)
					return false;
			}

			return true;
		}

		#endregion
	}
}
