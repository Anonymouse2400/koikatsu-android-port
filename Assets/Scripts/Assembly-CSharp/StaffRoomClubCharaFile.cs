using System.Linq;
using ActionGame.Chara;
using Illusion.Game;
using Manager;
using StaffRoom;
using UnityEngine;

public class StaffRoomClubCharaFile : MonoBehaviour
{
	[SerializeField]
	private StaffRoomMenuScene staffroomoscene;

	[SerializeField]
	private StaffRoomCharaListCtrl listCtrl;

	private void Start()
	{
		SaveData.Heroine[] heroines = (from p in Singleton<Game>.Instance.actScene.npcList
			select p.heroine into p
			where p != null
			where p.isStaff
			select p).ToArray();
		listCtrl.ClearList();
		foreach (var item in heroines.Select((SaveData.Heroine heroine, int index) => new { heroine, index }))
		{
			SaveData.Heroine heroine2 = item.heroine;
			bool disable = heroine2.isAnger || heroine2.talkTime <= 0;
			ChaFileControl charFile = heroine2.charFile;
			FolderAssist.FileInfo info2 = new FolderAssist.FileInfo(charFile.ConvertCharaFilePath(charFile.charaFileName, 1), charFile.charaFileName, null);
			listCtrl.AddList(new FileInfo(info2)
			{
				index = item.index,
				name = heroine2.Name,
				dataPng = charFile.facePngData,
				disable = disable
			});
		}
		listCtrl.eventOnPointerClick += delegate(FileInfo info)
		{
			staffroomoscene.SetHeroine((info != null) ? heroines[info.index] : null);
			Utils.Sound.Play(SystemSE.sel);
		};
		listCtrl.Create(null);
	}
}
