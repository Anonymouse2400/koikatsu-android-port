using Illusion;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavTest : MonoBehaviour
{
	private static float SPAWN_RANGE = 10f;

	public Transform target;

	[Button("SetDestination", "ターゲット更新", new object[] { })]
	public int targetButton;

	[Button("SetRandomPop", "ランダム配置", new object[] { })]
	public int randomButton;

	private NavMeshAgent agent;

	private Vector3? targetPos;

	public void SetDestination()
	{
		agent.SetDestination(target.position);
	}

	public void SetRandomPop()
	{
		Vector3 result;
		if (Utils.NavMesh.GetRandomPosition(base.transform.position, out result, SPAWN_RANGE))
		{
			agent.SetDestination(result);
		}
	}

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		for (int i = 0; i < 100; i++)
		{
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.transform.position = base.transform.position;
			gameObject.transform.Translate(base.transform.right * 5f);
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			gameObject.transform.Translate(new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y) * 4f);
		}
	}

	private void Update()
	{
		if (agent.remainingDistance < 1f)
		{
			targetPos = null;
		}
		if (Input.GetMouseButtonDown(0))
		{
			Quaternion rotationUniform = Random.rotationUniform;
			Quaternion rotation = Random.rotation;
		}
	}

	private void OnDrawGizmos()
	{
		if (!(agent == null))
		{
			Gizmos.color = Color.red;
			Utils.Gizmos.PointLine(agent.path.corners);
			float radius = 0.1f;
			Gizmos.color = Color.yellow;
			Vector3[] corners = agent.path.corners;
			foreach (Vector3 center in corners)
			{
				Gizmos.DrawSphere(center, radius);
			}
			radius = 0.15f;
			Gizmos.color = Color.green;
			Gizmos.DrawCube(agent.destination, new Vector3(radius, 10f, radius));
			if (targetPos.HasValue)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(agent.destination, new Vector3(radius, 10f, radius));
			}
		}
	}
}
