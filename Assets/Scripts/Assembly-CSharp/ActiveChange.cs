using System.Collections.Generic;
using UnityEngine;

public class ActiveChange
{
	private int _index = -1;

	private Dictionary<int, List<GameObject>> dic = new Dictionary<int, List<GameObject>>();

	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			if (_index == (uint)value || (uint)value >= dic.Count)
			{
				return;
			}
			for (int i = 0; i < dic.Count; i++)
			{
				foreach (GameObject item in dic[i])
				{
					item.SetActive(i == (uint)value);
				}
			}
			_index = value;
		}
	}

	public ActiveChange(Transform transform, string[] judges)
	{
		if (transform == null)
		{
			return;
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			if (!gameObject.activeSelf)
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < judges.Length; j++)
			{
				if (gameObject.name.IndexOf(judges[j]) != -1)
				{
					flag = true;
					Add(j + 1, gameObject);
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				Add(0, gameObject);
			}
		}
	}

	private void Add(int i, GameObject gameObject)
	{
		List<GameObject> value = null;
		if (!dic.TryGetValue(i, out value))
		{
			value = new List<GameObject>();
		}
		value.Add(gameObject);
		dic[i] = value;
	}
}
