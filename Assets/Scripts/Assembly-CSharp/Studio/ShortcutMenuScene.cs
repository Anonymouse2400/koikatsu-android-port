using Manager;
using UnityEngine;

namespace Studio
{
	public class ShortcutMenuScene : MonoBehaviour
	{
		private float timeScale = 1f;

		private void Awake()
		{
			timeScale = Time.timeScale;
			Time.timeScale = 0f;
		}

		private void Update()
		{
			if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.F2))
			{
				Singleton<Scene>.Instance.UnLoad();
			}
		}

		private void OnDestroy()
		{
			Time.timeScale = timeScale;
		}
	}
}
