using System;
using System.Collections.Generic;
using IllusionUtility.GetUtility;
using UnityEngine;

public abstract class ShapeInfoBase
{
	public class CategoryInfo
	{
		public int id;

		public string name = string.Empty;

		public bool[][] use = new bool[3][];

		public bool[] getflag = new bool[3];

		public void Initialize()
		{
			for (int i = 0; i < 3; i++)
			{
				use[i] = new bool[3];
				getflag[i] = false;
			}
		}
	}

	public class BoneInfo
	{
		public Transform trfBone;

		public Vector3 vctPos = Vector3.zero;

		public Vector3 vctRot = Vector3.zero;

		public Vector3 vctScl = Vector3.one;
	}

	private Dictionary<int, List<CategoryInfo>> dictCategory;

	protected Dictionary<int, BoneInfo> dictDst;

	protected Dictionary<int, BoneInfo> dictSrc;

	private AnimationKeyInfo anmKeyInfo = new AnimationKeyInfo();

	public bool InitEnd { get; protected set; }

	public ShapeInfoBase()
	{
		InitEnd = false;
		dictCategory = new Dictionary<int, List<CategoryInfo>>();
		dictDst = new Dictionary<int, BoneInfo>();
		dictSrc = new Dictionary<int, BoneInfo>();
	}

	public abstract void InitShapeInfo(string assetBundleAnmKey, string assetBundleCategory, string anmKeyInfoName, string cateInfoName, Transform trfObj);

	protected void InitShapeInfoBase(string assetBundleAnmKey, string assetBundleCategory, string anmKeyInfoName, string cateInfoName, Transform trfObj, Dictionary<string, int> dictEnumDst, Dictionary<string, int> dictEnumSrc, Action<string, string> funcAssetBundleEntry = null)
	{
		anmKeyInfo.LoadInfo(assetBundleAnmKey, anmKeyInfoName, funcAssetBundleEntry);
		LoadCategoryInfoList(assetBundleCategory, cateInfoName, dictEnumSrc);
		GetDstBoneInfo(trfObj, dictEnumDst);
		GetSrcBoneInfo();
	}

	public void ReleaseShapeInfo()
	{
		InitEnd = false;
		dictCategory.Clear();
		dictDst.Clear();
		dictSrc.Clear();
	}

