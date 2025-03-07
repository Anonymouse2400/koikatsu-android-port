using System;
using System.Collections.Generic;
using FBSAssist;
using UnityEngine;

[Serializable]
public class FBSBase
{
	public FBSTargetInfo[] FBSTarget;

	protected Dictionary<int, float> dictBackFace = new Dictionary<int, float>();

	protected Dictionary<int, float> dictNowFace = new Dictionary<int, float>();

	protected float openRate;

	[Range(0f, 1f)]
	public float OpenMin;

	[Range(0f, 1f)]
	public float OpenMax = 1f;

	[Range(-0.1f, 1f)]
	public float FixedRate = -0.1f;

	private float correctOpenMax = -1f;

	protected TimeProgressCtrl blendTimeCtrl;

	public bool Init()
	{
		blendTimeCtrl = new TimeProgressCtrl();
		blendTimeCtrl.End();
		for (int i = 0; i < FBSTarget.Length; i++)
		{
			FBSTarget[i].SetSkinnedMeshRenderer();
		}
		dictBackFace.Clear();
		dictBackFace[0] = 1f;
		dictNowFace.Clear();
		dictNowFace[0] = 1f;
		return true;
	}

	public void SetOpenRateForce(float rate)
	{
		openRate = rate;
	}

	public int GetMaxPtn()
	{
		if (FBSTarget.Length == 0)
		{
			return 0;
		}
		return FBSTarget[0].PtnSet.Length;
	}

	public void ChangePtn(int ptn, bool blend)
	{
		if (GetMaxPtn() > ptn && (dictNowFace.Count != 1 || !dictNowFace.ContainsKey(ptn) || dictNowFace[ptn] != 1f))
		{
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			dictionary[ptn] = 1f;
			ChangeFace(dictionary, blend);
		}
	}

	public void ChangeFace(Dictionary<int, float> dictFace, bool blend)
	{
		bool flag = false;
		byte b = 0;
		float num = 0f;
		FBSTargetInfo[] fBSTarget = FBSTarget;
		foreach (FBSTargetInfo fBSTargetInfo in fBSTarget)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = fBSTargetInfo.GetSkinnedMeshRenderer();
			foreach (int key in dictFace.Keys)
			{
				if (skinnedMeshRenderer.sharedMesh.blendShapeCount <= fBSTargetInfo.PtnSet[key].Close)
				{
					b = 1;
					break;
				}
				if (skinnedMeshRenderer.sharedMesh.blendShapeCount <= fBSTargetInfo.PtnSet[key].Open)
				{
					b = 1;
					break;
				}
				num += dictFace[key];
			}
			if (b != 0)
			{
				break;
			}
			if (!flag && num > 1f)
			{
				b = 2;
				break;
			}
			flag = true;
		}
		if (b == 1 || b == 2)
		{
			return;
		}
		dictBackFace.Clear();
		foreach (int key2 in dictNowFace.Keys)
		{
			dictBackFace[key2] = dictNowFace[key2];
		}
		dictNowFace.Clear();
		foreach (int key3 in dictFace.Keys)
		{
			dictNowFace[key3] = dictFace[key3];
		}
		if (!blend)
		{
			blendTimeCtrl.End();
		}
		else
		{
			blendTimeCtrl.Start();
		}
	}

	public void SetFixedRate(float value)
	{
		FixedRate = value;
	}

	public void SetCorrectOpenMax(float value)
	{
		correctOpenMax = value;
	}

	public void CalculateBlendShape()
	{
		if (FBSTarget.Length == 0)
		{
			return;
		}
		float b = ((!(correctOpenMax < 0f)) ? correctOpenMax : OpenMax);
		float num = Mathf.Lerp(OpenMin, b, openRate);
		if (0f <= FixedRate)
		{
			num = FixedRate;
		}
		float num2 = 0f;
		if (blendTimeCtrl != null)
		{
			num2 = blendTimeCtrl.Calculate();
		}
		FBSTargetInfo[] fBSTarget = FBSTarget;
		foreach (FBSTargetInfo fBSTargetInfo in fBSTarget)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = fBSTargetInfo.GetSkinnedMeshRenderer();
			Dictionary<int, float> dictionary = new Dictionary<int, float>();
			for (int j = 0; j < fBSTargetInfo.PtnSet.Length; j++)
			{
				dictionary[fBSTargetInfo.PtnSet[j].Close] = 0f;
				dictionary[fBSTargetInfo.PtnSet[j].Open] = 0f;
			}
			int num3 = (int)Mathf.Clamp(num * 100f, 0f, 100f);
			if (num2 != 1f)
			{
				foreach (int key in dictBackFace.Keys)
				{
					dictionary[fBSTargetInfo.PtnSet[key].Close] = dictionary[fBSTargetInfo.PtnSet[key].Close] + dictBackFace[key] * (float)(100 - num3) * (1f - num2);
					dictionary[fBSTargetInfo.PtnSet[key].Open] = dictionary[fBSTargetInfo.PtnSet[key].Open] + dictBackFace[key] * (float)num3 * (1f - num2);
				}
			}
			foreach (int key2 in dictNowFace.Keys)
			{
				dictionary[fBSTargetInfo.PtnSet[key2].Close] = dictionary[fBSTargetInfo.PtnSet[key2].Close] + dictNowFace[key2] * (float)(100 - num3) * num2;
				dictionary[fBSTargetInfo.PtnSet[key2].Open] = dictionary[fBSTargetInfo.PtnSet[key2].Open] + dictNowFace[key2] * (float)num3 * num2;
			}
			foreach (KeyValuePair<int, float> item in dictionary)
			{
				if (item.Key != -1)
				{
					skinnedMeshRenderer.SetBlendShapeWeight(item.Key, item.Value);
				}
			}
		}
	}
}
