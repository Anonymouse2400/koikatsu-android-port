using UnityEngine;
using UnityEngine.AI;

public class bl_RandomBot : MonoBehaviour
{
	[SerializeField]
	private float Radius = 50f;

	private NavMeshAgent m_Agent;

	private NavMeshAgent Agent
	{
		get
		{
			if (m_Agent == null)
			{
				m_Agent = GetComponent<NavMeshAgent>();
			}
			return m_Agent;
		}
	}

	private void FixedUpdate()
	{
		if (!Agent.hasPath)
		{
			RandomBot();
		}
	}

	private void RandomBot()
	{
		Vector3 sourcePosition = Random.insideUnitSphere * Radius;
		sourcePosition += base.transform.position;
		NavMeshHit hit;
		NavMesh.SamplePosition(sourcePosition, out hit, 75f, 1);
		Vector3 position = hit.position;
		Agent.SetDestination(position);
	}
}
