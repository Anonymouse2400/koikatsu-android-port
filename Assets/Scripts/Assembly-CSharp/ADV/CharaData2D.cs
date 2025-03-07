using UnityEngine;
using UnityEngine.UI;

namespace ADV
{
	public class CharaData2D
	{
		public Image image { get; private set; }

		public RectTransform rectTransform { get; private set; }

		public CharaData2D(Image image)
		{
			this.image = image;
			rectTransform = image.rectTransform;
		}

		public void Release()
		{
			if (image != null)
			{
				Object.Destroy(image.gameObject);
			}
			image = null;
		}
	}
}
