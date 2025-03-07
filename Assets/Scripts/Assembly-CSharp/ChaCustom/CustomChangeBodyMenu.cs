using System.Linq;
using Illusion.Extensions;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomChangeBodyMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private CvsBodyAll cvsBodyAll;

		[SerializeField]
		private CvsBreast cvsBreast;

		[SerializeField]
		private CvsNail cvsNail;

		[SerializeField]
		private CvsUnderhair cvsUnderhair;

		[SerializeField]
		private CvsSunburn cvsSunburn;

		[SerializeField]
		private CvsBodyPaint cvsBodyPaint;

		[SerializeField]
		private GameObject objUnderhair;

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
			if (customBase.chaCtrl.sex == 0 && (bool)objUnderhair)
			{
				objUnderhair.SetActiveIfDifferent(false);
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
					if (backIndex != item.idx)
					{
						ChangeMenuProc(item.idx);
					}
					backIndex = item.idx;
				});
			});
		}

		public void ChangeMenuProc()
		{
			int selectIndex = GetSelectIndex();
			if (selectIndex != -1)
			{
				ChangeMenuProc(selectIndex);
			}
		}

		public void ChangeMenuProc(int no)
		{
			switch (no)
			{
			case 0:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsBodyAll)
				{
					cvsBodyAll.SetDefaultColorWindow();
				}
				customBase.updateCvsBodyAll = true;
				break;
			case 1:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsBreast)
				{
					cvsBreast.SetDefaultColorWindow();
				}
				customBase.updateCvsBreast = true;
				break;
			case 2:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsBodyUpper = true;
				break;
			case 3:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsBodyLower = true;
				break;
			case 4:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsArm = true;
				break;
			case 5:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsLeg = true;
				break;
			case 6:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsNail)
				{
					cvsNail.SetDefaultColorWindow();
				}
				break;
			case 7:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsUnderhair)
				{
					cvsUnderhair.SetDefaultColorWindow();
				}
				break;
			case 8:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsSunburn)
				{
					cvsSunburn.SetDefaultColorWindow();
				}
				break;
			case 9:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsBodyPaint)
				{
					cvsBodyPaint.SetDefaultColorWindow();
				}
				break;
			case 10:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsBodyShapeAll = true;
				break;
			}
		}
	}
}
