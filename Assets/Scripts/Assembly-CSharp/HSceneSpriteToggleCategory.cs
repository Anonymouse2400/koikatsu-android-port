using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HSceneSpriteToggleCategory : MonoBehaviour
{
	public List<Toggle> lstToggle;

	public GameObject GetObject(int _array)
	{
		if (lstToggle.Count <= _array)
		{
			return null;
		}
		return lstToggle[_array].gameObject;
	}

	public Toggle GetToggle(int _array)
	{
		if (lstToggle.Count <= _array)
		{
			return null;
		}
		return lstToggle[_array];
	}

	public int GetAllEnable()
	{
		int num = 0;
		for (int i = 0; i < lstToggle.Count; i++)
		{
			if (lstToggle[i].gameObject.activeSelf)
			{
				num++;
			}
		}
		return (num == lstToggle.Count) ? 1 : ((num != 0) ? 2 : 0);
	}

	public int GetCount()
	{
		return lstToggle.Count;
	}

	public bool GetActive(int _array)
	{
		if (lstToggle.Count <= _array)
		{
			return false;
		}
		return lstToggle[_array].gameObject.activeSelf;
	}

	public void SetActive(bool _active, int _array = -1)
	{
		if (_array < 0)
		{
			for (int i = 0; i < lstToggle.Count; i++)
			{
				if (lstToggle[i].gameObject.activeSelf != _active)
				{
					lstToggle[i].gameObject.SetActive(_active);
				}
			}
		}
		else if (lstToggle.Count > _array && lstToggle[_array].gameObject.activeSelf != _active)
		{
			lstToggle[_array].gameObject.SetActive(_active);
		}
	}

	public void SetToggleON(int _array, bool _isON)
	{
		for (int i = 0; i < lstToggle.Count; i++)
		{
			if (i == _array)
			{
				lstToggle[i].isOn = _isON;
			}
		}
	}

	public void SetToggleAll(bool _isON)
	{
		for (int i = 0; i < lstToggle.Count; i++)
		{
			lstToggle[i].isOn = _isON;
		}
	}
}
