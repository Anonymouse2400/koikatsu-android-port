using System;

namespace UnityEngine.UI.RadarChart
{
	[AddComponentMenu("UI/RadarChart/Fill")]
	public class FillDrawer : BaseDrawer
	{
		[Serializable]
		public struct Status
		{
			[Range(0f, 1f)]
			public float fillPercent;

			public Vector2[] points;
		}

		public Status status;

		public static void Reset(ref Status rs)
		{
			rs.fillPercent = 0f;
			rs.points = null;
		}

		public static void Set(VertexHelper vh, Color color, ref Status status)
		{
			Vector2[] points = status.points;
			if (points == null)
			{
				return;
			}
			int segment = points.Length - 1;
			if (segment > 0)
			{
				float fillPercent = status.fillPercent;
				Func<int, Vector2> func = (int i) => Vector2.Lerp(points[segment], points[i], fillPercent);
				for (int j = 0; j < segment; j++)
				{
					Vector2 vector = points[j];
					Vector2 vector2 = ((j + 1 != segment) ? points[j + 1] : points[0]);
					Vector2 vector3 = ((j + 1 != segment) ? func(j + 1) : func(0));
					Vector2 vector4 = func(j);
					vh.AddUIVertexQuad(BaseDrawer.GetVert(color, vector, vector2, vector3, vector4));
				}
			}
		}

		protected override void OnPopulateMeshProcess(VertexHelper vh)
		{
			Set(vh, color, ref status);
		}
	}
}
