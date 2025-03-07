using UnityEngine;

[ExecuteInEditMode]
public class MirrorCamera : MonoBehaviour
{
	private void OnPreRender()
	{
		GL.invertCulling = true;
	}

	private void OnPostRender()
	{
		GL.invertCulling = false;
	}
}
