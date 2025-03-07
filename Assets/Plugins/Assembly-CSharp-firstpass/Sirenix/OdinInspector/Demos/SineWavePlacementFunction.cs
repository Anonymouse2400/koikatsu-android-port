using System;
using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	[Serializable]
	public class SineWavePlacementFunction : ObjectPlacementFunction
	{
		[Range(0f, 6f)]
		public float Frequency = 2f;

		[Range(0f, 1f)]
		public float Amplitude = 0.3f;

		[Wrap(0.0, 2.0)]
		public float Offset;

		[Wrap(0.0, 360.0)]
		public float Angle;

		public override Vector3 GetPosition(float t)
		{
			float f = Angle * ((float)Math.PI / 180f);
			Vector3 vector = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			Vector3 vector2 = new Vector3(0f - vector.z, 0f, vector.x);
			return vector * (t * 2f - 1f) + vector2 * Mathf.Sin((t + Offset) * (float)Math.PI * 2f * Frequency) * Amplitude;
		}
	}
}
