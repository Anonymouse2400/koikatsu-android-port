using System;
using StrayTech.CustomAttributes;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class ThirdPersonCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("Use the mouse to control the camera's orbit.")]
		[SerializeField]
		private bool _mouseOrbit;

		[Tooltip("The position offset from the target.")]
		[SerializeField]
		private Vector3 _targetOffset = new Vector3(0f, 6f, -5f);

		[Tooltip("The minimum and maximum distance the camera can be from the target.")]
		[SerializeField]
		private Vector2 _mouseOrbitDistance = new Vector2(1f, 5f);

		[Tooltip("The range of vertical rotation.")]
		[SerializeField]
		private Vector2 _mousePitchRange = new Vector2(-90f, 90f);

		[Tooltip("Sensitivity of mouse movement on each axis.")]
		[SerializeField]
		private Vector2 _mouseSensitivity = new Vector2(2f, 2f);

		[Tooltip("Invert mouse Y axis?")]
		[SerializeField]
		private bool _mouseInvertY;

		[Tooltip("The amount of smoothing to apply.")]
		[NonNegative]
		[SerializeField]
		private float _motionSmoothing = 6f;

		[SerializeField]
		[Tooltip("Whether to use camera collision or not. (Requires Camera Collision Component mentioned above)")]
		private bool _useCameraCollision;

		public bool MouseOrbit
		{
			get
			{
				return _mouseOrbit;
			}
		}

		public Vector3 TargetOffset
		{
			get
			{
				return _targetOffset;
			}
		}

		public Vector2 MouseOrbitDistance
		{
			get
			{
				return _mouseOrbitDistance;
			}
		}

		public Vector2 MousePitchRange
		{
			get
			{
				return _mousePitchRange;
			}
		}

		public Vector2 MouseSensitivity
		{
			get
			{
				return _mouseSensitivity;
			}
		}

		public bool MouseInvertY
		{
			get
			{
				return _mouseInvertY;
			}
		}

		public float MotionSmoothing
		{
			get
			{
				return _motionSmoothing;
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
				return CameraSystem.CameraStateEnum.ThirdPerson;
			}
		}

		public ThirdPersonCameraStateSettings(Vector3 targetOffset, bool mouseOrbit, Vector2 mouseOrbitDistance, Vector2 mousePitchRange, Vector2 mouseSensitivity, bool mouseInvertY, float motionSmoothing)
		{
			_targetOffset = targetOffset;
			_mouseOrbit = mouseOrbit;
			_mouseOrbitDistance = mouseOrbitDistance;
			_mousePitchRange = mousePitchRange;
			_mouseSensitivity = mouseSensitivity;
			_mouseInvertY = mouseInvertY;
			_motionSmoothing = motionSmoothing;
		}
	}
}
