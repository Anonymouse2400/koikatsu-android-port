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
	public class CvsNose : MonoBehaviour
	{
		[SerializeField]
		private Slider sldNoseTipH;

		[SerializeField]
		private TMP_InputField inpNoseTipH;

		[SerializeField]
		private Button btnNoseTipH;

		[SerializeField]
		private Slider sldNoseY;

		[SerializeField]
		private TMP_InputField inpNoseY;

		[SerializeField]
		private Button btnNoseY;

		[SerializeField]
		private Slider sldNoseBridgeH;

		[SerializeField]
		private TMP_InputField inpNoseBridgeH;

		[SerializeField]
		private Button btnNoseBridgeH;

		[SerializeField]
		private Toggle tglNoseKind;

		[SerializeField]
		private Image imgNoseKind;

		[SerializeField]
		private TextMeshProUGUI textNoseKind;

		[SerializeField]
		private CustomSelectKind customNose;

		[SerializeField]
		private CanvasGroup cgNoseWin;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[3] { 38, 39, 40 };

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
			if (null != customNose)
			{
				customNose.UpdateCustomUI();
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
		}

		public void UpdateSelectNoseKind(string name, Sprite sp, int index)
		{
			if ((bool)textNoseKind)
			{
				textNoseKind.text = name;
			}
			if ((bool)imgNoseKind)
			{
				imgNoseKind.sprite = sp;
			}
			if (face.noseId != index)
			{
				face.noseId = index;
				chaCtrl.ChangeSettingNose();
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingNose);
			}
		}

		protected virtual void Awake()
		{
			sliders = new Slider[3] { sldNoseTipH, sldNoseY, sldNoseBridgeH };
			inputs = new TMP_InputField[3] { inpNoseTipH, inpNoseY, inpNoseBridgeH };
			buttons = new Button[3] { btnNoseTipH, btnNoseY, btnNoseBridgeH };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.actUpdateCvsNose += UpdateCustomUI;
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
			tglNoseKind.OnValueChangedAsObservable().Subscribe(delegate(bool isOn)
			{
				if ((bool)cgNoseWin)
				{
					bool flag = ((cgNoseWin.alpha != 0f) ? true : false);
					if (flag != isOn)
					{
						cgNoseWin.Enable(isOn);
					}
				}
			});
			StartCoroutine(SetInputText());
		}
	}
}
