using System.Collections.Generic;
using UnityEngine;

public class NormalControl
{
	public static void GetNormalData(GameObject obj, List<Vector3> normal)
	{
		if (null == obj)
		{
			return;
		}
		MeshFilter meshFilter = new MeshFilter();
		meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (null != meshFilter)
		{
			Vector3[] normals = meshFilter.sharedMesh.normals;
			foreach (Vector3 item in normals)
			{
				normal.Add(item);
			}
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
		skinnedMeshRenderer = obj.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if ((bool)skinnedMeshRenderer)
		{
			Vector3[] normals2 = skinnedMeshRenderer.sharedMesh.normals;
			foreach (Vector3 item2 in normals2)
			{
				normal.Add(item2);
			}
		}
	}
}
