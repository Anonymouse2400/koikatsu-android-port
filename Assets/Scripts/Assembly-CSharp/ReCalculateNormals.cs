using UnityEngine;

public class ReCalculateNormals : MonoBehaviour
{
	public void Update()
	{
		Mesh mesh = null;
		MeshFilter meshFilter = new MeshFilter();
		meshFilter = base.gameObject.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if ((bool)meshFilter)
		{
			mesh = meshFilter.sharedMesh;
		}
		else
		{
			SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
			skinnedMeshRenderer = base.gameObject.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			mesh = skinnedMeshRenderer.sharedMesh;
		}
		if (null != mesh)
		{
			mesh.RecalculateNormals();
		}
	}
}
