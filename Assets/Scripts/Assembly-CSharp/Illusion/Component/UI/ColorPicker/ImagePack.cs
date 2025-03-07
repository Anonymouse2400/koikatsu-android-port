using UnityEngine;
using UnityEngine.UI;

namespace Illusion.Component.UI.ColorPicker
{
	internal class ImagePack
	{
		public RectTransform rectTransform { get; private set; }

		public Texture2D tex2D { get; private set; }

		public Vector2 size
		{
			get
			{
				return new Vector2(tex2D.width, tex2D.height);
			}
		}

		public bool isTex
		{
			get
			{
				return tex2D != null;
			}
		}

		public ImagePack(Image image)
		{
			rectTransform = image.rectTransform;
			tex2D = new Texture2D((int)rectTransform.rect.width, (int)rectTransform.rect.height);
			tex2D.Apply();
			image.sprite = Sprite.Create(tex2D, new Rect(0f, 0f, tex2D.width, tex2D.height), Vector2.zero);
		}

		public void SetPixels(Color[] colors)
		{
			tex2D.SetPixels(colors);
			tex2D.Apply();
		}
	}
}
