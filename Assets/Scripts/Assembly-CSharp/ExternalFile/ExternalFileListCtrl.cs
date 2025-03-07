using System;
using System.Collections.Generic;
using System.Linq;
using Localize.Translate;
using UGUI_AssistLibrary;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ExternalFile
{
	public class ExternalFileListCtrl : MonoBehaviour
	{
		public delegate void OnChangeItemFunc(ExternalFileInfo info);

		[SerializeField]
		private ExternalFileWindow cfWindow;

		[SerializeField]
		private Text textDrawName;

		[SerializeField]
		private RectTransform rtfScrollRect;

		[SerializeField]
		private RectTransform rtfContant;

		[SerializeField]
		private GameObject objListContent;

		[SerializeField]
		private GameObject objLineTemp;

		[SerializeField]
		private Button btnSortName;

		[SerializeField]
		private Button btnSortData;

		private string selectDrawName = string.Empty;

		private List<ExternalFileInfo> lstFileInfo = new List<ExternalFileInfo>();

		private bool ascendName;

		private bool ascendData;

		private byte lastSort;

		public OnChangeItemFunc onChangeItemFunc;

		private void Start()
		{
			if ((bool)btnSortData)
			{
				btnSortData.OnClickAsObservable().Subscribe(delegate
				{
					SortDate(!ascendData);
				});
			}
			if ((bool)btnSortName)
			{
				btnSortName.OnClickAsObservable().Subscribe(delegate
				{
					SortName(!ascendName);
				});
			}
		}

		private void Update()
		{
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.gameObject.activeSelf)
				{
					RectTransform rectTransform = lstFileInfo[i].fic.transform as RectTransform;
					float y = rectTransform.anchoredPosition.y;
					float height = rectTransform.rect.height;
					float num = 0f - rtfContant.anchoredPosition.y;
					float num2 = num - rtfScrollRect.rect.height;
					float num3 = y;
					float num4 = y - height;
					if (num4 <= num && num3 >= num2)
					{
						lstFileInfo[i].show = true;
					}
					else
					{
						lstFileInfo[i].show = false;
					}
					lstFileInfo[i].UpdateThumb(lstFileInfo[i].fic.imgThumb);
				}
			}
		}

		private void OnDestroy()
		{
		}

		public void Setup()
		{
		}

		public void SortDate(bool ascend)
		{
			ascendData = ascend;
			if (lstFileInfo.Count == 0)
			{
				return;
			}
			lastSort = 1;
			if (ascend)
			{
				lstFileInfo.Sort((ExternalFileInfo a, ExternalFileInfo b) => a.time.CompareTo(b.time));
			}
			else
			{
				lstFileInfo.Sort((ExternalFileInfo a, ExternalFileInfo b) => b.time.CompareTo(a.time));
			}
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject gameObject = lstFileInfo[i].fic.gameObject;
				if (null != gameObject)
				{
					gameObject.transform.SetSiblingIndex(i);
				}
			}
		}

		public void SortName(bool ascend)
		{
			ascendName = ascend;
			if (lstFileInfo.Count == 0)
			{
				return;
			}
			lastSort = 0;
			Localize.Translate.Manager.SetCulture(delegate
			{
				if (ascend)
				{
					lstFileInfo.Sort((ExternalFileInfo a, ExternalFileInfo b) => a.FileName.CompareTo(b.FileName));
				}
				else
				{
					lstFileInfo.Sort((ExternalFileInfo a, ExternalFileInfo b) => b.FileName.CompareTo(a.FileName));
				}
			});
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject gameObject = lstFileInfo[i].fic.gameObject;
				if (null != gameObject)
				{
					gameObject.transform.SetSiblingIndex(i);
				}
			}
		}

		public void ClearList()
		{
			lstFileInfo.Clear();
		}

		public void AddList(int index, string fullpath, string filename, DateTime time, bool disable = false)
		{
			ExternalFileInfo externalFileInfo = new ExternalFileInfo();
			externalFileInfo.index = index;
			externalFileInfo.FullPath = fullpath;
			externalFileInfo.FileName = filename;
			externalFileInfo.time = time;
			externalFileInfo.disable = disable;
			lstFileInfo.Add(externalFileInfo);
		}

		public void Create(OnChangeItemFunc _onChangeItemFunc)
		{
			onChangeItemFunc = _onChangeItemFunc;
			for (int num = objListContent.transform.childCount - 1; num >= 0; num--)
			{
				Transform child = objListContent.transform.GetChild(num);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			ToggleGroup component = objListContent.GetComponent<ToggleGroup>();
			component.allowSwitchOff = true;
			int num2 = 0;
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objLineTemp);
				ExternalFileInfoComponent component2 = gameObject.GetComponent<ExternalFileInfoComponent>();
				component2.info = lstFileInfo[i];
				component2.info.fic = component2;
				component2.tgl.group = component;
				gameObject.transform.SetParent(objListContent.transform, false);
				SetToggleHandler(gameObject);
				component2.Disvisible(lstFileInfo[i].disvisible);
				component2.Disable(lstFileInfo[i].disable);
				num2++;
			}
			SortName(true);
			SortDate(false);
			ToggleAllOff();
		}

		public void UpdateSort()
		{
			if (lastSort == 0)
			{
				SortDate(ascendData);
				SortName(ascendName);
			}
			else
			{
				SortName(ascendName);
				SortDate(ascendData);
			}
		}

		public ExternalFileInfo GetFileInfoFromIndex(int index)
		{
			return lstFileInfo.Find((ExternalFileInfo item) => item.index == index);
		}

		public ExternalFileInfo GetFileInfoFromName(string name)
		{
			return lstFileInfo.Find((ExternalFileInfo item) => item.FileName == name);
		}

		public string GetNameFormIndex(int index)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.index == index);
			if (externalFileInfo != null)
			{
				return externalFileInfo.FileName;
			}
			return string.Empty;
		}

		public int GetIndexFromName(string name)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.FileName == name);
			if (externalFileInfo != null)
			{
				return externalFileInfo.index;
			}
			return -1;
		}

		public int[] GetSelectIndex()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.tgl.interactable && lstFileInfo[i].fic.gameObject.activeSelf && lstFileInfo[i].fic.tgl.isOn)
				{
					list.Add(lstFileInfo[i].index);
				}
			}
			return list.ToArray();
		}

		public ExternalFileInfoComponent GetSelectTopItem()
		{
			int[] selectIndex = GetSelectIndex();
			if (selectIndex.IsNullOrEmpty())
			{
				return null;
			}
			ExternalFileInfo fileInfoFromIndex = GetFileInfoFromIndex(selectIndex[0]);
			if (fileInfoFromIndex != null)
			{
				return fileInfoFromIndex.fic;
			}
			return null;
		}

		public ExternalFileInfoComponent GetSelectableTopItem()
		{
			SortedDictionary<int, ExternalFileInfoComponent> sortedDictionary = new SortedDictionary<int, ExternalFileInfoComponent>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.tgl.interactable && lstFileInfo[i].fic.gameObject.activeSelf)
				{
					sortedDictionary[lstFileInfo[i].fic.gameObject.transform.GetSiblingIndex()] = lstFileInfo[i].fic;
				}
			}
			ExternalFileInfoComponent result = null;
			if (sortedDictionary.Count != 0)
			{
				result = sortedDictionary.First().Value;
			}
			return result;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			SortedDictionary<int, ExternalFileInfoComponent> sortedDictionary = new SortedDictionary<int, ExternalFileInfoComponent>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.gameObject.activeSelf)
				{
					sortedDictionary[lstFileInfo[i].fic.gameObject.transform.GetSiblingIndex()] = lstFileInfo[i].fic;
				}
			}
			foreach (var item in sortedDictionary.Select((KeyValuePair<int, ExternalFileInfoComponent> val, int idx) => new { val, idx }))
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
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.gameObject.activeSelf)
				{
					list.Add(lstFileInfo[i].index);
				}
			}
			return list.Count;
		}

		public void SelectItem(int index)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.index == index);
			externalFileInfo.fic.tgl.isOn = true;
			ChangeItem(externalFileInfo.fic.gameObject);
		}

		public void SelectItem(string name)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.FileName == name);
			externalFileInfo.fic.tgl.isOn = true;
			ChangeItem(externalFileInfo.fic.gameObject);
		}

		public void OnPointerClick(GameObject obj)
		{
			if (!(null == obj))
			{
				ExternalFileInfoComponent component = obj.GetComponent<ExternalFileInfoComponent>();
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
				ExternalFileInfoComponent component = obj.GetComponent<ExternalFileInfoComponent>();
				if (!(null == component) && component.tgl.interactable && (bool)textDrawName)
				{
					textDrawName.text = component.info.FileName;
				}
			}
		}

		public void OnPointerExit(GameObject obj)
		{
			if (!(null == obj))
			{
				ExternalFileInfoComponent component = obj.GetComponent<ExternalFileInfoComponent>();
				if (!(null == component) && component.tgl.interactable && (bool)textDrawName)
				{
					textDrawName.text = selectDrawName;
				}
			}
		}

		public void ChangeItem(GameObject obj)
		{
			ExternalFileInfoComponent component = obj.GetComponent<ExternalFileInfoComponent>();
			if (onChangeItemFunc != null)
			{
				onChangeItemFunc(component.info);
			}
			selectDrawName = component.info.FileName;
		}

		public void ToggleAllOff()
		{
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				lstFileInfo[i].fic.tgl.isOn = false;
			}
		}

		public void DisableItem(int index, bool disable)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.index == index);
			if (externalFileInfo != null)
			{
				externalFileInfo.disable = disable;
				externalFileInfo.fic.Disable(disable);
			}
		}

		public void DisableItem(string name, bool disable)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.FileName == name);
			externalFileInfo.disable = disable;
			externalFileInfo.fic.Disable(disable);
		}

		public void DisvisibleItem(int index, bool disvisible)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.index == index);
			if (externalFileInfo != null)
			{
				externalFileInfo.disvisible = disvisible;
				externalFileInfo.fic.Disvisible(disvisible);
			}
		}

		public void DisvisibleItem(string name, bool disvisible)
		{
			ExternalFileInfo externalFileInfo = lstFileInfo.Find((ExternalFileInfo item) => item.FileName == name);
			if (externalFileInfo != null)
			{
				externalFileInfo.disvisible = disvisible;
				externalFileInfo.fic.Disvisible(disvisible);
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
	}
}
