using System;
using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleGroupCtrl : MonoBehaviour
{
	[Serializable]
	public class ItemInfo
	{
		public Toggle tglItem;

		public CanvasGroup cgItem;
	}

	public ItemInfo[] items;

	public virtual void Start()
	{
		if (!items.Any())
		{
			return;
		}
		(from item in items.Select((ItemInfo val, int idx) => new { val, idx })
			where item.val != null && item.val.tglItem != null
			select item).ToList().ForEach(item =>
		{
			(from isOn in item.val.tglItem.OnValueChangedAsObservable()
				where isOn
				select isOn).Subscribe(delegate
			{
				foreach (var anon in items.Select((ItemInfo v, int i) => new { v, i }))
				{
					if (anon.i != item.idx && anon.v != null)
					{
						CanvasGroup cgItem = anon.v.cgItem;
						if ((bool)cgItem)
						{
							cgItem.Enable(false);
						}
					}
				}
				if ((bool)item.val.cgItem)
				{
					item.val.cgItem.Enable(true);
				}
			});
		});
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
}
