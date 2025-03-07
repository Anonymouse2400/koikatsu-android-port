using UnityEngine;

namespace Illusion.Extensions
{
	public static class ColorExtensions
	{
		private static string[] FormatRemoveSplit(string str)
		{
			return FormatRemove(str).Split(',');
		}

		private static string FormatRemove(string str)
		{
			return str.Replace("RGBA", string.Empty).Replace("(", string.Empty).Replace(")", string.Empty)
				.Replace(" ", string.Empty);
		}

		public static string Convert(this Color self, bool isDefault = true)
		{
			int num = 0;
			return string.Format((!isDefault) ? "{0},{1},{2},{3}" : "RGBA({0}, {1}, {2}, {3})", self[num++], self[num++], self[num++], self[num++]);
		}

		public static Color Convert(this Color _, string str)
		{
			string[] array = FormatRemoveSplit(str);
			Color clear = Color.clear;
			for (int i = 0; i < array.Length && i < 4; i++)
			{
				float result;
				if (float.TryParse(array[i], out result))
				{
					clear[i] = result;
				}
			}
			return clear;
		}

		public static float[] ToArray(this Color self)
		{
			int num = 0;
			return new float[4]
			{
				self[num++],
				self[num++],
				self[num++],
				self[num++]
			};
		}

		public static Color RGBToHSV(this Color self)
		{
			float H;
			float S;
			float V;
			Color.RGBToHSV(self, out H, out S, out V);
			return new Color(H, S, V, self.a);
		}

		public static Color HSVToRGB(this Color self)
		{
			int index = 0;
			Color result = Color.HSVToRGB(self[index++], self[index++], self[index++]);
			result[index] = self[index];
			return result;
		}

		public static Color HSVToRGB(this Color self, bool hdr)
		{
			int index = 0;
			Color result = Color.HSVToRGB(self[index++], self[index++], self[index++], hdr);
			result[index] = self[index];
			return result;
		}

		public static Color Get(this Color self, Color set, bool a = true, bool r = true, bool g = true, bool b = true)
		{
			return new Color((!r) ? self.r : set.r, (!g) ? self.g : set.g, (!b) ? self.b : set.b, (!a) ? self.a : set.a);
		}
	}
}
