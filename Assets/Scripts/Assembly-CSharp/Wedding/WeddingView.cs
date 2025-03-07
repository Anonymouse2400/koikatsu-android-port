using ChaCustom;
using UnityEngine;
using UnityEngine.UI;

namespace Wedding
{
	public class WeddingView : MonoBehaviour
	{
		[SerializeField]
		private Canvas _canvas;

		[SerializeField]
		private Button _btEnter;

		[SerializeField]
		private Button _btReturn;

		[SerializeField]
		private CameraControl_Ver2 _camCtrl;

		[SerializeField]
		private clothesFileControl _clothCtrl;

		public Canvas canvas
		{
			get
			{
				return _canvas;
			}
		}

		public Button btEnter
		{
			get
			{
				return _btEnter;
			}
		}

		public Button btReturn
		{
			get
			{
				return _btReturn;
			}
		}

		public CameraControl_Ver2 camCtrl
		{
			get
			{
				return _camCtrl;
			}
		}

		public clothesFileControl clothCtrl
		{
			get
			{
				return _clothCtrl;
			}
		}
	}
}
