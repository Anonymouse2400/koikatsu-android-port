using FileListUI;
using UnityEngine;
using UnityEngine.UI;

namespace StaffRoom
{
	public class FileInfo : ThreadFileInfo
	{
		private StaffRoomInfoComponent _fic;

		public StaffRoomInfoComponent fic
		{
			get
			{
				return _fic ?? (_fic = base.component as StaffRoomInfoComponent);
			}
		}

		public byte[] dataPng { get; set; }

		public FileInfo(FolderAssist.FileInfo info)
			: base(info)
		{
		}

		public override void UpdateThumb(RawImage img)
		{
			if (base.show)
			{
				if (dataPng == null)
				{
					LoadThumb();
				}
				else if (base.state == State.NO_TEX)
				{
					base.bytes = dataPng;
					base.state = State.LOAD_END;
				}
			}
			else
			{
				ReleaseThumb(img);
			}
			if (base.state == State.LOAD_END)
			{
				CreateImage();
			}
			if (base.isChangeTex)
			{
				img.color = ((!fic || !fic.tgl || fic.tgl.interactable) ? Color.white : Color.gray);
				img.texture = base.texThumb;
				base.isChangeTex = false;
			}
		}
	}
}
