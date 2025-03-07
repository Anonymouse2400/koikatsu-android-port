using SkinnedMetaballBuilder;
using UnityEngine;

public class MetaballNode : MonoBehaviour
{
	public float baseRadius = 1f;

	public bool bSubtract;

	private MetaballSeedBase _seed;

	private Mesh _boneMesh;

	public virtual float Density
	{
		get
		{
			return (!bSubtract) ? 1f : (-1f);
		}
	}

	public float Radius
	{
		get
		{
			return baseRadius;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (_seed == null)
		{
			_seed = Utils.FindComponentInParents<MetaballSeedBase>(base.transform);
		}
		if (Density == 0f || (_seed != null && _seed.sourceRoot != null && _seed.sourceRoot.gameObject == base.gameObject))
		{
			return;
		}
		Gizmos.color = ((!bSubtract) ? Color.white : Color.red);
		float num = Radius;
		if (_seed != null)
		{
			num *= 1f - Mathf.Sqrt(_seed.powerThreshold);
		}
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(Vector3.zero, num);
		MetaballNode component = base.transform.parent.GetComponent<MetaballNode>();
		if (component != null && component.Density != 0f && _seed != null && _seed.IsTreeShape)
		{
			if (_boneMesh == null)
			{
				_boneMesh = new Mesh();
				Vector3[] array = new Vector3[5];
				Vector3[] array2 = new Vector3[5];
				int[] array3 = new int[6];
				array[0] = new Vector3(0.1f, 0f, 0f);
				array[1] = new Vector3(-0.1f, 0f, 0f);
				array[2] = new Vector3(0f, 0.1f, 0f);
				array[3] = new Vector3(0f, -0.1f, 0f);
				array[4] = new Vector3(0f, 0f, 1f);
				array2[0] = new Vector3(0f, 0f, 1f);
				array2[1] = new Vector3(0f, 0f, 1f);
				array2[2] = new Vector3(0f, 0f, 1f);
				array2[3] = new Vector3(0f, 0f, 1f);
				array2[4] = new Vector3(0f, 0f, 1f);
				array3[0] = 0;
				array3[1] = 1;
				array3[2] = 4;
				array3[3] = 2;
				array3[4] = 3;
				array3[5] = 4;
				_boneMesh.vertices = array;
				_boneMesh.normals = array2;
				_boneMesh.SetIndices(array3, MeshTopology.Triangles, 0);
			}
			Vector3 one = Vector3.one;
			Vector3 position = base.transform.position;
			Vector3 position2 = base.transform.parent.position;
			if (!((position2 - position).sqrMagnitude < float.Epsilon))
			{
				one *= (position2 - position).magnitude;
				Matrix4x4 matrix2 = Matrix4x4.TRS(position2, Quaternion.LookRotation(position - position2), one);
				Gizmos.color = Color.blue;
				Gizmos.matrix = matrix2;
				Gizmos.DrawWireMesh(_boneMesh);
			}
		}
		Gizmos.color = Color.white;
		Gizmos.matrix = matrix;
	}
}
