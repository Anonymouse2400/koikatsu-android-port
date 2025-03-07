using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Extensions
{
	public static class RectTransformExtensions
	{
		public static void SetPosition(this RectTransform self, Transform target3D, Vector3 setPos, Camera targetCamera = null)
		{
			if (targetCamera == null)
			{
				targetCamera = Camera.main;
			}
			Vector2 vector = RectTransformUtility.WorldToScreenPoint(targetCamera, target3D.position);
			self.position = new Vector2(vector[0] + setPos[0], vector[1] + setPos[1]);
		}

		public static void AdjustSize(this RectTransform self, Text text)
		{
			self.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
		}

		public static void AdjustHeight(this RectTransform self, Text text)
		{
			self.sizeDelta = new Vector2(self.sizeDelta.x, text.preferredHeight);
		}

		public static bool IsHeightOver(this RectTransform self, Text text)
		{
			return text.preferredHeight > self.rect.height;
		}

		public static void Left(this RectTransform self, float v)
		{
			self.offsetMin = new Vector2(v, self.offsetMin.y);
		}

		public static void Top(this RectTransform self, float v)
		{
			self.offsetMin = new Vector2(self.offsetMin.x, v);
		}

		public static void Right(this RectTransform self, float v)
		{
			self.offsetMax = new Vector2(0f - v, self.offsetMax.y);
		}

		public static void Bottom(this RectTransform self, float v)
		{
			self.offsetMax = new Vector2(self.offsetMax.x, 0f - v);
		}

		public static void Width(this RectTransform self, Vector2 v)
		{
			self.Left(v.x);
			self.Right(v.y);
		}

		public static void Height(this RectTransform self, Vector2 v)
		{
			self.Top(v.x);
			self.Bottom(v.y);
		}

		public static float Left(this RectTransform self)
		{
			return self.offsetMin.x;
		}

		public static float Top(this RectTransform self)
		{
			return self.offsetMin.y;
		}

		public static float Right(this RectTransform self)
		{
			return 0f - self.offsetMax.x;
		}

		public static float Bottom(this RectTransform self)
		{
			return 0f - self.offsetMax.y;
		}

		public static Vector2 Width(this RectTransform self)
		{
			return new Vector2(self.Left(), self.Right());
		}

		public static Vector2 Height(this RectTransform self)
		{
			return new Vector2(self.Top(), self.Bottom());
		}

		public static bool IsStretchWidth(this RectTransform self)
		{
			return self.anchorMin.x == 0f && self.anchorMax.x == 1f;
		}

		public static bool IsStretchHeight(this RectTransform self)
		{
			return self.anchorMin.y == 0f && self.anchorMax.y == 1f;
		}

		public static bool IsStretch(this RectTransform self)
		{
			return self.IsStretchWidth() || self.IsStretchHeight();
		}

		public static bool IsStretchWidthHeight(this RectTransform self)
		{
			return self.IsStretchWidth() && self.IsStretchHeight();
		}
	}
}
