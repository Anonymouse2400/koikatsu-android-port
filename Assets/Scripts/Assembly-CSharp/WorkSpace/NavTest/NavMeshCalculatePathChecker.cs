using Illusion;
using UnityEngine;
using UnityEngine.AI;

namespace WorkSpace.NavTest
{
	internal class NavMeshCalculatePathChecker : MonoBehaviour
	{
		[SerializeField]
		private Transform targetTrans;

		[SerializeField]
		private Vector3 target;

		private Vector3[] corners;

		private void OnDrawGizmos()
		{
			if (corners != null)
			{
				Gizmos.color = Color.red;
				Utils.Gizmos.PointLine(corners);
				Vector3[] array = corners;
				foreach (Vector3 from in array)
				{
					Gizmos.DrawRay(from, Vector3.up * 2f);
				}
			}
		}

		private void Start()
		{
		}

		private void Update()
		{
			if (targetTrans != null)
			{
				target = targetTrans.position;
			}
			NavMeshPath navMeshPath = new NavMeshPath();
			if (NavMesh.CalculatePath(base.transform.position, target, -1, navMeshPath))
			{
				corners = navMeshPath.corners;
			}
		}
	}
}
