using System.Collections.Generic;
using IllusionUtility.GetUtility;
using UnityEngine;

public class ParentObjectCtrl : MonoBehaviour
{
	public bool isInit;

	public bool isActive = true;

	private Dictionary<string, GameObject> dicObject = new Dictionary<string, GameObject>();

	private Dictionary<string, List<string>> dicObjectCtrl = new Dictionary<string, List<string>>();

	private bool isAllVisible = true;

	public bool Init(string _strAssetFolderPath, string _file, GameObject _objBody, ChaControl _chaCtrl, HashSet<string> _hsAssetPath, bool _isAllVisible)
	{
		if (_objBody == null)
		{
			return false;
		}
		ReleaseObject();
		string text = GlobalMethod.LoadAllListText(_strAssetFolderPath, _file);
		if (text == string.Empty)
		{
			isInit = false;
			return false;
		}
		isAllVisible = _isAllVisible;
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			for (int j = 0; j < length2; j += 3)
			{
				string self = data[i, j];
				string text2 = data[i, j + 1];
				string text3 = data[i, j + 2];
				if (self.IsNullOrEmpty() && text2.IsNullOrEmpty() && text3.IsNullOrEmpty())
				{
					break;
				}
				GameObject gameObject = _objBody.transform.FindLoop(self);
				GameObject gameObject2 = CommonLib.LoadAsset<GameObject>(text2, text3, true, string.Empty);
				AssetBundleManager.UnloadAssetBundle(text2, true);
				EliminateScale[] componentsInChildren = gameObject2.GetComponentsInChildren<EliminateScale>(true);
				EliminateScale[] array = componentsInChildren;
				foreach (EliminateScale eliminateScale in array)
				{
					eliminateScale.chaCtrl = _chaCtrl;
				}
				if (gameObject != null && gameObject2 != null)
				{
					gameObject2.transform.SetParent(gameObject.transform, false);
				}
				if (!dicObject.ContainsKey(text3))
				{
					dicObject.Add(text3, gameObject2);
					continue;
				}
				Object.Destroy(dicObject[text3]);
				dicObject[text3] = gameObject2;
			}
		}
		isInit = true;
		return true;
	}

	public bool LoadText(string _assetpath, string _file)
	{
		dicObjectCtrl.Clear();
		if (_file == string.Empty)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText(_assetpath, _file);
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			string key = data[i, num++];
			if (!dicObjectCtrl.ContainsKey(key))
			{
				dicObjectCtrl.Add(key, new List<string>());
			}
			List<string> list = dicObjectCtrl[key];
			list.Clear();
			for (int j = num; j < length2; j++)
			{
				string text2 = data[i, j];
				if (text2.IsNullOrEmpty())
				{
					break;
				}
				list.Add(text2);
			}
		}
		return true;
	}

	public bool ReleaseObject()
	{
		foreach (KeyValuePair<string, GameObject> item in dicObject)
		{
			if (!(item.Value == null))
			{
				Object.Destroy(item.Value);
			}
		}
		dicObject.Clear();
		isInit = false;
		isActive = true;
		return true;
	}

	public bool Proc(string _nameNextAnimation)
	{
		Visible(isAllVisible);
		if (!dicObjectCtrl.ContainsKey(_nameNextAnimation))
		{
			return false;
		}
		isActive = !isAllVisible;
		foreach (string item in dicObjectCtrl[_nameNextAnimation])
		{
			dicObject[item].SetActive(!isAllVisible);
		}
		return true;
	}

	public GameObject GetObject(string _key)
	{
		GameObject value;
		dicObject.TryGetValue(_key, out value);
		return value;
	}

	private bool Visible(bool _visible)
	{
		if (isActive == _visible)
		{
			return false;
		}
		foreach (KeyValuePair<string, GameObject> item in dicObject)
		{
			item.Value.SetActive(_visible);
		}
		isActive = _visible;
		return false;
	}
}
