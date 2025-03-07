using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HitCollisionEnableCtrl : MonoBehaviour
{
	[Serializable]
	public class CollisionInfo
	{
		public string nameObj = string.Empty;

		public bool visible;
	}

	public Dictionary<string, List<CollisionInfo>> dicInfo = new Dictionary<string, List<CollisionInfo>>();

	public Dictionary<string, GameObject> dicObj = new Dictionary<string, GameObject>();

	public bool isActive = true;

	public bool Init(ChaControl _chara, GameObject _objHitHead)
	{
		Release();
		List<GameObject> headCollisionComponent = GetHeadCollisionComponent(_objHitHead);
		for (int i = 0; i < headCollisionComponent.Count; i++)
		{
			if (!dicObj.ContainsKey(headCollisionComponent[i].name))
			{
				dicObj.Add(headCollisionComponent[i].name, headCollisionComponent[i]);
			}
		}
		ChaHitControlComponent hitBodyCtrlCmp = _chara.hitBodyCtrlCmp;
		if ((bool)hitBodyCtrlCmp)
		{
			GameObject[] objList = hitBodyCtrlCmp.objList;
			foreach (GameObject gameObject in objList)
			{
				if (!dicObj.ContainsKey(gameObject.name))
				{
					dicObj.Add(gameObject.name, gameObject);
				}
			}
		}
		return true;
	}

	private List<GameObject> GetHeadCollisionComponent(GameObject _objHitHead)
	{
		List<GameObject> result = new List<GameObject>();
		if (_objHitHead == null)
		{
			return result;
		}
		SkinnedCollisionHelper[] componentsInChildren = _objHitHead.GetComponentsInChildren<SkinnedCollisionHelper>(true);
		if (componentsInChildren == null)
		{
			return result;
		}
		return componentsInChildren.Select((SkinnedCollisionHelper i) => i.gameObject).ToList();
	}

	public void Release()
	{
		dicObj.Clear();
		dicInfo.Clear();
		isActive = true;
	}

	public bool LoadText(string _assetpath, string _file)
	{
		dicInfo.Clear();
		if (_file == string.Empty)
		{
			return false;
		}
		List<string> list = GlobalMethod.LoadAllListTextFromList(_assetpath, _file);
		if (list == null)
		{
			return false;
		}
		foreach (string item2 in list)
		{
			string[,] data;
			GlobalMethod.GetListString(item2, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int i = 0; i < length; i++)
			{
				string key = data[i, 0];
				if (!dicInfo.ContainsKey(key))
				{
					dicInfo.Add(key, new List<CollisionInfo>());
				}
				for (int j = 1; j < length2; j += 2)
				{
					CollisionInfo collisionInfo = new CollisionInfo();
					collisionInfo.nameObj = data[i, j];
					collisionInfo.visible = data[i, j + 1] == "1";
					CollisionInfo item = collisionInfo;
					dicInfo[key].Add(item);
				}
			}
		}
		return true;
	}

	public bool SetPlay(string _animation)
	{
		if (dicInfo.ContainsKey(_animation))
		{
			Visible(true);
			for (int i = 0; i < dicInfo[_animation].Count; i++)
			{
				CollisionInfo collisionInfo = dicInfo[_animation][i];
				if (dicObj.ContainsKey(collisionInfo.nameObj))
				{
					dicObj[collisionInfo.nameObj].SetActive(collisionInfo.visible);
				}
			}
			return true;
		}
		Visible(false);
		return false;
	}

	private void Visible(bool _visible)
	{
		if (isActive == _visible)
		{
			return;
		}
		foreach (KeyValuePair<string, GameObject> item in dicObj)
		{
			if ((bool)item.Value)
			{
				item.Value.SetActive(_visible);
			}
		}
		isActive = _visible;
	}
}
