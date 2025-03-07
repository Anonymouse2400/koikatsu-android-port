using UnityEngine;

namespace IllusionUtility.SetUtility
{
	public static class TransformScaleEx
	{
		public static void SetLocalScaleX(this Transform transform, float x)
		{
			Vector3 localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
			transform.localScale = localScale;
		}

		public static void SetLocalScaleY(this Transform transform, float y)
		{
			Vector3 localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
			transform.localScale = localScale;
		}

		public static void SetLocalScaleZ(this Transform transform, float z)
		{
			Vector3 localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
			transform.localScale = localScale;
		}

		public static void SetLocalScale(this Transform transform, float x, float y, float z)
		{
			Vector3 localScale = new Vector3(x, y, z);
			transform.localScale = localScale;
		}
	}
}
