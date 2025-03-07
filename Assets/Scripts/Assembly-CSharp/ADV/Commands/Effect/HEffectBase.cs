using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ADV.Commands.Effect
{
	public abstract class HEffectBase : CommandBase
	{
		private bool isFront = true;

		private IDisposable dis;

		private Image filter;

		private bool isEnd;

		private ADVFade advFade;

		private float timer;

		private float time = 2f;

		private readonly Color initColor = new Color(1f, 1f, 1f, 0f);

		private Color sColor;

		private Color eColor;

		public override string[] ArgsLabel
		{
			get
			{
				return null;
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			sColor = initColor;
			eColor = initColor;
			advFade = base.scenario.advScene.AdvFade;
			advFade.enabled = false;
			advFade.SetColor(isFront, initColor);
			if (isFront)
			{
				filter = advFade.FilterFront;
			}
			else
			{
				filter = advFade.FilterBack;
			}
			dis = Observable.FromCoroutine(FadeLoop).Subscribe(delegate
			{
				isEnd = true;
			});
		}

		public override bool Process()
		{
			base.Process();
			return isEnd;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			dis.Dispose();
			dis = null;
			advFade.SetColor(isFront, initColor);
			isEnd = true;
		}

		private void FadeInit(bool isFadeIn, float t)
		{
			timer = 0f;
			time = t;
			sColor = filter.color;
			if (isFadeIn)
			{
				sColor.a = 0f;
				eColor = Color.white;
			}
			else
			{
				eColor = initColor;
			}
		}

		private bool FadeProc()
		{
			if (isEnd)
			{
				return false;
			}
			timer = Mathf.Min(timer + Time.deltaTime, time);
			filter.color = Color.Lerp(sColor, eColor, Mathf.InverseLerp(0f, time, timer));
			return timer < time;
		}

		protected IEnumerator InEffect(float t, CancellationToken cancel)
		{
			FadeInit(true, t);
			yield return Observable.FromCoroutine((CancellationToken _) => new WaitWhile(FadeProc)).StartAsCoroutine(cancel);
		}

		protected IEnumerator OutEffect(float t, CancellationToken cancel)
		{
			FadeInit(false, t);
			yield return Observable.FromCoroutine((CancellationToken _) => new WaitWhile(FadeProc)).StartAsCoroutine(cancel);
		}

		protected abstract IEnumerator FadeLoop(CancellationToken cancel);
	}
}
