using System;
using UnityEngine;

[Serializable]
public class NeckObjectVer2
{
	public string name;

	[Tooltip("計算参照オブジェクト こいつから計算している")]
	public Transform referenceCalc;

	[Tooltip("実際動かすオブジェクト")]
	public Transform neckBone;

	[Tooltip("リングオブジェクト")]
	public Transform controlBone;

	[Tooltip("デバッグ用表示")]
	[SerializeField]
	internal Quaternion fixAngle;

	[Tooltip("デバッグ用表示")]
	[SerializeField]
	internal float angleHRate;

	[Tooltip("デバッグ用表示")]
	[SerializeField]
	internal float angleVRate;

	[Tooltip("デバッグ用表示")]
	[SerializeField]
	internal float angleH;

	[SerializeField]
	[Tooltip("デバッグ用表示")]
	internal float angleV;

	internal Quaternion fixAngleBackup;

	internal Quaternion backupLocalRotaionByTarget;
}
