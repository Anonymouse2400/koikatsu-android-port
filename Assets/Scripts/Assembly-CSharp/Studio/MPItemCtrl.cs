using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class MPItemCtrl : MonoBehaviour
	{
		[Serializable]
		private class ColorCombination
		{
			public GameObject objRoot;

			public Image imageColor;

			public Button buttonColor;

			public Button buttonColorDefault;

			public bool interactable
			{
				set
				{
					buttonColor.interactable = value;
					if ((bool)buttonColorDefault)
					{
						buttonColorDefault.interactable = value;
					}
				}
			}

			public Color color
			{
				get
				{
					return imageColor.color;
				}
				set
				{
					imageColor.color = value;
				}
			}

			public bool active
			{
				set
				{
					objRoot.SetActiveIfDifferent(value);
				}
			}
		}

		[Serializable]
		private class InputCombination
		{
			public GameObject objRoot;

			public TMP_InputField input;

			public Slider slider;

			public Button buttonDefault;

			public bool interactable
			{
				set
				{
					input.interactable = value;
					slider.interactable = value;
					if ((bool)buttonDefault)
					{
						buttonDefault.interactable = value;
					}
				}
			}

			public string text
			{
				get
				{
					return input.text;
				}
				set
				{
					input.text = value;
					slider.value = Utility.StringToFloat(value);
				}
			}

			public float value
			{
				get
				{
					return slider.value;
				}
				set
				{
					slider.value = value;
					input.text = value.ToString("0.0");
				}
			}

			public bool active
			{
				set
				{
					objRoot.SetActiveIfDifferent(value);
				}
			}
		}

		[Serializable]
		private class ColorInfo
		{
			public GameObject objRoot;

			public ColorCombination _colorMain = new ColorCombination();

			public Button _buttonPattern;

			public TextMeshProUGUI _textPattern;

			public ColorCombination _colorPattern = new ColorCombination();

			public Toggle _toggleClamp;

			public InputCombination[] _input;

			public Image _imageBack;

			public LayoutElement _layout;

			public CanvasGroup groupPattern;

			public Color colorMain
			{
				set
				{
					_colorMain.imageColor.color = value;
				}
			}

			public string textPattern
			{
				set
				{
					_textPattern.text = value;
				}
			}

			public Color colorPattern
			{
				set
				{
					_colorPattern.imageColor.color = value;
				}
			}

			public bool isOn
			{
				set
				{
					_toggleClamp.isOn = value;
				}
			}

			public InputCombination this[int _idx]
			{
				get
				{
					return _input.SafeGet(_idx);
				}
			}

			public bool enable
			{
				get
				{
					return objRoot.activeSelf;
				}
				set
				{
					if (objRoot.activeSelf != value)
					{
						objRoot.SetActive(value);
					}
				}
			}

			public Sprite imageBack
			{
				set
				{
					_imageBack.sprite = value;
				}
			}

			public float layoutHeight
			{
				set
				{
					_layout.minHeight = value;
					_layout.preferredHeight = value;
				}
			}

			public bool enablePattern
			{
				set
				{
					groupPattern.Enable(value);
				}
			}
		}

		[Serializable]
		private class ColorInputCombination
		{
			public GameObject objRoot;

			public ColorCombination color = new ColorCombination();

			public InputCombination input = new InputCombination();

			public bool active
			{
				set
				{
					objRoot.SetActiveIfDifferent(value);
				}
			}
		}

		[Serializable]
		private class EmissionInfo : ColorInputCombination
		{
		}

		[Serializable]
		private class LineInfo : ColorInputCombination
		{
		}

		[Serializable]
		private class PanelInfo
		{
			public GameObject objRoot;

			public Button _buttonTex;

			public TextMeshProUGUI _textTex;

			public ColorCombination _color;

			public Toggle _toggleClamp;

			public InputCombination[] _input;

			public string textTex
			{
				set
				{
					_textTex.text = value;
				}
			}

			public Color color
			{
				set
				{
					_color.imageColor.color = value;
				}
			}

			public bool isOn
			{
				set
				{
					_toggleClamp.isOn = value;
				}
			}

			public InputCombination this[int _idx]
			{
				get
				{
					return _input.SafeGet(_idx);
				}
			}

			public bool enable
			{
				get
				{
					return objRoot.activeSelf;
				}
				set
				{
					objRoot.SetActiveIfDifferent(value);
				}
			}
		}

		[Serializable]
		private class PanelList
		{
			[SerializeField]
			private GameObject objRoot;

			[SerializeField]
			private GameObject objectNode;

			[SerializeField]
			private Transform transformRoot;

			public Action<string> actUpdatePath;

			private List<string> listPath = new List<string>();

			private Dictionary<int, StudioNode> dicNode = new Dictionary<int, StudioNode>();

			private int select = -1;

			public bool active
			{
				get
				{
					return objRoot.activeSelf;
				}
				set
				{
					objRoot.SetActiveIfDifferent(value);
				}
			}

			public void Init()
			{
				for (int i = 0; i < transformRoot.childCount; i++)
				{
					UnityEngine.Object.Destroy(transformRoot.GetChild(i).gameObject);
				}
				transformRoot.DetachChildren();
				listPath = Directory.GetFiles(UserData.Create(BackgroundList.dirName), "*.png").Select(Path.GetFileName).ToList();
				CreateNode(-1, "なし");
				int count = listPath.Count;
				for (int j = 0; j < count; j++)
				{
					CreateNode(j, Path.GetFileNameWithoutExtension(listPath[j]));
				}
			}

			public void Setup(string _file, Action<string> _actUpdate)
			{
				SetSelect(select, false);
				select = listPath.FindIndex((string s) => s == _file);
				SetSelect(select, true);
				actUpdatePath = _actUpdate;
				active = true;
			}

			private void OnClickSelect(int _idx)
			{
				SetSelect(select, false);
				select = _idx;
				SetSelect(select, true);
				if (actUpdatePath != null)
				{
					actUpdatePath((select == -1) ? string.Empty : listPath[_idx]);
				}
				active = false;
			}

			private void SetSelect(int _idx, bool _flag)
			{
				StudioNode value = null;
				if (dicNode.TryGetValue(_idx, out value))
				{
					value.select = _flag;
				}
			}

			private void CreateNode(int _idx, string _text)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(objectNode);
				gameObject.transform.SetParent(transformRoot, false);
				StudioNode component = gameObject.GetComponent<StudioNode>();
				component.active = true;
				component.addOnClick = delegate
				{
					OnClickSelect(_idx);
				};
				component.text = _text;
				dicNode.Add(_idx, component);
			}
		}

		[SerializeField]
		private ColorInfo[] colorInfo;

		[SerializeField]
		private Sprite[] spriteBack;

		[SerializeField]
		private PanelInfo panelInfo;

		[SerializeField]
		private PanelList panelList;

		[SerializeField]
		private ColorCombination colorShadow;

		[SerializeField]
		private InputCombination inputAlpha;

		[SerializeField]
		private EmissionInfo emissionInfo;

		[SerializeField]
		private InputCombination inputLightCancel;

		[SerializeField]
		private LineInfo lineInfo;

		[SerializeField]
		private Toggle toggleFK;

		[SerializeField]
		private Toggle toggleDynamicBone;

		[SerializeField]
		private CanvasGroup cgPattern;

		[SerializeField]
		private AnimeControl animeControl;

		[SerializeField]
		private MPCharCtrl mpCharCtrl;

		private OCIItem m_OCIItem;

		private bool m_Active;

		private bool isUpdateInfo;

		private bool isColorFunc;

		public OCIItem ociItem
		{
			get
			{
				return m_OCIItem;
			}
			set
			{
				m_OCIItem = value;
				if (m_OCIItem != null)
				{
					UpdateInfo();
				}
			}
		}

		public bool active
		{
			get
			{
				return m_Active;
			}
			set
			{
				m_Active = value;
				if (m_Active)
				{
					base.gameObject.SetActive(m_OCIItem != null && (m_OCIItem.isChangeColor | m_OCIItem.checkPanel | m_OCIItem.isFK));
					animeControl.active = m_OCIItem != null && m_OCIItem.isAnime;
					return;
				}
				if (!mpCharCtrl.active)
				{
					animeControl.active = false;
				}
				base.gameObject.SetActive(false);
				if (isColorFunc)
				{
					Singleton<Studio>.Instance.colorPalette.Close();
				}
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				panelList.active = false;
				cgPattern.Enable(false);
			}
		}

		public bool Deselect(OCIItem _ociItem)
		{
			if (m_OCIItem != _ociItem)
			{
				return false;
			}
			ociItem = null;
			active = false;
			return true;
		}

		public void UpdateInfo()
		{
			if (m_OCIItem == null)
			{
				return;
			}
			isUpdateInfo = true;
			bool[] useColor = m_OCIItem.useColor;
			bool[] usePattern = m_OCIItem.usePattern;
			for (int i = 0; i < 3; i++)
			{
				colorInfo[i].enable = useColor[i];
				if (useColor[i])
				{
					colorInfo[i].colorMain = m_OCIItem.itemInfo.color[i];
					colorInfo[i].imageBack = spriteBack[(!usePattern[i]) ? 1u : 0u];
					colorInfo[i].layoutHeight = ((!usePattern[i]) ? 30 : 230);
					colorInfo[i].enablePattern = usePattern[i];
					if (usePattern[i])
					{
						colorInfo[i].textPattern = m_OCIItem.itemInfo.pattern[i].name;
						colorInfo[i].colorPattern = m_OCIItem.itemInfo.color[i + 3];
						colorInfo[i].isOn = !m_OCIItem.itemInfo.pattern[i].clamp;
						colorInfo[i][0].value = m_OCIItem.itemInfo.pattern[i].ut;
						colorInfo[i][1].value = m_OCIItem.itemInfo.pattern[i].vt;
						colorInfo[i][2].value = m_OCIItem.itemInfo.pattern[i].us;
						colorInfo[i][3].value = m_OCIItem.itemInfo.pattern[i].vs;
						colorInfo[i][4].value = m_OCIItem.itemInfo.pattern[i].rot;
					}
				}
			}
			colorInfo[3].enable = m_OCIItem.useColor4;
			if (colorInfo[3].enable)
			{
				colorInfo[3].colorMain = m_OCIItem.itemInfo.color[7];
				colorInfo[3].imageBack = spriteBack[1];
				colorInfo[3].layoutHeight = 30f;
				colorInfo[3].enablePattern = false;
			}
			panelInfo.enable = m_OCIItem.checkPanel;
			panelList.active = false;
			SetPanelTexName(m_OCIItem.itemInfo.panel.filePath);
			panelInfo.color = m_OCIItem.itemInfo.color[0];
			panelInfo.isOn = !m_OCIItem.itemInfo.pattern[0].clamp;
			panelInfo[0].value = m_OCIItem.itemInfo.pattern[0].ut;
			panelInfo[1].value = m_OCIItem.itemInfo.pattern[0].vt;
			panelInfo[2].value = m_OCIItem.itemInfo.pattern[0].us;
			panelInfo[3].value = m_OCIItem.itemInfo.pattern[0].vs;
			panelInfo[4].value = m_OCIItem.itemInfo.pattern[0].rot;
			colorShadow.color = m_OCIItem.itemInfo.color[6];
			colorShadow.active = m_OCIItem.checkShadow;
			inputAlpha.value = m_OCIItem.itemInfo.alpha;
			inputAlpha.active = m_OCIItem.checkAlpha;
			emissionInfo.active = m_OCIItem.checkEmission;
			if (m_OCIItem.checkEmissionColor)
			{
				emissionInfo.color.interactable = true;
				emissionInfo.color.color = m_OCIItem.itemInfo.emissionColor;
			}
			else
			{
				emissionInfo.color.interactable = false;
				emissionInfo.color.color = Color.white;
			}
			if (m_OCIItem.checkEmissionPower)
			{
				emissionInfo.input.interactable = true;
				emissionInfo.input.value = m_OCIItem.itemInfo.emissionPower;
			}
			else
			{
				emissionInfo.input.interactable = false;
				emissionInfo.input.value = 0f;
			}
			inputLightCancel.active = m_OCIItem.checkLightCancel;
			inputLightCancel.value = m_OCIItem.itemInfo.lightCancel;
			lineInfo.active = m_OCIItem.checkLine;
			lineInfo.color.color = m_OCIItem.itemInfo.lineColor;
			lineInfo.input.value = m_OCIItem.itemInfo.lineWidth;
			toggleFK.interactable = m_OCIItem.isFK;
			toggleFK.isOn = m_OCIItem.itemInfo.enableFK;
			toggleDynamicBone.interactable = m_OCIItem.isDynamicBone;
			toggleDynamicBone.isOn = m_OCIItem.itemInfo.enableDynamicBone;
			animeControl.objectCtrlInfo = m_OCIItem;
			cgPattern.Enable(false);
			isUpdateInfo = false;
		}

		private void OnClickColorMain(int _idx)
		{
			string[] array = new string[3] { "アイテム カラー１", "アイテム カラー２", "アイテム カラー３" };
			if (Singleton<Studio>.Instance.colorPalette.Check(array[_idx]))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			IEnumerable<OCIItem> array2 = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem;
			Singleton<Studio>.Instance.colorPalette.Setup(array[_idx], m_OCIItem.itemInfo.color[_idx], delegate(Color _c)
			{
				foreach (OCIItem item in array2)
				{
					item.SetColor(_c, _idx);
				}
				colorInfo[_idx].colorMain = _c;
			}, m_OCIItem.isParticle);
			isColorFunc = true;
		}

		private void OnClickColor4()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("アイテム カラー４"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			IEnumerable<OCIItem> array = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem;
			Singleton<Studio>.Instance.colorPalette.Setup("アイテム カラー４", m_OCIItem.itemInfo.color[7], delegate(Color _c)
			{
				foreach (OCIItem item in array)
				{
					item.SetColor(_c, 7);
				}
				colorInfo[3].colorMain = _c;
			}, true);
			isColorFunc = true;
		}

		private void OnClickColorMainDef(int _idx)
		{
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				if (item.isChangeColor && item.useColor.SafeGet(_idx))
				{
					item.SetColor(item.defColor[_idx], _idx);
				}
			}
			m_OCIItem.defColor.SafeProc(_idx, delegate(Color _c)
			{
				colorInfo[_idx].colorMain = _c;
			});
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnClickColor4Def()
		{
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetColor((!(item.chaAccessoryComponent != null)) ? item.itemComponent.defGlass : item.chaAccessoryComponent.defColor04, 7);
			}
			colorInfo[3].colorMain = ((!(m_OCIItem.chaAccessoryComponent != null)) ? m_OCIItem.itemComponent.defGlass : m_OCIItem.chaAccessoryComponent.defColor04);
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnClickColorPattern(int _idx)
		{
			string[] array = new string[3] { "柄の色１", "柄の色２", "柄の色３" };
			if (Singleton<Studio>.Instance.colorPalette.Check(array[_idx]))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			IEnumerable<OCIItem> array2 = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem;
			Singleton<Studio>.Instance.colorPalette.Setup(array[_idx], m_OCIItem.itemInfo.color[_idx + 3], delegate(Color _c)
			{
				foreach (OCIItem item in array2)
				{
					item.SetColor(_c, _idx + 3);
				}
				colorInfo[_idx].colorPattern = _c;
			}, true);
			isColorFunc = true;
		}

		private void OnClickColorPatternDef(int _idx)
		{
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetColor(item.itemComponent.defColorPattern[_idx], _idx + 3);
			}
			colorInfo[_idx].colorPattern = m_OCIItem.itemComponent.defColorPattern[_idx];
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnToggleColor(int _idx, bool _flag)
		{
			if (m_OCIItem == null)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternClamp(_idx, !_flag);
			}
		}

		private void OnValueChangeUT(int _idx, float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternUT(_idx, _value);
			}
			colorInfo[_idx][0].value = _value;
		}

		private void OnEndEditUT(int _idx, string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternUT(_idx, value);
			}
			colorInfo[_idx][0].value = value;
		}

		private void OnClickUTDef(int _idx)
		{
			foreach (OCIItem v2 in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				v2.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
				{
					v2.SetPatternUT(_idx, info.ut);
				});
			}
			m_OCIItem.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
			{
				colorInfo[_idx][0].value = info.ut;
			});
		}

		private void OnValueChangeVT(int _idx, float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternVT(_idx, _value);
			}
			colorInfo[_idx][1].value = _value;
		}

		private void OnEndEditVT(int _idx, string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternVT(_idx, value);
			}
			colorInfo[_idx][1].value = value;
		}

		private void OnClickVTDef(int _idx)
		{
			foreach (OCIItem v2 in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				v2.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
				{
					v2.SetPatternVT(_idx, info.vt);
				});
			}
			m_OCIItem.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
			{
				colorInfo[_idx][1].value = info.vt;
			});
		}

		private void OnValueChangeUS(int _idx, float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternUS(_idx, _value);
			}
			colorInfo[_idx][2].value = _value;
		}

		private void OnEndEditUS(int _idx, string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0.01f, 20f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternUS(_idx, value);
			}
			colorInfo[_idx][2].value = value;
		}

		private void OnClickUSDef(int _idx)
		{
			foreach (OCIItem v2 in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				v2.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
				{
					v2.SetPatternUS(_idx, info.us);
				});
			}
			m_OCIItem.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
			{
				colorInfo[_idx][2].value = info.us;
			});
		}

		private void OnValueChangeVS(int _idx, float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternVS(_idx, _value);
			}
			colorInfo[_idx][3].value = _value;
		}

		private void OnEndEditVS(int _idx, string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0.01f, 20f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternVS(_idx, value);
			}
			colorInfo[_idx][3].value = value;
		}

		private void OnClickVSDef(int _idx)
		{
			foreach (OCIItem v2 in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				v2.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
				{
					v2.SetPatternVS(_idx, info.vs);
				});
			}
			m_OCIItem.itemComponent[_idx].SafeProc(delegate(ItemComponent.Info info)
			{
				colorInfo[_idx][3].value = info.vs;
			});
		}

		private void OnValueChangeRot(int _idx, float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternRot(_idx, _value);
			}
			colorInfo[_idx][4].value = _value;
		}

		private void OnEndEditRot(int _idx, string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetPatternRot(_idx, value);
			}
			colorInfo[_idx][4].value = value;
		}

		private void OnClickPanel()
		{
			if (panelList.active)
			{
				panelList.active = false;
				return;
			}
			panelList.Setup(m_OCIItem.itemInfo.panel.filePath, SetMainTex);
			isColorFunc = true;
		}

		private void SetMainTex(string _file)
		{
			SetPanelTexName(_file);
			m_OCIItem.SetMainTex(_file);
		}

		private void SetPanelTexName(string _str)
		{
			panelInfo.textTex = ((!_str.IsNullOrEmpty()) ? Path.GetFileNameWithoutExtension(_str) : "なし");
		}

		private void OnClickColorPanel()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("画像板"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			Singleton<Studio>.Instance.colorPalette.Setup("画像板", m_OCIItem.itemInfo.color[0], delegate(Color _c)
			{
				m_OCIItem.SetColor(_c, 0);
				panelInfo.color = _c;
			}, false);
			isColorFunc = true;
		}

		private void OnToggleColor(bool _flag)
		{
			if (m_OCIItem != null)
			{
				m_OCIItem.SetPatternClamp(0, !_flag);
			}
		}

		private void OnValueChangeUT(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetPatternUT(0, _value);
				panelInfo[0].value = _value;
			}
		}

		private void OnEndEditUT(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			m_OCIItem.SetPatternUT(0, value);
			panelInfo[0].value = value;
		}

		private void OnClickUTDef()
		{
			panelInfo[0].value = 0f;
		}

		private void OnValueChangeVT(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetPatternVT(0, _value);
				panelInfo[1].value = _value;
			}
		}

		private void OnEndEditVT(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			m_OCIItem.SetPatternVT(0, value);
			panelInfo[1].value = value;
		}

		private void OnClickVTDef()
		{
			panelInfo[1].value = 0f;
		}

		private void OnValueChangeUS(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetPatternUS(0, _value);
				panelInfo[2].value = _value;
			}
		}

		private void OnEndEditUS(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0.01f, 20f);
			m_OCIItem.SetPatternUS(0, value);
			panelInfo[2].value = value;
		}

		private void OnClickUSDef()
		{
			panelInfo[2].value = 1f;
		}

		private void OnValueChangeVS(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetPatternVS(0, _value);
				panelInfo[3].value = _value;
			}
		}

		private void OnEndEditVS(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), 0.01f, 20f);
			m_OCIItem.SetPatternVS(0, value);
			panelInfo[3].value = value;
		}

		private void OnClickVSDef()
		{
			panelInfo[3].value = 1f;
		}

		private void OnValueChangeRot(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetPatternRot(0, _value);
				panelInfo[4].value = _value;
			}
		}

		private void OnEndEditRot(string _text)
		{
			float value = Mathf.Clamp(Utility.StringToFloat(_text), -1f, 1f);
			m_OCIItem.SetPatternRot(0, value);
			panelInfo[4].value = value;
		}

		private void OnClickShadow()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("影"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			IEnumerable<OCIItem> array = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem;
			Singleton<Studio>.Instance.colorPalette.Setup("影", m_OCIItem.itemInfo.color[6], delegate(Color _c)
			{
				foreach (OCIItem item in array)
				{
					item.SetColor(_c, 6);
				}
				colorShadow.imageColor.color = _c;
			}, false);
			isColorFunc = true;
		}

		private void OnClickShadowDef()
		{
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetColor(item.itemComponent.defShadow, 6);
			}
			colorShadow.imageColor.color = m_OCIItem.itemComponent.defShadow;
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnValueChangeAlpha(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetAlpha(_value);
				inputAlpha.value = _value;
			}
		}

		private void OnEndEditAlpha(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
			m_OCIItem.SetAlpha(num);
			inputAlpha.value = num;
		}

		private void OnClickEmissionColor()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("発光色"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			Singleton<Studio>.Instance.colorPalette.Setup("発光色", m_OCIItem.itemInfo.emissionColor, delegate(Color _c)
			{
				m_OCIItem.SetEmissionColor(_c);
				emissionInfo.color.color = _c;
			}, false);
			isColorFunc = true;
		}

		private void OnClickEmissionColorDef()
		{
			Color defEmissionColor = m_OCIItem.itemComponent.defEmissionColor;
			m_OCIItem.SetEmissionColor(defEmissionColor);
			emissionInfo.color.color = defEmissionColor;
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnValueChangeEmissionPower(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetEmissionPower(_value);
				emissionInfo.input.value = _value;
			}
		}

		private void OnEndEditEmissionPower(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
			m_OCIItem.SetEmissionPower(num);
			emissionInfo.input.value = num;
		}

		private void OnClickEmissionPowerDef()
		{
			m_OCIItem.SetEmissionPower(m_OCIItem.itemComponent.defEmissionPower);
			emissionInfo.input.value = m_OCIItem.itemComponent.defEmissionPower;
		}

		private void OnValueChangeLightCancel(float _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.SetLightCancel(_value);
				inputLightCancel.value = _value;
			}
		}

		private void OnEndEditLightCancel(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
			m_OCIItem.SetLightCancel(num);
			inputLightCancel.value = num;
		}

		private void OnClickLightCancelDef()
		{
			m_OCIItem.SetLightCancel(m_OCIItem.itemComponent.defLightCancel);
			inputLightCancel.value = m_OCIItem.itemComponent.defLightCancel;
		}

		private void OnClickLineColor()
		{
			if (Singleton<Studio>.Instance.colorPalette.Check("ラインの色"))
			{
				isColorFunc = false;
				Singleton<Studio>.Instance.colorPalette.visible = false;
				return;
			}
			IEnumerable<OCIItem> array = from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem;
			Singleton<Studio>.Instance.colorPalette.Setup("ラインの色", m_OCIItem.itemInfo.lineColor, delegate(Color _c)
			{
				foreach (OCIItem item in array)
				{
					item.SetLineColor(_c);
				}
				lineInfo.color.color = _c;
			}, false);
			isColorFunc = true;
		}

		private void OnClickLineColorDef()
		{
			Color defLineColor = m_OCIItem.itemComponent.defLineColor;
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.ResetLineColor();
			}
			lineInfo.color.color = defLineColor;
			isColorFunc = false;
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		private void OnValueChangeLineWidth(float _value)
		{
			if (isUpdateInfo)
			{
				return;
			}
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetLineWidth(_value);
			}
			lineInfo.input.value = _value;
		}

		private void OnEndEditLineWidth(string _text)
		{
			float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 2f);
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.SetLineWidth(num);
			}
			lineInfo.input.value = num;
		}

		private void OnClickLineWidthDef()
		{
			foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
				where v.kind == 1
				select v as OCIItem)
			{
				item.ResetLineWidth();
			}
			lineInfo.input.value = m_OCIItem.itemComponent.defLineWidth;
		}

		private void OnValueChangedFK(bool _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.ActiveFK(_value);
				toggleDynamicBone.interactable = m_OCIItem.isDynamicBone;
			}
		}

		private void OnValueChangedDynamicBone(bool _value)
		{
			if (!isUpdateInfo)
			{
				m_OCIItem.ActiveDynamicBone(_value);
			}
		}

		private void OnClickPattern(int _idx)
		{
			if (cgPattern.alpha != 0f)
			{
				cgPattern.Enable(false);
				return;
			}
			Singleton<Studio>.Instance.patternSelectListCtrl.onChangeItemFunc = delegate(int _index)
			{
				string text = string.Empty;
				foreach (OCIItem item in from v in Studio.GetSelectObjectCtrl()
					where v.kind == 1
					select v as OCIItem)
				{
					string text2 = item.SetPatternTex(_idx, _index);
					if (text.IsNullOrEmpty())
					{
						text = text2;
					}
				}
				colorInfo[_idx].textPattern = Path.GetFileNameWithoutExtension(text);
				cgPattern.Enable(false);
			};
			cgPattern.Enable(true);
		}

		private string ConvertString(float _t)
		{
			return ((int)Mathf.Lerp(0f, 100f, _t)/*cast due to .constrained prefix*/).ToString();
		}

		private void Awake()
		{
			panelList.Init();
			for (int i = 0; i < 3; i++)
			{
				int no = i;
				colorInfo[i]._colorMain.buttonColor.OnClickAsObservable().Subscribe(delegate
				{
					OnClickColorMain(no);
				});
				colorInfo[i]._colorMain.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickColorMainDef(no);
				});
				colorInfo[i]._buttonPattern.OnClickAsObservable().Subscribe(delegate
				{
					OnClickPattern(no);
				});
				colorInfo[i]._colorPattern.buttonColor.OnClickAsObservable().Subscribe(delegate
				{
					OnClickColorPattern(no);
				});
				colorInfo[i]._colorPattern.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickColorPatternDef(no);
				});
				colorInfo[i]._toggleClamp.OnValueChangedAsObservable().Subscribe(delegate(bool f)
				{
					OnToggleColor(no, f);
				});
				colorInfo[i][0].slider.onValueChanged.AddListener(delegate(float f)
				{
					OnValueChangeUT(no, f);
				});
				colorInfo[i][0].input.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEditUT(no, s);
				});
				colorInfo[i][0].buttonDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickUTDef(no);
				});
				colorInfo[i][1].slider.onValueChanged.AddListener(delegate(float f)
				{
					OnValueChangeVT(no, f);
				});
				colorInfo[i][1].input.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEditVT(no, s);
				});
				colorInfo[i][1].buttonDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickVTDef(no);
				});
				colorInfo[i][2].slider.onValueChanged.AddListener(delegate(float f)
				{
					OnValueChangeUS(no, f);
				});
				colorInfo[i][2].input.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEditUS(no, s);
				});
				colorInfo[i][2].buttonDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickUSDef(no);
				});
				colorInfo[i][3].slider.onValueChanged.AddListener(delegate(float f)
				{
					OnValueChangeVS(no, f);
				});
				colorInfo[i][3].input.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEditVS(no, s);
				});
				colorInfo[i][3].buttonDefault.OnClickAsObservable().Subscribe(delegate
				{
					OnClickVSDef(no);
				});
				colorInfo[i][4].slider.onValueChanged.AddListener(delegate(float f)
				{
					OnValueChangeRot(no, f);
				});
				colorInfo[i][4].input.onEndEdit.AddListener(delegate(string s)
				{
					OnEndEditRot(no, s);
				});
			}
			colorInfo[3]._colorMain.buttonColor.OnClickAsObservable().Subscribe(delegate
			{
				OnClickColor4();
			});
			colorInfo[3]._colorMain.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickColor4Def();
			});
			panelInfo._buttonTex.OnClickAsObservable().Subscribe(delegate
			{
				OnClickPanel();
			});
			panelInfo._color.buttonColor.OnClickAsObservable().Subscribe(delegate
			{
				OnClickColorPanel();
			});
			panelInfo._toggleClamp.OnValueChangedAsObservable().Subscribe(delegate(bool f)
			{
				OnToggleColor(f);
			});
			panelInfo[0].slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeUT(f);
			});
			panelInfo[0].input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditUT(s);
			});
			panelInfo[0].buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickUTDef();
			});
			panelInfo[1].slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeVT(f);
			});
			panelInfo[1].input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditVT(s);
			});
			panelInfo[1].buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickVTDef();
			});
			panelInfo[2].slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeUS(f);
			});
			panelInfo[2].input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditUS(s);
			});
			panelInfo[2].buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickUSDef();
			});
			panelInfo[3].slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeVS(f);
			});
			panelInfo[3].input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditVS(s);
			});
			panelInfo[3].buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickVSDef();
			});
			panelInfo[4].slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeRot(f);
			});
			panelInfo[4].input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditRot(s);
			});
			colorShadow.buttonColor.OnClickAsObservable().Subscribe(delegate
			{
				OnClickShadow();
			});
			colorShadow.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickShadowDef();
			});
			inputAlpha.slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeAlpha(f);
			});
			inputAlpha.input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditAlpha(s);
			});
			emissionInfo.color.buttonColor.OnClickAsObservable().Subscribe(delegate
			{
				OnClickEmissionColor();
			});
			emissionInfo.color.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickEmissionColorDef();
			});
			emissionInfo.input.slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeEmissionPower(f);
			});
			emissionInfo.input.input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditEmissionPower(s);
			});
			emissionInfo.input.buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickEmissionPowerDef();
			});
			inputLightCancel.slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeLightCancel(f);
			});
			inputLightCancel.input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditLightCancel(s);
			});
			inputLightCancel.buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickLightCancelDef();
			});
			lineInfo.color.buttonColor.OnClickAsObservable().Subscribe(delegate
			{
				OnClickLineColor();
			});
			lineInfo.color.buttonColorDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickLineColorDef();
			});
			lineInfo.input.slider.onValueChanged.AddListener(delegate(float f)
			{
				OnValueChangeLineWidth(f);
			});
			lineInfo.input.input.onEndEdit.AddListener(delegate(string s)
			{
				OnEndEditLineWidth(s);
			});
			lineInfo.input.buttonDefault.OnClickAsObservable().Subscribe(delegate
			{
				OnClickLineWidthDef();
			});
			toggleFK.onValueChanged.AddListener(OnValueChangedFK);
			toggleDynamicBone.onValueChanged.AddListener(OnValueChangedDynamicBone);
			isUpdateInfo = false;
			m_Active = false;
			base.gameObject.SetActive(false);
		}
	}
}
