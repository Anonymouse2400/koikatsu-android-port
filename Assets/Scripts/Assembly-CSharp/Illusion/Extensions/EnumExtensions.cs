using System;
using System.Diagnostics;
using System.Text;

namespace Illusion.Extensions
{
	public static class EnumExtensions
	{
		[Conditional("UNITY_ASSERTIONS")]
		private static void Check(Enum self, Enum flag)
		{
		}

		public static bool HasFlag(this Enum self, Enum flag)
		{
			ulong num = Convert.ToUInt64(flag);
			return self.AND(num) == num;
		}

		public static ulong Add(this Enum self, Enum flag)
		{
			return self.OR(flag);
		}

		public static ulong Sub(this Enum self, Enum flag)
		{
			return Convert.ToUInt64(self) & flag.NOT();
		}

		public static ulong Get(this Enum self, Enum flag)
		{
			return self.AND(flag);
		}

		public static ulong Reverse(this Enum self, Enum flag)
		{
			return self.XOR(flag);
		}

		public static ulong NOT(this Enum self)
		{
			return ~Convert.ToUInt64(self);
		}

		public static ulong AND(this Enum self, Enum flag)
		{
			return Convert.ToUInt64(self) & Convert.ToUInt64(flag);
		}

		public static ulong AND(this Enum self, ulong flag)
		{
			return Convert.ToUInt64(self) & flag;
		}

		public static ulong OR(this Enum self, Enum flag)
		{
			return Convert.ToUInt64(self) | Convert.ToUInt64(flag);
		}

		public static ulong OR(this Enum self, ulong flag)
		{
			return Convert.ToUInt64(self) | flag;
		}

		public static ulong XOR(this Enum self, Enum flag)
		{
			return Convert.ToUInt64(self) ^ Convert.ToUInt64(flag);
		}

		public static ulong XOR(this Enum self, ulong flag)
		{
			return Convert.ToUInt64(self) ^ flag;
		}

		public static string LabelFormat(this Enum self, string label)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object value in Enum.GetValues(self.GetType()))
			{
				if (self.HasFlag((Enum)value))
				{
					stringBuilder.AppendFormat("{0} | ", value);
				}
			}
			return (stringBuilder.Length != 0) ? (label + stringBuilder) : string.Empty;
		}

		public static bool All(this Enum self)
		{
			return self.Reverse(self.Everything()) == 0;
		}

		public static bool Any(this Enum self)
		{
			return Convert.ToUInt64(self) != self.Nothing();
		}

		public static Enum Everything(this Enum self)
		{
			ulong num = 0uL;
			foreach (object value in Enum.GetValues(self.GetType()))
			{
				num += Convert.ToUInt64(value);
			}
			return (Enum)Enum.ToObject(self.GetType(), num);
		}

		public static ulong Nothing(this Enum self)
		{
			return 0uL;
		}

		public static ulong Normalize(this Enum self)
		{
			return (ulong)Enum.ToObject(self.GetType(), Convert.ToInt64(self) & Convert.ToInt64(self.Everything()));
		}
	}
}
