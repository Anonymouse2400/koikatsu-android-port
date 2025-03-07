using System.IO;
using IllusionUtility.GetUtility;
using UnityEngine;

public static class PngAssist
{
	public static Sprite LoadSpriteFromFile(string path, int width, int height, Vector2 pivot)
	{
		if (!File.Exists(path))
		{
			return null;
		}
		using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			using (BinaryReader binaryReader = new BinaryReader(input))
			{
				byte[] data = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
				Texture2D texture2D = new Texture2D(width, height);
				if (null == texture2D)
				{
					return null;
				}
				texture2D.LoadImage(data);
				if (width == 0 || height == 0)
				{
					width = texture2D.width;
					height = texture2D.height;
				}
				return Sprite.Create(texture2D, new Rect(0f, 0f, width, height), pivot);
			}
		}
	}

	public static Texture2D LoadTexture2DFromAssetBundle(string assetBundleName, string assetName)
	{
		TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		Texture2D texture2D = new Texture2D(0, 0, TextureFormat.ARGB32, false, true);
		if (null == texture2D)
		{
			return null;
		}
		texture2D.LoadImage(textAsset.bytes);
		return texture2D;
	}

	public static Sprite LoadSpriteFromAssetBundle(string assetBundleName, string assetName, int width, int height, Vector2 pivot)
	{
		TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
		AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		Texture2D texture2D = new Texture2D(width, height);
		if (null == texture2D)
		{
			return null;
		}
		texture2D.LoadImage(textAsset.bytes);
		if (width == 0 || height == 0)
		{
			width = texture2D.width;
			height = texture2D.height;
		}
		return Sprite.Create(texture2D, new Rect(0f, 0f, width, height), pivot);
	}

	public static Texture2D ChangeTextureFromByte(byte[] data, int width = 0, int height = 0)
	{
		Texture2D texture2D = new Texture2D(width, height);
		if (null == texture2D)
		{
			return null;
		}
		texture2D.LoadImage(data);
		return texture2D;
	}

	public static void SavePng(BinaryWriter writer, int capW = 504, int capH = 704, int createW = 252, int createH = 352, float renderRate = 1f, bool drawBackSp = true, bool drawFrontSp = true)
	{
		byte[] pngData = null;
		CreatePng(ref pngData, capW, capH, createW, createH, renderRate, drawBackSp, drawFrontSp);
		if (pngData != null)
		{
			writer.Write(pngData);
			pngData = null;
		}
	}

	public static void CreatePng(ref byte[] pngData, int capW = 504, int capH = 704, int createW = 252, int createH = 352, float renderRate = 1f, bool drawBackSp = true, bool drawFrontSp = true)
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("SpriteTop");
		Vector2 screenSize = ScreenInfo.GetScreenSize();
		float screenRate = ScreenInfo.GetScreenRate();
		float screenCorrectY = ScreenInfo.GetScreenCorrectY();
		float num = 720f * screenRate / screenSize.y;
		int num2 = (int)((float)capW * renderRate);
		int num3 = (int)((float)capH * renderRate);
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		renderTexture2 = ((QualitySettings.antiAliasing != 0) ? RenderTexture.GetTemporary((int)(1280f * renderRate / num), (int)(720f * renderRate / num), 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default, QualitySettings.antiAliasing) : RenderTexture.GetTemporary((int)(1280f * renderRate / num), (int)(720f * renderRate / num), 24));
		if (drawBackSp && (bool)gameObject)
		{
			GameObject gameObject2 = gameObject.transform.FindLoop("BackSpCam");
			if ((bool)gameObject2)
			{
				Camera component = gameObject2.GetComponent<Camera>();
				if ((bool)component)
				{
					component.targetTexture = renderTexture2;
					component.Render();
					component.targetTexture = null;
				}
			}
		}
		if ((bool)Camera.main)
		{
			Camera main = Camera.main;
			renderTexture = main.targetTexture;
			Rect rect = main.rect;
			main.targetTexture = renderTexture2;
			main.Render();
			main.targetTexture = renderTexture;
			main.rect = rect;
		}
		if (drawFrontSp && (bool)gameObject)
		{
			GameObject gameObject3 = gameObject.transform.FindLoop("FrontSpCam");
			if ((bool)gameObject3)
			{
				Camera component2 = gameObject3.GetComponent<Camera>();
				if ((bool)component2)
				{
					component2.targetTexture = renderTexture2;
					component2.Render();
					component2.targetTexture = null;
				}
			}
		}
		Texture2D texture2D = new Texture2D(num2, num3, TextureFormat.ARGB32, false, true);
		RenderTexture.active = renderTexture2;
		float x = (float)(1280 - capW) / 2f * renderRate + (1280f / num - 1280f) * 0.5f * renderRate;
		float y = (float)(720 - capH) / 2f * renderRate + screenCorrectY / screenRate * renderRate;
		texture2D.ReadPixels(new Rect(x, y, num2, num3), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(renderTexture2);
		if (num2 != createW || num3 != createH)
		{
			TextureScale.Bilinear(texture2D, createW, createH);
		}
		pngData = texture2D.EncodeToPNG();
		Object.Destroy(texture2D);
		texture2D = null;
	}

	public static void CreatePngScreen(ref byte[] pngData, int createW, int createH)
	{
		Vector2 screenSize = ScreenInfo.GetScreenSize();
		int num = (int)screenSize.x;
		int num2 = (int)screenSize.y;
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGB24, false);
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		renderTexture2 = ((QualitySettings.antiAliasing != 0) ? RenderTexture.GetTemporary(num, num2, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, QualitySettings.antiAliasing) : RenderTexture.GetTemporary(num, num2, 24));
		if ((bool)Camera.main)
		{
			Camera main = Camera.main;
			renderTexture = main.targetTexture;
			Rect rect = main.rect;
			main.targetTexture = renderTexture2;
			main.Render();
			main.targetTexture = renderTexture;
			main.rect = rect;
		}
		RenderTexture.active = renderTexture2;
		texture2D.ReadPixels(new Rect(0f, 0f, num, num2), 0, 0);
		texture2D.Apply();
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(renderTexture2);
		TextureScale.Bilinear(texture2D, createW, createH);
		pngData = texture2D.EncodeToPNG();
		Object.Destroy(texture2D);
		texture2D = null;
	}

	public static Sprite LoadSpriteFromFile(string path)
	{
		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
		{
			long pngSize = PngFile.GetPngSize(fileStream);
			if (pngSize == 0)
			{
				return null;
			}
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				byte[] data = binaryReader.ReadBytes((int)pngSize);
				int width = 0;
				int height = 0;
				Texture2D texture2D = ChangeTextureFromPngByte(data, ref width, ref height);
				if (null == texture2D)
				{
					return null;
				}
				return Sprite.Create(texture2D, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f));
			}
		}
	}

	public static Texture2D ChangeTextureFromPngByte(byte[] data, ref int width, ref int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		if (null == texture2D)
		{
			return null;
		}
		texture2D.LoadImage(data);
		width = texture2D.width;
		height = texture2D.height;
		return texture2D;
	}

	public static Texture2D LoadTexture(string _path)
	{
		using (FileStream fileStream = new FileStream(_path, FileMode.Open, FileAccess.Read))
		{
			long pngSize = PngFile.GetPngSize(fileStream);
			if (pngSize == 0)
			{
				return null;
			}
			using (BinaryReader binaryReader = new BinaryReader(fileStream))
			{
				byte[] data = binaryReader.ReadBytes((int)pngSize);
				int width = 0;
				int height = 0;
				return ChangeTextureFromPngByte(data, ref width, ref height);
			}
		}
	}
}
