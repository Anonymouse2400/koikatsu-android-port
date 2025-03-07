using UnityEngine;

public class CrossFadeCreater
{
	private static CrossFadeObject Create()
	{
		GameObject gameObject = new GameObject("CrossFade");
		return gameObject.AddComponent<CrossFadeObject>();
	}

	public static RenderTexture Capture(Camera renderCamera)
	{
		RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
		renderTexture.enableRandomWrite = false;
		renderCamera.targetTexture = renderTexture;
		renderCamera.Render();
		renderCamera.targetTexture = null;
		return renderTexture;
	}
}
