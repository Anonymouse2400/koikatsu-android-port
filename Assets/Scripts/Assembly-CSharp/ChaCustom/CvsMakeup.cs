using System.Collections;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsMakeup : MonoBehaviour
	{
		public enum MakeupMode
		{
			Base = 0,
			Coordinate = 1
		}

		public MakeupMode modeMakeup;

		[SerializeField]
		private GameObject objWarning;

		[SerializeField]
		private Toggle tglEyeshadowKind;

		[SerializeField]
		private Image imgEyeshadowKind;

		[SerializeField]
		private TextMeshProUGUI textEyeshadowKind;

		[SerializeField]
		private CustomSelectKind customEyeshadow;

		[SerializeField]
		private CanvasGroup cgEyeshadowWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnEyeshadowColor;

		[SerializeField]
		private Image imgEyeshadowColor;

		[SerializeField]
		private Toggle tglCheekKind;

		[SerializeField]
		private Image imgCheekKind;

		[SerializeField]
		private TextMeshProUGUI textCheekKind;

		[SerializeField]
		private CustomSelectKind customCheek;

		[SerializeField]
		private CanvasGroup cgCheekWin;

		[SerializeField]
		private Button btnCheekColor;

		[SerializeField]
		private Image imgCheekColor;

		[SerializeField]
		private Toggle tglLipKind;

		[SerializeField]
		private Image imgLipKind;

		[SerializeField]
		private TextMeshProUGUI textLipKind;

		[SerializeField]
		private CustomSelectKind customLip;

		[SerializeField]
		private CanvasGroup cgLipWin;

		[SerializeField]
		private Button btnLipColor;

		[SerializeField]
		private Image imgLipColor;

		[SerializeField]
		private Toggle tglPaint01Kind;

		[SerializeField]
		private Image imgPaint01Kind;

		[SerializeField]
		private TextMeshProUGUI textPaint01Kind;

		[SerializeField]
		private CustomSelectKind customPaint01;

		[SerializeField]
		private CanvasGroup cgPaint01Win;

		[SerializeField]
		private Button btnPaint01Color;

		[SerializeField]
		private Image imgPaint01Color;

		[SerializeField]
		private Slider sldPaint01PosX;

		[SerializeField]
		private TMP_InputField inpPaint01PosX;

		[SerializeField]
		private Button btnPaint01PosX;

		[SerializeField]
		private Slider sldPaint01PosY;

		[SerializeField]
		private TMP_InputField inpPaint01PosY;

		[SerializeField]
		private Button btnPaint01PosY;

		[SerializeField]
		private Slider sldPaint01Rot;

		[SerializeField]
		private TMP_InputField inpPaint01Rot;

		[SerializeField]
		private Button btnPaint01Rot;

		[SerializeField]
		private Slider sldPaint01Scale;

		[SerializeField]
		private TMP_InputField inpPaint01Scale;

		[SerializeField]
		private Button btnPaint01Scale;

		[SerializeField]
		private Toggle tglPaint01Preset;

		[SerializeField]
		private CanvasGroup cgPaint01PreWin;

		[SerializeField]
		private Toggle tglPaint02Kind;

		[SerializeField]
		private Image imgPaint02Kind;

		[SerializeField]
		private TextMeshProUGUI textPaint02Kind;

		[SerializeField]
		private CustomSelectKind customPaint02;

		[SerializeField]
		private CanvasGroup cgPaint02Win;

		[SerializeField]
		private Button btnPaint02Color;

		[SerializeField]
		private Image imgPaint02Color;

		[SerializeField]
		private Slider sldPaint02PosX;

		[SerializeField]
		private TMP_InputField inpPaint02PosX;

		[SerializeField]
		private Button btnPaint02PosX;

		[SerializeField]
		private Slider sldPaint02PosY;

		[SerializeField]
		private TMP_InputField inpPaint02PosY;

		[SerializeField]
		private Button btnPaint02PosY;

		[SerializeField]
		private Slider sldPaint02Rot;

		[SerializeField]
		private TMP_InputField inpPaint02Rot;

		[SerializeField]
		private Button btnPaint02Rot;

		[SerializeField]
		private Slider sldPaint02Scale;

		[SerializeField]
		private TMP_InputField inpPaint02Scale;

		[SerializeField]
		private Button btnPaint02Scale;

		[SerializeField]
		private Toggle tglPaint02Preset;

		[SerializeField]
		private CanvasGroup cgPaint02PreWin;

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

		private ChaFileMakeup makeup
		{
			get
			{
				if (modeMakeup == MakeupMode.Base)
				{
					return face.baseMakeup;
				}
				return chaCtrl.chaFile.coordinate[chaCtrl.chaFile.status.coordinateType].makeup;
			}
		}

		public void CalculateUI()
		{
			sldPaint01PosX.value = makeup.paintLayout[0].x;
			sldPaint01PosY.value = makeup.paintLayout[0].y;
			sldPaint01Rot.value = makeup.paintLayout[0].z;
			sldPaint01Scale.value = makeup.paintLayout[0].w;
			sldPaint02PosX.value = makeup.paintLayout[1].x;
			sldPaint02PosY.value = makeup.paintLayout[1].y;
			sldPaint02Rot.value = makeup.paintLayout[1].z;
			sldPaint02Scale.value = makeup.paintLayout[1].w;
			imgEyeshadowColor.color = makeup.eyeshadowColor;
			imgCheekColor.color = makeup.cheekColor;
			imgLipColor.color = makeup.lipColor;
			imgPaint01Color.color = makeup.paintColor[0];
			imgPaint02Color.color = makeup.paintColor[1];
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customEyeshadow)
			{
				customEyeshadow.UpdateCustomUI();
			}
			if (null != customCheek)
			{
				customCheek.UpdateCustomUI();
			}
			if (null != customLip)
			{
				customLip.UpdateCustomUI();
			}
			if (null != customPaint01)
			{
				customPaint01.UpdateCustomUI();
			}
			if (null != customPaint02)
			{
				customPaint02.UpdateCustomUI();
			}
			switch (cvsColor.connectColorKind)
			{
			case CvsColor.ConnectColorKind.BaseEyeShadow:
				cvsColor.SetColor(makeup.eyeshadowColor);
				break;
			case CvsColor.ConnectColorKind.BaseCheek:
				cvsColor.SetColor(makeup.cheekColor);
				break;
			case CvsColor.ConnectColorKind.BaseLip:
				cvsColor.SetColor(makeup.lipColor);
				break;
			case CvsColor.ConnectColorKind.BaseFacePaint01:
				cvsColor.SetColor(makeup.paintColor[0]);
				break;
			case CvsColor.ConnectColorKind.BaseFacePaint02:
				cvsColor.SetColor(makeup.paintColor[1]);
				break;
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[0].x);
			inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[0].y);
			inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[0].z);
			inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[0].w);
			inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[1].x);
			inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[1].y);
			inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[1].z);
			inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, makeup.paintLayout[1].w);
		}

		public void UpdateSelectEyeshadowKind(string name, Sprite sp, int index)
		{
			if ((bool)textEyeshadowKind)
			{
				textEyeshadowKind.text = name;
			}
			if ((bool)imgEyeshadowKind)
			{
				imgEyeshadowKind.sprite = sp;
			}
			if (makeup.eyeshadowId != index)
			{
				makeup.eyeshadowId = index;
				chaCtrl.ChangeSettingEyeShadow();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeShadow);
			}
		}

		public void UpdateEyeshadowColor(Color color)
		{
			makeup.eyeshadowColor = color;
			imgEyeshadowColor.color = color;
			chaCtrl.ChangeSettingEyeShadowColor();
		}

		public void UpdateEyeshadowColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingEyeShadowColor);
		}

		public void UpdateSelectCheekKind(string name, Sprite sp, int index)
		{
			if ((bool)textCheekKind)
			{
				textCheekKind.text = name;
			}
			if ((bool)imgCheekKind)
			{
				imgCheekKind.sprite = sp;
			}
			if (makeup.cheekId != index)
			{
				makeup.cheekId = index;
				FuncUpdateCheekKind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCheekKind);
			}
		}

		public void UpdateCheekColor(Color color)
		{
			makeup.cheekColor = color;
			imgCheekColor.color = color;
			FuncUpdateCheekColor();
		}

		public void UpdateCheekColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateCheekColor);
		}

		public bool FuncUpdateCheekKind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceTexFlags(false, false, false, false, true, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdateCheekColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, false, false, false, true, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public void UpdateSelectLipKind(string name, Sprite sp, int index)
		{
			if ((bool)textLipKind)
			{
				textLipKind.text = name;
			}
			if ((bool)imgLipKind)
			{
				imgLipKind.sprite = sp;
			}
			if (makeup.lipId != index)
			{
				makeup.lipId = index;
				chaCtrl.ChangeSettingLip();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingLip);
			}
		}

		public void UpdateLipColor(Color color)
		{
			makeup.lipColor = color;
			imgLipColor.color = color;
			chaCtrl.ChangeSettingLipColor();
		}

		public void UpdateLipColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingLipColor);
		}

		public void UpdateSelectPaint01Kind(string name, Sprite sp, int index)
		{
			if ((bool)textPaint01Kind)
			{
				textPaint01Kind.text = name;
			}
			if ((bool)imgPaint01Kind)
			{
				imgPaint01Kind.sprite = sp;
			}
			if (makeup.paintId[0] != index)
			{
				makeup.paintId[0] = index;
				FuncUpdatePaint01Kind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Kind);
			}
		}

		public void UpdatePaint01Color(Color color)
		{
			makeup.paintColor[0] = color;
			imgPaint01Color.color = color;
			FuncUpdatePaint01Color();
		}

		public void UpdatePaint01ColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Color);
		}

		public bool FuncUpdatePaint01Kind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceTexFlags(false, false, true, false, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdatePaint01Color()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, false, true, false, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdatePaint01Layout()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceLayoutFlags(true, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public void UpdateSelectPaint02Kind(string name, Sprite sp, int index)
		{
			if ((bool)textPaint02Kind)
			{
				textPaint02Kind.text = name;
			}
			if ((bool)imgPaint02Kind)
			{
				imgPaint02Kind.sprite = sp;
			}
			if (makeup.paintId[1] != index)
			{
				makeup.paintId[1] = index;
				FuncUpdatePaint02Kind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Kind);
			}
		}

		public void UpdatePaint02Color(Color color)
		{
			makeup.paintColor[1] = color;
			imgPaint02Color.color = color;
			FuncUpdatePaint02Color();
		}

		public void UpdatePaint02ColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Color);
		}

		public bool FuncUpdatePaint02Kind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceTexFlags(false, false, false, true, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdatePaint02Color()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, false, false, true, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdatePaint02Layout()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceLayoutFlags(false, true, false);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public void UpdatePushFacePaintLayout(Vector4 layout)
		{
			int num = ((!tglPaint01Preset.isOn) ? 1 : 0);
			chaCtrl.chaFile.custom.face.baseMakeup.paintLayout[num] = layout;
			if (num == 0)
			{
				FuncUpdatePaint01Layout();
				UpdateCustomUI();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			}
			else
			{
				FuncUpdatePaint02Layout();
				UpdateCustomUI();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			}
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BaseEyeShadow) ?? "アイシャドウの色", CvsColor.ConnectColorKind.BaseEyeShadow, makeup.eyeshadowColor, UpdateEyeshadowColor, UpdateEyeshadowColorHistory, true);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint01PosX);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint01PosY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint01Rot);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint01Scale);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint02PosX);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint02PosY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint02Rot);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPaint02Scale);
			Singleton<CustomBase>.Instance.actUpdateCvsBaseMakeup += UpdateCustomUI;
			tglEyeshadowKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgEyeshadowWin)
				{
					bool flag = ((cgEyeshadowWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgEyeshadowWin.Enable(isOn);
						if (isOn)
						{
							tglCheekKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			btnEyeshadowColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BaseEyeShadow)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			tglCheekKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgCheekWin)
				{
					bool flag2 = ((cgCheekWin.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgCheekWin.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			btnCheekColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BaseCheek)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BaseCheek) ?? "チークの色", CvsColor.ConnectColorKind.BaseCheek, makeup.cheekColor, UpdateCheekColor, UpdateCheekColorHistory, true);
				}
			});
			tglLipKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgLipWin)
				{
					bool flag3 = ((cgLipWin.alpha != 0f) ? true : false);
					if (flag3 != isOn)
					{
						cgLipWin.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglCheekKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			btnLipColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BaseLip)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BaseLip) ?? "リップの色", CvsColor.ConnectColorKind.BaseLip, makeup.lipColor, UpdateLipColor, UpdateLipColorHistory, true);
				}
			});
			tglPaint01Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint01Win)
				{
					bool flag4 = ((cgPaint01Win.alpha != 0f) ? true : false);
					if (flag4 != isOn)
					{
						cgPaint01Win.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglCheekKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			btnPaint01Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BaseFacePaint01)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BaseFacePaint01) ?? "ペイント０１の色", CvsColor.ConnectColorKind.BaseFacePaint01, makeup.paintColor[0], UpdatePaint01Color, UpdatePaint01ColorHistory, true);
				}
			});
			sldPaint01PosX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[0] = new Vector4(value, makeup.paintLayout[0].y, makeup.paintLayout[0].z, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint01PosX.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01PosX.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint01PosX.value = Mathf.Clamp(sldPaint01PosX.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint01PosX.value);
			});
			inpPaint01PosX.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint01PosX.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			btnPaint01PosX.onClick.AsObservable().Subscribe(delegate
			{
				float x = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[0].x;
				makeup.paintLayout[0] = new Vector4(x, makeup.paintLayout[0].y, makeup.paintLayout[0].z, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, x);
				sldPaint01PosX.value = x;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01PosY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, value, makeup.paintLayout[0].z, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint01PosY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01PosY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint01PosY.value = Mathf.Clamp(sldPaint01PosY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint01PosY.value);
			});
			inpPaint01PosY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint01PosY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			btnPaint01PosY.onClick.AsObservable().Subscribe(delegate
			{
				float y = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[0].y;
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, y, makeup.paintLayout[0].z, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, y);
				sldPaint01PosY.value = y;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Rot.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, makeup.paintLayout[0].y, value, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint01Rot.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Rot.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint01Rot.value = Mathf.Clamp(sldPaint01Rot.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint01Rot.value);
			});
			inpPaint01Rot.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint01Rot.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			btnPaint01Rot.onClick.AsObservable().Subscribe(delegate
			{
				float z = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[0].z;
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, makeup.paintLayout[0].y, z, makeup.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, z);
				sldPaint01Rot.value = z;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Scale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, makeup.paintLayout[0].y, makeup.paintLayout[0].z, value);
				FuncUpdatePaint01Layout();
				inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint01Scale.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Scale.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint01Scale.value = Mathf.Clamp(sldPaint01Scale.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint01Scale.value);
			});
			inpPaint01Scale.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint01Scale.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			btnPaint01Scale.onClick.AsObservable().Subscribe(delegate
			{
				float w = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[0].w;
				makeup.paintLayout[0] = new Vector4(makeup.paintLayout[0].x, makeup.paintLayout[0].y, makeup.paintLayout[0].z, w);
				FuncUpdatePaint01Layout();
				inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, w);
				sldPaint01Scale.value = w;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			tglPaint01Preset.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint01PreWin)
				{
					bool flag5 = ((cgPaint01PreWin.alpha != 0f) ? true : false);
					if (flag5 != isOn)
					{
						cgPaint01PreWin.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglCheekKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			tglPaint02Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint02Win)
				{
					bool flag6 = ((cgPaint02Win.alpha != 0f) ? true : false);
					if (flag6 != isOn)
					{
						cgPaint02Win.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglCheekKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Preset.isOn = false;
						}
					}
				}
			});
			btnPaint02Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BaseFacePaint02)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BaseFacePaint02) ?? "ペイント０２の色", CvsColor.ConnectColorKind.BaseFacePaint02, makeup.paintColor[1], UpdatePaint02Color, UpdatePaint02ColorHistory, true);
				}
			});
			sldPaint02PosX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[1] = new Vector4(value, makeup.paintLayout[1].y, makeup.paintLayout[1].z, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint02PosX.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02PosX.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint02PosX.value = Mathf.Clamp(sldPaint02PosX.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint02PosX.value);
			});
			inpPaint02PosX.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint02PosX.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			btnPaint02PosX.onClick.AsObservable().Subscribe(delegate
			{
				float x2 = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[1].x;
				makeup.paintLayout[1] = new Vector4(x2, makeup.paintLayout[1].y, makeup.paintLayout[1].z, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, x2);
				sldPaint02PosX.value = x2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02PosY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, value, makeup.paintLayout[1].z, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint02PosY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02PosY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint02PosY.value = Mathf.Clamp(sldPaint02PosY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint02PosY.value);
			});
			inpPaint02PosY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint02PosY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			btnPaint02PosY.onClick.AsObservable().Subscribe(delegate
			{
				float y2 = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[1].y;
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, y2, makeup.paintLayout[1].z, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, y2);
				sldPaint02PosY.value = y2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Rot.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, makeup.paintLayout[1].y, value, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint02Rot.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Rot.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint02Rot.value = Mathf.Clamp(sldPaint02Rot.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint02Rot.value);
			});
			inpPaint02Rot.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint02Rot.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			btnPaint02Rot.onClick.AsObservable().Subscribe(delegate
			{
				float z2 = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[1].z;
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, makeup.paintLayout[1].y, z2, makeup.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, z2);
				sldPaint02Rot.value = z2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Scale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, makeup.paintLayout[1].y, makeup.paintLayout[1].z, value);
				FuncUpdatePaint02Layout();
				inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPaint02Scale.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Scale.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPaint02Scale.value = Mathf.Clamp(sldPaint02Scale.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, sldPaint02Scale.value);
			});
			inpPaint02Scale.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPaint02Scale.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			btnPaint02Scale.onClick.AsObservable().Subscribe(delegate
			{
				float w2 = Singleton<CustomBase>.Instance.defChaInfo.custom.face.baseMakeup.paintLayout[1].w;
				makeup.paintLayout[1] = new Vector4(makeup.paintLayout[1].x, makeup.paintLayout[1].y, makeup.paintLayout[1].z, w2);
				FuncUpdatePaint02Layout();
				inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, w2);
				sldPaint02Scale.value = w2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			tglPaint02Preset.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint02PreWin)
				{
					bool flag7 = ((cgPaint02PreWin.alpha != 0f) ? true : false);
					if (flag7 != isOn)
					{
						cgPaint02PreWin.Enable(isOn);
						if (isOn)
						{
							tglEyeshadowKind.isOn = false;
							tglCheekKind.isOn = false;
							tglLipKind.isOn = false;
							tglPaint01Kind.isOn = false;
							tglPaint01Preset.isOn = false;
							tglPaint02Kind.isOn = false;
						}
					}
				}
			});
			StartCoroutine(SetInputText());
		}

		private void Update()
		{
			if ((bool)objWarning)
			{
				bool active = chaCtrl.IsGagEyes();
				objWarning.SetActiveIfDifferent(active);
			}
		}
	}
}
