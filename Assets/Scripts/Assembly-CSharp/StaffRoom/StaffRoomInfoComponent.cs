using FileListUI;
using UnityEngine;

namespace StaffRoom
{
	public class StaffRoomInfoComponent : ThreadFileInfoComponent
	{
		private FileInfo _info;

		public FileInfo info
		{
			get
			{
				return this.GetCache(ref _info, () => base.fileInfo as FileInfo);
			}
		}

		public override void Disable(bool disable)
		{
			base.Disable(disable);
			if (imgThumb != null)
			{
				imgThumb.color = Color.gray;
			}
		}
	}
}
