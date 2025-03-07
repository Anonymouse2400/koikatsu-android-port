using UnityEngine;
using UnityEngine.UI;

namespace FixCharaEvent
{
	public class ImageButton : MonoBehaviour
	{
		[SerializeField]
		private RawImage _faceImage;

		[SerializeField]
		private Button _button;

		public RawImage faceImage
		{
			get
			{
				return _faceImage;
			}
		}

		public Button button
		{
			get
			{
				return _button;
			}
		}
	}
}
