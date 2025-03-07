using UnityEngine;

namespace StrayTech
{
	public class DirectionalTriggerGate : MonoBehaviour, ITriggerGate
	{
		[SerializeField]
		[Range(0f, 360f)]
		[Tooltip("The primary direction the volume can be triggered from.")]
		private float _angle;

		[SerializeField]
		[Range(0f, 180f)]
		[Tooltip("The span of the primary direction the volume can be triggered from.")]
		private float _angleSpan = 90f;

		private bool _enteredFromValidDirection;

		private Vector3 _validDirection;

		public bool IsActive
		{
			get
			{
				return base.gameObject.activeInHierarchy;
			}
		}

		private void Start()
		{
			Vector3 forward = base.transform.forward;
			forward.y = 0f;
			_validDirection = Quaternion.AngleAxis(_angle, Vector3.up) * forward.normalized;
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null)
			{
				base.enabled = false;
			}
		}

		public void TriggerWasEntered(Collider other)
		{
			Vector3 vector = other.transform.position - base.transform.position;
			vector.y = 0f;
			Vector3 normalized = vector.normalized;
			float num = Vector3.Angle(normalized, _validDirection);
			if (num <= _angleSpan)
			{
				_enteredFromValidDirection = true;
			}
			else
			{
				_enteredFromValidDirection = false;
			}
		}

		public bool IsTriggerBlocked()
		{
			return !_enteredFromValidDirection;
		}
	}
}
