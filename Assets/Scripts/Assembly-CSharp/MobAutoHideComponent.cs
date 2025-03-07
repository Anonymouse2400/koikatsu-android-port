using System.Collections.Generic;
using Illusion.Extensions;
using Manager;
using UnityEngine;

public class MobAutoHideComponent : MonoBehaviour
{
	private Transform[] trfChild;

	private void Start()
	{
		int childCount = base.transform.childCount;
		trfChild = new Transform[childCount];
		for (int i = 0; i < childCount; i++)
		{
			trfChild[i] = base.transform.GetChild(i);
		}
	}

	private void Update()
	{
		if (!Singleton<Character>.IsInstance())
		{
			return;
		}
		bool flag = false;
		foreach (KeyValuePair<int, ChaControl> item in Singleton<Character>.Instance.dictEntryChara)
		{
			if (!(null == item.Value))
			{
				flag |= item.Value.hiPoly;
			}
		}
		Transform[] array = trfChild;
		foreach (Transform transform in array)
		{
			transform.gameObject.SetActiveIfDifferent(!flag);
		}
	}
}
