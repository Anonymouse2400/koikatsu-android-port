using Localize.Translate;
using UnityEngine;
using UnityEngine.UI;

namespace H
{
	public class SpriteChangeCtrl : MonoBehaviour
	{
		public Sprite[] sprites;

		private Image _image;

		public Image image
		{
			get
			{
				return _image;
			}
		}

		private void Awake()
		{
			_image = GetComponent<Image>();
		}

		public void OnChangeValue(int _num)
		{
			if (!(_image == null) && sprites.Length > _num)
			{
				if (_num < 0)
				{
					_image.enabled = false;
				}
				else
				{
					_image.enabled = true;
					_image.sprite = sprites[_num];
				}
				IHSpriteChangable component = GetComponent<IHSpriteChangable>();
				if (component != null)
				{
					component.OnChange(_num);
				}
			}
		}

		public int GetCount()
		{
			return sprites.Length;
		}

		public int GetVisibleNumber()
		{
			if (_image == null)
			{
				return 0;
			}
			for (int i = 0; i < sprites.Length; i++)
			{
				if (_image.sprite == sprites[i])
				{
					return i;
				}
			}
			return 0;
		}
	}
}
