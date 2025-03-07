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
	public class CvsFaceAll : MonoBehaviour
	{
		[SerializeField]
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

		[SerializeField]
		private Toggle tglDetailKind;

		[SerializeField]
		private Image imgDetailKind;

		[SerializeField]
		private TextMeshProUGUI textDetailKind;

		[SerializeField]
		private CustomSelectKind customFaceDetail;

		[SerializeField]
		private CanvasGroup cgDetailWin;

		[SerializeField]
		private Slider sldDetailPow;

		[SerializeField]
		private TMP_InputField inpDetailPow;

		[SerializeField]
		private Button btnDetailPow;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[6] { 0, 1, 2, 3, 4, 5 };

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
			sldDetailPow.value = face.detailPower;
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
			if (null != customFaceDetail)
			{
				customFaceDetail.UpdateCustomUI();
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
			inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, face.detailPower);
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
			if (face.detailId != index)
			{
				face.detailId = index;
				chaCtrl.ChangeSettingFaceDetail();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingFaceDetail);
			}
		}

		protected virtual void Awake()
		{
			sliders = new Slider[6] { sldFaceBaseW, sldFaceUpZ, sldFaceUpY, sldFaceUpSize, sldFaceLowZ, sldFaceLowW };
			inputs = new TMP_InputField[6] { inpFaceBaseW, inpFaceUpZ, inpFaceUpY, inpFaceUpSize, inpFaceLowZ, inpFaceLowW };
			buttons = new Button[6] { btnFaceBaseW, btnFaceUpZ, btnFaceUpY, btnFaceUpSize, btnFaceLowZ, btnFaceLowW };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpDetailPow);
			Singleton<CustomBase>.Instance.actUpdateCvsFaceAll += UpdateCustomUI;
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
				face.detailPower = value;
				chaCtrl.ChangeSettingFaceDetailPower();
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldDetailPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingFaceDetailPower);
			});
			sldDetailPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldDetailPow.value = Mathf.Clamp(sldDetailPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, sldDetailPow.value);
			});
			inpDetailPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldDetailPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingFaceDetailPower);
			});
			btnDetailPow.onClick.AsObservable().Subscribe(delegate
			{
				float detailPower = Singleton<CustomBase>.Instance.defChaInfo.custom.face.detailPower;
				face.detailPower = detailPower;
				inpDetailPow.text = CustomBase.ConvertTextFromRate(0, 100, detailPower);
				sldDetailPow.value = detailPower;
				chaCtrl.ChangeSettingFaceDetailPower();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingFaceDetailPower);
			});
			StartCoroutine(SetInputText());
		}
	}
}
