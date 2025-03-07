using System.Collections.Generic;
using System.Linq;

internal static class IntersectExtensions
{
	public static bool IntersectAny<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer = null)
	{
		return first.Intersect(second, comparer).Any();
	}
}
