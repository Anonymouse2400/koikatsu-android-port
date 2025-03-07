using UnityEngine;

public class MayaCamera : MonoBehaviour
{
	public Camera _camera;

	private void LateUpdate()
	{
		if (_camera != null)
		{
			_camera.fieldOfView = base.transform.localScale.z;
		}
	}
}
