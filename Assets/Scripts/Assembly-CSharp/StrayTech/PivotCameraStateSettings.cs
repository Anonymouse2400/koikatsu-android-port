using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class PivotCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("The transform to pivot on.")]
		[SerializeField]
		private Transform _pivotHost;

		[Tooltip("The offset from the pivot host position.")]
		[SerializeField]
		private Vector3 _pivotHostOffset = new Vector3(0f, 0f, 0.1f);

		[Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
		[SerializeField]
		private bool _useCameraCollision;

		public Transform PivotHost
		{
			get
			{
				return _pivotHost;
			}
		}

		public Vector3 PivotHostOffset
		{
			get
			{
				return _pivotHostOffset;
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
				return CameraSystem.CameraStateEnum.Pivot;
			}
		}

		public PivotCameraStateSettings(Transform pivotHost, Vector3 pivotHostOffset)
		{
			_pivotHost = pivotHost;
			_pivotHostOffset = pivotHostOffset;
		}
	}
}
