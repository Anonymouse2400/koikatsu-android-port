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
	public class CvsMole : MonoBehaviour
	{
		[SerializeField]
		private Toggle tglMoleKind;

		[SerializeField]
		private Image imgMoleKind;

		[SerializeField]
		private TextMeshProUGUI textMoleKind;

		[SerializeField]
		private CustomSelectKind customMole;

		[SerializeField]
		private CanvasGroup cgMoleWin;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnMoleColor;

		[SerializeField]
		private Image imgMoleColor;

		[SerializeField]
		private Slider sldPosX;

		[SerializeField]
		private TMP_InputField inpPosX;

		[SerializeField]
		private Button btnPosX;

		[SerializeField]
		private Slider sldPosY;

		[SerializeField]
		private TMP_InputField inpPosY;

		[SerializeField]
		private Button btnPosY;

		[SerializeField]
		private Slider sldScale;

		[SerializeField]
		private TMP_InputField inpScale;

		[SerializeField]
		private Button btnScale;

		[SerializeField]
		private Toggle tglPreset;

		[SerializeField]
		private CanvasGroup cgPreWin;

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
			sldPosX.value = face.moleLayout.x;
			sldPosY.value = face.moleLayout.y;
			sldScale.value = face.moleLayout.w;
			imgMoleColor.color = face.moleColor;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customMole)
			{
				customMole.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.Mole))
			{
				cvsColor.SetColor(face.moleColor);
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			float[] shape = face.shapeValueFace;
			inpPosX.text = CustomBase.ConvertTextFromRate(0, 100, face.moleLayout.x);
			inpPosY.text = CustomBase.ConvertTextFromRate(0, 100, face.moleLayout.y);
			inpScale.text = CustomBase.ConvertTextFromRate(0, 100, face.moleLayout.w);
		}

		public void UpdateSelectMoleKind(string name, Sprite sp, int index)
		{
			if ((bool)textMoleKind)
			{
				textMoleKind.text = name;
			}
			if ((bool)imgMoleKind)
			{
				imgMoleKind.sprite = sp;
			}
			if (face.moleId != index)
			{
				face.moleId = index;
				FuncUpdateMoleKind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleKind);
			}
		}

		public void UpdateMoleColor(Color color)
		{
			face.moleColor = color;
			imgMoleColor.color = color;
			FuncUpdateMoleColor();
		}

		public void UpdateMoleColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleColor);
		}

		public bool FuncUpdateMoleLayout()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceLayoutFlags(false, false, true);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdateMoleKind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceTexFlags(false, false, false, false, false, false, true);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public bool FuncUpdateMoleColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, false, false, false, false, false, true);
			flag |= chaCtrl.CreateFaceTexture();
			return flag | chaCtrl.SetFaceBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Mole) ?? "ホクロの色", CvsColor.ConnectColorKind.Mole, face.moleColor, UpdateMoleColor, UpdateMoleColorHistory, true);
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPosX);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpPosY);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpScale);
			Singleton<CustomBase>.Instance.actUpdateCvsMole += UpdateCustomUI;
			tglMoleKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgMoleWin)
				{
					bool flag = ((cgMoleWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgMoleWin.Enable(isOn);
						if (isOn)
						{
							tglPreset.isOn = false;
						}
					}
				}
			});
			btnMoleColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.Mole)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.Mole) ?? "ホクロの色", CvsColor.ConnectColorKind.Mole, face.moleColor, UpdateMoleColor, UpdateMoleColorHistory, true);
				}
			});
			sldPosX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.moleLayout = new Vector4(value, face.moleLayout.y, 0f, face.moleLayout.w);
				FuncUpdateMoleLayout();
				inpPosX.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPosX.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			sldPosX.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPosX.value = Mathf.Clamp(sldPosX.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPosX.text = CustomBase.ConvertTextFromRate(0, 100, sldPosX.value);
			});
			inpPosX.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPosX.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			btnPosX.onClick.AsObservable().Subscribe(delegate
			{
				float x = Singleton<CustomBase>.Instance.defChaInfo.custom.face.moleLayout.x;
				face.moleLayout = new Vector4(x, face.moleLayout.y, 0f, face.moleLayout.w);
				FuncUpdateMoleLayout();
				inpPosX.text = CustomBase.ConvertTextFromRate(0, 100, x);
				sldPosX.value = x;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			sldPosY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.moleLayout = new Vector4(face.moleLayout.x, value, 0f, face.moleLayout.w);
				FuncUpdateMoleLayout();
				inpPosY.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldPosY.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			sldPosY.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldPosY.value = Mathf.Clamp(sldPosY.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpPosY.text = CustomBase.ConvertTextFromRate(0, 100, sldPosY.value);
			});
			inpPosY.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldPosY.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			btnPosY.onClick.AsObservable().Subscribe(delegate
			{
				float y = Singleton<CustomBase>.Instance.defChaInfo.custom.face.moleLayout.y;
				face.moleLayout = new Vector4(face.moleLayout.x, y, 0f, face.moleLayout.w);
				FuncUpdateMoleLayout();
				inpPosY.text = CustomBase.ConvertTextFromRate(0, 100, y);
				sldPosY.value = y;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			sldScale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.moleLayout = new Vector4(face.moleLayout.x, face.moleLayout.y, 0f, value);
				FuncUpdateMoleLayout();
				inpScale.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldScale.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			sldScale.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldScale.value = Mathf.Clamp(sldScale.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpScale.text = CustomBase.ConvertTextFromRate(0, 100, sldScale.value);
			});
			inpScale.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldScale.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			btnScale.onClick.AsObservable().Subscribe(delegate
			{
				float w = Singleton<CustomBase>.Instance.defChaInfo.custom.face.moleLayout.w;
				face.moleLayout = new Vector4(face.moleLayout.x, face.moleLayout.y, 0f, w);
				FuncUpdateMoleLayout();
				inpScale.text = CustomBase.ConvertTextFromRate(0, 100, w);
				sldScale.value = w;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateMoleLayout);
			});
			tglPreset.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPreWin)
				{
					bool flag2 = ((cgPreWin.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgPreWin.Enable(isOn);
						if (isOn)
						{
							tglMoleKind.isOn = false;
						}
					}
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
