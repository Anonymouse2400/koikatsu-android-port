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
	public class CvsBodyUpper : MonoBehaviour
	{
		[SerializeField]
		private Slider sldNeckW;

		[SerializeField]
		private TMP_InputField inpNeckW;

		[SerializeField]
		private Button btnNeckW;

		[SerializeField]
		private Slider sldNeckZ;

		[SerializeField]
		private TMP_InputField inpNeckZ;

		[SerializeField]
		private Button btnNeckZ;

		[SerializeField]
		private Slider sldBodyShoulderW;

		[SerializeField]
		private TMP_InputField inpBodyShoulderW;

		[SerializeField]
		private Button btnBodyShoulderW;

		[SerializeField]
		private Slider sldBodyShoulderZ;

		[SerializeField]
		private TMP_InputField inpBodyShoulderZ;

		[SerializeField]
		private Button btnBodyShoulderZ;

		[SerializeField]
		private Slider sldBodyUpW;

		[SerializeField]
		private TMP_InputField inpBodyUpW;

		[SerializeField]
		private Button btnBodyUpW;

		[SerializeField]
		private Slider sldBodyUpZ;

		[SerializeField]
		private TMP_InputField inpBodyUpZ;

		[SerializeField]
		private Button btnBodyUpZ;

		[SerializeField]
		private Slider sldBodyLowW;

		[SerializeField]
		private TMP_InputField inpBodyLowW;

		[SerializeField]
		private Button btnBodyLowW;

		[SerializeField]
		private Slider sldBodyLowZ;

		[SerializeField]
		private TMP_InputField inpBodyLowZ;

		[SerializeField]
		private Button btnBodyLowZ;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[8] { 2, 3, 14, 15, 16, 17, 18, 19 };

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
		}

		public virtual void UpdateCustomUI()
		{
			CalculateUI();
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
		}

		protected virtual void Awake()
		{
			sliders = new Slider[8] { sldNeckW, sldNeckZ, sldBodyShoulderW, sldBodyShoulderZ, sldBodyUpW, sldBodyUpZ, sldBodyLowW, sldBodyLowZ };
			inputs = new TMP_InputField[8] { inpNeckW, inpNeckZ, inpBodyShoulderW, inpBodyShoulderZ, inpBodyUpW, inpBodyUpZ, inpBodyLowW, inpBodyLowZ };
			buttons = new Button[8] { btnNeckW, btnNeckZ, btnBodyShoulderW, btnBodyShoulderZ, btnBodyUpW, btnBodyUpZ, btnBodyLowW, btnBodyLowZ };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsBodyUpper += UpdateCustomUI;
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
			StartCoroutine(SetInputText());
		}
	}
}
