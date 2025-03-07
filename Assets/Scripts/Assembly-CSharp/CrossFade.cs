using Illusion.CustomAttributes;
using UnityEngine;

public class CrossFade : MonoBehaviour
{
	[Label("CrossFadeマテリアル")]
	public Material materiaEffect;

	[Label("フェード時間")]
	public float time = 0.3f;

	[Header("Debug表示")]
	[SerializeField]
	private RenderTexture texBase;

	[Range(0f, 1f)]
	[SerializeField]
	private float alpha;

	[SerializeField]
	[NotEditable]
	private float timer;

	private float timeCalc;

	private int _FadeTex;

	private int _Alpha;

	private bool isProcess;

	[Button("FadeStart", "FadeStart", new object[] { -1 })]
	public int FadeStartButton;

	public bool isEnd { get; private set; }

	public void FadeStart(float time = -1f)
	{
		if (!(texBase == null))
		{
			timeCalc = ((!(time < 0f)) ? time : this.time);
			if (!Mathf.Approximately(timeCalc, 0f))
			{
				timer = 0f;
				alpha = 0f;
				isProcess = true;
				isEnd = false;
			}
		}
	}

	public void End()
	{
		timer = timeCalc;
		isEnd = true;
		alpha = 1f;
	}

	private void OnDestroy()
	{
		if (texBase != null)
		{
			texBase.Release();
		}
	}

	private void Start()
	{
		_FadeTex = Shader.PropertyToID("_FadeTex");
		_Alpha = Shader.PropertyToID("_Alpha");
		isProcess = false;
		isEnd = true;
		texBase = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
	}

	private void Update()
	{
		if (isProcess)
		{
			timer = Mathf.Clamp(timer + Time.smoothDeltaTime, 0f, timeCalc);
			isEnd = timer >= timeCalc;
			if (isEnd)
			{
				alpha = 1f;
			}
			else
			{
				alpha = timer / timeCalc;
			}
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (texBase == null)
		{
			Graphics.Blit(src, dst);
			return;
		}
		if (!isProcess)
		{
			Graphics.Blit(src, texBase);
			Graphics.Blit(src, dst);
			return;
		}
		materiaEffect.SetTexture(_FadeTex, texBase);
		materiaEffect.SetFloat(_Alpha, alpha);
		Graphics.Blit(src, dst, materiaEffect);
		isProcess = !isEnd;
	}
}
