using UnityEngine;

namespace StrayTech
{
	public class ParentedCamera : ICameraState
	{
		private ParentedCameraStateSettings _stateSettings;

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
				return CameraSystem.CameraStateEnum.Parented;
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

		public ParentedCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as ParentedCameraStateSettings;
		}

		public void UpdateCamera(float deltaTime)
		{
			Position = _stateSettings.Parent.transform.position + _stateSettings.Parent.transform.rotation * _stateSettings.PositionOffset;
			Rotation = Quaternion.Euler(_stateSettings.RotationOffset) * _stateSettings.Parent.transform.rotation;
		}

		public void Cleanup()
		{
		}
	}
}
