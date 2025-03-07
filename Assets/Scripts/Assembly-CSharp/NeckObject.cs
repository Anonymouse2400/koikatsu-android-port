using System;
using UnityEngine;

[Serializable]
public class NeckObject
{
	public Transform neckTransform;

	internal float angleH;

	internal float angleV;

	internal Vector3 dirUp;

	internal Vector3 referenceLookDir;

	internal Vector3 referenceUpDir;

	internal Quaternion origRotation;
}
