using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace FileListUI
{
	public class ThreadFileInfo : FolderAssist.FileInfo
	{
		protected enum State
		{
			NO_TEX = 0,
			LOADING = 1,
			LOAD_END = 2,
			HAS_TEX = 3
		}

		public int index { get; set; }

		public string name { get; set; }

		public bool disable { get; set; }

		public bool disvisible { get; set; }

		public ThreadFileInfoComponent component { get; set; }

		public bool show { get; set; }

		protected State state { get; set; }

		protected bool isChangeTex { get; set; }

		protected Thread thread { get; set; }

		protected byte[] bytes { get; set; }

		protected Texture2D texThumb { get; set; }

		public ThreadFileInfo(FolderAssist.FileInfo info)
			: base(info)
		{
			name = string.Empty;
		}

		public virtual void UpdateThumb(RawImage img)
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

		public void UpdateInfo(DateTime? time)
		{
			if (time.HasValue)
			{
				UpdateTime(time.Value);
			}
			DeleteThumb();
		}

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
			state = State.NO_TEX;
		}

		protected void LoadThumb()
		{
			if (state == State.NO_TEX && null == texThumb && (thread == null || !thread.IsAlive))
			{
				state = State.LOADING;
				thread = new Thread(LoadImageBytes);
				thread.Start();
			}
		}

		protected void LoadImageBytes()
		{
			bytes = PngFile.LoadPngBytes(base.FullPath);
			if (bytes == null)
			{
				state = State.NO_TEX;
			}
			else
			{
				state = State.LOAD_END;
			}
		}

		protected void CreateImage()
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

		protected void ReleaseThumb(RawImage img)
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
	}
}
