using System;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ExternalFile
{
	public class ExternalFileInfo
	{
		private enum State
		{
			NO_TEX = 0,
			LOADING = 1,
			LOAD_END = 2,
			HAS_TEX = 3
		}

		private bool isChangeTex;

		private Thread thread;

		private State state;

		private byte[] bytes;

		public int index;

		public string FullPath = string.Empty;

		public string FileName = string.Empty;

		public DateTime time;

		public Texture2D texThumb;

		public int category;

		public bool disable;

		public bool disvisible;

		public ExternalFileInfoComponent fic;

		public bool show;

		public void DeleteThumb()
		{
			if (thread != null && thread.IsAlive)
			{
				thread.Abort();
			}
			if (null != texThumb)
			{
				UnityEngine.Object.Destroy(texThumb);
				texThumb = null;
			}
		}

		private void LoadThumb()
		{
			if (state == State.NO_TEX && null == texThumb && (thread == null || !thread.IsAlive))
			{
				state = State.LOADING;
				thread = new Thread(LoadImageBytes);
				thread.Start();
			}
		}

		private void LoadImageBytes()
		{
			bytes = File.ReadAllBytes(FullPath);
			if (bytes == null)
			{
				state = State.NO_TEX;
			}
			else
			{
				state = State.LOAD_END;
			}
		}

		private void CreateImage()
		{
			if (state == State.LOAD_END)
			{
				texThumb = new Texture2D(1, 1, TextureFormat.ARGB32, false);
				texThumb.LoadImage(bytes);
				bytes = null;
				state = State.HAS_TEX;
				isChangeTex = true;
			}
		}

		private void ReleaseThumb(RawImage img)
		{
			if (state != 0)
			{
				state = State.NO_TEX;
				bytes = null;
				if (thread != null && thread.IsAlive)
				{
					thread.Abort();
				}
				if (null != texThumb)
				{
					UnityEngine.Object.Destroy(texThumb);
					texThumb = null;
					isChangeTex = true;
					img.color = Color.black;
				}
			}
		}

		public void UpdateThumb(RawImage img)
		{
			if (show)
			{
				LoadThumb();
			}
			else
			{
				ReleaseThumb(img);
			}
			if (state == State.LOAD_END)
			{
				CreateImage();
			}
			if (isChangeTex)
			{
				img.color = Color.white;
				img.texture = texThumb;
				isChangeTex = false;
			}
		}
	}
}
