using System;
using Illusion.Component.UI.ColorPicker;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsColor : Singleton<CvsColor>
	{
		public enum ConnectColorKind
		{
			None = 0,
			Eyebrow = 1,
			Eyeline = 2,
			EyeW01 = 3,
			EyeW02 = 4,
			EyeHLUp = 5,
			EyeHLDown = 6,
			Pupil01 = 7,
			Pupil02 = 8,
			Lipline = 9,
			Mole = 10,
			BaseEyeShadow = 11,
			BaseCheek = 12,
			BaseLip = 13,
			BaseFacePaint01 = 14,
			BaseFacePaint02 = 15,
			SkinMain = 16,
			SkinSub = 17,
			Nip = 18,
			Nail = 19,
			Underhair = 20,
			Sunburn = 21,
			BodyPaint01 = 22,
			BodyPaint02 = 23,
			HairF_Base = 24,
			HairF_Start = 25,
			HairF_End = 26,
			HairF_Acs01 = 27,
			HairF_Acs02 = 28,
			HairF_Acs03 = 29,
			HairF_Outline = 30,
			HairB_Base = 31,
			HairB_Start = 32,
			HairB_End = 33,
			HairB_Acs01 = 34,
			HairB_Acs02 = 35,
			HairB_Acs03 = 36,
			HairB_Outline = 37,
			HairS_Base = 38,
			HairS_Start = 39,
			HairS_End = 40,
			HairS_Acs01 = 41,
			HairS_Acs02 = 42,
			HairS_Acs03 = 43,
			HairS_Outline = 44,
			HairO_Base = 45,
			HairO_Start = 46,
			HairO_End = 47,
			HairO_Acs01 = 48,
			HairO_Acs02 = 49,
			HairO_Acs03 = 50,
			HairO_Outline = 51,
			CosTop01 = 52,
			CosTop02 = 53,
			CosTop03 = 54,
			CosTop04 = 55,
			CosTopPtn01 = 56,
			CosTopPtn02 = 57,
			CosTopPtn03 = 58,
			CosTopPtn04 = 59,
			CosBot01 = 60,
			CosBot02 = 61,
			CosBot03 = 62,
			CosBot04 = 63,
			CosBotPtn01 = 64,
			CosBotPtn02 = 65,
			CosBotPtn03 = 66,
			CosBotPtn04 = 67,
			CosBra01 = 68,
			CosBra02 = 69,
			CosBra03 = 70,
			CosBra04 = 71,
			CosBraPtn01 = 72,
			CosBraPtn02 = 73,
			CosBraPtn03 = 74,
			CosBraPtn04 = 75,
			CosShorts01 = 76,
			CosShorts02 = 77,
			CosShorts03 = 78,
			CosShorts04 = 79,
			CosShortsPtn01 = 80,
			CosShortsPtn02 = 81,
			CosShortsPtn03 = 82,
			CosShortsPtn04 = 83,
			CosGloves01 = 84,
			CosGloves02 = 85,
			CosGloves03 = 86,
			CosGloves04 = 87,
			CosGlovesPtn01 = 88,
			CosGlovesPtn02 = 89,
			CosGlovesPtn03 = 90,
			CosGlovesPtn04 = 91,
			CosPanst01 = 92,
			CosPanst02 = 93,
			CosPanst03 = 94,
			CosPanst04 = 95,
			CosPanstPtn01 = 96,
			CosPanstPtn02 = 97,
			CosPanstPtn03 = 98,
			CosPanstPtn04 = 99,
			CosSocks01 = 100,
			CosSocks02 = 101,
			CosSocks03 = 102,
			CosSocks04 = 103,
			CosSocksPtn01 = 104,
			CosSocksPtn02 = 105,
			CosSocksPtn03 = 106,
			CosSocksPtn04 = 107,
			CosInnerShoes01 = 108,
			CosInnerShoes02 = 109,
			CosInnerShoes03 = 110,
			CosInnerShoes04 = 111,
			CosInnerShoesPtn01 = 112,
			CosInnerShoesPtn02 = 113,
			CosInnerShoesPtn03 = 114,
			CosInnerShoesPtn04 = 115,
			CosOuterShoes01 = 116,
			CosOuterShoes02 = 117,
			CosOuterShoes03 = 118,
			CosOuterShoes04 = 119,
			CosOuterShoesPtn01 = 120,
			CosOuterShoesPtn02 = 121,
			CosOuterShoesPtn03 = 122,
			CosOuterShoesPtn04 = 123,
			AcsSlot01_01 = 124,
			AcsSlot01_02 = 125,
			AcsSlot01_03 = 126,
			AcsSlot01_04 = 127,
			AcsSlot02_01 = 128,
			AcsSlot02_02 = 129,
			AcsSlot02_03 = 130,
			AcsSlot02_04 = 131,
			AcsSlot03_01 = 132,
			AcsSlot03_02 = 133,
			AcsSlot03_03 = 134,
			AcsSlot03_04 = 135,
			AcsSlot04_01 = 136,
			AcsSlot04_02 = 137,
			AcsSlot04_03 = 138,
			AcsSlot04_04 = 139,
			AcsSlot05_01 = 140,
			AcsSlot05_02 = 141,
			AcsSlot05_03 = 142,
			AcsSlot05_04 = 143,
			AcsSlot06_01 = 144,
			AcsSlot06_02 = 145,
			AcsSlot06_03 = 146,
			AcsSlot06_04 = 147,
			AcsSlot07_01 = 148,
			AcsSlot07_02 = 149,
			AcsSlot07_03 = 150,
			AcsSlot07_04 = 151,
			AcsSlot08_01 = 152,
			AcsSlot08_02 = 153,
			AcsSlot08_03 = 154,
			AcsSlot08_04 = 155,
			AcsSlot09_01 = 156,
			AcsSlot09_02 = 157,
			AcsSlot09_03 = 158,
			AcsSlot09_04 = 159,
			AcsSlot10_01 = 160,
			AcsSlot10_02 = 161,
			AcsSlot10_03 = 162,
			AcsSlot10_04 = 163,
			AcsSlot11_01 = 164,
			AcsSlot11_02 = 165,
			AcsSlot11_03 = 166,
			AcsSlot11_04 = 167,
			AcsSlot12_01 = 168,
			AcsSlot12_02 = 169,
			AcsSlot12_03 = 170,
			AcsSlot12_04 = 171,
			AcsSlot13_01 = 172,
			AcsSlot13_02 = 173,
			AcsSlot13_03 = 174,
			AcsSlot13_04 = 175,
			AcsSlot14_01 = 176,
			AcsSlot14_02 = 177,
			AcsSlot14_03 = 178,
			AcsSlot14_04 = 179,
			AcsSlot15_01 = 180,
			AcsSlot15_02 = 181,
			AcsSlot15_03 = 182,
			AcsSlot15_04 = 183,
			AcsSlot16_01 = 184,
			AcsSlot16_02 = 185,
			AcsSlot16_03 = 186,
			AcsSlot16_04 = 187,
			AcsSlot17_01 = 188,
			AcsSlot17_02 = 189,
			AcsSlot17_03 = 190,
			AcsSlot17_04 = 191,
			AcsSlot18_01 = 192,
			AcsSlot18_02 = 193,
			AcsSlot18_03 = 194,
			AcsSlot18_04 = 195,
			AcsSlot19_01 = 196,
			AcsSlot19_02 = 197,
			AcsSlot19_03 = 198,
			AcsSlot19_04 = 199,
			AcsSlot20_01 = 200,
			AcsSlot20_02 = 201,
			AcsSlot20_03 = 202,
			AcsSlot20_04 = 203
		}

		[SerializeField]
		[Tooltip("このキャンバスグループ")]
		private CanvasGroup cgWindow;

		[SerializeField]
		[Tooltip("ウィンドウタイトル")]
		private TextMeshProUGUI textWinTitle;

		[Tooltip("閉じるボタン")]
		[SerializeField]
		private Button btnClose;

		[SerializeField]
		[Tooltip("サンプルカラーScript")]
		private UI_SampleColor sampleColor;

		[SerializeField]
		[Tooltip("PickerのRect")]
		private PickerRectA cmpPickerRect;

		[Tooltip("PickerのSlider")]
		[SerializeField]
		private PickerSliderInput cmpPickerSliderI;

		[SerializeField]
		[Tooltip("PickerのPreset")]
		private UI_ColorPresets cmpPickerPreset;

		[Tooltip("PickerのSlider")]
		[SerializeField]
		private Slider pickerSlider;

		[Tooltip("PickerのSliderA")]
		[SerializeField]
		private Slider pickerSliderA;

		[SerializeField]
		[Tooltip("SliderのSlider01")]
		private Slider sliderSlider01;

		[Tooltip("SliderのSlider02")]
		[SerializeField]
		private Slider sliderSlider02;

		[Tooltip("SliderのSlider03")]
		[SerializeField]
		private Slider sliderSlider03;

		[Tooltip("SliderのSlider04")]
		[SerializeField]
		private Slider sliderSlider04;

		[SerializeField]
		[Tooltip("SliderのInput01")]
		private TMP_InputField sliderInput01;

		[SerializeField]
		[Tooltip("SliderのInput02")]
		private TMP_InputField sliderInput02;

		[SerializeField]
		[Tooltip("SliderのInput03")]
		private TMP_InputField sliderInput03;

		[SerializeField]
		[Tooltip("SliderのInput04")]
		private TMP_InputField sliderInput04;

		private bool useAlpha;

		public ConnectColorKind connectColorKind { get; private set; }

		public bool isOpen
		{
			get
			{
				return (cgWindow.alpha != 0f) ? true : false;
			}
		}

		public event Action actUpdateHistory;

		public void Setup(string winTitle, ConnectColorKind kind, Color color, Action<Color> _actUpdateColor, Action _actUpdateHistory, bool _useAlpha)
		{
			connectColorKind = kind;
			if ((bool)textWinTitle)
			{
				textWinTitle.text = winTitle;
			}
			if (null != sampleColor)
			{
				sampleColor.SetColor(color);
				sampleColor.actUpdateColor = _actUpdateColor;
			}
			this.actUpdateHistory = _actUpdateHistory;
			if ((bool)cgWindow)
			{
				cgWindow.Enable(true);
			}
			useAlpha = _useAlpha;
			cmpPickerRect.isAlpha.Value = _useAlpha;
			cmpPickerSliderI.useAlpha.Value = _useAlpha;
		}

		public void SetColor(Color color)
		{
			if (null != sampleColor)
			{
				sampleColor.SetColor(color);
			}
		}

		public void Close()
		{
			connectColorKind = ConnectColorKind.None;
			if ((bool)textWinTitle)
			{
				textWinTitle.text = string.Empty;
			}
			if (null != sampleColor)
			{
				sampleColor.actUpdateColor = null;
			}
			this.actUpdateHistory = null;
			if ((bool)cgWindow)
			{
				cgWindow.Enable(false);
			}
		}

		public bool CheckConnectKind(ConnectColorKind kind)
		{
			return connectColorKind == kind;
		}

		public void UpdateHistoryWithClickPreset()
		{
			this.actUpdateHistory.Call();
		}

		protected override void Awake()
		{
			connectColorKind = ConnectColorKind.None;
		}

		private void Start()
		{
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(sliderInput01);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(sliderInput02);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(sliderInput03);
			Singleton<CustomBase>.Instance.lstTmpInputField.Add(sliderInput04);
			cmpPickerPreset.clickAction += UpdateHistoryWithClickPreset;
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					Close();
				});
			}
			if ((bool)cmpPickerRect)
			{
				(from x in cmpPickerRect.ObserveEveryValueChanged((PickerRectA x) => x.info.isOn).Skip(1)
					where !x
					select x).Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)pickerSlider)
			{
				pickerSlider.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderSlider01)
			{
				sliderSlider01.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderSlider02)
			{
				sliderSlider02.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderSlider03)
			{
				sliderSlider03.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderInput01)
			{
				sliderInput01.onEndEdit.AsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderInput02)
			{
				sliderInput02.onEndEdit.AsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderInput03)
			{
				sliderInput03.onEndEdit.AsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if (!useAlpha)
			{
				return;
			}
			if ((bool)pickerSliderA)
			{
				pickerSliderA.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderSlider04)
			{
				sliderSlider04.OnPointerUpAsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
			if ((bool)sliderInput04)
			{
				sliderInput04.onEndEdit.AsObservable().Subscribe(delegate
				{
					this.actUpdateHistory.Call();
				});
			}
		}

		private void Update()
		{
		}
	}
}
