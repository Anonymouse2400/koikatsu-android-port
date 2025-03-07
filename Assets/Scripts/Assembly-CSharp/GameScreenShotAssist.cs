using UnityEngine;

public class GameScreenShotAssist : MonoBehaviour
{
	public RenderTexture rtCamera { get; private set; }

	private void Start()
	{
		rtCamera = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (rtCamera == null)
		{
			Graphics.Blit(src, dst);
			return;
		}
		Graphics.Blit(src, rtCamera);
		Graphics.Blit(src, dst);
	}

	private void OnDestroy()
	{
		if ((bool)rtCamera)
		{
			rtCamera.Release();
			rtCamera = null;
		}
	}
}
