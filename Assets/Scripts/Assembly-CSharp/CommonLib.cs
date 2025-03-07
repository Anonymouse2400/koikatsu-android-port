using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CommonLib
{
	public static List<string> GetAssetBundleNameListFromPath(string path, bool subdirCheck = false)
	{
		List<string> result = new List<string>();
		if (!AssetBundleCheck.IsSimulation)
		{
			string path2 = AssetBundleManager.BaseDownloadingURL + path;
			if (subdirCheck)
			{
				if (!Directory.Exists(path2))
				{
					return result;
				}
				result = (from s in Directory.GetFiles(path2, "*.unity3d", SearchOption.AllDirectories)
					select s.Replace(AssetBundleManager.BaseDownloadingURL, string.Empty)).ToList();
			}
			else
			{
				if (!Directory.Exists(path2))
				{
					return result;
				}
				result = (from s in Directory.GetFiles(path2, "*.unity3d")
					select s.Replace(AssetBundleManager.BaseDownloadingURL, string.Empty)).ToList();
			}
		}
		return result;
	}

	public static bool CheckSameColor(Color c1, Color c2, bool checkAlpha = true)
	{
		if (c1.r == c2.r && c1.g == c2.g && c1.b == c2.b)
		{
			if (checkAlpha)
			{
				if (c1.a == c2.a)
				{
					return true;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	public static void CopySameNameTransform(Transform trfDst, Transform trfSrc)
	{
		FindAssist findAssist = new FindAssist();
		findAssist.Initialize(trfDst);
		Dictionary<string, GameObject> dictObjName = findAssist.dictObjName;
		FindAssist findAssist2 = new FindAssist();
		findAssist2.Initialize(trfSrc);
		Dictionary<string, GameObject> dictObjName2 = findAssist2.dictObjName;
		GameObject value = null;
		foreach (KeyValuePair<string, GameObject> item in dictObjName)
		{
			if (dictObjName2.TryGetValue(item.Key, out value))
			{
				item.Value.transform.localPosition = value.transform.localPosition;
				item.Value.transform.localRotation = value.transform.localRotation;
				item.Value.transform.localScale = value.transform.localScale;
			}
		}
	}

	public static void CreateUUID()
	{
		string dataPath = Application.dataPath;
		dataPath = Path.GetDirectoryName(dataPath);
		string path = dataPath + "/UserData/save/netUID.dat";
		if (File.Exists(path))
		{
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					string text = binaryReader.ReadString();
					if (text != string.Empty)
					{
						return;
					}
				}
			}
		}
		string directoryName = Path.GetDirectoryName(path);
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		string value = YS_Assist.CreateUUID();
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(output))
			{
				binaryWriter.Write(value);
			}
		}
	}

	public static string GetUUID()
	{
		string dataPath = Application.dataPath;
		dataPath = Path.GetDirectoryName(dataPath);
		string path = dataPath + "/UserData/save/netUID.dat";
		if (File.Exists(path))
		{
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					return binaryReader.ReadString();
				}
			}
		}
		return string.Empty;
	}

	public static T LoadAsset<T>(string assetBundleName, string assetName, bool clone = false, string manifestName = "") where T : Object
	{
		if (AssetBundleCheck.IsSimulation)
		{
			manifestName = string.Empty;
		}
		if (!AssetBundleCheck.IsFile(assetBundleName, assetName))
		{
			string text = "読み込みエラー\r\nassetBundleName：" + assetBundleName + "\tassetName：" + assetName;
			return (T)null;
		}
		AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(T), (!manifestName.IsNullOrEmpty()) ? manifestName : null);
		if (assetBundleLoadAssetOperation.IsEmpty())
		{
			string text2 = "読み込みエラー\r\nassetName：" + assetName;
			return (T)null;
		}
		T val = assetBundleLoadAssetOperation.GetAsset<T>();
		if (null != val && clone)
		{
			T val2 = Object.Instantiate(val);
			val2.name = val.name;
			val = val2;
		}
		return val;
	}
}
