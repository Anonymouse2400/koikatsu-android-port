using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Localize.Translate
{
	public abstract class UITextSetting : MonoBehaviour
	{
		[SerializeField]
		private bool _isOrderOnly;

		[SerializeField]
		private float _fontSize = -1f;

		[SerializeField]
		private float _fontSizeMin = -1f;

		[SerializeField]
		private float _fontSizeMax = -1f;

		public bool isOrderOnly
		{
			get
			{
				return _isOrderOnly;
			}
		}

		public float fontSize
		{
			get
			{
				return _fontSize;
			}
		}

		public float fontSizeMin
		{
			get
			{
				return _fontSizeMin;
			}
		}

		public float fontSizeMax
		{
			get
			{
				return _fontSizeMax;
			}
		}

		public virtual bool Set(Text text)
		{
			return false;
		}

		public virtual bool Set(TMP_Text text)
		{
			return false;
		}
	}
}
