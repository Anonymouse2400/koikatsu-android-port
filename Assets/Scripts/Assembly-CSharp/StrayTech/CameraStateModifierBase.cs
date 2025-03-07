using System.Collections;
using UnityEngine;

namespace StrayTech
{
	public abstract class CameraStateModifierBase : MonoBehaviour
	{
		[Tooltip("Duration of the transition into the enabled state.")]
		[SerializeField]
		private float _transitionIntoEnabledDuration = 1f;

		[Tooltip("Duration of the transition into the disabled state.")]
		[SerializeField]
		private float _transitionIntoDisabledDuration = 1f;

		[Tooltip("The animation clip to play. (Needs to be a Legacy Animation Clip)")]
		[SerializeField]
		private int _priority = int.MaxValue;

		private bool _transitioning;

		private float _transitionLerpT;

		protected Vector3 _cameraTargetPosition;

		protected Quaternion _cameraTargetRotation;

		public abstract string Name { get; }

		public int Priority
		{
			get
			{
				return _priority;
			}
		}

		public float TransitionLerpT
		{
			get
			{
				return _transitionLerpT;
			}
		}

		public virtual void Initialize()
		{
		}

		protected abstract void CalculateModification(ICameraState cameraState, float deltaTime);

		public void ModifiyCamera(ICameraState cameraState, float deltaTime)
		{
			CalculateModification(cameraState, deltaTime);
			if (_transitioning)
			{
				Vector3 position = cameraState.Position;
				Quaternion rotation = cameraState.Rotation;
				cameraState.Position = Vector3.Lerp(position, _cameraTargetPosition, _transitionLerpT);
				Vector3 a = rotation * Vector3.forward;
				Vector3 b = _cameraTargetRotation * Vector3.forward;
				cameraState.Rotation = Quaternion.LookRotation(Vector3.Lerp(a, b, _transitionLerpT).normalized, Vector3.up);
			}
			else
			{
				cameraState.Position = _cameraTargetPosition;
				cameraState.Rotation = _cameraTargetRotation;
			}
		}

		public virtual bool Enable()
		{
			_transitionIntoEnabledDuration = Mathf.Max(0f, _transitionIntoEnabledDuration);
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				return false;
			}
			MonoBehaviourSingleton<CameraSystem>.Instance.AddModifier(this);
			if (!Mathf.Approximately(_transitionIntoEnabledDuration, 0f))
			{
				StartCoroutine("DoTransitionIn", _transitionIntoEnabledDuration);
			}
			return true;
		}

		public virtual void Disable()
		{
			_transitionIntoDisabledDuration = Mathf.Max(0f, _transitionIntoDisabledDuration);
			if (!Mathf.Approximately(_transitionIntoDisabledDuration, 0f))
			{
				StartCoroutine("DoTransitionOut", _transitionIntoDisabledDuration);
			}
			else
			{
				MonoBehaviourSingleton<CameraSystem>.Instance.RemoveModifier(this);
			}
		}

		public virtual void Cleanup()
		{
		}

		private IEnumerator DoTransitionIn(float transitionDuration)
		{
			_transitioning = true;
			float elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;
				float curveSamplePosition = Mathf.Clamp01(elapsed / transitionDuration);
				_transitionLerpT = MonoBehaviourSingleton<CameraSystem>.Instance.CameraInterpolationCurve.Evaluate(curveSamplePosition);
				yield return null;
			}
			_transitionLerpT = 1f;
			_transitioning = false;
		}

		private IEnumerator DoTransitionOut(float transitionDuration)
		{
			_transitioning = true;
			float elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;
				float curveSamplePosition = 1f - Mathf.Clamp01(elapsed / transitionDuration);
				_transitionLerpT = MonoBehaviourSingleton<CameraSystem>.Instance.CameraInterpolationCurve.Evaluate(curveSamplePosition);
				yield return null;
			}
			_transitionLerpT = 0f;
			_transitioning = false;
			MonoBehaviourSingleton<CameraSystem>.Instance.RemoveModifier(this);
			Cleanup();
		}
	}
}
