using System;
using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomChangeClothesMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CvsDrawCtrl cvsDrawCtrl;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private CvsClothes[] cvsClothes;

		private int backIndex = -1;

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		public override void Start()
		{
			base.Start();
			if (customBase.chaCtrl.sex == 0 && items.Any())
			{
				items[2].tglItem.gameObject.SetActiveIfDifferent(customBase.IsMaleCoordinateBra());
			}
			if (!items.Any())
			{
				return;
			}
			int posCnt = 0;
			(from item in items.Select((ItemInfo val, int idx) => new { val, idx })
				where item.val != null && item.val.tglItem != null
				select item).ToList().ForEach(item =>
			{
				if (item.val.cgItem.gameObject.activeInHierarchy)
				{
					Vector3 localPosition = item.val.cgItem.transform.localPosition;
					localPosition.y = 40 * posCnt;
					item.val.cgItem.transform.localPosition = localPosition;
					posCnt++;
				}
				(from isOn in item.val.tglItem.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					switch (item.idx)
					{
					case 0:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
						break;
					case 1:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
						break;
					case 2:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(1);
						break;
					case 3:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(1);
						break;
					case 4:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(1);
						break;
					case 5:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(1);
						break;
					case 6:
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(1);
						break;
					case 7:
						if (cvsDrawCtrl.tglShoesType != null)
						{
							cvsDrawCtrl.tglShoesType[1].isOn = false;
							cvsDrawCtrl.tglShoesType[0].isOn = true;
						}
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
						break;
					case 8:
						if (cvsDrawCtrl.tglShoesType != null)
						{
							cvsDrawCtrl.tglShoesType[0].isOn = false;
							cvsDrawCtrl.tglShoesType[1].isOn = true;
						}
						Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
						break;
					case 9:
						Singleton<CustomBase>.Instance.updateCvsClothesCopy = true;
						break;
					}
					if (backIndex != item.idx)
					{
						ChangeColorWindow(item.idx);
					}
					backIndex = item.idx;
				});
			});
		}

		public void ChangeColorWindow()
		{
			int selectIndex = GetSelectIndex();
			if (selectIndex != -1)
			{
				ChangeColorWindow(selectIndex);
			}
		}

		public void ChangeColorWindow(int no)
		{
			if (null == cvsColor || !cvsColor.isOpen)
			{
				return;
			}
			if (no < Enum.GetNames(typeof(ChaFileDefine.ClothesKind)).Length)
			{
				if ((bool)cvsClothes[no])
				{
					cvsClothes[no].SetDefaultColorWindow(no);
				}
			}
			else
			{
				cvsColor.Close();
			}
		}
	}
}
