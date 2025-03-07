using System.Collections.Generic;

namespace MessagePack.Formatters
{
	public class StackFormatter<T> : CollectionFormatterBase<T, T[], Stack<T>.Enumerator, Stack<T>>
	{
		protected override int? GetCount(Stack<T> sequence)
		{
			return sequence.Count;
		}

		protected override void Add(T[] collection, int index, T value)
		{
			collection[collection.Length - 1 - index] = value;
		}

		protected override T[] Create(int count)
		{
			return new T[count];
		}

		protected override Stack<T>.Enumerator GetSourceEnumerator(Stack<T> source)
		{
			return source.GetEnumerator();
		}

		protected override Stack<T> Complete(T[] intermediateCollection)
		{
			return new Stack<T>(intermediateCollection);
		}
	}
}
