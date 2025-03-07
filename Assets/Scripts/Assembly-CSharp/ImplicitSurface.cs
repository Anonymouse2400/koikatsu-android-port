using UnityEngine;

public class ImplicitSurface : ImplicitSurfaceMeshCreaterBase
{
	public MeshFilter meshFilter;

	public MeshCollider meshCollider;

	protected Vector3[] _positionMap;

	protected float[] _powerMap;

	protected float[] _powerMapMask;

	protected int _countX;

	protected int _countY;

	protected int _countZ;

	private bool _bMapsDirty = true;

	public MeshFilter MeshFilter
	{
		get
		{
			if (meshFilter == null)
			{
				meshFilter = GetComponent<MeshFilter>();
			}
			return meshFilter;
		}
	}

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

	protected void ResetMaps()
	{
		int maxGridCellCount = MetaballBuilder.MaxGridCellCount;
		float num = 1f;
		if (bAutoGridSize)
		{
			int num2 = (int)((float)maxGridCellCount * Mathf.Clamp01(autoGridQuarity));
			num = Mathf.Pow(fixedBounds.size.x * fixedBounds.size.y * fixedBounds.size.z / (float)num2, 1f / 3f);
		}
		else
		{
			num = gridSize;
		}
		int num3 = Mathf.CeilToInt(fixedBounds.extents.x / num) + 1;
		int num4 = Mathf.CeilToInt(fixedBounds.extents.y / num) + 1;
		int num5 = Mathf.CeilToInt(fixedBounds.extents.z / num) + 1;
		_countX = num3 * 2;
		_countY = num4 * 2;
		_countZ = num5 * 2;
		Vector3 vector = new Vector3((float)num3 * num, (float)num4 * num, (float)num5 * num);
		Vector3 vector2 = fixedBounds.center - vector;
		int countX = _countX;
		int num6 = _countX * _countY;
		int num7 = _countX * _countY * _countZ;
		_positionMap = new Vector3[num7];
		_powerMap = new float[num7];
		_powerMapMask = new float[num7];
		for (int i = 0; i < num7; i++)
		{
			_powerMap[i] = 0f;
		}
		for (int j = 0; j < _countZ; j++)
		{
			for (int k = 0; k < _countY; k++)
			{
				for (int l = 0; l < _countX; l++)
				{
					int num8 = l + k * countX + j * num6;
					_positionMap[num8] = vector2 + new Vector3(num * (float)l, num * (float)k, num * (float)j);
					if (j == 0 || j == _countZ - 1 || k == 0 || k == _countY - 1 || l == 0 || l == _countX - 1)
					{
						_powerMapMask[num8] = 0f;
					}
					else
					{
						_powerMapMask[num8] = 1f;
					}
				}
			}
		}
		InitializePowerMap();
		_bMapsDirty = false;
	}

	protected virtual void InitializePowerMap()
	{
		int num = _countX * _countY * _countZ;
		for (int i = 0; i < num; i++)
		{
			_powerMap[i] = 0f;
		}
	}

	public override void CreateMesh()
	{
		if (_bMapsDirty)
		{
			ResetMaps();
		}
		Vector3 uDir;
		Vector3 vDir;
		Vector3 offset;
		GetUVBaseVector(out uDir, out vDir, out offset);
		Mesh mesh = MetaballBuilder.Instance.CreateImplicitSurfaceMesh(_countX, _countY, _countZ, _positionMap, _powerMap, bReverse, powerThreshold, uDir, vDir, offset);
		mesh.RecalculateBounds();
		Mesh = mesh;
		if (meshCollider != null)
		{
			meshCollider.sharedMesh = mesh;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube(fixedBounds.center + base.transform.position, fixedBounds.size);
	}
}
