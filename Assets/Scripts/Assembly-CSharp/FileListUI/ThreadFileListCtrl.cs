using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Localize.Translate;
using UGUI_AssistLibrary;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FileListUI
{
	public abstract class ThreadFileListCtrl<Info, InfoComponent> : MonoBehaviour where Info : ThreadFileInfo where InfoComponent : ThreadFileInfoComponent
	{
		private List<Image> _imgRaycast = new List<Image>();

		[SerializeField]
		protected RectTransform rtfScrollRect;

		[SerializeField]
		protected RectTransform rtfContant;

		[SerializeField]
		protected GameObject objListContent;

		[SerializeField]
		protected GameObject objLineTemp;

		[SerializeField]
		private bool multiSelect;

		[Tooltip("OFFだと最低でも一つは選択されてる必要がある")]
		[SerializeField]
		private bool allowSwitchOff = true;

		[SerializeField]
		private Text textDrawName;

		[SerializeField]
		private Button btnSortName;

		[SerializeField]
		private Button btnSortData;

		[SerializeField]
		private TypeReactiveProperty _visibleType = new TypeReactiveProperty(VisibleType.AllDisplay);

		private List<Info> _lstFileInfo = new List<Info>();

		private Selectable[] _baseInfos;

		protected Selectable[] _addInfos;

		protected Selectable[] _allInfos;

		protected bool ascendName;

		protected bool ascendData;

		public Action<Info> onChangeItem;

		protected string selectDrawName = string.Empty;

		private StringReactiveProperty selName = new StringReactiveProperty(string.Empty);

		private Transform _cloneParent;

		private ToggleGroup _tglGrp;

		public CanvasGroup[] canvasGrp { get; private set; }

		public List<Image> imgRaycast
		{
			get
			{
				return _imgRaycast;
			}
		}

		public VisibleType visibleType
		{
			get
			{
				return _visibleType.Value;
			}
			set
			{
				_visibleType.Value = value;
			}
		}

		protected List<Info> lstFileInfo
		{
			get
			{
				return _lstFileInfo;
			}
		}

		protected Selectable[] baseInfos
		{
			get
			{
				return this.GetCache(ref _baseInfos, () => new Selectable[2] { btnSortName, btnSortData }.Where((Selectable p) => p != null).ToArray());
			}
		}

		protected abstract Selectable[] addInfos { get; }

		protected Selectable[] allInfos
		{
			get
			{
				return this.GetCache(ref _allInfos, () => baseInfos.Concat(addInfos).ToArray());
			}
		}

		protected byte lastSort { get; private set; }

		private Transform cloneParent
		{
			get
			{
				return this.GetCache(ref _cloneParent, () => objListContent.transform);
			}
		}

		private ToggleGroup tglGrp
		{
			get
			{
				return this.GetCacheObject(ref _tglGrp, delegate
				{
					ToggleGroup component = objListContent.GetComponent<ToggleGroup>();
					component.allowSwitchOff = allowSwitchOff;
					return component;
				});
			}
		}

		public void ChangeRaycastTarget(bool enable)
		{
			foreach (Image item in imgRaycast)
			{
				item.raycastTarget = enable;
			}
		}

		private void DisableInfo(bool isAdd, bool disable)
		{
			Selectable[] array = ((!isAdd) ? allInfos : addInfos);
			foreach (Selectable selectable in array)
			{
				selectable.interactable = !disable;
			}
		}

		private void HideInfo(bool isAdd, bool hide)
		{
			Selectable[] array = ((!isAdd) ? allInfos : addInfos);
			foreach (Selectable selectable in array)
			{
				selectable.gameObject.SetActiveIfDifferent(!hide);
			}
		}

		protected virtual void Awake()
		{
			canvasGrp = new CanvasGroup[0];
		}

		protected virtual void Start()
		{
			_visibleType.Subscribe(delegate(VisibleType type)
			{
				bool isAdd = false;
				bool isAdd2 = true;
				bool flag = true;
				bool flag2 = false;
				switch (type)
				{
				case VisibleType.AllDisplay:
					HideInfo(isAdd, flag2);
					break;
				case VisibleType.AddDisabled:
					DisableInfo(isAdd2, flag);
					break;
				case VisibleType.AddHide:
					HideInfo(isAdd2, flag);
					break;
				case VisibleType.AllDisabled:
					DisableInfo(isAdd, flag);
					break;
				case VisibleType.AllHide:
					HideInfo(isAdd, flag);
					break;
				case VisibleType.AddEnabled:
					DisableInfo(isAdd2, flag2);
					break;
				case VisibleType.AllEnabled:
					DisableInfo(isAdd, flag2);
					break;
				case VisibleType.AddDisplay:
					HideInfo(isAdd2, flag2);
					break;
				}
			});
			if (textDrawName != null)
			{
				selName.SubscribeToText(textDrawName);
			}
			if (btnSortData != null)
			{
				btnSortData.OnClickAsObservable().Subscribe(delegate
				{
					SortDate(!ascendData);
				});
			}
			if (btnSortName != null)
			{
				btnSortName.OnClickAsObservable().Subscribe(delegate
				{
					SortName(!ascendName);
				});
			}
			canvasGrp = GetCanvasGroup();
		}

		protected virtual void Update()
		{
			float num = 0f - rtfContant.anchoredPosition.y;
			float num2 = num - rtfScrollRect.rect.height;
			foreach (Info item in lstFileInfo.Where((Info p) => p.component.gameObject.activeSelf))
			{
				Info current = item;
				RectTransform rectTransform = current.component.rectTransform;
				float y = rectTransform.anchoredPosition.y;
				float height = rectTransform.rect.height;
				float num3 = y;
				float num4 = y - height;
				current.show = num4 <= num && num3 >= num2;
				current.UpdateThumb(current.component.imgThumb);
			}
			ChangeRaycastTarget(canvasGrp.All((CanvasGroup p) => p.blocksRaycasts));
		}

		protected virtual void OnDestroy()
		{
			foreach (Info item in lstFileInfo)
			{
				Info current = item;
				current.DeleteThumb();
			}
		}

		private CanvasGroup[] GetCanvasGroup()
		{
			List<CanvasGroup> list = new List<CanvasGroup>();
			Func<Transform, Transform> func = delegate(Transform t)
			{
				t.GetComponent<CanvasGroup>().SafeProcObject(delegate(CanvasGroup cgrp)
				{
					list.Add(cgrp);
				});
				return t.parent;
			};
			Transform arg = base.transform;
			while ((arg = func(arg)) != null)
			{
			}
			return list.ToArray();
		}

		public virtual void UpdateSort()
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

		public virtual void Sort(byte type, bool ascend, out bool ascendData, Action sort)
		{
			ascendData = ascend;
			if (lstFileInfo.Any())
			{
				lastSort = type;
				sort();
				ListForSortByObject();
			}
		}

		public void SortDate(bool ascend)
		{
			Sort(1, ascend, out ascendData, SortDate);
		}

		public void SortName(bool ascend)
		{
			Sort(0, ascend, out ascendName, SortName);
		}

		private void SortDate()
		{
			if (ascendData)
			{
				lstFileInfo.Sort((Info a, Info b) => a.time.CompareTo(b.time));
			}
			else
			{
				lstFileInfo.Sort((Info a, Info b) => b.time.CompareTo(a.time));
			}
		}

		private void SortName()
		{
			Localize.Translate.Manager.SetCulture(delegate
			{
				if (ascendName)
				{
					lstFileInfo.Sort((Info a, Info b) => a.name.CompareTo(b.name));
				}
				else
				{
					lstFileInfo.Sort((Info a, Info b) => b.name.CompareTo(a.name));
				}
			});
		}

		private void ListForSortByObject()
		{
			foreach (Info item in lstFileInfo.Where((Info p) => p.component != null))
			{
				Info current = item;
				current.component.rectTransform.SetAsLastSibling();
			}
		}

		public void ClearList()
		{
			lstFileInfo.Clear();
		}

		public void AddList(Info info)
		{
			lstFileInfo.Add(info);
		}

		public void RemoveList(int index)
		{
			Info fileInfoFromIndex = GetFileInfoFromIndex(index);
			fileInfoFromIndex.DeleteThumb();
			lstFileInfo.Remove(fileInfoFromIndex);
		}

		public virtual void Add(Info info)
		{
			AddList(info);
			CreateCloneBinding(info);
		}

		protected void CreateCloneBinding(Info info)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(objLineTemp, cloneParent, false);
			Image image = FindThumbBG(gameObject.transform);
			if (image != null)
			{
				imgRaycast.Add(image);
			}
			InfoComponent component = gameObject.GetComponent<InfoComponent>();
			component.SetInfo(info);
			if (!multiSelect)
			{
				component.tgl.group = tglGrp;
			}
			SetToggleHandler(component);
		}

		private static Image FindThumbBG(Transform transform)
		{
			Transform transform2 = transform.Find("ThumbBG");
			return (!(transform2 == null)) ? transform2.GetComponent<Image>() : null;
		}

		public void Delete(int index)
		{
			Info info = GetFileInfoFromIndex(index);
			info.DeleteThumb();
			lstFileInfo.Remove(info);
			info.component.tgl.isOn = false;
			List<GameObject> list = objListContent.Children();
			GameObject gameObject = list.Find((GameObject item) => item.GetComponent<InfoComponent>() == info.component);
			if (gameObject != null)
			{
				imgRaycast.Remove(FindThumbBG(gameObject.transform));
				UnityEngine.Object.Destroy(gameObject);
			}
		}

		public void ReCreate()
		{
			Create(onChangeItem, true);
		}

		public abstract void Create(Action<Info> onChangeItem, bool reCreate = false);

		public Info GetFileInfoFromIndex(int index)
		{
			return lstFileInfo.Find((Info item) => item.index == index);
		}

		public Info GetFileInfoFromName(string name)
		{
			return lstFileInfo.Find((Info item) => item.name == name);
		}

		public string GetNameFormIndex(int index)
		{
			Info val = lstFileInfo.Find((Info item) => item.index == index);
			return (val != null) ? val.name : string.Empty;
		}

		public int GetIndexFromName(string name)
		{
			Info val = lstFileInfo.Find((Info item) => item.name == name);
			return (val != null) ? val.index : (-1);
		}

		public int[] GetSelectIndex()
		{
			return (from p in lstFileInfo
				where p.component.tgl.interactable
				where p.component.gameObject.activeSelf
				where p.component.tgl.isOn
				select p.index).ToArray();
		}

		public int GetNoUseIndex()
		{
			int[] array = (from info in lstFileInfo
				select info.index into idx
				orderby idx
				select idx).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != i)
				{
					return i;
				}
			}
			return array.Length;
		}

		public InfoComponent GetSelectTopItem()
		{
			int[] selectIndex = GetSelectIndex();
			if (!selectIndex.Any())
			{
				return (InfoComponent)null;
			}
			Info fileInfoFromIndex = GetFileInfoFromIndex(selectIndex[0]);
			return (fileInfoFromIndex != null) ? (fileInfoFromIndex.component as InfoComponent) : ((InfoComponent)null);
		}

		public InfoComponent GetSelectableTopItem()
		{
			return (from p in lstFileInfo
				select p.component into fic
				where fic.tgl.interactable
				where fic.gameObject.activeSelf
				orderby fic.rectTransform.GetSiblingIndex()
				select fic).FirstOrDefault() as InfoComponent;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			var anon = (from p in lstFileInfo
				select p.component into fic
				where fic.gameObject.activeSelf
				orderby fic.rectTransform.GetSiblingIndex()
				select fic).Select((ThreadFileInfoComponent fic, int i) => new { fic, i }).FirstOrDefault(p => p.fic.fileInfo.index == index);
			return (anon != null) ? anon.i : (-1);
		}

		public int GetInclusiveCount()
		{
			return lstFileInfo.Count((Info p) => p.component.gameObject.activeSelf);
		}

		public void SelectItem(int index)
		{
			Info val = lstFileInfo.Find((Info item) => item.index == index);
			if (val != null)
			{
				SelectItem(val);
			}
		}

		public void SelectItem(string name)
		{
			Info val = lstFileInfo.Find((Info item) => item.name == name);
			if (val != null)
			{
				SelectItem(val);
			}
		}

		public void SelectItem(Info info)
		{
			info.component.tgl.isOn = true;
			ChangeItem(info.component as InfoComponent);
			UpdateScrollPosition();
		}

		public void UpdateScrollPosition()
		{
		}

		public virtual bool OnPointerClick(InfoComponent fic)
		{
			if (!CheckToggle(fic))
			{
				return false;
			}
			ChangeItem(fic);
			return true;
		}

		public virtual bool OnPointerEnter(InfoComponent fic)
		{
			if (!CheckToggle(fic))
			{
				return false;
			}
			selName.Value = fic.fileInfo.name;
			return true;
		}

		public virtual bool OnPointerExit(InfoComponent fic)
		{
			if (!CheckToggle(fic))
			{
				return false;
			}
			selName.Value = selectDrawName;
			return true;
		}

		public virtual void ChangeItem(InfoComponent fic)
		{
			onChangeItem.Call(fic.fileInfo as Info);
			selectDrawName = (fic.tgl.isOn ? fic.fileInfo.name : string.Empty);
		}

		public void ToggleAllOff()
		{
			lstFileInfo.ForEach(delegate(Info item)
			{
				item.component.OFF();
			});
		}

		public void ToggleAllOn()
		{
			lstFileInfo.ForEach(delegate(Info item)
			{
				item.component.ON();
			});
		}

		public void DisableItem(int index, bool disable)
		{
			DisableItem(lstFileInfo.Find((Info item) => item.index == index), disable);
		}

		public void DisableItem(string name, bool disable)
		{
			DisableItem(lstFileInfo.Find((Info item) => item.name == name), disable);
		}

		public void DisvisibleItem(int index, bool disvisible)
		{
			DisvisibleItem(lstFileInfo.Find((Info item) => item.index == index), disvisible);
		}

		public void DisvisibleItem(string name, bool disvisible)
		{
			DisvisibleItem(lstFileInfo.Find((Info item) => item.name == name), disvisible);
		}

		private void SetToggleHandler(InfoComponent fic)
		{
			UIAL_EventTrigger uIAL_EventTrigger = fic.gameObject.AddComponent<UIAL_EventTrigger>();
			uIAL_EventTrigger.triggers = new List<UIAL_EventTrigger.Entry>();
			UIAL_EventTrigger.Entry entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.buttonType = UIAL_EventTrigger.ButtonType.Left;
			entry.callback.AddListener(delegate
			{
				OnPointerClick(fic);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			entry.callback.AddListener(delegate
			{
				OnPointerEnter(fic);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerExit;
			entry.callback.AddListener(delegate
			{
				OnPointerExit(fic);
			});
			uIAL_EventTrigger.triggers.Add(entry);
		}

		private static bool CheckToggle(InfoComponent fic)
		{
			if (fic == null)
			{
				return false;
			}
			if (!fic.tgl.interactable)
			{
				return false;
			}
			return true;
		}

		private static void DisableItem(Info fi, bool disable)
		{
			if (fi != null)
			{
				fi.disable = disable;
				fi.component.Disable(disable);
			}
		}

		private static void DisvisibleItem(Info fi, bool disvisible)
		{
			if (fi != null)
			{
				fi.disvisible = disvisible;
				fi.component.Disvisible(disvisible);
			}
		}
	}
}
