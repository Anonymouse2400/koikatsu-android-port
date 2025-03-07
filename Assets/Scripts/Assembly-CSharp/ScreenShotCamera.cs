using System.Collections;
using UnityEngine;

public class ScreenShotCamera : MonoBehaviour
{
	public RenderTexture renderTexture { get; private set; }

	private IEnumerator Start()
	{
		base.enabled = false;
		do
		{
			renderTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			yield return null;
		}
		while (renderTexture == null);
		base.enabled = true;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		Graphics.Blit(src, renderTexture);
		Graphics.Blit(src, dst);
	}

	private void OnDestroy()
	{
		if ((bool)renderTexture)
		{
			renderTexture.Release();
			renderTexture = null;
		}
	}
}
