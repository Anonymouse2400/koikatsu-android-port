using System.Linq;
using Illusion.Extensions;
using TMPro;
using UniRx;
using UnityEngine;

namespace ChaCustom
{
	public class CustomAcsChangeSlot : UI_ToggleGroupCtrl
	{
		[SerializeField]
		private CanvasGroup cgAccessoryTop;

		[SerializeField]
		private CustomAcsParentWindow customAcsParentWin;

		[SerializeField]
		private CustomAcsMoveWindow[] customAcsMoveWin;

		[SerializeField]
		private CustomAcsSelectKind[] customAcsSelectKind;

		[SerializeField]
		private TextMeshProUGUI[] textSlotNames;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private CvsAccessory[] cvsAccessory;

		private int backIndex = -1;

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
					if (item.idx < 20)
					{
						bool open = false;
						if (chaCtrl.nowCoordinate.accessory.parts[item.idx].type != 120)
						{
							open = true;
						}
						if (chaCtrl.hideHairAcs[item.idx])
						{
							open = false;
						}
						customAcsParentWin.ChangeSlot(item.idx, open);
						CustomAcsMoveWindow[] array = customAcsMoveWin;
						foreach (CustomAcsMoveWindow customAcsMoveWindow in array)
						{
							customAcsMoveWindow.ChangeSlot(item.idx, open);
						}
						CustomAcsSelectKind[] array2 = this.customAcsSelectKind;
						foreach (CustomAcsSelectKind customAcsSelectKind in array2)
						{
							customAcsSelectKind.ChangeSlot(item.idx, open);
						}
						Singleton<CustomBase>.Instance.selectSlot = item.idx;
						Singleton<CustomBase>.Instance.SetUpdateCvsAccessory(item.idx, true);
						if (backIndex != item.idx)
						{
							ChangeColorWindow(item.idx);
						}
					}
					else if (item.idx == 20)
					{
						CloseWindow();
						Singleton<CustomBase>.Instance.updateCvsAccessoryCopy = true;
					}
					else if (item.idx == 21)
					{
						CloseWindow();
						Singleton<CustomBase>.Instance.updateCvsAccessoryChange = true;
					}
					backIndex = item.idx;
				});
			});
		}

		private void LateUpdate()
		{
			bool[] array = new bool[2];
			if (cgAccessoryTop.alpha == 1f)
			{
				int selectIndex = GetSelectIndex();
				if (selectIndex != -1 && 20 > selectIndex)
				{
					if (cvsAccessory[selectIndex].isController01Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[0])
					{
						array[0] = true;
					}
					if (cvsAccessory[selectIndex].isController02Active && Singleton<CustomBase>.Instance.customSettingSave.drawController[1])
					{
						array[1] = true;
					}
				}
			}
			for (int i = 0; i < 2; i++)
			{
				Singleton<CustomBase>.Instance.customCtrl.cmpGuid[i].gameObject.SetActiveIfDifferent(array[i]);
			}
		}

		public void UpdateSlotNames()
		{
			if (textSlotNames == null)
			{
				return;
			}
			int i;
			for (i = 0; i < 20; i++)
			{
				Singleton<CustomBase>.Instance.FontBind(textSlotNames[i]);
				if (accessory.parts[i].type == 120)
				{
					string format = "スロット{0:00}";
					Singleton<CustomBase>.Instance.TranslateSlotTitle(0).SafeProc(delegate(string text)
					{
						format = text;
					});
					textSlotNames[i].text = string.Format(format, i + 1);
				}
				else
				{
					chaCtrl.infoAccessory.SafeProc(i, delegate(ListInfoBase info)
					{
						textSlotNames[i].text = info.Name;
					});
				}
			}
		}

		public void CloseWindow()
		{
			customAcsParentWin.CloseWindow();
			CustomAcsMoveWindow[] array = customAcsMoveWin;
			foreach (CustomAcsMoveWindow customAcsMoveWindow in array)
			{
				customAcsMoveWindow.CloseWindow();
			}
			CustomAcsSelectKind[] array2 = this.customAcsSelectKind;
			foreach (CustomAcsSelectKind customAcsSelectKind in array2)
			{
				customAcsSelectKind.CloseWindow();
			}
			cvsColor.Close();
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
			if (no < 20)
			{
				if ((bool)cvsAccessory[no])
				{
					cvsAccessory[no].SetDefaultColorWindow(no);
				}
			}
			else
			{
				cvsColor.Close();
			}
		}
	}
}
