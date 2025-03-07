using System.Collections.Generic;
using System.Linq;

namespace MessagePack.Formatters
{
	public class InterfaceLookupFormatter<TKey, TElement> : CollectionFormatterBase<IGrouping<TKey, TElement>, Dictionary<TKey, IGrouping<TKey, TElement>>, ILookup<TKey, TElement>>
	{
		protected override void Add(Dictionary<TKey, IGrouping<TKey, TElement>> collection, int index, IGrouping<TKey, TElement> value)
		{
			collection.Add(value.Key, value);
		}

		protected override ILookup<TKey, TElement> Complete(Dictionary<TKey, IGrouping<TKey, TElement>> intermediateCollection)
		{
			return new Lookup<TKey, TElement>(intermediateCollection);
		}

		protected override Dictionary<TKey, IGrouping<TKey, TElement>> Create(int count)
		{
			return new Dictionary<TKey, IGrouping<TKey, TElement>>(count);
		}
	}
}
