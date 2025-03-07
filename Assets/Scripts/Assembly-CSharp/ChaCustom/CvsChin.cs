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
	public class CvsChin : MonoBehaviour
	{
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

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[8] { 6, 7, 8, 9, 10, 11, 12, 13 };

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
			sliders = new Slider[8] { sldChinLowY, sldChinLowZ, sldChinY, sldChinW, sldChinZ, sldChinTipY, sldChinTipZ, sldChinTipW };
			inputs = new TMP_InputField[8] { inpChinLowY, inpChinLowZ, inpChinY, inpChinW, inpChinZ, inpChinTipY, inpChinTipZ, inpChinTipW };
			buttons = new Button[8] { btnChinLowY, btnChinLowZ, btnChinY, btnChinW, btnChinZ, btnChinTipY, btnChinTipZ, btnChinTipW };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsChin += UpdateCustomUI;
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
			StartCoroutine(SetInputText());
		}
	}
}
