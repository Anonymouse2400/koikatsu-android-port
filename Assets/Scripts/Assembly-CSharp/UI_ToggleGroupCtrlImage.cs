using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleGroupCtrlImage : MonoBehaviour
{
	[Serializable]
	public class ItemInfo
	{
		public Toggle tglItem;

		public Image imgItem;
	}

	public ItemInfo[] items;

	public void Start()
	{
		OnChange(0);
	}

	public int GetSelectIndex()
	{
		var anon = items.Select((ItemInfo v, int i) => new { v, i }).FirstOrDefault(x => x.v.tglItem.isOn);
		if (anon != null)
		{
			return anon.i;
		}
		return -1;
	}

	public void OnChange(int idx)
	{
		if (!MathfEx.RangeEqualOn(0, idx, items.Length - 1) || items[idx] == null)
		{
			return;
		}
		Toggle tglItem = items[idx].tglItem;
		if (null == tglItem || !tglItem.isOn)
		{
			return;
		}
		foreach (var item in items.Select((ItemInfo v, int i) => new { v, i }))
		{
			if (item.i != idx && item.v != null)
			{
				Image imgItem = item.v.imgItem;
				if ((bool)imgItem)
				{
					imgItem.enabled = false;
				}
			}
		}
		if ((bool)items[idx].imgItem)
		{
			items[idx].imgItem.enabled = true;
		}
	}
}
