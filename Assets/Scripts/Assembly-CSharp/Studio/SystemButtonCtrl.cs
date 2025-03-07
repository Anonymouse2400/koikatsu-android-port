using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

namespace Studio
{
	public class SystemButtonCtrl : MonoBehaviour
	{
		[Serializable]
		private class CommonInfo
		{
			public CanvasGroup group;

			public Button button;

			public bool active
			{
				set
				{
					group.Enable(value);
					button.image.color = ((!value) ? Color.white : Color.green);
				}
			}
		}

		[Serializable]
		private class Selector
		{
			public Button _button;

			public TextMeshProUGUI _text;

			public string text
			{
				get
				{
					return _text.text;
				}
				set
				{
					_text.text = value;
				}
			}
		}

		[Serializable]
		private class InputCombination
		{
			public Slider slider;

			public InputField input;

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
					input.text = value.ToString();
				}
			}
		}

		[Serializable]
		private class EffectInfo
		{
			public GameObject obj;

			public Button button;

			public Sprite[] sprite;

			public bool active
			{
				get
				{
					return obj.activeSelf;
				}
				set
				{
					if (obj.SetActiveIfDifferent(value))
					{
						button.image.sprite = sprite[value ? 1 : 0];
					}
				}
			}

			public bool isUpdateInfo { get; set; }

			public virtual void Init(Sprite[] _sprite)
			{
				button.onClick.AddListener(OnClickActive);
				sprite = _sprite;
				isUpdateInfo = false;
			}

			public virtual void UpdateInfo()
			{
			}

			private void OnClickActive()
			{
				active = !active;
			}
		}

		[Serializable]
		private class AmplifyColorEffectInfo : EffectInfo
		{
			public Dropdown dropdownLut;

			public InputCombination icBlend;

			private AmplifyColorEffect ace { get; set; }

			public void Init(Sprite[] _sprite, AmplifyColorEffect _ace)
			{
				base.Init(_sprite);
				ace = _ace;
				dropdownLut.options = Singleton<Info>.Instance.dicFilterLoadInfo.Select((KeyValuePair<int, Info.LoadCommonInfo> v) => new Dropdown.OptionData(v.Value.name)).ToList();
				dropdownLut.onValueChanged.AddListener(OnValueChangedLut);
				icBlend.slider.onValueChanged.AddListener(OnValueChangedBlend);
				icBlend.input.onEndEdit.AddListener(OnEndEditBlend);
				icBlend.buttonDefault.onClick.AddListener(OnClickBlendDef);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				dropdownLut.value = Singleton<Studio>.Instance.sceneInfo.aceNo;
				icBlend.value = Singleton<Studio>.Instance.sceneInfo.aceBlend;
				if (ace != null)
				{
					Singleton<Studio>.Instance.SetACE(Singleton<Studio>.Instance.sceneInfo.aceNo);
					ace.BlendAmount = Singleton<Studio>.Instance.sceneInfo.aceBlend;
				}
				base.isUpdateInfo = false;
			}

			private void OnValueChangedLut(int _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.SetACE(_value);
				}
			}

			private void OnValueChangedBlend(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.aceBlend = _value;
					ace.BlendAmount = _value;
					icBlend.value = _value;
				}
			}

			private void OnEndEditBlend(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.aceBlend = num;
					ace.BlendAmount = num;
					icBlend.text = _text;
				}
			}

			private void OnClickBlendDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.aceBlend = 0f;
					ace.BlendAmount = 0f;
					icBlend.value = 0f;
				}
			}
		}

		[Serializable]
		private class AmplifyOcculusionEffectInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public Button buttonColor;

			public InputCombination icRadius;

			private AmplifyOcclusionEffect aoe { get; set; }

			public void Init(Sprite[] _sprite, AmplifyOcclusionEffect _aoe)
			{
				base.Init(_sprite);
				aoe = _aoe;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
				buttonColor.onClick.AddListener(OnClickColor);
				icRadius.slider.onValueChanged.AddListener(OnValueChangedRadius);
				icRadius.input.onEndEdit.AddListener(OnEndEditRadius);
				icRadius.buttonDefault.onClick.AddListener(OnClickRadiusDef);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableAOE;
				buttonColor.image.color = Singleton<Studio>.Instance.sceneInfo.aoeColor;
				icRadius.value = Singleton<Studio>.Instance.sceneInfo.aoeRadius;
				if (aoe != null)
				{
					aoe.enabled = Singleton<Studio>.Instance.sceneInfo.enableAOE;
					aoe.Tint = Singleton<Studio>.Instance.sceneInfo.aoeColor;
					aoe.Radius = Singleton<Studio>.Instance.sceneInfo.aoeRadius;
				}
				base.isUpdateInfo = false;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.enableVignette = _value;
					aoe.enabled = _value;
				}
			}

			private void OnClickColor()
			{
				if (base.isUpdateInfo)
				{
					return;
				}
				if (Singleton<Studio>.Instance.colorPalette.Check("アンビエントオクルージョン"))
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
					return;
				}
				Singleton<Studio>.Instance.colorPalette.Setup("アンビエントオクルージョン", Singleton<Studio>.Instance.sceneInfo.aoeColor, delegate(Color _c)
				{
					Singleton<Studio>.Instance.sceneInfo.aoeColor = _c;
					buttonColor.image.color = _c;
					aoe.Tint = Singleton<Studio>.Instance.sceneInfo.aoeColor;
				}, false);
			}

			private void OnValueChangedRadius(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.aoeRadius = _value;
					aoe.Radius = _value;
					icRadius.value = _value;
				}
			}

			private void OnEndEditRadius(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.aoeRadius = num;
					aoe.Radius = num;
					icRadius.text = _text;
				}
			}

			private void OnClickRadiusDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.aoeRadius = 0.1f;
					aoe.Radius = 0.1f;
					icRadius.value = 0.1f;
				}
			}
		}

		[Serializable]
		private class BloomInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public InputCombination icIntensity;

			public InputCombination icThreshold;

			public InputCombination icBlur;

			private BloomAndFlares bloomAndFlares { get; set; }

			public void Init(Sprite[] _sprite, BloomAndFlares _bloom)
			{
				base.Init(_sprite);
				bloomAndFlares = _bloom;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
				icIntensity.slider.onValueChanged.AddListener(OnValueChangedIntensity);
				icIntensity.input.onValueChanged.AddListener(OnEndEditIntensity);
				icIntensity.buttonDefault.onClick.AddListener(OnClickIntensityDef);
				icThreshold.slider.onValueChanged.AddListener(OnValueChangedThreshold);
				icThreshold.input.onValueChanged.AddListener(OnEndEditThreshold);
				icThreshold.buttonDefault.onClick.AddListener(OnClickThresholdDef);
				icBlur.slider.onValueChanged.AddListener(OnValueChangedBlur);
				icBlur.input.onValueChanged.AddListener(OnEndEditBlur);
				icBlur.buttonDefault.onClick.AddListener(OnClickBlurDef);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableBloom;
				icIntensity.value = Singleton<Studio>.Instance.sceneInfo.bloomIntensity;
				icThreshold.value = Singleton<Studio>.Instance.sceneInfo.bloomThreshold;
				icBlur.value = Singleton<Studio>.Instance.sceneInfo.bloomBlur;
				if (bloomAndFlares != null)
				{
					bloomAndFlares.enabled = Singleton<Studio>.Instance.sceneInfo.enableBloom;
					bloomAndFlares.bloomIntensity = Singleton<Studio>.Instance.sceneInfo.bloomIntensity;
					bloomAndFlares.bloomThreshold = Singleton<Studio>.Instance.sceneInfo.bloomThreshold;
					bloomAndFlares.sepBlurSpread = Singleton<Studio>.Instance.sceneInfo.bloomBlur;
				}
				base.isUpdateInfo = false;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.enableBloom = _value;
					bloomAndFlares.enabled = _value;
				}
			}

			private void OnValueChangedIntensity(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomIntensity = _value;
					bloomAndFlares.bloomIntensity = _value;
					icIntensity.value = _value;
				}
			}

			private void OnEndEditIntensity(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float bloomIntensity = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 5f);
					Singleton<Studio>.Instance.sceneInfo.bloomIntensity = bloomIntensity;
					bloomAndFlares.bloomIntensity = bloomIntensity;
					icIntensity.text = _text;
				}
			}

			private void OnClickIntensityDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomIntensity = 0.4f;
					bloomAndFlares.bloomIntensity = 0.4f;
					icIntensity.value = 0.4f;
				}
			}

			private void OnValueChangedThreshold(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomThreshold = _value;
					bloomAndFlares.bloomThreshold = _value;
					icThreshold.value = _value;
				}
			}

			private void OnEndEditThreshold(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float bloomThreshold = Mathf.Clamp(Utility.StringToFloat(_text), 0.5f, 1f);
					Singleton<Studio>.Instance.sceneInfo.bloomThreshold = bloomThreshold;
					bloomAndFlares.bloomThreshold = bloomThreshold;
					icThreshold.text = _text;
				}
			}

			private void OnClickThresholdDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomThreshold = 0.6f;
					bloomAndFlares.bloomThreshold = 0.6f;
					icThreshold.value = 0.6f;
				}
			}

			private void OnValueChangedBlur(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomBlur = _value;
					bloomAndFlares.sepBlurSpread = _value;
					icBlur.value = _value;
				}
			}

			private void OnEndEditBlur(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0.1f, 10f);
					Singleton<Studio>.Instance.sceneInfo.bloomBlur = num;
					bloomAndFlares.sepBlurSpread = num;
					icBlur.text = _text;
				}
			}

			private void OnClickBlurDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.bloomBlur = 0.8f;
					bloomAndFlares.sepBlurSpread = 0.8f;
					icBlur.value = 0.8f;
				}
			}
		}

		[Serializable]
		private class DOFInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public InputCombination icFocalSize;

			public InputCombination icAperture;

			private DepthOfField depthOfField { get; set; }

			public void Init(Sprite[] _sprite, DepthOfField _dof)
			{
				base.Init(_sprite);
				depthOfField = _dof;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
				icFocalSize.slider.onValueChanged.AddListener(OnValueChangedFocalSize);
				icFocalSize.input.onValueChanged.AddListener(OnEndEditFocalSize);
				icFocalSize.buttonDefault.onClick.AddListener(OnClickFocalSizeDef);
				icAperture.slider.onValueChanged.AddListener(OnValueChangedAperture);
				icAperture.input.onValueChanged.AddListener(OnEndEditAperture);
				icAperture.buttonDefault.onClick.AddListener(OnClickApertureDef);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableDepth;
				icFocalSize.value = Singleton<Studio>.Instance.sceneInfo.depthFocalSize;
				icAperture.value = Singleton<Studio>.Instance.sceneInfo.depthAperture;
				if (depthOfField != null)
				{
					depthOfField.enabled = Singleton<Studio>.Instance.sceneInfo.enableDepth;
					depthOfField.focalSize = Singleton<Studio>.Instance.sceneInfo.depthFocalSize;
					depthOfField.aperture = Singleton<Studio>.Instance.sceneInfo.depthAperture;
				}
				base.isUpdateInfo = false;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.enableDepth = _value;
					depthOfField.enabled = _value;
				}
			}

			private void OnValueChangedFocalSize(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.depthFocalSize = _value;
					depthOfField.focalSize = _value;
					icFocalSize.value = _value;
				}
			}

			private void OnEndEditFocalSize(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 2f);
					Singleton<Studio>.Instance.sceneInfo.depthFocalSize = num;
					depthOfField.focalSize = num;
					icFocalSize.text = _text;
				}
			}

			private void OnClickFocalSizeDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.depthFocalSize = 0.95f;
					depthOfField.focalSize = 0.95f;
					icFocalSize.value = 0.95f;
				}
			}

			private void OnValueChangedAperture(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.depthAperture = _value;
					depthOfField.aperture = _value;
					icAperture.value = _value;
				}
			}

			private void OnEndEditAperture(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.depthAperture = num;
					depthOfField.aperture = num;
					icAperture.text = _text;
				}
			}

			private void OnClickApertureDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.depthAperture = 0.6f;
					depthOfField.aperture = 0.6f;
					icAperture.value = 0.6f;
				}
			}
		}

		[Serializable]
		private class VignetteInfo : EffectInfo
		{
			public Toggle toggleEnable;

			private VignetteAndChromaticAberration vignette { get; set; }

			public void Init(Sprite[] _sprite, VignetteAndChromaticAberration _vignette)
			{
				base.Init(_sprite);
				vignette = _vignette;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableVignette;
				if (vignette != null)
				{
					vignette.enabled = Singleton<Studio>.Instance.sceneInfo.enableVignette;
				}
				base.isUpdateInfo = false;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.enableVignette = _value;
					vignette.enabled = _value;
				}
			}
		}

		[Serializable]
		private class FogInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public Button buttonColor;

			public InputCombination icHeight;

			public InputCombination icStartDistance;

			private GlobalFog globalFog { get; set; }

			public void Init(Sprite[] _sprite, GlobalFog _fog)
			{
				base.Init(_sprite);
				globalFog = _fog;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
				buttonColor.onClick.AddListener(OnClickColor);
				icHeight.slider.onValueChanged.AddListener(OnValueChangedHeight);
				icHeight.input.onValueChanged.AddListener(OnEndEditHeight);
				icHeight.buttonDefault.onClick.AddListener(OnClickHeightDef);
				icStartDistance.slider.onValueChanged.AddListener(OnValueChangedStartDistance);
				icStartDistance.input.onValueChanged.AddListener(OnEndEditStartDistance);
				icStartDistance.buttonDefault.onClick.AddListener(OnClickStartDistanceDef);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableFog;
				buttonColor.image.color = Singleton<Studio>.Instance.sceneInfo.fogColor;
				icHeight.value = Singleton<Studio>.Instance.sceneInfo.fogHeight;
				icStartDistance.value = Singleton<Studio>.Instance.sceneInfo.fogStartDistance;
				if (globalFog != null)
				{
					RenderSettings.fog = Singleton<Studio>.Instance.sceneInfo.enableFog;
					RenderSettings.fogColor = Singleton<Studio>.Instance.sceneInfo.fogColor;
					RenderSettings.fogStartDistance = Singleton<Studio>.Instance.sceneInfo.fogStartDistance;
					globalFog.enabled = Singleton<Studio>.Instance.sceneInfo.enableFog;
					globalFog.height = Singleton<Studio>.Instance.sceneInfo.fogHeight;
					globalFog.startDistance = Singleton<Studio>.Instance.sceneInfo.fogStartDistance;
				}
				base.isUpdateInfo = false;
			}

			public void SetEnable(bool _value, bool _UI = true)
			{
				Singleton<Studio>.Instance.sceneInfo.enableFog = _value;
				globalFog.enabled = _value;
				RenderSettings.fog = _value;
				if (_UI)
				{
					toggleEnable.isOn = _value;
				}
			}

			public void SetColor(Color _color)
			{
				Singleton<Studio>.Instance.sceneInfo.fogColor = _color;
				RenderSettings.fogColor = _color;
				buttonColor.image.color = _color;
			}

			public void SetStartDistance(float _value)
			{
				Singleton<Studio>.Instance.sceneInfo.fogStartDistance = _value;
				RenderSettings.fogStartDistance = _value;
				globalFog.startDistance = _value;
				icStartDistance.value = _value;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					SetEnable(_value, false);
				}
			}

			private void OnClickColor()
			{
				if (!base.isUpdateInfo)
				{
					if (Singleton<Studio>.Instance.colorPalette.Check("フォグ"))
					{
						Singleton<Studio>.Instance.colorPalette.visible = false;
					}
					else
					{
						Singleton<Studio>.Instance.colorPalette.Setup("フォグ", Singleton<Studio>.Instance.sceneInfo.fogColor, SetColor, false);
					}
				}
			}

			private void OnValueChangedHeight(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.fogHeight = _value;
					globalFog.height = _value;
					icHeight.value = _value;
				}
			}

			private void OnEndEditHeight(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 50f);
					Singleton<Studio>.Instance.sceneInfo.fogHeight = num;
					globalFog.height = num;
					icHeight.text = _text;
				}
			}

			private void OnClickHeightDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.fogHeight = 1f;
					globalFog.height = 1f;
					icHeight.value = 1f;
				}
			}

			private void OnValueChangedStartDistance(float _value)
			{
				if (!base.isUpdateInfo)
				{
					SetStartDistance(_value);
				}
			}

			private void OnEndEditStartDistance(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float num = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 50f);
					Singleton<Studio>.Instance.sceneInfo.fogStartDistance = num;
					RenderSettings.fogStartDistance = num;
					globalFog.startDistance = num;
					icStartDistance.text = _text;
				}
			}

			private void OnClickStartDistanceDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.fogStartDistance = 0f;
					RenderSettings.fogStartDistance = 0f;
					globalFog.startDistance = 0f;
					icStartDistance.value = 0f;
				}
			}
		}

		[Serializable]
		private class SunShaftsInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public Button buttonThresholdColor;

			public Button buttonShaftsColor;

			public Selector selectorCaster;

			private SunShafts sunShafts { get; set; }

			public void Init(Sprite[] _sprite, SunShafts _sunShafts)
			{
				base.Init(_sprite);
				sunShafts = _sunShafts;
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
				buttonThresholdColor.onClick.AddListener(OnClickThresholdColor);
				buttonShaftsColor.onClick.AddListener(OnClickShaftsColor);
				selectorCaster._button.onClick.AddListener(OnClickCaster);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableSunShafts;
				buttonThresholdColor.image.color = Singleton<Studio>.Instance.sceneInfo.sunThresholdColor;
				buttonShaftsColor.image.color = Singleton<Studio>.Instance.sceneInfo.sunColor;
				Singleton<Studio>.Instance.SetSunCaster(Singleton<Studio>.Instance.sceneInfo.sunCaster);
				if (sunShafts != null)
				{
					sunShafts.enabled = Singleton<Studio>.Instance.sceneInfo.enableSunShafts;
					sunShafts.sunThreshold = Singleton<Studio>.Instance.sceneInfo.sunThresholdColor;
					sunShafts.sunColor = Singleton<Studio>.Instance.sceneInfo.sunColor;
				}
				base.isUpdateInfo = false;
			}

			public void SetShaftsColor(Color _color)
			{
				Singleton<Studio>.Instance.sceneInfo.sunColor = _color;
				buttonShaftsColor.image.color = _color;
				sunShafts.sunColor = _color;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.enableSunShafts = _value;
					sunShafts.enabled = _value;
				}
			}

			private void OnClickThresholdColor()
			{
				if (base.isUpdateInfo)
				{
					return;
				}
				if (Singleton<Studio>.Instance.colorPalette.Check("サンシャフト しきい色"))
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
					return;
				}
				Singleton<Studio>.Instance.colorPalette.Setup("サンシャフト しきい色", Singleton<Studio>.Instance.sceneInfo.sunThresholdColor, delegate(Color _c)
				{
					Singleton<Studio>.Instance.sceneInfo.sunThresholdColor = _c;
					buttonThresholdColor.image.color = _c;
					sunShafts.sunThreshold = _c;
				}, false);
			}

			private void OnClickShaftsColor()
			{
				if (!base.isUpdateInfo)
				{
					if (Singleton<Studio>.Instance.colorPalette.Check("サンシャフト 光の色"))
					{
						Singleton<Studio>.Instance.colorPalette.visible = false;
					}
					else
					{
						Singleton<Studio>.Instance.colorPalette.Setup("サンシャフト 光の色", Singleton<Studio>.Instance.sceneInfo.sunColor, SetShaftsColor, false);
					}
				}
			}

			private void OnClickCaster()
			{
				if (!base.isUpdateInfo)
				{
					GuideObject selectObject = Singleton<GuideObjectManager>.Instance.selectObject;
					Singleton<Studio>.Instance.SetSunCaster((!(selectObject != null)) ? (-1) : selectObject.dicKey);
				}
			}
		}

		[Serializable]
		private class SelfShadowInfo : EffectInfo
		{
			public Toggle toggleEnable;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				toggleEnable.onValueChanged.AddListener(OnValueChangedEnable);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				toggleEnable.isOn = Singleton<Studio>.Instance.sceneInfo.enableShadow;
				Set(Singleton<Studio>.Instance.sceneInfo.enableShadow);
				base.isUpdateInfo = false;
			}

			private void OnValueChangedEnable(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Set(_value);
				}
			}

			private void Set(bool _value)
			{
				int qualityLevel = QualitySettings.GetQualityLevel() / 2 * 2 + ((!_value) ? 1 : 0);
				QualitySettings.SetQualityLevel(qualityLevel);
			}
		}

		[Serializable]
		private class EtcInfo : EffectInfo
		{
			public Toggle toggleFaceNormal;

			public Toggle toggleFaceShadow;

			public Dropdown dropdownRamp;

			public InputCombination icAmbientShadow;

			public InputCombination icLineColor;

			public InputCombination icLineWidth;

			public Button buttonAmbientShadow;

			private Texture texRamp;

			private List<int> lstID;

			public override void Init(Sprite[] _sprite)
			{
				base.Init(_sprite);
				Dictionary<int, ListInfoBase> dic = Singleton<Character>.Instance.chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mt_ramp);
				lstID = dic.Select((KeyValuePair<int, ListInfoBase> v) => v.Value.Id).ToList();
				dropdownRamp.options = lstID.Select((int id) => new Dropdown.OptionData(dic[id].Name)).ToList();
				dropdownRamp.onValueChanged.AddListener(OnValueChangedRamp);
				icAmbientShadow.slider.onValueChanged.AddListener(OnValueChangedAmbientShadow);
				icAmbientShadow.input.onValueChanged.AddListener(OnEndEditAmbientShadow);
				icAmbientShadow.buttonDefault.onClick.AddListener(OnClickAmbientShadowDef);
				icLineColor.slider.onValueChanged.AddListener(OnValueChangedLineColor);
				icLineColor.input.onValueChanged.AddListener(OnEndEditLineColor);
				icLineColor.buttonDefault.onClick.AddListener(OnClickLineColorDef);
				icLineWidth.slider.onValueChanged.AddListener(OnValueChangedLineWidth);
				icLineWidth.input.onValueChanged.AddListener(OnEndEditLineWidth);
				icLineWidth.buttonDefault.onClick.AddListener(OnClickLineWidthDef);
				buttonAmbientShadow.onClick.AddListener(OnClickAmbientShadow);
			}

			public override void UpdateInfo()
			{
				base.UpdateInfo();
				base.isUpdateInfo = true;
				dropdownRamp.value = lstID.FindIndex((int id) => id == Singleton<Studio>.Instance.sceneInfo.rampG);
				icAmbientShadow.value = Singleton<Studio>.Instance.sceneInfo.ambientShadowG;
				icLineColor.value = Singleton<Studio>.Instance.sceneInfo.lineColorG;
				icLineWidth.value = Singleton<Studio>.Instance.sceneInfo.lineWidthG;
				buttonAmbientShadow.image.color = Singleton<Studio>.Instance.sceneInfo.ambientShadow;
				SetRamp();
				SetLineColor();
				SetLineWidth();
				SetAmbientShadow();
				base.isUpdateInfo = false;
			}

			private void OnValueChangedFaceNormal(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.faceNormal = _value;
					SetFaceNormal();
				}
			}

			private void OnValueChangedFaceShadow(bool _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.faceShadow = _value;
					SetFaceShadow();
				}
			}

			private void OnValueChangedRamp(int _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.rampG = lstID[_value];
					SetRamp();
				}
			}

			private void OnValueChangedAmbientShadow(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.ambientShadowG = _value;
					SetAmbientShadow();
					icAmbientShadow.value = _value;
				}
			}

			private void OnEndEditAmbientShadow(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float ambientShadowG = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.ambientShadowG = ambientShadowG;
					SetAmbientShadow();
					icAmbientShadow.text = _text;
				}
			}

			private void OnClickAmbientShadowDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.ambientShadowG = Manager.Config.EtcData.shadowDepth;
					SetAmbientShadow();
					icAmbientShadow.value = Manager.Config.EtcData.shadowDepth;
				}
			}

			private void OnValueChangedLineColor(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.lineColorG = _value;
					SetLineColor();
					icLineColor.value = _value;
				}
			}

			private void OnEndEditLineColor(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float lineColorG = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.lineColorG = lineColorG;
					SetLineColor();
					icLineColor.text = _text;
				}
			}

			private void OnClickLineColorDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.lineColorG = Manager.Config.EtcData.lineDepth;
					SetLineColor();
					icLineColor.value = Manager.Config.EtcData.lineDepth;
				}
			}

			private void OnValueChangedLineWidth(float _value)
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.lineWidthG = _value;
					SetLineWidth();
					icLineWidth.value = _value;
				}
			}

			private void OnEndEditLineWidth(string _text)
			{
				if (!base.isUpdateInfo)
				{
					float lineWidthG = Mathf.Clamp(Utility.StringToFloat(_text), 0f, 1f);
					Singleton<Studio>.Instance.sceneInfo.lineWidthG = lineWidthG;
					SetLineWidth();
					icLineWidth.text = _text;
				}
			}

			private void OnClickLineWidthDef()
			{
				if (!base.isUpdateInfo)
				{
					Singleton<Studio>.Instance.sceneInfo.lineWidthG = Manager.Config.EtcData.lineWidth;
					SetLineWidth();
					icLineWidth.value = Manager.Config.EtcData.lineWidth;
				}
			}

			private void OnClickAmbientShadow()
			{
				if (base.isUpdateInfo)
				{
					return;
				}
				if (Singleton<Studio>.Instance.colorPalette.Check("全体の陰影の色"))
				{
					Singleton<Studio>.Instance.colorPalette.visible = false;
					return;
				}
				Singleton<Studio>.Instance.colorPalette.Setup("全体の陰影の色", Singleton<Studio>.Instance.sceneInfo.ambientShadow, delegate(Color _c)
				{
					Singleton<Studio>.Instance.sceneInfo.ambientShadow = _c;
					buttonAmbientShadow.image.color = _c;
					SetAmbientShadow();
				}, false);
			}

			public void SetFaceNormal()
			{
			}

			public void SetFaceShadow()
			{
			}

			public void SetRamp()
			{
				ListInfoBase listInfo = Singleton<Character>.Instance.chaListCtrl.GetListInfo(ChaListDefine.CategoryNo.mt_ramp, Singleton<Studio>.Instance.sceneInfo.rampG);
				if (listInfo != null)
				{
					string info = listInfo.GetInfo(ChaListDefine.KeyType.MainTexAB);
					string info2 = listInfo.GetInfo(ChaListDefine.KeyType.MainTex);
					if ("0" != info && "0" != info2)
					{
						texRamp = CommonLib.LoadAsset<Texture2D>(info, info2, false, string.Empty);
					}
					if ((bool)texRamp)
					{
						Shader.SetGlobalTexture(ChaShader._RampG, texRamp);
					}
				}
			}

			public void SetLineColor()
			{
				Shader.SetGlobalColor(value: new Color(0.5f, 0.5f, 0.5f, 1f - Singleton<Studio>.Instance.sceneInfo.lineColorG), nameID: ChaShader._LineColorG);
			}

			public void SetLineWidth()
			{
				Shader.SetGlobalFloat(ChaShader._linewidthG, Singleton<Studio>.Instance.sceneInfo.lineWidthG);
			}

			public void SetAmbientShadow()
			{
				Color ambientShadow = Singleton<Studio>.Instance.sceneInfo.ambientShadow;
				ambientShadow.a = 1f - Singleton<Studio>.Instance.sceneInfo.ambientShadowG;
				Shader.SetGlobalColor(ChaShader._ambientshadowG, ambientShadow);
			}
		}

		[SerializeField]
		private CommonInfo[] commonInfo = new CommonInfo[5];

		private int select = -1;

		[SerializeField]
		private Sprite spriteSave;

		[SerializeField]
		private Sprite spriteInit;

		[SerializeField]
		private AmplifyColorEffect amplifyColorEffect;

		[SerializeField]
		private AmplifyOcclusionEffect amplifyOcclusionEffect;

		[SerializeField]
		private BloomAndFlares bloomAndFlares;

		[SerializeField]
		private DepthOfField depthOfField;

		[SerializeField]
		private VignetteAndChromaticAberration vignetteAndChromaticAberration;

		[SerializeField]
		private GlobalFog globalFog;

		[SerializeField]
		private SunShafts _sunShafts;

		[SerializeField]
		private Sprite[] spriteExpansion;

		[SerializeField]
		private AmplifyColorEffectInfo amplifyColorEffectInfo = new AmplifyColorEffectInfo();

		[SerializeField]
		private AmplifyOcculusionEffectInfo amplifyOcculusionEffectInfo = new AmplifyOcculusionEffectInfo();

		[SerializeField]
		private BloomInfo bloomInfo = new BloomInfo();

		[SerializeField]
		private DOFInfo dofInfo = new DOFInfo();

		[SerializeField]
		private VignetteInfo vignetteInfo = new VignetteInfo();

		[SerializeField]
		private FogInfo fogInfo = new FogInfo();

		[SerializeField]
		private SunShaftsInfo sunShaftsInfo = new SunShaftsInfo();

		[SerializeField]
		private SelfShadowInfo selfShadowInfo = new SelfShadowInfo();

		[SerializeField]
		private EtcInfo etcInfo = new EtcInfo();

		private bool isInit;

		public SunShafts sunShafts
		{
			get
			{
				return _sunShafts;
			}
		}

		public bool visible
		{
			set
			{
				if (value)
				{
					commonInfo.SafeProc(select, delegate(CommonInfo v)
					{
						v.active = true;
					});
					return;
				}
				for (int i = 0; i < commonInfo.Length; i++)
				{
					commonInfo[i].active = false;
				}
			}
		}

		public void OnClickSelect(int _idx)
		{
			if (MathfEx.RangeEqualOn(0, select, commonInfo.Length - 1))
			{
				commonInfo[select].active = false;
				if (select == 2)
				{
					Singleton<Studio>.Instance.SaveOption();
				}
			}
			select = ((select != _idx) ? _idx : (-1));
			if (MathfEx.RangeEqualOn(0, select, commonInfo.Length - 1))
			{
				commonInfo[select].active = true;
			}
			Singleton<Studio>.Instance.colorPalette.visible = false;
		}

		public void OnClickSave()
		{
			Singleton<Studio>.Instance.colorPalette.visible = false;
			Singleton<Studio>.Instance.SaveScene();
			NotificationScene.spriteMessage = spriteSave;
			NotificationScene.waitTime = 1f;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioNotification",
				isAdd = true
			}, false);
		}

		public void OnClickLoad()
		{
			Singleton<Studio>.Instance.colorPalette.visible = false;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioSceneLoad",
				isAdd = true
			}, false);
		}

		public void OnClickInit()
		{
			Singleton<Studio>.Instance.colorPalette.visible = false;
			CheckScene.sprite = spriteInit;
			CheckScene.unityActionYes = OnSelectInitYes;
			CheckScene.unityActionNo = OnSelectIniteNo;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioCheck",
				isAdd = true
			}, false);
		}

		private void OnSelectInitYes()
		{
			Singleton<Scene>.Instance.UnLoad();
			Singleton<Studio>.Instance.InitScene();
		}

		private void OnSelectIniteNo()
		{
			Singleton<Scene>.Instance.UnLoad();
		}

		public void OnClickEnd()
		{
			Singleton<Studio>.Instance.colorPalette.visible = false;
			Singleton<Scene>.Instance.GameEnd();
		}

		public void Init()
		{
			if (!isInit)
			{
				GameObject gameObject = Camera.main.gameObject;
				if (bloomAndFlares == null)
				{
					bloomAndFlares = gameObject.GetComponent<BloomAndFlares>();
				}
				if (depthOfField == null)
				{
					depthOfField = gameObject.GetComponent<DepthOfField>();
				}
				if (vignetteAndChromaticAberration == null)
				{
					vignetteAndChromaticAberration = gameObject.GetComponent<VignetteAndChromaticAberration>();
				}
				if (globalFog == null)
				{
					globalFog = gameObject.GetComponent<GlobalFog>();
				}
				if (_sunShafts == null)
				{
					_sunShafts = gameObject.GetComponent<SunShafts>();
				}
				for (int i = 0; i < commonInfo.Length; i++)
				{
					commonInfo[i].active = false;
				}
				amplifyColorEffectInfo.Init(spriteExpansion, amplifyColorEffect);
				amplifyOcculusionEffectInfo.Init(spriteExpansion, amplifyOcclusionEffect);
				bloomInfo.Init(spriteExpansion, bloomAndFlares);
				dofInfo.Init(spriteExpansion, depthOfField);
				vignetteInfo.Init(spriteExpansion, vignetteAndChromaticAberration);
				fogInfo.Init(spriteExpansion, globalFog);
				sunShaftsInfo.Init(spriteExpansion, sunShafts);
				selfShadowInfo.Init(spriteExpansion);
				etcInfo.Init(spriteExpansion);
				isInit = true;
				UpdateInfo();
			}
		}

		public void UpdateInfo()
		{
			amplifyColorEffectInfo.UpdateInfo();
			amplifyOcculusionEffectInfo.UpdateInfo();
			bloomInfo.UpdateInfo();
			dofInfo.UpdateInfo();
			vignetteInfo.UpdateInfo();
			fogInfo.UpdateInfo();
			sunShaftsInfo.UpdateInfo();
			selfShadowInfo.UpdateInfo();
			etcInfo.UpdateInfo();
		}

		public void SetACE(int _no)
		{
			Singleton<Studio>.Instance.sceneInfo.aceNo = _no;
			Info.LoadCommonInfo value = null;
			if (Singleton<Info>.Instance.dicFilterLoadInfo.TryGetValue(_no, out value))
			{
				Texture lutTexture = CommonLib.LoadAsset<Texture>(value.bundlePath, value.fileName, false, string.Empty);
				amplifyColorEffect.LutTexture = lutTexture;
			}
		}

		public void SetSunCaster(int _key)
		{
			Transform sunTransform = null;
			string text = "None";
			ObjectCtrlInfo ctrlInfo = Studio.GetCtrlInfo(_key);
			if (ctrlInfo == null || ctrlInfo.kind != 1)
			{
				Singleton<Studio>.Instance.sceneInfo.sunCaster = -1;
			}
			else
			{
				Singleton<Studio>.Instance.sceneInfo.sunCaster = _key;
				sunTransform = (ctrlInfo as OCIItem).objectItem.transform;
				text = (ctrlInfo as OCIItem).treeNodeObject.textName;
			}
			if (Singleton<Map>.Instance.isSunLightInfo)
			{
				sunShaftsInfo.selectorCaster.text = "マップ依存";
				return;
			}
			sunShafts.sunTransform = sunTransform;
			sunShaftsInfo.selectorCaster.text = text;
		}

		public void MapDependent()
		{
			if (Singleton<Map>.Instance.isSunLightInfo)
			{
				SunLightInfo.Info mapEffectInfo = Singleton<Map>.Instance.mapEffectInfo;
				sunShaftsInfo.selectorCaster.text = "マップ依存";
				sunShaftsInfo.SetShaftsColor(mapEffectInfo.sunShaftsColor);
				fogInfo.SetEnable(Singleton<Studio>.Instance.sceneInfo.enableFog);
			}
		}
	}
}
