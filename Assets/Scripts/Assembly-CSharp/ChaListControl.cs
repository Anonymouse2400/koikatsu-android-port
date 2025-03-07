using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using UnityEngine;

public class ChaListControl
{
	private Dictionary<ChaListDefine.CategoryNo, Dictionary<int, ListInfoBase>> dictListInfo = new Dictionary<ChaListDefine.CategoryNo, Dictionary<int, ListInfoBase>>();

	private const string strSave = "save/checkitem.dat";

	public Version version = ChaListDefine.CheckItemVersion;

	public Dictionary<int, byte> itemIDInfo = new Dictionary<int, byte>();

	private List<int> lstItemIsInit = new List<int>();

	private List<int> lstItemIsNew = new List<int>();

	public ChaListControl()
	{
		ChaListDefine.CategoryNo[] array = (ChaListDefine.CategoryNo[])Enum.GetValues(typeof(ChaListDefine.CategoryNo));
		ChaListDefine.CategoryNo[] array2 = array;
		foreach (ChaListDefine.CategoryNo key in array2)
		{
			dictListInfo[key] = new Dictionary<int, ListInfoBase>();
		}
	}

	public Dictionary<int, ListInfoBase> GetCategoryInfo(ChaListDefine.CategoryNo type)
	{
		Dictionary<int, ListInfoBase> value = null;
		if (!dictListInfo.TryGetValue(type, out value))
		{
			return null;
		}
		return new Dictionary<int, ListInfoBase>(value);
	}

	public ListInfoBase GetListInfo(ChaListDefine.CategoryNo type, int id)
	{
		Dictionary<int, ListInfoBase> value = null;
		if (!dictListInfo.TryGetValue(type, out value))
		{
			return null;
		}
		ListInfoBase value2 = null;
		if (!value.TryGetValue(id, out value2))
		{
			return null;
		}
		return value2.Clone();
	}

	public bool LoadListInfoAll()
	{
		ChaListDefine.CategoryNo[] array = (ChaListDefine.CategoryNo[])Enum.GetValues(typeof(ChaListDefine.CategoryNo));
		Dictionary<int, ListInfoBase> value = null;
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("list/characustom/");
		for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
		{
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(assetBundleNameListFromPath[i], typeof(TextAsset));
			if (assetBundleLoadAssetOperation == null)
			{
				string text = "読み込みエラー\r\nassetBundleName：" + assetBundleNameListFromPath[i];
				continue;
			}
			if (assetBundleLoadAssetOperation.IsEmpty())
			{
				AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
				continue;
			}
			TextAsset[] allAssets = assetBundleLoadAssetOperation.GetAllAssets<TextAsset>();
			if (allAssets == null || allAssets.Length == 0)
			{
				AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
				continue;
			}
			ChaListDefine.CategoryNo[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				ChaListDefine.CategoryNo key = array2[j];
				if (!dictListInfo.TryGetValue(key, out value))
				{
					continue;
				}
				TextAsset[] array3 = allAssets;
				foreach (TextAsset textAsset in array3)
				{
					string removeStringRight = YS_Assist.GetRemoveStringRight(textAsset.name, "_");
					if (!(removeStringRight != key.ToString() + "_"))
					{
						LoadListInfo(value, textAsset);
					}
				}
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleNameListFromPath[i], true);
		}
		EntryClothesIsInit();
		LoadItemID();
		Resources.UnloadUnusedAssets();
		GC.Collect();
		return true;
	}

	private bool LoadListInfo(Dictionary<int, ListInfoBase> dictData, string assetBundleName, string assetName)
	{
		TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
		if (null == textAsset)
		{
			return false;
		}
		bool result = LoadListInfo(dictData, textAsset);
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		return result;
	}

