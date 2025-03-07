using UnityEngine;

namespace Illusion.Extensions
{
	public static class VectorExtensions
	{
		private static string[] FormatRemoveSplit(string str)
		{
			return FormatRemove(str).Split(',');
		}

		private static string FormatRemove(string str)
		{
			return str.Replace("(", string.Empty).Replace(")", string.Empty).Replace(" ", string.Empty);
		}

		public static string Convert(this Vector2 self, bool isDefault = true)
		{
			int num = 0;
			return string.Format((!isDefault) ? "{0},{1}" : "({0}, {1})", self[num++], self[num++]);
		}

		public static string Convert(this Vector3 self, bool isDefault = true)
		{
			int num = 0;
			return string.Format((!isDefault) ? "{0},{1},{2}" : "({0}, {1}, {2})", self[num++], self[num++], self[num++]);
		}

		public static string Convert(this Vector4 self, bool isDefault = true)
		{
			int num = 0;
			return string.Format((!isDefault) ? "{0},{1},{2},{3}" : "({0}, {1}, {2}, {3})", self[num++], self[num++], self[num++], self[num++]);
		}

		public static Vector2 Convert(this Vector2 _, string str)
		{
			string[] array = FormatRemoveSplit(str);
			Vector2 zero = Vector2.zero;
			for (int i = 0; i < array.Length && i < 2; i++)
			{
				float result;
				if (float.TryParse(array[i], out result))
				{
					zero[i] = result;
				}
			}
			return zero;
		}

		public static Vector3 Convert(this Vector3 _, string str)
		{
			string[] array = FormatRemoveSplit(str);
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < array.Length && i < 3; i++)
			{
				float result;
				if (float.TryParse(array[i], out result))
				{
					zero[i] = result;
				}
			}
			return zero;
		}

		public static Vector4 Convert(this Vector4 _, string str)
		{
			string[] array = FormatRemoveSplit(str);
			Vector4 zero = Vector4.zero;
			for (int i = 0; i < array.Length && i < 4; i++)
			{
				float result;
				if (float.TryParse(array[i], out result))
				{
					zero[i] = result;
				}
			}
			return zero;
		}

		public static Vector2 Convert(this Vector2 self, float[] fArray)
		{
			int num = 0;
			return new Vector2(fArray[num++], fArray[num++]);
		}

		public static Vector3 Convert(this Vector3 self, float[] fArray)
		{
			int num = 0;
			return new Vector3(fArray[num++], fArray[num++], fArray[num++]);
		}

		public static Vector4 Convert(this Vector4 self, float[] fArray)
		{
			int num = 0;
			return new Vector4(fArray[num++], fArray[num++], fArray[num++], fArray[num++]);
		}

		public static float[] ToArray(this Vector2 self)
		{
			int num = 0;
			return new float[2]
			{
				self[num++],
				self[num++]
			};
		}

		public static float[] ToArray(this Vector3 self)
		{
			int num = 0;
			return new float[3]
			{
				self[num++],
				self[num++],
				self[num++]
			};
		}

		public static float[] ToArray(this Vector4 self)
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

		public static Vector2 Get(this Vector2 self, Vector2 set, bool x = true, bool y = true)
		{
			return new Vector2((!x) ? self.x : set.x, (!y) ? self.y : set.y);
		}

		public static Vector3 Get(this Vector3 self, Vector3 set, bool x = true, bool y = true, bool z = true)
		{
			return new Vector3((!x) ? self.x : set.x, (!y) ? self.y : set.y, (!z) ? self.z : set.z);
		}

		public static Vector4 Get(this Vector4 self, Vector4 set, bool x = true, bool y = true, bool z = true, bool w = true)
		{
			return new Vector4((!x) ? self.x : set.x, (!y) ? self.y : set.y, (!z) ? self.z : set.z, (!w) ? self.w : set.w);
		}

		public static bool isNaN(this Vector2 self)
		{
			for (int i = 0; i < 2; i++)
			{
				if (float.IsNaN(self[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isNaN(this Vector3 self)
		{
			for (int i = 0; i < 3; i++)
			{
				if (float.IsNaN(self[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isNaN(this Vector4 self)
		{
			for (int i = 0; i < 4; i++)
			{
				if (float.IsNaN(self[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isInfinity(this Vector2 self)
		{
			for (int i = 0; i < 2; i++)
			{
				if (float.IsInfinity(self[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isInfinity(this Vector3 self)
		{
			for (int i = 0; i < 3; i++)
			{
				if (float.IsInfinity(self[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool isInfinity(this Vector4 self)
		{
			for (int i = 0; i < 4; i++)
			{
				if (float.IsInfinity(self[i]))
				{
					return true;
				}
			}
			return false;
		}
	}
}
