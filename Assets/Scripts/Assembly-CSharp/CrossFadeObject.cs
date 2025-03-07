using UnityEngine;

public class CrossFadeObject : MonoBehaviour
{
	public RenderTexture tex;

	private float span = 1f;

	private float delay;

	private float timer;

	private int depth = 10;

	private float alpha = 1f;

	private float CalcRate()
	{
		if (span == 0f)
		{
			return 1f;
		}
		if (timer < delay)
		{
			return 0f;
		}
		float num = timer - delay;
		return Mathf.Clamp01(num / span);
	}

	private void Awake()
	{
		alpha = 1f;
	}

	private void Update()
	{
		float num = CalcRate();
		if (timer > delay && num >= 1f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		alpha = 1f - num;
		timer += Time.deltaTime;
	}

	private void OnDestroy()
	{
		if ((bool)tex)
		{
			tex.Release();
			Object.Destroy(tex);
			tex = null;
		}
	}

	private void OnGUI()
	{
		GUI.depth = depth;
		GUI.color = new Color(1f, 1f, 1f, alpha);
		if (!(tex == null))
		{
			GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), tex, ScaleMode.ScaleToFit, false);
		}
	}
}
