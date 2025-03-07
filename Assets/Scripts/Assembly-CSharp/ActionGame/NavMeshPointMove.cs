using Illusion;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame
{
	public class NavMeshPointMove : MonoBehaviour
	{
		[SerializeField]
		private NavMeshAgent agent;

		[SerializeField]
		[Label("追従オブジェクト")]
		[Tooltip("無ければマウスクリックによる移動")]
		private Transform traceTarget;

		[SerializeField]
		[Label("目的地描画範囲")]
		private float destinationRange = 0.15f;

		[Label("ルート描画範囲")]
		[SerializeField]
		private float rootRange = 0.1f;

		[Label("ランダム移動モード")]
		[SerializeField]
		private bool isRandomMove;

		[SerializeField]
		[Label("停止フラグ")]
		[Header("NavMesh停止")]
		private bool isStopped;

		[SerializeField]
		[Label("停止キー")]
		private KeyCode stopKey = KeyCode.S;

		private void Start()
		{
			agent = this.GetOrAddComponent<NavMeshAgent>();
		}

		private void Update()
		{
			if (!agent.isOnNavMesh)
			{
				return;
			}
			if (Input.GetKeyDown(stopKey))
			{
				isStopped = !isStopped;
			}
			agent.isStopped = isStopped;
			RaycastHit hitInfo;
			if (isRandomMove)
			{
				if (agent.remainingDistance < 1f)
				{
					Vector3 vector = new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
					agent.SetDestination(base.transform.position + vector);
				}
			}
			else if (traceTarget != null)
			{
				agent.SetDestination(traceTarget.position);
			}
			else if (Input.GetMouseButtonDown(0) && Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo))
			{
				agent.SetDestination(hitInfo.point);
			}
		}

		private void OnDrawGizmos()
		{
			if (agent != null)
			{
				Gizmos.color = Color.red;
				Utils.Gizmos.PointLine(agent.path.corners);
				Gizmos.color = Color.yellow;
				rootRange = Mathf.Max(rootRange, 0.01f);
				Vector3[] corners = agent.path.corners;
				foreach (Vector3 center in corners)
				{
					Gizmos.DrawSphere(center, rootRange);
				}
				Gizmos.color = Color.green;
				destinationRange = Mathf.Max(destinationRange, 0.01f);
				Gizmos.DrawCube(agent.destination, new Vector3(destinationRange, 10f, destinationRange));
			}
		}
	}
}
