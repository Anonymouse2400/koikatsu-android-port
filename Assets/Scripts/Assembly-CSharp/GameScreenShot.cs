using System;
using System.Collections;
using System.IO;
using System.Text;
using Illusion.CustomAttributes;
using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;

public class GameScreenShot : MonoBehaviour
{
	[Button("Capture", "キャプチャー", new object[] { "" })]
	public int excuteCapture;

	[Button("UnityCapture", "Unityキャプチャー", new object[] { "" })]
	public int excuteCaptureEx;

	public bool capExMode;

	public bool modeARGB;

	public Camera[] renderCam;

	public GameScreenShotAssist[] scriptGssAssist;

	public Texture texCapMark;

	public Vector2 texPosition = new Vector2(1152f, 688f);

	public int capRate = 1;

	private string savePath = string.Empty;

	private IDisposable captureDisposable;

	public string CreateCaptureFileName()
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		stringBuilder.Append(UserData.Create("cap"));
		DateTime now = DateTime.Now;
		stringBuilder.Append(now.Year.ToString("0000"));
		stringBuilder.Append(now.Month.ToString("00"));
		stringBuilder.Append(now.Day.ToString("00"));
		stringBuilder.Append(now.Hour.ToString("00"));
		stringBuilder.Append(now.Minute.ToString("00"));
		stringBuilder.Append(now.Second.ToString("00"));
		stringBuilder.Append(now.Millisecond.ToString("000"));
		stringBuilder.Append(".png");
		return stringBuilder.ToString();
	}

	public void Capture(string path = "")
	{
		if (captureDisposable != null || (Singleton<Scene>.IsInstance() && Singleton<Scene>.Instance.IsNowLoadingFade))
		{
			return;
		}
		bool isRenderSetCam = false;
		if (!capExMode)
		{
			if (renderCam.IsNullOrEmpty())
			{
				if (Camera.main == null)
				{
					return;
				}
				isRenderSetCam = true;
				renderCam = new Camera[1] { Camera.main };
			}
		}
		else if (scriptGssAssist.Length == 0)
		{
			return;
		}
		savePath = path;
		if (savePath == string.Empty)
		{
			savePath = CreateCaptureFileName();
		}
		captureDisposable = Observable.FromCoroutine(CaptureFunc).Subscribe(delegate
		{
			if (isRenderSetCam)
			{
				renderCam = null;
			}
			captureDisposable = null;
			Utils.Sound.Play(SystemSE.photo);
		});
	}

	public void UnityCapture(string path = "")
	{
		savePath = path;
		if (string.Empty == savePath)
		{
			savePath = CreateCaptureFileName();
		}
        Application.CaptureScreenshot(savePath, capRate);
	}

	private IEnumerator CaptureFunc()
	{
		GameScreenShotOnGUI shotGUI = null;
		if (texCapMark != null)
		{
			shotGUI = this.GetOrAddComponent<GameScreenShotOnGUI>();
		}
		yield return new WaitForEndOfFrame();
		if (shotGUI != null)
		{
			UnityEngine.Object.Destroy(shotGUI);
		}
		float rate = ((capRate == 0) ? 1 : capRate);
		Texture2D screenShot = new Texture2D((int)((float)Screen.width * rate), (int)((float)Screen.height * rate), (!modeARGB) ? TextureFormat.RGB24 : TextureFormat.ARGB32, false);
		RenderTexture rt = RenderTexture.GetTemporary(antiAliasing: (QualitySettings.antiAliasing == 0) ? 1 : QualitySettings.antiAliasing, width: screenShot.width, height: screenShot.height, depthBuffer: 24, format: RenderTextureFormat.Default, readWrite: RenderTextureReadWrite.Default);
		Action drawCapMark = delegate
		{
			float num = (float)Screen.width / 1920f;
			Graphics.DrawTexture(new Rect(texPosition.x * num, texPosition.y * num, (float)texCapMark.width * num, (float)texCapMark.height * num), texCapMark);
		};
		if (!capExMode)
		{
			Graphics.SetRenderTarget(rt);
			GL.Clear(true, true, Color.black);
			Graphics.SetRenderTarget(null);
			Camera[] array = renderCam;
			foreach (Camera camera in array)
			{
				if (!(null == camera))
				{
					bool flag = camera.enabled;
					RenderTexture targetTexture = camera.targetTexture;
					Rect rect = camera.rect;
					camera.enabled = true;
					camera.targetTexture = rt;
					camera.Render();
					camera.targetTexture = targetTexture;
					camera.rect = rect;
					camera.enabled = flag;
				}
			}
			if ((bool)texCapMark)
			{
				Graphics.SetRenderTarget(rt);
				drawCapMark();
				Graphics.SetRenderTarget(null);
			}
		}
		else
		{
			Graphics.Blit(scriptGssAssist[0].rtCamera, rt);
			for (int j = 1; j < scriptGssAssist.Length; j++)
			{
				Graphics.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), scriptGssAssist[j].rtCamera);
			}
			if ((bool)texCapMark)
			{
				drawCapMark();
			}
		}
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0f, 0f, screenShot.width, screenShot.height), 0, 0);
		screenShot.Apply();
		RenderTexture.active = null;
		byte[] bytes = screenShot.EncodeToPNG();
		RenderTexture.ReleaseTemporary(rt);
		UnityEngine.Object.Destroy(screenShot);
		screenShot = null;
		File.WriteAllBytes(savePath, bytes);
		yield return null;
	}
}
