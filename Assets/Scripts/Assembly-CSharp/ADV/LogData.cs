using UnityEngine;
using UnityEngine.UI;

namespace ADV
{
	public class LogData : MonoBehaviour
	{
		[SerializeField]
		private Text _name;

		[SerializeField]
		private Text _message;

		[SerializeField]
		private Button _voiceButton;

		public Text Name
		{
			get
			{
				return _name;
			}
		}

		public Text Message
		{
			get
			{
				return _message;
			}
		}

		public Button VoiceButton
		{
			get
			{
				return _voiceButton;
			}
		}
	}
}
