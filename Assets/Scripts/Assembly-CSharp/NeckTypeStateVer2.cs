using System;
using UnityEngine;

[Serializable]
public class NeckTypeStateVer2
{
	public string name;

	[Tooltip("各骨のパラメーター")]
	public NeckLimitParameter[] aParam;

	[Range(0f, 100f)]
	[Tooltip("補間速度")]
	public float leapSpeed = 2.5f;

	[Range(0f, 180f)]
	[Tooltip("視野角(正面からどの範囲までターゲットを追うか)水色線")]
	public float hAngleLimit = 110f;

	[Range(0f, 180f)]
	[Tooltip("視野角(正面からどの範囲までターゲットを追うか)水色線")]
	public float vAngleLimit = 80f;

	[Range(0f, 180f)]
	[Tooltip("視野角から離脱するまでの補正値(黄色線 黄色線を超えると離脱する)")]
	public float limitBreakCorrectionValue;

	[Range(0f, 180f)]
	[Tooltip("逸らすときに逆を向くための範囲\n[首が向ける最大角度からの]\n紫線")]
	public float limitAway = 20f;

	[SerializeField]
	[Tooltip("デバッグ用表示")]
	internal bool isLimitBreakBackup;

	public NECK_LOOK_TYPE_VER2 lookType = NECK_LOOK_TYPE_VER2.TARGET;
}
