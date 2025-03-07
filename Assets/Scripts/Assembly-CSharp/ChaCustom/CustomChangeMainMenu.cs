using System.Linq;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomChangeMainMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CustomChangeFaceMenu ccFaceMenu;

		[SerializeField]
		private CustomChangeBodyMenu ccBodyMenu;

		[SerializeField]
		private CustomChangeHairMenu ccHairMenu;

		[SerializeField]
		private CustomChangeClothesMenu ccClothesMenu;

		[SerializeField]
		private CustomAcsChangeSlot ccAcsMenu;

		[SerializeField]
		private CustomChangeParameterMenu ccParameterMenu;

		[SerializeField]
		private CustomChangeSystemMenu ccSystemMenu;

		public override void Start()
		{
			base.Start();
			if (!items.Any())
			{
				return;
			}
			(from item in items.Select((ItemInfo val, int idx) => new { val, idx })
				where item.val != null && item.val.tglItem != null
				select item).ToList().ForEach(item =>
			{
				(from isOn in item.val.tglItem.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					ChangeWindowSetting(item.idx);
				});
			});
		}

		public void ChangeWindowSetting(int no)
		{
			switch (no)
			{
			case 0:
				if ((bool)ccFaceMenu)
				{
					ccFaceMenu.ChangeMenuProc();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			case 1:
				if ((bool)ccBodyMenu)
				{
					ccBodyMenu.ChangeMenuProc();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(2);
				}
				break;
			case 2:
				if ((bool)ccHairMenu)
				{
					ccHairMenu.ChangeColorWindow();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			case 3:
				if ((bool)ccClothesMenu)
				{
					ccClothesMenu.ChangeColorWindow();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			case 4:
				if ((bool)ccAcsMenu)
				{
					ccAcsMenu.ChangeColorWindow();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			case 5:
				if ((bool)ccParameterMenu)
				{
					ccParameterMenu.ChangeColorWindow();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			case 6:
				if ((bool)ccSystemMenu)
				{
					ccSystemMenu.ChangeColorWindow();
					Singleton<CustomBase>.Instance.ChangeClothesStateAuto(0);
				}
				break;
			}
		}
	}
}
