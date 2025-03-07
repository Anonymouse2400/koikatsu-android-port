using UnityEngine;

namespace IllusionUtility.SetUtility
{
	public static class TransformPositionEx
	{
		public static void SetPositionX(this Transform transform, float x)
		{
			Vector3 position = new Vector3(x, transform.position.y, transform.position.z);
			transform.position = position;
		}

		public static void SetPositionY(this Transform transform, float y)
		{
			Vector3 position = new Vector3(transform.position.x, y, transform.position.z);
			transform.position = position;
		}

		public static void SetPositionZ(this Transform transform, float z)
		{
			Vector3 position = new Vector3(transform.position.x, transform.position.y, z);
			transform.position = position;
		}

		public static void SetPosition(this Transform transform, float x, float y, float z)
		{
			Vector3 position = new Vector3(x, y, z);
			transform.position = position;
		}

		public static void SetLocalPositionX(this Transform transform, float x)
		{
			Vector3 localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
			transform.localPosition = localPosition;
		}

		public static void SetLocalPositionY(this Transform transform, float y)
		{
			Vector3 localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
			transform.localPosition = localPosition;
		}

		public static void SetLocalPositionZ(this Transform transform, float z)
		{
			Vector3 localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
			transform.localPosition = localPosition;
		}

		public static void SetLocalPosition(this Transform transform, float x, float y, float z)
		{
			Vector3 localPosition = new Vector3(x, y, z);
			transform.localPosition = localPosition;
		}
	}
}
