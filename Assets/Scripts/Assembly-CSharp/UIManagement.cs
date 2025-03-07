using StrayTech;
using UnityEngine;
using UnityEngine.UI;

public class UIManagement : MonoBehaviour
{
	[SerializeField]
	private Text _currentControlsText;

	[SerializeField]
	private Text _cameraStateText;

	[SerializeField]
	private Text _cameraModifierText;

	private void Update()
	{
		if (!(MonoBehaviourSingleton<CameraSystem>.Instance != null))
		{
			return;
		}
		if (_currentControlsText != null)
		{
			bool key = Input.GetKey(KeyCode.C);
			bool key2 = Input.GetKey(KeyCode.E);
			bool key3 = Input.GetKey(KeyCode.LeftShift);
			bool buttonDown = Input.GetButtonDown("Jump");
			float axis = Input.GetAxis("Horizontal");
			float axis2 = Input.GetAxis("Vertical");
			_currentControlsText.text = string.Empty;
			if (string.IsNullOrEmpty(_currentControlsText.text))
			{
				if (!Mathf.Approximately(axis2, 0f))
				{
					_currentControlsText.text += ((!(axis2 > 0f)) ? "▼, " : "▲, ");
				}
				if (!Mathf.Approximately(axis, 0f))
				{
					_currentControlsText.text += ((!(axis > 0f)) ? "◄, " : "►, ");
				}
				if (buttonDown)
				{
					_currentControlsText.text += "Jump, ";
				}
				if (key3)
				{
					_currentControlsText.text += "Run, ";
				}
				if (key)
				{
					_currentControlsText.text += "Crouch, ";
				}
				if (key2)
				{
					_currentControlsText.text += "Use, ";
				}
			}
		}
		if (_cameraStateText != null)
		{
			if (MonoBehaviourSingleton<CameraSystem>.Instance.SystemStatus == CameraSystem.CameraSystemStatus.Transitioning)
			{
				_cameraStateText.text = MonoBehaviourSingleton<CameraSystem>.Instance.NextCameraStateDefinition.State.StateType.ToString();
			}
			else if (MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition != null)
			{
				_cameraStateText.text = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCameraStateDefinition.State.StateType.ToString();
			}
			else
			{
				_cameraStateText.text = string.Empty;
			}
		}
		if (!(_cameraModifierText != null))
		{
			return;
		}
		_cameraModifierText.text = string.Empty;
		foreach (CameraStateModifierBase cameraStateModifier in MonoBehaviourSingleton<CameraSystem>.Instance.CameraStateModifiers)
		{
			if (string.IsNullOrEmpty(_cameraModifierText.text))
			{
				_cameraModifierText.text = cameraStateModifier.Name + "\n";
				continue;
			}
			Text cameraModifierText = _cameraModifierText;
			cameraModifierText.text = cameraModifierText.text + cameraStateModifier.Name + "\n";
		}
	}
}
