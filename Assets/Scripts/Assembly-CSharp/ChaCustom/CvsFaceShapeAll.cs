using System.Collections;
using System.Linq;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsFaceShapeAll : MonoBehaviour
	{
		[SerializeField]
		private Button btnMin;

		[SerializeField]
		private Button btnDefault;

		[SerializeField]
		private Button btnMax;

		[SerializeField]
		[Header("顔")]
		private Slider sldFaceBaseW;

		[SerializeField]
		private TMP_InputField inpFaceBaseW;

		[SerializeField]
		private Button btnFaceBaseW;

		[SerializeField]
		private Slider sldFaceUpZ;

		[SerializeField]
		private TMP_InputField inpFaceUpZ;

		[SerializeField]
		private Button btnFaceUpZ;

		[SerializeField]
		private Slider sldFaceUpY;

		[SerializeField]
		private TMP_InputField inpFaceUpY;

		[SerializeField]
		private Button btnFaceUpY;

		[SerializeField]
		private Slider sldFaceUpSize;

		[SerializeField]
		private TMP_InputField inpFaceUpSize;

		[SerializeField]
		private Button btnFaceUpSize;

		[SerializeField]
		private Slider sldFaceLowZ;

		[SerializeField]
		private TMP_InputField inpFaceLowZ;

		[SerializeField]
		private Button btnFaceLowZ;

		[SerializeField]
		private Slider sldFaceLowW;

		[SerializeField]
		private TMP_InputField inpFaceLowW;

		[SerializeField]
		private Button btnFaceLowW;

		[Header("顎")]
		[SerializeField]
		private Slider sldChinLowY;

		[SerializeField]
		private TMP_InputField inpChinLowY;

		[SerializeField]
		private Button btnChinLowY;

		[SerializeField]
		private Slider sldChinLowZ;

		[SerializeField]
		private TMP_InputField inpChinLowZ;

		[SerializeField]
		private Button btnChinLowZ;

		[SerializeField]
		private Slider sldChinY;

		[SerializeField]
		private TMP_InputField inpChinY;

		[SerializeField]
		private Button btnChinY;

		[SerializeField]
		private Slider sldChinW;

		[SerializeField]
		private TMP_InputField inpChinW;

		[SerializeField]
		private Button btnChinW;

		[SerializeField]
		private Slider sldChinZ;

		[SerializeField]
		private TMP_InputField inpChinZ;

		[SerializeField]
		private Button btnChinZ;

		[SerializeField]
		private Slider sldChinTipY;

		[SerializeField]
		private TMP_InputField inpChinTipY;

		[SerializeField]
		private Button btnChinTipY;

		[SerializeField]
		private Slider sldChinTipZ;

		[SerializeField]
		private TMP_InputField inpChinTipZ;

		[SerializeField]
		private Button btnChinTipZ;

		[SerializeField]
		private Slider sldChinTipW;

		[SerializeField]
		private TMP_InputField inpChinTipW;

		[SerializeField]
		private Button btnChinTipW;

		[SerializeField]
		[Header("頬")]
		private Slider sldCheekBoneW;

		[SerializeField]
		private TMP_InputField inpCheekBoneW;

		[SerializeField]
		private Button btnCheekBoneW;

		[SerializeField]
		private Slider sldCheekBoneZ;

		[SerializeField]
		private TMP_InputField inpCheekBoneZ;

		[SerializeField]
		private Button btnCheekBoneZ;

		[SerializeField]
		private Slider sldCheekW;

		[SerializeField]
		private TMP_InputField inpCheekW;

		[SerializeField]
		private Button btnCheekW;

		[SerializeField]
		private Slider sldCheekZ;

		[SerializeField]
		private TMP_InputField inpCheekZ;

		[SerializeField]
		private Button btnCheekZ;

		[SerializeField]
		private Slider sldCheekY;

		[SerializeField]
		private TMP_InputField inpCheekY;

		[SerializeField]
		private Button btnCheekY;

		[SerializeField]
		[Header("眉")]
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
		[Header("目")]
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
		[Header("鼻")]
		private Slider sldNoseTipH;

		[SerializeField]
		private TMP_InputField inpNoseTipH;

		[SerializeField]
		private Button btnNoseTipH;

		[SerializeField]
		private Slider sldNoseY;

		[SerializeField]
		private TMP_InputField inpNoseY;

		[SerializeField]
		private Button btnNoseY;

		[SerializeField]
		private Slider sldNoseBridgeH;

		[SerializeField]
		private TMP_InputField inpNoseBridgeH;

		[SerializeField]
		private Button btnNoseBridgeH;

		[Header("口")]
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
		[Header("耳")]
		private Slider sldEarSize;

		[SerializeField]
		private TMP_InputField inpEarSize;

		[SerializeField]
		private Button btnEarSize;

		[SerializeField]
		private Slider sldEarRotY;

		[SerializeField]
		private TMP_InputField inpEarRotY;

		[SerializeField]
		private Button btnEarRotY;

		[SerializeField]
		private Slider sldEarRotZ;

		[SerializeField]
		private TMP_InputField inpEarRotZ;

		[SerializeField]
		private Button btnEarRotZ;

		[SerializeField]
		private Slider sldEarUpForm;

		[SerializeField]
		private TMP_InputField inpEarUpForm;

		[SerializeField]
		private Button btnEarUpForm;

		[SerializeField]
		private Slider sldEarLowForm;

		[SerializeField]
		private TMP_InputField inpEarLowForm;

		[SerializeField]
		private Button btnEarLowForm;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[52]
		{
			0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
			10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
			20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
			30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
			40, 41, 42, 43, 44, 45, 46, 47, 48, 49,
			50, 51
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
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
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

		protected virtual void Awake()
		{
			sliders = new Slider[52]
			{
				sldFaceBaseW, sldFaceUpZ, sldFaceUpY, sldFaceUpSize, sldFaceLowZ, sldFaceLowW, sldChinLowY, sldChinLowZ, sldChinY, sldChinW,
				sldChinZ, sldChinTipY, sldChinTipZ, sldChinTipW, sldCheekBoneW, sldCheekBoneZ, sldCheekW, sldCheekZ, sldCheekY, sldEyebrowY,
				sldEyebrowX, sldEyebrowRotZ, sldEyebrowInForm, sldEyebrowOutForm, sldEyelidsUpForm1, sldEyelidsUpForm2, sldEyelidsUpForm3, sldEyelidsLowForm1, sldEyelidsLowForm2, sldEyelidsLowForm3,
				sldEyeY, sldEyeX, sldEyeZ, sldEyeTilt, sldEyeH, sldEyeW, sldEyeInX, sldEyeOutY, sldNoseTipH, sldNoseY,
				sldNoseBridgeH, sldMouthY, sldMouthW, sldMouthZ, sldMouthUpForm, sldMouthLowForm, sldMouthCornerForm, sldEarSize, sldEarRotY, sldEarRotZ,
				sldEarUpForm, sldEarLowForm
			};
			inputs = new TMP_InputField[52]
			{
				inpFaceBaseW, inpFaceUpZ, inpFaceUpY, inpFaceUpSize, inpFaceLowZ, inpFaceLowW, inpChinLowY, inpChinLowZ, inpChinY, inpChinW,
				inpChinZ, inpChinTipY, inpChinTipZ, inpChinTipW, inpCheekBoneW, inpCheekBoneZ, inpCheekW, inpCheekZ, inpCheekY, inpEyebrowY,
				inpEyebrowX, inpEyebrowRotZ, inpEyebrowInForm, inpEyebrowOutForm, inpEyelidsUpForm1, inpEyelidsUpForm2, inpEyelidsUpForm3, inpEyelidsLowForm1, inpEyelidsLowForm2, inpEyelidsLowForm3,
				inpEyeY, inpEyeX, inpEyeZ, inpEyeTilt, inpEyeH, inpEyeW, inpEyeInX, inpEyeOutY, inpNoseTipH, inpNoseY,
				inpNoseBridgeH, inpMouthY, inpMouthW, inpMouthZ, inpMouthUpForm, inpMouthLowForm, inpMouthCornerForm, inpEarSize, inpEarRotY, inpEarRotZ,
				inpEarUpForm, inpEarLowForm
			};
			buttons = new Button[52]
			{
				btnFaceBaseW, btnFaceUpZ, btnFaceUpY, btnFaceUpSize, btnFaceLowZ, btnFaceLowW, btnChinLowY, btnChinLowZ, btnChinY, btnChinW,
				btnChinZ, btnChinTipY, btnChinTipZ, btnChinTipW, btnCheekBoneW, btnCheekBoneZ, btnCheekW, btnCheekZ, btnCheekY, btnEyebrowY,
				btnEyebrowX, btnEyebrowRotZ, btnEyebrowInForm, btnEyebrowOutForm, btnEyelidsUpForm1, btnEyelidsUpForm2, btnEyelidsUpForm3, btnEyelidsLowForm1, btnEyelidsLowForm2, btnEyelidsLowForm3,
				btnEyeY, btnEyeX, btnEyeZ, btnEyeTilt, btnEyeH, btnEyeW, btnEyeInX, btnEyeOutY, btnNoseTipH, btnNoseY,
				btnNoseBridgeH, btnMouthY, btnMouthW, btnMouthZ, btnMouthUpForm, btnMouthLowForm, btnMouthCornerForm, btnEarSize, btnEarRotY, btnEarRotZ,
				btnEarUpForm, btnEarLowForm
			};
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsFaceShapeAll += UpdateCustomUI;
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
			btnMin.OnClickAsObservable().Subscribe(delegate
			{
				for (int j = 0; j < arrIndex.Length; j++)
				{
					chaCtrl.SetShapeFaceValue(arrIndex[j], 0f);
					inputs[j].text = "0";
					sliders[j].value = 0f;
				}
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
			});
			btnDefault.OnClickAsObservable().Subscribe(delegate
			{
				float[] shapeValueFace = Singleton<CustomBase>.Instance.defChaInfo.custom.face.shapeValueFace;
				for (int k = 0; k < arrIndex.Length; k++)
				{
					float value3 = shapeValueFace[arrIndex[k]];
					chaCtrl.SetShapeFaceValue(arrIndex[k], value3);
					inputs[k].text = CustomBase.ConvertTextFromRate(0, 100, value3);
					sliders[k].value = value3;
				}
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
			});
			btnMax.OnClickAsObservable().Subscribe(delegate
			{
				for (int l = 0; l < arrIndex.Length; l++)
				{
					chaCtrl.SetShapeFaceValue(arrIndex[l], 1f);
					inputs[l].text = "100";
					sliders[l].value = 1f;
				}
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.UpdateShapeFaceValueFromCustomInfo);
			});
			StartCoroutine(SetInputText());
		}
	}
}
