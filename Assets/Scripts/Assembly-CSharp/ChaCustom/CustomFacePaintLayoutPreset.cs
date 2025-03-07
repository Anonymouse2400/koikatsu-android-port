using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace ChaCustom
{
	public class CustomFacePaintLayoutPreset : MonoBehaviour
	{
		public class FacePaintPreset
		{
			public int index = -1;

			public string name = string.Empty;

			public float x;

			public float y;

			public float r;

			public float s;

			public string thumbAB = string.Empty;

			public string thumb = string.Empty;
		}

		[SerializeField]
		private CustomPushListCtrl listCtrl;

		[SerializeField]
		private CvsMakeup cvsMakeup;

		private List<FacePaintPreset> lstPreset = new List<FacePaintPreset>();

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
			Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(ChaListDefine.CategoryNo.facepaint_layout);
			lstPreset = categoryInfo.Select((KeyValuePair<int, ListInfoBase> dict) => new FacePaintPreset
			{
				index = dict.Value.Id,
				name = dict.Value.Name,
				x = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosX),
				y = dict.Value.GetInfoFloat(ChaListDefine.KeyType.PosY),
				r = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Rot),
				s = dict.Value.GetInfoFloat(ChaListDefine.KeyType.Scale),
				thumbAB = dict.Value.GetInfo(ChaListDefine.KeyType.ThumbAB),
				thumb = dict.Value.GetInfo(ChaListDefine.KeyType.ThumbTex)
			}).ToList();
			lstPreset.ForEach(delegate(FacePaintPreset info)
			{
				listCtrl.AddList(info.index, info.name, info.thumbAB, info.thumb);
			});
			listCtrl.Create(OnPush);
		}

		public void OnPush(int index)
		{
			float x = lstPreset[index].x;
			float y = lstPreset[index].y;
			float r = lstPreset[index].r;
			float s = lstPreset[index].s;
			Vector4 layout = new Vector4(x, y, r, s);
			cvsMakeup.UpdatePushFacePaintLayout(layout);
		}
	}
}
