using UnityEngine;

public class MayaCameraConverter : MonoBehaviour
{
	private Camera[] _camera;

	private void Awake()
	{
		_camera = GetComponentsInChildren<Camera>();
	}

	private void LateUpdate()
	{
		Camera[] camera = _camera;
		foreach (Camera camera2 in camera)
		{
			camera2.nearClipPlane = base.transform.localScale.x;
			camera2.farClipPlane = base.transform.localScale.y;
			camera2.fieldOfView = base.transform.localScale.z;
		}
	}
}
