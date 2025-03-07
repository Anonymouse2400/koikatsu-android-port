using UnityEngine;

namespace StrayTech
{
	public class InputBasedCameraStateTransition : MonoBehaviour
	{
		[SerializeField]
		private CameraStateDefinition[] _cameraStateDefinitionArray;

		[SerializeField]
		private KeyCode _transitionKey = KeyCode.C;

		private int _currentStateIndex;

		private bool _pressedLastFrame;

		private void Update()
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance == null || _cameraStateDefinitionArray.Length == 0)
			{
				return;
			}
			if (Input.GetKey(_transitionKey))
			{
				if (!_pressedLastFrame)
				{
					MonoBehaviourSingleton<CameraSystem>.Instance.SetCameraStateTempOverride(_cameraStateDefinitionArray[_currentStateIndex]);
					_currentStateIndex++;
					if (_currentStateIndex > _cameraStateDefinitionArray.Length - 1)
					{
						_currentStateIndex = 0;
					}
				}
				_pressedLastFrame = true;
			}
			else
			{
				_pressedLastFrame = false;
			}
		}
	}
}
