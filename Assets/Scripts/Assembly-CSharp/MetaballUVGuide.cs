using UnityEngine;

public class MetaballUVGuide : MonoBehaviour
{
	public float uScale = 1f;

	public float vScale = 1f;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmosSelected()
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(new Vector3(uScale * 0.5f, vScale * 0.5f, 0f), new Vector3(uScale, vScale, 15f));
		Gizmos.matrix = matrix;
	}
}
