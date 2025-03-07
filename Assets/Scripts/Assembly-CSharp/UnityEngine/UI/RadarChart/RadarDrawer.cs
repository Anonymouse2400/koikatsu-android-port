using System;
using System.Linq;

namespace UnityEngine.UI.RadarChart
{
	[AddComponentMenu("UI/RadarChart/Create")]
	public class RadarDrawer : BaseDrawer
	{
		[Serializable]
		public struct Status
		{
			public Vector2 point;

			[Range(0f, 1f)]
			[Header("Values")]
			public float[] values;

			[Range(0f, 1f)]
			public float defaultValue;

			[Header("Fill")]
			public FillDrawer.Status fillStatus;

			[Header("Line")]
			public bool isDrawLine;

			public Color lineColor;

			public LineDrawer.Status lineStatus;
		}

		public Status status;

		public static void Reset(ref Status rs, RectTransform rt)
		{
			rs.isDrawLine = true;
			rs.lineColor = Color.white;
			FillDrawer.Reset(ref rs.fillStatus);
			LineDrawer.Reset(ref rs.lineStatus, rt);
		}

		public static void Set(VertexHelper vh, Color color, ref Status status)
		{
			if (!(status.lineStatus.rt == null))
			{
				status.fillStatus.points = CalculatePosition(ref status);
				FillDrawer.Set(vh, color, ref status.fillStatus);
				if (status.isDrawLine)
				{
					LineDrawer.Set(vh, status.lineColor, ref status.lineStatus);
				}
			}
		}

		public static Vector2[] CalculatePosition(ref Status status)
		{
			if (status.values == null || status.values.Length == 0)
			{
				return null;
			}
			Vector2[] points = GetPoints(ref status.lineStatus);
			if (points.Length == 0)
			{
				return null;
			}
			int segment = status.lineStatus.segment;
			float[] values = status.values.Concat(Enumerable.Repeat(status.defaultValue, Mathf.Max(0, segment - status.values.Length))).ToArray();
			return (from i in Enumerable.Range(0, segment)
				select points[i] * values[i]).Concat(new Vector2[1] { status.point }).ToArray();
		}

		private static Vector2[] GetPoints(ref LineDrawer.Status lineStatus)
		{
			return GetPoints(lineStatus.rt, lineStatus.segment, lineStatus.offsetAngle);
		}

		private static Vector2[] GetPoints(RectTransform rt, int segment, float offsetAngle)
		{
			Vector2 size = rt.rect.size * 0.5f;
			return (from i in Enumerable.Range(0, segment)
				select (offsetAngle + 360f / (float)segment * (float)i) * ((float)Math.PI / 180f) into angle
				select new Vector2(Mathf.Sin(angle) * size.x, Mathf.Cos(angle) * size.y)).ToArray();
		}

		protected override void OnPopulateMeshProcess(VertexHelper vh)
		{
			Set(vh, color, ref status);
		}
	}
}
