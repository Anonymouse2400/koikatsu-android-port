using System.Collections.Generic;
using UnityEngine;

public class HSceneSpriteObjectCategory : MonoBehaviour
{
	public List<GameObject> lstObj;

	public GameObject GetObject(int _array)
	{
		if (lstObj.Count <= _array)
		{
			return null;
		}
		return lstObj[_array];
	}

	public int GetAllEnable()
	{
		int num = 0;
		for (int i = 0; i < lstObj.Count; i++)
		{
			if (!(lstObj[i] == null) && lstObj[i].activeSelf)
			{
				num++;
			}
		}
		return (num == lstObj.Count) ? 1 : ((num != 0) ? 2 : 0);
	}

	public int GetCount()
	{
		return lstObj.Count;
	}

	public bool GetActive(int _array)
	{
		if (lstObj.Count <= _array)
		{
			return false;
		}
		if (lstObj[_array] == null)
		{
			return false;
		}
		return lstObj[_array].activeSelf;
	}

	public void SetActive(bool _active, int _array = -1)
	{
		if (_array < 0)
		{
			for (int i = 0; i < lstObj.Count; i++)
			{
				if (!(lstObj[i] == null) && lstObj[i].activeSelf != _active)
				{
					lstObj[i].gameObject.SetActive(_active);
				}
			}
		}
		else if (lstObj.Count > _array && (bool)lstObj[_array] && lstObj[_array].activeSelf != _active)
		{
			lstObj[_array].gameObject.SetActive(_active);
		}
	}

	public void SetActiveToggle(int _array)
	{
		for (int i = 0; i < lstObj.Count; i++)
		{
			if (!(lstObj[i] == null))
			{
				lstObj[i].gameObject.SetActive(_array == i);
			}
		}
	}

	public bool IsEmpty(int _array)
	{
		if (lstObj.Count <= _array)
		{
			return true;
		}
		return lstObj[_array] == null;
	}
}
