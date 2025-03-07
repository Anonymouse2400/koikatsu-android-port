using UnityEngine;

namespace StrayTech
{
	public class UserDefinedFlagTriggerGate : MonoBehaviour, ITriggerGate
	{
		[SerializeField]
		[Tooltip("If this user defined flag is false, OnTriggerEnter logic will be bypassed.")]
		private string _userDefinedFlagName;

		public bool IsActive
		{
			get
			{
				return base.gameObject.activeInHierarchy;
			}
		}

		private void Start()
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				base.enabled = false;
			}
		}

		public void TriggerWasEntered(Collider other)
		{
		}

		public bool IsTriggerBlocked()
		{
			return !MonoBehaviourSingleton<CameraSystem>.Instance.GetUserDefinedFlagValue(_userDefinedFlagName);
		}
	}
}
