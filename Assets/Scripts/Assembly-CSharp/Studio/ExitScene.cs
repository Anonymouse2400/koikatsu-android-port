using Manager;
using SceneAssist;
using UnityEngine;

namespace Studio
{
	public class ExitScene : MonoBehaviour
	{
		[SerializeField]
		private VoiceNode yes;

		[SerializeField]
		private VoiceNode no;

		private float timeScale = 1f;

		private void Awake()
		{
			timeScale = Time.timeScale;
			Time.timeScale = 0f;
		}

		private void Start()
		{
			yes.addOnClick = delegate
			{
				Singleton<Scene>.Instance.GameEnd(false);
				Singleton<Scene>.Instance.isSkipGameExit = false;
			};
			yes.addOnClick = delegate
			{
				Assist.PlayDecisionSE();
			};
			no.addOnClick = delegate
			{
				Singleton<Scene>.Instance.UnLoad();
				Singleton<Scene>.Instance.isGameEndCheck = true;
				Singleton<Scene>.Instance.isSkipGameExit = false;
			};
			no.addOnClick = delegate
			{
				Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, Assist.AssetBundleSystemSE, "sse_00_03");
			};
		}

		private void OnDestroy()
		{
			Time.timeScale = timeScale;
		}
	}
}
