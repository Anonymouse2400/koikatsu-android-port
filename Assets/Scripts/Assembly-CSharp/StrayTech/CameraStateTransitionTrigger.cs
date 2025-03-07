using UnityEngine;

namespace StrayTech
{
	public class CameraStateTransitionTrigger : CameraSystemTriggerBase
	{
		[Tooltip("The target Camera State Definition to transition to.")]
		[SerializeField]
		private CameraStateDefinition _targetCameraStateDefinition;

		public CameraStateDefinition TargetCameraStateDefinition
		{
			get
			{
				return _targetCameraStateDefinition;
			}
			set
			{
				_targetCameraStateDefinition = value;
			}
		}

		protected override void TriggerEntered()
		{
			MonoBehaviourSingleton<CameraSystem>.Instance.RegisterCameraState(_targetCameraStateDefinition);
		}

		protected override void TriggerExited()
		{
			MonoBehaviourSingleton<CameraSystem>.Instance.UnregisterCameraState(_targetCameraStateDefinition);
		}
	}
}
