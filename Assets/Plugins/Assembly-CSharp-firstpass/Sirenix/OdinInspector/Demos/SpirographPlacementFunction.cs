using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class SpirographPlacementFunction : ObjectPlacementFunction
	{
		[Range(0f, 1f)]
		public float Arc = 1f;

		[Range(0f, 12f)]
		public float Arms = 4f;

		public override Vector3 GetPosition(float t)
		{
			float num = t * (float)Math.PI * 2f * Arc;
			return new Vector3(Mathf.Cos(num), 0f, Mathf.Sin(num)) * (1f - (Mathf.Cos(num * Arms) * 0.5f + 0.5f));
		}
	}
}
