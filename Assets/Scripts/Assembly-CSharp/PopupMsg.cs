using System;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PopupMsg : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup cgrp;

	[SerializeField]
	private TextMeshProUGUI txt;

	private CompositeDisposable disposables = new CompositeDisposable();

	public bool active { get; private set; }

	public void StartMessage(float st, float lt, float et, string msg, int mode)
	{
		if (null == cgrp)
		{
			return;
		}
		IObservable<float> source = (from _ in this.UpdateAsObservable()
			select Time.deltaTime).Scan((float acc, float current) => acc + current);
		IObservable<float> source2 = source.TakeWhile((float t) => t < st);
		IObservable<float> loopStream = source.TakeWhile((float t) => !Input.anyKeyDown && (mode == 1 || t < lt));
		IObservable<float> endStream = source.TakeWhile((float t) => t < et);
		disposables.Clear();
		if ((bool)txt)
		{
			txt.text = msg;
		}
		cgrp.blocksRaycasts = true;
		active = true;
		source2.Subscribe((Action<float>)delegate(float t)
		{
			cgrp.alpha = Mathf.Lerp(0f, 1f, Mathf.InverseLerp(0f, st, t));
		}, (Action)delegate
		{
			cgrp.alpha = 1f;
			loopStream.Subscribe((Action<float>)delegate
			{
			}, (Action)delegate
			{
				endStream.Subscribe((Action<float>)delegate(float t)
				{
					cgrp.alpha = Mathf.Lerp(1f, 0f, Mathf.InverseLerp(0f, et, t));
				}, (Action)delegate
				{
					cgrp.alpha = 0f;
					cgrp.blocksRaycasts = false;
					active = false;
				}).AddTo(disposables);
			}).AddTo(disposables);
		}).AddTo(disposables);
	}

	private void Start()
	{
		cgrp.alpha = 0f;
		cgrp.blocksRaycasts = false;
		active = false;
		this.OnDestroyAsObservable().Subscribe(delegate
		{
			disposables.Clear();
			cgrp.alpha = 0f;
			cgrp.blocksRaycasts = false;
			active = false;
		});
	}
}
