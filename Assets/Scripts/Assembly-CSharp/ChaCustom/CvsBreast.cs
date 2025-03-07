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
	public class CvsBreast : MonoBehaviour
	{
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

		[SerializeField]
		[Header("胸の柔らかさ")]
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
		[Header("")]
		private Toggle tglNipKind;

		[SerializeField]
		private Image imgNipKind;

		[SerializeField]
		private TextMeshProUGUI textNipKind;

		[SerializeField]
		private CustomSelectKind customNip;

		[SerializeField]
		private CanvasGroup cgNipWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnNipColor;

		[SerializeField]
		private Image imgNipColor;

		[SerializeField]
		private Slider sldGlossPow;

		[SerializeField]
		private TMP_InputField inpGlossPow;

		[SerializeField]
		private Button btnGlossPow;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[10] { 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

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
			imgNipColor.color = body.nipColor;
			sldGlossPow.value = body.nipGlossPower;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customNip)
			{
				customNip.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Nip))
			{
				cvsColor.SetColor(body.nipColor);
			}
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
			inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, body.nipGlossPower);
		}

		public void UpdateSelectNipKind(string name, Sprite sp, int index)
		{
			if ((bool)textNipKind)
			{
				textNipKind.text = name;
			}
			if ((bool)imgNipKind)
			{
				imgNipKind.sprite = sp;
			}
			if (body.nipId != index)
			{
				body.nipId = index;
				chaCtrl.ChangeSettingNip();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNip);
			}
		}

		public void UpdateNipColor(Color color)
		{
			body.nipColor = color;
			imgNipColor.color = color;
			chaCtrl.ChangeSettingNipColor();
		}

		public void UpdateNipColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNipColor);
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Nip) ?? "乳首の色", CvsColor.ConnectColorKind.Nip, body.nipColor, UpdateNipColor, UpdateNipColorHistory, true);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[10] { sldBustSize, sldBustY, sldBustRotX, sldBustX, sldBustRotY, sldBustSharp, sldBustForm, sldAreolaBulge, sldNipWeight, sldNipStand };
			inputs = new TMP_InputField[10] { inpBustSize, inpBustY, inpBustRotX, inpBustX, inpBustRotY, inpBustSharp, inpBustForm, inpAreolaBulge, inpNipWeight, inpNipStand };
			buttons = new Button[10] { btnBustSize, btnBustY, btnBustRotX, btnBustX, btnBustRotY, btnBustSharp, btnBustForm, btnAreolaBulge, btnNipWeight, btnNipStand };
		}

		protected virtual void Start()
		{
			if (chaCtrl.sex == 0)
			{
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
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGlossPow);
			Singleton<CustomBase>.Instance.actUpdateCvsBreast += UpdateCustomUI;
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
				float bustSoftness = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustSoftness;
				body.bustSoftness = bustSoftness;
				chaCtrl.UpdateBustSoftness();
				inpBustSoftness.text = CustomBase.ConvertTextFromRate(0, 100, bustSoftness);
				sldBustSoftness.value = bustSoftness;
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
				float bustWeight = Singleton<CustomBase>.Instance.defChaInfo.custom.body.bustWeight;
				body.bustWeight = bustWeight;
				chaCtrl.UpdateBustGravity();
				inpBustWeight.text = CustomBase.ConvertTextFromRate(0, 100, bustWeight);
				sldBustWeight.value = bustWeight;
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
				float areolaSize = Singleton<CustomBase>.Instance.defChaInfo.custom.body.areolaSize;
				body.areolaSize = areolaSize;
				chaCtrl.ChangeSettingAreolaSize();
				inpAreolaSize.text = CustomBase.ConvertTextFromRate(0, 100, areolaSize);
				sldAreolaSize.value = areolaSize;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingAreolaSize);
			});
			tglNipKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgNipWin)
				{
					bool flag = ((cgNipWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgNipWin.Enable(isOn);
					}
				}
			});
			btnNipColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Nip)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			sldGlossPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.nipGlossPower = value;
				chaCtrl.ChangeSettingNipGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldGlossPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNipGlossPower);
			});
			sldGlossPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGlossPow.value = Mathf.Clamp(sldGlossPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, sldGlossPow.value);
			});
			inpGlossPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGlossPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNipGlossPower);
			});
			btnGlossPow.onClick.AsObservable().Subscribe(delegate
			{
				float nipGlossPower = Singleton<CustomBase>.Instance.defChaInfo.custom.body.nipGlossPower;
				body.nipGlossPower = nipGlossPower;
				chaCtrl.ChangeSettingNipGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, nipGlossPower);
				sldGlossPow.value = nipGlossPower;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNipGlossPower);
			});
			StartCoroutine(SetInputText());
		}
	}
}
