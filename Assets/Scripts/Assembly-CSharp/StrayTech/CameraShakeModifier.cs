using System.Collections;
using UnityEngine;

namespace StrayTech
{
	public class CameraShakeModifier : CameraStateModifierBase
	{
		[Tooltip("Defines the duration and intensity multiplier of the camera shake.")]
		[SerializeField]
		private AnimationCurve _shakeIntensityMultiplierCurve;

		[Tooltip("The base intensity of the camera shake.")]
		[SerializeField]
		private float _shakeIntensity = 3f;

		private Vector3 _positionOffset = Vector3.zero;

		private Quaternion _rotationOffset = Quaternion.identity;

		private float _shakeDuration;

		private float _shakeLerpT;

		public override string Name
		{
			get
			{
				return "Camera Shake Modifier";
			}
		}

		protected override void CalculateModification(ICameraState cameraState, float deltaTime)
		{
			_positionOffset += Random.insideUnitSphere * _shakeIntensity * deltaTime;
			_rotationOffset *= Quaternion.AngleAxis(_shakeIntensity * 10f * deltaTime, Random.insideUnitSphere);
			_cameraTargetPosition = Vector3.Lerp(cameraState.Position, cameraState.Position + _positionOffset, _shakeLerpT);
			_cameraTargetRotation = Quaternion.Slerp(cameraState.Rotation, cameraState.Rotation * _rotationOffset, _shakeLerpT);
		}

		public override bool Enable()
		{
			_shakeDuration = _shakeIntensityMultiplierCurve.keys[_shakeIntensityMultiplierCurve.length - 1].time;
			StartCoroutine(DoCurveBasedCameraShake());
			return base.Enable();
		}

		private IEnumerator DoCurveBasedCameraShake()
		{
			float elapsedTime = 0f;
			while (elapsedTime <= _shakeDuration)
			{
				elapsedTime += Time.deltaTime;
				_shakeLerpT = _shakeIntensityMultiplierCurve.Evaluate(elapsedTime);
				yield return null;
			}
			Disable();
		}
	}
}
