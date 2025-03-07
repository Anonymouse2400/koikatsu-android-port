using System.Collections.Generic;
using UnityEngine;

public class AverageNormals : MonoBehaviour
{
	public GameObject[] objUpdate = new GameObject[2];

	public GameObject ObjRange;

	public float Range = 1f;

	private Mesh[] meshUpdate = new Mesh[2];

	public int[] calcIndexA;

	public int[] calcIndexB;

	private bool meshInit = true;

	public void Init()
	{
		if (null == objUpdate[0] || null == objUpdate[1] || null == ObjRange)
		{
			return;
		}
		Mesh[] array = new Mesh[2];
		List<int>[] array2 = new List<int>[2];
		for (int i = 0; i < 2; i++)
		{
			array2[i] = new List<int>();
		}
		List<int>[] array3 = new List<int>[2];
		for (int j = 0; j < 2; j++)
		{
			array3[j] = new List<int>();
			MeshFilter meshFilter = new MeshFilter();
			meshFilter = objUpdate[j].GetComponent(typeof(MeshFilter)) as MeshFilter;
			if ((bool)meshFilter)
			{
				array[j] = meshFilter.sharedMesh;
			}
			else
			{
				SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
				skinnedMeshRenderer = objUpdate[j].GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				array[j] = skinnedMeshRenderer.sharedMesh;
			}
			if (!(null != array[j]))
			{
				continue;
			}
			if ((bool)ObjRange)
			{
				for (int k = 0; k < array[j].vertexCount; k++)
				{
					Vector3 b = objUpdate[j].transform.TransformPoint(array[j].vertices[k]);
					float num = Vector3.Distance(ObjRange.transform.position, b);
					if (num < Range)
					{
						array3[j].Add(k);
					}
				}
			}
			else
			{
				for (int l = 0; l < array[j].vertexCount; l++)
				{
					array3[j].Add(l);
				}
			}
		}
		if (!(null != array[0]) || !(null != array[1]))
		{
			return;
		}
		for (int m = 0; m < array3[0].Count; m++)
		{
			for (int n = 0; n < array3[1].Count; n++)
			{
				int num2 = array3[0][m];
				int num3 = array3[1][n];
				Vector3 vector = objUpdate[0].transform.TransformPoint(array[0].vertices[num2]);
				Vector3 vector2 = objUpdate[1].transform.TransformPoint(array[1].vertices[num3]);
				if (vector == vector2)
				{
					array2[0].Add(num2);
					array2[1].Add(num3);
					break;
				}
			}
		}
		calcIndexA = new int[array2[0].Count];
		calcIndexB = new int[array2[1].Count];
		for (int num4 = 0; num4 < array2[0].Count; num4++)
		{
			calcIndexA[num4] = array2[0][num4];
			calcIndexB[num4] = array2[1][num4];
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (meshInit)
		{
			GetUpdateMesh();
			meshInit = false;
		}
		Average();
	}

	public void GetUpdateMesh()
	{
		for (int i = 0; i < 2; i++)
		{
			if (!(null == objUpdate[i]))
			{
				MeshFilter meshFilter = new MeshFilter();
				meshFilter = objUpdate[i].GetComponent(typeof(MeshFilter)) as MeshFilter;
				if ((bool)meshFilter)
				{
					meshUpdate[i] = meshFilter.sharedMesh;
					continue;
				}
				SkinnedMeshRenderer skinnedMeshRenderer = new SkinnedMeshRenderer();
				skinnedMeshRenderer = objUpdate[i].GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
				meshUpdate[i] = skinnedMeshRenderer.sharedMesh;
			}
		}
	}

	public void Average()
	{
		if (calcIndexA.Length != 0 && calcIndexB.Length != 0 && !(null == meshUpdate[0]) && !(null == meshUpdate[1]))
		{
			Vector3[] array = new Vector3[calcIndexA.Length];
			for (int i = 0; i < calcIndexA.Length; i++)
			{
				int num = calcIndexA[i];
				int num2 = calcIndexB[i];
				array[i] = Vector3.Lerp(meshUpdate[0].normals[num], meshUpdate[1].normals[num2], 0.5f);
			}
			Vector3[] array2 = new Vector3[meshUpdate[0].vertexCount];
			array2 = meshUpdate[0].normals;
			for (int j = 0; j < calcIndexA.Length; j++)
			{
				array2[calcIndexA[j]] = array[j];
			}
			meshUpdate[0].normals = array2;
			Vector3[] array3 = new Vector3[meshUpdate[1].vertexCount];
			array3 = meshUpdate[1].normals;
			for (int k = 0; k < calcIndexB.Length; k++)
			{
				array3[calcIndexB[k]] = array[k];
			}
			meshUpdate[1].normals = array3;
		}
	}

	private void OnDrawGizmos()
	{
		if (!(null == ObjRange))
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(ObjRange.transform.position, Range);
		}
	}
}
