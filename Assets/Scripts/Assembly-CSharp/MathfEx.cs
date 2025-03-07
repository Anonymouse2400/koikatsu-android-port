using System;
using UnityEngine;

public static class MathfEx
{
	public static float LerpAccel(float from, float to, float t)
	{
		return Mathf.Lerp(from, to, Mathf.Sqrt(t));
	}

	public static bool IsRange<T>(T min, T n, T max, bool isEqual) where T : IComparable
	{
		return (!isEqual) ? RangeEqualOff(min, n, max) : RangeEqualOn(min, n, max);
	}

	public static bool RangeEqualOn<T>(T min, T n, T max) where T : IComparable
	{
		return n.CompareTo(max) <= 0 && n.CompareTo(min) >= 0;
	}

	public static bool RangeEqualOff<T>(T min, T n, T max) where T : IComparable
	{
		return n.CompareTo(max) < 0 && n.CompareTo(min) > 0;
	}

	public static float LerpBrake(float from, float to, float t)
	{
		return Mathf.Lerp(from, to, t * (2f - t));
	}

	public static int LoopValue(ref int value, int start, int end)
	{
		if (value > end)
		{
			value = start;
		}
		else if (value < start)
		{
			value = end;
		}
		return value;
	}

	public static int LoopValue(int value, int start, int end)
	{
		return LoopValue(ref value, start, end);
	}

	public static Rect AspectRect(float baseH = 1280f, float rate = 720f)
	{
		float y = ((float)Screen.height - (float)Screen.width / baseH * rate) * 0.5f / (float)Screen.height;
		float height = rate * (float)Screen.width / baseH / (float)Screen.height;
		return new Rect(0f, y, 1f, height);
	}

	public static long Min(long _a, long _b)
	{
		return (_a <= _b) ? _a : _b;
	}

	public static long Max(long _a, long _b)
	{
		return (_a <= _b) ? _b : _a;
	}

	public static long Clamp(long _value, long _min, long _max)
	{
		return Min(Max(_value, _min), _max);
	}

	public static float ToRadian(float degree)
	{
		return degree * ((float)Math.PI / 180f);
	}

	public static float ToDegree(float radian)
	{
		return radian * 57.29578f;
	}
}
