using System.Collections.Generic;
using UnityEngine;

public class AnimatorLayerCtrl : MonoBehaviour
{
	public bool isInit;

	[SerializeField]
	private Dictionary<string, List<int>> dicLayer = new Dictionary<string, List<int>>();

	[SerializeField]
	private Animator anim;

	public bool Init(Animator _animator)
	{
		anim = _animator;
		return true;
	}

	public bool Load(string _strAssetFolderPath, string _file)
	{
		isInit = false;
		dicLayer.Clear();
		ResetAllLayer();
		if (!anim)
		{
			return false;
		}
		string text = GlobalMethod.LoadAllListText(_strAssetFolderPath, "al_" + _file);
		if (text == string.Empty)
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
			if (!dicLayer.ContainsKey(key))
			{
				dicLayer.Add(key, new List<int>());
			}
			List<int> list = dicLayer[key];
			list.Clear();
			for (int j = num; j < length2; j++)
			{
				string text2 = data[i, j];
				if (text2.IsNullOrEmpty())
				{
					break;
				}
				list.Add(int.Parse(text2));
			}
		}
		isInit = true;
		return true;
	}

	public bool ResetAllLayer()
	{
		if (!anim)
		{
			return false;
		}
		for (int i = 73; i < anim.layerCount; i++)
		{
			anim.SetLayerWeight(i, 0f);
		}
		return true;
	}

	public bool Proc(string _nameNextAnimation)
	{
		if (!isInit)
		{
			return false;
		}
		ResetAllLayer();
		if (!dicLayer.ContainsKey(_nameNextAnimation))
		{
			return false;
		}
		foreach (int item in dicLayer[_nameNextAnimation])
		{
			if (anim.layerCount > item)
			{
				anim.SetLayerWeight(item, 1f);
			}
		}
		return true;
	}
}
