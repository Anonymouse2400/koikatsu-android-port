using UnityEngine;

public class CameraLookObject : MonoBehaviour
{
	public string targetCamera = "Preview Camera";

	private void OnBecameInvisible()
	{
		base.enabled = false;
	}

	private void OnBecameVisible()
	{
		base.enabled = true;
	}

	private void OnWillRenderObject()
	{
	}
}
