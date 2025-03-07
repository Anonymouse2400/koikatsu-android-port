using UnityEngine;

public class SkinnedMetaballSeed : MetaballSeedBase
{
	public SkinnedMeshRenderer skinnedMesh;

	private SkinnedMetaballCell _rootCell;

	public override Mesh Mesh
	{
		get
		{
			return skinnedMesh.sharedMesh;
		}
		set
		{
			skinnedMesh.sharedMesh = value;
		}
	}

	public override bool IsTreeShape
	{
		get
		{
			return true;
		}
	}

	[ContextMenu("CreateMesh")]
	public override void CreateMesh()
	{
		CleanupBoneRoot();
		_rootCell = new SkinnedMetaballCell();
		_rootCell.radius = sourceRoot.Radius;
		_rootCell.tag = sourceRoot.gameObject.name;
		_rootCell.density = sourceRoot.Density;
		_rootCell.modelPosition = sourceRoot.transform.position - base.transform.position;
		Matrix4x4 worldToLocalMatrix = skinnedMesh.transform.worldToLocalMatrix;
		ConstructTree(sourceRoot.transform, _rootCell, worldToLocalMatrix);
		Vector3 uDir;
		Vector3 vDir;
		Vector3 offset;
		GetUVBaseVector(out uDir, out vDir, out offset);
		Bounds? bounds = null;
		if (bUseFixedBounds)
		{
			bounds = fixedBounds;
		}
		Mesh out_mesh;
		Transform[] out_bones;
		_errorMsg = MetaballBuilder.Instance.CreateMeshWithSkeleton(_rootCell, boneRoot.transform, powerThreshold, base.GridSize, uDir, vDir, offset, out out_mesh, out out_bones, cellObjPrefab, bReverse, bounds, bAutoGridSize, autoGridQuarity);
		if (string.IsNullOrEmpty(_errorMsg))
		{
			out_mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 500f);
			skinnedMesh.bones = out_bones;
			skinnedMesh.sharedMesh = out_mesh;
			skinnedMesh.localBounds = new Bounds(Vector3.zero, Vector3.one * 500f);
			skinnedMesh.rootBone = boneRoot;
			EnumBoneNodes();
		}
	}

	private void ConstructTree(Transform node, SkinnedMetaballCell cell, Matrix4x4 toLocalMtx)
	{
		for (int i = 0; i < node.childCount; i++)
		{
			Transform child = node.GetChild(i);
			MetaballNode component = child.GetComponent<MetaballNode>();
			if (component != null)
			{
				SkinnedMetaballCell skinnedMetaballCell = cell.AddChild(toLocalMtx * (child.transform.position - base.transform.position), component.Radius, 0f);
				skinnedMetaballCell.tag = child.gameObject.name;
				skinnedMetaballCell.density = component.Density;
				ConstructTree(child, skinnedMetaballCell, toLocalMtx);
			}
		}
	}
}
