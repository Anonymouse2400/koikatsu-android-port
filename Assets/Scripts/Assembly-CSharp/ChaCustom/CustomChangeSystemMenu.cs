using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomChangeSystemMenu : UI_ToggleGroupCtrl
	{
		public CustomFileWindow[] fileWindow;

		private CustomFileWindow.FileWindowType[] types = new CustomFileWindow.FileWindowType[4]
		{
			CustomFileWindow.FileWindowType.CharaLoad,
			CustomFileWindow.FileWindowType.CharaSave,
			CustomFileWindow.FileWindowType.CoordinateLoad,
			CustomFileWindow.FileWindowType.CoordinateSave
		};

		[SerializeField]
		private CustomCheckWindow checkWindow;

		[SerializeField]
		private CvsColor cvsColor;

		[SerializeField]
		private Button btnCapFaceOnly;

		[SerializeField]
		private Button btnRestore;

		public override void Start()
		{
			base.Start();
			int PosCnt = 0;
			bool flag = !Singleton<CustomBase>.Instance.modeNew;
			btnCapFaceOnly.gameObject.SetActiveIfDifferent(flag);
			btnRestore.gameObject.SetActiveIfDifferent(flag);
			PosCnt = (flag ? 2 : 0);
			if (items != null && fileWindow != null)
			{
				int posCnt = 0;
				(from item in items.Select((ItemInfo val, int idx) => new { val, idx })
					where item.val != null && item.val.tglItem != null
					select item).ToList().ForEach(item =>
				{
					if (item.idx == 4 && item.val.cgItem.gameObject.activeInHierarchy)
					{
						Vector3 localPosition = item.val.cgItem.transform.localPosition;
						localPosition.y = 160 + 40 * PosCnt;
						item.val.cgItem.transform.localPosition = localPosition;
						posCnt++;
					}
					(from isOn in item.val.tglItem.OnValueChangedAsObservable()
						where isOn
						select isOn).Subscribe(delegate
					{
						fileWindow.SafeProc(item.idx, delegate(CustomFileWindow window)
						{
							window.fwType = types[item.idx];
						});
					});
				});
			}
			string question = Singleton<CustomBase>.Instance.translateQuestionTitle.Values.FindTagText("Restore");
			btnRestore.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
				checkWindow.Setup(CustomCheckWindow.CheckType.YesNo, question ?? "キャラを編集前に戻しますか？", null, null, RestoreCharacter, null);
			});
			btnCapFaceOnly.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
				Singleton<CustomBase>.Instance.customCtrl.capFaceOnly = true;
				Singleton<CustomBase>.Instance.customCtrl.saveMode = true;
			});
		}

		public void RestoreCharacter(string buf)
		{
			ChaControl chaCtrl = Singleton<CustomBase>.Instance.chaCtrl;
			chaCtrl.chaFile.CopyAll(Singleton<CustomBase>.Instance.customCtrl.backChaFileCtrl);
			chaCtrl.ChangeCoordinateType();
			chaCtrl.Reload();
			if (chaCtrl.sex == 1)
			{
				chaCtrl.fileStatus.visibleSonAlways = false;
			}
			else
			{
				chaCtrl.fileStatus.visibleSon = false;
			}
			Singleton<CustomBase>.Instance.updateCustomUI = true;
			Singleton<CustomHistory>.Instance.Add5(chaCtrl, chaCtrl.Reload, false, false, false, false);
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
