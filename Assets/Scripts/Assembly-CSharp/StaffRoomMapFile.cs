using System.Collections.Generic;
using System.Linq;
using Illusion.Game;
using Localize.Translate;
using StaffRoom;
using UnityEngine;

public class StaffRoomMapFile : MonoBehaviour
{
	[SerializeField]
	private StaffRoomMenuScene staffroomoscene;

	[SerializeField]
	private StaffRoomMapListCtrl listCtrl;

	private void Start()
	{
		List<MapInfo.Param> list = staffroomoscene.baseMap.infoDic.Values.Where((MapInfo.Param m) => m.isGate).ToList();
		Dictionary<int, Data.Param> self = base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.MAP).Get(0);
		listCtrl.ClearList();
		int count = list.Count;
		int num = 0;
		Dictionary<int, MapInfo.Param> chaFileDic = new Dictionary<int, MapInfo.Param>();
		for (int i = 0; i < count; i++)
		{
			MapInfo.Param param2 = list[i];
			string bundle = param2.ThumbnailBundle;
			string asset = param2.ThumbnailAsset;
			self.SafeGet(param2.No).SafeProc(delegate(Data.Param param)
			{
				bundle = param.Bundle;
				asset = param.asset;
			});
			listCtrl.AddList(num, bundle, asset);
			chaFileDic.Add(num, param2);
			num++;
		}
		listCtrl.OnPointerClick += delegate(MapFileInfo info)
		{
			staffroomoscene.SetMapInfo((info != null) ? chaFileDic[info.index] : null);
			Utils.Sound.Play(SystemSE.sel);
		};
		listCtrl.Create(delegate
		{
		});
	}
}
