using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class InterfaceCollectionFormatter<T> : CollectionFormatterBase<T, T[], ICollection<T>>
	{
		protected override void Add(T[] collection, int index, T value)
		{
			collection[index] = value;
		}

		protected override T[] Create(int count)
		{
			return new T[count];
		}

		protected override ICollection<T> Complete(T[] intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
