using UnityEngine;

namespace StrayTech
{
	public class CameraStateModifierTrigger : CameraSystemTriggerBase
	{
		[Tooltip("The target Camera Modifier to enable and/or disable.")]
		[SerializeField]
		private CameraStateModifierBase _cameraStateModifierTarget;

		[Tooltip("Ignore the OnTriggerExit event?")]
		[SerializeField]
		private bool _ignoreTriggerExit;

		protected override void TriggerEntered()
		{
			_cameraStateModifierTarget.Enable();
		}

		protected override void TriggerExited()
		{
			if (!_ignoreTriggerExit)
			{
				_cameraStateModifierTarget.Disable();
			}
		}
	}
}
