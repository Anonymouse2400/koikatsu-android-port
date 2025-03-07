using System;
using System.Collections.Generic;
using System.Linq;

namespace Illusion.Extensions
{
	public static class IEnumerableExtensions
	{
		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> self)
		{
			return self.OrderBy((T _) => Guid.NewGuid());
		}

		public static IEnumerable<T> SymmetricExcept<T>(this IEnumerable<T> self, IEnumerable<T> target)
		{
			return self.Except(target).Concat(target.Except(self));
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, T second)
		{
			return first.Concat(new T[1] { second });
		}
	}
}
