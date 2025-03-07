using System;
using UnityEngine;

[Serializable]
public class EyeObject
{
	public Transform eyeTransform;

	public EYE_LR eyeLR;

	internal float angleH;

	internal float angleV;

	internal Vector3 dirUp;

	internal Vector3 referenceLookDir;

	internal Vector3 referenceUpDir;

	internal Quaternion origRotation;
}
