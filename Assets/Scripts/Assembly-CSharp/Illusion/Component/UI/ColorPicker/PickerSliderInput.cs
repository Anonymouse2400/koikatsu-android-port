using System.Collections.Generic;
using Localize.Translate;
using TMPro;
using UniRx;
using UnityEngine;

namespace Illusion.Component.UI.ColorPicker
{
	public class PickerSliderInput : PickerSlider
	{
		[Tooltip("RedInputField")]
		[SerializeField]
		private TMP_InputField inputR;

		[Tooltip("GreenInputField")]
		[SerializeField]
		private TMP_InputField inputG;

		[SerializeField]
		[Tooltip("BlueInputField")]
		private TMP_InputField inputB;

		[Tooltip("AlphaInputField")]
		[SerializeField]
		private TMP_InputField inputA;

		[SerializeField]
		[Tooltip("R or H")]
		private TextMeshProUGUI textR;

		[Tooltip("G or S")]
		[SerializeField]
		private TextMeshProUGUI textG;

		[Tooltip("B or V")]
		[SerializeField]
		private TextMeshProUGUI textB;

		public string ConvertTextFromValue(int min, int max, float value)
		{
			return ((int)Mathf.Lerp(min, max, value)/*cast due to .constrained prefix*/).ToString();
		}

		public float ConvertValueFromText(int min, int max, string buf)
		{
			if (buf.IsNullOrEmpty())
			{
				return 0f;
			}
			int result;
			if (!int.TryParse(buf, out result))
			{
				return 0f;
			}
			return Mathf.InverseLerp(min, max, result);
		}

		public void SetInputText()
		{
			float[] array = new float[4] { sliderR.value, sliderG.value, sliderB.value, sliderA.value };
			int num = 0;
			if (base.isHSV)
			{
				inputR.text = ConvertTextFromValue(0, 360, array[num++]);
				inputG.text = ConvertTextFromValue(0, 100, array[num++]);
				inputB.text = ConvertTextFromValue(0, 100, array[num++]);
				inputA.text = ConvertTextFromValue(0, 100, array[num++]);
			}
			else
			{
				inputR.text = ConvertTextFromValue(0, 255, array[num++]);
				inputG.text = ConvertTextFromValue(0, 255, array[num++]);
				inputB.text = ConvertTextFromValue(0, 255, array[num++]);
				inputA.text = ConvertTextFromValue(0, 100, array[num++]);
			}
		}

		protected override void Start()
		{
			base.Start();
			sliderR.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				if (base.isHSV)
				{
					inputR.text = ConvertTextFromValue(0, 360, value);
				}
				else
				{
					inputR.text = ConvertTextFromValue(0, 255, value);
				}
			});
			inputR.onEndEdit.AddListener(delegate(string s)
			{
				if (base.isHSV)
				{
					sliderR.value = ConvertValueFromText(0, 360, s);
				}
				else
				{
					sliderR.value = ConvertValueFromText(0, 255, s);
				}
			});
			sliderG.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				if (base.isHSV)
				{
					inputG.text = ConvertTextFromValue(0, 100, value);
				}
				else
				{
					inputG.text = ConvertTextFromValue(0, 255, value);
				}
			});
			inputG.onEndEdit.AddListener(delegate(string s)
			{
				if (base.isHSV)
				{
					sliderG.value = ConvertValueFromText(0, 100, s);
				}
				else
				{
					sliderG.value = ConvertValueFromText(0, 255, s);
				}
			});
			sliderB.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				if (base.isHSV)
				{
					inputB.text = ConvertTextFromValue(0, 100, value);
				}
				else
				{
					inputB.text = ConvertTextFromValue(0, 255, value);
				}
			});
			inputB.onEndEdit.AddListener(delegate(string s)
			{
				if (base.isHSV)
				{
					sliderB.value = ConvertValueFromText(0, 100, s);
				}
				else
				{
					sliderB.value = ConvertValueFromText(0, 255, s);
				}
			});
			sliderA.onValueChanged.AsObservable().Subscribe(delegate(float value)
			{
				if (base.isHSV)
				{
					inputA.text = ConvertTextFromValue(0, 100, value);
				}
				else
				{
					inputA.text = ConvertTextFromValue(0, 100, value);
				}
			});
			inputA.onEndEdit.AddListener(delegate(string s)
			{
				if (base.isHSV)
				{
					sliderA.value = ConvertValueFromText(0, 100, s);
				}
				else
				{
					sliderA.value = ConvertValueFromText(0, 100, s);
				}
			});
			Dictionary<int, Data.Param>.ValueCollection values = Localize.Translate.Manager.OtherData.Get(1).Values;
			string[] hsvTexts = values.ToArray("HSV");
			string[] rgbTexts = values.ToArray("RGB");
			_isHSV.TakeUntilDestroy(this).Subscribe(delegate(bool isOn)
			{
				float[] array = new float[3] { sliderR.value, sliderG.value, sliderB.value };
				int num = 0;
				if (isOn)
				{
					int num2 = 0;
					if (!hsvTexts.SafeProc(num2++, delegate(string text)
					{
						textR.text = text;
					}))
					{
						textR.text = "色合い";
					}
					if (!hsvTexts.SafeProc(num2++, delegate(string text)
					{
						textG.text = text;
					}))
					{
						textG.text = "鮮やかさ";
					}
					if (!hsvTexts.SafeProc(num2++, delegate(string text)
					{
						textB.text = text;
					}))
					{
						textB.text = "明るさ";
					}
					inputR.text = ConvertTextFromValue(0, 360, array[num++]);
					inputG.text = ConvertTextFromValue(0, 100, array[num++]);
					inputB.text = ConvertTextFromValue(0, 100, array[num++]);
				}
				else
				{
					int num3 = 0;
					if (!rgbTexts.SafeProc(num3++, delegate(string text)
					{
						textR.text = text;
					}))
					{
						textR.text = "赤";
					}
					if (!rgbTexts.SafeProc(num3++, delegate(string text)
					{
						textG.text = text;
					}))
					{
						textG.text = "緑";
					}
					if (!rgbTexts.SafeProc(num3++, delegate(string text)
					{
						textB.text = text;
					}))
					{
						textB.text = "青";
					}
					inputR.text = ConvertTextFromValue(0, 255, array[num++]);
					inputG.text = ConvertTextFromValue(0, 255, array[num++]);
					inputB.text = ConvertTextFromValue(0, 255, array[num++]);
				}
			});
		}
	}
}
