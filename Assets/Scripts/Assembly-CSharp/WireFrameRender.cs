using UnityEngine;

public class WireFrameRender : MonoBehaviour
{
	public bool wireFrameDraw;

	private void OnPreRender()
	{
		if (wireFrameDraw)
		{
			GL.wireframe = true;
		}
	}

	private void OnPostRender()
	{
		if (wireFrameDraw)
		{
			GL.wireframe = false;
		}
	}
}
