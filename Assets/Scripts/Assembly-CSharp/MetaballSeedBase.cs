using System.Collections.Generic;
using UnityEngine;

public abstract class MetaballSeedBase : ImplicitSurfaceMeshCreaterBase
{
	public Transform boneRoot;

	public MetaballNode sourceRoot;

	public MetaballCellObject cellObjPrefab;

	public float baseRadius = 1f;

	public bool bUseFixedBounds;

	protected string _errorMsg;

	[SerializeField]
	private GameObject[] _boneNodes = new GameObject[0];

	public abstract bool IsTreeShape { get; }

	private void OnDrawGizmos()
	{
		if (bUseFixedBounds)
		{
			Gizmos.color = Color.white;
			Gizmos.DrawWireCube(fixedBounds.center + base.transform.position, fixedBounds.size);
		}
	}

	protected void EnumBoneNodes()
	{
		List<GameObject> list = new List<GameObject>();
		EnumerateGameObjects(boneRoot.gameObject, list);
		_boneNodes = list.ToArray();
	}

	private void EnumerateGameObjects(GameObject parent, List<GameObject> list)
	{
		for (int i = 0; i < parent.transform.childCount; i++)
		{
			GameObject gameObject = parent.transform.GetChild(i).gameObject;
			list.Add(gameObject);
			EnumerateGameObjects(gameObject, list);
		}
	}

	protected void CleanupBoneRoot()
	{
		if (_boneNodes == null)
		{
			_boneNodes = new GameObject[0];
		}
		int num = _boneNodes.Length;
		for (int i = 0; i < num; i++)
		{
			if (!(_boneNodes[i] == null))
			{
				_boneNodes[i].transform.DetachChildren();
				Object.Destroy(_boneNodes[i]);
			}
		}
	}
}
