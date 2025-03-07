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
	public class CvsLeg : MonoBehaviour
	{
		[SerializeField]
		private Slider sldThighUpW;

		[SerializeField]
		private TMP_InputField inpThighUpW;

		[SerializeField]
		private Button btnThighUpW;

		[SerializeField]
		private Slider sldThighUpZ;

		[SerializeField]
		private TMP_InputField inpThighUpZ;

		[SerializeField]
		private Button btnThighUpZ;

		[SerializeField]
		private Slider sldThighLowW;

		[SerializeField]
		private TMP_InputField inpThighLowW;

		[SerializeField]
		private Button btnThighLowW;

		[SerializeField]
		private Slider sldThighLowZ;

		[SerializeField]
		private TMP_InputField inpThighLowZ;

		[SerializeField]
		private Button btnThighLowZ;

		[SerializeField]
		private Slider sldKneeLowW;

		[SerializeField]
		private TMP_InputField inpKneeLowW;

		[SerializeField]
		private Button btnKneeLowW;

		[SerializeField]
		private Slider sldKneeLowZ;

		[SerializeField]
		private TMP_InputField inpKneeLowZ;

		[SerializeField]
		private Button btnKneeLowZ;

		[SerializeField]
		private Slider sldCalf;

		[SerializeField]
		private TMP_InputField inpCalf;

		[SerializeField]
		private Button btnCalf;

		[SerializeField]
		private Slider sldAnkleW;

		[SerializeField]
		private TMP_InputField inpAnkleW;

		[SerializeField]
		private Button btnAnkleW;

		[SerializeField]
		private Slider sldAnkleZ;

		[SerializeField]
		private TMP_InputField inpAnkleZ;

		[SerializeField]
		private Button btnAnkleZ;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[9] { 28, 29, 30, 31, 32, 33, 34, 35, 36 };

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
			sliders = new Slider[9] { sldThighUpW, sldThighUpZ, sldThighLowW, sldThighLowZ, sldKneeLowW, sldKneeLowZ, sldCalf, sldAnkleW, sldAnkleZ };
			inputs = new TMP_InputField[9] { inpThighUpW, inpThighUpZ, inpThighLowW, inpThighLowZ, inpKneeLowW, inpKneeLowZ, inpCalf, inpAnkleW, inpAnkleZ };
			buttons = new Button[9] { btnThighUpW, btnThighUpZ, btnThighLowW, btnThighLowZ, btnKneeLowW, btnKneeLowZ, btnCalf, btnAnkleW, btnAnkleZ };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsLeg += UpdateCustomUI;
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