	private bool LoadCategoryInfoList(string assetBundleName, string assetName, Dictionary<string, int> dictEnumSrc)
	{
		if (!AssetBundleCheck.IsFile(assetBundleName, assetName))
		{
			string text = "読み込みエラー\r\nassetBundleName：" + assetBundleName + "\tassetName：" + assetName;
			return false;
		}
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(TextAsset));
		if (assetBundleLoadAssetOperation == null)
		{
			string text2 = "読み込みエラー\r\nassetName：" + assetName;
			return false;
		}
		TextAsset asset = assetBundleLoadAssetOperation.GetAsset<TextAsset>();
		if (null == asset)
		{
			return false;
		}
		string[,] data;
		YS_Assist.GetListString(asset.text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		dictCategory.Clear();
		if (length != 0 && length2 != 0)
		{
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				CategoryInfo categoryInfo = new CategoryInfo();
				categoryInfo.Initialize();
				num = int.Parse(data[i, 0]);
				categoryInfo.name = data[i, 1];
				int value = 0;
				if (!dictEnumSrc.TryGetValue(categoryInfo.name, out value))
				{
					string text3 = "SrcBone【" + categoryInfo.name + "】のIDが見つかりません";
					continue;
				}
				categoryInfo.id = value;
				categoryInfo.use[0][0] = ((!(data[i, 2] == "0")) ? true : false);
				categoryInfo.use[0][1] = ((!(data[i, 3] == "0")) ? true : false);
				categoryInfo.use[0][2] = ((!(data[i, 4] == "0")) ? true : false);
				if (categoryInfo.use[0][0] || categoryInfo.use[0][1] || categoryInfo.use[0][2])
				{
					categoryInfo.getflag[0] = true;
				}
				categoryInfo.use[1][0] = ((!(data[i, 5] == "0")) ? true : false);
				categoryInfo.use[1][1] = ((!(data[i, 6] == "0")) ? true : false);
				categoryInfo.use[1][2] = ((!(data[i, 7] == "0")) ? true : false);
				if (categoryInfo.use[1][0] || categoryInfo.use[1][1] || categoryInfo.use[1][2])
				{
					categoryInfo.getflag[1] = true;
				}
				categoryInfo.use[2][0] = ((!(data[i, 8] == "0")) ? true : false);
				categoryInfo.use[2][1] = ((!(data[i, 9] == "0")) ? true : false);
				categoryInfo.use[2][2] = ((!(data[i, 10] == "0")) ? true : false);
				if (categoryInfo.use[2][0] || categoryInfo.use[2][1] || categoryInfo.use[2][2])
				{
					categoryInfo.getflag[2] = true;
				}
				List<CategoryInfo> value2 = null;
				if (!dictCategory.TryGetValue(num, out value2))
				{
					value2 = new List<CategoryInfo>();
					dictCategory[num] = value2;
				}
				value2.Add(categoryInfo);
			}
		}
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		return true;
	}

	private bool GetDstBoneInfo(Transform trfBone, Dictionary<string, int> dictEnumDst)
	{
		dictDst.Clear();
		foreach (KeyValuePair<string, int> item in dictEnumDst)
		{
			GameObject gameObject = trfBone.FindLoop(item.Key);
			if (null != gameObject)
			{
				BoneInfo boneInfo = new BoneInfo();
				boneInfo.trfBone = gameObject.transform;
				dictDst[item.Value] = boneInfo;
			}
		}
		return true;
	}

	private void GetSrcBoneInfo()
	{
		dictSrc.Clear();
		foreach (KeyValuePair<int, List<CategoryInfo>> item in dictCategory)
		{
			int count = item.Value.Count;
			for (int i = 0; i < count; i++)
			{
				BoneInfo value = null;
				if (!dictSrc.TryGetValue(item.Value[i].id, out value))
				{
					value = new BoneInfo();
					dictSrc[item.Value[i].id] = value;
				}
			}
		}
	}

	public bool ChangeValue(int category, float value)
	{
		if (anmKeyInfo == null)
		{
			return false;
		}
		if (!dictCategory.ContainsKey(category))
		{
			return false;
		}
		int count = dictCategory[category].Count;
		string empty = string.Empty;
		int num = 0;
		for (int i = 0; i < count; i++)
		{
			List<CategoryInfo> list = dictCategory[category];
			BoneInfo value2 = null;
			num = list[i].id;
			empty = list[i].name;
			if (dictSrc.TryGetValue(num, out value2))
			{
				Vector3[] value3 = new Vector3[3];
				for (int j = 0; j < 3; j++)
				{
					value3[j] = Vector3.zero;
				}
				bool[] array = new bool[3];
				for (int k = 0; k < 3; k++)
				{
					array[k] = list[i].getflag[k];
				}
				anmKeyInfo.GetInfo(empty, value, ref value3, array);
				if (list[i].use[0][0])
				{
					value2.vctPos.x = value3[0].x;
				}
				if (list[i].use[0][1])
				{
					value2.vctPos.y = value3[0].y;
				}
				if (list[i].use[0][2])
				{
					value2.vctPos.z = value3[0].z;
				}
				if (list[i].use[1][0])
				{
					value2.vctRot.x = value3[1].x;
				}
				if (list[i].use[1][1])
				{
					value2.vctRot.y = value3[1].y;
				}
				if (list[i].use[1][2])
				{
					value2.vctRot.z = value3[1].z;
				}
				if (list[i].use[2][0])
				{
					value2.vctScl.x = value3[2].x;
				}
				if (list[i].use[2][1])
				{
					value2.vctScl.y = value3[2].y;
				}
				if (list[i].use[2][2])
				{
					value2.vctScl.z = value3[2].z;
				}
			}
		}
		return true;
	}

	public abstract void Update();

	public abstract void UpdateAlways();
}
