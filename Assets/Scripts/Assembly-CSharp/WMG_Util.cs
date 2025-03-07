using System.Collections.Generic;
using UnityEngine;

public static class WMG_Util
{
	public static float RemapFloat(float val, float start1, float end1, float start2, float end2)
	{
		return start2 + (val - start1) / (end1 - start1) * (end2 - start2);
	}

	public static Vector2 RemapVec2(float val, float start1, float end1, Vector2 start2, Vector2 end2)
	{
		float num = (val - start1) / (end1 - start1);
		return new Vector2(start2.x + num * (end2.x - start2.x), start2.y + num * (end2.y - start2.y));
	}

	public static Sprite createSprite(Texture2D tex)
	{
		Texture2D texture2D = new Texture2D(tex.width, tex.height, TextureFormat.RGBA32, false);
		texture2D.filterMode = FilterMode.Bilinear;
		return Sprite.Create(texture2D, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
	}

	public static Texture2D createTexture(int resolution)
	{
		Texture2D texture2D = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		return texture2D;
	}

	public static Texture2D createTexture(int x, int y)
	{
		Texture2D texture2D = new Texture2D(x, y, TextureFormat.RGBA32, false);
		texture2D.filterMode = FilterMode.Point;
		texture2D.wrapMode = TextureWrapMode.Clamp;
		return texture2D;
	}

	public static Sprite createAlphaSprite(int resolution)
	{
		Texture2D texture = new Texture2D(resolution, resolution, TextureFormat.Alpha8, false);
		return Sprite.Create(texture, new Rect(0f, 0f, resolution, resolution), new Vector2(0.5f, 0.5f), 100f);
	}

	public static void listChanged<T>(bool editorChange, ref WMG_List<T> w_list, ref List<T> list, bool oneValChanged, int index)
	{
		if (editorChange)
		{
			if (oneValChanged)
			{
				w_list.SetValViaEditor(index, list[index]);
			}
			else
			{
				w_list.SetListViaEditor(list);
			}
		}
		else if (oneValChanged)
		{
			list[index] = w_list[index];
		}
		else
		{
			list = new List<T>(w_list);
		}
	}

	public static void updateBandColors(ref Color[] colors, float maxSize, float inner, float outer, bool antiAliasing, float antiAliasingStrength, Color[] orig = null)
	{
		int num = Mathf.RoundToInt(Mathf.Sqrt(colors.Length));
		float num2 = maxSize / (float)num;
		inner /= num2;
		outer /= num2;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = i + num * j;
				Color color = ((orig != null) ? orig[num3] : new Color(1f, 1f, 1f, 1f));
				int num4 = i - num / 2;
				int num5 = j - num / 2;
				float num6 = Mathf.Sqrt(num4 * num4 + num5 * num5);
				if (num6 >= inner && num6 < outer)
				{
					if (antiAliasing)
					{
						if (num6 >= inner + antiAliasingStrength && num6 < outer - antiAliasingStrength)
						{
							colors[num3] = color;
						}
						else if (num6 > inner + antiAliasingStrength)
						{
							colors[num3] = new Color(color.r, color.g, color.b, (outer - num6) / antiAliasingStrength);
						}
						else
						{
							colors[num3] = new Color(color.r, color.g, color.b, (num6 - inner) / antiAliasingStrength);
						}
					}
					else
					{
						colors[num3] = color;
					}
				}
				else
				{
					colors[num3] = new Color(1f, 1f, 1f, 0f);
				}
			}
		}
	}

	public static bool LineIntersectsCircle(float x0, float y0, float x1, float y1, float x2, float y2, float r)
	{
		float num = x1 - x0;
		float num2 = y1 - y0;
		float num3 = x2 - x0;
		float num4 = y2 - y0;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		if (r * r * (num * num + num2 * num2) - (num4 * num - num3 * num2) * (num4 * num - num3 * num2) >= 0f)
		{
			if (num3 * num3 + num4 * num4 <= r * r)
			{
				flag = true;
			}
			if ((num - num3) * (num - num3) + (num2 - num4) * (num2 - num4) <= r * r)
			{
				flag2 = true;
			}
			if (!flag && !flag2 && num3 * num + num4 * num2 >= 0f && num3 * num + num4 * num2 <= num * num + num2 * num2)
			{
				flag3 = true;
			}
		}
		return flag || flag2 || flag3;
	}

	public static bool LineSegmentsIntersect(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y, float p4x, float p4y)
	{
		if (PointInterArea(p1x, p1y, p2x, p2y, p3x, p3y) * PointInterArea(p1x, p1y, p2x, p2y, p4x, p4y) < 0f && PointInterArea(p3x, p3y, p4x, p4y, p1x, p1y) * PointInterArea(p3x, p3y, p4x, p4y, p2x, p2y) < 0f)
		{
			return true;
		}
		return false;
	}

	private static float PointInterArea(float p1x, float p1y, float p2x, float p2y, float p3x, float p3y)
	{
		return (p2y - p1y) * (p3x - p2x) - (p2x - p1x) * (p3y - p2y);
	}
}
