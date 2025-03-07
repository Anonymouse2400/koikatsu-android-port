using UnityEngine;

namespace IllusionUtility.SetUtility
{
	public static class TransformRotationEx
	{
		public static void SetRotationX(this Transform transform, float x)
		{
			Vector3 euler = new Vector3(x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
			transform.rotation = Quaternion.Euler(euler);
		}

		public static void SetRotationY(this Transform transform, float y)
		{
			Vector3 euler = new Vector3(transform.rotation.eulerAngles.x, y, transform.rotation.eulerAngles.z);
			transform.rotation = Quaternion.Euler(euler);
		}

		public static void SetRotationZ(this Transform transform, float z)
		{
			Vector3 euler = new Vector3(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, z);
			transform.rotation = Quaternion.Euler(euler);
		}

		public static void SetRotation(this Transform transform, float x, float y, float z)
		{
			Vector3 euler = new Vector3(x, y, z);
			transform.rotation = Quaternion.Euler(euler);
		}

		public static void SetLocalRotationX(this Transform transform, float x)
		{
			Vector3 euler = new Vector3(x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z);
			transform.localRotation = Quaternion.Euler(euler);
		}

		public static void SetLocalRotationY(this Transform transform, float y)
		{
			Vector3 euler = new Vector3(transform.localRotation.eulerAngles.x, y, transform.localRotation.eulerAngles.z);
			transform.localRotation = Quaternion.Euler(euler);
		}

		public static void SetLocalRotationZ(this Transform transform, float z)
		{
			Vector3 euler = new Vector3(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, z);
			transform.localRotation = Quaternion.Euler(euler);
		}

		public static void SetLocalRotation(this Transform transform, float x, float y, float z)
		{
			Vector3 euler = new Vector3(x, y, z);
			transform.localRotation = Quaternion.Euler(euler);
		}
	}
}
