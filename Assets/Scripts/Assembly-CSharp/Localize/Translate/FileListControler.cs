using System.Linq;
using ChaCustom;
using FileListUI;

namespace Localize.Translate
{
	public static class FileListControler
	{
		public static void Execute(int sex, Manager.ChaFileInfo[] fileInfo, CustomFileListCtrl listCtrl)
		{
			listCtrl.ClearList();
			if (sex == 0)
			{
				listCtrl.visibleType = VisibleType.AddHide;
				{
					foreach (var item in fileInfo.Select((Manager.ChaFileInfo p, int index) => new { p, index }))
					{
						Manager.ChaFileInfo p2 = item.p;
						ChaFileParameter parameter = p2.chaFile.parameter;
						FolderAssist.FileInfo info = p2.info;
						listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(info))
						{
							index = item.index,
							name = parameter.fullname,
							isDefaultData = p2.isDefault
						});
					}
					return;
				}
			}
			foreach (var item2 in fileInfo.Select((Manager.ChaFileInfo p, int index) => new { p, index }))
			{
				Manager.ChaFileInfo p3 = item2.p;
				ChaFileParameter parameter2 = p3.chaFile.parameter;
				FolderAssist.FileInfo info2 = p3.info;
				listCtrl.AddList(new CustomFileInfo(new FolderAssist.FileInfo(info2))
				{
					index = item2.index,
					name = parameter2.fullname,
					club = Manager.GetClubName(parameter2.clubActivities, false),
					personality = Manager.GetPersonalityName(parameter2.personality, false),
					isDefaultData = p3.isDefault
				});
			}
		}
	}
}
