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
	public class CvsEye02 : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglEyeWGradeKind;

		[SerializeField]
		private Image imgEyeWGradeKind;

		[SerializeField]
		private TextMeshProUGUI textEyeWGradeKind;

		[SerializeField]
		private CustomSelectKind customEyeWGrade;

		[SerializeField]
		private CanvasGroup cgEyeWGradeWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnEyeW01Color;

		[SerializeField]
		private Image imgEyeW01Color;

		[SerializeField]
		private Button btnEyeW02Color;

		[SerializeField]
		private Image imgEyeW02Color;

		[SerializeField]
		private Toggle tglEyeHLUpKind;

		[SerializeField]
		private Image imgEyeHLUpKind;

		[SerializeField]
		private TextMeshProUGUI textEyeHLUpKind;

		[SerializeField]
		private CustomSelectKind customEyeHLUp;

		[SerializeField]
		private CanvasGroup cgEyeHLUpWin;

		[SerializeField]
		private Slider sldHLUpY;

		[SerializeField]
		private TMP_InputField inpHLUpY;

		[SerializeField]
		private Button btnHLUpY;

		[SerializeField]
		private Slider sldHLDownY;

		[SerializeField]
		private TMP_InputField inpHLDownY;

		[SerializeField]
		private Button btnHLDownY;

		[SerializeField]
		private Button btnEyeHLUpColor;

		[SerializeField]
		private Image imgEyeHLUpColor;

		[SerializeField]
		private Toggle tglEyeHLDownKind;

		[SerializeField]
		private Image imgEyeHLDownKind;

		[SerializeField]
		private TextMeshProUGUI textEyeHLDownKind;

		[SerializeField]
		private CustomSelectKind customEyeHLDown;

		[SerializeField]
		private CanvasGroup cgEyeHLDownWin;

		[SerializeField]
		private Button btnEyeHLDownColor;

		[SerializeField]
		private Image imgEyeHLDownColor;

		[SerializeField]
		private Slider sldPupilX;

		[SerializeField]
		private TMP_InputField inpPupilX;

		[SerializeField]
		private Button btnPupilX;

		[SerializeField]
		private Slider sldPupilY;

		[SerializeField]
		private TMP_InputField inpPupilY;

		[SerializeField]
		private Button btnPupilY;

		[SerializeField]
		private Slider sldPupilWidth;

		[SerializeField]
		private TMP_InputField inpPupilWidth;

		[SerializeField]
		private Button btnPupilWidth;

		[SerializeField]
		private Slider sldPupilHeight;

		[SerializeField]
		private TMP_InputField inpPupilHeight;

		[SerializeField]
		private Button btnPupilHeight;

		[SerializeField]
		private Toggle[] tglForegroundEye;

		[SerializeField]
		private Toggle[] tglEyeSetType;

		private byte selEyeSetType;

		[SerializeField]
		private Toggle tglPupilKind;

		[SerializeField]
		private Image imgPupilKind;

		[SerializeField]
		private TextMeshProUGUI textPupilKind;

		[SerializeField]
		private CustomSelectKind customPupil;

		[SerializeField]
		private CanvasGroup cgPupilWin;

		[SerializeField]
		private Button btnPupil01Color;

		[SerializeField]
		private Image imgPupil01Color;

		[SerializeField]
		private Button btnPupil02Color;

		[SerializeField]
		private Image imgPupil02Color;

		[SerializeField]
		private Toggle tglPupilGradeKind;

		[SerializeField]
		private Image imgPupilGradeKind;

		[SerializeField]
		private TextMeshProUGUI textPupilGradeKind;

		[SerializeField]
		private CustomSelectKind customPupilGrade;

		[SerializeField]
		private CanvasGroup cgPupilGradeWin;

		[SerializeField]
		private Slider sldGradBlend;

		[SerializeField]
		private TMP_InputField inpGradBlend;

		[SerializeField]
		private Button btnGradBlend;

		[SerializeField]
		private Slider sldGradOffsetY;

		[SerializeField]
		private TMP_InputField inpGradOffsetY;

		[SerializeField]
		private Button btnGradOffsetY;

		[SerializeField]
		private Slider sldGradScale;

		[SerializeField]
		private TMP_InputField inpGradScale;

		[SerializeField]
		private Button btnGradScale;

		[SerializeField]
		private Button btnCopyLtoR;

		[SerializeField]
		private Button btnCopyRtoL;

		private int refLR
		{
			get
			{
				return (selEyeSetType == 2) ? 1 : 0;
			}
		}

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

		private Color pupilBaseColor
		{
			get
			{
				return (refLR != 0) ? face.pupil[1].baseColor : face.pupil[0].baseColor;
			}
		}

		private Color pupilSubColor
		{
			get
			{
				return (refLR != 0) ? face.pupil[1].subColor : face.pupil[0].subColor;
			}
		}

		public void ChangeEyeSetType(int type)
		{
			bool[,] array = new bool[3, 3]
			{
				{ true, false, false },
				{ false, true, false },
				{ false, false, true }
			};
			for (int i = 0; i < tglEyeSetType.Length; i++)
			{
				tglEyeSetType[i].isOn = array[type, i];
			}
		}

		public void CalculateUI()
		{
			sldHLUpY.value = face.hlUpY;
			sldHLDownY.value = face.hlDownY;
			sldPupilX.value = face.pupilX;
			sldPupilY.value = face.pupilY;
			sldPupilWidth.value = face.pupilWidth;
			sldPupilHeight.value = face.pupilHeight;
			sldGradBlend.value = face.pupil[refLR].gradBlend;
			sldGradOffsetY.value = face.pupil[refLR].gradOffsetY;
			sldGradScale.value = face.pupil[refLR].gradScale;
			for (int i = 0; i < 3; i++)
			{
				tglForegroundEye[i].isOn = i == face.foregroundEyes;
			}
			imgEyeW01Color.color = face.whiteBaseColor;
			imgEyeW02Color.color = face.whiteSubColor;
			imgEyeHLUpColor.color = face.hlUpColor;
			imgEyeHLDownColor.color = face.hlDownColor;
			imgPupil01Color.color = pupilBaseColor;
			imgPupil02Color.color = pupilSubColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customEyeWGrade)
			{
				customEyeWGrade.UpdateCustomUI();
			}
			if (null != customEyeHLUp)
			{
				customEyeHLUp.UpdateCustomUI();
			}
			if (null != customEyeHLDown)
			{
				customEyeHLDown.UpdateCustomUI();
			}
			if (null != customPupil)
			{
				customPupil.UpdateCustomUI(refLR);
			}
			if (null != customPupilGrade)
			{
				customPupilGrade.UpdateCustomUI(refLR);
			}
			switch (cvsColor.connectColorKind)
			{
			case CvsColor.ConnectColorKind.EyeW01:
				cvsColor.SetColor(face.whiteBaseColor);
				break;
			case CvsColor.ConnectColorKind.EyeW02:
				cvsColor.SetColor(face.whiteSubColor);
				break;
			case CvsColor.ConnectColorKind.EyeHLUp:
				cvsColor.SetColor(face.hlUpColor);
				break;
			case CvsColor.ConnectColorKind.EyeHLDown:
				cvsColor.SetColor(face.hlDownColor);
				break;
			case CvsColor.ConnectColorKind.Pupil01:
				cvsColor.SetColor(pupilBaseColor);
				break;
			case CvsColor.ConnectColorKind.Pupil02:
				cvsColor.SetColor(pupilSubColor);
				break;
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpHLUpY.text = CustomBase.ConvertTextFromRate(0, 100, face.hlUpY);
			inpHLDownY.text = CustomBase.ConvertTextFromRate(0, 100, face.hlDownY);
			inpPupilX.text = CustomBase.ConvertTextFromRate(0, 100, face.pupilX);
			inpPupilY.text = CustomBase.ConvertTextFromRate(0, 100, face.pupilY);
			inpPupilWidth.text = CustomBase.ConvertTextFromRate(0, 100, face.pupilWidth);
			inpPupilHeight.text = CustomBase.ConvertTextFromRate(0, 100, face.pupilHeight);
			inpGradBlend.text = CustomBase.ConvertTextFromRate(0, 100, face.pupil[refLR].gradBlend);
			inpGradOffsetY.text = CustomBase.ConvertTextFromRate(0, 100, face.pupil[refLR].gradOffsetY);
			inpGradScale.text = CustomBase.ConvertTextFromRate(0, 100, face.pupil[refLR].gradScale);
		}

		public void UpdateSelectEyeWGradeKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyeWGradeKind)
			{
				textEyeWGradeKind.text = name;
			}
			if ((bool)imgEyeWGradeKind)
			{
				imgEyeWGradeKind.sprite = sp;
			}
			if (face.whiteId != index)
			{
				face.whiteId = index;
				chaCtrl.ChangeSettingWhiteOfEye(true, false);
				Singleton<CustomHistory>.Instance.Add3(chaCtrl, chaCtrl.ChangeSettingWhiteOfEye, true, false);
			}
		}

		public void UpdateSelectEyeHLUpKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyeHLUpKind)
			{
				textEyeHLUpKind.text = name;
			}
			if ((bool)imgEyeHLUpKind)
			{
				imgEyeHLUpKind.sprite = sp;
			}
			if (face.hlUpId != index)
			{
				face.hlUpId = index;
				chaCtrl.ChangeSettingEyeHiUp();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHiUp);
			}
		}

		public void UpdateSelectEyeHLDownKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyeHLDownKind)
			{
				textEyeHLDownKind.text = name;
			}
			if ((bool)imgEyeHLDownKind)
			{
				imgEyeHLDownKind.sprite = sp;
			}
			if (face.hlDownId != index)
			{
				face.hlDownId = index;
				chaCtrl.ChangeSettingEyeHiDown();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHiDown);
			}
		}

		public void UpdateSelectPupilKind(string name, Sprite sp, int index)
		{
			if ((bool)textPupilKind)
			{
				textPupilKind.text = name;
			}
			if ((bool)imgPupilKind)
			{
				imgPupilKind.sprite = sp;
			}
			if (selEyeSetType == 0)
			{
				if (face.pupil[0].id != index || face.pupil[1].id != index)
				{
					face.pupil[0].id = index;
					face.pupil[1].id = index;
					chaCtrl.ChangeSettingEye(true, false, false);
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, true, false, false);
				}
			}
			else if (selEyeSetType == 1)
			{
				if (face.pupil[0].id != index)
				{
					face.pupil[0].id = index;
					chaCtrl.ChangeSettingEyeL(true, false, false);
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, true, false, false);
				}
			}
			else if (face.pupil[1].id != index)
			{
				face.pupil[1].id = index;
				chaCtrl.ChangeSettingEyeR(true, false, false);
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, true, false, false);
			}
		}

		public void UpdateSelectPupilGradeKind(string name, Sprite sp, int index)
		{
			if ((bool)textPupilGradeKind)
			{
				textPupilGradeKind.text = name;
			}
			if ((bool)imgPupilGradeKind)
			{
				imgPupilGradeKind.sprite = sp;
			}
			if (selEyeSetType == 0)
			{
				if (face.pupil[0].gradMaskId != index || face.pupil[1].gradMaskId != index)
				{
					face.pupil[0].gradMaskId = index;
					face.pupil[1].gradMaskId = index;
					chaCtrl.ChangeSettingEye(false, true, false);
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, true, false);
				}
			}
			else if (selEyeSetType == 1)
			{
				if (face.pupil[0].gradMaskId != index)
				{
					face.pupil[0].gradMaskId = index;
					chaCtrl.ChangeSettingEyeL(false, true, false);
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, true, false);
				}
			}
			else if (face.pupil[1].gradMaskId != index)
			{
				face.pupil[1].gradMaskId = index;
				chaCtrl.ChangeSettingEyeR(false, true, false);
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, true, false);
			}
		}

		public void UpdateEyeW01Color(Color color)
		{
			face.whiteBaseColor = color;
			imgEyeW01Color.color = color;
			chaCtrl.ChangeSettingWhiteOfEye(false, true);
		}

		public void UpdateEyeW02Color(Color color)
		{
			face.whiteSubColor = color;
			imgEyeW02Color.color = color;
			chaCtrl.ChangeSettingWhiteOfEye(false, true);
		}

		public void UpdateEyeWColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add3(chaCtrl, chaCtrl.ChangeSettingWhiteOfEye, false, true);
		}

		public void UpdateEyeHLUpColor(Color color)
		{
			face.hlUpColor = color;
			imgEyeHLUpColor.color = color;
			chaCtrl.ChangeSettingEyeHiUpColor();
		}

		public void UpdateEyeHLUpColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHiUpColor);
		}

		public void UpdateEyeHLDownColor(Color color)
		{
			face.hlDownColor = color;
			imgEyeHLDownColor.color = color;
			chaCtrl.ChangeSettingEyeHiDownColor();
		}

		public void UpdateEyeHLDownColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHiDownColor);
		}

		public void UpdatePupil01Color(Color color)
		{
			if (selEyeSetType == 0)
			{
				face.pupil[0].baseColor = color;
				face.pupil[1].baseColor = color;
				chaCtrl.ChangeSettingEye(false, false, true);
			}
			else if (selEyeSetType == 1)
			{
				face.pupil[0].baseColor = color;
				chaCtrl.ChangeSettingEyeL(false, false, true);
			}
			else
			{
				face.pupil[1].baseColor = color;
				chaCtrl.ChangeSettingEyeR(false, false, true);
			}
			imgPupil01Color.color = color;
		}

		public void UpdatePupil02Color(Color color)
		{
			if (selEyeSetType == 0)
			{
				face.pupil[0].subColor = color;
				face.pupil[1].subColor = color;
				chaCtrl.ChangeSettingEye(false, false, true);
			}
			else if (selEyeSetType == 1)
			{
				face.pupil[0].subColor = color;
				chaCtrl.ChangeSettingEyeL(false, false, true);
			}
			else
			{
				face.pupil[1].subColor = color;
				chaCtrl.ChangeSettingEyeR(false, false, true);
			}
			imgPupil02Color.color = color;
		}

		public void UpdatePupilColorHistory()
		{
			if (selEyeSetType == 0)
			{
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
			}
			else if (selEyeSetType == 1)
			{
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
			}
			else
			{
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
			}
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Pupil01) ?? "瞳の色①", CvsColor.ConnectColorKind.Pupil01, pupilBaseColor, UpdatePupil01Color, UpdatePupilColorHistory, false);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpHLUpY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpHLDownY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPupilX);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPupilY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPupilWidth);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPupilHeight);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGradBlend);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGradOffsetY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGradScale);
			Singleton<CustomBase>.Instance.actUpdateCvsEye02 += UpdateCustomUI;
			sldHLUpY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.hlUpY = value;
				chaCtrl.ChangeSettingEyeHLUpPosY();
				inpHLUpY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldHLUpY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLUpPosY);
			});
			sldHLUpY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldHLUpY.value = Mathf.Clamp(sldHLUpY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpHLUpY.text = CustomBase.ConvertTextFromRate(0, 100, sldHLUpY.value);
			});
			inpHLUpY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldHLUpY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLUpPosY);
			});
			btnHLUpY.onClick.AsObservable().Subscribe(delegate
			{
				float hlUpY = Singleton<CustomBase>.Instance.defChaInfo.custom.face.hlUpY;
				face.hlUpY = hlUpY;
				chaCtrl.ChangeSettingEyeHLUpPosY();
				inpHLUpY.text = CustomBase.ConvertTextFromRate(0, 100, hlUpY);
				sldHLUpY.value = hlUpY;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLUpPosY);
			});
			sldHLDownY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.hlDownY = value;
				chaCtrl.ChangeSettingEyeHLDownPosY();
				inpHLDownY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldHLDownY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLDownPosY);
			});
			sldHLDownY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldHLDownY.value = Mathf.Clamp(sldHLDownY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpHLDownY.text = CustomBase.ConvertTextFromRate(0, 100, sldHLDownY.value);
			});
			inpHLDownY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldHLDownY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLDownPosY);
			});
			btnHLDownY.onClick.AsObservable().Subscribe(delegate
			{
				float hlDownY = Singleton<CustomBase>.Instance.defChaInfo.custom.face.hlDownY;
				face.hlDownY = hlDownY;
				chaCtrl.ChangeSettingEyeHLDownPosY();
				inpHLDownY.text = CustomBase.ConvertTextFromRate(0, 100, hlDownY);
				sldHLDownY.value = hlDownY;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeHLDownPosY);
			});
			sldPupilX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.pupilX = value;
				chaCtrl.ChangeSettingEyePosX();
				inpPupilX.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPupilX.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosX);
			});
			sldPupilX.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPupilX.value = Mathf.Clamp(sldPupilX.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPupilX.text = CustomBase.ConvertTextFromRate(0, 100, sldPupilX.value);
			});
			inpPupilX.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPupilX.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosX);
			});
			btnPupilX.onClick.AsObservable().Subscribe(delegate
			{
				float pupilX = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupilX;
				face.pupilX = pupilX;
				chaCtrl.ChangeSettingEyePosX();
				inpPupilX.text = CustomBase.ConvertTextFromRate(0, 100, pupilX);
				sldPupilX.value = pupilX;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosX);
			});
			sldPupilY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.pupilY = value;
				chaCtrl.ChangeSettingEyePosY();
				inpPupilY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPupilY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosY);
			});
			sldPupilY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPupilY.value = Mathf.Clamp(sldPupilY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPupilY.text = CustomBase.ConvertTextFromRate(0, 100, sldPupilY.value);
			});
			inpPupilY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPupilY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosY);
			});
			btnPupilY.onClick.AsObservable().Subscribe(delegate
			{
				float pupilY = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupilY;
				face.pupilY = pupilY;
				chaCtrl.ChangeSettingEyePosY();
				inpPupilY.text = CustomBase.ConvertTextFromRate(0, 100, pupilY);
				sldPupilY.value = pupilY;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyePosY);
			});
			sldPupilWidth.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.pupilWidth = value;
				chaCtrl.ChangeSettingEyeScaleWidth();
				inpPupilWidth.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPupilWidth.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleWidth);
			});
			sldPupilWidth.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPupilWidth.value = Mathf.Clamp(sldPupilWidth.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPupilWidth.text = CustomBase.ConvertTextFromRate(0, 100, sldPupilWidth.value);
			});
			inpPupilWidth.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPupilWidth.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleWidth);
			});
			btnPupilWidth.onClick.AsObservable().Subscribe(delegate
			{
				float pupilWidth = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupilWidth;
				face.pupilWidth = pupilWidth;
				chaCtrl.ChangeSettingEyeScaleWidth();
				inpPupilWidth.text = CustomBase.ConvertTextFromRate(0, 100, pupilWidth);
				sldPupilWidth.value = pupilWidth;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleWidth);
			});
			sldPupilHeight.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.pupilHeight = value;
				chaCtrl.ChangeSettingEyeScaleHeight();
				inpPupilHeight.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPupilHeight.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleHeight);
			});
			sldPupilHeight.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPupilHeight.value = Mathf.Clamp(sldPupilHeight.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPupilHeight.text = CustomBase.ConvertTextFromRate(0, 100, sldPupilHeight.value);
			});
			inpPupilHeight.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPupilHeight.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleHeight);
			});
			btnPupilHeight.onClick.AsObservable().Subscribe(delegate
			{
				float pupilHeight = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupilHeight;
				face.pupilHeight = pupilHeight;
				chaCtrl.ChangeSettingEyeScaleHeight();
				inpPupilHeight.text = CustomBase.ConvertTextFromRate(0, 100, pupilHeight);
				sldPupilHeight.value = pupilHeight;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeScaleHeight);
			});
			tglForegroundEye.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (!Singleton<CustomBase>.Instance.updateCustomUI && isOn && face.foregroundEyes != p.index)
					{
						face.foregroundEyes = p.index;
						Singleton<CustomHistory>.Instance.Add1(chaCtrl, null);
					}
				});
			});
			tglEyeSetType.Select((Toggle p, int idx) => new
			{
				toggle = p,
				index = (byte)idx
			}).ToList().ForEach(p =>
			{
				p.toggle.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
				{
					if (isOn && selEyeSetType != p.index)
					{
						selEyeSetType = p.index;
						if (selEyeSetType == 0 && !face.isPupilSameSetting)
						{
							face.pupil[1].Copy(face.pupil[0]);
							chaCtrl.ChangeSettingEye(true, true, true);
							Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, true, true, true);
						}
						UpdateCustomUI();
					}
				});
			});
			sldGradBlend.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				bool[] array = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b = 0; b < 2; b++)
				{
					if (array[b])
					{
						face.pupil[b].gradBlend = value;
						chaCtrl.ChangeSettingEye(b, false, false, true);
						inpGradBlend.text = CustomBase.ConvertTextFromRate(0, 100, value);
					}
				}
			});
			sldGradBlend.OnPointerUpAsObservable().Subscribe(delegate
			{
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			sldGradBlend.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGradBlend.value = Mathf.Clamp(sldGradBlend.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGradBlend.text = CustomBase.ConvertTextFromRate(0, 100, sldGradBlend.value);
			});
			inpGradBlend.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGradBlend.value = CustomBase.ConvertRateFromText(0, 100, value);
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			btnGradBlend.onClick.AsObservable().Subscribe(delegate
			{
				float gradBlend = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupil[0].gradBlend;
				bool[] array2 = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b2 = 0; b2 < 2; b2++)
				{
					if (array2[b2])
					{
						face.pupil[b2].gradBlend = gradBlend;
						chaCtrl.ChangeSettingEye(b2, false, false, true);
					}
				}
				inpGradBlend.text = CustomBase.ConvertTextFromRate(0, 100, gradBlend);
				sldGradBlend.value = gradBlend;
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			sldGradOffsetY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				bool[] array3 = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b3 = 0; b3 < 2; b3++)
				{
					if (array3[b3])
					{
						face.pupil[b3].gradOffsetY = value;
						chaCtrl.ChangeSettingEye(b3, false, false, true);
						inpGradOffsetY.text = CustomBase.ConvertTextFromRate(0, 100, value);
					}
				}
			});
			sldGradOffsetY.OnPointerUpAsObservable().Subscribe(delegate
			{
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			sldGradOffsetY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGradOffsetY.value = Mathf.Clamp(sldGradOffsetY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGradOffsetY.text = CustomBase.ConvertTextFromRate(0, 100, sldGradOffsetY.value);
			});
			inpGradOffsetY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGradOffsetY.value = CustomBase.ConvertRateFromText(0, 100, value);
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			btnGradOffsetY.onClick.AsObservable().Subscribe(delegate
			{
				float gradOffsetY = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupil[0].gradOffsetY;
				bool[] array4 = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b4 = 0; b4 < 2; b4++)
				{
					if (array4[b4])
					{
						face.pupil[b4].gradOffsetY = gradOffsetY;
						chaCtrl.ChangeSettingEye(b4, false, false, true);
					}
				}
				inpGradOffsetY.text = CustomBase.ConvertTextFromRate(0, 100, gradOffsetY);
				sldGradOffsetY.value = gradOffsetY;
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			sldGradScale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				bool[] array5 = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b5 = 0; b5 < 2; b5++)
				{
					if (array5[b5])
					{
						face.pupil[b5].gradScale = value;
						chaCtrl.ChangeSettingEye(b5, false, false, true);
						inpGradScale.text = CustomBase.ConvertTextFromRate(0, 100, value);
					}
				}
			});
			sldGradScale.OnPointerUpAsObservable().Subscribe(delegate
			{
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			sldGradScale.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGradScale.value = Mathf.Clamp(sldGradScale.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGradScale.text = CustomBase.ConvertTextFromRate(0, 100, sldGradScale.value);
			});
			inpGradScale.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGradScale.value = CustomBase.ConvertRateFromText(0, 100, value);
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			btnGradScale.onClick.AsObservable().Subscribe(delegate
			{
				float gradScale = Singleton<CustomBase>.Instance.defChaInfo.custom.face.pupil[0].gradScale;
				bool[] array6 = new bool[2]
				{
					(selEyeSetType != 2) ? true : false,
					(selEyeSetType != 1) ? true : false
				};
				for (byte b6 = 0; b6 < 2; b6++)
				{
					if (array6[b6])
					{
						face.pupil[b6].gradScale = gradScale;
						chaCtrl.ChangeSettingEye(b6, false, false, true);
					}
				}
				inpGradScale.text = CustomBase.ConvertTextFromRate(0, 100, gradScale);
				sldGradScale.value = gradScale;
				switch (selEyeSetType)
				{
				case 0:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, false, false, true);
					break;
				case 1:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeL, false, false, true);
					break;
				case 2:
					Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEyeR, false, false, true);
					break;
				}
			});
			tglEyeWGradeKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyeWGradeWin)
				{
					bool flag = ((cgEyeWGradeWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgEyeWGradeWin.Enable(isOn);
						if (isOn)
						{
							tglEyeHLUpKind.isOn = false;
							tglEyeHLDownKind.isOn = false;
							tglPupilKind.isOn = false;
							tglPupilGradeKind.isOn = false;
						}
					}
				}
			});
			tglEyeHLUpKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyeHLUpWin)
				{
					bool flag2 = ((cgEyeHLUpWin.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgEyeHLUpWin.Enable(isOn);
						if (isOn)
						{
							tglEyeWGradeKind.isOn = false;
							tglEyeHLDownKind.isOn = false;
							tglPupilKind.isOn = false;
							tglPupilGradeKind.isOn = false;
						}
					}
				}
			});
			tglEyeHLDownKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyeHLDownWin)
				{
					bool flag3 = ((cgEyeHLDownWin.alpha != 0f) ? true : false);
					if (flag3 != isOn)
					{
						cgEyeHLDownWin.Enable(isOn);
						if (isOn)
						{
							tglEyeWGradeKind.isOn = false;
							tglEyeHLUpKind.isOn = false;
							tglPupilKind.isOn = false;
							tglPupilGradeKind.isOn = false;
						}
					}
				}
			});
			tglPupilKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPupilWin)
				{
					bool flag4 = ((cgPupilWin.alpha != 0f) ? true : false);
					if (flag4 != isOn)
					{
						cgPupilWin.Enable(isOn);
						if (isOn)
						{
							tglEyeWGradeKind.isOn = false;
							tglEyeHLUpKind.isOn = false;
							tglEyeHLDownKind.isOn = false;
							tglPupilGradeKind.isOn = false;
						}
					}
				}
			});
			tglPupilGradeKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPupilGradeWin)
				{
					bool flag5 = ((cgPupilGradeWin.alpha != 0f) ? true : false);
					if (flag5 != isOn)
					{
						cgPupilGradeWin.Enable(isOn);
						if (isOn)
						{
							tglEyeWGradeKind.isOn = false;
							tglEyeHLUpKind.isOn = false;
							tglEyeHLDownKind.isOn = false;
							tglPupilKind.isOn = false;
						}
					}
				}
			});
			btnEyeW01Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.EyeW01)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.EyeW01) ?? "白目の色①", CvsColor.ConnectColorKind.EyeW01, face.whiteBaseColor, UpdateEyeW01Color, UpdateEyeWColorHistory, false);
				}
			});
			btnEyeW02Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.EyeW02)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.EyeW02) ?? "白目の色②", CvsColor.ConnectColorKind.EyeW02, face.whiteSubColor, UpdateEyeW02Color, UpdateEyeWColorHistory, false);
				}
			});
			btnEyeHLUpColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.EyeHLUp)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.EyeHLUp) ?? "ハイライト上の色", CvsColor.ConnectColorKind.EyeHLUp, face.hlUpColor, UpdateEyeHLUpColor, UpdateEyeHLUpColorHistory, true);
				}
			});
			btnEyeHLDownColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.EyeHLDown)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.EyeHLDown) ?? "ハイライト下の色", CvsColor.ConnectColorKind.EyeHLDown, face.hlDownColor, UpdateEyeHLDownColor, UpdateEyeHLDownColorHistory, true);
				}
			});
			btnPupil01Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Pupil01)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			btnPupil02Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Pupil02)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Pupil02) ?? "瞳の色②", CvsColor.ConnectColorKind.Pupil02, pupilSubColor, UpdatePupil02Color, UpdatePupilColorHistory, false);
				}
			});
			btnCopyLtoR.OnClickAsObservable().Subscribe(delegate
			{
				face.pupil[1].Copy(face.pupil[0]);
				chaCtrl.ChangeSettingEye(true, true, true);
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, true, true, true);
				UpdateCustomUI();
			});
			btnCopyRtoL.OnClickAsObservable().Subscribe(delegate
			{
				face.pupil[0].Copy(face.pupil[1]);
				chaCtrl.ChangeSettingEye(true, true, true);
				Singleton<CustomHistory>.Instance.Add4(chaCtrl, chaCtrl.ChangeSettingEye, true, true, true);
				UpdateCustomUI();
			});
			StartCoroutine(SetInputText());
		}
	}
}
