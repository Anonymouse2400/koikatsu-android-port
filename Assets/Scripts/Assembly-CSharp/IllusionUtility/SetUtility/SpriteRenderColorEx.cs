using UnityEngine;

namespace IllusionUtility.SetUtility
{
	public static class SpriteRenderColorEx
	{
		public static void SetColorR(this SpriteRenderer sr, float r)
		{
			Color color = new Color(r, sr.color.g, sr.color.b, sr.color.a);
			sr.color = color;
		}

		public static void SetColorG(this SpriteRenderer sr, float g)
		{
			Color color = new Color(sr.color.r, g, sr.color.b, sr.color.a);
			sr.color = color;
		}

		public static void SetColorB(this SpriteRenderer sr, float b)
		{
			Color color = new Color(sr.color.r, sr.color.g, b, sr.color.a);
			sr.color = color;
		}

		public static void SetColorA(this SpriteRenderer sr, float a)
		{
			Color color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
			sr.color = color;
		}
	}
}
