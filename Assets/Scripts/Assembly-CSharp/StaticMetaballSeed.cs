using UnityEngine;

public class StaticMetaballSeed : MetaballSeedBase
{
	public MeshFilter meshFilter;

	public Vector3 fixedMin = -Vector3.one;

	public Vector3 fixedMax = Vector3.one;

	private MetaballCellCluster _cellCluster;

	private Bounds boundsShow = default(Bounds);

	public override Mesh Mesh
	{
		get
		{
			return meshFilter.sharedMesh;
		}
		set
		{
			meshFilter.sharedMesh = value;
		}
	}

	public override bool IsTreeShape
	{
		get
		{
			return false;
		}
	}

	private void ConstructCellCluster(MetaballCellCluster cluster, Transform parentNode, Matrix4x4 toLocalMtx, Transform meshTrans)
	{
		for (int i = 0; i < parentNode.childCount; i++)
		{
			Transform child = parentNode.GetChild(i);
			MetaballNode component = child.GetComponent<MetaballNode>();
			if (component != null)
			{
				MetaballCell metaballCell = _cellCluster.AddCell(meshTrans.InverseTransformPoint(child.position), 0f, component.Radius, child.gameObject.name);
				metaballCell.density = component.Density;
			}
			ConstructCellCluster(cluster, child, toLocalMtx, meshTrans);
		}
	}

	private void WorldPositionBounds(Transform parentNode, ref Bounds bounds)
	{
		for (int i = 0; i < parentNode.childCount; i++)
		{
			Transform child = parentNode.GetChild(i);
			MetaballNode component = child.GetComponent<MetaballNode>();
			if (component != null)
			{
				for (int j = 0; j < 3; j++)
				{
					float num = child.transform.position[j] - component.Radius;
					if (num < bounds.min[j])
					{
						Vector3 min = bounds.min;
						min[j] = num;
						bounds.min = min;
					}
					num = child.transform.position[j] + component.Radius;
					if (num > bounds.max[j])
					{
						Vector3 max = bounds.max;
						max[j] = num;
						bounds.max = max;
					}
				}
			}
			WorldPositionBounds(child, ref bounds);
		}
	}

	private bool WorldPositionBoundsFirst(Transform parentNode, ref Bounds bounds)
	{
		for (int i = 0; i < parentNode.childCount; i++)
		{
			Transform child = parentNode.GetChild(i);
			MetaballNode component = child.GetComponent<MetaballNode>();
			if (component != null)
			{
				for (int j = 0; j < 3; j++)
				{
					float value = child.transform.position[j] - component.Radius;
					Vector3 min = bounds.min;
					min[j] = value;
					bounds.min = min;
					min = bounds.max;
					min[j] = value;
					bounds.max = min;
				}
				return true;
			}
			if (WorldPositionBoundsFirst(child, ref bounds))
			{
				return true;
			}
		}
		return false;
	}

	[ContextMenu("CreateMesh")]
	public override void CreateMesh()
	{
		CleanupBoneRoot();
		_cellCluster = new MetaballCellCluster();
		Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
		WorldPositionBoundsFirst(sourceRoot.transform, ref bounds);
		WorldPositionBounds(sourceRoot.transform, ref bounds);
		meshFilter.transform.position = bounds.center;
		Matrix4x4 worldToLocalMatrix = meshFilter.transform.worldToLocalMatrix;
		ConstructCellCluster(_cellCluster, sourceRoot.transform, worldToLocalMatrix, meshFilter.transform);
		Vector3 uDir;
		Vector3 vDir;
		Vector3 offset;
		GetUVBaseVector(out uDir, out vDir, out offset);
		Bounds? bounds2 = null;
		if (bUseFixedBounds)
		{
			bounds2 = fixedBounds;
		}
		Mesh out_mesh;
		_errorMsg = MetaballBuilder.Instance.CreateMesh(_cellCluster, boneRoot.transform, powerThreshold, base.GridSize, uDir, vDir, offset, out out_mesh, fixedMin, fixedMax, out boundsShow, cellObjPrefab, bReverse, bounds2, bAutoGridSize, autoGridQuarity);
		if (string.IsNullOrEmpty(_errorMsg))
		{
			out_mesh.RecalculateBounds();
			meshFilter.sharedMesh = out_mesh;
			EnumBoneNodes();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		if ((bool)meshFilter && (bool)Mesh)
		{
			Gizmos.DrawWireCube(meshFilter.transform.position + Mesh.bounds.center, Mesh.bounds.size);
		}
		Gizmos.color = Color.green;
		if ((bool)meshFilter)
		{
			Gizmos.DrawWireCube(meshFilter.transform.position + boundsShow.center, boundsShow.size);
		}
	}
}
