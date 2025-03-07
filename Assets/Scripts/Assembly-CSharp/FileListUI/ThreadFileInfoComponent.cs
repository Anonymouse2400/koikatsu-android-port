using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace FileListUI
{
	public class ThreadFileInfoComponent : MonoBehaviour
	{
		private RectTransform _rectTransform;

		public Toggle tgl;

		public RawImage imgThumb;

		public RectTransform rectTransform
		{
			get
			{
				return this.GetComponentCache(ref _rectTransform);
			}
		}

		public ThreadFileInfo fileInfo
		{
			get
			{
				return info;
			}
		}

		private ThreadFileInfo info { get; set; }

		public void SetInfo(ThreadFileInfo info)
		{
			this.info = info;
			info.component = this;
		}

		public void ON()
		{
			if (!(tgl == null))
			{
				tgl.isOn = true;
			}
		}

		public void OFF()
		{
			if (!(tgl == null))
			{
				tgl.isOn = false;
			}
		}

		public virtual void Disable(bool disable)
		{
			if (!(tgl == null))
			{
				tgl.interactable = !disable;
			}
		}

		public void Disable()
		{
			Disable(info.disable);
		}

		public virtual void Disvisible(bool disvisible)
		{
			base.gameObject.SetActiveIfDifferent(!disvisible);
		}

		public void Disvisible()
		{
			Disvisible(info.disvisible);
		}

		public void UpdateInfo()
		{
			Disable();
			Disvisible();
		}
	}
}
