using UnityEngine;

public class CameraEffectorColorMask : MonoBehaviour
{
	private Camera myCamera;

	private CameraEffector effector;

	private Camera effectorCamera;

	public bool Enabled
	{
		get
		{
			return base.enabled;
		}
		set
		{
			myCamera.enabled = value;
			base.enabled = value;
			effector.amplifyColor.MaskTexture = (value ? myCamera.targetTexture : null);
		}
	}

	private void Awake()
	{
		myCamera = GetComponent<Camera>();
		if (myCamera == null)
		{
			Object.Destroy(this);
		}
		effector = base.transform.parent.GetComponent<CameraEffector>();
		if (effector == null)
		{
			Object.Destroy(base.gameObject);
		}
		effectorCamera = effector.GetComponent<Camera>();
		myCamera.CopyFrom(effectorCamera);
		myCamera.clearFlags = CameraClearFlags.Color;
		myCamera.backgroundColor = Color.black;
		myCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
		Enabled = false;
	}

	private void Update()
	{
		myCamera.fieldOfView = effectorCamera.fieldOfView;
	}
}
