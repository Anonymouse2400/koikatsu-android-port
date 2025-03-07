using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using ActionGame.Chara.Mover;
using StrayTech;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class CameraStateDefinitionChange : MonoBehaviour
	{
		[SerializeField]
		private CameraStateDefinition TPS;

		[SerializeField]
		private CameraStateDefinition FPS;

		[SerializeField]
		private ActionScene actScene;

		private CameraMode? prevMode;

		private CameraMode? orderMode;

		[SerializeField]
		private float _visibleCharaDistance = 0.95f;

		[SerializeField]
		private float _visibleNPCDistance = 0.95f;

		private bool _changedFrameNow;

		public ThirdPersonActionCamera TPSCamera
		{
			get
			{
				return (Mode != 0) ? null : (TPS.State as ThirdPersonActionCamera);
			}
		}

		public FirstPersonActionCamera FPSCamera
		{
			get
			{
				return (Mode != CameraMode.FPS) ? null : (FPS.State as FirstPersonActionCamera);
			}
		}

		public IActionCamera actCamera
		{
			get
			{
				switch (Mode)
				{
				case CameraMode.TPS:
					return TPS.State as IActionCamera;
				case CameraMode.FPS:
					return FPS.State as IActionCamera;
				default:
					return null;
				}
			}
		}

		public CameraMode Mode
		{
			get
			{
				CameraStateDefinition cameraStateDefinition = ((!(MonoBehaviourSingleton<CameraSystem>.Instance.NextCameraStateDefinition != null)) ? MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition : MonoBehaviourSingleton<CameraSystem>.Instance.NextCameraStateDefinition);
				if (cameraStateDefinition == FPS)
				{
					return CameraMode.FPS;
				}
				if (cameraStateDefinition == TPS)
				{
					return CameraMode.TPS;
				}
				return CameraMode.Other;
			}
		}

		public void ModeChangeForce(CameraMode? mode, bool isPrev = true)
		{
			if (mode.HasValue && !(actScene == null) && !(actScene.Player == null) && (Mode != mode.GetValueOrDefault() || !mode.HasValue))
			{
				if (isPrev)
				{
					prevMode = Mode;
				}
				ModeChange(mode.Value, actScene.Player);
			}
		}

		public void ModeChangePrev()
		{
			ModeChangeForce(prevMode);
			prevMode = null;
		}

		public void SetAngle(Vector3 angle)
		{
			IActionCamera actionCamera = actCamera;
			if (actionCamera != null)
			{
				actionCamera.Angle = angle;
			}
		}

		private void RemoveState()
		{
			MonoBehaviourSingleton<CameraSystem>.Instance.UnregisterCameraState(TPS);
			MonoBehaviourSingleton<CameraSystem>.Instance.UnregisterCameraState(FPS);
		}

		private void ModeChange(CameraMode mode, Player player)
		{
			if (!(player == null))
			{
				orderMode = mode;
				bool visibleAll = player.chaCtrl.visibleAll;
				player.chaCtrl.visibleAll = mode != CameraMode.FPS;
				if (visibleAll != player.chaCtrl.visibleAll)
				{
					_changedFrameNow = true;
				}
				player.chaCtrl.LateUpdateForce();
				switch (mode)
				{
				case CameraMode.FPS:
					RemoveState();
					MonoBehaviourSingleton<CameraSystem>.Instance.RegisterCameraState(FPS);
					break;
				case CameraMode.TPS:
					RemoveState();
					MonoBehaviourSingleton<CameraSystem>.Instance.RegisterCameraState(TPS);
					break;
				}
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitWhile(() => MonoBehaviourSingleton<CameraSystem>.Instance == null);
			IObservable<Unit> updateCheck = from _ in this.UpdateAsObservable().TakeUntilDestroy(actScene).TakeUntilDestroy(MonoBehaviourSingleton<CameraSystem>.Instance)
				where base.enabled
				where actScene.isCursorLock
				where actScene.Player != null
				where actScene.Player.initialized
				where !orderMode.HasValue
				where MonoBehaviourSingleton<CameraSystem>.Instance.NextCameraStateDefinition == null
				select _;
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(MonoBehaviourSingleton<CameraSystem>.Instance)
				where orderMode.HasValue
				select _).Subscribe(delegate
			{
				if (orderMode.Value == Mode)
				{
					orderMode = null;
				}
			});
			(from _ in updateCheck.TakeUntilDestroy(actScene)
				where ActionInput.isViewChange
				where actScene.Player != null && !actScene.Player.isActionNow_Origin
				select actCamera).Subscribe(delegate(IActionCamera actCam)
			{
				if (actCam != null && actCam.isControl)
				{
					switch (Mode)
					{
					case CameraMode.FPS:
						ModeChange(CameraMode.TPS, actScene.Player);
						break;
					case CameraMode.TPS:
						ModeChange(CameraMode.FPS, actScene.Player);
						break;
					}
				}
			});
			(from _ in updateCheck
				where ActionInput.isViewTurn
				select actCamera).Subscribe(delegate(IActionCamera actCam)
			{
				if (actCam != null && actCam.isControl)
				{
					actCam.Angle += new Vector3(0f, 180f);
				}
			});
			(from _ in updateCheck
				where ActionInput.isViewPlayer
				select actCamera).Subscribe(delegate(IActionCamera actCam)
			{
				if (actCam != null && actCam.isControl)
				{
					actCam.Angle = new Vector3(actCam.Angle.x, actScene.Player.eulerAngles.y);
				}
			});
			updateCheck.Select((Unit _) => actScene.npcList.Any((NPC npc) => npc.AI.actionNo == 24)).SkipWhile((bool isOn) => !isOn).DistinctUntilChanged()
				.Subscribe(delegate(bool isOn)
				{
					if (isOn)
					{
						actScene.Player.move.agentSpeeder.mode = AgentSpeeder.Mode.Run;
						ModeChangeForce(CameraMode.FPS);
					}
					else
					{
						ModeChangePrev();
					}
				});
			(from _ in updateCheck
				where actScene.Player != null
				where actScene.Player.isActive
				select _).Subscribe(delegate
			{
				bool flag = Mode == CameraMode.FPS;
				UpdateVisible((!flag) ? new float?(_visibleCharaDistance) : ((float?)null));
				List<NPC> list = actScene.npcList.FindAll((NPC x) => x.isActive);
				bool flag2 = false;
				for (int i = 0; i < list.Count; i++)
				{
					flag2 |= UpdateVisibleNPC(list, i, (!flag) ? new float?(_visibleNPCDistance) : ((float?)null));
				}
			});
		}

		private void UpdateVisible(float? visibleDistance)
		{
			if (_changedFrameNow)
			{
				_changedFrameNow = false;
				return;
			}
			Player player = actScene.Player;
			bool visibleAll = player.chaCtrl.visibleAll;
			bool flag2;
			if (visibleDistance.HasValue)
			{
				float num = Vector3.Distance(MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position, MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.position);
				bool flag = visibleDistance.HasValue && num > visibleDistance.GetValueOrDefault();
				player.chaCtrl.visibleAll = flag;
				flag2 = flag;
			}
			else
			{
				bool flag = false;
				player.chaCtrl.visibleAll = flag;
				flag2 = flag;
			}
			foreach (GameObject itemObj in player.itemObjList)
			{
				Renderer component = itemObj.GetComponent<Renderer>();
				component.enabled = flag2;
			}
			if (flag2 != visibleAll)
			{
				player.chaCtrl.LateUpdateForce();
			}
		}

		private bool UpdateVisibleNPC(List<NPC> npcList, int index, float? visibleDistance)
		{
			NPC nPC = npcList[index];
			bool visibleAll = nPC.chaCtrl.visibleAll;
			bool flag2;
			if (visibleDistance.HasValue)
			{
				float num = Vector3.Distance(nPC.BustPos, MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.position);
				bool flag = visibleDistance.HasValue && num > visibleDistance.GetValueOrDefault();
				nPC.chaCtrl.visibleAll = flag;
				flag2 = flag;
			}
			else
			{
				bool flag = true;
				nPC.chaCtrl.visibleAll = flag;
				flag2 = flag;
			}
			foreach (GameObject itemObj in nPC.itemObjList)
			{
				Renderer[] componentsInChildren = itemObj.GetComponentsInChildren<Renderer>(true);
				if (!componentsInChildren.IsNullOrEmpty())
				{
					Renderer[] array = componentsInChildren;
					foreach (Renderer renderer in array)
					{
						renderer.enabled = flag2;
					}
				}
			}
			return flag2 != visibleAll;
		}
	}
}