	private bool LoadListInfo(Dictionary<int, ListInfoBase> dictData, TextAsset ta)
	{
		if (null == ta)
		{
			return false;
		}
		ChaListData chaListData = MessagePackSerializer.Deserialize<ChaListData>(ta.bytes);
		if (chaListData == null)
		{
			return false;
		}
		foreach (KeyValuePair<int, List<string>> dict in chaListData.dictList)
		{
			ListInfoBase listInfoBase = new ListInfoBase();
			if (listInfoBase.Set(chaListData.categoryNo, chaListData.distributionNo, chaListData.lstKey, dict.Value) && !dictData.ContainsKey(listInfoBase.Id))
			{
				dictData[listInfoBase.Id] = listInfoBase;
				int infoInt = listInfoBase.GetInfoInt(ChaListDefine.KeyType.Possess);
				int item = listInfoBase.Category * 1000 + listInfoBase.Id;
				switch (infoInt)
				{
				case 1:
					lstItemIsInit.Add(item);
					break;
				case 2:
					lstItemIsNew.Add(item);
					break;
				}
			}
		}
		return true;
	}

	public static List<ExcelData.Param> LoadExcelData(string assetBunndlePath, string assetName, int cellS, int rowS)
	{
		if (!AssetBundleCheck.IsFile(assetBunndlePath, assetName))
		{
			return null;
		}
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBunndlePath, assetName, typeof(ExcelData));
		AssetBundleManager.UnloadAssetBundle(assetBunndlePath, true);
		if (assetBundleLoadAssetOperation.IsEmpty())
		{
			return null;
		}
		ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
		int num = asset.MaxCell - 1;
		int row = asset.list[num].list.Count - 1;
		return asset.Get(new ExcelData.Specify(cellS, rowS), new ExcelData.Specify(num, row));
	}

	public void EntryClothesIsInit()
	{
		for (int i = 0; i < lstItemIsInit.Count; i++)
		{
			AddItemID(lstItemIsInit[i], 2);
		}
		for (int j = 0; j < lstItemIsNew.Count; j++)
		{
			AddItemID(lstItemIsNew[j], 1);
		}
		lstItemIsInit.Clear();
		lstItemIsInit.TrimExcess();
		lstItemIsNew.Clear();
		lstItemIsNew.TrimExcess();
	}

	public void SaveItemID()
	{
		string path = UserData.Path + "save/checkitem.dat";
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(itemIDInfo.Count);
				foreach (KeyValuePair<int, byte> item in itemIDInfo)
				{
					binaryWriter.Write(item.Key);
					binaryWriter.Write(item.Value);
				}
			}
		}
	}

	public void LoadItemID()
	{
		string path = UserData.Path + "save/checkitem.dat";
		if (!File.Exists(path))
		{
			return;
		}
		using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				int num = binaryReader.ReadInt32();
				for (int i = 0; i < num; i++)
				{
					int key = binaryReader.ReadInt32();
					byte b = binaryReader.ReadByte();
					byte value = 0;
					if (itemIDInfo.TryGetValue(key, out value))
					{
						if (value < b)
						{
							itemIDInfo[key] = b;
						}
					}
					else
					{
						itemIDInfo.Add(key, b);
					}
				}
			}
		}
	}

	public void AddItemID(string IDStr, byte flags = 1)
	{
		string[] array = IDStr.Split('/');
		string[] array2 = array;
		foreach (string s in array2)
		{
			int key = int.Parse(s);
			byte value = 0;
			if (itemIDInfo.TryGetValue(key, out value))
			{
				if (value < flags)
				{
					itemIDInfo[key] = flags;
				}
			}
			else
			{
				itemIDInfo.Add(key, flags);
			}
		}
	}

	public void AddItemID(int pid, byte flags = 1)
	{
		byte value = 0;
		if (itemIDInfo.TryGetValue(pid, out value))
		{
			if (value < flags)
			{
				itemIDInfo[pid] = flags;
			}
		}
		else
		{
			itemIDInfo.Add(pid, flags);
		}
	}

	public void AddItemID(int category, int id, byte flags)
	{
		int pid = category * 1000 + id;
		AddItemID(pid, flags);
	}

	public byte CheckItemID(int pid)
	{
		byte value = 0;
		if (itemIDInfo.TryGetValue(pid, out value))
		{
			return value;
		}
		return value;
	}

	public byte CheckItemID(int category, int id)
	{
		int pid = category * 1000 + id;
		return CheckItemID(pid);
	}
}
