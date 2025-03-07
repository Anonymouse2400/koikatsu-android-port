using System;
using System.Collections.Generic;
using Manager;
using UnityEngine;

public class BlendUV
{
	private SkinnedMeshRenderer[] smrTarg;

	private List<Vector2>[] uv = new List<Vector2>[3];

	private int[] rangeIndex;

	private byte uvSetNo;

	private Vector2[][] calc;

	private bool initEnd;

	private float blendRate = -1f;

	public bool Init(string assetBundleName, string assetName, string manifestName, byte _uvSetNo, params GameObject[] targets)
	{
		initEnd = false;
		if (targets == null || targets.Length == 0)
		{
			return false;
		}
		smrTarg = new SkinnedMeshRenderer[targets.Length];
		for (int i = 0; i < targets.Length; i++)
		{
			smrTarg[i] = targets[i].GetComponent<SkinnedMeshRenderer>();
			if ((bool)smrTarg[i] && (bool)smrTarg[i].sharedMesh)
			{
				Mesh mesh = UnityEngine.Object.Instantiate(smrTarg[i].sharedMesh);
				mesh.name = smrTarg[i].sharedMesh.name;
				smrTarg[i].sharedMesh = mesh;
			}
		}
		UVData uVData = CommonLib.LoadAsset<UVData>(assetBundleName, assetName, true, manifestName);
		if (null == uVData)
		{
			return false;
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, manifestName);
		if (uVData.data.Count != 3)
		{
			UnityEngine.Object.Destroy(uVData);
			uVData = null;
			return false;
		}
		uvSetNo = _uvSetNo;
		for (int j = 0; j < uVData.data.Count; j++)
		{
			uv[j] = new List<Vector2>();
			for (int k = 0; k < uVData.data[j].UV.Count; k++)
			{
				uv[j].Add(uVData.data[j].UV[k]);
			}
		}
		if (uVData.rangeIndex != null && uVData.rangeIndex.Length != 0)
		{
			rangeIndex = new int[uVData.rangeIndex.Length];
			Array.Copy(uVData.rangeIndex, rangeIndex, rangeIndex.Length);
		}
		UnityEngine.Object.Destroy(uVData);
		for (int l = 0; l < smrTarg.Length; l++)
		{
			if (uvSetNo == 0)
			{
				if (smrTarg[l].sharedMesh.uv.Length != uv[0].Count)
				{
					return false;
				}
			}
			else if (uvSetNo == 1)
			{
				if (smrTarg[l].sharedMesh.uv2.Length != uv[0].Count)
				{
					return false;
				}
			}
			else if (uvSetNo == 2)
			{
				if (smrTarg[l].sharedMesh.uv3.Length != uv[0].Count)
				{
					return false;
				}
			}
			else if (uvSetNo == 3 && smrTarg[l].sharedMesh.uv4.Length != uv[0].Count)
			{
				return false;
			}
		}
		calc = new Vector2[targets.Length][];
		for (int m = 0; m < targets.Length; m++)
		{
			calc[m] = new Vector2[uv[0].Count];
			if (uvSetNo == 0)
			{
				Array.Copy(smrTarg[m].sharedMesh.uv, calc[m], calc[m].Length);
			}
			else if (uvSetNo == 1)
			{
				Array.Copy(smrTarg[m].sharedMesh.uv2, calc[m], calc[m].Length);
			}
			else if (uvSetNo == 2)
			{
				Array.Copy(smrTarg[m].sharedMesh.uv3, calc[m], calc[m].Length);
			}
			else if (uvSetNo == 3)
			{
				Array.Copy(smrTarg[m].sharedMesh.uv4, calc[m], calc[m].Length);
			}
		}
		initEnd = true;
		return true;
	}

	public void Blend(int idx, float rate)
	{
		if (!initEnd || idx >= smrTarg.Length || blendRate == rate)
		{
			return;
		}
		int num = 0;
		int num2 = 1;
		float num3 = 0f;
		if (rate < 0.5f)
		{
			num = 0;
			num2 = 1;
			num3 = Mathf.InverseLerp(0f, 0.5f, rate);
		}
		else
		{
			num = 1;
			num2 = 2;
			num3 = Mathf.InverseLerp(0.5f, 1f, rate);
		}
		if (rangeIndex != null && rangeIndex.Length != 0)
		{
			for (int i = 0; i < rangeIndex.Length; i++)
			{
				int num4 = rangeIndex[i];
				calc[idx][num4] = Vector2.Lerp(uv[num][num4], uv[num2][num4], num3);
			}
		}
		else
		{
			for (int j = 0; j < calc[idx].Length; j++)
			{
				calc[idx][j] = Vector2.Lerp(uv[num][j], uv[num2][j], num3);
			}
		}
		if ((bool)smrTarg[idx])
		{
			if (uvSetNo == 0)
			{
				smrTarg[idx].sharedMesh.uv = calc[idx];
			}
			else if (uvSetNo == 1)
			{
				smrTarg[idx].sharedMesh.uv2 = calc[idx];
			}
			else if (uvSetNo == 2)
			{
				smrTarg[idx].sharedMesh.uv3 = calc[idx];
			}
			else if (uvSetNo == 3)
			{
				smrTarg[idx].sharedMesh.uv4 = calc[idx];
			}
		}
		blendRate = rate;
	}
}
