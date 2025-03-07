using UnityEngine;

namespace StrayTech
{
	public class ThirdPersonCamera : ICameraState
	{
		private ThirdPersonCameraStateSettings _stateSettings;

		private Transform _cameraLookAtTransform;

		private float _orbitDistance = 1f;

		private float _mouseOrbitY;

		private float _mouseOrbitX;

		private Quaternion _currentOrbitRotation;

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
				return CameraSystem.CameraStateEnum.ThirdPerson;
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

		public ThirdPersonCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as ThirdPersonCameraStateSettings;
			_cameraLookAtTransform = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget;
			_orbitDistance = (_stateSettings.MouseOrbitDistance.x + _stateSettings.MouseOrbitDistance.y) * 0.5f;
			if (_stateSettings.MouseOrbit)
			{
				Vector3 normalized = (MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position - MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.position).normalized;
				Quaternion quaternion = Quaternion.LookRotation(normalized, Vector3.up);
				_mouseOrbitX = quaternion.eulerAngles.x;
				_mouseOrbitY = quaternion.eulerAngles.y;
			}
			UpdateCamera(100f);
		}

		public void UpdateCamera(float deltaTime)
		{
			if (!(_cameraLookAtTransform == null))
			{
				if (_stateSettings.MouseOrbit)
				{
					_mouseOrbitY += Input.GetAxis("Mouse X") * _stateSettings.MouseSensitivity.x * _orbitDistance * 0.02f;
					_mouseOrbitX -= Input.GetAxis("Mouse Y") * _stateSettings.MouseSensitivity.y * 0.02f * (float)((!_stateSettings.MouseInvertY) ? 1 : (-1));
					_mouseOrbitX = ClampAngle(_mouseOrbitX, _stateSettings.MousePitchRange.x, _stateSettings.MousePitchRange.y);
					Quaternion b = Quaternion.Euler(_mouseOrbitX, _mouseOrbitY, 0f);
					_currentOrbitRotation = Quaternion.Slerp(_currentOrbitRotation, b, _stateSettings.MotionSmoothing * deltaTime);
					_orbitDistance = Mathf.Clamp(_orbitDistance - Input.GetAxis("Mouse ScrollWheel") * 5f, _stateSettings.MouseOrbitDistance.x, _stateSettings.MouseOrbitDistance.y);
					Vector3 vector = new Vector3(0f, 0f, 0f - _orbitDistance);
					Vector3 position = _currentOrbitRotation * vector + _cameraLookAtTransform.position;
					Rotation = _currentOrbitRotation;
					Position = position;
				}
				else
				{
					Vector3 b2 = _cameraLookAtTransform.position + _cameraLookAtTransform.rotation * _stateSettings.TargetOffset;
					Vector3 vector2 = Vector3.Lerp(Position, b2, _stateSettings.MotionSmoothing * deltaTime);
					float magnitude = (vector2 - _cameraLookAtTransform.position).magnitude;
					Vector3 normalized = (vector2 - _cameraLookAtTransform.position).normalized;
					Quaternion rotation = Quaternion.LookRotation(_cameraLookAtTransform.position - Position, Vector3.up);
					Position = _cameraLookAtTransform.position + normalized * magnitude;
					Rotation = rotation;
				}
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
