using System;
using Illusion.Component.UI.ColorPicker;
using Illusion.Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class BackColorCtrl : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("このキャンバスグループ")]
		private CanvasGroup cgWindow;

		[SerializeField]
		[Tooltip("閉じるボタン")]
		private Button btnClose;

		[SerializeField]
		[Tooltip("サンプルカラーScript")]
		private UI_SampleColor sampleColor;

		[SerializeField]
		[Tooltip("PickerのRect")]
		private PickerRectA cmpPickerRect;

		[SerializeField]
		[Tooltip("PickerのSlider")]
		private PickerSliderInput cmpPickerSliderI;

		public bool isOpen
		{
			get
			{
				return (cgWindow.alpha != 0f) ? true : false;
			}
		}

		public void Setup(Color color, Action<Color> _actUpdateColor)
		{
			if (null != sampleColor)
			{
				sampleColor.SetColor(color);
				sampleColor.actUpdateColor = _actUpdateColor;
			}
			if ((bool)cgWindow)
			{
				cgWindow.Enable(true);
			}
			cmpPickerRect.isAlpha.Value = false;
			cmpPickerSliderI.useAlpha.Value = false;
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
			if (null != sampleColor)
			{
				sampleColor.actUpdateColor = null;
			}
			if ((bool)cgWindow)
			{
				cgWindow.Enable(false);
			}
		}

		private void Start()
		{
			if ((bool)btnClose)
			{
				btnClose.OnClickAsObservable().Subscribe(delegate
				{
					Close();
				});
			}
		}
	}
}
