using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class ExitScene : MonoBehaviour
{
	[SerializeField]
	[Label("Yesボタン")]
	private Button Yes;

	[SerializeField]
	[Label("Noボタン")]
	private Button No;

	[Label("キャンバス")]
	[SerializeField]
	private Canvas canvas;

	private float timeScale = 1f;

	private void Awake()
	{
		timeScale = Time.timeScale;
		Time.timeScale = 0f;
	}

	private void Start()
	{
		Canvas[] source = (from p in Object.FindObjectsOfType<Canvas>()
			where p != canvas
			select p).ToArray();
		if (source.Any())
		{
			canvas.sortingOrder = source.Max((Canvas p) => p.sortingOrder) + 1;
		}
		Yes.OnClickAsObservable().Take(1).Subscribe(delegate
		{
			Singleton<Scene>.Instance.GameEnd(false);
			Singleton<Scene>.Instance.isSkipGameExit = false;
		});
		No.OnClickAsObservable().Take(1).Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.sel);
			Singleton<Scene>.Instance.isGameEndCheck = true;
			Singleton<Scene>.Instance.isSkipGameExit = false;
			Observable.NextFrame().Subscribe(delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			});
		});
		(from _ in this.UpdateAsObservable()
			where Input.GetMouseButtonDown(1)
			select _).Take(1).Subscribe(delegate
		{
			No.onClick.Invoke();
		});
	}

	private void OnDestroy()
	{
		Time.timeScale = timeScale;
	}
}
