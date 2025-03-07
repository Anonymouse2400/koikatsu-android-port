using System;
using Illusion.Component.UI.ColorPicker;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class SampleColor : MonoBehaviour
	{
		[SerializeField]
		private Image image;

		[SerializeField]
		private PickerRect rect;

		[SerializeField]
		private PickerSliderInput slider;

		[SerializeField]
		private ColorPresets preset;

		private bool callUpdate;

		public Action<Color> actUpdateColor;

		private void Start()
		{
			if (!image)
			{
				return;
			}
			if ((bool)rect)
			{
				rect.SetColor(image.color);
				rect.updateColorAction += delegate(Color color)
				{
					UpdateRectColor(color);
				};
			}
			if ((bool)slider)
			{
				slider.color = image.color;
				slider.SetInputText();
				slider.updateColorAction += delegate(Color color)
				{
					UpdateSliderColor(color);
				};
			}
			if ((bool)preset)
			{
				preset.color = image.color;
				preset.updateColorAction += delegate(Color color)
				{
					UpdatePresetsColor(color);
				};
			}
		}

		public void SetColor(Color color)
		{
			callUpdate = true;
			image.color = color;
			if ((bool)rect)
			{
				rect.SetColor(color);
			}
			if ((bool)slider)
			{
				slider.color = color;
			}
			if ((bool)preset)
			{
				preset.color = color;
			}
			callUpdate = false;
		}

		public void UpdateRectColor(Color color)
		{
			if (!callUpdate)
			{
				callUpdate = true;
				image.color = color;
				if ((bool)slider)
				{
					slider.color = color;
				}
				if ((bool)preset)
				{
					preset.color = color;
				}
				actUpdateColor.Call(color);
				callUpdate = false;
			}
		}

		public void UpdateSliderColor(Color color)
		{
			if (!callUpdate)
			{
				callUpdate = true;
				image.color = color;
				if ((bool)rect)
				{
					rect.SetColor(color);
				}
				if ((bool)preset)
				{
					preset.color = color;
				}
				actUpdateColor.Call(color);
				callUpdate = false;
			}
		}

		public void UpdatePresetsColor(Color color)
		{
			if (!callUpdate)
			{
				callUpdate = true;
				image.color = color;
				if ((bool)rect)
				{
					rect.SetColor(color);
				}
				if ((bool)slider)
				{
					slider.color = color;
				}
				actUpdateColor.Call(color);
				callUpdate = false;
			}
		}
	}
}
