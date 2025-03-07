using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class IsometricCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("World space Euler rotation to lock the camera’s view to.")]
		[SerializeField]
		private Vector3 _rotation = new Vector3(45f, 90f, 0f);

		[Tooltip("The distance the camera will be from the target.")]
		[SerializeField]
		private float _distance = 5f;

		[Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
		[SerializeField]
		private bool _useCameraCollision;

		public Vector3 Rotation
		{
			get
			{
				return _rotation;
			}
		}

		public float Distance
		{
			get
			{
				return _distance;
			}
		}

		public bool UseCameraCollision
		{
			get
			{
				return _useCameraCollision;
			}
		}

		public CameraSystem.CameraStateEnum StateType
		{
			get
			{
				return CameraSystem.CameraStateEnum.Isometric;
			}
		}

		public IsometricCameraStateSettings(Vector3 rotation, float distance)
		{
			_rotation = rotation;
			_distance = distance;
		}
	}
}
