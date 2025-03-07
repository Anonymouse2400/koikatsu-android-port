using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class HashSetFormatter<T> : CollectionFormatterBase<T, HashSet<T>, HashSet<T>.Enumerator, HashSet<T>>
	{
		protected override int? GetCount(HashSet<T> sequence)
		{
			return sequence.Count;
		}

		protected override void Add(HashSet<T> collection, int index, T value)
		{
			collection.Add(value);
		}

		protected override HashSet<T> Complete(HashSet<T> intermediateCollection)
		{
			return intermediateCollection;
		}

		protected override HashSet<T> Create(int count)
		{
			return new HashSet<T>();
		}

		protected override HashSet<T>.Enumerator GetSourceEnumerator(HashSet<T> source)
		{
			return source.GetEnumerator();
		}
	}
}
