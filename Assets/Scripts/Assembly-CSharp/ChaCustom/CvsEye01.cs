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
	public class CvsEye01 : MonoBehaviour
	{
		[SerializeField]
		private Slider sldEyelidsUpForm1;

		[SerializeField]
		private TMP_InputField inpEyelidsUpForm1;

		[SerializeField]
		private Button btnEyelidsUpForm1;

		[SerializeField]
		private Slider sldEyelidsUpForm2;

		[SerializeField]
		private TMP_InputField inpEyelidsUpForm2;

		[SerializeField]
		private Button btnEyelidsUpForm2;

		[SerializeField]
		private Slider sldEyelidsUpForm3;

		[SerializeField]
		private TMP_InputField inpEyelidsUpForm3;

		[SerializeField]
		private Button btnEyelidsUpForm3;

		[SerializeField]
		private Slider sldEyelidsLowForm1;

		[SerializeField]
		private TMP_InputField inpEyelidsLowForm1;

		[SerializeField]
		private Button btnEyelidsLowForm1;

		[SerializeField]
		private Slider sldEyelidsLowForm2;

		[SerializeField]
		private TMP_InputField inpEyelidsLowForm2;

		[SerializeField]
		private Button btnEyelidsLowForm2;

		[SerializeField]
		private Slider sldEyelidsLowForm3;

		[SerializeField]
		private TMP_InputField inpEyelidsLowForm3;

		[SerializeField]
		private Button btnEyelidsLowForm3;

		[SerializeField]
		private Slider sldEyeY;

		[SerializeField]
		private TMP_InputField inpEyeY;

		[SerializeField]
		private Button btnEyeY;

		[SerializeField]
		private Slider sldEyeX;

		[SerializeField]
		private TMP_InputField inpEyeX;

		[SerializeField]
		private Button btnEyeX;

		[SerializeField]
		private Slider sldEyeZ;

		[SerializeField]
		private TMP_InputField inpEyeZ;

		[SerializeField]
		private Button btnEyeZ;

		[SerializeField]
		private Slider sldEyeTilt;

		[SerializeField]
		private TMP_InputField inpEyeTilt;

		[SerializeField]
		private Button btnEyeTilt;

		[SerializeField]
		private Slider sldEyeH;

		[SerializeField]
		private TMP_InputField inpEyeH;

		[SerializeField]
		private Button btnEyeH;

		[SerializeField]
		private Slider sldEyeW;

		[SerializeField]
		private TMP_InputField inpEyeW;

		[SerializeField]
		private Button btnEyeW;

		[SerializeField]
		private Slider sldEyeInX;

		[SerializeField]
		private TMP_InputField inpEyeInX;

		[SerializeField]
		private Button btnEyeInX;

		[SerializeField]
		private Slider sldEyeOutY;

		[SerializeField]
		private TMP_InputField inpEyeOutY;

		[SerializeField]
		private Button btnEyeOutY;

		[SerializeField]
		private Toggle tglEyelineUpKind;

		[SerializeField]
		private Image imgEyelineUpKind;

		[SerializeField]
		private TextMeshProUGUI textEyelineUpKind;

		[SerializeField]
		private CustomSelectKind customEyelineUp;

		[SerializeField]
		private CanvasGroup cgEyelineUpWin;

		[SerializeField]
		private Toggle tglEyelineDownKind;

		[SerializeField]
		private Image imgEyelineDownKind;

		[SerializeField]
		private TextMeshProUGUI textEyelineDownKind;

		[SerializeField]
		private CustomSelectKind customEyelineDown;

		[SerializeField]
		private CanvasGroup cgEyelineDownWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnEyelineColor;

		[SerializeField]
		private Image imgEyelineColor;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[14]
		{
			24, 25, 26, 27, 28, 29, 30, 31, 32, 33,
			34, 35, 36, 37
		};

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
			imgEyelineColor.color = face.eyelineColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customEyelineUp)
			{
				customEyelineUp.UpdateCustomUI();
			}
			if (null != customEyelineDown)
			{
				customEyelineDown.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Eyeline))
			{
				cvsColor.SetColor(face.eyelineColor);
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
		}

		public void UpdateSelectEyelineUpKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyelineUpKind)
			{
				textEyelineUpKind.text = name;
			}
			if ((bool)imgEyelineUpKind)
			{
				imgEyelineUpKind.sprite = sp;
			}
			if (face.eyelineUpId != index)
			{
				face.eyelineUpId = index;
				chaCtrl.ChangeSettingEyelineUp();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyelineUp);
			}
		}

		public void UpdateSelectEyelineDownKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyelineDownKind)
			{
				textEyelineDownKind.text = name;
			}
			if ((bool)imgEyelineDownKind)
			{
				imgEyelineDownKind.sprite = sp;
			}
			if (face.eyelineDownId != index)
			{
				face.eyelineDownId = index;
				chaCtrl.ChangeSettingEyelineDown();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyelineDown);
			}
		}

		public void UpdateEyelineColor(Color color)
		{
			face.eyelineColor = color;
			imgEyelineColor.color = color;
			chaCtrl.ChangeSettingEyelineColor();
		}

		public void UpdateEyelineColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyelineColor);
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Eyeline) ?? "アイラインの色", CvsColor.ConnectColorKind.Eyeline, face.eyelineColor, UpdateEyelineColor, UpdateEyelineColorHistory, false);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[14]
			{
				sldEyelidsUpForm1, sldEyelidsUpForm2, sldEyelidsUpForm3, sldEyelidsLowForm1, sldEyelidsLowForm2, sldEyelidsLowForm3, sldEyeY, sldEyeX, sldEyeZ, sldEyeTilt,
				sldEyeH, sldEyeW, sldEyeInX, sldEyeOutY
			};
			inputs = new TMP_InputField[14]
			{
				inpEyelidsUpForm1, inpEyelidsUpForm2, inpEyelidsUpForm3, inpEyelidsLowForm1, inpEyelidsLowForm2, inpEyelidsLowForm3, inpEyeY, inpEyeX, inpEyeZ, inpEyeTilt,
				inpEyeH, inpEyeW, inpEyeInX, inpEyeOutY
			};
			buttons = new Button[14]
			{
				btnEyelidsUpForm1, btnEyelidsUpForm2, btnEyelidsUpForm3, btnEyelidsLowForm1, btnEyelidsLowForm2, btnEyelidsLowForm3, btnEyeY, btnEyeX, btnEyeZ, btnEyeTilt,
				btnEyeH, btnEyeW, btnEyeInX, btnEyeOutY
			};
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsEye01 += UpdateCustomUI;
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
			tglEyelineUpKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyelineUpWin)
				{
					bool flag = ((cgEyelineUpWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgEyelineUpWin.Enable(isOn);
						if (isOn)
						{
							tglEyelineDownKind.isOn = false;
						}
					}
				}
			});
			tglEyelineDownKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyelineDownWin)
				{
					bool flag2 = ((cgEyelineDownWin.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgEyelineDownWin.Enable(isOn);
						if (isOn)
						{
							tglEyelineUpKind.isOn = false;
						}
					}
				}
			});
			btnEyelineColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Eyeline)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
