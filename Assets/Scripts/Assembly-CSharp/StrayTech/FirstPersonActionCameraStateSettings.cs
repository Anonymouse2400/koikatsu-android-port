using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class FirstPersonActionCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("Position offset from the PositionRootTransform")]
		[SerializeField]
		private Vector3 _positionOffset = new Vector3(0f, 0f, 0.1f);

		[Tooltip("The range of vertical rotation.")]
		[SerializeField]
		private Vector2 _pitchRange = new Vector2(-90f, 90f);

		[Tooltip("Sensitivity of mouse movement on each axis.")]
		[SerializeField]
		private Vector2 _mouseLookSensitivity = new Vector2(2f, 2f);

		[Tooltip("Amount of mouse smoothing to apply.")]
		[SerializeField]
		private float _mouseSmoothing = 5f;

		public Vector3 PositionOffset
		{
			get
			{
				return _positionOffset;
			}
		}

		public Vector2 PitchRange
		{
			get
			{
				return _pitchRange;
			}
		}

		public Vector2 MouseLookSensitivity
		{
			get
			{
				return _mouseLookSensitivity;
			}
		}

		public float MouseSmoothing
		{
			get
			{
				return _mouseSmoothing;
			}
		}

		public bool UseCameraCollision
		{
			get
			{
				return false;
			}
		}

		public CameraSystem.CameraStateEnum StateType
		{
			get
			{
				return CameraSystem.CameraStateEnum.FirstPersonAction;
			}
		}

		public FirstPersonActionCameraStateSettings(Vector3 positionOffset, Vector2 pitchRange, Vector2 mouseLookSensitivity, float mouseSmoothing)
		{
			_positionOffset = positionOffset;
			_pitchRange = pitchRange;
			_mouseLookSensitivity = mouseLookSensitivity;
			_mouseSmoothing = mouseSmoothing;
		}
	}
}
