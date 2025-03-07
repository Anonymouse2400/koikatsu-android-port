using UnityEngine;

namespace StrayTech
{
	public class PivotCamera : ICameraState
	{
		private PivotCameraStateSettings _stateSettings;

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
				return CameraSystem.CameraStateEnum.Pivot;
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

		public PivotCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as PivotCameraStateSettings;
		}

		public void UpdateCamera(float deltaTime)
		{
			if (!(MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget == null))
			{
				Vector3 vector = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position - _stateSettings.PivotHost.position;
				Vector3 normalized = new Vector3(vector.x, 0f, vector.z).normalized;
				Vector3 position = _stateSettings.PivotHost.position + Quaternion.LookRotation(normalized, Vector3.up) * _stateSettings.PivotHostOffset;
				Position = position;
				Rotation = Quaternion.LookRotation((MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position - Position).normalized, Vector3.up);
			}
		}

		public void Cleanup()
		{
		}
	}
}
