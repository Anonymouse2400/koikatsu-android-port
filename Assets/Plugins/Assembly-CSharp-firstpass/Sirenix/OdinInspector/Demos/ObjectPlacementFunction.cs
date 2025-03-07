using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
	public abstract class ObjectPlacementFunction
	{
		public abstract Vector3 GetPosition(float t);

		public virtual Vector3 GetTangent(float t)
		{
			float num = Mathf.Clamp01(t + 0.01f);
			float num2 = Mathf.Clamp01(t - 0.01f);
			if (num2 >= num)
			{
				num = num2 - 0.01f;
			}
			if (num <= num2)
			{
				num2 = num + 0.01f;
			}
			Vector3 position = GetPosition(num);
			Vector3 position2 = GetPosition(num2);
			return (position2 - position).normalized;
		}

		public virtual Vector3 GetBinormal(float t)
		{
			Vector3 tangent = GetTangent(t);
			return new Vector3(0f - tangent.z, 0f, tangent.x);
		}
	}
}
