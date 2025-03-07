using System;
using System.Collections.Generic;
using System.Linq;
using MorphAssist;
using UnityEngine;

[Serializable]
public class MorphFaceBase : MorphBase
{
	protected int backPtn;

	[Range(0f, 255f)]
	public int NowPtn;

	[Range(0f, 1f)]
	protected float openRate;

	[Range(0f, 1f)]
	public float OpenMin;

	[Range(0f, 1f)]
	public float OpenMax = 1f;

	[Range(-0.1f, 1f)]
	public float FixedRate = -0.1f;

	private float correctOpenMax = -1f;

	public bool BlendNormals;

	protected TimeProgressCtrl blendTimeCtrl;

	public bool Create(GameObject o)
	{
		if (!CreateCalcInfo(o))
		{
			return false;
		}
		blendTimeCtrl = new TimeProgressCtrl();
		return true;
	}

	public bool Init(List<MorphingTargetInfo> MorphTargetList)
	{
		ChangeRefTargetMesh(MorphTargetList);
		blendTimeCtrl = new TimeProgressCtrl();
		return true;
	}

	public void ChangePtn(int ptn, bool blend)
	{
		if (NowPtn != ptn)
		{
			backPtn = NowPtn;
			NowPtn = ptn;
			if (!blend)
			{
				blendTimeCtrl.End();
			}
			else
			{
				blendTimeCtrl.Start();
			}
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

	public void CalculateBlendVertex()
	{
		if (CalcInfo == null)
		{
			return;
		}
		float b = ((!(correctOpenMax < 0f)) ? correctOpenMax : OpenMax);
		float t = Mathf.Lerp(OpenMin, b, openRate);
		if (0f <= FixedRate)
		{
			t = FixedRate;
		}
		float num = 0f;
		if (blendTimeCtrl != null)
		{
			num = blendTimeCtrl.Calculate();
		}
		if (num == 1f)
		{
			MorphCalcInfo[] calcInfo = CalcInfo;
			foreach (MorphCalcInfo morphCalcInfo in calcInfo)
			{
				if (null == morphCalcInfo.TargetMesh || NowPtn * 2 + 1 >= morphCalcInfo.UpdateInfo.Length)
				{
					continue;
				}
				Vector3[] vertices = morphCalcInfo.TargetMesh.vertices;
				foreach (var item in morphCalcInfo.UpdateIndex.Select((int value, int index) => new { value, index }))
				{
					vertices[item.value] = Vector3.Lerp(morphCalcInfo.UpdateInfo[NowPtn * 2].Pos[item.index], morphCalcInfo.UpdateInfo[NowPtn * 2 + 1].Pos[item.index], t);
				}
				morphCalcInfo.TargetMesh.vertices = vertices;
				if (!BlendNormals)
				{
					continue;
				}
				Vector3[] normals = morphCalcInfo.TargetMesh.normals;
				foreach (var item2 in morphCalcInfo.UpdateIndex.Select((int value, int index) => new { value, index }))
				{
					normals[item2.value] = Vector3.Lerp(morphCalcInfo.UpdateInfo[NowPtn * 2].Normmal[item2.index], morphCalcInfo.UpdateInfo[NowPtn * 2 + 1].Normmal[item2.index], t);
				}
				morphCalcInfo.TargetMesh.normals = normals;
			}
			return;
		}
		MorphCalcInfo[] calcInfo2 = CalcInfo;
		foreach (MorphCalcInfo morphCalcInfo2 in calcInfo2)
		{
			if (null == morphCalcInfo2.TargetMesh || NowPtn * 2 + 1 >= morphCalcInfo2.UpdateInfo.Length || backPtn * 2 + 1 >= morphCalcInfo2.UpdateInfo.Length)
			{
				continue;
			}
			Vector3[] vertices2 = morphCalcInfo2.TargetMesh.vertices;
			foreach (var item3 in morphCalcInfo2.UpdateIndex.Select((int value, int index) => new { value, index }))
			{
				Vector3 a = Vector3.Lerp(morphCalcInfo2.UpdateInfo[backPtn * 2].Pos[item3.index], morphCalcInfo2.UpdateInfo[backPtn * 2 + 1].Pos[item3.index], t);
				Vector3 b2 = Vector3.Lerp(morphCalcInfo2.UpdateInfo[NowPtn * 2].Pos[item3.index], morphCalcInfo2.UpdateInfo[NowPtn * 2 + 1].Pos[item3.index], t);
				vertices2[item3.value] = Vector3.Lerp(a, b2, num);
			}
			morphCalcInfo2.TargetMesh.vertices = vertices2;
			if (!BlendNormals)
			{
				continue;
			}
			Vector3[] normals2 = morphCalcInfo2.TargetMesh.normals;
			foreach (var item4 in morphCalcInfo2.UpdateIndex.Select((int value, int index) => new { value, index }))
			{
				Vector3 a2 = Vector3.Lerp(morphCalcInfo2.UpdateInfo[backPtn * 2].Normmal[item4.index], morphCalcInfo2.UpdateInfo[backPtn * 2 + 1].Normmal[item4.index], t);
				Vector3 b3 = Vector3.Lerp(morphCalcInfo2.UpdateInfo[NowPtn * 2].Normmal[item4.index], morphCalcInfo2.UpdateInfo[NowPtn * 2 + 1].Normmal[item4.index], t);
				normals2[item4.value] = Vector3.Lerp(a2, b3, num);
			}
			morphCalcInfo2.TargetMesh.normals = normals2;
		}
	}
}
