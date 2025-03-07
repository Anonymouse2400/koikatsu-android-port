using System;
using UnityEngine;

[Serializable]
public class MorphCalcInfo
{
	public GameObject TargetObj;

	public Mesh OriginalMesh;

	public Mesh TargetMesh;

	public Vector3[] OriginalPos;

	public Vector3[] OriginalNormal;

	public bool WeightFlags;

	public int[] UpdateIndex;

	public MorphUpdateInfo[] UpdateInfo;
}
