using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Studio
{
	public class CheckScene : MonoBehaviour
	{
		[SerializeField]
		private Image back;

		[SerializeField]
		private VoiceNode buttonYes;

		[SerializeField]
		private VoiceNode buttonNo;

		private float timeScale = 1f;

		public static Sprite sprite;

		public static UnityAction unityActionYes;

		public static UnityAction unityActionNo;

		private void Awake()
		{
			timeScale = Time.timeScale;
			Time.timeScale = 0f;
		}

		private void Start()
		{
			back.sprite = sprite;
			buttonYes.addOnClick = unityActionYes;
			buttonNo.addOnClick = unityActionNo;
		}

		private void OnDestroy()
		{
			Time.timeScale = timeScale;
		}
	}
}
