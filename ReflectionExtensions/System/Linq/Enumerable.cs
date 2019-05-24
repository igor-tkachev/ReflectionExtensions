using System;

#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

namespace System.Linq
{
	static class Enumerable
	{
		public static TElement[] ToArray<TElement>([NotNull] this IEnumerable<TElement> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (source is ICollection<TElement> collection)
			{
				if (collection.Count == 0)
					return new TElement[0];

				var arr = new TElement[collection.Count];

				collection.CopyTo(arr, 0);

				return arr;
			}

			var list = new List<TElement>();

			foreach (var item in source)
				list.Add(item);

			return list.ToArray();
		}

		public static bool Any<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource,bool> predicate)
		{
			if (source    == null) throw new ArgumentNullException(nameof(source));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			foreach (var item in source)
				if (predicate(item))
					return true;

			return false;
		}

		public static IEnumerable<TSource> Where<TSource>([NotNull] this IEnumerable<TSource> source, [NotNull] Func<TSource,bool> predicate)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			foreach (var item in source)
				if (predicate(item))
					yield return item;
		}

		[CanBeNull]
		public static TSource FirstOrDefault<TSource>([NotNull] this IEnumerable<TSource> source)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			if (source is IList<TSource> sourceList)
			{
				if (sourceList.Count > 0)
					return sourceList[0];
			}
			else
			{
				using var enumerator = source.GetEnumerator();
				if (enumerator.MoveNext())
					return enumerator.Current;
			}

#nullable disable
			return default;
#nullable restore
		}

		[CanBeNull]
		public static TSource FirstOrDefault<TSource>([NotNull] this IEnumerable<TSource> source, Func<TSource,bool> predicate)
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			foreach (var item in source)
				if (predicate(item))
					return item;

#nullable disable
			return default;
#nullable restore
		}

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			if (source is IEnumerable<TResult> results)
				return results;

			if (source == null) throw new ArgumentNullException(nameof(source));

			IEnumerable<TResult> Iterator()
			{
				foreach (var item in source)
					yield return (TResult)item;
			}

			return Iterator();
		}

		public static IEnumerable<TResult> OfType<TResult>(this IEnumerable source)
		{
			if (source is IEnumerable<TResult> results)
				return results;

			if (source == null) throw new ArgumentNullException(nameof(source));

			IEnumerable<TResult> Iterator()
			{
				foreach (var item in source)
					if (item is TResult result)
						yield return result;
			}

			return Iterator();
		}

		/// <summary>Projects each element of a sequence into a new form.</summary>
		/// <param name="source">A sequence of values to invoke a transform function on.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TResult">The type of the value returned by <paramref name="selector" />.</typeparam>
		/// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are the result of invoking the transform function on each element of <paramref name="source" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="source" /> or <paramref name="selector" /> is <see langword="null" />.</exception>
		public static IEnumerable<TResult> Select<TSource,TResult>(this IEnumerable<TSource> source, Func<TSource,TResult> selector)
		{
			foreach (var item in source)
				yield return selector(item);
		}


		/// <summary>Creates a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to specified key selector and element selector functions.</summary>
		/// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
		/// <returns>A <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains values of type <paramref name="TElement" /> selected from the input sequence.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		/// <paramref name="source" /> or <paramref name="keySelector" /> or <paramref name="elementSelector" /> is <see langword="null" />.-or-
		/// <paramref name="keySelector" /> produces a key that is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// <paramref name="keySelector" /> produces duplicate keys for two elements.</exception>
		public static Dictionary<TKey,TElement> ToDictionary<TSource,TKey,TElement>(
			this IEnumerable<TSource> source, Func<TSource,TKey> keySelector, Func<TSource,TElement> elementSelector)
		{
			var dic = new Dictionary<TKey,TElement>();

			foreach (var item in source)
				dic.Add(keySelector(item), elementSelector(item));

			return dic;
		}
	}
}

#endif
