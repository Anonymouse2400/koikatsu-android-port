using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class GenericCollectionFormatter<TElement, TCollection> : CollectionFormatterBase<TElement, TCollection> where TCollection : ICollection<TElement>, new()
	{
		protected override TCollection Create(int count)
		{
			return new TCollection();
		}

		protected override void Add(TCollection collection, int index, TElement value)
		{
			collection.Add(value);
		}
	}
}
