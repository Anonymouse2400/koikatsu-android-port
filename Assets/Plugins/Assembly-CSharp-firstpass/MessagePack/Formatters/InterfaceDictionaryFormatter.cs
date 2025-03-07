using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class InterfaceDictionaryFormatter<TKey, TValue> : DictionaryFormatterBase<TKey, TValue, Dictionary<TKey, TValue>, IDictionary<TKey, TValue>>
	{
		protected override void Add(Dictionary<TKey, TValue> collection, int index, TKey key, TValue value)
		{
			collection.Add(key, value);
		}

		protected override Dictionary<TKey, TValue> Create(int count)
		{
			return new Dictionary<TKey, TValue>(count);
		}

		protected override IDictionary<TKey, TValue> Complete(Dictionary<TKey, TValue> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
