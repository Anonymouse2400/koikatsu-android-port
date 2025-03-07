using System;
using System.Collections.Generic;
using MorphAssist;
using UnityEngine;

[Serializable]
public class MorphCtrlMouth : MorphFaceBase
{
	public bool useAjustWidthScale;

	private TimeProgressCtrlRandom tpcRand;

	public GameObject objAdjustWidthScale;

	[Range(0.01f, 1f)]
	public float randTimeMin = 0.1f;

	[Range(0.01f, 1f)]
	public float randTimeMax = 0.2f;

	[Range(0.1f, 2f)]
	public float randScaleMin = 0.65f;

	[Range(0.1f, 2f)]
	public float randScaleMax = 1f;

	[Range(0f, 1f)]
	public float openRefValue = 0.2f;

	private float sclNow = 1f;

	private float sclStart = 1f;

	private float sclEnd = 1f;

	public new void Init(List<MorphingTargetInfo> MorphTargetList)
	{
		base.Init(MorphTargetList);
		tpcRand = new TimeProgressCtrlRandom();
		tpcRand.Init(randTimeMin, randTimeMax);
	}

	public void CalcBlend(float openValue)
	{
		openRate = openValue;
		CalculateBlendVertex();
		if (useAjustWidthScale)
		{
			AdjustWidthScale();
		}
	}

	public void UseAdjustWidthScale(bool useFlags)
	{
		useAjustWidthScale = useFlags;
	}

	public bool AdjustWidthScale()
	{
		if (null == objAdjustWidthScale)
		{
			return false;
		}
		bool flag = false;
		float num = tpcRand.Calculate();
		if (num == 1f)
		{
			sclStart = (sclNow = sclEnd);
			sclEnd = UnityEngine.Random.Range(randScaleMin, randScaleMax);
			flag = true;
		}
		if (flag)
		{
			num = 0f;
		}
		sclNow = Mathf.Lerp(sclStart, sclEnd, num);
		sclNow = Mathf.Max(0f, sclNow - openRefValue * openRate);
		if (0.2f < openRate)
		{
			objAdjustWidthScale.transform.localScale = new Vector3(sclNow, 1f, 1f);
		}
		return true;
	}
}
