using StrayTech.CustomAttributes;
using UnityEngine;

namespace StrayTech
{
	public class CameraZoomInOnTargetModifier : CameraStateModifierBase
	{
		[Tooltip("The target to zoom in on.")]
		[SerializeField]
		private Transform _target;

		[SerializeField]
		[Tooltip("The offset from target.")]
		private Vector3 _targetOffset;

		[NonNegative]
		[Tooltip("The distance to zoom into from target.")]
		[SerializeField]
		private float _distanceFromTarget = 8f;

		public override string Name
		{
			get
			{
				return "Camera Zoom In On Target Modifier";
			}
		}

		protected override void CalculateModification(ICameraState cameraState, float deltaTime)
		{
			Vector3 vector = _target.position + _targetOffset;
			Vector3 normalized = (cameraState.Position - vector).normalized;
			_cameraTargetPosition = vector + normalized * _distanceFromTarget;
			_cameraTargetRotation = Quaternion.LookRotation(-normalized, Vector3.up);
		}
	}
}
