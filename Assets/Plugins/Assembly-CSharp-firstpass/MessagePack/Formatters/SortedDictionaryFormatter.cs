using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class SortedDictionaryFormatter<TKey, TValue> : DictionaryFormatterBase<TKey, TValue, SortedDictionary<TKey, TValue>, SortedDictionary<TKey, TValue>>
	{
		protected override void Add(SortedDictionary<TKey, TValue> collection, int index, TKey key, TValue value)
		{
			collection.Add(key, value);
		}

		protected override SortedDictionary<TKey, TValue> Complete(SortedDictionary<TKey, TValue> intermediateCollection)
		{
			return intermediateCollection;
		}

		protected override SortedDictionary<TKey, TValue> Create(int count)
		{
			return new SortedDictionary<TKey, TValue>();
		}
	}
}
