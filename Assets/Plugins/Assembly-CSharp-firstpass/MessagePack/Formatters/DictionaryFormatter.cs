using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class DictionaryFormatter<TKey, TValue> : DictionaryFormatterBase<TKey, TValue, Dictionary<TKey, TValue>, Dictionary<TKey, TValue>>
	{
		protected override void Add(Dictionary<TKey, TValue> collection, int index, TKey key, TValue value)
		{
			collection.Add(key, value);
		}

		protected override Dictionary<TKey, TValue> Complete(Dictionary<TKey, TValue> intermediateCollection)
		{
			return intermediateCollection;
		}

		protected override Dictionary<TKey, TValue> Create(int count)
		{
			return new Dictionary<TKey, TValue>(count);
		}
	}
}
