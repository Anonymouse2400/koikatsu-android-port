using UnityEngine;

namespace Illusion.Component.UI.ColorPicker
{
	public class SampleImageSlider : SampleImage
	{
		[SerializeField]
		private PickerSlider slider;

		private void Start()
		{
			slider.color = image.color;
			slider.updateColorAction += delegate(Color color)
			{
				image.color = color;
			};
		}
	}
}
