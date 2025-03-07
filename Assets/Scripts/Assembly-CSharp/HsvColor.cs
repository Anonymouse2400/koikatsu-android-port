using System;
using UnityEngine;

[Serializable]
public class HsvColor
{
	private float _h;

	private float _s;

	private float _v;

	public float this[int index]
	{
		get
		{
			switch (index)
			{
			case 0:
				return H;
			case 1:
				return S;
			case 2:
				return V;
			default:
				return float.MaxValue;
			}
		}
		set
		{
			switch (index)
			{
			case 0:
				H = value;
				break;
			case 1:
				S = value;
				break;
			case 2:
				V = value;
				break;
			}
		}
	}

	public float H
	{
		get
		{
			return _h;
		}
		set
		{
			_h = value;
		}
	}

	public float S
	{
		get
		{
			return _s;
		}
		set
		{
			_s = value;
		}
	}

	public float V
	{
		get
		{
			return _v;
		}
		set
		{
			_v = value;
		}
	}

	public HsvColor(float hue, float saturation, float brightness)
	{
		if (hue < 0f || 360f < hue)
		{
			throw new ArgumentException("hueは0~360の値です。", "hue");
		}
		if (saturation < 0f || 1f < saturation)
		{
			throw new ArgumentException("saturationは0以上1以下の値です。", "saturation");
		}
		if (brightness < 0f || 1f < brightness)
		{
			throw new ArgumentException("brightnessは0以上1以下の値です。", "brightness");
		}
		_h = hue;
		_s = saturation;
		_v = brightness;
	}

	public HsvColor(HsvColor src)
	{
		_h = src._h;
		_s = src._s;
		_v = src._v;
	}

	public void Copy(HsvColor src)
	{
		_h = src._h;
		_s = src._s;
		_v = src._v;
	}

	public static HsvColor FromRgb(Color rgb)
	{
		float r = rgb.r;
		float g = rgb.g;
		float b = rgb.b;
		float num = Math.Max(r, Math.Max(g, b));
		float num2 = Math.Min(r, Math.Min(g, b));
		float hue = 0f;
		if (num == num2)
		{
			hue = 0f;
		}
		else if (num == r)
		{
			hue = (60f * (g - b) / (num - num2) + 360f) % 360f;
		}
		else if (num == g)
		{
			hue = 60f * (b - r) / (num - num2) + 120f;
		}
		else if (num == b)
		{
			hue = 60f * (r - g) / (num - num2) + 240f;
		}
		float num3 = 0f;
		num3 = ((num != 0f) ? ((num - num2) / num) : 0f);
		float brightness = num;
		return new HsvColor(hue, num3, brightness);
	}

	public static Color ToRgb(float h, float s, float v)
	{
		return ToRgb(new HsvColor(h, s, v));
	}

	public static Color ToRgb(HsvColor hsv)
	{
		float v = hsv.V;
		float s = hsv.S;
		float r;
		float g;
		float b;
		if (s == 0f)
		{
			r = v;
			g = v;
			b = v;
		}
		else
		{
			float num = hsv.H / 60f;
			int num2 = (int)Math.Floor(num) % 6;
			float num3 = num - (float)Math.Floor(num);
			float num4 = v * (1f - s);
			float num5 = v * (1f - s * num3);
			float num6 = v * (1f - s * (1f - num3));
			switch (num2)
			{
			case 0:
				r = v;
				g = num6;
				b = num4;
				break;
			case 1:
				r = num5;
				g = v;
				b = num4;
				break;
			case 2:
				r = num4;
				g = v;
				b = num6;
				break;
			case 3:
				r = num4;
				g = num5;
				b = v;
				break;
			case 4:
				r = num6;
				g = num4;
				b = v;
				break;
			case 5:
				r = v;
				g = num4;
				b = num5;
				break;
			default:
				throw new ArgumentException("色相の値が不正です。", "hsv");
			}
		}
		return new Color(r, g, b);
	}

	public static Color ToRgba(HsvColor hsv, float alpha)
	{
		Color result = ToRgb(hsv);
		result.a = alpha;
		return result;
	}
}
