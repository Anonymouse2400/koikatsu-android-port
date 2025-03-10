using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

namespace RootMotion.Demos
{
	public class VRSetup : MonoBehaviour
	{
		public Text text;

		public GameObject model;

		public GameObject[] enableOnR;

		public VRCharacterController characterController;

		public bool disableMovement;

		private float moveSpeed;

		public bool isFinished { get; private set; }

		private void Awake()
		{
			GameObject[] array = enableOnR;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(false);
			}
			Cursor.lockState = CursorLockMode.Locked;
			if (characterController != null)
			{
				moveSpeed = characterController.moveSpeed;
				characterController.moveSpeed = 0f;
			}
		}

		private void LateUpdate()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				Application.Quit();
			}
			if (!isFinished && characterController != null)
			{
				characterController.transform.rotation = Quaternion.identity;
			}
			if (!Input.GetKeyDown(KeyCode.R))
			{
				return;
			}
			GameObject[] array = enableOnR;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(true);
			}
			UnityEngine.VR.InputTracking.Recenter();
			text.gameObject.SetActive(false);
			if (characterController != null)
			{
				if (!disableMovement)
				{
					characterController.moveSpeed = moveSpeed;
				}
				characterController.transform.position += Vector3.up * 0.001f;
			}
			isFinished = true;
		}
	}
}
