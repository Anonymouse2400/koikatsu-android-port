using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class SplineCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("The spline to use.")]
		[SerializeField]
		private BezierSpline _spline;

		[Tooltip("Offset the camera on the spline from the start in world units of length.")]
		[SerializeField]
		private float _splinePositionOffset;

		[Tooltip("Offset along the line of sight to the target.")]
		[SerializeField]
		private float _cameraLineOfSightOffset;

		[Tooltip("Maximum distance the camera can be from the target.")]
		[SerializeField]
		private float _cameraMaxDistance = 5f;

		[Tooltip("The maximum speed the camera can travel along the spline in world units of length per second.")]
		[SerializeField]
		private float _splineTravelMaxSpeed = 0.1f;

		[Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
		[SerializeField]
		private bool _useCameraCollision;

		public BezierSpline Spline
		{
			get
			{
				return _spline;
			}
		}

		public float SplinePositionOffset
		{
			get
			{
				return _splinePositionOffset;
			}
		}

		public float CameraLineOfSightOffset
		{
			get
			{
				return _cameraLineOfSightOffset;
			}
		}

		public float CameraMaxDistance
		{
			get
			{
				return _cameraMaxDistance;
			}
		}

		public float SplineTravelMaxSpeed
		{
			get
			{
				return _splineTravelMaxSpeed;
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
				return CameraSystem.CameraStateEnum.Spline;
			}
		}

		public SplineCameraStateSettings(BezierSpline spline, float splinePositionOffset, float cameraLineOfSightOffset, float cameraMaxDistance, float splineTravelMaxSpeed)
		{
			_spline = spline;
			_splinePositionOffset = splinePositionOffset;
			_cameraLineOfSightOffset = cameraLineOfSightOffset;
			_cameraMaxDistance = cameraMaxDistance;
			_splineTravelMaxSpeed = splineTravelMaxSpeed;
		}
	}
}
