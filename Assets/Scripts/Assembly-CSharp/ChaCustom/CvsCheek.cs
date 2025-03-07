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
	public class CvsCheek : MonoBehaviour
	{
		[SerializeField]
		private Slider sldCheekBoneW;

		[SerializeField]
		private TMP_InputField inpCheekBoneW;

		[SerializeField]
		private Button btnCheekBoneW;

		[SerializeField]
		private Slider sldCheekBoneZ;

		[SerializeField]
		private TMP_InputField inpCheekBoneZ;

		[SerializeField]
		private Button btnCheekBoneZ;

		[SerializeField]
		private Slider sldCheekW;

		[SerializeField]
		private TMP_InputField inpCheekW;

		[SerializeField]
		private Button btnCheekW;

		[SerializeField]
		private Slider sldCheekZ;

		[SerializeField]
		private TMP_InputField inpCheekZ;

		[SerializeField]
		private Button btnCheekZ;

		[SerializeField]
		private Slider sldCheekY;

		[SerializeField]
		private TMP_InputField inpCheekY;

		[SerializeField]
		private Button btnCheekY;

		[SerializeField]
		private Slider sldGlossPow;

		[SerializeField]
		private TMP_InputField inpGlossPow;

		[SerializeField]
		private Button btnGlossPow;

		private Slider[] sliders;

		private TMP_InputField[] inputs;

		private Button[] buttons;

		private readonly int[] arrIndex = new int[5] { 14, 15, 16, 17, 18 };

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
			sldGlossPow.value = face.cheekGlossPower;
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
			inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, face.cheekGlossPower);
		}

		protected virtual void Awake()
		{
			sliders = new Slider[5] { sldCheekBoneW, sldCheekBoneZ, sldCheekW, sldCheekZ, sldCheekY };
			inputs = new TMP_InputField[5] { inpCheekBoneW, inpCheekBoneZ, inpCheekW, inpCheekZ, inpCheekY };
			buttons = new Button[5] { btnCheekBoneW, btnCheekBoneZ, btnCheekW, btnCheekZ, btnCheekY };
		}

		protected virtual void Start()
		{
			TMP_InputField[] array = inputs;
			foreach (TMP_InputField item in array)
			{
				Singleton<CustomBase>.Instance.lstTmpInputField.Add(item);
			}
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(inpGlossPow);
			Singleton<CustomBase>.Instance.actUpdateCvsCheek += UpdateCustomUI;
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
			sldGlossPow.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				face.cheekGlossPower = value;
				chaCtrl.ChangeSettingCheekGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, value);
			});
			sldGlossPow.OnPointerUpAsObservable().Subscribe(delegate
			{
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingCheekGlossPower);
			});
			sldGlossPow.OnScrollAsObservable().Subscribe(delegate(PointerEventData scl)
			{
				sldGlossPow.value = Mathf.Clamp(sldGlossPow.value + scl.scrollDelta.y * Singleton<CustomBase>.Instance.sliderWheelSensitive, 0f, 100f);
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, sldGlossPow.value);
			});
			inpGlossPow.onEndEdit.AsObservable().Subscribe(delegate(string value)
			{
				sldGlossPow.value = CustomBase.ConvertRateFromText(0, 100, value);
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingCheekGlossPower);
			});
			btnGlossPow.onClick.AsObservable().Subscribe(delegate
			{
				float cheekGlossPower = Singleton<CustomBase>.Instance.defChaInfo.custom.face.cheekGlossPower;
				face.cheekGlossPower = cheekGlossPower;
				chaCtrl.ChangeSettingCheekGlossPower();
				inpGlossPow.text = CustomBase.ConvertTextFromRate(0, 100, cheekGlossPower);
				sldGlossPow.value = cheekGlossPower;
				Singleton<CustomHistory>.Instance.Add1(chaCtrl, chaCtrl.ChangeSettingCheekGlossPower);
			});
			StartCoroutine(SetInputText());
		}
	}
}
