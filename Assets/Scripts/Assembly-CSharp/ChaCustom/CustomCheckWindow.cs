using System;
using System.Linq;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomCheckWindow : MonoBehaviour
	{
		public enum CheckType
		{
			CoordinateInput = 0,
			YesNo = 1,
			CharaOverwrite = 2,
			ExitEditScene = 3
		}

		private CheckType checkType;

		[SerializeField]
		private GameObject objCheckCanvas;

		[SerializeField]
		private CustomCheckInfoComponent[] checkInfo;

		private SystemSE[,] seKind = new SystemSE[4, 3]
		{
			{
				SystemSE.ok_s,
				SystemSE.cancel,
				SystemSE.ok_s
			},
			{
				SystemSE.ok_s,
				SystemSE.cancel,
				SystemSE.ok_s
			},
			{
				SystemSE.window_o,
				SystemSE.ok_s,
				SystemSE.cancel
			},
			{
				SystemSE.ok_s,
				SystemSE.ok_l,
				SystemSE.cancel
			}
		};

		private Action<string>[] actBtn;

		public void ChangeCheckType(CheckType type)
		{
			checkType = type;
			for (int i = 0; i < checkInfo.Length; i++)
			{
				checkInfo[i].objRoot.SetActiveIfDifferent(i == (int)type);
			}
		}

		public void Setup(CheckType type, string strMainMsg, string strSubMsg, string strInput, params Action<string>[] act)
		{
			ChangeCheckType(type);
			if ((bool)checkInfo[(int)type].textMain && strMainMsg != null)
			{
				checkInfo[(int)type].textMain.text = strMainMsg;
			}
			if ((bool)checkInfo[(int)type].textSub && strSubMsg != null)
			{
				checkInfo[(int)type].textSub.text = strSubMsg;
			}
			if ((bool)checkInfo[(int)type].inpName && strInput != null)
			{
				checkInfo[(int)type].inpName.text = strInput;
			}
			actBtn = act;
			objCheckCanvas.SetActiveIfDifferent(true);
			if ((bool)checkInfo[(int)type].inpName && strInput != null)
			{
				checkInfo[(int)type].inpName.ActivateInputField();
			}
		}

		private void Start()
		{
			for (int i = 0; i < checkInfo.Length; i++)
			{
				if ((bool)checkInfo[i].inpName)
				{
					Singleton<CustomBase>.Instance.lstInputField.Add(checkInfo[i].inpName);
				}
			}
			for (int j = 0; j < checkInfo.Length; j++)
			{
				(from item in checkInfo[j].btn.Select((Button btn, int idx) => new { btn, idx })
					where item.btn
					select item).ToList().ForEach(item =>
				{
					item.btn.OnClickAsObservable().Subscribe(delegate
					{
						string arg = ((!checkInfo[(int)checkType].inpName) ? null : checkInfo[(int)checkType].inpName.text);
						if (item.idx < actBtn.Length)
						{
							actBtn[item.idx].Call(arg);
						}
						objCheckCanvas.SetActiveIfDifferent(false);
						Utils.Sound.Play(seKind[(int)checkType, item.idx]);
					});
				});
			}
			(from _ in this.UpdateAsObservable()
				where Input.GetMouseButtonDown(1)
				where !Singleton<Scene>.Instance.IsNowLoadingFade
				select _).Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.cancel);
				string arg2 = ((!checkInfo[(int)checkType].inpName) ? null : checkInfo[(int)checkType].inpName.text);
				switch (checkType)
				{
				case CheckType.CoordinateInput:
					actBtn[1].Call(arg2);
					break;
				case CheckType.YesNo:
					actBtn[1].Call(arg2);
					break;
				case CheckType.CharaOverwrite:
					actBtn[2].Call(arg2);
					break;
				case CheckType.ExitEditScene:
					actBtn[2].Call(arg2);
					break;
				}
				objCheckCanvas.SetActiveIfDifferent(false);
			});
		}
	}
}
