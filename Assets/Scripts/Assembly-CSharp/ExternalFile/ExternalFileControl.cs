using UniRx;
using UnityEngine;

namespace ExternalFile
{
	public class ExternalFileControl : MonoBehaviour
	{
		public ExternalFileListCtrl listCtrl;

		public ExternalFileWindow fileWindow;

		private void Start()
		{
			Initialize();
			if ((bool)fileWindow && (bool)fileWindow.btnLoad)
			{
				fileWindow.btnLoad.OnClickAsObservable().Subscribe(delegate
				{
				});
			}
		}

		private void Initialize()
		{
			FolderAssist folderAssist = new FolderAssist();
			string[] searchPattern = new string[2] { "*.png", "*.jpg" };
			string empty = string.Empty;
			folderAssist.CreateFolderInfoEx(empty, searchPattern);
			listCtrl.ClearList();
			int fileCount = folderAssist.GetFileCount();
			int num = 0;
			for (int i = 0; i < fileCount; i++)
			{
				listCtrl.AddList(num, folderAssist.lstFile[i].FullPath, folderAssist.lstFile[i].FileName, folderAssist.lstFile[i].time);
				num++;
			}
			listCtrl.Create(OnChangeSelect);
		}

		public void OnChangeSelect(ExternalFileInfo info)
		{
		}

		private void Update()
		{
		}
	}
}
