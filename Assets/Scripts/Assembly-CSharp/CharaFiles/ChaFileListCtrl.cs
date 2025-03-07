using System;
using FileListUI;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace CharaFiles
{
	public class ChaFileListCtrl : ThreadFileListCtrl<ChaFileInfo, ChaFileInfoComponent>
	{
		protected override Selectable[] addInfos
		{
			get
			{
				return this.GetCache(ref _addInfos, () => new Selectable[0]);
			}
		}

		protected override void Start()
		{
			base.Start();
		}

		public override void Add(ChaFileInfo info)
		{
			base.Add(info);
			info.fic.UpdateInfo();
			UpdateSort();
		}

		public override void Create(Action<ChaFileInfo> onChangeItem, bool reCreate = false)
		{
			base.onChangeItem = onChangeItem;
			base.imgRaycast.Clear();
			objListContent.Children().ForEach(delegate(GameObject go)
			{
				UnityEngine.Object.Destroy(go);
			});
			foreach (ChaFileInfo item in base.lstFileInfo)
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
