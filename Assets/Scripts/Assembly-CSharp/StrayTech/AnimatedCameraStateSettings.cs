using System;
using UnityEngine;

namespace StrayTech
{
	[Serializable]
	public class AnimatedCameraStateSettings : ICameraStateSettings
	{
		[Tooltip("The animation clip to play. (Needs to be a Legacy Animation Clip)")]
		[SerializeField]
		private AnimationClip _animationClip;

		[Tooltip("Use the parent override to override the root of the animation.")]
		[SerializeField]
		private Transform _parentOverride;

		[Tooltip("Y axis rotation adjustment (Some animations from Maya need adjustment)")]
		[SerializeField]
		private float _yRotationFix;

		public AnimationClip AnimationClip
		{
			get
			{
				return _animationClip;
			}
		}

		public Transform ParentOverride
		{
			get
			{
				return _parentOverride;
			}
		}

		public float YRotationFix
		{
			get
			{
				return _yRotationFix;
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
				return CameraSystem.CameraStateEnum.Animated;
			}
		}

		public AnimatedCameraStateSettings(AnimationClip animationClipToPlay, Transform parentOverride, float yRotationFix)
		{
			_animationClip = animationClipToPlay;
			_parentOverride = parentOverride;
			_yRotationFix = yRotationFix;
		}
	}
}
