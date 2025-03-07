using UnityEngine;

namespace Illusion.Component.UI.ColorPicker
{
	public class SampleImageRect : SampleImage
	{
		[SerializeField]
		private PickerRect rect;

		private void Start()
		{
			rect.SetColor(image.color);
			rect.updateColorAction += delegate(Color color)
			{
				image.color = color;
			};
		}
	}
}
