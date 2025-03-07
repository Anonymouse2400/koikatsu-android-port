using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class EMTransition : MonoBehaviour
{
	[SerializeField]
	private Texture2D m_gradationTexture;

	[SerializeField]
	private float m_duration = 1f;

	[SerializeField]
	private bool m_playOnAwake = true;

	[SerializeField]
	private bool m_flipAfterAnimation;

	[SerializeField]
	private bool m_flip;

	[SerializeField]
	private bool m_invert;

	[SerializeField]
	private bool m_ignoreTimeScale;

	[SerializeField]
	private bool m_pingPong;

	[SerializeField]
	private AnimationCurve m_curve;

	[SerializeField]
	[Range(0f, 1f)]
	private float m_threshold;

	public UnityEvent onTransitionStart;

	public UnityEvent onTransitionComplete;

	private RawImage _image;

	private static int? _gradationID;

	private static int? _invertID;

	private static int? _cutoffID;

	private Coroutine playCoroutine;

	public RawImage image
	{
		get
		{
			return this.GetComponentCache(ref _image);
		}
	}

	private int _GradationID
	{
		get
		{
			if (!_gradationID.HasValue)
			{
				_gradationID = Shader.PropertyToID("_Gradation");
			}
			return _gradationID.Value;
		}
	}

	private int _InvertID
	{
		get
		{
			if (!_invertID.HasValue)
			{
				_invertID = Shader.PropertyToID("_Invert");
			}
			return _invertID.Value;
		}
	}

	private int _CutoffID
	{
		get
		{
			if (!_cutoffID.HasValue)
			{
				_cutoffID = Shader.PropertyToID("_Cutoff");
			}
			return _cutoffID.Value;
		}
	}

	public Texture2D gradationTexture
	{
		get
		{
			return m_gradationTexture;
		}
		set
		{
			m_gradationTexture = value;
			image.material.SetTexture(_GradationID, m_gradationTexture);
		}
	}

	public float duration
	{
		get
		{
			return m_duration;
		}
		set
		{
			m_duration = Mathf.Max(value, 0f);
		}
	}

	public bool playOnAwake
	{
		get
		{
			return m_playOnAwake;
		}
		set
		{
			m_playOnAwake = value;
		}
	}

	public bool flipAfterAnimation
	{
		get
		{
			return m_flipAfterAnimation;
		}
		set
		{
			m_flipAfterAnimation = value;
		}
	}

	public bool flip
	{
		get
		{
			return m_flip;
		}
		set
		{
			m_flip = value;
		}
	}

	public bool invert
	{
		get
		{
			return m_invert;
		}
		set
		{
			m_invert = value;
		}
	}

	public bool ignoreTimeScale
	{
		get
		{
			return m_ignoreTimeScale;
		}
		set
		{
			m_ignoreTimeScale = value;
		}
	}

	public AnimationCurve curve
	{
		get
		{
			return m_curve;
		}
		set
		{
			m_curve = value;
		}
	}

	public float threshold
	{
		get
		{
			return m_threshold;
		}
		set
		{
			m_threshold = value;
		}
	}

	private void Start()
	{
		if (m_flip)
		{
			m_curve = FlipCurve();
		}
		if (m_playOnAwake)
		{
			Play();
		}
	}

	private void OnValidate()
	{
		Material material = image.material;
		material.SetInt(_InvertID, m_invert ? 1 : 0);
		material.SetTexture(_GradationID, m_gradationTexture);
		material.SetFloat(_CutoffID, m_threshold);
		m_duration = Mathf.Max(m_duration, 0f);
	}

	public void Play()
	{
		Stop();
		m_threshold = ((!(m_curve.Evaluate(0f) > 0.5f)) ? 0f : 1f);
		if (!m_pingPong)
		{
			onTransitionStart.Invoke();
		}
		playCoroutine = StartCoroutine(PlayCoroutine());
	}

	public void Stop()
	{
		if (playCoroutine != null)
		{
			StopCoroutine(playCoroutine);
			playCoroutine = null;
		}
	}

	public void Set()
	{
		Stop();
		image.material.SetFloat(_CutoffID, m_threshold);
	}

	public void Set(float t)
	{
		m_threshold = m_curve.Evaluate(t);
		Set();
	}

	private IEnumerator PlayCoroutine()
	{
		Material material = image.material;
		if (!Mathf.Approximately(m_duration, 0f))
		{
			if (!m_ignoreTimeScale)
			{
				float t = Time.time;
				while (Time.time - t < m_duration)
				{
					m_threshold = m_curve.Evaluate((Time.time - t) / m_duration);
					material.SetFloat(_CutoffID, m_threshold);
					yield return null;
				}
			}
			else
			{
				float t = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup - t < m_duration)
				{
					m_threshold = m_curve.Evaluate((Time.realtimeSinceStartup - t) / m_duration);
					material.SetFloat(_CutoffID, m_threshold);
					yield return null;
				}
			}
		}
		m_threshold = m_curve.Evaluate(1f);
		material.SetFloat(_CutoffID, m_threshold);
		if (m_pingPong)
		{
			m_flip = !m_flip;
			m_curve = FlipCurve();
			Play();
			yield break;
		}
		if (m_flipAfterAnimation)
		{
			m_flip = !m_flip;
			m_curve = FlipCurve();
		}
		onTransitionComplete.Invoke();
	}

	public void SetColor(Color col)
	{
		image.color = col;
	}

	public void SetTexture(Texture2D tex)
	{
		image.texture = tex;
	}

	public void SetGradationTexture(Texture2D tex)
	{
		gradationTexture = tex;
	}

	public void FlipAnimationCurve()
	{
		m_curve = FlipCurve();
	}

	private AnimationCurve FlipCurve()
	{
		AnimationCurve animationCurve = new AnimationCurve();
		for (int i = 0; i < m_curve.length; i++)
		{
			Keyframe key = m_curve[i];
			key.time = 1f - key.time;
			key.inTangent *= -1f;
			key.outTangent *= -1f;
			animationCurve.AddKey(key);
		}
		return animationCurve;
	}
}
