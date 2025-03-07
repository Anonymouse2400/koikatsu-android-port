using System.Collections.ObjectModel;

namespace MessagePack.Formatters
{
	public class ReadOnlyCollectionFormatter<T> : CollectionFormatterBase<T, T[], ReadOnlyCollection<T>>
	{
		protected override void Add(T[] collection, int index, T value)
		{
			collection[index] = value;
		}

		protected override ReadOnlyCollection<T> Complete(T[] intermediateCollection)
		{
			return new ReadOnlyCollection<T>(intermediateCollection);
		}

		protected override T[] Create(int count)
		{
			return new T[count];
		}
	}
}
