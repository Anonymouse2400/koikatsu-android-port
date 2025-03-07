using System;
using UnityEngine;

namespace StrayTech
{
	public class FirstPersonCamera : ICameraState
	{
		private FirstPersonCameraStateSettings _stateSettings;

		private Quaternion _characterTargetRot;

		private Quaternion _cameraTargetRot;

		private Quaternion _cameraPitchRotation;

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
				return CameraSystem.CameraStateEnum.FirstPerson;
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

		public FirstPersonCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as FirstPersonCameraStateSettings;
			if (_stateSettings.CharacterTransform != null)
			{
				_characterTargetRot = _stateSettings.CharacterTransform.rotation;
				Rotation = _stateSettings.CharacterTransform.rotation;
			}
			_cameraPitchRotation = Quaternion.identity;
		}

		public void UpdateCamera(float deltaTime)
		{
			if (!(_stateSettings.PositionRootTransform == null) && !(_stateSettings.CharacterTransform == null))
			{
				Vector3 position = _stateSettings.PositionRootTransform.position + _stateSettings.CharacterTransform.rotation * _stateSettings.PositionOffset;
				Position = position;
				float y = Input.GetAxis("Mouse X") * _stateSettings.MouseLookSensitivity.x;
				float num = Input.GetAxis("Mouse Y") * _stateSettings.MouseLookSensitivity.y;
				_characterTargetRot *= Quaternion.Euler(0f, y, 0f);
				_cameraPitchRotation *= Quaternion.Euler(0f - num, 0f, 0f);
				_cameraPitchRotation = ClampRotationAroundXAxis(_cameraPitchRotation);
				_cameraTargetRot = _stateSettings.CharacterTransform.rotation * _cameraPitchRotation;
				_stateSettings.CharacterTransform.rotation = Quaternion.Slerp(_stateSettings.CharacterTransform.rotation, _characterTargetRot, _stateSettings.MouseSmoothing * deltaTime);
				Rotation = Quaternion.Slerp(Rotation, _cameraTargetRot, _stateSettings.MouseSmoothing * deltaTime);
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
