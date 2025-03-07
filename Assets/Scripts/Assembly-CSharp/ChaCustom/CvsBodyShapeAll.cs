using System.Collections;
using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsBodyShapeAll : MonoBehaviour
	{
		[SerializeField]
		private Button btnMin;

		[SerializeField]
		private Button btnDefault;

		[SerializeField]
		private Button btnMax;

		[SerializeField]
		[Header("全体")]
		private Slider sldHeight;

		[SerializeField]
		private TMP_InputField inpHeight;

		[SerializeField]
		private Button btnHeight;

		[SerializeField]
		private Slider sldHeadSize;

		[SerializeField]
		private TMP_InputField inpHeadSize;

		[SerializeField]
		private Button btnHeadSize;

		[Header("胸")]
		[SerializeField]
		private Slider sldBustSize;

		[SerializeField]
		private TMP_InputField inpBustSize;

		[SerializeField]
		private Button btnBustSize;

		[SerializeField]
		private Slider sldBustY;

		[SerializeField]
		private TMP_InputField inpBustY;

		[SerializeField]
		private Button btnBustY;

		[SerializeField]
		private Slider sldBustRotX;

		[SerializeField]
		private TMP_InputField inpBustRotX;

		[SerializeField]
		private Button btnBustRotX;

		[SerializeField]
		private Slider sldBustX;

		[SerializeField]
		private TMP_InputField inpBustX;

		[SerializeField]
		private Button btnBustX;

		[SerializeField]
		private Slider sldBustRotY;

		[SerializeField]
		private TMP_InputField inpBustRotY;

		[SerializeField]
		private Button btnBustRotY;

		[SerializeField]
		private Slider sldBustSharp;

		[SerializeField]
		private TMP_InputField inpBustSharp;

		[SerializeField]
		private Button btnBustSharp;

		[SerializeField]
		private Slider sldBustForm;

		[SerializeField]
		private TMP_InputField inpBustForm;

		[SerializeField]
		private Button btnBustForm;

		[Header("胸の柔らかさ")]
		[SerializeField]
		private Slider sldBustSoftness;

		[SerializeField]
		private TMP_InputField inpBustSoftness;

		[SerializeField]
		private Button btnBustSoftness;

		[SerializeField]
		[Header("胸の重さ")]
		private Slider sldBustWeight;

		[SerializeField]
		private TMP_InputField inpBustWeight;

		[SerializeField]
		private Button btnBustWeight;

		[SerializeField]
		[Header("")]
		private Slider sldAreolaBulge;

		[SerializeField]
		private TMP_InputField inpAreolaBulge;

		[SerializeField]
		private Button btnAreolaBulge;

		[SerializeField]
		private Slider sldNipWeight;

		[SerializeField]
		private TMP_InputField inpNipWeight;

		[SerializeField]
		private Button btnNipWeight;

		[SerializeField]
		private Slider sldNipStand;

		[SerializeField]
		private TMP_InputField inpNipStand;

		[SerializeField]
		private Button btnNipStand;

		[SerializeField]
		[Header("乳輪の大きさ")]
		private Slider sldAreolaSize;

		[SerializeField]
		private TMP_InputField inpAreolaSize;

		[SerializeField]
		private Button btnAreolaSize;

		[SerializeField]
		[Header("上半身")]
		[Header("")]
		private Slider sldNeckW;

		[SerializeField]
		private TMP_InputField inpNeckW;

		[SerializeField]
		private Button btnNeckW;

		[SerializeField]
		private Slider sldNeckZ;

		[SerializeField]
		private TMP_InputField inpNeckZ;

		[SerializeField]
		private Button btnNeckZ;

		[SerializeField]
		private Slider sldBodyShoulderW;

		[SerializeField]
		private TMP_InputField inpBodyShoulderW;

		[SerializeField]
		private Button btnBodyShoulderW;

		[SerializeField]
		private Slider sldBodyShoulderZ;

		[SerializeField]
		private TMP_InputField inpBodyShoulderZ;

		[SerializeField]
		private Button btnBodyShoulderZ;

		[SerializeField]
		private Slider sldBodyUpW;

		[SerializeField]
		private TMP_InputField inpBodyUpW;

		[SerializeField]
		private Button btnBodyUpW;

		[SerializeField]
		private Slider sldBodyUpZ;

		[SerializeField]
		private TMP_InputField inpBodyUpZ;

		[SerializeField]
		private Button btnBodyUpZ;

		[SerializeField]
		private Slider sldBodyLowW;

		[SerializeField]
		private TMP_InputField inpBodyLowW;

		[SerializeField]
		private Button btnBodyLowW;

		[SerializeField]
		private Slider sldBodyLowZ;

		[SerializeField]
		private TMP_InputField inpBodyLowZ;

		[SerializeField]
		private Button btnBodyLowZ;

		[Header("下半身")]
		[SerializeField]
		private Slider sldWaistY;

		[SerializeField]
		private TMP_InputField inpWaistY;

		[SerializeField]
		private Button btnWaistY;

		[SerializeField]
		private Slider sldBelly;

		[SerializeField]
		private TMP_InputField inpBelly;

		[SerializeField]
		private Button btnBelly;

		[SerializeField]
		private Slider sldWaistUpW;

		[SerializeField]
		private TMP_InputField inpWaistUpW;

		[SerializeField]
		private Button btnWaistUpW;

		[SerializeField]
		private Slider sldWaistUpZ;

		[SerializeField]
		private TMP_InputField inpWaistUpZ;

		[SerializeField]
		private Button btnWaistUpZ;

		[SerializeField]
		private Slider sldWaistLowW;

		[SerializeField]
		private TMP_InputField inpWaistLowW;

		[SerializeField]
		private Button btnWaistLowW;

		[SerializeField]
		private Slider sldWaistLowZ;

		[SerializeField]
		private TMP_InputField inpWaistLowZ;

		[SerializeField]
		private Button btnWaistLowZ;

		[SerializeField]
		private Slider sldHip;

		[SerializeField]
		private TMP_InputField inpHip;

		[SerializeField]
		private Button btnHip;

		[SerializeField]
		private Slider sldHipRotX;

		[SerializeField]
		private TMP_InputField inpHipRotX;

		[SerializeField]
		private Button btnHipRotX;

		[Header("腕")]
		[SerializeField]
		private Slider sldShoulderW;

		[SerializeField]
		private TMP_InputField inpShoulderW;

		[SerializeField]
		private Button btnShoulderW;

		[SerializeField]
		private Slider sldShoulderZ;

		[SerializeField]
		private TMP_InputField inpShoulderZ;

		[SerializeField]
		private Button btnShoulderZ;

		[SerializeField]
		private Slider sldArmUpW;

		[SerializeField]
		private TMP_InputField inpArmUpW;

		[SerializeField]
		private Button btnArmUpW;

		[SerializeField]
		private Slider sldArmUpZ;

		[SerializeField]
		private TMP_InputField inpArmUpZ;

		[SerializeField]
		private Button btnArmUpZ;

		[SerializeField]
		private Slider sldElbowW;

		[SerializeField]
		private TMP_InputField inpElbowW;

		[SerializeField]
		private Button btnElbowW;

		[SerializeField]
		private Slider sldElbowZ;

		[SerializeField]
		private TMP_InputField inpElbowZ;

		[SerializeField]
		private Button btnElbowZ;

		[SerializeField]
		private Slider sldArmLow;

		[SerializeField]
		private TMP_InputField inpArmLow;

		[SerializeField]
		private Button btnArmLow;

		[Header("脚")]
		[SerializeField]
		private Slider sldThighUpW;

		[SerializeField]
		private TMP_InputField inpThighUpW;

		[SerializeField]
		private Button btnThighUpW;

		[SerializeField]
		private Slider sldThighUpZ;

		[SerializeField]
		private TMP_InputField inpThighUpZ;

		[SerializeField]
		private Button btnThighUpZ;

		[SerializeField]
		private Slider sldThighLowW;

		[SerializeField]
		private TMP_InputField inpThighLowW;

		[SerializeField]
		private Button btnThighLowW;

		[SerializeField]
		private Slider sldThighLowZ;

		[SerializeField]
		private TMP_InputField inpThighLowZ;

		[SerializeField]
		private Button btnThighLowZ;

		[SerializeField]
		private Slider sldKneeLowW;

		[SerializeField]
		private TMP_InputField inpKneeLowW;

		[SerializeField]
		private Button btnKneeLowW;

		[SerializeField]
		private Slider sldKneeLowZ;

		[SerializeField]
		private TMP_InputField inpKneeLowZ;

		[SerializeField]
		private Button btnKneeLowZ;

		[SerializeField]
		private Slider sldCalf;

		[SerializeField]
		private TMP_InputField inpCalf;

		[SerializeField]
		private Button btnCalf;

		[SerializeField]
		private Slider sldAnkleW;

		[SerializeField]
		private TMP_InputField inpAnkleW;

		[SerializeField]
		private Button btnAnkleW;

		[SerializeField]
		private Slider sldAnkleZ;

		[SerializeField]
		private TMP_InputField inpAnkleZ;

		[SerializeField]
		private Button btnAnkleZ;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[44]
		{
			0, 1, 4, 5, 6, 7, 8, 9, 10, 11,
			12, 13, 2, 3, 14, 15, 16, 17, 18, 19,
			20, 21, 22, 23, 24, 25, 26, 27, 37, 38,
			39, 40, 41, 42, 43, 28, 29, 30, 31, 32,
			33, 34, 35, 36
		};

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileBody body
		{
			get
			{
				return chaCtrl.chaFile.custom.body;
			}
		}

		public void CalculateUI()
		{
			float[] shapeValueBody = body.shapeValueBody;
			for (int i = 0; i < sliders.Length; i++)
			{
				if (!(null == sliders[i]))
				{
					sliders[i].value = shapeValueBody[arrIndex[i]];
				}
			}
			sldBustSoftness.value = body.bustSoftness;
			sldBustWeight.value = body.bustWeight;
			sldAreolaSize.value = body.areolaSize;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			float[] shape = body.shapeValueBody;
			for (int i = 0; i < inputs.Length; i++)
			{
				if (!(null == inputs[i]))
				{
					inputs[i].text = CustomBase.ConvertTextFromRate(0, 100, shape[arrIndex[i]]);
				}
			}
			inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, body.bustSoftness);
			inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, body.bustWeight);
			inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, body.areolaSize);
		}

		public bool SetMin()
		{
			chaCtrl.UpdateShapeBodyValueFromCustomInfo();
			chaCtrl.UpdateBustSoftnessAndGravity();
			chaCtrl.ChangeSettingAreolaSize();
			return true;
		}

		public bool SetDefault()
		{
			chaCtrl.UpdateShapeBodyValueFromCustomInfo();
			chaCtrl.UpdateBustSoftnessAndGravity();
			chaCtrl.ChangeSettingAreolaSize();
			return true;
		}

		public bool SetMax()
		{
			chaCtrl.UpdateShapeBodyValueFromCustomInfo();
			chaCtrl.UpdateBustSoftnessAndGravity();
			chaCtrl.ChangeSettingAreolaSize();
			return true;
		}

		protected virtual void Awake()
		{
			sliders = new Slider[44]
			{
				sldHeight, sldHeadSize, sldBustSize, sldBustY, sldBustRotX, sldBustX, sldBustRotY, sldBustSharp, sldBustForm, sldAreolaBulge,
				sldNipWeight, sldNipStand, sldNeckW, sldNeckZ, sldBodyShoulderW, sldBodyShoulderZ, sldBodyUpW, sldBodyUpZ, sldBodyLowW, sldBodyLowZ,
				sldWaistY, sldBelly, sldWaistUpW, sldWaistUpZ, sldWaistLowW, sldWaistLowZ, sldHip, sldHipRotX, sldShoulderW, sldShoulderZ,
				sldArmUpW, sldArmUpZ, sldElbowW, sldElbowZ, sldArmLow, sldThighUpW, sldThighUpZ, sldThighLowW, sldThighLowZ, sldKneeLowW,
				sldKneeLowZ, sldCalf, sldAnkleW, sldAnkleZ
			};
			inputs = new TMP_InputField[44]
			{
				inpHeight, inpHeadSize, inpBustSize, inpBustY, inpBustRotX, inpBustX, inpBustRotY, inpBustSharp, inpBustForm, inpAreolaBulge,
				inpNipWeight, inpNipStand, inpNeckW, inpNeckZ, inpBodyShoulderW, inpBodyShoulderZ, inpBodyUpW, inpBodyUpZ, inpBodyLowW, inpBodyLowZ,
				inpWaistY, inpBelly, inpWaistUpW, inpWaistUpZ, inpWaistLowW, inpWaistLowZ, inpHip, inpHipRotX, inpShoulderW, inpShoulderZ,
				inpArmUpW, inpArmUpZ, inpElbowW, inpElbowZ, inpArmLow, inpThighUpW, inpThighUpZ, inpThighLowW, inpThighLowZ, inpKneeLowW,
				inpKneeLowZ, inpCalf, inpAnkleW, inpAnkleZ
			};
			buttons = new Button[44]
			{
				btnHeight, btnHeadSize, btnBustSize, btnBustY, btnBustRotX, btnBustX, btnBustRotY, btnBustSharp, btnBustForm, btnAreolaBulge,
				btnNipWeight, btnNipStand, btnNeckW, btnNeckZ, btnBodyShoulderW, btnBodyShoulderZ, btnBodyUpW, btnBodyUpZ, btnBodyLowW, btnBodyLowZ,
				btnWaistY, btnBelly, btnWaistUpW, btnWaistUpZ, btnWaistLowW, btnWaistLowZ, btnHip, btnHipRotX, btnShoulderW, btnShoulderZ,
				btnArmUpW, btnArmUpZ, btnElbowW, btnElbowZ, btnArmLow, btnThighUpW, btnThighUpZ, btnThighLowW, btnThighLowZ, btnKneeLowW,
				btnKneeLowZ, btnCalf, btnAnkleW, btnAnkleZ
			};
		}

		protected virtual void Start()
		{
			if (chaCtrl.sex == 0)
			{
				if ((bool)sldHeight)
				{
					sldHeight.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)sldBustForm)
				{
					sldBustForm.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)sldAreolaBulge)
				{
					sldAreolaBulge.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)sldNipWeight)
				{
					sldNipWeight.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
				if ((bool)sldNipStand)
				{
					sldNipStand.transform.parent.gameObject.SetActiveIfDifferent(false);
				}
			}
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpBustSoftness);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpBustWeight);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpAreolaSize);
			Singleton<CustomBase>.Instance.actUpdateCvsBodyShapeAll += UpdateCustomUI;
			sliders.Select((Slider p, int index) => new
			{
				slider = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.slider.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					chaCtrl.SetShapeBodyValue(arrIndex[p.index], p.slider.value);
					inputs[p.index].text = CustomBase.ConvertTextFromRate(0, 100, value);
				});
				p.slider.OnPointerUpAsObservable().Subscribe(delegate
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeBodyValueFromCustomInfo);
				});
				p.slider.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
				{
					p.slider.value = Mathf.Clamp(p.slider.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
					inputs[p.index].text = CustomBase.ConvertTextFromRate(0, 100, p.slider.value);
				});
			});
			inputs.Select((TMP_InputField p, int index) => new
			{
				input = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.input.onEndEdit.AsObservable().Subscribe(delegate(string value)
				{
					sliders[p.index].value = CustomBase.ConvertRateFromText(0, 100, value);
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeBodyValueFromCustomInfo);
				});
			});
			buttons.Select((Button p, int index) => new
			{
				button = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.button.onClick.AsObservable().Subscribe(delegate
				{
					float value2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody[arrIndex[p.index]];
					chaCtrl.SetShapeBodyValue(arrIndex[p.index], value2);
					inputs[p.index].text = CustomBase.ConvertTextFromRate(0, 100, value2);
					sliders[p.index].value = value2;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeBodyValueFromCustomInfo);
				});
			});
			btnMin.OnClickAsObservable().Subscribe(delegate
			{
				for (int j = 0; j < arrIndex.Length; j++)
				{
					chaCtrl.SetShapeBodyValue(arrIndex[j], 0f);
					inputs[j].text = "0";
					sliders[j].value = 0f;
				}
				body.bustSoftness = 0f;
				inpBustSoftness.text = "0";
				sldBustSoftness.value = 0f;
				body.bustWeight = 0f;
				inpBustWeight.text = "0";
				sldBustWeight.value = 0f;
				body.areolaSize = 0f;
				inpAreolaSize.text = "0";
				sldAreolaSize.value = 0f;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, SetMin);
			});
			btnDefault.OnClickAsObservable().Subscribe(delegate
			{
				float[] shapeValueBody = Singleton<CustomBase>.Instance.defChaInfo.custom.body.shapeValueBody;
				for (int k = 0; k < arrIndex.Length; k++)
				{
					float value3 = shapeValueBody[arrIndex[k]];
					chaCtrl.SetShapeBodyValue(arrIndex[k], value3);
					inputs[k].text = CustomBase.ConvertTextFromRate(0, 100, value3);
					sliders[k].value = value3;
				}
				float bustSoftness = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustSoftness;
				body.bustSoftness = bustSoftness;
				inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, bustSoftness);
				sldBustSoftness.value = bustSoftness;
				float bustWeight = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustWeight;
				body.bustWeight = bustWeight;
				inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, bustWeight);
				sldBustWeight.value = bustWeight;
				float areolaSize = Singleton<CustomBase>.Instance.defChaInfo.custom.body.areolaSize;
				body.areolaSize = areolaSize;
				inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, areolaSize);
				sldAreolaSize.value = areolaSize;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, SetDefault);
			});
			btnMax.OnClickAsObservable().Subscribe(delegate
			{
				for (int l = 0; l < arrIndex.Length; l++)
				{
					chaCtrl.SetShapeBodyValue(arrIndex[l], 1f);
					inputs[l].text = "100";
					sliders[l].value = 1f;
				}
				body.bustSoftness = 1f;
				inpBustSoftness.text = "100";
				sldBustSoftness.value = 1f;
				body.bustWeight = 1f;
				inpBustWeight.text = "100";
				sldBustWeight.value = 1f;
				body.areolaSize = 1f;
				inpAreolaSize.text = "100";
				sldAreolaSize.value = 1f;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, SetMax);
			});
			sldBustSoftness.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.bustSoftness = value;
				chaCtrl.UpdateBustSoftness();
				inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldBustSoftness.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustSoftness);
			});
			sldBustSoftness.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldBustSoftness.value = Mathf.Clamp(sldBustSoftness.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, sldBustSoftness.value);
			});
			inpBustSoftness.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldBustSoftness.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustSoftness);
			});
			btnBustSoftness.onClick.AsObservable().Subscribe(delegate
			{
				float bustSoftness2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustSoftness;
				body.bustSoftness = bustSoftness2;
				chaCtrl.UpdateBustSoftness();
				inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, bustSoftness2);
				sldBustSoftness.value = bustSoftness2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustSoftness);
			});
			sldBustWeight.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.bustWeight = value;
				chaCtrl.UpdateBustGravity();
				inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldBustWeight.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustGravity);
			});
			sldBustWeight.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldBustWeight.value = Mathf.Clamp(sldBustWeight.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, sldBustWeight.value);
			});
			inpBustWeight.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldBustWeight.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustGravity);
			});
			btnBustWeight.onClick.AsObservable().Subscribe(delegate
			{
				float bustWeight2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustWeight;
				body.bustWeight = bustWeight2;
				chaCtrl.UpdateBustGravity();
				inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, bustWeight2);
				sldBustWeight.value = bustWeight2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateBustGravity);
			});
			sldAreolaSize.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.areolaSize = value;
				chaCtrl.ChangeSettingAreolaSize();
				inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldAreolaSize.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingAreolaSize);
			});
			sldAreolaSize.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldAreolaSize.value = Mathf.Clamp(sldAreolaSize.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, sldAreolaSize.value);
			});
			inpAreolaSize.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldAreolaSize.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingAreolaSize);
			});
			btnAreolaSize.onClick.AsObservable().Subscribe(delegate
			{
				float areolaSize2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.areolaSize;
				body.areolaSize = areolaSize2;
				chaCtrl.ChangeSettingAreolaSize();
				inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, areolaSize2);
				sldAreolaSize.value = areolaSize2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingAreolaSize);
			});
			StartCoroutine(SetInputText());
		}
	}
}
