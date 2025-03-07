using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class HSceneSpriteCategory : MonoBehaviour
{
	public List<Button> lstButton;

	public void SetEnable(bool _enable, int _array = -1)
	{
		if (_array < 0)
		{
			for (int i = 0; i < lstButton.Count; i++)
			{
				if (lstButton[i].interactable != _enable)
				{
					lstButton[i].interactable = _enable;
				}
			}
		}
		else if (lstButton.Count > _array && lstButton[_array].interactable != _enable)
		{
			lstButton[_array].interactable = _enable;
		}
	}

	public bool GetEnable(int _array)
	{
		if (lstButton.Count > _array)
		{
			return lstButton[_array].interactable;
		}
		return false;
	}

	public int GetAllEnable()
	{
		int num = 0;
		for (int i = 0; i < lstButton.Count; i++)
		{
			if (!(lstButton[i] == null) && lstButton[i].interactable)
			{
				num++;
			}
		}
		return (num == lstButton.Count) ? 1 : ((num != 0) ? 2 : 0);
	}

	public GameObject GetObject(int _array)
	{
		if (lstButton.Count > _array)
		{
			return lstButton[_array].gameObject;
		}
		return null;
	}

	public int GetCount()
	{
		return lstButton.Count;
	}

	public void SetActive(bool _active, int _array = -1)
	{
		if (_array < 0)
		{
			for (int i = 0; i < lstButton.Count; i++)
			{
				if (lstButton[i].isActiveAndEnabled != _active)
				{
					lstButton[i].gameObject.SetActive(_active);
				}
			}
		}
		else if (lstButton.Count > _array && lstButton[_array].isActiveAndEnabled != _active)
		{
			lstButton[_array].gameObject.SetActive(_active);
		}
	}

	public bool GetActive(int _array)
	{
		if (lstButton.Count > _array)
		{
			return lstButton[_array].isActiveAndEnabled;
		}
		return false;
	}

	public bool SetAction(int _array, Action<Unit> _action)
	{
		if (lstButton.Count <= _array)
		{
			return false;
		}
		lstButton[_array].OnClickAsObservable().Subscribe(_action);
		return true;
	}

	public void SetActiveToggle(int _array)
	{
		for (int i = 0; i < lstButton.Count; i++)
		{
			if (!(lstButton[i] == null))
			{
				lstButton[i].gameObject.SetActive(_array == i);
			}
		}
	}
}
