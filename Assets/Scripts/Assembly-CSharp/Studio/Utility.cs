using System;
using System.IO;
using UnityEngine;

namespace Studio
{
	internal static class Utility
	{
		public static bool SetColor(ref Color currentValue, Color newValue)
		{
			if (currentValue.r == newValue.r && currentValue.g == newValue.g && currentValue.b == newValue.b && currentValue.a == newValue.a)
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
		{
			if (currentValue.Equals(newValue))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static bool SetClass<T>(ref T currentValue, T newValue) where T : class
		{
			if ((currentValue == null && newValue == null) || (currentValue != null && currentValue.Equals(newValue)))
			{
				return false;
			}
			currentValue = newValue;
			return true;
		}

		public static void SaveColor(BinaryWriter _writer, Color _color)
		{
			_writer.Write(_color.r);
			_writer.Write(_color.g);
			_writer.Write(_color.b);
			_writer.Write(_color.a);
		}

		public static Color LoadColor(BinaryReader _reader)
		{
			Color result = default(Color);
			result.r = _reader.ReadSingle();
			result.g = _reader.ReadSingle();
			result.b = _reader.ReadSingle();
			result.a = _reader.ReadSingle();
			return result;
		}

		public static T LoadAsset<T>(string _bundle, string _file, string _manifest) where T : UnityEngine.Object
		{
			return CommonLib.LoadAsset<T>(_bundle, _file, true, _manifest);
		}

		public static float StringToFloat(string _text)
		{
			float result = 0f;
			return (!float.TryParse(_text, out result)) ? 0f : result;
		}

		public static string GetCurrentTime()
		{
			DateTime now = DateTime.Now;
			return string.Format("{0}_{1:00}{2:00}_{3:00}{4:00}_{5:00}_{6:000}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
		}

		public static Color ConvertColor(int _r, int _g, int _b)
		{
			Color color;
			return (!ColorUtility.TryParseHtmlString(string.Format("#{0:X2}{1:X2}{2:X2}", _r, _g, _b), out color)) ? Color.clear : color;
		}
	}
}
