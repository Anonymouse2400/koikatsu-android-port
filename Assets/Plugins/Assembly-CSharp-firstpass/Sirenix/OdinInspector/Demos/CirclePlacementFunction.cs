using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class CirclePlacementFunction : ObjectPlacementFunction
	{
		[Range(0f, 1f)]
		public float Arc = 1f;

		public override Vector3 GetPosition(float t)
		{
			float f = t * (float)Math.PI * 2f * Arc;
			return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
		}
	}
}
