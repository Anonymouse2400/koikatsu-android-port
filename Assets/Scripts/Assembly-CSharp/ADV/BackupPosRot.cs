using UnityEngine;

namespace ADV
{
	public class BackupPosRot
	{
		public Vector3 position { get; private set; }

		public Quaternion rotation { get; private set; }

		public BackupPosRot(Transform transform)
		{
			if (!(transform == null))
			{
				position = transform.localPosition;
				rotation = transform.localRotation;
			}
		}

		public void Set(Transform transform)
		{
			if (!(transform == null))
			{
				transform.localPosition = position;
				transform.localRotation = rotation;
			}
		}
	}
}
