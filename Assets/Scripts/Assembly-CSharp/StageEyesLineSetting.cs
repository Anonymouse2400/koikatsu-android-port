using UnityEngine;

public class StageEyesLineSetting : MonoBehaviour
{
	public SaveData.Heroine heroine;

	private void EyesLine(int _eyeline)
	{
		if (heroine != null && !(heroine.chaCtrl == null))
		{
			heroine.chaCtrl.ChangeLookEyesPtn(_eyeline);
		}
	}

	private void EyesLeapSpeed_0(float _speed)
	{
		if (heroine != null && !(heroine.chaCtrl == null))
		{
			heroine.chaCtrl.eyeLookCtrl.eyeLookScript.eyeTypeStates[0].leapSpeed = _speed;
		}
	}

	private void EyesLeapSpeed_1(float _speed)
	{
		if (heroine != null && !(heroine.chaCtrl == null))
		{
			heroine.chaCtrl.eyeLookCtrl.eyeLookScript.eyeTypeStates[1].leapSpeed = _speed;
		}
	}

	private void EyesLeapSpeed_3(float _speed)
	{
		if (heroine != null && !(heroine.chaCtrl == null))
		{
			heroine.chaCtrl.eyeLookCtrl.eyeLookScript.eyeTypeStates[3].leapSpeed = _speed;
		}
	}
}
