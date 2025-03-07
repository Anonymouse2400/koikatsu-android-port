using UnityEngine;

namespace StrayTech
{
	public class LookAtTargetModifier : CameraStateModifierBase
	{
		[Tooltip("The target Transform to look at.")]
		[SerializeField]
		private Transform _lookAtTarget;

		public override string Name
		{
			get
			{
				return "Look At Target Modifier";
			}
		}

		protected override void CalculateModification(ICameraState cameraState, float deltaTime)
		{
			if (_lookAtTarget != null)
			{
				_cameraTargetPosition = cameraState.Position;
				_cameraTargetRotation = Quaternion.LookRotation((_lookAtTarget.position - cameraState.Position).normalized, Vector3.up);
			}
		}
	}
}
