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

		public static IEnumerable<TResult> Cast<TResult>(this IEnumerable source)
		{
			var results = source as IEnumerable<TResult>;

			if (results != null)
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
			var results = source as IEnumerable<TResult>;

			if (results != null)
				return results;

			if (source == null) throw new ArgumentNullException(nameof(source));

			IEnumerable<TResult> Iterator()
			{
				foreach (var item in source)
					if (item is TResult)
						yield return (TResult)item;
			}

			return Iterator();
		}
	}
}

#endif
