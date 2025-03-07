using UnityEngine;

namespace ChaCustom
{
	public class CustomRender : MonoBehaviour
	{
		public bool update = true;

		public RenderTexture rtCamera { get; private set; }

		private void Start()
		{
			rtCamera = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		}

		private void OnRenderImage(RenderTexture src, RenderTexture dst)
		{
			if (update)
			{
				if (rtCamera == null)
				{
					Graphics.Blit(src, dst);
					return;
				}
				Graphics.Blit(src, rtCamera);
				Graphics.Blit(src, dst);
			}
			else
			{
				Graphics.Blit(rtCamera, dst);
			}
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
}
