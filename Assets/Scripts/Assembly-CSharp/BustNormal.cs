using System.Collections.Generic;
using IllusionUtility.GetUtility;
using Manager;
using UnityEngine;

public class BustNormal
{
	private bool initEnd;

	private NormalData normal;

	private Dictionary<GameObject, NormalData.Param> dictNormal = new Dictionary<GameObject, NormalData.Param>();

	private Dictionary<GameObject, SkinnedMeshRenderer> dictSmr = new Dictionary<GameObject, SkinnedMeshRenderer>();

	private Dictionary<GameObject, Vector3[]> dictCalc = new Dictionary<GameObject, Vector3[]>();

	public bool Init(GameObject objTarg, string assetBundleName, string assetName, string manifest)
	{
		initEnd = false;
		normal = CommonLib.LoadAsset<NormalData>(assetBundleName, assetName, true, manifest);
		if (null == normal)
		{
			return false;
		}
		Singleton<Character>.Instance.AddLoadAssetBundle(assetBundleName, string.Empty);
		dictNormal.Clear();
		dictSmr.Clear();
		for (int i = 0; i < normal.data.Count; i++)
		{
			GameObject gameObject = objTarg.transform.FindLoop(normal.data[i].ObjectName);
			if (null == gameObject)
			{
				continue;
			}
			SkinnedMeshRenderer component = gameObject.GetComponent<SkinnedMeshRenderer>();
			if (!(null == component))
			{
				dictSmr[gameObject] = component;
				if ((bool)dictSmr[gameObject] && (bool)dictSmr[gameObject].sharedMesh)
				{
					Mesh mesh = Object.Instantiate(dictSmr[gameObject].sharedMesh);
					mesh.name = dictSmr[gameObject].sharedMesh.name;
					dictSmr[gameObject].sharedMesh = mesh;
				}
				dictNormal[gameObject] = normal.data[i];
				Vector3[] value = new Vector3[normal.data[i].NormalMin.Count];
				dictCalc[gameObject] = value;
			}
		}
		CheckNormals(assetName);
		if (dictNormal.Count != 0)
		{
			initEnd = true;
		}
		return initEnd;
	}

	public void Release()
	{
		initEnd = false;
		normal = null;
		dictNormal.Clear();
	}

	private void CheckNormals(string assetName)
	{
		if (dictNormal == null)
		{
			return;
		}
		foreach (KeyValuePair<GameObject, NormalData.Param> item in dictNormal)
		{
			if (item.Value.NormalMin.Count == item.Value.NormalMax.Count)
			{
			}
		}
	}

	public void Blend(float rate)
	{
		if (!initEnd)
		{
			return;
		}
		foreach (KeyValuePair<GameObject, NormalData.Param> item in dictNormal)
		{
			if (item.Value.NormalMin.Count == item.Value.NormalMax.Count && item.Value.NormalMin.Count == dictSmr[item.Key].sharedMesh.normals.Length)
			{
				for (int i = 0; i < item.Value.NormalMin.Count; i++)
				{
					dictCalc[item.Key][i] = Vector3.Lerp(item.Value.NormalMin[i], item.Value.NormalMax[i], rate);
				}
				dictSmr[item.Key].sharedMesh.normals = dictCalc[item.Key];
			}
		}
	}
}
