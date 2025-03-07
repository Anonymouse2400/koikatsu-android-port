using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UGUI_AssistLibrary;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public class PatternSelectListCtrl : MonoBehaviour
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

		private List<PatternSelectInfo> _lstSelectInfo = new List<PatternSelectInfo>();

		public OnChangeItemFunc onChangeItemFunc;

		public List<PatternSelectInfo> lstSelectInfo
		{
			get
			{
				return _lstSelectInfo;
			}
		}

		public void ClearList()
		{
			_lstSelectInfo.Clear();
		}

		public void AddList(int index, string name, string assetBundle, string assetName)
		{
			PatternSelectInfo patternSelectInfo = new PatternSelectInfo();
			patternSelectInfo.index = index;
			patternSelectInfo.name = name;
			patternSelectInfo.assetBundle = assetBundle;
			patternSelectInfo.assetName = assetName;
			_lstSelectInfo.Add(patternSelectInfo);
		}

		public void AddOutside(int _start)
		{
			List<string> list = Directory.GetFiles(UserData.Create("pattern_thumb"), "*.png").Select(Path.GetFileName).ToList();
			for (int i = 0; i < list.Count; i++)
			{
				AddList(_start + i, Path.GetFileNameWithoutExtension(list[i]), string.Empty, list[i]);
			}
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
			for (int i = 0; i < _lstSelectInfo.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(objTemp);
				PatternSelectInfoComponent component2 = gameObject.GetComponent<PatternSelectInfoComponent>();
				component2.info = _lstSelectInfo[i];
				component2.info.sic = component2;
				component2.tgl.group = component;
				gameObject.transform.SetParent(objContent.transform, false);
				SetToggleHandler(gameObject, component2);
				component2.img = gameObject.GetComponent<Image>();
				if ((bool)component2.img)
				{
					Texture2D texture2D = null;
					if (_lstSelectInfo[i].assetBundle.IsNullOrEmpty())
					{
						string path = UserData.Path + "pattern_thumb/" + _lstSelectInfo[i].assetName;
						texture2D = PngAssist.LoadTexture(path);
					}
					else
					{
						texture2D = CommonLib.LoadAsset<Texture2D>(_lstSelectInfo[i].assetBundle, _lstSelectInfo[i].assetName, false, string.Empty);
					}
					if ((bool)texture2D)
					{
						component2.img.sprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
					}
				}
				component2.Disvisible(_lstSelectInfo[i].disvisible);
				component2.Disable(_lstSelectInfo[i].disable);
				num2++;
			}
			ToggleAllOff();
		}

		public PatternSelectInfo GetSelectInfoFromIndex(int index)
		{
			return _lstSelectInfo.Find((PatternSelectInfo item) => item.index == index);
		}

		public PatternSelectInfo GetSelectInfoFromName(string name)
		{
			return _lstSelectInfo.Find((PatternSelectInfo item) => item.name == name);
		}

		public string GetNameFormIndex(int index)
		{
			PatternSelectInfo patternSelectInfo = _lstSelectInfo.Find((PatternSelectInfo item) => item.index == index);
			return (patternSelectInfo == null) ? string.Empty : patternSelectInfo.name;
		}

		public int GetIndexFromName(string name)
		{
			PatternSelectInfo patternSelectInfo = _lstSelectInfo.Find((PatternSelectInfo item) => item.name == name);
			return (patternSelectInfo == null) ? (-1) : patternSelectInfo.index;
		}

		public int GetSelectIndex()
		{
			PatternSelectInfo patternSelectInfo = _lstSelectInfo.Find((PatternSelectInfo psi) => psi.interactable & psi.activeSelf & psi.isOn);
			return (patternSelectInfo == null) ? (-1) : patternSelectInfo.index;
		}

		public PatternSelectInfoComponent GetSelectTopItem()
		{
			int selectIndex = GetSelectIndex();
			if (selectIndex == -1)
			{
				return null;
			}
			PatternSelectInfo selectInfoFromIndex = GetSelectInfoFromIndex(selectIndex);
			if (selectInfoFromIndex != null)
			{
				return selectInfoFromIndex.sic;
			}
			return null;
		}

		public PatternSelectInfoComponent GetSelectableTopItem()
		{
			SortedDictionary<int, PatternSelectInfoComponent> sortedDictionary = new SortedDictionary<int, PatternSelectInfoComponent>();
			for (int i = 0; i < _lstSelectInfo.Count; i++)
			{
				if (_lstSelectInfo[i].sic.tgl.interactable && _lstSelectInfo[i].sic.gameObject.activeSelf)
				{
					sortedDictionary[_lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = _lstSelectInfo[i].sic;
				}
			}
			PatternSelectInfoComponent result = null;
			if (sortedDictionary.Count != 0)
			{
				result = sortedDictionary.First().Value;
			}
			return result;
		}

		public int GetDrawOrderFromIndex(int index)
		{
			SortedDictionary<int, PatternSelectInfoComponent> sortedDictionary = new SortedDictionary<int, PatternSelectInfoComponent>();
			for (int i = 0; i < _lstSelectInfo.Count; i++)
			{
				if (_lstSelectInfo[i].sic.gameObject.activeSelf)
				{
					sortedDictionary[_lstSelectInfo[i].sic.gameObject.transform.GetSiblingIndex()] = _lstSelectInfo[i].sic;
				}
			}
			foreach (var item in sortedDictionary.Select((KeyValuePair<int, PatternSelectInfoComponent> val, int idx) => new { val, idx }))
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
			return _lstSelectInfo.Count((PatternSelectInfo _psi) => _psi.activeSelf);
		}

		public void SelectPrevItem()
		{
			List<PatternSelectInfo> list = _lstSelectInfo.Where((PatternSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
			int num = list.FindIndex((PatternSelectInfo lst) => lst.sic.tgl.isOn);
			if (num != -1)
			{
				int count = list.Count;
				int index = (num + count - 1) % count;
				SelectItem(list[index].index);
			}
		}

		public void SelectNextItem()
		{
			List<PatternSelectInfo> list = _lstSelectInfo.Where((PatternSelectInfo lst) => lst.sic.tgl.interactable && lst.sic.gameObject.activeSelf).ToList();
			int num = list.FindIndex((PatternSelectInfo lst) => lst.sic.tgl.isOn);
			if (num != -1)
			{
				int count = list.Count;
				int index = (num + 1) % count;
				SelectItem(list[index].index);
			}
		}

		public void SelectItem(int index)
		{
			PatternSelectInfo patternSelectInfo = _lstSelectInfo.Find((PatternSelectInfo item) => item.index == index);
			if (patternSelectInfo != null)
			{
				patternSelectInfo.sic.tgl.isOn = true;
				ChangeItem(patternSelectInfo.sic);
				UpdateScrollPosition();
			}
		}

		public void SelectItem(string name)
		{
			PatternSelectInfo patternSelectInfo = _lstSelectInfo.Find((PatternSelectInfo item) => item.name == name);
			if (patternSelectInfo != null)
			{
				patternSelectInfo.sic.tgl.isOn = true;
				ChangeItem(patternSelectInfo.sic);
				UpdateScrollPosition();
			}
		}

		public void UpdateScrollPosition()
		{
		}

		public void OnPointerClick(PatternSelectInfoComponent _psic)
		{
			if (!(null == _psic) && _psic.tgl.interactable)
			{
				ChangeItem(_psic);
			}
		}

		public void OnPointerEnter(PatternSelectInfoComponent _psic)
		{
			if (!(null == _psic) && _psic.tgl.interactable && (bool)textDrawName)
			{
				textDrawName.text = _psic.info.name;
			}
		}

		public void OnPointerExit(PatternSelectInfoComponent _psic)
		{
			if (!(null == _psic) && _psic.tgl.interactable && (bool)textDrawName)
			{
				textDrawName.text = selectDrawName;
			}
		}

		public void ChangeItem(PatternSelectInfoComponent _psic)
		{
			if (onChangeItemFunc != null)
			{
				onChangeItemFunc(_psic.info.index);
			}
			selectDrawName = _psic.info.name;
			if ((bool)textDrawName)
			{
				textDrawName.text = selectDrawName;
			}
		}

		public void ToggleAllOff()
		{
			for (int i = 0; i < _lstSelectInfo.Count; i++)
			{
				_lstSelectInfo[i].sic.tgl.isOn = false;
			}
		}

		public void DisableItem(int index, bool _disable)
		{
			_lstSelectInfo.Find((PatternSelectInfo item) => item.index == index).SafeProc(delegate(PatternSelectInfo psi)
			{
				psi.disable = _disable;
				psi.sic.Disable(_disable);
			});
		}

		public void DisableItem(string name, bool _disable)
		{
			_lstSelectInfo.Find((PatternSelectInfo item) => item.name == name).SafeProc(delegate(PatternSelectInfo psi)
			{
				psi.disable = _disable;
				psi.sic.Disable(_disable);
			});
		}

		public void DisvisibleItem(int index, bool _disvisible)
		{
			_lstSelectInfo.Find((PatternSelectInfo item) => item.index == index).SafeProc(delegate(PatternSelectInfo psi)
			{
				psi.disvisible = _disvisible;
				psi.sic.Disvisible(_disvisible);
			});
		}

		public void DisvisibleItem(string name, bool _disvisible)
		{
			_lstSelectInfo.Find((PatternSelectInfo item) => item.name == name).SafeProc(delegate(PatternSelectInfo psi)
			{
				psi.disvisible = _disvisible;
				psi.sic.Disvisible(_disvisible);
			});
		}

		private void SetToggleHandler(GameObject obj, PatternSelectInfoComponent _psic)
		{
			UIAL_EventTrigger uIAL_EventTrigger = obj.AddComponent<UIAL_EventTrigger>();
			uIAL_EventTrigger.triggers = new List<UIAL_EventTrigger.Entry>();
			UIAL_EventTrigger.Entry entry = new UIAL_EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.buttonType = UIAL_EventTrigger.ButtonType.Left;
			entry.callback.AddListener(delegate
			{
				OnPointerClick(_psic);
			});
			uIAL_EventTrigger.triggers.Add(entry);
			if ((bool)textDrawName)
			{
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerEnter;
				entry.callback.AddListener(delegate
				{
					OnPointerEnter(_psic);
				});
				uIAL_EventTrigger.triggers.Add(entry);
				entry = new UIAL_EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerExit;
				entry.callback.AddListener(delegate
				{
					OnPointerExit(_psic);
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
}
