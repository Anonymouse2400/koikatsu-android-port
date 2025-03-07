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
	public class CvsMouth : MonoBehaviour
	{
		[SerializeField]
		private Slider sldMouthY;

		[SerializeField]
		private TMP_InputField inpMouthY;

		[SerializeField]
		private Button btnMouthY;

		[SerializeField]
		private Slider sldMouthW;

		[SerializeField]
		private TMP_InputField inpMouthW;

		[SerializeField]
		private Button btnMouthW;

		[SerializeField]
		private Slider sldMouthZ;

		[SerializeField]
		private TMP_InputField inpMouthZ;

		[SerializeField]
		private Button btnMouthZ;

		[SerializeField]
		private Slider sldMouthUpForm;

		[SerializeField]
		private TMP_InputField inpMouthUpForm;

		[SerializeField]
		private Button btnMouthUpForm;

		[SerializeField]
		private Slider sldMouthLowForm;

		[SerializeField]
		private TMP_InputField inpMouthLowForm;

		[SerializeField]
		private Button btnMouthLowForm;

		[SerializeField]
		private Slider sldMouthCornerForm;

		[SerializeField]
		private TMP_InputField inpMouthCornerForm;

		[SerializeField]
		private Button btnMouthCornerForm;

		[SerializeField]
		private Toggle tglLiplineKind;

		[SerializeField]
		private Image imgLiplineKind;

		[SerializeField]
		private TextMeshProUGUI textLiplineKind;

		[SerializeField]
		private CustomSelectKind customLipline;

		[SerializeField]
		private CanvasGroup cgLiplineWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnLiplineColor;

		[SerializeField]
		private Image imgLiplineColor;

		[SerializeField]
		private Slider sldGlossPow;

		[SerializeField]
		private TMP_InputField inpGlossPow;

		[SerializeField]
		private Button btnGlossPow;

		[SerializeField]
		private Toggle tglDoubleTooth;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[6] { 41, 42, 43, 44, 45, 46 };

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileFace face
		{
			get
			{
				return chaCtrl.chaFile.custom.face;
			}
		}

		public void CalculateUI()
		{
			float[] shapeValueFace = face.shapeValueFace;
			for (int i = 0; i < sliders.Length; i++)
			{
				if (!(null == sliders[i]))
				{
					sliders[i].value = shapeValueFace[arrIndex[i]];
				}
			}
			imgLiplineColor.color = face.lipLineColor;
			sldGlossPow.value = face.lipGlossPower;
			tglDoubleTooth.isOn = face.doubleTooth;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customLipline)
			{
				customLipline.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Lipline))
			{
				cvsColor.SetColor(face.lipLineColor);
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			float[] shape = face.shapeValueFace;
			for (int i = 0; i < inputs.Length; i++)
			{
				if (!(null == inputs[i]))
				{
					inputs[i].text = CustomBase.ConvertTextFromRate(0, 100, shape[arrIndex[i]]);
				}
			}
			inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, face.lipGlossPower);
		}

		public void UpdateSelectLiplineKind(string name, Sprite sp, int index)
		{
			if ((bool)textLiplineKind)
			{
				textLiplineKind.text = name;
			}
			if ((bool)imgLiplineKind)
			{
				imgLiplineKind.sprite = sp;
			}
			if (face.lipLineId != index)
			{
				face.lipLineId = index;
				FuncUpdateLiplineKind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateLiplineKind);
			}
		}

		public void UpdateLiplineColor(Color color)
		{
			face.lipLineColor = color;
			imgLiplineColor.color = color;
			FuncUpdateLiplineColor();
		}

		public void UpdateLiplineColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateLiplineColor);
		}

		public bool FuncUpdateLiplineKind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceTexFlags(false, false, false, false, false, true, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdateLiplineColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, false, false, false, false, true, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Lipline) ?? "リップラインの色", CvsColor.ConnectColorKind.Lipline, face.lipLineColor, UpdateLiplineColor, UpdateLiplineColorHistory, true);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[6] { sldMouthY, sldMouthW, sldMouthZ, sldMouthUpForm, sldMouthLowForm, sldMouthCornerForm };
			inputs = new TMP_InputField[6] { inpMouthY, inpMouthW, inpMouthZ, inpMouthUpForm, inpMouthLowForm, inpMouthCornerForm };
			buttons = new Button[6] { btnMouthY, btnMouthW, btnMouthZ, btnMouthUpForm, btnMouthLowForm, btnMouthCornerForm };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGlossPow);
			Singleton<CustomBase>.Instance.actUpdateCvsMouth += UpdateCustomUI;
			sliders.Select((Slider p, int index) => new
			{
				slider = p,
				index = index
			}).ToList().ForEach(p =>
			{
				p.slider.onValueChanged.AsObservable().Subscribe(delegate(float value)
				{
					chaCtrl.SetShapeFaceValue(arrIndex[p.index], p.slider.value);
					inputs[p.index].text = CustomBase.ConvertTextFromRate(0, 100, value);
				});
				p.slider.OnPointerUpAsObservable().Subscribe(delegate
				{
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
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
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
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
					float value2 = Singleton<CustomBase>.Instance.defChaInfo.custom.face.shapeValueFace[arrIndex[p.index]];
					chaCtrl.SetShapeFaceValue(arrIndex[p.index], value2);
					inputs[p.index].text = CustomBase.ConvertTextFromRate(0, 100, value2);
					sliders[p.index].value = value2;
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
				});
			});
			tglLiplineKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgLiplineWin)
				{
					bool flag = ((cgLiplineWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgLiplineWin.Enable(isOn);
					}
				}
			});
			btnLiplineColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Lipline)
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
				face.lipGlossPower = value;
				chaCtrl.ChangeSettingLipGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldGlossPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingLipGlossPower);
			});
			sldGlossPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGlossPow.value = Mathf.Clamp(sldGlossPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, sldGlossPow.value);
			});
			inpGlossPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGlossPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingLipGlossPower);
			});
			btnGlossPow.onClick.AsObservable().Subscribe(delegate
			{
				float lipGlossPower = Singleton<CustomBase>.Instance.defChaInfo.custom.face.lipGlossPower;
				face.lipGlossPower = lipGlossPower;
				chaCtrl.ChangeSettingLipGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, lipGlossPower);
				sldGlossPow.value = lipGlossPower;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingLipGlossPower);
			});
			tglDoubleTooth.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && face.doubleTooth != isOn)
				{
					face.doubleTooth = isOn;
					chaCtrl.VisibleDoubleTooth();
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.VisibleDoubleTooth);
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
