using Illusion;
using UnityEngine;

public static class DayTimeScheduler
{
	public enum DayTime
	{
		Morning = 0,
		Noon = 1,
		Evening = 2,
		Night = 3
	}

	private static float _pitch;

	public static float pitch
	{
		get
		{
			return _pitch;
		}
		set
		{
			_pitch = Mathf.Clamp(value, 0f, 360f);
		}
	}

	public static GameObject SunLightObject { get; set; }

	public static DayTime daytime
	{
		get
		{
			int num = 0;
			int length = Utils.Enum<DayTime>.Length;
			int num2 = (int)DivisionAngle(length);
			while (pitch > (float)(num2 * ++num))
			{
			}
			if (--num >= length)
			{
				return (DayTime)(length - 1);
			}
			return (DayTime)num;
		}
		set
		{
			pitch = (float)value * DivisionAngle(Utils.Enum<DayTime>.Length);
		}
	}

	public static string GetName(this DayTime self)
	{
		switch (self)
		{
		case DayTime.Morning:
			return "朝";
		case DayTime.Noon:
			return "昼";
		case DayTime.Evening:
			return "夕方";
		case DayTime.Night:
			return "夜";
		default:
			return string.Empty;
		}
	}

	public static DayTime Convert(string str)
	{
		foreach (DayTime item in Utils.Enum<DayTime>.Enumerate())
		{
			if (item.GetName() == str)
			{
				return item;
			}
		}
		return Utils.Enum<DayTime>.Cast(str);
	}

	public static void Set(string daytime)
	{
		Set(Convert(daytime));
	}

	public static void Set(DayTime _daytime)
	{
		daytime = _daytime;
		Set(pitch);
	}

	public static void Set(float _pitch)
	{
		pitch = _pitch;
		Update();
	}

	public static void Update()
	{
		if (SunLightObject == null)
		{
			SunLightObject = GameObject.FindWithTag("Sun");
			if (SunLightObject == null)
			{
				return;
			}
		}
		SunLightObject.transform.rotation = Quaternion.Euler(pitch, 0f, 0f);
	}

	public static float DivisionAngle(int len)
	{
		return 360 / len;
	}
}
