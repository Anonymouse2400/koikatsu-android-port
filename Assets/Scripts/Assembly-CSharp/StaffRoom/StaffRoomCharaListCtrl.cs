using System;
using FileListUI;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace StaffRoom
{
	public class StaffRoomCharaListCtrl : ThreadFileListCtrl<FileInfo, StaffRoomInfoComponent>
	{
		protected override Selectable[] addInfos
		{
			get
			{
				return this.GetCache(ref _addInfos, () => new Selectable[0]);
			}
		}

		public event Action<FileInfo> eventOnPointerClick = delegate
		{
		};

		public event Action<FileInfo> eventOnPointerEnter = delegate
		{
		};

		public event Action<FileInfo> eventOnPointerExit = delegate
		{
		};

		private static FileInfo GetSelect(StaffRoomInfoComponent fic)
		{
			return (!fic.tgl.isOn) ? null : fic.info;
		}

		public override bool OnPointerClick(StaffRoomInfoComponent fic)
		{
			if (!base.OnPointerClick(fic))
			{
				return false;
			}
			this.eventOnPointerClick(GetSelect(fic));
			return true;
		}

		public override bool OnPointerEnter(StaffRoomInfoComponent fic)
		{
			if (!base.OnPointerEnter(fic))
			{
				return false;
			}
			this.eventOnPointerEnter(GetSelect(fic));
			return true;
		}

		public override bool OnPointerExit(StaffRoomInfoComponent fic)
		{
			if (!base.OnPointerExit(fic))
			{
				return false;
			}
			this.eventOnPointerExit(GetSelect(fic));
			return true;
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void Add(FileInfo info)
		{
			base.Add(info);
		}

		public override void Create(Action<FileInfo> onChangeItem, bool reCreate = false)
		{
			base.onChangeItem = onChangeItem;
			base.imgRaycast.Clear();
			objListContent.Children().ForEach(delegate(GameObject go)
			{
				UnityEngine.Object.Destroy(go);
			});
			foreach (FileInfo item in base.lstFileInfo)
			{
				item.DeleteThumb();
				CreateCloneBinding(item);
				item.fic.UpdateInfo();
			}
			if (reCreate)
			{
				UpdateSort();
			}
			else
			{
				SortName(true);
				SortDate(false);
			}
			ToggleAllOff();
		}
	}
}
