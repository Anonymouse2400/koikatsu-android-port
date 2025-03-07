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
	public class CvsArm : MonoBehaviour
	{
		[SerializeField]
		private Slider sldShoulderW;

		[SerializeField]
		private TMP_InputField inpShoulderW;

		[SerializeField]
		private Button btnShoulderW;

		[SerializeField]
		private Slider sldShoulderZ;

		[SerializeField]
		private TMP_InputField inpShoulderZ;

		[SerializeField]
		private Button btnShoulderZ;

		[SerializeField]
		private Slider sldArmUpW;

		[SerializeField]
		private TMP_InputField inpArmUpW;

		[SerializeField]
		private Button btnArmUpW;

		[SerializeField]
		private Slider sldArmUpZ;

		[SerializeField]
		private TMP_InputField inpArmUpZ;

		[SerializeField]
		private Button btnArmUpZ;

		[SerializeField]
		private Slider sldElbowW;

		[SerializeField]
		private TMP_InputField inpElbowW;

		[SerializeField]
		private Button btnElbowW;

		[SerializeField]
		private Slider sldElbowZ;

		[SerializeField]
		private TMP_InputField inpElbowZ;

		[SerializeField]
		private Button btnElbowZ;

		[SerializeField]
		private Slider sldArmLow;

		[SerializeField]
		private TMP_InputField inpArmLow;

		[SerializeField]
		private Button btnArmLow;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[7] { 37, 38, 39, 40, 41, 42, 43 };

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
			sliders = new Slider[7] { sldShoulderW, sldShoulderZ, sldArmUpW, sldArmUpZ, sldElbowW, sldElbowZ, sldArmLow };
			inputs = new TMP_InputField[7] { inpShoulderW, inpShoulderZ, inpArmUpW, inpArmUpZ, inpElbowW, inpElbowZ, inpArmLow };
			buttons = new Button[7] { btnShoulderW, btnShoulderZ, btnArmUpW, btnArmUpZ, btnElbowW, btnElbowZ, btnArmLow };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsArm += UpdateCustomUI;
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
