using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using IllusionUtility.GetUtility;
using Localize.Translate;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UGUI_AssistLibrary
{
	public class UIAL_ListCtrl : MonoBehaviour
	{
		public class FileInfo
		{
			public int index;

			public string FullPath = string.Empty;

			public string FileName = string.Empty;

			public DateTime time;

			public string name = string.Empty;

			public bool disable;

			public bool disvisible;

			public FileInfoComponent fic;
		}

		public class FileInfoComponent : MonoBehaviour
		{
			public FileInfo info;

			public GameObject obj;

			public Toggle tgl;

			public Image imgBG;

			public Image imgCheck;

			public Text text;

			public float disableTextAlpha = 0.6f;

			public void ChangeTextColor(Color color)
			{
				if ((bool)text)
				{
					text.color = color;
				}
			}

			public void ChangeTextAlpha(float alpha)
			{
				if ((bool)text)
				{
					text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
				}
			}

			public void SetDisableTextAlpha(float alpha)
			{
				disableTextAlpha = alpha;
				if ((bool)text && (bool)tgl && !tgl.interactable)
				{
					ChangeTextAlpha(alpha);
				}
			}

			public void ChangeBackgroundImageColor(Color color)
			{
				if ((bool)imgBG)
				{
					imgBG.color = color;
				}
			}

			public void ChangeCheckImageColor(Color color)
			{
				if ((bool)imgCheck)
				{
					imgCheck.color = color;
				}
			}

			public void Enable()
			{
				if ((bool)tgl)
				{
					tgl.interactable = true;
				}
				ChangeTextAlpha(1f);
			}

			public void Disable()
			{
				if ((bool)tgl)
				{
					tgl.interactable = false;
				}
				ChangeTextAlpha(disableTextAlpha);
			}

			public void Visible()
			{
				if ((bool)obj)
				{
					obj.SetActiveIfDifferent(true);
				}
			}

			public void Disvisible()
			{
				if ((bool)obj)
				{
					obj.SetActiveIfDifferent(false);
				}
			}
		}

		public delegate void OnChangeItemFunc(FileInfo info);

		public VerticalLayoutGroup vlgroup;

		public LayoutElement layoutElement;

		public RectTransform rtfScrollRect;

		public GameObject objListContent;

		public GameObject objLineBase;

		public Dropdown ddKind;

		public Image imgPrev;

		public bool multiSelect;

		[Tooltip("OFFだと最低でも一つは選択されてる必要がある")]
		public bool allowSwitchOff = true;

		[Header("MultiSelectにチェックを入れた場合はPrevTypeは1にしてください")]
		[Tooltip("0:選択されてたら表示,1:カーソルが乗ったら表示")]
		public byte prevType;

		[Tooltip("ドロップダウンの全ファイル表示する時の項目名")]
		public string dropdownAllOptName = "全て表示";

		[Tooltip("ドロップダウンでタグ指定なしの時の項目名")]
		public string dropdownNotOptName = "未分類";

		private string newPrevPath = string.Empty;

		private string oldPrevPath = string.Empty;

		private List<int> lstNotOpt = new List<int>();

		private Dictionary<string, List<int>> dictKind = new Dictionary<string, List<int>>();

		private List<FileInfo> lstFileInfo = new List<FileInfo>();

		private bool ascendName;

		private bool ascendData;

		private byte lastSort;

		public OnChangeItemFunc onChangeItemFunc;

		private void Update()
		{
			if (prevType != 1 || newPrevPath.IsNullOrEmpty() || !(oldPrevPath != newPrevPath))
			{
				return;
			}
			Sprite sprite = PngAssist.LoadSpriteFromFile(newPrevPath, 0, 0, new Vector2(0.5f, 0.5f));
			if ((bool)sprite && (bool)imgPrev)
			{
				if ((bool)imgPrev.sprite)
				{
					if ((bool)imgPrev.sprite.texture)
					{
						UnityEngine.Object.Destroy(imgPrev.sprite.texture);
					}
					UnityEngine.Object.Destroy(imgPrev.sprite);
				}
				imgPrev.sprite = sprite;
			}
			oldPrevPath = newPrevPath;
		}

		private void OnDestroy()
		{
			ReleasePrev();
		}

		private void ReleasePrev()
		{
			if ((bool)imgPrev && (bool)imgPrev.sprite)
			{
				if ((bool)imgPrev.sprite.texture)
				{
					UnityEngine.Object.Destroy(imgPrev.sprite.texture);
				}
				UnityEngine.Object.Destroy(imgPrev.sprite);
			}
		}

		public void ClearList()
		{
			lstFileInfo.Clear();
		}

		public void AddList(int index, string name, string fullpath, string filename, DateTime time, bool disable = false, bool disvisible = false)
		{
			FileInfo fileInfo = new FileInfo();
			fileInfo.index = index;
			fileInfo.name = name;
			fileInfo.FullPath = fullpath;
			fileInfo.FileName = filename;
			fileInfo.time = time;
			fileInfo.disable = disable;
			fileInfo.disvisible = disvisible;
			lstFileInfo.Add(fileInfo);
		}

		public void ClearKind()
		{
			dictKind.Clear();
			lstNotOpt.Clear();
		}

		public void AddKind(string[] kind, int index)
		{
			List<int> value = null;
			int num = 0;
			foreach (string text in kind)
			{
				if (!text.IsNullOrEmpty())
				{
					if (!dictKind.TryGetValue(text, out value))
					{
						value = new List<int>();
						dictKind[text] = value;
					}
					value.Add(index);
					num++;
				}
			}
			if (num == 0)
			{
				lstNotOpt.Add(index);
			}
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
			component.allowSwitchOff = allowSwitchOff;
			int num2 = 0;
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objLineBase);
				FileInfoComponent fileInfoComponent = gameObject.AddComponent<FileInfoComponent>();
				fileInfoComponent.info = lstFileInfo[i];
				fileInfoComponent.info.fic = fileInfoComponent;
				fileInfoComponent.obj = gameObject;
				GameObject gameObject2 = null;
				fileInfoComponent.tgl = gameObject.GetComponent<Toggle>();
				gameObject2 = gameObject.transform.FindLoop("Background");
				if ((bool)gameObject2)
				{
					fileInfoComponent.imgBG = gameObject2.GetComponent<Image>();
				}
				gameObject2 = gameObject.transform.FindLoop("Checkmark");
				if ((bool)gameObject2)
				{
					fileInfoComponent.imgCheck = gameObject2.GetComponent<Image>();
				}
				gameObject2 = gameObject.transform.FindLoop("Label");
				if ((bool)gameObject2)
				{
					fileInfoComponent.text = gameObject2.GetComponent<Text>();
					fileInfoComponent.text.text = fileInfoComponent.info.name;
				}
				if (!multiSelect)
				{
					fileInfoComponent.tgl.group = component;
				}
				gameObject.transform.SetParent(objListContent.transform, false);
				SetToggleHandler(gameObject);
				if (lstFileInfo[i].disvisible)
				{
					fileInfoComponent.Disvisible();
				}
				else
				{
					fileInfoComponent.Visible();
				}
				if (lstFileInfo[i].disable)
				{
					fileInfoComponent.Disable();
				}
				else
				{
					fileInfoComponent.Enable();
				}
				num2++;
			}
			if ((bool)ddKind)
			{
				ddKind.ClearOptions();
				List<string> list = new List<string>();
				list.Add(dropdownAllOptName);
				if (dictKind.Count != 0)
				{
					if (lstNotOpt.Count != 0)
					{
						list.Add(dropdownNotOptName);
					}
					foreach (KeyValuePair<string, List<int>> item in dictKind)
					{
						list.Add(item.Key);
					}
				}
				ddKind.AddOptions(list);
			}
			SortName(true);
			SortDate(false);
			if ((bool)imgPrev)
			{
				imgPrev.enabled = false;
			}
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

		public void OnSortName()
		{
			SortName(!ascendName);
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
					lstFileInfo.Sort((FileInfo a, FileInfo b) => a.name.CompareTo(b.name));
				}
				else
				{
					lstFileInfo.Sort((FileInfo a, FileInfo b) => b.name.CompareTo(a.name));
				}
			});
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject obj = lstFileInfo[i].fic.obj;
				if (null != obj)
				{
					obj.transform.SetSiblingIndex(i);
				}
			}
		}

		public void OnSortDate()
		{
			SortDate(!ascendData);
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
				lstFileInfo.Sort((FileInfo a, FileInfo b) => a.time.CompareTo(b.time));
			}
			else
			{
				lstFileInfo.Sort((FileInfo a, FileInfo b) => b.time.CompareTo(a.time));
			}
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				GameObject obj = lstFileInfo[i].fic.obj;
				if (null != obj)
				{
					obj.transform.SetSiblingIndex(i);
				}
			}
		}

		public FileInfo GetFileInfoFromIndex(int index)
		{
			return lstFileInfo.Find((FileInfo item) => item.index == index);
		}

		public FileInfo GetFileInfoFromName(string name)
		{
			return lstFileInfo.Find((FileInfo item) => item.name == name);
		}

		public string GetNameFormIndex(int index)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.index == index);
			if (fileInfo != null)
			{
				return fileInfo.name;
			}
			return string.Empty;
		}

		public int GetIndexFromName(string name)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.name == name);
			if (fileInfo != null)
			{
				return fileInfo.index;
			}
			return -1;
		}

		public int[] GetSelectIndex()
		{
			List<int> list = new List<int>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.tgl.interactable && lstFileInfo[i].fic.obj.activeSelf && lstFileInfo[i].fic.tgl.isOn)
				{
					list.Add(lstFileInfo[i].index);
				}
			}
			return list.ToArray();
		}

		public FileInfoComponent GetSelectTopItem()
		{
			int[] selectIndex = GetSelectIndex();
			if (selectIndex.IsNullOrEmpty())
			{
				return null;
			}
			FileInfo fileInfoFromIndex = GetFileInfoFromIndex(selectIndex[0]);
			if (fileInfoFromIndex != null)
			{
				return fileInfoFromIndex.fic;
			}
			return null;
		}

		public FileInfoComponent GetSelectableTopItem()
		{
			SortedDictionary<int, FileInfoComponent> sortedDictionary = new SortedDictionary<int, FileInfoComponent>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.tgl.interactable && lstFileInfo[i].fic.obj.activeSelf)
				{
					sortedDictionary[lstFileInfo[i].fic.obj.transform.GetSiblingIndex()] = lstFileInfo[i].fic;
				}
			}
			FileInfoComponent result = null;
			if (sortedDictionary.Count != 0)
			{
				result = sortedDictionary.First().Value;
			}
			return result;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			SortedDictionary<int, FileInfoComponent> sortedDictionary = new SortedDictionary<int, FileInfoComponent>();
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				if (lstFileInfo[i].fic.obj.activeSelf)
				{
					sortedDictionary[lstFileInfo[i].fic.obj.transform.GetSiblingIndex()] = lstFileInfo[i].fic;
				}
			}
			foreach (var item in sortedDictionary.Select((KeyValuePair<int, FileInfoComponent> val, int idx) => new { val, idx }))
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
				if (lstFileInfo[i].fic.obj.activeSelf)
				{
					list.Add(lstFileInfo[i].index);
				}
			}
			return list.Count;
		}

		public void SelectItem(int index)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.index == index);
			fileInfo.fic.tgl.isOn = true;
			ChangeItem(fileInfo.fic.obj);
			UpdateScrollPosition();
		}

		public void SelectItem(string name)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.name == name);
			fileInfo.fic.tgl.isOn = true;
			ChangeItem(fileInfo.fic.obj);
			UpdateScrollPosition();
		}

		public void UpdateScrollPosition()
		{
			FileInfoComponent selectTopItem = GetSelectTopItem();
			if (!(null == selectTopItem))
			{
				int drawOrderFromIndex = GetDrawOrderFromIndex(selectTopItem.info.index);
				float num = layoutElement.minHeight + vlgroup.spacing;
				RectTransform rectTransform = objListContent.transform as RectTransform;
				RectTransform rectTransform2 = rtfScrollRect.parent as RectTransform;
				float num2 = rectTransform2.sizeDelta.y + rtfScrollRect.sizeDelta.y;
				int inclusiveCount = GetInclusiveCount();
				float b = num * (float)inclusiveCount + (float)vlgroup.padding.top + (float)vlgroup.padding.bottom - vlgroup.spacing - num2;
				float y = Mathf.Min((float)drawOrderFromIndex * num, b);
				rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);
			}
		}

		public void OnPointerClick(GameObject obj)
		{
			if (!(null == obj))
			{
				FileInfoComponent component = obj.GetComponent<FileInfoComponent>();
				if (!(null == component) && component.tgl.interactable)
				{
					ChangeItem(obj);
				}
			}
		}

		public void ChangeItem(GameObject obj)
		{
			FileInfoComponent component = obj.GetComponent<FileInfoComponent>();
			if (onChangeItemFunc != null)
			{
				onChangeItemFunc(component.info);
			}
			if (prevType == 0)
			{
				ChangeDrawImage(component);
			}
		}

		public void OnPointerEnter(GameObject obj)
		{
			if (!(null == obj))
			{
				FileInfoComponent component = obj.GetComponent<FileInfoComponent>();
				if (!(null == component) && component.tgl.interactable)
				{
					newPrevPath = component.info.FullPath;
				}
			}
		}

		public void OnPointerEnterDrawImage(GameObject obj)
		{
			if (!(null == obj))
			{
				FileInfoComponent component = obj.GetComponent<FileInfoComponent>();
				if (!(null == component) && component.tgl.interactable && (bool)imgPrev)
				{
					imgPrev.enabled = true;
				}
			}
		}

		public void OnPointerExitDrawImage(GameObject obj)
		{
			if (!(null == obj))
			{
				FileInfoComponent component = obj.GetComponent<FileInfoComponent>();
				if (!(null == component) && component.tgl.interactable && (bool)imgPrev)
				{
					imgPrev.enabled = false;
				}
			}
		}

		public void ToggleAllOff()
		{
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				lstFileInfo[i].fic.tgl.isOn = false;
			}
		}

		public void ToggleAllOn()
		{
			for (int i = 0; i < lstFileInfo.Count; i++)
			{
				lstFileInfo[i].fic.tgl.isOn = true;
			}
		}

		public void ChangeDrawImage(FileInfoComponent fic)
		{
			if (null == imgPrev)
			{
				return;
			}
			imgPrev.enabled = false;
			if (fic.tgl.isOn)
			{
				ReleasePrev();
				Sprite sprite = PngAssist.LoadSpriteFromFile(fic.info.FullPath);
				if ((bool)sprite)
				{
					imgPrev.sprite = sprite;
					imgPrev.enabled = true;
				}
			}
		}

		public void UpdateDrawImage()
		{
			if (null == imgPrev)
			{
				return;
			}
			imgPrev.enabled = false;
			string text = string.Empty;
			for (int num = lstFileInfo.Count - 1; num >= 0; num--)
			{
				if (lstFileInfo[num].fic.tgl.isOn)
				{
					text = lstFileInfo[num].FullPath;
					break;
				}
			}
			if (string.Empty != text)
			{
				ReleasePrev();
				Sprite sprite = PngAssist.LoadSpriteFromFile(text);
				if ((bool)sprite)
				{
					imgPrev.sprite = sprite;
					imgPrev.enabled = true;
				}
			}
		}

		public void DisableItem(int index, bool disable)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.index == index);
			if (fileInfo != null)
			{
				fileInfo.disable = disable;
				if (disable)
				{
					fileInfo.fic.Disable();
				}
				else
				{
					fileInfo.fic.Enable();
				}
			}
		}

		public void DisableItem(string name, bool disable)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.name == name);
			fileInfo.disable = disable;
			if (disable)
			{
				fileInfo.fic.Disable();
			}
			else
			{
				fileInfo.fic.Enable();
			}
		}

		public void DisvisibleItem(int index, bool disvisible)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.index == index);
			if (fileInfo != null)
			{
				fileInfo.disvisible = disvisible;
				if (disvisible)
				{
					fileInfo.fic.Disvisible();
				}
				else
				{
					fileInfo.fic.Visible();
				}
			}
		}

		public void DisvisibleItem(string name, bool disvisible)
		{
			FileInfo fileInfo = lstFileInfo.Find((FileInfo item) => item.name == name);
			if (fileInfo != null)
			{
				fileInfo.disvisible = disvisible;
				if (disvisible)
				{
					fileInfo.fic.Disvisible();
				}
				else
				{
					fileInfo.fic.Visible();
				}
			}
		}

		public void OnChangeListKind(int no)
		{
			if (null == ddKind)
			{
				return;
			}
			if (no == 0)
			{
				for (int i = 0; i < lstFileInfo.Count; i++)
				{
					if (!lstFileInfo[i].disvisible)
					{
						lstFileInfo[i].fic.obj.SetActiveIfDifferent(true);
					}
				}
				return;
			}
			string empty = string.Empty;
			List<int> value = null;
			if (no == 1 && lstNotOpt.Count != 0)
			{
				empty = dropdownNotOptName;
				value = lstNotOpt;
			}
			else
			{
				empty = ddKind.options[no].text;
				if (!dictKind.TryGetValue(empty, out value))
				{
					return;
				}
			}
			for (int j = 0; j < lstFileInfo.Count; j++)
			{
				int index = lstFileInfo[j].index;
				bool flag = false;
				if (!lstFileInfo[j].disvisible)
				{
					flag = value.Contains(index);
				}
				lstFileInfo[j].fic.obj.SetActiveIfDifferent(flag);
				if (!flag && lstFileInfo[j].fic.tgl.isOn)
				{
					lstFileInfo[j].fic.tgl.isOn = false;
					if (prevType == 0)
					{
						ChangeDrawImage(lstFileInfo[j].fic);
					}
				}
			}
			if (allowSwitchOff)
			{
				return;
			}
			int[] selectIndex = GetSelectIndex();
			if (selectIndex.IsNullOrEmpty())
			{
				FileInfoComponent selectableTopItem = GetSelectableTopItem();
				if ((bool)selectableTopItem)
				{
					SelectItem(selectableTopItem.info.index);
				}
			}
		}

		private void SetToggleHandler(GameObject obj)
		{
			UIAL_EventTrigger uIAL_EventTrigger = obj.AddComponent<UIAL_EventTrigger>();
			uIAL_EventTrigger.triggers = new List<UIAL_EventTrigger.Entry>();
			UIAL_EventTrigger.Entry entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(delegate
			{
				OnPointerClick(obj);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			if (prevType == 1)
			{
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate
				{
					OnPointerEnter(obj);
					OnPointerEnterDrawImage(obj);
				});
				uIAL_EventTrigger.triggers.Add(entry);
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate
				{
					OnPointerExitDrawImage(obj);
				});
				uIAL_EventTrigger.triggers.Add(entry);
			}
		}
	}
}
