using System;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.UI;

public class LiveFadeSprite : MonoBehaviour
{
	public enum FadeKind
	{
		Out = 0,
		In = 1,
		OutIn = 2
	}

	public enum FadeKindProc
	{
		None = 0,
		Out = 1,
		OutEnd = 2,
		In = 3,
		InEnd = 4,
		OutIn = 5,
		OutInEnd = 6
	}

	[Header("フェード")]
	[Label("フェード用RawImage")]
	public RawImage fade;

	[Label("基本フェード時間")]
	public float timeFadeBase;

	[Label("フェード処理をしてるか")]
	public bool isFade;

	[Label("フェードアニメーション")]
	public AnimationCurve fadeAnimation;

	private FadeKind kindFade;

	private FadeKindProc kindFadeProc;

	private float timeFade;

	private float timeFadeTime;

	private void Update()
	{
		FadeProc();
	}

	public void FadeState(FadeKind _kind, float _timeFade = -1f)
	{
		isFade = true;
		timeFadeTime = 0f;
		fade.enabled = true;
		if (_timeFade < 0f)
		{
			timeFade = ((_kind == FadeKind.OutIn) ? (timeFadeBase * 2f) : timeFadeBase);
		}
		else
		{
			timeFade = ((_kind == FadeKind.OutIn) ? (_timeFade * 2f) : _timeFade);
		}
		kindFade = _kind;
		switch (kindFade)
		{
		case FadeKind.Out:
			kindFadeProc = FadeKindProc.Out;
			break;
		case FadeKind.In:
			kindFadeProc = FadeKindProc.In;
			break;
		case FadeKind.OutIn:
			kindFadeProc = FadeKindProc.OutIn;
			break;
		}
	}

	public FadeKindProc GetFadeKindProc()
	{
		return kindFadeProc;
	}

	private bool FadeProc()
	{
		if (!fade)
		{
			return false;
		}
		if (!isFade)
		{
			return false;
		}
		timeFadeTime += Time.deltaTime;
		Color color = fade.color;
		float time = Mathf.Clamp01(timeFadeTime / timeFade);
		time = fadeAnimation.Evaluate(time);
		switch (kindFade)
		{
		case FadeKind.Out:
			color.a = time;
			break;
		case FadeKind.In:
			color.a = 1f - time;
			break;
		case FadeKind.OutIn:
			color.a = Mathf.Sin((float)Math.PI / 180f * Mathf.Lerp(0f, 180f, time));
			break;
		}
		fade.color = color;
		if (time >= 1f)
		{
			isFade = false;
			switch (kindFade)
			{
			case FadeKind.Out:
				kindFadeProc = FadeKindProc.OutEnd;
				break;
			case FadeKind.In:
				kindFadeProc = FadeKindProc.InEnd;
				fade.enabled = false;
				break;
			case FadeKind.OutIn:
				kindFadeProc = FadeKindProc.OutInEnd;
				fade.enabled = false;
				break;
			}
		}
		return true;
	}
}
