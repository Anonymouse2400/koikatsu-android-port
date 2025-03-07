using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Illusion.Extensions
{
	public static class StringExtensions
	{
		public static string RemoveExtension(this string self)
		{
			return self.Substring(0, self.LastIndexOf("."));
		}

		public static string SetExtension(this string self, string extension)
		{
			return self.Substring(0, self.LastIndexOf(".")) + "." + extension;
		}

		public static string RemoveNewLine(this string self)
		{
			return self.Replace("\r", string.Empty).Replace("\n", string.Empty);
		}

		public static bool Compare(this string self, string str, bool ignoreCase = false)
		{
			return string.Compare(self, str, ignoreCase) == 0;
		}

		public static bool Compare(this string self, string str, StringComparison comparison)
		{
			return string.Compare(self, str, comparison) == 0;
		}

		public static bool CompareParts(this string self, string str, bool ignoreCase = false)
		{
			return ignoreCase ? (self.IndexOf(str, StringComparison.OrdinalIgnoreCase) != -1) : (self.IndexOf(str) != -1);
		}

		public static bool CompareParts(this string self, string str, StringComparison comparison)
		{
			return self.IndexOf(str, comparison) != -1;
		}

		public static string[] LastStringEmptyRemove(this string[] self)
		{
			int num = self.Length;
			while (--num >= 0 && self[num].IsNullOrEmpty())
			{
			}
			string[] array = new string[++num];
			Array.Copy(self, 0, array, 0, num);
			return array;
		}

		public static List<string> LastStringEmptyRemove(this List<string> self)
		{
			int num = self.Count;
			while (--num >= 0 && self[num].IsNullOrEmpty())
			{
			}
			return self.GetRange(0, num + 1);
		}

		public static string[] LastStringEmptySpaceRemove(this string[] self)
		{
			int num = self.Length;
			while (--num >= 0 && self[num].IsNullOrWhiteSpace())
			{
			}
			string[] array = new string[++num];
			Array.Copy(self, 0, array, 0, num);
			return array;
		}

		public static List<string> LastStringEmptySpaceRemove(this List<string> self)
		{
			int num = self.Count;
			while (--num >= 0 && self[num].IsNullOrWhiteSpace())
			{
			}
			return self.GetRange(0, num + 1);
		}

		public static string Coloring(this string self, string color)
		{
			return string.Format("<color={0}>{1}</color>", color, self);
		}

		public static string Size(this string self, int size)
		{
			return string.Format("<size={0}>{1}</size>", size, self);
		}

		public static string Bold(this string self)
		{
			return string.Format("<b>{0}</b>", self);
		}

		public static string Italic(this string self)
		{
			return string.Format("<i>{0}</i>", self);
		}

		public static Color GetColor(this string self)
		{
			Color? colorCheck = self.GetColorCheck();
			return (!colorCheck.HasValue) ? Color.clear : colorCheck.Value;
		}

		public static Color? GetColorCheck(this string self)
		{
			if (self.IsNullOrEmpty())
			{
				return null;
			}
			string[] array = self.Split(',');
			if (array.Length >= 3)
			{
				int num = 0;
				Color value = default(Color);
				float.TryParse(array.SafeGet(num++), out value.r);
				float.TryParse(array.SafeGet(num++), out value.g);
				float.TryParse(array.SafeGet(num++), out value.b);
				if (!float.TryParse(array.SafeGet(num++), out value.a))
				{
					value.a = 1f;
				}
				for (int i = 0; i < num; i++)
				{
					if (value[i] > 1f)
					{
						value[i] = Mathf.InverseLerp(0f, 255f, value[i]);
					}
				}
				return value;
			}
			Color color;
			if (ColorUtility.TryParseHtmlString(self, out color))
			{
				return color;
			}
			return null;
		}

		public static Vector2 GetVector2(this string self)
		{
			string[] array = StringVectorReplace(self);
			return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
		}

		public static Vector3 GetVector3(this string self)
		{
			string[] array = StringVectorReplace(self);
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		private static string[] StringVectorReplace(string str)
		{
			return str.Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", string.Empty)
				.Split(',');
		}

		public static int Check(this string self, bool ignoreCase, params string[] strs)
		{
			return self.Check(ignoreCase, (string s) => s, strs);
		}

		public static int Check(this string self, bool ignoreCase, Func<string, string> func, params string[] strs)
		{
			int num = -1;
			while (++num < strs.Length && !self.Compare(func(strs[num]), ignoreCase))
			{
			}
			return (num < strs.Length) ? num : (-1);
		}

		public static string ToTitleCase(this string self)
		{
			return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(self);
		}

		public static string CopyNameReplace(this string self, int cnt)
		{
			return (cnt <= 0) ? self : string.Format("{0} {1}", self, cnt);
		}
	}
}
