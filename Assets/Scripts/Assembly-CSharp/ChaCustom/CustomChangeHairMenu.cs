using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomChangeHairMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private GameObject objCommon;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private CvsHair[] cvsHair;

		private int backIndex = -1;

		public override void Start()
		{
			base.Start();
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
					localPosition.y = -120 + 40 * posCnt;
					if (item.idx == 4)
					{
						localPosition.y = 40 * posCnt;
					}
					item.val.cgItem.transform.localPosition = localPosition;
					posCnt++;
				}
				(from isOn in item.val.tglItem.OnValueChangedAsObservable()
					where isOn
					select isOn).Subscribe(delegate
				{
					if ((bool)objCommon)
					{
						objCommon.SetActiveIfDifferent(4 != item.idx);
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
			switch (no)
			{
			case 4:
				cvsColor.Close();
				break;
			case 0:
			case 1:
			case 2:
			case 3:
				if ((bool)cvsHair[no])
				{
					cvsHair[no].SetDefaultColorWindow(no);
				}
				break;
			}
		}
	}
}
