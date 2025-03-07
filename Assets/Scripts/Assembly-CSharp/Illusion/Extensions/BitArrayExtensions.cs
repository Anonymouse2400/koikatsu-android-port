using System.Collections;

namespace Illusion.Extensions
{
	public static class BitArrayExtensions
	{
		public static bool Any(this BitArray array)
		{
			foreach (bool item in array)
			{
				if (item)
				{
					return true;
				}
			}
			return false;
		}

		public static bool All(this BitArray array)
		{
			foreach (bool item in array)
			{
				if (!item)
				{
					return false;
				}
			}
			return true;
		}

		public static bool None(this BitArray array)
		{
			return !array.Any();
		}

		public static void Flip(this BitArray array, int index)
		{
			array.Set(index, !array.Get(index));
		}
	}
}
