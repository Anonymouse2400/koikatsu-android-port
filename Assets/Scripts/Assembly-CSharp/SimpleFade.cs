using UnityEngine;

public class SimpleFade : MonoBehaviour
{
	public enum Fade
	{
		In = 0,
		Out = 1
	}

	public class FadeInOut
	{
		public float inTime = 1f;

		public float outTime = 1f;

		public Color inColor = Color.white;

		public Color outColor = Color.white;

		public float waitTime = 1f;

		private float timer;

		public FadeInOut()
		{
		}

		public FadeInOut(SimpleFade fade)
		{
			inTime = (outTime = fade._Time);
			inColor = (outColor = fade._Color);
		}

		public bool Update()
		{
			timer = Mathf.Min(timer + Time.deltaTime, waitTime);
			return timer == waitTime;
		}
	}

	public Fade _Fade;

	public float _Time = 1f;

	public Color _Color = Color.white;

	public Texture2D _Texture;

	protected float timer;

	private FadeInOut fadeInOut;

	public bool IsFadeNow
	{
		get
		{
			return _Fade == Fade.In || !IsEnd;
		}
	}

	public bool IsEnd
	{
		get
		{
			return timer == _Time && fadeInOut == null;
		}
	}

	public void FadeSet(Fade fade, float time = -1f, Texture2D tex = null)
	{
		_Fade = fade;
		if (time != -1f)
		{
			_Time = time;
		}
		if (tex != null)
		{
			_Texture = tex;
		}
		Init();
	}

	public void FadeInOutSet(FadeInOut set, Texture2D tex = null)
	{
		if (tex != null)
		{
			_Texture = tex;
		}
		FadeInOutStart(set);
		Init();
	}

	public virtual void Init()
	{
		timer = 0f;
		_Color.a = ((_Fade != 0) ? 1 : 0);
		if (_Texture == null)
		{
			_Texture = Texture2D.whiteTexture;
		}
	}

	public virtual void ForceEnd()
	{
		timer = _Time;
		_Color.a = ((_Fade == Fade.In) ? 1 : 0);
	}

	private void FadeInOutStart(FadeInOut set)
	{
		if (set != null)
		{
			fadeInOut = set;
		}
		if (fadeInOut != null)
		{
			_Fade = ((set == null) ? Fade.Out : Fade.In);
			_Time = ((set == null) ? fadeInOut.outTime : fadeInOut.inTime);
			_Color = ((set == null) ? fadeInOut.outColor : fadeInOut.inColor);
			fadeInOut = set;
			Init();
		}
	}

	protected virtual void Awake()
	{
		ForceEnd();
	}

	protected virtual void Update()
	{
		float a = ((_Fade != 0) ? 1 : 0);
		float b = ((_Fade == Fade.In) ? 1 : 0);
		timer = Mathf.Min(timer + Time.deltaTime, _Time);
		if (fadeInOut != null && timer == _Time && fadeInOut.Update())
		{
			FadeInOutStart(null);
		}
		else
		{
			_Color.a = Mathf.Lerp(a, b, Mathf.InverseLerp(0f, _Time, timer));
		}
	}
}
