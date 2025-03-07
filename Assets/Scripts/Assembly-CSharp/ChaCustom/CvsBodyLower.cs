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
	public class CvsBodyLower : MonoBehaviour
	{
		[SerializeField]
		private Slider sldWaistY;

		[SerializeField]
		private TMP_InputField inpWaistY;

		[SerializeField]
		private Button btnWaistY;

		[SerializeField]
		private Slider sldBelly;

		[SerializeField]
		private TMP_InputField inpBelly;

		[SerializeField]
		private Button btnBelly;

		[SerializeField]
		private Slider sldWaistUpW;

		[SerializeField]
		private TMP_InputField inpWaistUpW;

		[SerializeField]
		private Button btnWaistUpW;

		[SerializeField]
		private Slider sldWaistUpZ;

		[SerializeField]
		private TMP_InputField inpWaistUpZ;

		[SerializeField]
		private Button btnWaistUpZ;

		[SerializeField]
		private Slider sldWaistLowW;

		[SerializeField]
		private TMP_InputField inpWaistLowW;

		[SerializeField]
		private Button btnWaistLowW;

		[SerializeField]
		private Slider sldWaistLowZ;

		[SerializeField]
		private TMP_InputField inpWaistLowZ;

		[SerializeField]
		private Button btnWaistLowZ;

		[SerializeField]
		private Slider sldHip;

		[SerializeField]
		private TMP_InputField inpHip;

		[SerializeField]
		private Button btnHip;

		[SerializeField]
		private Slider sldHipRotX;

		[SerializeField]
		private TMP_InputField inpHipRotX;

		[SerializeField]
		private Button btnHipRotX;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[8] { 20, 21, 22, 23, 24, 25, 26, 27 };

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
			sliders = new Slider[8] { sldWaistY, sldBelly, sldWaistUpW, sldWaistUpZ, sldWaistLowW, sldWaistLowZ, sldHip, sldHipRotX };
			inputs = new TMP_InputField[8] { inpWaistY, inpBelly, inpWaistUpW, inpWaistUpZ, inpWaistLowW, inpWaistLowZ, inpHip, inpHipRotX };
			buttons = new Button[8] { btnWaistY, btnBelly, btnWaistUpW, btnWaistUpZ, btnWaistLowW, btnWaistLowZ, btnHip, btnHipRotX };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsBodyLower += UpdateCustomUI;
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
