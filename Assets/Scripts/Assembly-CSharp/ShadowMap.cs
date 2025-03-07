using UnityEngine;

[ExecuteInEditMode]
public class ShadowMap : MonoBehaviour
{
	[SerializeField]
	private Material m_mat;

	[SerializeField]
	private Renderer m_renderer;

	private void OnWillRenderObject()
	{
		Camera current = Camera.current;
		if (!(current.name == "Light"))
		{
			return;
		}
		Matrix4x4 worldToCameraMatrix = current.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(current.projectionMatrix, false);
		Matrix4x4 matrix4x = gPUProjectionMatrix * worldToCameraMatrix;
		Matrix4x4 matrix4x2 = default(Matrix4x4);
		matrix4x2.SetRow(0, new Vector4(0.5f, 0f, 0f, 0.5f));
		matrix4x2.SetRow(1, new Vector4(0f, 0.5f, 0f, 0.5f));
		matrix4x2.SetRow(2, new Vector4(0f, 0f, 0.5f, 0.5f));
		matrix4x2.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		if (m_mat != null)
		{
			m_mat.SetMatrix("_LightVP", matrix4x2 * matrix4x);
		}
		if (!(m_renderer != null))
		{
			return;
		}
		if (m_renderer.material != null)
		{
			m_renderer.material.SetMatrix("_LightVP", matrix4x2 * matrix4x);
		}
		if (m_renderer.materials != null)
		{
			Material[] materials = m_renderer.materials;
			foreach (Material material in materials)
			{
				material.SetMatrix("_LightVP", matrix4x2 * matrix4x);
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
