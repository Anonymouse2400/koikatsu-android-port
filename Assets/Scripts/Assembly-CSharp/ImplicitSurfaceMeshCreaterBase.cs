using UnityEngine;

public abstract class ImplicitSurfaceMeshCreaterBase : MonoBehaviour
{
	public float gridSize = 0.2f;

	[Tooltip("Ignore gridSize and use automatically determined value by autoGridQuarity")]
	public bool bAutoGridSize;

	[Range(0.005f, 1f)]
	public float autoGridQuarity = 0.2f;

	public MetaballUVGuide uvProjectNode;

	public float powerThreshold = 0.15f;

	public bool bReverse;

	public Bounds fixedBounds = new Bounds(Vector3.zero, Vector3.one * 10f);

	public float GridSize
	{
		get
		{
			return Mathf.Max(float.Epsilon, gridSize);
		}
	}

	public abstract Mesh Mesh { get; set; }

	public abstract void CreateMesh();

	protected virtual void Update()
	{
	}

	protected void GetUVBaseVector(out Vector3 uDir, out Vector3 vDir, out Vector3 offset)
	{
		if (uvProjectNode != null)
		{
			float num = Mathf.Max(uvProjectNode.uScale, 0.001f);
			float num2 = Mathf.Max(uvProjectNode.vScale, 0.001f);
			uDir = uvProjectNode.transform.right / num;
			vDir = uvProjectNode.transform.up / num2;
			offset = -uvProjectNode.transform.localPosition;
		}
		else
		{
			uDir = Vector3.right;
			vDir = Vector3.up;
			offset = Vector3.zero;
		}
	}
}
