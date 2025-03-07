using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class InterfaceListFormatter<T> : CollectionFormatterBase<T, T[], IList<T>>
	{
		protected override void Add(T[] collection, int index, T value)
		{
			collection[index] = value;
		}

		protected override T[] Create(int count)
		{
			return new T[count];
		}

		protected override IList<T> Complete(T[] intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
