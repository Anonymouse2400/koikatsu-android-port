using System;
using Illusion.Component.UI.ColorPicker;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class ColorPalette : Singleton<ColorPalette>
	{
		[SerializeField]
		private Canvas canvas;

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
		private SampleColor sampleColor;

		[SerializeField]
		[Tooltip("PickerのRect")]
		private PickerRectA cmpPickerRect;

		[SerializeField]
		[Tooltip("PickerのSlider")]
		private PickerSliderInput cmpPickerSliderI;

		private BoolReactiveProperty _visible = new BoolReactiveProperty(false);

		private bool _outsideVisible = true;

		public bool isOpen
		{
			get
			{
				return (cgWindow.alpha != 0f) ? true : false;
			}
		}

		public bool visible
		{
			get
			{
				return _visible.Value;
			}
			set
			{
				_visible.Value = value;
			}
		}

		public bool outsideVisible
		{
			set
			{
				_outsideVisible = value;
				if ((bool)cgWindow)
				{
					cgWindow.Enable(_visible.Value && _outsideVisible);
				}
			}
		}

		public void Setup(string winTitle, Color color, Action<Color> _actUpdateColor, bool _useAlpha)
		{
			if ((bool)textWinTitle && !winTitle.IsNullOrEmpty())
			{
				textWinTitle.text = winTitle;
			}
			if (null != sampleColor)
			{
				sampleColor.SetColor(color);
				sampleColor.actUpdateColor = _actUpdateColor;
			}
			visible = true;
			cmpPickerRect.isAlpha.Value = _useAlpha;
			cmpPickerSliderI.useAlpha.Value = _useAlpha;
		}

		public void Close()
		{
			if ((bool)textWinTitle)
			{
				textWinTitle.text = string.Empty;
			}
			if (null != sampleColor)
			{
				sampleColor.actUpdateColor = null;
			}
			if ((bool)cgWindow)
			{
				cgWindow.Enable(false);
			}
		}

		public bool Check(string _text)
		{
			return !_text.IsNullOrEmpty() && textWinTitle.text == _text;
		}

		protected override void Awake()
		{
			_visible.Subscribe(delegate(bool b)
			{
				if ((bool)cgWindow)
				{
					cgWindow.Enable(b && _outsideVisible);
				}
				if (!b)
				{
					Close();
				}
				if (isOpen)
				{
					SortCanvas.select = canvas;
				}
			});
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					Close();
				});
			}
			Close();
		}
	}
}
