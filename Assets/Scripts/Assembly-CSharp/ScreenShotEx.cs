using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ScreenShotEx : MonoBehaviour
{
	public class SSInfo
	{
		public string path = string.Empty;

		public bool alpha;

		public int width;

		public int height;

		public int rate = 1;

		public void Set(string _path, bool _alpha, int _width, int _height, int _rate)
		{
			path = _path;
			alpha = _alpha;
			width = _width;
			height = _height;
			rate = _rate;
		}
	}

	private bool capFlag;

	private SSInfo ssinfo = new SSInfo();

	private void Update()
	{
		if (capFlag)
		{
			capFlag = false;
			StartCoroutine(CaptureProc(ssinfo));
		}
	}

	private IEnumerator CaptureProc(SSInfo ssinfo)
	{
		yield return new WaitForEndOfFrame();
		int width = ((ssinfo.width != 0) ? ssinfo.width : Screen.width) * ssinfo.rate;
		int height = ((ssinfo.height != 0) ? ssinfo.height : Screen.height) * ssinfo.rate;
		Texture2D tex = new Texture2D(width, height, (!ssinfo.alpha) ? TextureFormat.RGB24 : TextureFormat.ARGB32, false);
		RenderTexture rt = null;
		rt = ((QualitySettings.antiAliasing != 0) ? RenderTexture.GetTemporary(width, height, (!ssinfo.alpha) ? 24 : 32, RenderTextureFormat.Default, RenderTextureReadWrite.Default, QualitySettings.antiAliasing) : RenderTexture.GetTemporary(width, height, (!ssinfo.alpha) ? 24 : 32));
		Camera RenderCam = Camera.main;
		RenderTexture backRenderTexture = RenderCam.targetTexture;
		Rect backRect = RenderCam.rect;
		RenderCam.targetTexture = rt;
		RenderCam.Render();
		RenderCam.targetTexture = backRenderTexture;
		RenderCam.rect = backRect;
		RenderTexture.active = rt;
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		RenderTexture.active = null;
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		RenderTexture.ReleaseTemporary(rt);
		tex = null;
		if (string.Empty == ssinfo.path)
		{
			ssinfo.path = UserData.Create("cap");
			string text = ".png";
			string text2 = DateTime.Now.Year.ToString("0000");
			text2 += DateTime.Now.Month.ToString("00");
			text2 += DateTime.Now.Day.ToString("00");
			text2 += DateTime.Now.Hour.ToString("00");
			text2 += DateTime.Now.Minute.ToString("00");
			text2 += DateTime.Now.Second.ToString("00");
			text2 += DateTime.Now.Millisecond.ToString("000");
			ssinfo.path = ssinfo.path + text2 + text;
		}
		File.WriteAllBytes(ssinfo.path, bytes);
		UnityEngine.Object.Destroy(base.gameObject);
		yield return null;
	}

	public static void Capture(string _path, bool _alpha, int _width, int _height, int _rate)
	{
		GameObject gameObject = new GameObject("CapExObj");
		if ((bool)gameObject)
		{
			ScreenShotEx screenShotEx = gameObject.AddComponent<ScreenShotEx>();
			screenShotEx.ssinfo.Set(_path, _alpha, _width, _height, _rate);
			screenShotEx.capFlag = true;
		}
	}
}
