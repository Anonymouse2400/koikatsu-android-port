using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class ParentedCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("The GameObject to parent to.")]
		[SerializeField]
		private GameObject _parent;

		[Tooltip("Position offset from parent.")]
		[SerializeField]
		private Vector3 _positionOffset = new Vector3(0f, 0f, 0f);

		[Tooltip("Rotation offset from parent in euler angles.")]
		[SerializeField]
		private Vector3 _rotationOffset = new Vector3(0f, 0f, 0f);

		[Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
		[SerializeField]
		private bool _useCameraCollision;

		public GameObject Parent
		{
			get
			{
				return _parent;
			}
		}

		public Vector3 PositionOffset
		{
			get
			{
				return _positionOffset;
			}
		}

		public Vector3 RotationOffset
		{
			get
			{
				return _rotationOffset;
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
				return CameraSystem.CameraStateEnum.Parented;
			}
		}

		public ParentedCameraStateSettings(GameObject parent, Vector3 positionOffset, Vector3 rotationOffset)
		{
			_parent = parent;
			_positionOffset = positionOffset;
			_rotationOffset = rotationOffset;
		}
	}
}
