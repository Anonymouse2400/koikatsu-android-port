using System;
using IllusionUtility.GetUtility;
using UnityEngine;

public class Fade : MonoBehaviour
{
	public enum Type
	{
		InOut = 0,
		In = 1,
		Out = 2
	}

	[Serializable]
	public class FadeIn
	{
		public float start;

		public float end = 1f;

		public float time = 2f;
	}

	[Serializable]
	public class FadeOut
	{
		public float start = 1f;

		public float end;

		public float time = 2f;
	}

	[SerializeField]
	private CanvasRenderer fadeRenderer;

	[SerializeField]
	private CanvasRenderer loadingRenderer;

	public float fadeWaitTime = 1f;

	public FadeIn fadeIn;

	public FadeOut fadeOut;

	private float fadeTimer;

	private float nowWaitTimer;

	private bool usingLoadingTex;

	public Type nowType { get; private set; }

	public bool isFadeNow { get; private set; }

	public bool FadeSet(Type type, bool _usingLoadingTex = true)
	{
		if (!fadeRenderer)
		{
			isFadeNow = false;
			return false;
		}
		if (!loadingRenderer)
		{
			isFadeNow = false;
			return false;
		}
		isFadeNow = true;
		usingLoadingTex = _usingLoadingTex;
		nowType = type;
		nowWaitTimer = 0f;
		fadeTimer = 0f;
		switch (type)
		{
		case Type.InOut:
		case Type.In:
			fadeRenderer.SetAlpha(fadeIn.start);
			loadingRenderer.SetAlpha((!usingLoadingTex) ? 0f : fadeIn.start);
			break;
		case Type.Out:
			fadeRenderer.SetAlpha(fadeOut.start);
			loadingRenderer.SetAlpha((!usingLoadingTex) ? 0f : fadeOut.start);
			break;
		}
		return true;
	}

	public void FadeEnd()
	{
		nowType = Type.Out;
		isFadeNow = false;
		if ((bool)fadeRenderer)
		{
			fadeRenderer.SetAlpha(0f);
		}
		if ((bool)loadingRenderer)
		{
			loadingRenderer.SetAlpha(0f);
		}
	}

	public bool IsFadeIn()
	{
		return nowType == Type.In || nowType == Type.InOut;
	}

	public bool IsAlphaMax()
	{
		return fadeRenderer.GetAlpha() >= 1f;
	}

	public bool IsAlphaMin()
	{
		return fadeRenderer.GetAlpha() <= 0f;
	}

	public void SetColor(Color _color)
	{
		fadeRenderer.SetColor(_color);
	}

	private void Awake()
	{
		if (fadeRenderer == null)
		{
			GameObject gameObject = base.transform.FindLoop("Fade");
			if ((bool)gameObject)
			{
				fadeRenderer = gameObject.GetComponent<CanvasRenderer>();
			}
		}
		if (loadingRenderer == null)
		{
			GameObject gameObject2 = base.transform.FindLoop("NowLoading");
			if ((bool)gameObject2)
			{
				loadingRenderer = gameObject2.GetComponent<CanvasRenderer>();
			}
		}
		fadeRenderer.SetAlpha(0f);
		loadingRenderer.SetAlpha(0f);
		isFadeNow = false;
		nowType = Type.Out;
		nowWaitTimer = 0f;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!isFadeNow)
		{
			return;
		}
		float num = Mathf.Clamp01(Time.unscaledDeltaTime);
		if (nowType == Type.In || nowType == Type.InOut)
		{
			fadeTimer += num;
			float num2 = Mathf.Clamp01(Mathf.Lerp(fadeIn.start, fadeIn.end, Mathf.InverseLerp(0f, fadeIn.time, fadeTimer)));
			fadeRenderer.SetAlpha(num2);
			loadingRenderer.SetAlpha((!usingLoadingTex) ? 0f : num2);
			if (num2 == fadeIn.end && nowType == Type.InOut)
			{
				nowWaitTimer = Mathf.Min(nowWaitTimer + num, fadeWaitTime);
				if (nowWaitTimer >= fadeWaitTime)
				{
					nowType = Type.Out;
					fadeTimer = 0f;
				}
			}
		}
		else if (nowType == Type.Out)
		{
			fadeTimer += num;
			float num3 = Mathf.Clamp01(Mathf.Lerp(fadeOut.start, fadeOut.end, Mathf.InverseLerp(0f, fadeOut.time, fadeTimer)));
			fadeRenderer.SetAlpha(num3);
			loadingRenderer.SetAlpha((!usingLoadingTex) ? 0f : num3);
			if (num3 == fadeOut.end)
			{
				FadeEnd();
			}
		}
	}
}
