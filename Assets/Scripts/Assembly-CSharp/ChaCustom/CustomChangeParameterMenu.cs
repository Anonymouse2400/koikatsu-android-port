using System.Linq;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomChangeParameterMenu : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private GameObject objCharacter;

		[SerializeField]
		private GameObject objCharacterEx;

		[SerializeField]
		private GameObject objH;

		[SerializeField]
		private GameObject objQA;

		[SerializeField]
		private GameObject objAttribute;

		[SerializeField]
		private GameObject objADK;

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		public override void Start()
		{
			if (customBase.chaCtrl.sex == 0)
			{
				if ((bool)objCharacter)
				{
					objCharacter.GetComponent<Toggle>().isOn = true;
				}
				if ((bool)objCharacterEx)
				{
					objCharacterEx.SetActiveIfDifferent(false);
					objCharacterEx.GetComponent<Toggle>().isOn = false;
				}
				if ((bool)objH)
				{
					objH.SetActiveIfDifferent(false);
				}
				if ((bool)objQA)
				{
					objQA.SetActiveIfDifferent(false);
				}
				if ((bool)objAttribute)
				{
					objAttribute.SetActiveIfDifferent(false);
				}
			}
			else
			{
				if (!customBase.modeNew)
				{
					if ((bool)objCharacter)
					{
						objCharacter.SetActiveIfDifferent(false);
					}
					if ((bool)objH)
					{
						objH.SetActiveIfDifferent(false);
					}
					if ((bool)objQA)
					{
						objQA.SetActiveIfDifferent(false);
					}
				}
				if ((bool)objCharacterEx)
				{
					bool flag = !customBase.modeNew;
					objCharacterEx.SetActiveIfDifferent(flag);
					if ((bool)objCharacter)
					{
						objCharacter.GetComponent<Toggle>().isOn = !flag;
					}
					if ((bool)objCharacterEx)
					{
						objCharacterEx.GetComponent<Toggle>().isOn = flag;
					}
				}
				if ((bool)objADK)
				{
					objADK.SetActiveIfDifferent(false);
				}
			}
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
			if ((bool)cvsColor)
			{
				cvsColor.Close();
			}
		}
	}
}
