using System.Collections;
using Manager;
using UnityEngine;

namespace Studio
{
	public class StartScene : MonoBehaviour
	{
		private IEnumerator LoadCoroutine()
		{
			yield return Singleton<Info>.Instance.LoadExcelDataCoroutine();
			Singleton<Scene>.Instance.SetFadeColor(Color.black);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Studio",
				isFade = true
			}, false);
		}

		private void Start()
		{
			StartCoroutine(LoadCoroutine());
		}
	}
}
