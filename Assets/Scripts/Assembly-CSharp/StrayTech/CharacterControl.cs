using UnityEngine;

namespace StrayTech
{
	[RequireComponent(typeof(Character))]
	public class CharacterControl : MonoBehaviour
	{
		private Character _character;

		private Vector3 _camForward = Vector3.zero;

		private Vector3 _move = Vector3.zero;

		private bool _jump;

		private void Start()
		{
			_character = GetComponent<Character>();
		}

		private void Update()
		{
			if (!_jump)
			{
				_jump = Input.GetButtonDown("Jump");
			}
		}

		private void FixedUpdate()
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null || MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera == null || MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition == null)
			{
				return;
			}
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			bool key = Input.GetKey(KeyCode.C);
			Quaternion rotation;
			CameraSystem.CameraStateEnum stateType;
			if (MonoBehaviourSingleton<CameraSystem>.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
			{
				rotation = MonoBehaviourSingleton<CameraSystem>.Instance.NextCamera.transform.rotation;
				stateType = MonoBehaviourSingleton<CameraSystem>.Instance.NextCameraStateDefinition.State.StateType;
			}
			else
			{
				rotation = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.rotation;
				stateType = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition.State.StateType;
			}
			if (stateType == CameraSystem.CameraStateEnum.FirstPerson)
			{
				_move = axis2 * Vector3.forward * 0.5f;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					_move *= 2f;
				}
				_character.MoveFirstPerson(_move, key, _jump);
			}
			else
			{
				_camForward = Vector3.Scale(rotation * Vector3.forward, new Vector3(1f, 0f, 1f)).normalized;
				_move = (axis2 * _camForward + axis * (rotation * Vector3.right)) * 0.5f;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					_move *= 2f;
				}
				_character.MoveThirdPerson(_move, key, _jump);
			}
			_jump = false;
			if (key)
			{
				MonoBehaviourSingleton<CameraSystem>.Instance.SetUserDefinedFlagValue("PlayerInput_Crouch", true);
			}
			else
			{
				MonoBehaviourSingleton<CameraSystem>.Instance.SetUserDefinedFlagValue("PlayerInput_Crouch", false);
			}
			if (Input.GetKey(KeyCode.E))
			{
				MonoBehaviourSingleton<CameraSystem>.Instance.SetUserDefinedFlagValue("PlayerInput_Use", true);
			}
			else
			{
				MonoBehaviourSingleton<CameraSystem>.Instance.SetUserDefinedFlagValue("PlayerInput_Use", false);
			}
		}
	}
}
