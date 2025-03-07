using UnityEngine;

public class BaseRenderCrossFade : MonoBehaviour
{
	public Camera uiCamera;

	public Camera targetCamera;

	protected float maxTime = 1f;

	protected float timer;

	protected bool isAlphaAdd = true;

	public RenderTexture texture;

	protected bool isInitRenderSetting = true;

	public bool isDrawGUI { get; protected set; }

	public float alpha { get; protected set; }

	public float MaxTime
	{
		set
		{
			maxTime = value;
			timer = 0f;
		}
	}

	protected void AlphaCalc()
	{
		alpha = Mathf.InverseLerp(0f, maxTime, timer);
		if (!isAlphaAdd)
		{
			alpha = Mathf.Lerp(1f, 0f, alpha);
		}
	}

	public void Capture()
	{
		if (texture != null && (texture.width != Screen.width || texture.height != Screen.height))
		{
			CreateRenderTexture();
		}
		if (!isInitRenderSetting)
		{
			RenderTargetSetting();
		}
		RenderTexture renderTexture = null;
		if (targetCamera != null)
		{
			renderTexture = targetCamera.targetTexture;
			Rect rect = targetCamera.rect;
			targetCamera.targetTexture = texture;
			targetCamera.Render();
			targetCamera.targetTexture = renderTexture;
			targetCamera.rect = rect;
		}
		if (uiCamera != null)
		{
			renderTexture = uiCamera.targetTexture;
			Rect rect = uiCamera.rect;
			uiCamera.targetTexture = texture;
			uiCamera.Render();
			uiCamera.targetTexture = renderTexture;
			uiCamera.rect = rect;
		}
		timer = 0f;
		isDrawGUI = false;
		AlphaCalc();
	}

	public virtual void End()
	{
		timer = maxTime;
		AlphaCalc();
	}

	public void Destroy()
	{
		ReleaseRenderTexture();
	}

	protected virtual void Awake()
	{
		CreateRenderTexture();
		RenderTargetSetting();
		isDrawGUI = false;
	}

	protected virtual void Update()
	{
		timer += Time.deltaTime;
		timer = Mathf.Min(timer, maxTime);
		AlphaCalc();
	}

	protected virtual void OnGUI()
	{
		GUI.depth = 10;
		GUI.color = new Color(1f, 1f, 1f, alpha);
		GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), texture);
		isDrawGUI = true;
	}

	private void CreateRenderTexture()
	{
		ReleaseRenderTexture();
		texture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.RGB565);
		texture.antiAliasing = ((QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing);
		texture.enableRandomWrite = false;
	}

	private void ReleaseRenderTexture()
	{
		if (texture != null)
		{
			texture.Release();
			Object.Destroy(texture);
			texture = null;
		}
	}

	private void RenderTargetSetting()
	{
		if (uiCamera == null)
		{
			GameObject gameObject = GameObject.Find("SpDef");
			if ((bool)gameObject)
			{
				uiCamera = gameObject.GetComponent<Camera>();
			}
		}
		if (targetCamera == null)
		{
			targetCamera = Camera.main;
		}
	}
}
