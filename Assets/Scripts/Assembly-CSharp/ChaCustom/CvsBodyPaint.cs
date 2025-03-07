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
	public class CvsBodyPaint : MonoBehaviour
	{
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
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnPaint01Color;

		[SerializeField]
		private Image imgPaint01Color;

		[SerializeField]
		private Toggle tglPaint01Layout;

		[SerializeField]
		private Image imgPaint01Layout;

		[SerializeField]
		private TextMeshProUGUI textPaint01Layout;

		[SerializeField]
		private CustomSelectKind customPaint01Layout;

		[SerializeField]
		private CanvasGroup cgPaint01LayoutWin;

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
		private Toggle tglPaint02Layout;

		[SerializeField]
		private Image imgPaint02Layout;

		[SerializeField]
		private TextMeshProUGUI textPaint02Layout;

		[SerializeField]
		private CustomSelectKind customPaint02Layout;

		[SerializeField]
		private CanvasGroup cgPaint02LayoutWin;

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
			sldPaint01PosX.value = body.paintLayout[0].x;
			sldPaint01PosY.value = body.paintLayout[0].y;
			sldPaint01Rot.value = body.paintLayout[0].z;
			sldPaint01Scale.value = body.paintLayout[0].w;
			sldPaint02PosX.value = body.paintLayout[1].x;
			sldPaint02PosY.value = body.paintLayout[1].y;
			sldPaint02Rot.value = body.paintLayout[1].z;
			sldPaint02Scale.value = body.paintLayout[1].w;
			imgPaint01Color.color = body.paintColor[0];
			imgPaint02Color.color = body.paintColor[1];
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customPaint01)
			{
				customPaint01.UpdateCustomUI();
			}
			if (null != customPaint02)
			{
				customPaint02.UpdateCustomUI();
			}
			if (null != customPaint01Layout)
			{
				customPaint01Layout.UpdateCustomUI();
			}
			if (null != customPaint02Layout)
			{
				customPaint02Layout.UpdateCustomUI();
			}
			switch (cvsColor.connectColorKind)
			{
			case CvsColor.ConnectColorKind.BodyPaint01:
				cvsColor.SetColor(body.paintColor[0]);
				break;
			case CvsColor.ConnectColorKind.BodyPaint02:
				cvsColor.SetColor(body.paintColor[1]);
				break;
			}
		}

		public IEnumerator SetInputText()
		{
			yield return new WaitUntil(() => null != chaCtrl);
			inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[0].x);
			inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[0].y);
			inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[0].z);
			inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[0].w);
			inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[1].x);
			inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[1].y);
			inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[1].z);
			inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, body.paintLayout[1].w);
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
			if (body.paintId[0] != index)
			{
				body.paintId[0] = index;
				FuncUpdatePaint01Kind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Kind);
			}
		}

		public void UpdatePaint01Color(Color color)
		{
			body.paintColor[0] = color;
			imgPaint01Color.color = color;
			FuncUpdatePaint01Color();
		}

		public void UpdatePaint01ColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Color);
		}

		public void UpdateSelectPaint01Layout(string name, Sprite sp, int index)
		{
			if ((bool)textPaint01Layout)
			{
				textPaint01Layout.text = name;
			}
			if ((bool)imgPaint01Layout)
			{
				imgPaint01Layout.sprite = sp;
			}
			if (body.paintLayoutId[0] != index)
			{
				body.paintLayoutId[0] = index;
				body.paintLayout[0] = chaCtrl.GetLayoutInfo(index);
				FuncUpdatePaint01Layout();
				UpdateCustomUI();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			}
		}

		public bool FuncUpdatePaint01Kind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyTexFlags(false, false, true, false, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public bool FuncUpdatePaint01Color()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyColorFlags(false, false, true, false, false, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public bool FuncUpdatePaint01Layout()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyLayoutFlags(true, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
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
			if (body.paintId[1] != index)
			{
				body.paintId[1] = index;
				FuncUpdatePaint02Kind();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Kind);
			}
		}

		public void UpdatePaint02Color(Color color)
		{
			body.paintColor[1] = color;
			imgPaint02Color.color = color;
			FuncUpdatePaint02Color();
		}

		public void UpdatePaint02ColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Color);
		}

		public void UpdateSelectPaint02Layout(string name, Sprite sp, int index)
		{
			if ((bool)textPaint02Layout)
			{
				textPaint02Layout.text = name;
			}
			if ((bool)imgPaint02Layout)
			{
				imgPaint02Layout.sprite = sp;
			}
			if (body.paintLayoutId[1] != index)
			{
				body.paintLayoutId[1] = index;
				body.paintLayout[1] = chaCtrl.GetLayoutInfo(index);
				FuncUpdatePaint02Layout();
				UpdateCustomUI();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			}
		}

		public bool FuncUpdatePaint02Kind()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyTexFlags(false, false, false, true, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public bool FuncUpdatePaint02Color()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyColorFlags(false, false, false, true, false, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public bool FuncUpdatePaint02Layout()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMBodyLayoutFlags(false, true);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BodyPaint01) ?? "ペイント０１の色", CvsColor.ConnectColorKind.BodyPaint01, body.paintColor[0], UpdatePaint01Color, UpdatePaint01ColorHistory, true);
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
			Singleton<CustomBase>.Instance.actUpdateCvsBodyPaint += UpdateCustomUI;
			tglPaint01Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint01Win)
				{
					bool flag = ((cgPaint01Win.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgPaint01Win.Enable(isOn);
						if (isOn)
						{
							tglPaint01Layout.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Layout.isOn = false;
						}
					}
				}
			});
			btnPaint01Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BodyPaint01)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			tglPaint01Layout.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint01LayoutWin)
				{
					bool flag2 = ((cgPaint01LayoutWin.alpha != 0f) ? true : false);
					if (flag2 != isOn)
					{
						cgPaint01LayoutWin.Enable(isOn);
						if (isOn)
						{
							tglPaint01Kind.isOn = false;
							tglPaint02Kind.isOn = false;
							tglPaint02Layout.isOn = false;
						}
					}
				}
			});
			sldPaint01PosX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[0] = new Vector4(value, body.paintLayout[0].y, body.paintLayout[0].z, body.paintLayout[0].w);
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
				float x = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[0].x;
				body.paintLayout[0] = new Vector4(x, body.paintLayout[0].y, body.paintLayout[0].z, body.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosX.text = CustomBase.ConvertTextFromRate(0, 100, x);
				sldPaint01PosX.value = x;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01PosY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, value, body.paintLayout[0].z, body.paintLayout[0].w);
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
				float y = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[0].y;
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, y, body.paintLayout[0].z, body.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01PosY.text = CustomBase.ConvertTextFromRate(0, 100, y);
				sldPaint01PosY.value = y;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Rot.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, body.paintLayout[0].y, value, body.paintLayout[0].w);
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
				float z = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[0].z;
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, body.paintLayout[0].y, z, body.paintLayout[0].w);
				FuncUpdatePaint01Layout();
				inpPaint01Rot.text = CustomBase.ConvertTextFromRate(0, 100, z);
				sldPaint01Rot.value = z;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			sldPaint01Scale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, body.paintLayout[0].y, body.paintLayout[0].z, value);
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
				float w = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[0].w;
				body.paintLayout[0] = new Vector4(body.paintLayout[0].x, body.paintLayout[0].y, body.paintLayout[0].z, w);
				FuncUpdatePaint01Layout();
				inpPaint01Scale.text = CustomBase.ConvertTextFromRate(0, 100, w);
				sldPaint01Scale.value = w;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint01Layout);
			});
			tglPaint02Kind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint02Win)
				{
					bool flag3 = ((cgPaint02Win.alpha != 0f) ? true : false);
					if (flag3 != isOn)
					{
						cgPaint02Win.Enable(isOn);
						if (isOn)
						{
							tglPaint01Kind.isOn = false;
							tglPaint01Layout.isOn = false;
							tglPaint02Layout.isOn = false;
						}
					}
				}
			});
			btnPaint02Color.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.BodyPaint02)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.BodyPaint02) ?? "ペイント０２の色", CvsColor.ConnectColorKind.BodyPaint02, body.paintColor[1], UpdatePaint02Color, UpdatePaint02ColorHistory, true);
				}
			});
			tglPaint02Layout.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgPaint02LayoutWin)
				{
					bool flag4 = ((cgPaint02LayoutWin.alpha != 0f) ? true : false);
					if (flag4 != isOn)
					{
						cgPaint02LayoutWin.Enable(isOn);
						if (isOn)
						{
							tglPaint01Kind.isOn = false;
							tglPaint01Layout.isOn = false;
							tglPaint02Kind.isOn = false;
						}
					}
				}
			});
			sldPaint02PosX.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[1] = new Vector4(value, body.paintLayout[1].y, body.paintLayout[1].z, body.paintLayout[1].w);
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
				float x2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[1].x;
				body.paintLayout[1] = new Vector4(x2, body.paintLayout[1].y, body.paintLayout[1].z, body.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosX.text = CustomBase.ConvertTextFromRate(0, 100, x2);
				sldPaint02PosX.value = x2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02PosY.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, value, body.paintLayout[1].z, body.paintLayout[1].w);
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
				float y2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[1].y;
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, y2, body.paintLayout[1].z, body.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02PosY.text = CustomBase.ConvertTextFromRate(0, 100, y2);
				sldPaint02PosY.value = y2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Rot.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, body.paintLayout[1].y, value, body.paintLayout[1].w);
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
				float z2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[1].z;
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, body.paintLayout[1].y, z2, body.paintLayout[1].w);
				FuncUpdatePaint02Layout();
				inpPaint02Rot.text = CustomBase.ConvertTextFromRate(0, 100, z2);
				sldPaint02Rot.value = z2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			sldPaint02Scale.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, body.paintLayout[1].y, body.paintLayout[1].z, value);
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
				float w2 = Singleton<CustomBase>.Instance.defChaInfo.custom.body.paintLayout[1].w;
				body.paintLayout[1] = new Vector4(body.paintLayout[1].x, body.paintLayout[1].y, body.paintLayout[1].z, w2);
				FuncUpdatePaint02Layout();
				inpPaint02Scale.text = CustomBase.ConvertTextFromRate(0, 100, w2);
				sldPaint02Scale.value = w2;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdatePaint02Layout);
			});
			StartCoroutine(SetInputText());
		}
	}
}
