using System.Collections.Generic;
using System.Linq;
using Manager;
using UnityEngine;

namespace ChaCustom
{
	public class CustomAcsSelectKind : MonoBehaviour
	{
		public ChaListDefine.CategoryNo cate = ChaListDefine.CategoryNo.ao_hair;

		[SerializeField]
		private CustomSelectWindow selWin;

		[SerializeField]
		private CustomSelectListCtrl listCtrl;

		[SerializeField]
		private CvsAccessory[] cvsAccessory;

		public int slotNo;

		private ChaControl chaCtrl
		{
			get
			{
				return Singleton<CustomBase>.Instance.chaCtrl;
			}
		}

		private ChaFileAccessory accessory
		{
			get
			{
				return chaCtrl.nowCoordinate.accessory;
			}
		}

		private void Start()
		{
			Initialize();
		}

		private void Initialize()
		{
			ChaListControl chaListCtrl = Singleton<Character>.Instance.chaListCtrl;
			Dictionary<int, ListInfoBase> categoryInfo = chaListCtrl.GetCategoryInfo(cate);
			List<ListInfoBase> list = categoryInfo.Values.ToList();
			list.ForEach(delegate(ListInfoBase info)
			{
				listCtrl.AddList(info.Category, info.Id, info.Name, info.GetInfo(ChaListDefine.KeyType.ThumbAB), info.GetInfo(ChaListDefine.KeyType.ThumbTex));
			});
			listCtrl.Create(OnSelect);
		}

		public void ChangeSlot(int _no, bool open)
		{
			slotNo = _no;
			bool isOn = selWin.tglReference.isOn;
			selWin.tglReference.isOn = false;
			selWin.tglReference = cvsAccessory[slotNo].tglAcsKind;
			if (open && isOn)
			{
				selWin.tglReference.isOn = true;
			}
		}

		public void CloseWindow()
		{
			selWin.tglReference.isOn = false;
		}

		public void UpdateCustomUI(int param = 0)
		{
			listCtrl.SelectItem(accessory.parts[slotNo].id);
		}

		public void OnSelect(int index)
		{
			CustomSelectInfo selectInfoFromIndex = listCtrl.GetSelectInfoFromIndex(index);
			if (selectInfoFromIndex != null)
			{
				cvsAccessory[slotNo].UpdateSelectAccessoryKind(selectInfoFromIndex.name, selectInfoFromIndex.sic.img.sprite, index);
			}
		}
	}
}
