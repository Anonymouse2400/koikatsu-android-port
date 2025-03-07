using System.Collections.Generic;

namespace System.Linq
{
	public static class AnonymousComparer
	{
		private class Comparer<T> : IComparer<T>
		{
			private readonly Func<T, T, int> compare;

			public Comparer(Func<T, T, int> compare)
			{
				this.compare = compare;
			}

			public int Compare(T x, T y)
			{
				return compare(x, y);
			}
		}

		private class EqualityComparer<T> : IEqualityComparer<T>
		{
			private readonly Func<T, T, bool> equals;

			private readonly Func<T, int> getHashCode;

			public EqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
			{
				this.equals = equals;
				this.getHashCode = getHashCode;
			}

			public bool Equals(T x, T y)
			{
				return equals(x, y);
			}

			public int GetHashCode(T obj)
			{
				return getHashCode(obj);
			}
		}

		public static IComparer<T> Create<T>(Func<T, T, int> compare)
		{
			if (compare == null)
			{
				throw new ArgumentNullException("compare");
			}
			return new Comparer<T>(compare);
		}

		public static IEqualityComparer<T> Create<T, TKey>(Func<T, TKey> compareKeySelector)
		{
			if (compareKeySelector == null)
			{
				throw new ArgumentNullException("compareKeySelector");
			}
			return new EqualityComparer<T>(delegate(T x, T y)
			{
				if (object.ReferenceEquals(x, y))
				{
					return true;
				}
				return x != null && y != null && compareKeySelector(x).Equals(compareKeySelector(y));
			}, (T obj) => (obj != null) ? compareKeySelector(obj).GetHashCode() : 0);
		}

		public static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int> getHashCode)
		{
			if (equals == null)
			{
				throw new ArgumentNullException("equals");
			}
			if (getHashCode == null)
			{
				throw new ArgumentNullException("getHashCode");
			}
			return new EqualityComparer<T>(equals, getHashCode);
		}

		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.OrderBy(keySelector, Create(compare));
		}

		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.OrderByDescending(keySelector, Create(compare));
		}

		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.ThenBy(keySelector, Create(compare));
		}

		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> compare)
		{
			return source.ThenByDescending(keySelector, Create(compare));
		}

		public static bool Contains<TSource, TCompareKey>(this IEnumerable<TSource> source, TSource value, Func<TSource, TCompareKey> compareKeySelector)
		{
			return source.Contains(value, Create(compareKeySelector));
		}

		public static IEnumerable<TSource> Distinct<TSource, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TCompareKey> compareKeySelector)
		{
			return source.Distinct(Create(compareKeySelector));
		}

		public static IEnumerable<TSource> Except<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Except(second, Create(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, resultSelector, Create(compareKeySelector));
		}

		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, elementSelector, Create(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, Create(compareKeySelector));
		}

		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, Create(compareKeySelector));
		}

		public static IEnumerable<TSource> Intersect<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Intersect(second, Create(compareKeySelector));
		}

		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult, TCompareKey>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, Create(compareKeySelector));
		}

		public static bool SequenceEqual<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.SequenceEqual(second, Create(compareKeySelector));
		}

		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.ToDictionary(keySelector, elementSelector, Create(compareKeySelector));
		}

		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement, TCompareKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TCompareKey> compareKeySelector)
		{
			return source.ToLookup(keySelector, elementSelector, Create(compareKeySelector));
		}

		public static IEnumerable<TSource> Union<TSource, TCompareKey>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TCompareKey> compareKeySelector)
		{
			return first.Union(second, Create(compareKeySelector));
		}
	}
}
