using System;
using FBSAssist;
using UnityEngine;

[Serializable]
public class FBSCtrlMouth : FBSBase
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

	private float adjustWidthScale = 1f;

	public float GetAdjustWidthScale()
	{
		return adjustWidthScale;
	}

	public new void Init()
	{
		base.Init();
		tpcRand = new TimeProgressCtrlRandom();
		tpcRand.Init(randTimeMin, randTimeMax);
	}

	public void CalcBlend(float openValue)
	{
		openRate = openValue;
		CalculateBlendShape();
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
		adjustWidthScale = 1f;
		bool flag = false;
		float num = tpcRand.Calculate(randTimeMin, randTimeMax);
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
			adjustWidthScale = sclNow;
		}
		if (null != objAdjustWidthScale)
		{
			objAdjustWidthScale.transform.localScale = new Vector3(adjustWidthScale, 1f, 1f);
		}
		return true;
	}
}
