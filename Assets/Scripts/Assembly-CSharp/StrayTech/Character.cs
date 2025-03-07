using UnityEngine;

namespace StrayTech
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class Character : MonoBehaviour
	{
		[SerializeField]
		private float _movingTurnSpeed = 360f;

		[SerializeField]
		private float _stationaryTurnSpeed = 180f;

		[SerializeField]
		private float _jumpPower = 12f;

		[Range(1f, 4f)]
		[SerializeField]
		private float _gravityMultiplier = 2f;

		[SerializeField]
		private float _runCycleLegOffset = 0.2f;

		[SerializeField]
		private float _moveSpeedMultiplier = 1f;

		[SerializeField]
		private float _animSpeedMultiplier = 1f;

		[SerializeField]
		private float _groundCheckDistance = 0.1f;

		private Rigidbody _rigidbody;

		private Animator _animator;

		private bool _isGrounded;

		private float _origGroundCheckDistance;

		private Vector3 _groundNormal;

		private float _turnAmount;

		private float _forwardAmount;

		private CapsuleCollider _capsule;

		private float _capsuleHeight;

		private Vector3 _capsuleCenter;

		private bool _crouching;

		private void Start()
		{
			_animator = GetComponent<Animator>();
			_rigidbody = GetComponent<Rigidbody>();
			_capsule = GetComponent<CapsuleCollider>();
			_capsuleHeight = _capsule.height;
			_capsuleCenter = _capsule.center;
			_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			_origGroundCheckDistance = _groundCheckDistance;
		}

		public void MoveThirdPerson(Vector3 move, bool crouch, bool jump)
		{
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			move = base.transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, _groundNormal);
			_turnAmount = Mathf.Atan2(move.x, move.z);
			_forwardAmount = move.z;
			ApplyExtraTurnRotation();
			if (_isGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}
			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();
			UpdateAnimator(move);
		}

		public void MoveFirstPerson(Vector3 move, bool crouch, bool jump)
		{
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, _groundNormal);
			_turnAmount = 0f;
			_forwardAmount = move.z;
			if (_isGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}
			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();
			UpdateAnimator(move);
		}

		private void ScaleCapsuleForCrouching(bool crouch)
		{
			if (_isGrounded && crouch)
			{
				if (!_crouching)
				{
					_capsule.height *= 0.5f;
					_capsule.center *= 0.5f;
					_crouching = true;
				}
				return;
			}
			Ray ray = new Ray(_rigidbody.position + Vector3.up * _capsule.radius * 0.5f, Vector3.up);
			float maxDistance = _capsuleHeight - _capsule.radius * 0.5f;
			if (Physics.SphereCast(ray, _capsule.radius * 0.5f, maxDistance))
			{
				_crouching = true;
				return;
			}
			_capsule.height = _capsuleHeight;
			_capsule.center = _capsuleCenter;
			_crouching = false;
		}

		private void PreventStandingInLowHeadroom()
		{
			if (!_crouching)
			{
				Ray ray = new Ray(_rigidbody.position + Vector3.up * _capsule.radius * 0.5f, Vector3.up);
				float maxDistance = _capsuleHeight - _capsule.radius * 0.5f;
				if (Physics.SphereCast(ray, _capsule.radius * 0.5f, maxDistance))
				{
					_crouching = true;
				}
			}
		}

		private void UpdateAnimator(Vector3 move)
		{
			_animator.SetFloat("Forward", _forwardAmount, 0.1f, Time.deltaTime);
			_animator.SetFloat("Turn", _turnAmount, 0.1f, Time.deltaTime);
			_animator.SetBool("Crouch", _crouching);
			_animator.SetBool("OnGround", _isGrounded);
			if (!_isGrounded)
			{
				_animator.SetFloat("Jump", _rigidbody.velocity.y);
			}
			float num = Mathf.Repeat(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime + _runCycleLegOffset, 1f);
			float value = (float)((num < 0.5f) ? 1 : (-1)) * _forwardAmount;
			if (_isGrounded)
			{
				_animator.SetFloat("JumpLeg", value);
			}
			if (_isGrounded && move.magnitude > 0f)
			{
				_animator.speed = _animSpeedMultiplier;
			}
			else
			{
				_animator.speed = 1f;
			}
		}

		private void HandleAirborneMovement()
		{
			Vector3 force = Physics.gravity * _gravityMultiplier - Physics.gravity;
			_rigidbody.AddForce(force);
			_groundCheckDistance = ((!(_rigidbody.velocity.y < 0f)) ? 0.01f : _origGroundCheckDistance);
		}

		private void HandleGroundedMovement(bool crouch, bool jump)
		{
			if (jump && !crouch && _animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, _jumpPower, _rigidbody.velocity.z);
				_isGrounded = false;
				_animator.applyRootMotion = false;
				_groundCheckDistance = 0.1f;
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(_stationaryTurnSpeed, _movingTurnSpeed, _forwardAmount);
			base.transform.Rotate(0f, _turnAmount * num * Time.deltaTime, 0f);
		}

		private void CheckGroundStatus()
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position + Vector3.up * 0.1f, Vector3.down, out hitInfo, _groundCheckDistance))
			{
				_groundNormal = hitInfo.normal;
				_isGrounded = true;
				_animator.applyRootMotion = true;
			}
			else
			{
				_isGrounded = false;
				_groundNormal = Vector3.up;
				_animator.applyRootMotion = false;
			}
		}

		public void OnAnimatorMove()
		{
			if (_isGrounded && Time.deltaTime > 0f)
			{
				Vector3 velocity = _animator.deltaPosition * _moveSpeedMultiplier / Time.deltaTime;
				velocity.y = _rigidbody.velocity.y;
				_rigidbody.velocity = velocity;
			}
		}
	}
}
