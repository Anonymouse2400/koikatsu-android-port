using System.Linq;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomChangeFaceMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private CvsEyebrow cvsEyebrow;

		[SerializeField]
		private CvsEye01 cvsEye01;

		[SerializeField]
		private CvsEye02 cvsEye02;

		[SerializeField]
		private CvsMouth cvsMouth;

		[SerializeField]
		private CvsMole cvsMole;

		[SerializeField]
		private CvsMakeup cvsMakeup;

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
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsFaceAll = true;
				break;
			case 1:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsEar = true;
				break;
			case 2:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsChin = true;
				break;
			case 3:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsCheek = true;
				break;
			case 4:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsEyebrow)
				{
					cvsEyebrow.SetDefaultColorWindow();
				}
				customBase.updateCvsEyebrow = true;
				break;
			case 5:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsEye01)
				{
					cvsEye01.SetDefaultColorWindow();
				}
				customBase.updateCvsEye01 = true;
				break;
			case 6:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsEye02)
				{
					cvsEye02.SetDefaultColorWindow();
				}
				break;
			case 7:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsNose = true;
				break;
			case 8:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsMouth)
				{
					cvsMouth.SetDefaultColorWindow();
				}
				customBase.updateCvsMouth = true;
				break;
			case 9:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsMole)
				{
					cvsMole.SetDefaultColorWindow();
				}
				break;
			case 10:
				if (null != cvsColor && cvsColor.isOpen && (bool)cvsMakeup)
				{
					cvsMakeup.SetDefaultColorWindow();
				}
				break;
			case 11:
				if (null != cvsColor && cvsColor.isOpen)
				{
					cvsColor.Close();
				}
				customBase.updateCvsFaceShapeAll = true;
				break;
			}
		}
	}
}
