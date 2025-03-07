using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StrayTech
{
	public static class CollectionExtension
	{
		public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
		{
			if (self != null)
			{
				if (!self.ContainsKey(key))
				{
					self.Add(key, value);
				}
				else
				{
					self[key] = value;
				}
			}
		}

		public static int WrapIndex(this ICollection self, int index)
		{
			if (self == null)
			{
				return 0;
			}
			return WrapIndex(index, self.Count);
		}

		public static int WrapIndex(int index, int collectionCount)
		{
			if (collectionCount == 0)
			{
				return 0;
			}
			collectionCount = Mathf.Abs(collectionCount);
			return (collectionCount + index) % collectionCount;
		}

		public static int ClampIndex(this ICollection self, int index)
		{
			if (self == null)
			{
				return 0;
			}
			if (self.Count <= 0)
			{
				return 0;
			}
			return Mathf.Clamp(index, 0, self.Count - 1);
		}

		public static T GetRandomElement<T>(this List<T> self)
		{
			if (self == null)
			{
				return default(T);
			}
			if (self.Count == 0)
			{
				return default(T);
			}
			if (self.Count == 1)
			{
				return self[0];
			}
			return self[UnityEngine.Random.Range(0, self.Count)];
		}

		public static T GetRandomElement<T>(this HashSet<T> self)
		{
			if (self == null)
			{
				return default(T);
			}
			if (self.Count == 0)
			{
				return default(T);
			}
			if (self.Count == 1)
			{
				return self.ElementAt(0);
			}
			int index = UnityEngine.Random.Range(0, self.Count);
			return self.ElementAt(index);
		}

		public static T GetRandomElement<T>(this IEnumerable<T> self)
		{
			if (self == null)
			{
				return default(T);
			}
			int num = self.Count();
			switch (num)
			{
			case 0:
				return default(T);
			case 1:
				return self.ElementAt(0);
			default:
				return self.ElementAt(UnityEngine.Random.Range(0, num));
			}
		}

		public static List<T> GenerateListWithValues<T>(T value, int count)
		{
			count = Mathf.Max(0, count);
			List<T> list = new List<T>(count);
			for (int i = 0; i < count; i++)
			{
				list.Add(value);
			}
			return list;
		}

		public static void Shuffle<T>(this T[] self)
		{
			if (self != null)
			{
				for (int num = self.Length; num > 1; num--)
				{
					int num2 = UnityEngine.Random.Range(0, num);
					T val = self[num2];
					self[num2] = self[num - 1];
					self[num - 1] = val;
				}
			}
		}

		public static IEnumerable<T> ScrapeValidItems<T>(this IEnumerable<T> self) where T : IValidates
		{
			if (self == null)
			{
				yield break;
			}
			foreach (T item in self)
			{
				if (item != null && item.IsValid())
				{
					yield return item;
				}
			}
		}

		public static HashSet<T> ScrapeValidAndUniqueItems<T>(this IEnumerable<T> self) where T : IValidates
		{
			HashSet<T> hashSet = new HashSet<T>();
			if (self == null)
			{
				return hashSet;
			}
			foreach (T item in self)
			{
				if (!item.IsNullOrInvalid() && !hashSet.Contains(item))
				{
					hashSet.Add(item);
				}
			}
			return hashSet;
		}

		public static IEnumerable<T> ScrapeNonNullAndUniqueItems<T>(this IEnumerable<T> self)
		{
			if (self == null)
			{
				yield break;
			}
			HashSet<T> toReturn = new HashSet<T>();
			foreach (T item in self)
			{
				if (item != null && !toReturn.Contains(item))
				{
					toReturn.Add(item);
				}
			}
			foreach (T item2 in toReturn)
			{
				yield return item2;
			}
		}

		public static IEnumerable<T> ScrapeNonNullItems<T>(this IEnumerable<T> self) where T : class
		{
			if (self == null)
			{
				yield break;
			}
			foreach (T item in self)
			{
				if (item != null)
				{
					yield return item;
				}
			}
		}

		public static Dictionary<TKey, TValue> ToDictionary<TSelf, TKey, TValue>(this TSelf self) where TSelf : List<IndexedItem<TKey, TValue>>
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			if (self == null)
			{
				return dictionary;
			}
			foreach (IndexedItem<TKey, TValue> item in self)
			{
				if (!item.IsNullOrInvalid() && !dictionary.ContainsKey(item.ID))
				{
					dictionary.Add(item.ID, item.Value);
				}
			}
			return dictionary;
		}

		public static T ElementAtSafe<T>(this List<T> self, int index)
		{
			if (self == null)
			{
				return default(T);
			}
			if (index < 0 || index >= self.Count)
			{
				return default(T);
			}
			return self[index];
		}

		public static bool IsNullOrEmpty(this ICollection self)
		{
			return self == null || self.Count == 0;
		}

		public static IEnumerable<T> ToEnumerable<T>(this T item)
		{
			if (item != null)
			{
				yield return item;
			}
		}

		public static IEnumerable<TField> Field<TSource, TField>(this IEnumerable<TSource> source, Func<TSource, TField> fieldExtractor)
		{
			if (fieldExtractor == null)
			{
				yield break;
			}
			foreach (TSource item in source)
			{
				if (item != null)
				{
					yield return fieldExtractor(item);
				}
			}
		}

		public static void Remove<T>(this T[] self, T toRemove) where T : class
		{
			if (self == null)
			{
				return;
			}
			for (int i = 0; i < self.Length; i++)
			{
				if (self[i] == toRemove)
				{
					self[i] = (T)null;
				}
			}
		}

		public static T FirstOrDefaultQuick<T>(this List<T> self)
		{
			if (self == null)
			{
				return default(T);
			}
			if (self.Count == 0)
			{
				return default(T);
			}
			return self[0];
		}

		public static T[] Subset<T>(this T[] source, int length)
		{
			if (source == null)
			{
				return null;
			}
			length = Mathf.Clamp(length, 0, source.Length);
			T[] array = new T[length];
			Array.Copy(source, 0, array, 0, length);
			return array;
		}

		public static void PreallocateCapacity(this HashSet<int> toPopulate, int capacity)
		{
			if (toPopulate != null)
			{
				capacity = Mathf.Max(0, capacity);
				for (int i = 0; i < capacity; i++)
				{
					toPopulate.Add(i);
				}
				toPopulate.Clear();
			}
		}

		public static void AddSubsetToSelf<T>(this List<T> self, int startIndex, int count)
		{
			if (self != null && self.Count != 0 && count > 0 && startIndex >= 0 && startIndex < self.Count)
			{
				int num = Mathf.Clamp(startIndex + count, 0, self.Count);
				for (int i = startIndex; i < num; i++)
				{
					self.Add(self[i]);
				}
			}
		}

		public static void AddSubsetFromOther<T>(this List<T> self, List<T> other, int startIndex, int count)
		{
			if (self != null && other != null && other.Count != 0 && count > 0 && startIndex >= 0 && startIndex < other.Count)
			{
				int num = Mathf.Clamp(startIndex + count, 0, other.Count);
				for (int i = startIndex; i < num; i++)
				{
					self.Add(other[i]);
				}
			}
		}
	}
}
