using Illusion.Game;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CvsExit : MonoBehaviour
	{
		[SerializeField]
		private CustomCheckWindow checkWindow;

		[SerializeField]
		private Button btnExit;

		private CustomBase customBase
		{
			get
			{
				return Singleton<CustomBase>.Instance;
			}
		}

		private ChaControl chaCtrl
		{
			get
			{
				return customBase.chaCtrl;
			}
		}

		private void Start()
		{
			if (!btnExit)
			{
				return;
			}
			string[] questions = Singleton<CustomBase>.Instance.translateQuestionTitle.Values.ToArray("CvsExit");
			btnExit.OnClickAsObservable().Subscribe(delegate
			{
				Utils.Sound.Play(SystemSE.window_o);
				string title = null;
				questions.SafeProc((!customBase.modeNew) ? 1 : 0, delegate(string text)
				{
					title = text;
				});
				if (customBase.modeNew)
				{
					checkWindow.Setup(CustomCheckWindow.CheckType.YesNo, title ?? "キャラメイクを終了しますか？", null, string.Empty, ExitScene, null);
				}
				else
				{
					checkWindow.Setup(CustomCheckWindow.CheckType.ExitEditScene, title ?? "キャラ編集を終了しますか？", null, string.Empty, ExitSceneRestoreStatus, ExitSceneRestoreAll, null);
				}
			});
		}

		public void ExitScene(string strInput)
		{
			customBase.customCtrl.cvsChangeScene.gameObject.SetActive(true);
			customBase.customSettingSave.Save();
			Singleton<Scene>.Instance.UnLoad();
		}

		public void ExitSceneRestoreAll(string strInput)
		{
			customBase.customCtrl.cvsChangeScene.gameObject.SetActive(true);
			customBase.customSettingSave.Save();
			Singleton<Character>.Instance.editChara = customBase.customCtrl.backChaFileCtrl;
			Singleton<Scene>.Instance.UnLoad();
		}

		public void ExitSceneRestoreStatus(string strInput)
		{
			customBase.customCtrl.cvsChangeScene.gameObject.SetActive(true);
			customBase.customSettingSave.Save();
			Singleton<Character>.Instance.editChara = new ChaFileControl();
			ChaFile.CopyChaFile(Singleton<Character>.Instance.editChara, chaCtrl.chaFile);
			Singleton<Character>.Instance.editChara.facePngData = chaCtrl.chaFile.facePngData;
			Singleton<Character>.Instance.editChara.pngData = chaCtrl.chaFile.pngData;
			byte[] statusBytes = customBase.customCtrl.backChaFileCtrl.GetStatusBytes();
			Singleton<Character>.Instance.editChara.SetStatusBytes(statusBytes);
			Singleton<Scene>.Instance.UnLoad();
		}
	}
}
