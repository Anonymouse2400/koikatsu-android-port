using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomCapture : MonoBehaviour
	{
		[SerializeField]
		private Camera camBG;

		public Camera camMain;

		[SerializeField]
		private RawImage riFace;

		[SerializeField]
		private RawImage riCard;

		[SerializeField]
		private RectTransform rtfBG;

		public void UpdateFaceImage(byte[] data)
		{
			if (!(null == riFace))
			{
				riFace.enabled = false;
				if (null != riFace.texture)
				{
					Object.DestroyImmediate(riFace.texture);
				}
				if (data != null)
				{
					riFace.enabled = true;
					Texture2D texture = PngAssist.ChangeTextureFromByte(data);
					riFace.texture = texture;
				}
			}
		}

		public void UpdateCardImage(byte[] data)
		{
			if (!(null == riCard))
			{
				riCard.enabled = false;
				if (null != riCard.texture)
				{
					Object.DestroyImmediate(riCard.texture);
				}
				if (data != null)
				{
					riCard.enabled = true;
					Texture2D texture = PngAssist.ChangeTextureFromByte(data);
					riCard.texture = texture;
				}
			}
		}

		public byte[] CapCharaCard(bool enableBG, SaveFrameAssist saveFrameAssist)
		{
			byte[] pngData = null;
			bool flag = saveFrameAssist != null && saveFrameAssist.backFrameDraw;
			bool flag2 = saveFrameAssist != null && saveFrameAssist.frontFrameDraw;
			Camera camBackFrame = ((saveFrameAssist == null) ? null : ((!flag) ? null : saveFrameAssist.backFrameCam));
			Camera camFrontFrame = ((saveFrameAssist == null) ? null : ((!flag2) ? null : saveFrameAssist.frontFrameCam));
			if ((bool)rtfBG)
			{
				rtfBG.localScale = new Vector3(1.023f, 1.023f, 1f);
			}
			if (saveFrameAssist != null)
			{
				if (null != saveFrameAssist.rtfSpBack)
				{
					saveFrameAssist.rtfSpBack.localScale = new Vector3(1.023f, 1.023f, 1f);
				}
				if (null != saveFrameAssist.rtfSpFront)
				{
					saveFrameAssist.rtfSpFront.localScale = new Vector3(1.023f, 1.023f, 1f);
				}
			}
			CreatePng(ref pngData, 252, 352, (!enableBG) ? null : camBG, camBackFrame, camMain, camFrontFrame);
			if ((bool)rtfBG)
			{
				rtfBG.localScale = new Vector3(1f, 1f, 1f);
			}
			if (saveFrameAssist != null)
			{
				if (null != saveFrameAssist.rtfSpBack)
				{
					saveFrameAssist.rtfSpBack.localScale = new Vector3(1f, 1f, 1f);
				}
				if (null != saveFrameAssist.rtfSpFront)
				{
					saveFrameAssist.rtfSpFront.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			return pngData;
		}

		public byte[] CapCharaFace(bool enableBG)
		{
			byte[] pngData = null;
			if ((bool)rtfBG)
			{
				rtfBG.localScale = new Vector3(1.689f, 1.689f, 1f);
			}
			CreatePng(ref pngData, 240, 320, (!enableBG) ? null : camBG, null, camMain);
			if ((bool)rtfBG)
			{
				rtfBG.localScale = new Vector3(1f, 1f, 1f);
			}
			return pngData;
		}

		public byte[] CapCoordinateCard(bool enableBG, SaveFrameAssist saveFrameAssist, Camera main)
		{
			byte[] pngData = null;
			bool flag = saveFrameAssist != null && saveFrameAssist.backFrameDraw;
			bool flag2 = saveFrameAssist != null && saveFrameAssist.frontFrameDraw;
			Camera camBackFrame = ((saveFrameAssist == null) ? null : ((!flag) ? null : saveFrameAssist.backFrameCam));
			Camera camFrontFrame = ((saveFrameAssist == null) ? null : ((!flag2) ? null : saveFrameAssist.frontFrameCam));
			if (saveFrameAssist != null)
			{
				if (null != saveFrameAssist.rtfSpBack)
				{
					saveFrameAssist.rtfSpBack.localScale = new Vector3(1.023f, 1.023f, 1f);
				}
				if (null != saveFrameAssist.rtfSpFront)
				{
					saveFrameAssist.rtfSpFront.localScale = new Vector3(1.023f, 1.023f, 1f);
				}
			}
			CreatePng(ref pngData, 252, 352, (!enableBG) ? null : camBG, camBackFrame, main, camFrontFrame);
			if (saveFrameAssist != null)
			{
				if (null != saveFrameAssist.rtfSpBack)
				{
					saveFrameAssist.rtfSpBack.localScale = new Vector3(1f, 1f, 1f);
				}
				if (null != saveFrameAssist.rtfSpFront)
				{
					saveFrameAssist.rtfSpFront.localScale = new Vector3(1f, 1f, 1f);
				}
			}
			return pngData;
		}

		public static void CreatePng(ref byte[] pngData, int createW = 252, int createH = 352, Camera _camBG = null, Camera _camBackFrame = null, Camera _camMain = null, Camera _camFrontFrame = null)
		{
			RenderTexture renderTexture = null;
			RenderTexture renderTexture2 = null;
			renderTexture2 = ((QualitySettings.antiAliasing != 0) ? RenderTexture.GetTemporary(createW, createH, 24, RenderTextureFormat.Default, RenderTextureReadWrite.Default, QualitySettings.antiAliasing) : RenderTexture.GetTemporary(createW, createH, 24));
			if ((bool)_camBG)
			{
				bool allowHDR = _camBG.allowHDR;
				_camBG.allowHDR = false;
				_camBG.targetTexture = renderTexture2;
				_camBG.Render();
				_camBG.targetTexture = null;
				_camBG.allowHDR = allowHDR;
			}
			if ((bool)_camBackFrame)
			{
				_camBackFrame.targetTexture = renderTexture2;
				_camBackFrame.Render();
				_camBackFrame.targetTexture = null;
			}
			if ((bool)_camMain)
			{
				renderTexture = _camMain.targetTexture;
				Rect rect = _camMain.rect;
				bool allowHDR2 = _camMain.allowHDR;
				_camMain.allowHDR = false;
				_camMain.targetTexture = renderTexture2;
				_camMain.rect = new Rect(0f, 0f, 1f, 1f);
				_camMain.Render();
				_camMain.targetTexture = renderTexture;
				_camMain.rect = rect;
				_camMain.allowHDR = allowHDR2;
			}
			if ((bool)_camFrontFrame)
			{
				_camFrontFrame.targetTexture = renderTexture2;
				_camFrontFrame.Render();
				_camFrontFrame.targetTexture = null;
			}
			Texture2D texture2D = new Texture2D(createW, createH, TextureFormat.RGB24, false, true);
			RenderTexture.active = renderTexture2;
			texture2D.ReadPixels(new Rect(0f, 0f, createW, createH), 0, 0);
			texture2D.Apply();
			RenderTexture.active = null;
			RenderTexture.ReleaseTemporary(renderTexture2);
			pngData = texture2D.EncodeToPNG();
			Object.Destroy(texture2D);
			texture2D = null;
		}
	}
}
