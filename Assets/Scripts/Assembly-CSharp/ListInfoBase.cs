using System;
using System.Collections.Generic;
using Illusion;
using Illusion.Extensions;

[Serializable]
public class ListInfoBase
{
	public Dictionary<int, string> dictInfo = new Dictionary<int, string>();

	public int Category
	{
		get
		{
			return GetInfoInt(ChaListDefine.KeyType.Category);
		}
	}

	public int Distribution
	{
		get
		{
			return GetInfoInt(ChaListDefine.KeyType.DistributionNo);
		}
	}

	public int Id
	{
		get
		{
			return GetInfoInt(ChaListDefine.KeyType.ID);
		}
	}

	public int Kind
	{
		get
		{
			return GetInfoInt(ChaListDefine.KeyType.Kind);
		}
	}

	public string Name
	{
		get
		{
			return GetInfo(ChaListDefine.KeyType.Name);
		}
	}

	public bool Set(int _cateNo, int _distNo, List<string> lstKey, List<string> lstData)
	{
		string[] names = Utils.Enum<ChaListDefine.KeyType>.Names;
		int key = names.Check("Category");
		dictInfo[key] = _cateNo.ToString();
		int key2 = names.Check("DistributionNo");
		dictInfo[key2] = _distNo.ToString();
		for (int i = 0; i < lstKey.Count; i++)
		{
			int key3 = names.Check(lstKey[i]);
			dictInfo[key3] = lstData[i];
		}
		return true;
	}

	public int GetInfoInt(ChaListDefine.KeyType keyType)
	{
		string info = GetInfo(keyType);
		int result;
		if (!int.TryParse(info, out result))
		{
			return -1;
		}
		return result;
	}

	public float GetInfoFloat(ChaListDefine.KeyType keyType)
	{
		string info = GetInfo(keyType);
		float result;
		if (!float.TryParse(info, out result))
		{
			return -1f;
		}
		return result;
	}

	public string GetInfo(ChaListDefine.KeyType keyType)
	{
		string value;
		if (!dictInfo.TryGetValue((int)keyType, out value))
		{
			return "0";
		}
		return value;
	}

	public ListInfoBase Clone()
	{
		ListInfoBase listInfoBase = (ListInfoBase)MemberwiseClone();
		listInfoBase.dictInfo = new Dictionary<int, string>(dictInfo);
		return listInfoBase;
	}
}
