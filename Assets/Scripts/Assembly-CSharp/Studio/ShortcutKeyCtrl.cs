using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace Studio
{
	public class ShortcutKeyCtrl : MonoBehaviour
	{
		[SerializeField]
		private StudioScene studioScene;

		[SerializeField]
		private SystemButtonCtrl systemButtonCtrl;

		[SerializeField]
		private WorkspaceCtrl workspaceCtrl;

		[SerializeField]
		private CameraControl cameraControl;

		[SerializeField]
		private TreeNodeCtrl treeNodeCtrl;

		[SerializeField]
		private GameScreenShot gameScreenShot;

		[SerializeField]
		private Sprite[] sprites;

		private readonly KeyCode[] cameraKey = new KeyCode[10]
		{
			KeyCode.Alpha1,
			KeyCode.Alpha2,
			KeyCode.Alpha3,
			KeyCode.Alpha4,
			KeyCode.Alpha5,
			KeyCode.Alpha6,
			KeyCode.Alpha7,
			KeyCode.Alpha8,
			KeyCode.Alpha9,
			KeyCode.Alpha0
		};

		private void Notification(int _id)
		{
			NotificationScene.spriteMessage = sprites[_id];
			NotificationScene.waitTime = 2f;
			NotificationScene.width = 416f;
			NotificationScene.height = 160f;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "StudioNotification",
				isAdd = true
			}, false);
		}

		private void Update()
		{
			if (!Singleton<Studio>.IsInstance() || Singleton<Studio>.Instance.isInputNow || !Singleton<Scene>.IsInstance() || Singleton<Scene>.Instance.AddSceneName != string.Empty)
			{
				return;
			}
			bool flag = Input.GetKey(KeyCode.LeftControl) | Input.GetKey(KeyCode.RightControl);
			bool flag2 = Input.GetKey(KeyCode.LeftShift) | Input.GetKey(KeyCode.RightShift);
			if (flag2 & Input.GetKeyDown(KeyCode.Z))
			{
				if (Singleton<UndoRedoManager>.Instance.CanRedo)
				{
					Singleton<UndoRedoManager>.Instance.Redo();
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.Z))
			{
				if (Singleton<UndoRedoManager>.Instance.CanUndo)
				{
					Singleton<UndoRedoManager>.Instance.Undo();
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.F))
			{
				TreeNodeObject selectNode = treeNodeCtrl.selectNode;
				if (!(selectNode == null))
				{
					ObjectCtrlInfo value = null;
					if (Singleton<Studio>.Instance.dicInfo.TryGetValue(selectNode, out value))
					{
						cameraControl.targetPos = value.guideObject.transform.position;
					}
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.C))
			{
				GuideObject[] selectObjects = Singleton<GuideObjectManager>.Instance.selectObjects;
				if (selectObjects.IsNullOrEmpty())
				{
					return;
				}
				List<GuideCommand.EqualsInfo> list = new List<GuideCommand.EqualsInfo>();
				GuideObject[] array = selectObjects;
				foreach (GuideObject guideObject in array)
				{
					if (guideObject.enablePos)
					{
						list.Add(guideObject.SetWorldPos(cameraControl.targetPos));
					}
				}
				if (!list.IsNullOrEmpty())
				{
					Singleton<UndoRedoManager>.Instance.Push(new GuideCommand.MoveEqualsCommand(list.ToArray()));
				}
				return;
			}
			if (flag && Input.GetKeyDown(KeyCode.S))
			{
				systemButtonCtrl.OnClickSave();
				return;
			}
			if (flag && Input.GetKeyDown(KeyCode.D))
			{
				Singleton<Studio>.Instance.Duplicate();
				return;
			}
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				workspaceCtrl.OnClickDelete();
				return;
			}
			if (Input.GetKeyDown(KeyCode.W))
			{
				Singleton<GuideObjectManager>.Instance.mode = 0;
				return;
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				Singleton<GuideObjectManager>.Instance.mode = 1;
				return;
			}
			if (Input.GetKeyDown(KeyCode.R))
			{
				Singleton<GuideObjectManager>.Instance.mode = 2;
				return;
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				studioScene.OnClickAxis();
				return;
			}
			if (Input.GetKeyDown(KeyCode.J))
			{
				studioScene.OnClickAxisTrans();
				return;
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
				studioScene.OnClickAxisCenter();
				return;
			}
			if (Input.GetKeyDown(KeyCode.F11))
			{
				gameScreenShot.Capture(string.Empty);
				return;
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				if (Singleton<GameCursor>.IsInstance())
				{
					Singleton<GameCursor>.Instance.SetCursorLock(false);
				}
				Singleton<Scene>.Instance.LoadReserve(new Scene.Data
				{
					levelName = "StudioShortcutMenu",
					isAdd = true
				}, false);
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (Singleton<GameCursor>.IsInstance())
				{
					Singleton<GameCursor>.Instance.SetCursorLock(false);
				}
				Singleton<Scene>.Instance.GameEnd();
				return;
			}
			bool flag3 = false;
			for (int j = 0; j < 10; j++)
			{
				if (Input.GetKeyDown(cameraKey[j]))
				{
					studioScene.OnClickLoadCamera(j);
					flag3 = true;
					break;
				}
			}
			if (!flag3 && Input.GetKeyDown(KeyCode.H))
			{
				Singleton<Studio>.Instance.cameraSelector.NextCamera();
			}
		}
	}
}
