using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class MorphBase
{
	public const int morphFilesVersion = 100;

	public MorphCalcInfo[] CalcInfo;

	public int GetMaxPtn()
	{
		if (CalcInfo.Length == 0)
		{
			return 0;
		}
		return CalcInfo[0].UpdateInfo.Length / 2;
	}

	protected bool CreateCalcInfo(GameObject obj)
	{
		if (null == obj)
		{
			return false;
		}
		MorphSetting morphSetting = (MorphSetting)obj.GetComponent("MorphSetting");
		if (null == morphSetting)
		{
			return false;
		}
		CalcInfo = null;
		GC.Collect();
		CalcInfo = new MorphCalcInfo[morphSetting.MorphDataList.Count];
		int num = 0;
		foreach (MorphData morphData in morphSetting.MorphDataList)
		{
			if (null == morphData.TargetObj)
			{
				continue;
			}
			CalcInfo[num] = new MorphCalcInfo();
			CalcInfo[num].TargetObj = morphData.TargetObj;
			MeshFilter meshFilter = new MeshFilter();
			meshFilter = morphData.TargetObj.GetComponent(typeof(MeshFilter)) as MeshFilter;
			if ((bool)meshFilter)
			{
				CalcInfo[num].OriginalMesh = meshFilter.sharedMesh;
				CalcInfo[num].OriginalPos = meshFilter.sharedMesh.vertices;
				CalcInfo[num].OriginalNormal = meshFilter.sharedMesh.normals;
				CalcInfo[num].WeightFlags = false;
			}
			else
			{
				SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
				skinnedMeshRenderer = morphData.TargetObj.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				CalcInfo[num].OriginalMesh = skinnedMeshRenderer.sharedMesh;
				CalcInfo[num].OriginalPos = skinnedMeshRenderer.sharedMesh.vertices;
				CalcInfo[num].OriginalNormal = skinnedMeshRenderer.sharedMesh.normals;
				CalcInfo[num].WeightFlags = true;
			}
			int num2 = 0;
			if (null == morphData.MorphArea)
			{
				num2 = CalcInfo[num].OriginalMesh.vertices.Length;
				CalcInfo[num].UpdateIndex = new int[num2];
				for (int i = 0; i < num2; i++)
				{
					CalcInfo[num].UpdateIndex[i] = i;
				}
			}
			else if (morphData.MorphArea.colors.Length != 0)
			{
				List<int> list = new List<int>();
				foreach (var item in morphData.MorphArea.colors.Select((Color value, int index) => new { value, index }))
				{
					if (item.value == morphData.AreaColor)
					{
						list.Add(item.index);
					}
				}
				CalcInfo[num].UpdateIndex = new int[list.Count];
				foreach (var item2 in list.Select((int value, int index) => new { value, index }))
				{
					CalcInfo[num].UpdateIndex[item2.index] = item2.value;
				}
				num2 = list.Count;
			}
			else
			{
				num2 = CalcInfo[num].OriginalMesh.vertices.Length;
				CalcInfo[num].UpdateIndex = new int[num2];
				for (int j = 0; j < num2; j++)
				{
					CalcInfo[num].UpdateIndex[j] = j;
				}
			}
			int num3 = morphData.MorphMesh.Length;
			CalcInfo[num].UpdateInfo = new MorphUpdateInfo[num3];
			for (int k = 0; k < num3; k++)
			{
				CalcInfo[num].UpdateInfo[k] = new MorphUpdateInfo();
				CalcInfo[num].UpdateInfo[k].Pos = new Vector3[num2];
				CalcInfo[num].UpdateInfo[k].Normmal = new Vector3[num2];
				if (null == morphData.MorphMesh[k])
				{
					for (int l = 0; l < num2; l++)
					{
						CalcInfo[num].UpdateInfo[k].Pos[l] = CalcInfo[num].OriginalMesh.vertices[CalcInfo[num].UpdateIndex[l]];
						CalcInfo[num].UpdateInfo[k].Normmal[l] = CalcInfo[num].OriginalMesh.normals[CalcInfo[num].UpdateIndex[l]];
					}
				}
				else
				{
					for (int m = 0; m < num2; m++)
					{
						CalcInfo[num].UpdateInfo[k].Pos[m] = morphData.MorphMesh[k].vertices[CalcInfo[num].UpdateIndex[m]];
						CalcInfo[num].UpdateInfo[k].Normmal[m] = morphData.MorphMesh[k].normals[CalcInfo[num].UpdateIndex[m]];
					}
				}
			}
			num++;
		}
		return true;
	}

	protected bool ChangeRefTargetMesh(List<MorphingTargetInfo> MorphTargetList)
	{
		MorphCalcInfo[] calcInfo = CalcInfo;
		foreach (MorphCalcInfo morphCalcInfo in calcInfo)
		{
			if (null == morphCalcInfo.OriginalMesh)
			{
				continue;
			}
			Mesh mesh = null;
			foreach (MorphingTargetInfo MorphTarget in MorphTargetList)
			{
				if (MorphTarget.TargetObj == morphCalcInfo.TargetObj)
				{
					mesh = MorphTarget.TargetMesh;
					break;
				}
			}
			if ((bool)mesh)
			{
				morphCalcInfo.TargetMesh = mesh;
			}
			else
			{
				MorphCloneMesh.Clone(out morphCalcInfo.TargetMesh, morphCalcInfo.OriginalMesh);
				morphCalcInfo.TargetMesh.name = morphCalcInfo.OriginalMesh.name;
				MorphingTargetInfo morphingTargetInfo = new MorphingTargetInfo();
				morphingTargetInfo.TargetMesh = morphCalcInfo.TargetMesh;
				morphingTargetInfo.TargetObj = morphCalcInfo.TargetObj;
				MorphTargetList.Add(morphingTargetInfo);
			}
			if (morphCalcInfo.WeightFlags)
			{
				SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
				skinnedMeshRenderer = morphCalcInfo.TargetObj.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				skinnedMeshRenderer.sharedMesh = morphCalcInfo.TargetMesh;
			}
			else
			{
				MeshFilter meshFilter = new MeshFilter();
				meshFilter = morphCalcInfo.TargetObj.GetComponent(typeof(MeshFilter)) as MeshFilter;
				meshFilter.sharedMesh = morphCalcInfo.TargetMesh;
			}
		}
		return true;
	}
}
