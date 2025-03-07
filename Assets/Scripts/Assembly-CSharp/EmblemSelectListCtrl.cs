using System.Collections.Generic;
using System.Linq;
using TMPro;
using UGUI_AssistLibrary;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EmblemSelectListCtrl : MonoBehaviour
{
	public delegate void OnChangeItemFunc(int index);

	[SerializeField]
	private TextMeshProUGUI textDrawName;

	[SerializeField]
	private RectTransform rtfScrollRect;

	[SerializeField]
	private RectTransform rtfContant;

	[SerializeField]
	private GameObject objContent;

	[SerializeField]
	private GameObject objTemp;

	[SerializeField]
	private Button btnPrev;

	[SerializeField]
	private Button btnNext;

	private string selectDrawName = string.Empty;

	private List<EmblemSelectInfo> lstSelectInfo = new List<EmblemSelectInfo>();

	public OnChangeItemFunc onChangeItemFunc;

	public void ClearList()
	{
		lstSelectInfo.Clear();
	}

	public void AddList(int index, string name, string assetBundle, string assetName)
	{
		EmblemSelectInfo emblemSelectInfo = new EmblemSelectInfo();
		emblemSelectInfo.index = index;
		emblemSelectInfo.name = name;
		emblemSelectInfo.assetBundle = assetBundle;
		emblemSelectInfo.assetName = assetName;
		lstSelectInfo.Add(emblemSelectInfo);
	}

	public void Create(OnChangeItemFunc _onChangeItemFunc)
	{
		onChangeItemFunc = _onChangeItemFunc;
		for (int num = objContent.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = objContent.transform.GetChild(num);
			Object.Destroy(child.gameObject);
		}
		ToggleGroup component = objContent.GetComponent<ToggleGroup>();
		int num2 = 0;
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			GameObject gameObject = Object.Instantiate(objTemp);
			EmblemSelectInfoComponent component2 = gameObject.GetComponent<EmblemSelectInfoComponent>();
			component2.info = lstSelectInfo[i];
			component2.info.sic = component2;
			component2.tgl.group = component;
			gameObject.transform.SetParent(objContent.transform, false);
			SetToggleHandler(gameObject);
			component2.img = gameObject.GetComponent<Image>();
			if ((bool)component2.img)
			{
				Texture2D texture2D = CommonLib.LoadAsset<Texture2D>(lstSelectInfo[i].assetBundle, lstSelectInfo[i].assetName, false, string.Empty);
				if ((bool)texture2D)
				{
					component2.img.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
				}
			}
			component2.Disvisible(lstSelectInfo[i].disvisible);
			component2.Disable(lstSelectInfo[i].disable);
			num2++;
		}
		ToggleAllOff();
	}

	public EmblemSelectInfo GetSelectInfoFromIndex(int index)
	{
		return lstSelectInfo.Find((EmblemSelectInfo item) => item.index == index);
	}

	public EmblemSelectInfo GetSelectInfoFromName(string name)
	{
		return lstSelectInfo.Find((EmblemSelectInfo item) => item.name == name);
	}

	public string GetNameFormIndex(int index)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.index == index);
		if (emblemSelectInfo != null)
		{
			return emblemSelectInfo.name;
		}
		return string.Empty;
	}

	public int GetIndexFromName(string name)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.name == name);
		if (emblemSelectInfo != null)
		{
			return emblemSelectInfo.index;
		}
		return -1;
	}

	public int GetSelectIndex()
	{
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			if (lstSelectInfo[i].sic.tgl.interactable && lstSelectInfo[i].sic.gameObject.activeSelf && lstSelectInfo[i].sic.tgl.isOn)
			{
				return lstSelectInfo[i].index;
			}
		}
		return -1;
	}

	public EmblemSelectInfoComponent GetSelectTopItem()
	{
		int selectIndex = GetSelectIndex();
		if (selectIndex == -1)
		{
			return null;
		}
		EmblemSelectInfo selectInfoFromIndex = GetSelectInfoFromIndex(selectIndex);
		if (selectInfoFromIndex != null)
		{
			return selectInfoFromIndex.sic;
		}
		return null;
	}

	public EmblemSelectInfoComponent GetSelectableTopItem()
	{
		SortedDictionary<int, EmblemSelectInfoComponent> sortedDictionary = new SortedDictionary<int, EmblemSelectInfoComponent>();
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			if (lstSelectInfo[i].sic.tgl.interactable && lstSelectInfo[i].sic.gameObject.activeSelf)
			{
				sortedDictionary[lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = lstSelectInfo[i].sic;
			}
		}
		EmblemSelectInfoComponent result = null;
		if (sortedDictionary.Count != 0)
		{
			result = sortedDictionary.First().Value;
		}
		return result;
	}

	public int GetDrawOrderFromIndex(int index)
	{
		SortedDictionary<int, EmblemSelectInfoComponent> sortedDictionary = new SortedDictionary<int, EmblemSelectInfoComponent>();
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			if (lstSelectInfo[i].sic.gameObject.activeSelf)
			{
				sortedDictionary[lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = lstSelectInfo[i].sic;
			}
		}
		foreach (var item in sortedDictionary.Select((KeyValuePair<int, EmblemSelectInfoComponent> val, int idx) => new { val, idx }))
		{
			if (item.val.Value.info.index == index)
			{
				return item.idx;
			}
		}
		return -1;
	}

	public int GetInclusiveCount()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			if (lstSelectInfo[i].sic.gameObject.activeSelf)
			{
				list.Add(lstSelectInfo[i].index);
			}
		}
		return list.Count;
	}

	public void SelectPrevItem()
	{
		List<EmblemSelectInfo> list = lstSelectInfo.Where((EmblemSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
		int num = list.FindIndex((EmblemSelectInfo lst) => lst.sic.tgl.isOn);
		if (num != -1)
		{
			int count = list.Count;
			int index = (num + count - 1) % count;
			SelectItem(list[index].index);
		}
	}

	public void SelectNextItem()
	{
		List<EmblemSelectInfo> list = lstSelectInfo.Where((EmblemSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
		int num = list.FindIndex((EmblemSelectInfo lst) => lst.sic.tgl.isOn);
		if (num != -1)
		{
			int count = list.Count;
			int index = (num + 1) % count;
			SelectItem(list[index].index);
		}
	}

	public void SelectItem(int index)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.index == index);
		if (emblemSelectInfo != null)
		{
			emblemSelectInfo.sic.tgl.isOn = true;
			ChangeItem(emblemSelectInfo.sic.gameObject);
			UpdateScrollPosition();
		}
	}

	public void SelectItem(string name)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.name == name);
		if (emblemSelectInfo != null)
		{
			emblemSelectInfo.sic.tgl.isOn = true;
			ChangeItem(emblemSelectInfo.sic.gameObject);
			UpdateScrollPosition();
		}
	}

	public void UpdateScrollPosition()
	{
	}

	public void OnPointerClick(GameObject obj)
	{
		if (!(null == obj))
		{
			EmblemSelectInfoComponent component = obj.GetComponent<EmblemSelectInfoComponent>();
			if (!(null == component) && component.tgl.interactable)
			{
				ChangeItem(obj);
			}
		}
	}

	public void OnPointerEnter(GameObject obj)
	{
		if (!(null == obj))
		{
			EmblemSelectInfoComponent component = obj.GetComponent<EmblemSelectInfoComponent>();
			if (!(null == component) && component.tgl.interactable && (bool)textDrawName)
			{
				textDrawName.text = component.info.name;
			}
		}
	}

	public void OnPointerExit(GameObject obj)
	{
		if (!(null == obj))
		{
			EmblemSelectInfoComponent component = obj.GetComponent<EmblemSelectInfoComponent>();
			if (!(null == component) && component.tgl.interactable && (bool)textDrawName)
			{
				textDrawName.text = selectDrawName;
			}
		}
	}

	public void ChangeItem(GameObject obj)
	{
		EmblemSelectInfoComponent component = obj.GetComponent<EmblemSelectInfoComponent>();
		if (onChangeItemFunc != null)
		{
			onChangeItemFunc(component.info.index);
		}
		selectDrawName = component.info.name;
		if ((bool)textDrawName)
		{
			textDrawName.text = selectDrawName;
		}
	}

	public void ToggleAllOff()
	{
		for (int i = 0; i < lstSelectInfo.Count; i++)
		{
			lstSelectInfo[i].sic.tgl.isOn = false;
		}
	}

	public void DisableItem(int index, bool disable)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.index == index);
		if (emblemSelectInfo != null)
		{
			emblemSelectInfo.disable = disable;
			emblemSelectInfo.sic.Disable(disable);
		}
	}

	public void DisableItem(string name, bool disable)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.name == name);
		emblemSelectInfo.disable = disable;
		emblemSelectInfo.sic.Disable(disable);
	}

	public void DisvisibleItem(int index, bool disvisible)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.index == index);
		if (emblemSelectInfo != null)
		{
			emblemSelectInfo.disvisible = disvisible;
			emblemSelectInfo.sic.Disvisible(disvisible);
		}
	}

	public void DisvisibleItem(string name, bool disvisible)
	{
		EmblemSelectInfo emblemSelectInfo = lstSelectInfo.Find((EmblemSelectInfo item) => item.name == name);
		if (emblemSelectInfo != null)
		{
			emblemSelectInfo.disvisible = disvisible;
			emblemSelectInfo.sic.Disvisible(disvisible);
		}
	}

	private void SetToggleHandler(GameObject obj)
	{
		UIAL_EventTrigger uIAL_EventTrigger = obj.AddComponent<UIAL_EventTrigger>();
		uIAL_EventTrigger.triggers = new List<UIAL_EventTrigger.Entry>();
		UIAL_EventTrigger.Entry entry = new UIAL_EventTrigger.Entry();
		entry.eventID = EventTriggerType.PointerClick;
		entry.buttonType = UIAL_EventTrigger.ButtonType.Left;
		entry.callback.AddListener(delegate
		{
			OnPointerClick(obj);
		});
		uIAL_EventTrigger.triggers.Add(entry);
		if ((bool)textDrawName)
		{
			entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(delegate
			{
				OnPointerEnter(obj);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener(delegate
			{
				OnPointerExit(obj);
			});
			uIAL_EventTrigger.triggers.Add(entry);
		}
	}

	private void Start()
	{
		btnPrev.OnClickAsObservable().Subscribe(delegate
		{
			SelectPrevItem();
		});
		btnNext.OnClickAsObservable().Subscribe(delegate
		{
			SelectNextItem();
		});
	}
}
