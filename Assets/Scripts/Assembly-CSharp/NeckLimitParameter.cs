using System;
using UnityEngine;

[Serializable]
public class NeckLimitParameter
{
	public string name;

	[Tooltip("上を向く際の限界値(値が大きくなればなるほど上を向けられる)")]
	[Range(-100f, 0f)]
	public float upBendingAngle = -1f;

	[Tooltip("下を向く際の限界値(値が大きくなればなるほど下を向けられる)")]
	[Range(0f, 100f)]
	public float downBendingAngle = 6f;

	[Range(-100f, 0f)]
	[Tooltip("左側を向く際の限界値")]
	public float minBendingAngle = -6f;

	[Tooltip("右側を向く際の限界値")]
	[Range(0f, 100f)]
	public float maxBendingAngle = 6f;
}
