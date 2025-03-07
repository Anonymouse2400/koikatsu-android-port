using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.FirstPerson
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(AudioSource))]
	public class bl_FirstPersonController : MonoBehaviour
	{
		[SerializeField]
		private bool is2D;

		[SerializeField]
		private bool m_IsWalking;

		[SerializeField]
		private float m_WalkSpeed;

		[SerializeField]
		private float m_RunSpeed;

		[Range(0f, 1f)]
		[SerializeField]
		private float m_RunstepLenghten;

		[SerializeField]
		private float m_JumpSpeed;

		[SerializeField]
		private float m_StickToGroundForce;

		[SerializeField]
		private float m_GravityMultiplier;

		[SerializeField]
		private bl_MouseLook m_MouseLook;

		[SerializeField]
		private bool m_UseFovKick;

		[SerializeField]
		private bl_FOVKick m_FovKick = new bl_FOVKick();

		[SerializeField]
		private bool m_UseHeadBob;

		[SerializeField]
		private bl_CurveControlledBob m_HeadBob = new bl_CurveControlledBob();

		[SerializeField]
		private bl_LerpControlledBob m_JumpBob = new bl_LerpControlledBob();

		[SerializeField]
		private float m_StepInterval;

		[SerializeField]
		private AudioClip[] m_FootstepSounds;

		[SerializeField]
		private AudioClip m_JumpSound;

		[SerializeField]
		private AudioClip m_LandSound;

		private Camera m_Camera;

		private bool m_Jump;

		private float m_YRotation;

		private Vector2 m_Input;

		private Vector3 m_MoveDir = Vector3.zero;

		private CharacterController m_CharacterController;

		private CollisionFlags m_CollisionFlags;

		private bool m_PreviouslyGrounded;

		private Vector3 m_OriginalCameraPosition;

		private float m_StepCycle;

		private float m_NextStep;

		private bool m_Jumping;

		private AudioSource m_AudioSource;

		private void Start()
		{
			m_CharacterController = GetComponent<CharacterController>();
			m_Camera = Camera.main;
			m_OriginalCameraPosition = m_Camera.transform.localPosition;
			if (!is2D)
			{
				m_FovKick.Setup(m_Camera);
				m_HeadBob.Setup(m_Camera, m_StepInterval);
			}
			m_StepCycle = 0f;
			m_NextStep = m_StepCycle / 2f;
			m_Jumping = false;
			m_AudioSource = GetComponent<AudioSource>();
			if (!is2D)
			{
				m_MouseLook.Init(base.transform, m_Camera.transform);
			}
		}

		private void Update()
		{
			if (!is2D)
			{
				RotateView();
			}
			if (!m_Jump)
			{
				m_Jump = Input.GetKeyDown(KeyCode.Space);
			}
			if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
			{
				StartCoroutine(m_JumpBob.DoBobCycle());
				PlayLandingSound();
				m_MoveDir.y = 0f;
				m_Jumping = false;
			}
			if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
			{
				m_MoveDir.y = 0f;
			}
			m_PreviouslyGrounded = m_CharacterController.isGrounded;
		}

		private void PlayLandingSound()
		{
			m_AudioSource.clip = m_LandSound;
			m_AudioSource.Play();
			m_NextStep = m_StepCycle + 0.5f;
		}

		private void FixedUpdate()
		{
			float speed;
			GetInput(out speed);
			Vector3 vector = base.transform.forward * m_Input.y + base.transform.right * m_Input.x;
			RaycastHit hitInfo;
			Physics.SphereCast(base.transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f);
			vector = Vector3.ProjectOnPlane(vector, hitInfo.normal).normalized;
			m_MoveDir.x = vector.x * speed;
			if (!is2D)
			{
				m_MoveDir.z = vector.z * speed;
			}
			if (m_CharacterController.isGrounded)
			{
				m_MoveDir.y = 0f - m_StickToGroundForce;
				if (m_Jump)
				{
					m_MoveDir.y = m_JumpSpeed;
					PlayJumpSound();
					m_Jump = false;
					m_Jumping = true;
				}
			}
			else
			{
				m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
			}
			m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
			ProgressStepCycle(speed);
			UpdateCameraPosition(speed);
		}

		private void PlayJumpSound()
		{
			m_AudioSource.clip = m_JumpSound;
			m_AudioSource.Play();
		}

		private void ProgressStepCycle(float speed)
		{
			if (m_CharacterController.velocity.sqrMagnitude > 0f && (m_Input.x != 0f || m_Input.y != 0f))
			{
				m_StepCycle += (m_CharacterController.velocity.magnitude + speed * ((!m_IsWalking) ? m_RunstepLenghten : 1f)) * Time.fixedDeltaTime;
			}
			if (m_StepCycle > m_NextStep)
			{
				m_NextStep = m_StepCycle + m_StepInterval;
				PlayFootStepAudio();
			}
		}

		private void PlayFootStepAudio()
		{
			if (m_CharacterController.isGrounded)
			{
				int num = Random.Range(1, m_FootstepSounds.Length);
				m_AudioSource.clip = m_FootstepSounds[num];
				m_AudioSource.PlayOneShot(m_AudioSource.clip);
				m_FootstepSounds[num] = m_FootstepSounds[0];
				m_FootstepSounds[0] = m_AudioSource.clip;
			}
		}

		private void UpdateCameraPosition(float speed)
		{
			if (!is2D)
			{
				if (m_UseHeadBob)
				{
					Vector3 localPosition;
					if (m_CharacterController.velocity.magnitude > 0f && m_CharacterController.isGrounded)
					{
						m_Camera.transform.localPosition = m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + speed * ((!m_IsWalking) ? m_RunstepLenghten : 1f));
						localPosition = m_Camera.transform.localPosition;
						localPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
					}
					else
					{
						localPosition = m_Camera.transform.localPosition;
						localPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
					}
					m_Camera.transform.localPosition = localPosition;
				}
			}
			else
			{
				Vector3 position = base.transform.position;
				m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, new Vector3(position.x, position.y, m_OriginalCameraPosition.z), Time.deltaTime * 7f);
			}
		}

		private void GetInput(out float speed)
		{
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool isWalking = m_IsWalking;
			m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
			speed = ((!m_IsWalking) ? m_RunSpeed : m_WalkSpeed);
			m_Input = new Vector2(axis, axis2);
			if (m_Input.sqrMagnitude > 1f)
			{
				m_Input.Normalize();
			}
			if (m_IsWalking != isWalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0f)
			{
				StopAllCoroutines();
				StartCoroutine(m_IsWalking ? m_FovKick.FOVKickDown() : m_FovKick.FOVKickUp());
			}
		}

		private void RotateView()
		{
			m_MouseLook.LookRotation(base.transform, m_Camera.transform);
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody attachedRigidbody = hit.collider.attachedRigidbody;
			if (m_CollisionFlags != CollisionFlags.Below && !(attachedRigidbody == null) && !attachedRigidbody.isKinematic)
			{
				attachedRigidbody.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
			}
		}
	}
}
