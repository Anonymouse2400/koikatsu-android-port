using System.Text;
using Illusion;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame
{
	public class NavMeshDebug : MonoBehaviour
	{
		private static class NavDebug
		{
			public static string AgentToString(NavMeshAgent agent)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(ToS("isOnNavMesh", agent.isOnNavMesh));
				if (agent.isOnNavMesh)
				{
					stringBuilder.AppendLine(ToS("isOnOffMeshLink", agent.isOnOffMeshLink));
					stringBuilder.AppendLine(ToS("isPathStale", agent.isPathStale));
					stringBuilder.AppendLine(ToS("hasPath", agent.hasPath));
					stringBuilder.AppendLine(ToS("pathPending", agent.pathPending));
					stringBuilder.AppendLine(ToS("pathStatus", agent.pathStatus));
					stringBuilder.AppendLine(ToS("path", agent.path));
					stringBuilder.AppendLine(ToS("pathEndPosition", agent.pathEndPosition));
					stringBuilder.AppendLine(ToS("steeringTarget", agent.steeringTarget));
					stringBuilder.AppendLine(ToS("destination", agent.destination));
					stringBuilder.AppendLine(ToS("remainingDistance", agent.remainingDistance));
					stringBuilder.AppendLine(string.Empty);
					NavMeshHit hit;
					stringBuilder.AppendLine(ToS("Raycast", agent.Raycast(agent.destination, out hit)));
					string empty = string.Empty;
					empty = ((!(agent.currentOffMeshLinkData.offMeshLink != null)) ? ToS("currentOffMeshLinkData", "Null") : ToS("currentOffMeshLinkData", agent.currentOffMeshLinkData.offMeshLink.name));
					stringBuilder.AppendLine(empty);
					empty = ((!(agent.nextOffMeshLinkData.offMeshLink != null)) ? ToS("nextOffMeshLinkData", "Null") : ToS("nextOffMeshLinkData", agent.nextOffMeshLinkData.offMeshLink.name));
					stringBuilder.AppendLine(empty);
				}
				stringBuilder.AppendLine(string.Empty);
				stringBuilder.AppendLine(ToS("nextPosition", agent.nextPosition));
				stringBuilder.AppendLine(ToS("velocity", agent.velocity));
				stringBuilder.AppendLine(ToS("desiredVelocity", agent.desiredVelocity));
				stringBuilder.AppendLine(string.Empty);
				stringBuilder.AppendLine(ToS("obstacleAvoidanceType", agent.obstacleAvoidanceType));
				stringBuilder.AppendLine(ToS("height", agent.height));
				stringBuilder.AppendLine(ToS("radius", agent.radius));
				stringBuilder.AppendLine(ToS("speed", agent.speed));
				stringBuilder.AppendLine(ToS("stoppingDistance", agent.stoppingDistance));
				stringBuilder.AppendLine(ToS("updatePosition", agent.updatePosition));
				stringBuilder.AppendLine(ToS("updateRotation", agent.updateRotation));
				stringBuilder.AppendLine(ToS("acceleration", agent.acceleration));
				stringBuilder.AppendLine(ToS("angularSpeed", agent.angularSpeed));
				stringBuilder.AppendLine(ToS("areaMask", agent.areaMask));
				stringBuilder.AppendLine(ToS("autoBraking", agent.autoBraking));
				stringBuilder.AppendLine(ToS("autoRepath", agent.autoRepath));
				stringBuilder.AppendLine(ToS("autoTraverseOffMeshLink", agent.autoTraverseOffMeshLink));
				stringBuilder.AppendLine(ToS("avoidancePriority", agent.avoidancePriority));
				stringBuilder.AppendLine(ToS("baseOffset", agent.baseOffset));
				return stringBuilder.ToString();
			}

			private static string ToS(string name, object o)
			{
				return name + ":" + o;
			}
		}

		public NavMeshAgent agent;

		private void Awake()
		{
			if (agent == null)
			{
				agent = GetComponentInChildren<NavMeshAgent>();
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
			}
		}
	}
}
