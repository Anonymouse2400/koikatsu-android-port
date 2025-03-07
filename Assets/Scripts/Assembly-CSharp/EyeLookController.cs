using UnityEngine;

public class EyeLookController : MonoBehaviour
{
	public EyeLookCalc eyeLookScript;

	public int ptnNo;

	public Transform target;

	private void Start()
	{
		if (!target && (bool)Camera.main)
		{
			target = Camera.main.transform;
		}
	}

	private void LateUpdate()
	{
		if (target != null && null != eyeLookScript)
		{
			eyeLookScript.EyeUpdateCalc(target.position, ptnNo);
		}
	}

	public void ForceLateUpdate()
	{
		LateUpdate();
	}
}
