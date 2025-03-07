using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FarCamera : MonoBehaviour
{
	public GameObject target;

	private Vector3 _relativePosition;

	private void Start()
	{
		_relativePosition = base.transform.position - target.transform.position;
	}

	private void FixedUpdate()
	{
		base.transform.position = target.transform.position + _relativePosition;
	}
}
