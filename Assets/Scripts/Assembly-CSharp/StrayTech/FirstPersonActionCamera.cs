using System;
using System.Linq;
using ActionGame;
using ActionGame.Chara;
using Config;
using Manager;
using UnityEngine;

namespace StrayTech
{
	public class FirstPersonActionCamera : ICameraState, IActionCamera
	{
		private FirstPersonActionCameraStateSettings _stateSettings;

		private Quaternion _characterTargetRot;

		private Quaternion _cameraPitchRotation;

		private ActionScene actScene
		{
			get
			{
				return Singleton<Game>.IsInstance() ? Singleton<Game>.Instance.actScene : null;
			}
		}

		public ICameraStateSettings StateSettings
		{
			get
			{
				return _stateSettings;
			}
		}

		public CameraSystem.CameraStateEnum StateType
		{
			get
			{
				return CameraSystem.CameraStateEnum.FirstPersonAction;
			}
		}

		public bool AllowsModifiers
		{
			get
			{
				return true;
			}
		}

		public Vector3 Position { get; set; }

		public Quaternion Rotation { get; set; }

		public Vector3 Angle
		{
			get
			{
				return Rotation.eulerAngles;
			}
			set
			{
				Vector3 eulerAngles = _cameraPitchRotation.eulerAngles;
				eulerAngles.x = value.x;
				_cameraPitchRotation = ClampRotationAroundXAxis(Quaternion.Euler(eulerAngles));
				ActionScene actionScene = actScene;
				if (actionScene != null && actionScene.Player != null)
				{
					actionScene.Player.SetRotation(value);
					_characterTargetRot = actionScene.Player.rotation;
				}
				else
				{
					_characterTargetRot = Quaternion.Euler(new Vector3(0f, value.y, 0f));
				}
				Rotation = _characterTargetRot * _cameraPitchRotation;
			}
		}

		public bool isControl
		{
			get
			{
				if (actScene == null || actScene.Player == null || actScene.Player.isActionNow_Origin)
				{
					return false;
				}
				if (actScene.isCursorLock && !actScene.Map.isMapLoading && !Singleton<Scene>.Instance.IsNowLoadingFade && !actScene.npcList.Any((NPC npc) => npc.AI.actionNo == 24))
				{
					return true;
				}
				return false;
			}
		}

		public FirstPersonActionCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as FirstPersonActionCameraStateSettings;
			ActionScene actionScene = actScene;
			if (actionScene != null && actionScene.Player != null)
			{
				Quaternion quaternion = Quaternion.Euler(new Vector3(0f, MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.eulerAngles.y, 0f));
				actionScene.Player.rotation = quaternion;
				_characterTargetRot = quaternion;
			}
			_cameraPitchRotation = Quaternion.identity;
		}

		public void UpdateCamera(float deltaTime)
		{
			ActionScene actionScene = actScene;
			if (actionScene == null)
			{
				return;
			}
			Player player = actionScene.Player;
			if (!(player == null))
			{
				Quaternion rotation = player.rotation;
				Position = player.fpsPos + rotation * _stateSettings.PositionOffset;
				ActionSystem actData = Manager.Config.ActData;
				float angle = 0f;
				float angle2 = 0f;
				if (isControl)
				{
					angle = Input.GetAxis("Mouse X") * ((float)actData.FPSSensitivityX * 0.1f) * (float)((!actData.InvertMoveX) ? 1 : (-1));
					angle2 = Input.GetAxis("Mouse Y") * ((float)actData.FPSSensitivityY * 0.1f) * (float)((!actData.InvertMoveY) ? 1 : (-1));
				}
				_characterTargetRot *= Quaternion.AngleAxis(angle, Vector3.up);
				_cameraPitchRotation *= Quaternion.AngleAxis(angle2, Vector3.left);
				_cameraPitchRotation = ClampRotationAroundXAxis(_cameraPitchRotation);
				Quaternion b = rotation * _cameraPitchRotation;
				float t = (float)Mathf.Max(1, actData.FPSSmoothMoveTime) * deltaTime;
				player.rotation = Quaternion.Slerp(rotation, _characterTargetRot, t);
				Rotation = Quaternion.Slerp(Rotation, b, t);
			}
		}

		private Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1f;
			float value = 114.59156f * Mathf.Atan(q.x);
			value = Mathf.Clamp(value, _stateSettings.PitchRange.x, _stateSettings.PitchRange.y);
			q.x = Mathf.Tan((float)Math.PI / 360f * value);
			return q;
		}

		public void Cleanup()
		{
		}
	}
}
