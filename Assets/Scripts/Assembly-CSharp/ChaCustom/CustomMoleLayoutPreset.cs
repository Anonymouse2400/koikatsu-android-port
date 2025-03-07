using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace ChaCustom
{
	public class CustomMoleLayoutPreset : MonoBehaviour
	{
		public class MolePreset
		{
			public int index = -1;

			public string name = string.Empty;

			public float x;

			public float y;

			public float w;

			public string thumbAB = string.Empty;

			public string thumb = string.Empty;
		}

		[SerializeField]
		private CustomPushListCtrl listCtrl;

		[SerializeField]
		private CvsMole cvsMole;

		private List<MolePreset> lstPreset = new List<MolePreset>();

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
			Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.mole_layout);
			lstPreset = categoryInfo.Select((KeyValuePair<int, ListInfoBase> dict) => new MolePreset
			{
				index = dict.Value.Id,
				name = dict.Value.Name,
				x = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosX),
				y = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosY),
				w = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Scale),
				thumbAB = dict.Value.GetInfo(ChaListDefine.KeyType.ThumbAB),
				thumb = dict.Value.GetInfo(ChaListDefine.KeyType.ThumbTex)
			}).ToList();
			lstPreset.ForEach(delegate(MolePreset info)
			{
				listCtrl.AddList(info.index, info.name, info.thumbAB, info.thumb);
			});
			listCtrl.Create(OnPush);
		}

		public void OnPush(int index)
		{
			float x = lstPreset[index].x;
			float y = lstPreset[index].y;
			float w = lstPreset[index].w;
			ChaControl chaCtrl = Singleton<CustomBase>.Instance.chaCtrl;
			chaCtrl.chaFile.custom.face.moleLayout = new Vector4(x, y, 0f, w);
			cvsMole.FuncUpdateMoleLayout();
			cvsMole.UpdateCustomUI();
			Singleton<CustomHistory>.Instance.Add1(chaCtrl, cvsMole.FuncUpdateMoleLayout);
		}
	}
}
