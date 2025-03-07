using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using TMPro;
using UGUI_AssistLibrary;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomSelectListCtrl : MonoBehaviour
	{
		public delegate void OnChangeItemFunc(int index);

		[HideInInspector]
		public CanvasGroup[] canvasGrp;

		[HideInInspector]
		public Image[] imgRaycast;

		[SerializeField]
		private CustomSelectWindow csWindow;

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

		private List<CustomSelectInfo> lstSelectInfo = new List<CustomSelectInfo>();

		public OnChangeItemFunc onChangeItemFunc;

		public void ClearList()
		{
			lstSelectInfo.Clear();
		}

		public void AddList(int category, int index, string name, string assetBundle, string assetName)
		{
			CustomSelectInfo customSelectInfo = new CustomSelectInfo();
			customSelectInfo.category = category;
			customSelectInfo.index = index;
			customSelectInfo.name = name;
			customSelectInfo.assetBundle = assetBundle;
			customSelectInfo.assetName = assetName;
			lstSelectInfo.Add(customSelectInfo);
		}

		public void Create(OnChangeItemFunc _onChangeItemFunc)
		{
			onChangeItemFunc = _onChangeItemFunc;
			List<Image> list = new List<Image>();
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
				CustomSelectInfoComponent component2 = gameObject.GetComponent<CustomSelectInfoComponent>();
				component2.info = lstSelectInfo[i];
				component2.info.sic = component2;
				Image component3 = gameObject.GetComponent<Image>();
				if ((bool)component3)
				{
					list.Add(component3);
				}
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
				Transform transform = gameObject.transform.Find("New");
				if ((bool)transform)
				{
					byte b = Singleton<Character>.Instance.chaListCtrl.CheckItemID(lstSelectInfo[i].category, lstSelectInfo[i].index);
					if (b == 1)
					{
						transform.gameObject.SetActiveIfDifferent(true);
					}
					component2.objNew = transform.gameObject;
				}
				component2.Disvisible(lstSelectInfo[i].disvisible);
				component2.Disable(lstSelectInfo[i].disable);
				num2++;
			}
			ToggleAllOff();
			imgRaycast = list.ToArray();
		}

		public void UpdateStateNew()
		{
			if (lstSelectInfo == null)
			{
				return;
			}
			for (int i = 0; i < lstSelectInfo.Count; i++)
			{
				byte b = Singleton<Character>.Instance.chaListCtrl.CheckItemID(lstSelectInfo[i].category, lstSelectInfo[i].index);
				if (b != 1)
				{
					lstSelectInfo[i].sic.objNew.SetActiveIfDifferent(false);
				}
			}
		}

		public CustomSelectInfo GetSelectInfoFromIndex(int index)
		{
			return lstSelectInfo.Find((CustomSelectInfo item) => item.index == index);
		}

		public CustomSelectInfo GetSelectInfoFromName(string name)
		{
			return lstSelectInfo.Find((CustomSelectInfo item) => item.name == name);
		}

		public string GetNameFormIndex(int index)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.index == index);
			if (customSelectInfo != null)
			{
				return customSelectInfo.name;
			}
			return string.Empty;
		}

		public int GetIndexFromName(string name)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.name == name);
			if (customSelectInfo != null)
			{
				return customSelectInfo.index;
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

		public CustomSelectInfoComponent GetSelectTopItem()
		{
			int selectIndex = GetSelectIndex();
			if (selectIndex == -1)
			{
				return null;
			}
			CustomSelectInfo selectInfoFromIndex = GetSelectInfoFromIndex(selectIndex);
			if (selectInfoFromIndex != null)
			{
				return selectInfoFromIndex.sic;
			}
			return null;
		}

		public CustomSelectInfoComponent GetSelectableTopItem()
		{
			SortedDictionary<int, CustomSelectInfoComponent> sortedDictionary = new SortedDictionary<int, CustomSelectInfoComponent>();
			for (int i = 0; i < lstSelectInfo.Count; i++)
			{
				if (lstSelectInfo[i].sic.tgl.interactable && lstSelectInfo[i].sic.gameObject.activeSelf)
				{
					sortedDictionary[lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = lstSelectInfo[i].sic;
				}
			}
			CustomSelectInfoComponent result = null;
			if (sortedDictionary.Count != 0)
			{
				result = sortedDictionary.First().Value;
			}
			return result;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			SortedDictionary<int, CustomSelectInfoComponent> sortedDictionary = new SortedDictionary<int, CustomSelectInfoComponent>();
			for (int i = 0; i < lstSelectInfo.Count; i++)
			{
				if (lstSelectInfo[i].sic.gameObject.activeSelf)
				{
					sortedDictionary[lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = lstSelectInfo[i].sic;
				}
			}
			foreach (var item in sortedDictionary.Select((KeyValuePair<int, CustomSelectInfoComponent> val, int idx) => new { val, idx }))
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
			List<CustomSelectInfo> list = lstSelectInfo.Where((CustomSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
			int num = list.FindIndex((CustomSelectInfo lst) => lst.sic.tgl.isOn);
			if (num != -1)
			{
				int count = list.Count;
				int index = (num + count - 1) % count;
				SelectItem(list[index].index);
			}
		}

		public void SelectNextItem()
		{
			List<CustomSelectInfo> list = lstSelectInfo.Where((CustomSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
			int num = list.FindIndex((CustomSelectInfo lst) => lst.sic.tgl.isOn);
			if (num != -1)
			{
				int count = list.Count;
				int index = (num + 1) % count;
				SelectItem(list[index].index);
			}
		}

		public void SelectItem(int index)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.index == index);
			if (customSelectInfo != null)
			{
				customSelectInfo.sic.tgl.isOn = true;
				ChangeItem(customSelectInfo.sic.gameObject);
				UpdateScrollPosition();
			}
		}

		public void SelectItem(string name)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.name == name);
			if (customSelectInfo != null)
			{
				customSelectInfo.sic.tgl.isOn = true;
				ChangeItem(customSelectInfo.sic.gameObject);
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
				CustomSelectInfoComponent component = obj.GetComponent<CustomSelectInfoComponent>();
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
				CustomSelectInfoComponent component = obj.GetComponent<CustomSelectInfoComponent>();
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
				CustomSelectInfoComponent component = obj.GetComponent<CustomSelectInfoComponent>();
				if (!(null == component) && component.tgl.interactable && (bool)textDrawName)
				{
					textDrawName.text = selectDrawName;
				}
			}
		}

		public void ChangeItem(GameObject obj)
		{
			CustomSelectInfoComponent component = obj.GetComponent<CustomSelectInfoComponent>();
			if (onChangeItemFunc != null)
			{
				onChangeItemFunc(component.info.index);
			}
			selectDrawName = component.info.name;
			if ((bool)textDrawName)
			{
				textDrawName.text = selectDrawName;
			}
			if ((bool)component.objNew)
			{
				Singleton<Character>.Instance.chaListCtrl.AddItemID(component.info.category, component.info.index, 2);
				component.objNew.SetActiveIfDifferent(false);
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
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.index == index);
			if (customSelectInfo != null)
			{
				customSelectInfo.disable = disable;
				customSelectInfo.sic.Disable(disable);
			}
		}

		public void DisableItem(string name, bool disable)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.name == name);
			customSelectInfo.disable = disable;
			customSelectInfo.sic.Disable(disable);
		}

		public void DisvisibleItem(int index, bool disvisible)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.index == index);
			if (customSelectInfo != null)
			{
				customSelectInfo.disvisible = disvisible;
				customSelectInfo.sic.Disvisible(disvisible);
			}
		}

		public void DisvisibleItem(string name, bool disvisible)
		{
			CustomSelectInfo customSelectInfo = lstSelectInfo.Find((CustomSelectInfo item) => item.name == name);
			if (customSelectInfo != null)
			{
				customSelectInfo.disvisible = disvisible;
				customSelectInfo.sic.Disvisible(disvisible);
			}
		}

		public void UpdateCategory()
		{
			Toggle[] tglCategory = csWindow.tglCategory;
			if (tglCategory == null || tglCategory.Length == 0)
			{
				return;
			}
			bool[] array = tglCategory.Select((Toggle x) => x.isOn).ToArray();
			bool[] array2 = array.Where((bool x) => x).ToArray();
			List<CustomSelectInfo> list = null;
			if (array2.Length == 0)
			{
				for (int i = 0; i < lstSelectInfo.Count; i++)
				{
					lstSelectInfo[i].sic.Disvisible(false);
				}
				list = new List<CustomSelectInfo>(lstSelectInfo);
			}
			else
			{
				bool flag = false;
				list = new List<CustomSelectInfo>();
				for (int j = 0; j < lstSelectInfo.Count; j++)
				{
					flag = array[lstSelectInfo[j].searchType];
					lstSelectInfo[j].sic.Disvisible(!flag);
					if (flag)
					{
						list.Add(lstSelectInfo[j]);
					}
				}
			}
			CustomSelectInfo customSelectInfo = list.Find((CustomSelectInfo x) => x.sic.tgl.isOn);
			if (customSelectInfo == null)
			{
				selectDrawName = string.Empty;
			}
			else
			{
				selectDrawName = customSelectInfo.sic.name;
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

		private void GetCanvasGroup()
		{
			List<CanvasGroup> list = new List<CanvasGroup>();
			CanvasGroup component = GetComponent<CanvasGroup>();
			if (null != component)
			{
				list.Add(component);
			}
			Transform parent = base.transform.parent;
			if (null == parent)
			{
				return;
			}
			while (true)
			{
				component = parent.GetComponent<CanvasGroup>();
				if (null != component)
				{
					list.Add(component);
				}
				if (null == parent.parent)
				{
					break;
				}
				parent = parent.parent;
			}
			canvasGrp = list.ToArray();
		}

		public void ChangeRaycastTarget(bool enable)
		{
			Image[] array = imgRaycast;
			foreach (Image image in array)
			{
				image.raycastTarget = enable;
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
			GetCanvasGroup();
		}

		private void Update()
		{
			if (canvasGrp == null || imgRaycast == null)
			{
				return;
			}
			bool enable = true;
			CanvasGroup[] array = canvasGrp;
			foreach (CanvasGroup canvasGroup in array)
			{
				if (!canvasGroup.blocksRaycasts)
				{
					enable = false;
					break;
				}
			}
			ChangeRaycastTarget(enable);
		}
	}
}
