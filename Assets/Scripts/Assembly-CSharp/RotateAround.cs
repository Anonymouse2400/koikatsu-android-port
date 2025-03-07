using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public Transform Target;

	public float Speed = 1f;

	protected Vector3 m_TargetPosition;

	private void Start()
	{
		m_TargetPosition = Target.position;
	}

	private void Update()
	{
		base.transform.RotateAround(m_TargetPosition, Vector3.up, Time.deltaTime * Speed);
	}
}
