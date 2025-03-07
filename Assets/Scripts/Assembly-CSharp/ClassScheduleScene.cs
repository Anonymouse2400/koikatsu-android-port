using System;
using System.Collections;
using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ClassScheduleScene : BaseLoader
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Button returnButton;

	private IEnumerator Start()
	{
		base.enabled = false;
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			this.ObserveEveryValueChanged((ClassScheduleScene _) => !Singleton<Scene>.Instance.IsNowLoadingFade).Subscribe(delegate(bool isOn)
			{
				canvasGroup.interactable = isOn;
			});
		}
		Action returnProc = delegate
		{
			Observable.NextFrame().Subscribe(delegate
			{
				Singleton<Scene>.Instance.UnLoad();
			});
		};
		returnButton.OnClickAsObservable().Subscribe(delegate
		{
			Utils.Sound.Play(SystemSE.cancel);
			returnProc();
		});
		base.enabled = true;
		yield break;
	}
}
