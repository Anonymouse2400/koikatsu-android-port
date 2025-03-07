using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class SortedListFormatter<TKey, TValue> : DictionaryFormatterBase<TKey, TValue, SortedList<TKey, TValue>, SortedList<TKey, TValue>>
	{
		protected override void Add(SortedList<TKey, TValue> collection, int index, TKey key, TValue value)
		{
			collection.Add(key, value);
		}

		protected override SortedList<TKey, TValue> Complete(SortedList<TKey, TValue> intermediateCollection)
		{
			return intermediateCollection;
		}

		protected override SortedList<TKey, TValue> Create(int count)
		{
			return new SortedList<TKey, TValue>(count);
		}
	}
}
