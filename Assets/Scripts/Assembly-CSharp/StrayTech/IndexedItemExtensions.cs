using System.Collections.Generic;

namespace StrayTech
{
	public static class IndexedItemExtensions
	{
		public static Dictionary<TKey, TValue> ToDictionary<TSource, TKey, TValue>(this List<TSource> toInflate) where TSource : IndexedItem<TKey, TValue>
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			foreach (TSource item in toInflate)
			{
				TSource current = item;
				if (current != null && current.IsValid())
				{
					if (dictionary.ContainsKey(current.ID))
					{
						return new Dictionary<TKey, TValue>();
					}
					dictionary.Add(current.ID, current.Value);
				}
			}
			return dictionary;
		}
	}
}
