using UnityEngine;

namespace StrayTech
{
	public class SplineCamera : ICameraState
	{
		private SplineCameraStateSettings _stateSettings;

		private Transform _cameraLookAtTransform;

		private float _currentSplineT = -1f;

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
				return CameraSystem.CameraStateEnum.Spline;
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

		public SplineCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as SplineCameraStateSettings;
			_currentSplineT = -1f;
		}

		public void UpdateCamera(float deltaTime)
		{
			if (_cameraLookAtTransform == null)
			{
				_cameraLookAtTransform = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget;
				if (_cameraLookAtTransform == null)
				{
					return;
				}
			}
			if (_stateSettings.Spline == null)
			{
				return;
			}
			float closestPointParam = _stateSettings.Spline.GetClosestPointParam(_cameraLookAtTransform.position, 5);
			if (_stateSettings.Spline.Loop)
			{
				closestPointParam += 0.5f;
				closestPointParam = Mathf.Abs(closestPointParam % 1f);
				if (_currentSplineT < 0f)
				{
					_currentSplineT = closestPointParam;
				}
				Vector3 vector = _cameraLookAtTransform.position - _stateSettings.Spline.transform.position;
				vector.y = 0f;
				float magnitude = vector.magnitude;
				float num = Mathf.Max(_stateSettings.Spline.transform.lossyScale.x, _stateSettings.Spline.transform.lossyScale.z);
				float f = magnitude / num;
				float splineTravelMaxSpeed = _stateSettings.SplineTravelMaxSpeed;
				splineTravelMaxSpeed *= Mathf.Clamp01(Mathf.Pow(f, 4f));
				Quaternion a = Quaternion.Euler(0f, _currentSplineT * 360f, 0f);
				Quaternion b = Quaternion.Euler(0f, closestPointParam * 360f, 0f);
				_currentSplineT = Quaternion.Slerp(a, b, splineTravelMaxSpeed * deltaTime).eulerAngles.y / 360f;
				_currentSplineT = Mathf.Abs(_currentSplineT % 1f);
			}
			else
			{
				if (_currentSplineT < 0f)
				{
					_currentSplineT = closestPointParam;
				}
				float num2 = _stateSettings.SplinePositionOffset / _stateSettings.Spline.Length;
				_currentSplineT = Mathf.Lerp(_currentSplineT, closestPointParam + num2, _stateSettings.SplineTravelMaxSpeed * deltaTime);
			}
			Vector3 position = _stateSettings.Spline.GetPosition(_currentSplineT);
			Rotation = Quaternion.LookRotation(_cameraLookAtTransform.position - position, Vector3.up);
			Position = position + Rotation * Vector3.forward * _stateSettings.CameraLineOfSightOffset;
			float num3 = Vector3.Distance(Position, _cameraLookAtTransform.position);
			if (num3 > _stateSettings.CameraMaxDistance)
			{
				Position += Rotation * (Vector3.forward * (num3 - _stateSettings.CameraMaxDistance));
			}
		}

		public void Cleanup()
		{
			_currentSplineT = -1f;
		}
	}
}
