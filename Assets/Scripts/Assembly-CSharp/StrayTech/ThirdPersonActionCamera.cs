using System.Linq;
using ActionGame;
using ActionGame.Chara;
using Config;
using Manager;
using UnityEngine;

namespace StrayTech
{
	public class ThirdPersonActionCamera : ICameraState, IActionCamera
	{
		private ThirdPersonActionCameraStateSettings _stateSettings;

		private Transform _cameraLookAtTransform;

		private float _distance = 1f;

		private Vector3 _angle = Vector3.zero;

		private Quaternion _currentOrbitRotation;

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
				return CameraSystem.CameraStateEnum.ThirdPersonAction;
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

		public float Distance
		{
			get
			{
				return _distance;
			}
		}

		public Vector3 Angle
		{
			get
			{
				return _angle;
			}
			set
			{
				_angle = value;
				_currentOrbitRotation = Quaternion.Euler(value);
				Rotation = _currentOrbitRotation;
			}
		}

		public bool isControl
		{
			get
			{
				bool result = false;
				if (actScene != null && actScene.isCursorLock && !actScene.Map.isMapLoading && !Singleton<Scene>.Instance.IsNowLoadingFade && !actScene.npcList.Any((NPC npc) => npc.AI.actionNo == 24))
				{
					result = true;
				}
				return result;
			}
		}

		public ThirdPersonActionCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as ThirdPersonActionCameraStateSettings;
			_cameraLookAtTransform = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget;
			_distance = (_stateSettings.MouseOrbitDistance.x + _stateSettings.MouseOrbitDistance.y) * 0.5f;
			if (_stateSettings.MouseOrbit)
			{
				Vector3 forward = (MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position - MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.position).normalized;
				ActionScene actionScene = actScene;
				if (actionScene != null && actionScene.Player != null)
				{
					forward = actionScene.Player.cachedTransform.forward;
				}
				_angle = Quaternion.LookRotation(forward, Vector3.up).eulerAngles;
			}
			UpdateCamera(100f);
		}

		public void UpdateCamera(float deltaTime)
		{
			if (_cameraLookAtTransform == null)
			{
				return;
			}
			if (_stateSettings.MouseOrbit)
			{
				ActionScene actionScene = actScene;
				ActionSystem actData = Manager.Config.ActData;
				if (isControl)
				{
					_angle.y += Input.GetAxis("Mouse X") * (float)actData.TPSSensitivityX * _distance * 0.02f * (float)((!actData.InvertMoveX) ? 1 : (-1));
					_angle.x -= Input.GetAxis("Mouse Y") * (float)actData.TPSSensitivityY * 0.02f * (float)((!actData.InvertMoveY) ? 1 : (-1));
					_distance = Mathf.Clamp(_distance - Input.GetAxis("Mouse ScrollWheel") * 5f, _stateSettings.MouseOrbitDistance.x, _stateSettings.MouseOrbitDistance.y);
				}
				_angle.x = ClampAngle(_angle.x, _stateSettings.MousePitchRange.x, _stateSettings.MousePitchRange.y);
				float t = (float)Mathf.Max(1, actData.TPSSmoothMoveTime) * deltaTime;
				_currentOrbitRotation = Quaternion.Slerp(_currentOrbitRotation, Quaternion.Euler(_angle), t);
				Rotation = _currentOrbitRotation;
				Position = _currentOrbitRotation * new Vector3(0f, 0f, 0f - _distance) + _cameraLookAtTransform.position;
			}
			else
			{
				Vector3 b = _cameraLookAtTransform.position + _cameraLookAtTransform.rotation * _stateSettings.TargetOffset;
				float t2 = _stateSettings.MotionSmoothing * deltaTime;
				Vector3 vector = Vector3.Lerp(Position, b, t2);
				float magnitude = (vector - _cameraLookAtTransform.position).magnitude;
				Vector3 normalized = (vector - _cameraLookAtTransform.position).normalized;
				Quaternion rotation = Quaternion.LookRotation(_cameraLookAtTransform.position - Position, Vector3.up);
				Position = _cameraLookAtTransform.position + normalized * magnitude;
				Rotation = rotation;
			}
		}

		public void Cleanup()
		{
		}

		public static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f)
			{
				angle += 360f;
			}
			if (angle > 360f)
			{
				angle -= 360f;
			}
			return Mathf.Clamp(angle, min, max);
		}
	}
}
