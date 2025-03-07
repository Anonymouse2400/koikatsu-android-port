using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class InterfaceEnumerableFormatter<T> : CollectionFormatterBase<T, T[], IEnumerable<T>>
	{
		protected override void Add(T[] collection, int index, T value)
		{
			collection[index] = value;
		}

		protected override T[] Create(int count)
		{
			return new T[count];
		}

		protected override IEnumerable<T> Complete(T[] intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
