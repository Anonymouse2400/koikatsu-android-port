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
	public class CvsBodyAll : MonoBehaviour
	{
		[SerializeField]
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

		[SerializeField]
		private Toggle tglDetailKind;

		[SerializeField]
		private Image imgDetailKind;

		[SerializeField]
		private TextMeshProUGUI textDetailKind;

		[SerializeField]
		private CustomSelectKind customBodyDetail;

		[SerializeField]
		private CanvasGroup cgDetailWin;

		[SerializeField]
		private Slider sldDetailPow;

		[SerializeField]
		private TMP_InputField inpDetailPow;

		[SerializeField]
		private Button btnDetailPow;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnSkinMainColor;

		[SerializeField]
		private Image imgSkinMainColor;

		[SerializeField]
		private Button btnSkinSubColor;

		[SerializeField]
		private Image imgSkinSubColor;

		[SerializeField]
		private Slider sldGlossPow;

		[SerializeField]
		private TMP_InputField inpGlossPow;

		[SerializeField]
		private Button btnGlossPow;

		[SerializeField]
		private Toggle tglSkinLine;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[2] { 0, 1 };

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
			sldDetailPow.value = body.detailPower;
			imgSkinMainColor.color = body.skinMainColor;
			imgSkinSubColor.color = body.skinSubColor;
			sldGlossPow.value = body.skinGlossPower;
			tglSkinLine.isOn = body.drawAddLine;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customBodyDetail)
			{
				customBodyDetail.UpdateCustomUI();
			}
			if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.SkinMain))
			{
				cvsColor.SetColor(body.skinMainColor);
			}
			else if (cvsColor.CheckConnectKind(CvsColor.ConnectColorKind.SkinSub))
			{
				cvsColor.SetColor(body.skinSubColor);
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
			inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, body.detailPower);
			inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, body.skinGlossPower);
		}

		public void UpdateSelectDetailKind(string name, Sprite sp, int index)
		{
			if ((bool)textDetailKind)
			{
				textDetailKind.text = name;
			}
			if ((bool)imgDetailKind)
			{
				imgDetailKind.sprite = sp;
			}
			if (body.detailId != index)
			{
				body.detailId = index;
				chaCtrl.ChangeSettingBodyDetail();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingBodyDetail);
			}
		}

		public void UpdateSkinMainColor(Color color)
		{
			body.skinMainColor = color;
			imgSkinMainColor.color = color;
			FuncUpdateSkinMainColor();
		}

		public void UpdateSkinMainColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSkinMainColor);
		}

		public bool FuncUpdateSkinMainColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(true, false, false, false, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			flag |= chaCtrl.SetFaceBaseMaterial();
			chaCtrl.AddUpdateCMBodyColorFlags(true, false, false, false, false, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public void UpdateSkinSubColor(Color color)
		{
			body.skinSubColor = color;
			imgSkinSubColor.color = color;
			FuncUpdateSkinSubColor();
		}

		public void UpdateSkinSubColorHistory()
		{
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, FuncUpdateSkinSubColor);
		}

		public bool FuncUpdateSkinSubColor()
		{
			bool flag = true;
			chaCtrl.AddUpdateCMFaceColorFlags(false, true, false, false, false, false, false);
			flag |= chaCtrl.CreateFaceTexture();
			flag |= chaCtrl.SetFaceBaseMaterial();
			chaCtrl.AddUpdateCMBodyColorFlags(false, true, false, false, false, false);
			flag |= chaCtrl.CreateBodyTexture();
			return flag | chaCtrl.SetBodyBaseMaterial();
		}

		public void SetDefaultColorWindow()
		{
			cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.SkinMain) ?? "肌の色", CvsColor.ConnectColorKind.SkinMain, body.skinMainColor, UpdateSkinMainColor, UpdateSkinMainColorHistory, false);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[2] { sldHeight, sldHeadSize };
			inputs = new TMP_InputField[2] { inpHeight, inpHeadSize };
			buttons = new Button[2] { btnHeight, btnHeadSize };
		}

		protected virtual void Start()
		{
			if (chaCtrl.sex == 0 && (bool)sldHeight)
			{
				sldHeight.transform.parent.gameObject.SetActiveIfDifferent(false);
			}
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpDetailPow);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGlossPow);
			Singleton<CustomBase>.Instance.actUpdateCvsBodyAll += UpdateCustomUI;
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
			tglDetailKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgDetailWin)
				{
					bool flag = ((cgDetailWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgDetailWin.Enable(isOn);
					}
				}
			});
			sldDetailPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.detailPower = value;
				chaCtrl.ChangeSettingBodyDetailPower();
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldDetailPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingBodyDetailPower);
			});
			sldDetailPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldDetailPow.value = Mathf.Clamp(sldDetailPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, sldDetailPow.value);
			});
			inpDetailPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldDetailPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingBodyDetailPower);
			});
			btnDetailPow.onClick.AsObservable().Subscribe(delegate
			{
				float detailPower = Singleton<CustomBase>.Instance.defChaInfo.custom.body.detailPower;
				body.detailPower = detailPower;
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, detailPower);
				sldDetailPow.value = detailPower;
				chaCtrl.ChangeSettingBodyDetailPower();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingBodyDetailPower);
			});
			btnSkinMainColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.SkinMain)
				{
					cvsColor.Close();
				}
				else
				{
					SetDefaultColorWindow();
				}
			});
			btnSkinSubColor.OnClickAsObservable().Subscribe(delegate
			{
				if (cvsColor.isOpen && cvsColor.connectColorKind == CvsColor.ConnectColorKind.SkinSub)
				{
					cvsColor.Close();
				}
				else
				{
					cvsColor.Setup(Singleton<CustomBase>.Instance.TranslateColorWindowTitle(CvsColor.ConnectColorKind.SkinSub) ?? "肌の色(赤み部分)", CvsColor.ConnectColorKind.SkinSub, body.skinSubColor, UpdateSkinSubColor, UpdateSkinSubColorHistory, false);
				}
			});
			sldGlossPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				body.skinGlossPower = value;
				chaCtrl.ChangeSettingSkinGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldGlossPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingSkinGlossPower);
			});
			sldGlossPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGlossPow.value = Mathf.Clamp(sldGlossPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, sldGlossPow.value);
			});
			inpGlossPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGlossPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingSkinGlossPower);
			});
			btnGlossPow.onClick.AsObservable().Subscribe(delegate
			{
				float skinGlossPower = Singleton<CustomBase>.Instance.defChaInfo.custom.body.skinGlossPower;
				body.skinGlossPower = skinGlossPower;
				chaCtrl.ChangeSettingSkinGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, skinGlossPower);
				sldGlossPow.value = skinGlossPower;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingSkinGlossPower);
			});
			tglSkinLine.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if (!Singleton<CustomBase>.Instance.updateCustomUI && body.drawAddLine != isOn)
				{
					body.drawAddLine = isOn;
					chaCtrl.VisibleAddBodyLine();
					Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.VisibleAddBodyLine);
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
