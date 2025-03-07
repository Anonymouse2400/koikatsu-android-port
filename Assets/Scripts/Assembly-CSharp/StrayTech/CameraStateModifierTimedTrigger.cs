using System.Collections;
using UnityEngine;

namespace StrayTech
{
	public class CameraStateModifierTimedTrigger : CameraSystemTriggerBase
	{
		[Tooltip("The target Camera Modifier to enable and/or disable.")]
		[SerializeField]
		private CameraStateModifierBase _cameraStateModifierTarget;

		[Tooltip("The modifier will be disabled after this duration.")]
		[SerializeField]
		private float _enabledDuration = 1f;

		protected override void TriggerEntered()
		{
			_cameraStateModifierTarget.Enable();
			StartCoroutine(DoTimedDisable());
		}

		protected override void TriggerExited()
		{
		}

		private IEnumerator DoTimedDisable()
		{
			yield return new WaitForSeconds(_enabledDuration);
			_cameraStateModifierTarget.Disable();
			if (_singleUseTrigger)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
