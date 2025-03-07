using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class AnimeControl : MonoBehaviour
	{
		[SerializeField]
		private Slider sliderSpeed;

		[SerializeField]
		private EventTrigger eventSpeed;

		[SerializeField]
		private InputField inputSpeed;

		[SerializeField]
		private Slider sliderPattern;

		[SerializeField]
		private EventTrigger eventPattern;

		[SerializeField]
		private InputField inputPattern;

		[SerializeField]
		private Slider[] sliderOptionParam;

		[SerializeField]
		private EventTrigger[] eventOptionParam;

		[SerializeField]
		private InputField[] inputOptionParam;

		[SerializeField]
		private Toggle toggleOption;

		[SerializeField]
		private Toggle toggleLoop;

		[SerializeField]
		private Button[] buttons;

		private ObjectCtrlInfo m_ObjectCtrlInfo;

		private bool isUpdateInfo;

		private ObjectCtrlInfo[] arrayTarget;

		private int num;

		private float[] oldValue;

		private int sex = -1;

		private OICharInfo.AnimeInfo animeInfo = new OICharInfo.AnimeInfo();

		public ObjectCtrlInfo objectCtrlInfo
		{
			get
			{
				return m_ObjectCtrlInfo;
			}
			set
			{
				m_ObjectCtrlInfo = value;
				if (m_ObjectCtrlInfo != null)
				{
					UpdateInfo();
				}
			}
		}

		public bool active
		{
			set
			{
				if (base.gameObject.activeSelf != value)
				{
					base.gameObject.SetActive(value);
				}
			}
		}

		private void OnValueChangedSpeed(float _value)
		{
			if (!isUpdateInfo)
			{
				if (arrayTarget.IsNullOrEmpty())
				{
					OnPointerDownSpeed(null);
				}
				for (int i = 0; i < num; i++)
				{
					arrayTarget[i].animeSpeed = _value;
				}
				inputSpeed.text = _value.ToString("0.00");
			}
		}

		private void OnPointerDownSpeed(BaseEventData _data)
		{
			if (arrayTarget.IsNullOrEmpty())
			{
				arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0 || v.kind == 1).ToArray();
				num = arrayTarget.Length;
				oldValue = arrayTarget.Select((ObjectCtrlInfo v) => v.animeSpeed).ToArray();
			}
		}

		private void OnPointerUpSpeed(BaseEventData _data)
		{
			if (arrayTarget.Length != 0)
			{
				Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.SpeedCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), sliderSpeed.value, oldValue));
				arrayTarget = null;
				num = 0;
			}
		}

		private void OnEndEditSpeed(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 3f);
			arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0 || v.kind == 1).ToArray();
			this.num = arrayTarget.Length;
			oldValue = arrayTarget.Select((ObjectCtrlInfo v) => v.animeSpeed).ToArray();
			Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.SpeedCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), num, oldValue));
			isUpdateInfo = true;
			sliderSpeed.value = num;
			inputSpeed.text = num.ToString("0.00");
			isUpdateInfo = false;
			arrayTarget = null;
			this.num = 0;
		}

		private void OnValueChangedPattern(float _value)
		{
			if (!isUpdateInfo)
			{
				if (arrayTarget.IsNullOrEmpty())
				{
					OnPointerDownPattern(null);
				}
				float animePattern = Mathf.Lerp(0f, 1f, _value);
				for (int i = 0; i < num; i++)
				{
					(arrayTarget[i] as OCIChar).animePattern = animePattern;
				}
				inputPattern.text = animePattern.ToString("0.00");
			}
		}

		private void OnPointerDownPattern(BaseEventData _data)
		{
			if (arrayTarget.IsNullOrEmpty())
			{
				arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0).ToArray();
				num = arrayTarget.Length;
				oldValue = arrayTarget.Select((ObjectCtrlInfo v) => (v as OCIChar).animePattern).ToArray();
			}
		}

		private void OnPointerUpPattern(BaseEventData _data)
		{
			if (arrayTarget.Length != 0)
			{
				float value = Mathf.Lerp(0f, 1f, sliderPattern.value);
				Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.PatternCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), value, oldValue));
			}
		}

		private void OnEndEditPattern(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
			arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0).ToArray();
			num = arrayTarget.Length;
			oldValue = arrayTarget.Select((ObjectCtrlInfo v) => (v as OCIChar).animePattern).ToArray();
			Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.PatternCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), value, oldValue));
			isUpdateInfo = true;
			sliderPattern.value = Mathf.InverseLerp(0f, 1f, value);
			inputPattern.text = value.ToString("0.00");
			isUpdateInfo = false;
			arrayTarget = null;
			num = 0;
		}

		private void ChangedOptionParam(float _value, int _kind)
		{
			if (isUpdateInfo)
			{
				return;
			}
			if (arrayTarget.IsNullOrEmpty())
			{
				DownOptionParam(null, _kind);
			}
			float num = Mathf.Lerp(0f, 1f, _value);
			for (int i = 0; i < this.num; i++)
			{
				OCIChar oCIChar = arrayTarget[i] as OCIChar;
				if (_kind == 0)
				{
					oCIChar.animeOptionParam1 = num;
				}
				else
				{
					oCIChar.animeOptionParam2 = num;
				}
			}
			inputOptionParam[_kind].text = num.ToString("0.00");
		}

		private void DownOptionParam(BaseEventData _data, int _kind)
		{
			if (arrayTarget.IsNullOrEmpty())
			{
				arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0).ToArray();
				num = arrayTarget.Length;
				oldValue = arrayTarget.Select((ObjectCtrlInfo v) => (v as OCIChar).animeOptionParam[_kind]).ToArray();
			}
		}

		private void UpOptionParam(BaseEventData _data, int _kind)
		{
			if (arrayTarget.Length != 0)
			{
				float value = Mathf.Lerp(0f, 1f, sliderOptionParam[_kind].value);
				Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.OptionParamCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), value, oldValue, _kind));
			}
		}

		private void EndEditOptionParam(string _text, int _kind)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
			arrayTarget = Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl.Where((ObjectCtrlInfo v) => v.kind == 0).ToArray();
			num = arrayTarget.Length;
			oldValue = arrayTarget.Select((ObjectCtrlInfo v) => (v as OCIChar).animeOptionParam[_kind]).ToArray();
			Singleton<UndoRedoManager>.Instance.Do(new AnimeCommand.OptionParamCommand(arrayTarget.Select((ObjectCtrlInfo v) => v.objectInfo.dicKey).ToArray(), value, oldValue, _kind));
			isUpdateInfo = true;
			sliderOptionParam[_kind].value = value;
			inputOptionParam[_kind].text = value.ToString("0.00");
			isUpdateInfo = false;
			arrayTarget = null;
			num = 0;
		}

		private void OnValueChangedOption(bool _value)
		{
			if (!isUpdateInfo)
			{
				OCIChar oCIChar = m_ObjectCtrlInfo as OCIChar;
				if (oCIChar != null)
				{
					oCIChar.optionItemCtrl.visible = _value;
				}
			}
		}

		private void OnValueChangedLoop(bool _value)
		{
			if (!isUpdateInfo)
			{
				OCIChar oCIChar = m_ObjectCtrlInfo as OCIChar;
				if (oCIChar != null)
				{
					oCIChar.charAnimeCtrl.isForceLoop = _value;
				}
			}
		}

		private void OnClickRestart()
		{
			OCIChar[] array = (from v in Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl
				select v as OCIChar into v
				where v != null
				select v).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RestartAnime();
			}
			OCIItem[] array2 = (from v in Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl
				select v as OCIItem into v
				where v != null
				select v).ToArray();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].RestartAnime();
			}
		}

		private void OnClickAllRestart()
		{
			OCIChar[] array = (from v in Singleton<Studio>.Instance.dicObjectCtrl
				where v.Value.kind == 0
				select v.Value as OCIChar).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RestartAnime();
			}
			OCIItem[] array2 = (from v in Singleton<Studio>.Instance.dicObjectCtrl
				where v.Value.kind == 1
				select v.Value as OCIItem).ToArray();
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].RestartAnime();
			}
		}

		private void OnClickCopy()
		{
			OCIChar[] array = (from v in Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl
				select v as OCIChar into v
				where v != null
				select v).ToArray();
			if (array.Length != 0)
			{
				animeInfo.Copy(array[0].oiCharInfo.animeInfo);
				sex = array[0].oiCharInfo.sex;
			}
		}

		private void OnClickPaste()
		{
			if (sex != -1 && animeInfo.exist)
			{
				OCIChar[] array = (from v in Singleton<Studio>.Instance.treeNodeCtrl.selectObjectCtrl
					select v as OCIChar into v
					where v != null
					select v).ToArray();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].LoadAnime(animeInfo.group, animeInfo.category, animeInfo.no);
				}
			}
		}

		private void Init()
		{
			sliderSpeed.onValueChanged.AddListener(OnValueChangedSpeed);
			AddEventTrigger(eventSpeed, EventTriggerType.PointerDown, OnPointerDownSpeed);
			AddEventTrigger(eventSpeed, EventTriggerType.PointerUp, OnPointerUpSpeed);
			inputSpeed.onEndEdit.AddListener(OnEndEditSpeed);
			sliderPattern.onValueChanged.AddListener(OnValueChangedPattern);
			AddEventTrigger(eventPattern, EventTriggerType.PointerDown, OnPointerDownPattern);
			AddEventTrigger(eventPattern, EventTriggerType.PointerUp, OnPointerUpPattern);
			inputPattern.onEndEdit.AddListener(OnEndEditPattern);
			for (int i = 0; i < 2; i++)
			{
				int kind = i;
				sliderOptionParam[i].onValueChanged.AddListener(delegate(float t)
				{
					ChangedOptionParam(t, kind);
				});
				AddEventTrigger(eventOptionParam[i], EventTriggerType.PointerDown, delegate(BaseEventData d)
				{
					DownOptionParam(d, kind);
				});
				AddEventTrigger(eventOptionParam[i], EventTriggerType.PointerUp, delegate(BaseEventData d)
				{
					UpOptionParam(d, kind);
				});
				inputOptionParam[i].onEndEdit.AddListener(delegate(string s)
				{
					EndEditOptionParam(s, kind);
				});
			}
			toggleOption.onValueChanged.AddListener(OnValueChangedOption);
			toggleLoop.onValueChanged.AddListener(OnValueChangedLoop);
			buttons[0].onClick.AddListener(OnClickRestart);
			buttons[1].onClick.AddListener(OnClickAllRestart);
			buttons[2].onClick.AddListener(OnClickCopy);
			buttons[3].onClick.AddListener(OnClickPaste);
		}

		public void UpdateInfo()
		{
			isUpdateInfo = true;
			arrayTarget = null;
			bool flag = m_ObjectCtrlInfo.kind == 0;
			OCIChar oCIChar = m_ObjectCtrlInfo as OCIChar;
			bool flag2 = flag && oCIChar.isHAnime;
			sliderSpeed.value = m_ObjectCtrlInfo.animeSpeed;
			inputSpeed.text = m_ObjectCtrlInfo.animeSpeed.ToString("0.00");
			sliderPattern.interactable = flag;
			inputPattern.interactable = flag;
			sliderPattern.value = ((!flag) ? 0.5f : Mathf.InverseLerp(0f, 1f, oCIChar.animePattern));
			inputPattern.text = ((!flag) ? "-" : oCIChar.animePattern.ToString("0.00"));
			for (int i = 0; i < 2; i++)
			{
				sliderOptionParam[i].interactable = flag2;
				inputOptionParam[i].interactable = flag2;
				sliderOptionParam[i].value = ((!flag2) ? 0.5f : oCIChar.animeOptionParam[i]);
				inputOptionParam[i].text = ((!flag2) ? "-" : oCIChar.animeOptionParam[i].ToString("0.00"));
			}
			toggleOption.interactable = flag;
			toggleOption.isOn = flag && oCIChar.optionItemCtrl.visible;
			toggleLoop.interactable = flag;
			toggleLoop.isOn = flag && oCIChar.charAnimeCtrl.isForceLoop;
			buttons[2].interactable = flag;
			buttons[3].interactable = flag;
			isUpdateInfo = false;
		}

		private void AddEventTrigger(EventTrigger _event, EventTriggerType _type, UnityAction<BaseEventData> _action)
		{
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = _type;
			entry.callback.AddListener(_action);
			_event.triggers.Add(entry);
		}

		private void Awake()
		{
			arrayTarget = null;
			num = 0;
			Init();
			base.gameObject.SetActive(false);
		}
	}
}
