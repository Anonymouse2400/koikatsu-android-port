using System.Collections.Generic;
using UnityEngine;

public class UVControl
{
	public static void GetUVData(GameObject obj, List<Vector2> uv, int index)
	{
		if (null == obj)
		{
			return;
		}
		MeshFilter meshFilter = new MeshFilter();
		meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (null != meshFilter)
		{
			switch (index)
			{
			case 0:
			{
				Vector2[] uv5 = meshFilter.sharedMesh.uv;
				foreach (Vector2 item4 in uv5)
				{
					uv.Add(item4);
				}
				break;
			}
			case 1:
			{
				Vector2[] uv3 = meshFilter.sharedMesh.uv2;
				foreach (Vector2 item2 in uv3)
				{
					uv.Add(item2);
				}
				break;
			}
			case 2:
			{
				Vector2[] uv4 = meshFilter.sharedMesh.uv3;
				foreach (Vector2 item3 in uv4)
				{
					uv.Add(item3);
				}
				break;
			}
			case 3:
			{
				Vector2[] uv2 = meshFilter.sharedMesh.uv4;
				foreach (Vector2 item in uv2)
				{
					uv.Add(item);
				}
				break;
			}
			}
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
		skinnedMeshRenderer = obj.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (!skinnedMeshRenderer)
		{
			return;
		}
		switch (index)
		{
		case 0:
		{
			Vector2[] uv9 = skinnedMeshRenderer.sharedMesh.uv;
			foreach (Vector2 item8 in uv9)
			{
				uv.Add(item8);
			}
			break;
		}
		case 1:
		{
			Vector2[] uv7 = skinnedMeshRenderer.sharedMesh.uv2;
			foreach (Vector2 item6 in uv7)
			{
				uv.Add(item6);
			}
			break;
		}
		case 2:
		{
			Vector2[] uv8 = skinnedMeshRenderer.sharedMesh.uv3;
			foreach (Vector2 item7 in uv8)
			{
				uv.Add(item7);
			}
			break;
		}
		case 3:
		{
			Vector2[] uv6 = skinnedMeshRenderer.sharedMesh.uv4;
			foreach (Vector2 item5 in uv6)
			{
				uv.Add(item5);
			}
			break;
		}
		}
	}

	public static void GetRangeIndex(GameObject obj, out int[] rangeIndex)
	{
		rangeIndex = null;
		if (null == obj)
		{
			return;
		}
		List<int> list = new List<int>();
		MeshFilter meshFilter = new MeshFilter();
		meshFilter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
		if (null != meshFilter)
		{
			for (int i = 0; i < meshFilter.sharedMesh.colors.Length; i++)
			{
				if (meshFilter.sharedMesh.colors[i].r == 1f)
				{
					list.Add(i);
				}
			}
		}
		else
		{
			SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
			skinnedMeshRenderer = obj.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
			if ((bool)skinnedMeshRenderer)
			{
				for (int j = 0; j < skinnedMeshRenderer.sharedMesh.colors.Length; j++)
				{
					if (skinnedMeshRenderer.sharedMesh.colors[j].r == 1f)
					{
						list.Add(j);
					}
				}
			}
		}
		if (list.Count != 0)
		{
			rangeIndex = new int[list.Count];
			for (int k = 0; k < list.Count; k++)
			{
				rangeIndex[k] = list[k];
			}
		}
		else
		{
			rangeIndex = null;
		}
	}
}
