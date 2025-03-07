using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Studio
{
	public class FrameCtrl : MonoBehaviour
	{
		[SerializeField]
		private Camera cameraUI;

		[SerializeField]
		private RawImage imageFrame;

		public bool Load(string _file)
		{
			Release();
			string path = UserData.Path + "frame/" + _file;
			if (!File.Exists(path))
			{
				Singleton<Studio>.Instance.sceneInfo.frame = string.Empty;
				return false;
			}
			Texture texture = PngAssist.LoadTexture(path);
			if (texture == null)
			{
				return false;
			}
			imageFrame.texture = texture;
			imageFrame.enabled = true;
			cameraUI.enabled = true;
			Singleton<Studio>.Instance.sceneInfo.frame = _file;
			Resources.UnloadUnusedAssets();
			GC.Collect();
			return true;
		}

		public void Release()
		{
			UnityEngine.Object.Destroy(imageFrame.texture);
			imageFrame.texture = null;
			imageFrame.enabled = false;
			cameraUI.enabled = false;
			Resources.UnloadUnusedAssets();
		}
	}
}
