using System;
using System.Collections.Generic;

namespace Illusion.Extensions
{
	public static class ValueExtensions
	{
		public static int Check<T>(this IList<T> list, Func<T, bool> func)
		{
			return Utils.Value.Check(list.Count, (int index) => func(list[index]));
		}

		public static int Check<T>(this List<T> list, Func<T, bool> func)
		{
			return Utils.Value.Check(list.Count, (int index) => func(list[index]));
		}

		public static int Check<T>(this T[] array, Func<T, bool> func)
		{
			return Utils.Value.Check(array.Length, (int index) => func(array[index]));
		}

		public static int Check<T>(this IList<T> list, T value)
		{
			return Utils.Value.Check(list.Count, (int index) => list[index].Equals(value));
		}

		public static int Check<T>(this List<T> list, T value)
		{
			return Utils.Value.Check(list.Count, (int index) => list[index].Equals(value));
		}

		public static int Check<T>(this T[] array, T value)
		{
			return Utils.Value.Check(array.Length, (int index) => array[index].Equals(value));
		}
	}
}
