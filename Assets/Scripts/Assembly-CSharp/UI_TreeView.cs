using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_TreeView : MonoBehaviour
{
	public bool rootFlag;

	public bool ExpandFirst = true;

	public float topMargin = 2f;

	public float bottomMargin = 2f;

	public ScrollRect scrollRect;

	public RectTransform rtScroll;

	public bool unused;

	private Toggle minmax;

	private UI_TreeView tvRoot;

	private List<UI_TreeView> lstChild = new List<UI_TreeView>();

	private void Start()
	{
		if (rootFlag)
		{
			CreateTree(this);
			if (ExpandFirst)
			{
				ExpandAll();
			}
			else
			{
				CollapseAll();
			}
		}
	}

	private void CreateTree(UI_TreeView tvroot)
	{
		if (null == minmax)
		{
			minmax = base.gameObject.GetComponent<Toggle>();
		}
		if ((bool)minmax)
		{
			minmax.onValueChanged.AddListener(MinMaxChange);
		}
		tvRoot = tvroot;
		foreach (Transform item in base.gameObject.transform)
		{
			UI_TreeView component = item.gameObject.GetComponent<UI_TreeView>();
			if (!(null == component))
			{
				lstChild.Add(component);
				component.CreateTree(tvroot);
			}
		}
	}

	public void UpdateView(ref float totalPosY, float parentPosY)
	{
		float parentPosY2 = totalPosY;
		if (rootFlag)
		{
			totalPosY = 0f - topMargin;
		}
		else
		{
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			if ((bool)component)
			{
				component.anchoredPosition = new Vector2(component.anchoredPosition.x, totalPosY - parentPosY);
				if (base.gameObject.activeSelf && !unused)
				{
					totalPosY -= component.sizeDelta.y;
				}
			}
		}
		foreach (Transform item in base.gameObject.transform)
		{
			UI_TreeView component2 = item.gameObject.GetComponent<UI_TreeView>();
			if (!(null == component2))
			{
				if (!base.gameObject.activeSelf || component2.unused)
				{
					item.gameObject.SetActive(false);
				}
				else if ((bool)minmax)
				{
					item.gameObject.SetActive(minmax.isOn);
				}
				component2.UpdateView(ref totalPosY, parentPosY2);
			}
		}
		if (rootFlag && (bool)rtScroll)
		{
			rtScroll.sizeDelta = new Vector2(rtScroll.sizeDelta.x, 0f - totalPosY + bottomMargin);
			if ((bool)scrollRect)
			{
				scrollRect.enabled = false;
				scrollRect.enabled = true;
			}
		}
	}

	public void ExpandAll()
	{
		if (rootFlag)
		{
			ChangeExpandOrCollapseLoop(true);
			float totalPosY = 0f;
			UpdateView(ref totalPosY, 0f);
		}
	}

	public void CollapseAll()
	{
		if (rootFlag)
		{
			ChangeExpandOrCollapseLoop(false);
			float totalPosY = 0f;
			UpdateView(ref totalPosY, 0f);
		}
	}

	private void ChangeExpandOrCollapseLoop(bool expand)
	{
		if ((bool)minmax)
		{
			minmax.isOn = expand;
		}
		foreach (Transform item in base.gameObject.transform)
		{
			UI_TreeView component = item.gameObject.GetComponent<UI_TreeView>();
			if (!(null == component))
			{
				component.ChangeExpandOrCollapseLoop(expand);
			}
		}
	}

	public void SetUnused(bool flag)
	{
		unused = flag;
		base.gameObject.SetActive(!unused);
	}

	private void Update()
	{
	}

	private void MinMaxChange(bool flag)
	{
		float totalPosY = 0f;
		if ((bool)tvRoot)
		{
			tvRoot.UpdateView(ref totalPosY, 0f);
		}
	}

	public void UpdateView()
	{
		float totalPosY = 0f;
		if ((bool)tvRoot && tvRoot.gameObject.activeSelf)
		{
			tvRoot.UpdateView(ref totalPosY, 0f);
		}
	}

	public UI_TreeView GetRoot()
	{
		return tvRoot;
	}
}
