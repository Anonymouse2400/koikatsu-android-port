using UnityEngine;

namespace StrayTech
{
	public class IsometricCamera : ICameraState
	{
		private IsometricCameraStateSettings _stateSettings;

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
				return CameraSystem.CameraStateEnum.Isometric;
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

		public IsometricCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as IsometricCameraStateSettings;
		}

		public void UpdateCamera(float deltaTime)
		{
			if (!(MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget == null))
			{
				Position = MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position + Quaternion.Euler(_stateSettings.Rotation) * -Vector3.forward * _stateSettings.Distance;
				Rotation = Quaternion.LookRotation(MonoBehaviourSingleton<CameraSystem>.Instance.CameraTarget.position - Position, Vector3.up);
			}
		}

		public void Cleanup()
		{
		}
	}
}
