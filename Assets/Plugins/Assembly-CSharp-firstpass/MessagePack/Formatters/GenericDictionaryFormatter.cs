using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class GenericDictionaryFormatter<TKey, TValue, TDictionary> : DictionaryFormatterBase<TKey, TValue, TDictionary, TDictionary> where TDictionary : IDictionary<TKey, TValue>, new()
	{
		protected override void Add(TDictionary collection, int index, TKey key, TValue value)
		{
			collection.Add(key, value);
		}

		protected override TDictionary Complete(TDictionary intermediateCollection)
		{
			return intermediateCollection;
		}

		protected override TDictionary Create(int count)
		{
			return new TDictionary();
		}
	}
}
