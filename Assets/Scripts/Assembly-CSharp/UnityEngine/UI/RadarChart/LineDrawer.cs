using System;

namespace UnityEngine.UI.RadarChart
{
	[AddComponentMenu("UI/RadarChart/Line")]
	public class LineDrawer : BaseDrawer
	{
		[Serializable]
		public struct Status
		{
			[Header("Segment")]
			[Range(3f, 360f)]
			public int segment;

			public Vector2 point;

			[Range(0f, 360f)]
			public float offsetAngle;

			[Range(0f, 10f)]
			public float lineWidth;

			[Header("Length")]
			public bool isLineLength;

			[Range(0f, 100f)]
			public float lineLength;

			public RectTransform rt;
		}

		public Status status;

		public static void Reset(ref Status rs, RectTransform rt)
		{
			rs.lineWidth = 0.5f;
			rs.isLineLength = true;
			rs.lineLength = 50f;
			rs.rt = rt;
		}

		public static void Set(VertexHelper vh, Color color, ref Status status)
		{
			int segment = status.segment;
			if (segment > 0)
			{
				Vector2 vector = ((!status.isLineLength) ? (Vector2.one * status.lineLength) : (status.rt.rect.size * 0.5f));
				float lineWidth = status.lineWidth;
				for (int i = 0; i < segment; i++)
				{
					float angle = status.offsetAngle + 360f / (float)segment * (float)i;
					float num = 90f;
					Vector2 vector2 = new Vector2(Sin(angle, 0f) * vector.x, Cos(angle, 0f) * vector.y);
					Vector2 vector3 = new Vector2(Sin(angle, num) * lineWidth, Cos(angle, num) * lineWidth);
					Vector2 vector4 = new Vector2(Sin(angle, 0f - num) * lineWidth, Cos(angle, 0f - num) * lineWidth);
					Vector2 point = status.point;
					vh.AddUIVertexQuad(BaseDrawer.GetVert(color, vector2 + vector3, vector2 + vector4, point + vector4, point + vector3));
				}
			}
		}

		private static float Sin(float angle, float offset)
		{
			return Mathf.Sin(Func(angle, offset));
		}

		private static float Cos(float angle, float offset)
		{
			return Mathf.Cos(Func(angle, offset));
		}

		private static float Func(float angle, float offset)
		{
			return (angle + offset) * ((float)Math.PI / 180f);
		}

		protected override void OnPopulateMeshProcess(VertexHelper vh)
		{
			Set(vh, color, ref status);
		}
	}
}
