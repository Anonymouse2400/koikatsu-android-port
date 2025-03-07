using System;
using UnityEngine;

[Serializable]
public class NeckTypeState
{
	[Range(-10f, 10f)]
	public float thresholdAngleDifference;

	[Range(0f, 10f)]
	public float bendingMultiplier = 0.4f;

	[Range(0f, 100f)]
	public float maxAngleDifference = 10f;

	[Range(-100f, 0f)]
	public float upBendingAngle = -1f;

	[Range(0f, 100f)]
	public float downBendingAngle = 6f;

	[Range(-100f, 0f)]
	public float minBendingAngle = -6f;

	[Range(0f, 100f)]
	public float maxBendingAngle = 6f;

	[Range(0f, 100f)]
	public float leapSpeed = 2.5f;

	[Range(0f, 100f)]
	public float forntTagDis = 50f;

	[Range(0f, 100f)]
	public float nearDis = 2f;

	[Range(0f, 180f)]
	public float hAngleLimit = 110f;

	[Range(0f, 180f)]
	public float vAngleLimit = 80f;

	public NECK_LOOK_TYPE lookType = NECK_LOOK_TYPE.TARGET;
}
