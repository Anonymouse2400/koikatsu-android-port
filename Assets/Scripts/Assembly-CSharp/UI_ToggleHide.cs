using System.Linq;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleHide : MonoBehaviour
{
	public Toggle[] tglItem;

	public GameObject[] tabItem;

	public void Start()
	{
		OnChange(0);
	}

	public void OnChange(int idx)
	{
		if (!MathfEx.RangeEqualOn(0, idx, tabItem.Length - 1))
		{
			return;
		}
		Toggle toggle = tglItem[idx];
		if (null == toggle || !toggle.isOn)
		{
			return;
		}
		foreach (var item in tabItem.Select((GameObject v, int i) => new { v, i }))
		{
			if (item.i != idx && (bool)item.v)
			{
				item.v.gameObject.SetActiveIfDifferent(false);
			}
		}
		if ((bool)tabItem[idx])
		{
			tabItem[idx].gameObject.SetActiveIfDifferent(true);
		}
	}
}
