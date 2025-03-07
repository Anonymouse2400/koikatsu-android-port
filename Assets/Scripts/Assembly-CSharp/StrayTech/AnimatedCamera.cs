using System;
using UnityEngine;

namespace StrayTech
{
	public class AnimatedCamera : ICameraState, IValidates
	{
		public class OnFinishedEventArgs
		{
			private bool _animationFinished;

			public bool AnimationFinished
			{
				get
				{
					return _animationFinished;
				}
			}

			public OnFinishedEventArgs(bool animationFinished)
			{
				_animationFinished = animationFinished;
			}
		}

		private AnimatedCameraStateSettings _stateSettings;

		private float _clipDuration;

		private float _currentClipTime;

		private bool _animationComplete;

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
				return CameraSystem.CameraStateEnum.Animated;
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

		public event Action<OnFinishedEventArgs> OnFinished;

		public event Action OnStarted;

		public AnimatedCamera(ICameraStateSettings stateSettings)
		{
			_stateSettings = stateSettings as AnimatedCameraStateSettings;
			if (IsValid())
			{
				_clipDuration = _stateSettings.AnimationClip.length;
				_currentClipTime = 0f;
				if (this.OnStarted != null)
				{
					this.OnStarted();
				}
			}
		}

		public void StopCurrentAnimation()
		{
			if (IsValid())
			{
				_animationComplete = true;
				if (this.OnFinished != null)
				{
					this.OnFinished(new OnFinishedEventArgs(true));
				}
			}
		}

		public void UpdateCamera(float deltaTime)
		{
			if (_animationComplete)
			{
				return;
			}
			if (!IsValid())
			{
				_animationComplete = true;
				if (this.OnFinished != null)
				{
					this.OnFinished(new OnFinishedEventArgs(true));
				}
			}
			else if (_currentClipTime < _clipDuration)
			{
				_currentClipTime += deltaTime;
				if (_currentClipTime > _clipDuration)
				{
					_currentClipTime = _clipDuration;
				}
				GameObject gameObject = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.gameObject;
				_stateSettings.AnimationClip.SampleAnimation(gameObject, _currentClipTime);
				if (_stateSettings.ParentOverride != null)
				{
					Position = _stateSettings.ParentOverride.transform.position + _stateSettings.ParentOverride.transform.rotation * gameObject.transform.position;
					Rotation = _stateSettings.ParentOverride.transform.rotation * gameObject.transform.rotation * Quaternion.Euler(0f, _stateSettings.YRotationFix, 0f);
				}
				else
				{
					Position = gameObject.transform.position;
					Rotation = gameObject.transform.rotation;
					Rotation *= Quaternion.Euler(0f, _stateSettings.YRotationFix, 0f);
				}
			}
			else
			{
				_animationComplete = true;
				Cleanup();
				if (this.OnFinished != null)
				{
					this.OnFinished(new OnFinishedEventArgs(true));
				}
			}
		}

		public void Cleanup()
		{
			if (IsValid())
			{
				if (!_animationComplete && this.OnFinished != null)
				{
					this.OnFinished(new OnFinishedEventArgs(false));
				}
				_animationComplete = true;
			}
		}

		public bool IsValid()
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				return false;
			}
			if (MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera == null)
			{
				return false;
			}
			if (_stateSettings == null)
			{
				return false;
			}
			if (_stateSettings.AnimationClip == null || string.IsNullOrEmpty(_stateSettings.AnimationClip.name))
			{
				return false;
			}
			return true;
		}
	}
}
