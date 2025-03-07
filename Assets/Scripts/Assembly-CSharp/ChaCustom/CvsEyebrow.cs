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
	public class CvsEyebrow : MonoBehaviour
	{
		[SerializeField]
		private Slider sldEyebrowY;

		[SerializeField]
		private TMP_InputField inpEyebrowY;

		[SerializeField]
		private Button btnEyebrowY;

		[SerializeField]
		private Slider sldEyebrowX;

		[SerializeField]
		private TMP_InputField inpEyebrowX;

		[SerializeField]
		private Button btnEyebrowX;

		[SerializeField]
		private Slider sldEyebrowRotZ;

		[SerializeField]
		private TMP_InputField inpEyebrowRotZ;

		[SerializeField]
		private Button btnEyebrowRotZ;

		[SerializeField]
		private Slider sldEyebrowInForm;

		[SerializeField]
		private TMP_InputField inpEyebrowInForm;

		[SerializeField]
		private Button btnEyebrowInForm;

		[SerializeField]
		private Slider sldEyebrowOutForm;

		[SerializeField]
		private TMP_InputField inpEyebrowOutForm;

		[SerializeField]
		private Button btnEyebrowOutForm;

		[SerializeField]
		private Toggle tglEyebrowKind;

		[SerializeField]
		private Image imgEyebrowKind;

		[SerializeField]
		private TextMeshProUGUI textEyebrowKind;

		[SerializeField]
		private CustomSelectKind customEyebrow;

		[SerializeField]
		private CanvasGroup cgEyebrowWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnEyebrowColor;

		[SerializeField]
		private Image imgEyebrowColor;

		[SerializeField]
		private Toggle[] tglForegroundEyebrow;

		[SerializeField]
		private Button btnReflectColor;

		[SerializeField]
		private Button btnHairColor;

		[SerializeField]
		private Button btnUnderhairColor;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[5] { 19, 20, 21, 22, 23 };

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

		private ChaFileBody body
		{
			get
			{
				return chaCtrl.chaFile.custom.body;
			}
		}

		private ChaFileHair hair
		{
			get
			{
				return chaCtrl.chaFile.custom.hair;
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
			for (int j = 0; j < 3; j++)
			{
				tglForegroundEyebrow[j].isOn = j == face.foregroundEyebrow;
			}
			imgEyebrowColor.color = face.eyebrowColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customEyebrow)
			{
				customEyebrow.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Eyebrow))
			{
				cvsColor.SetColor(face.eyebrowColor);
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

		public void UpdateSelectEyebrowKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyebrowKind)
			{
				textEyebrowKind.text = name;
			}
			if ((bool)imgEyebrowKind)
			{
				imgEyebrowKind.sprite = sp;
			}
			if (face.eyebrowId != index)
			{
				face.eyebrowId = index;
				chaCtrl.ChangeSettingEyebrow();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyebrow);
			}
		}

		public bool UpdateHairAndUnderhair()
		{
			for (int i = 0; i < hair.parts.Length; i++)
			{
				chaCtrl.ChangeSettingHairColor(i, true, true, true);
			}
			chaCtrl.ChangeSettingUnderhairColor();
			return true;
		}

		public void UpdateEyebrowColor(Color color)
		{
			face.eyebrowColor = color;
			imgEyebrowColor.color = color;
			chaCtrl.ChangeSettingEyebrowColor();
		}

		public void UpdateEyebrowColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyebrowColor);
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Eyebrow) ?? "眉の色", CvsColor.ConnectColorKind.Eyebrow, face.eyebrowColor, UpdateEyebrowColor, UpdateEyebrowColorHistory, false);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[5] { sldEyebrowY, sldEyebrowX, sldEyebrowRotZ, sldEyebrowInForm, sldEyebrowOutForm };
			inputs = new TMP_InputField[5] { inpEyebrowY, inpEyebrowX, inpEyebrowRotZ, inpEyebrowInForm, inpEyebrowOutForm };
			buttons = new Button[5] { btnEyebrowY, btnEyebrowX, btnEyebrowRotZ, btnEyebrowInForm, btnEyebrowOutForm };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsEyebrow += UpdateCustomUI;
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
			tglEyebrowKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyebrowWin)
				{
					bool flag = ((cgEyebrowWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgEyebrowWin.Enable(isOn);
					}
				}
			});
			btnEyebrowColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Eyebrow)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			tglForegroundEyebrow.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn && face.foregroundEyebrow != p.index)
					{
						face.foregroundEyebrow = p.index;
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					}
				});
			});
			btnReflectColor.OnClickAsObservable().Subscribe(delegate
			{
				Color eyebrowColor = face.eyebrowColor;
				float H;
				float S;
				float V;
				Color.RGBToHSV(eyebrowColor, out H, out S, out V);
				Color startColor = Color.HSVToRGB(H, S, Mathf.Max(V - 0.2f, 0f));
				Color endColor = Color.HSVToRGB(H, S, Mathf.Min(V + 0.1f, 1f));
				ChaFileHair.PartsInfo[] parts = hair.parts;
				foreach (ChaFileHair.PartsInfo partsInfo in parts)
				{
					partsInfo.baseColor = eyebrowColor;
					partsInfo.startColor = startColor;
					partsInfo.endColor = endColor;
				}
				body.underhairColor = eyebrowColor;
				UpdateHairAndUnderhair();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, UpdateHairAndUnderhair);
				Singleton<CustomBase>.Instance.updateCvsHairBack = true;
				Singleton<CustomBase>.Instance.updateCvsHairFront = true;
				Singleton<CustomBase>.Instance.updateCvsHairSide = true;
				Singleton<CustomBase>.Instance.updateCvsHairExtension = true;
				Singleton<CustomBase>.Instance.updateCvsUnderhair = true;
			});
			btnHairColor.OnClickAsObservable().Subscribe(delegate
			{
				Color baseColor = hair.parts[0].baseColor;
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Eyebrow))
				{
					cvsColor.SetColor(baseColor);
				}
				UpdateEyebrowColor(baseColor);
				UpdateEyebrowColorHistory();
			});
			btnUnderhairColor.OnClickAsObservable().Subscribe(delegate
			{
				Color underhairColor = body.underhairColor;
				if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Eyebrow))
				{
					cvsColor.SetColor(underhairColor);
				}
				UpdateEyebrowColor(underhairColor);
				UpdateEyebrowColorHistory();
			});
			StartCoroutine(SetInputText());
		}
	}
}
