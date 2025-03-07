using System.Collections.Generic;

namespace Illusion.Extensions
{
	public static class ListExtensions
	{
		public static T Peek<T>(this IList<T> self)
		{
			return self[0];
		}

		public static T Pop<T>(this IList<T> self)
		{
			T result = self[0];
			self.RemoveAt(0);
			return result;
		}

		public static void Push<T>(this IList<T> self, T item)
		{
			self.Insert(0, item);
		}

		public static T Dequeue<T>(this IList<T> self)
		{
			T result = self[0];
			self.RemoveAt(0);
			return result;
		}

		public static void Enqueue<T>(this IList<T> self, T item)
		{
			self.Add(item);
		}
	}
}
