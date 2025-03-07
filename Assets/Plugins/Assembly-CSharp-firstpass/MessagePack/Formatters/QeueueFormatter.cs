using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class QeueueFormatter<T> : CollectionFormatterBase<T, Queue<T>, Queue<T>.Enumerator, Queue<T>>
	{
		protected override int? GetCount(Queue<T> sequence)
		{
			return sequence.Count;
		}

		protected override void Add(Queue<T> collection, int index, T value)
		{
			collection.Enqueue(value);
		}

		protected override Queue<T> Create(int count)
		{
			return new Queue<T>(count);
		}

		protected override Queue<T>.Enumerator GetSourceEnumerator(Queue<T> source)
		{
			return source.GetEnumerator();
		}

		protected override Queue<T> Complete(Queue<T> intermediateCollection)
		{
			return intermediateCollection;
		}
	}
}
