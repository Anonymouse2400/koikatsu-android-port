using UnityEngine.AI;

namespace ActionGame.Chara.Backup
{
	public class NavMeshAgent
	{
		private UnityEngine.AI.NavMeshAgent agent;

		public float speed { get; private set; }

		public int priority { get; private set; }

		public bool updatePosition { get; private set; }

		public bool updateRotation { get; private set; }

		public NavMeshAgent(UnityEngine.AI.NavMeshAgent agent)
		{
			if (!(agent == null))
			{
				this.agent = agent;
				speed = agent.speed;
				priority = agent.avoidancePriority;
				updatePosition = agent.updatePosition;
				updateRotation = agent.updateRotation;
			}
		}

		public void Restore(UnityEngine.AI.NavMeshAgent agent = null)
		{
			agent = agent ?? this.agent;
			if (!(agent == null))
			{
				agent.speed = speed;
				agent.avoidancePriority = priority;
				agent.updatePosition = updatePosition;
				agent.updateRotation = updateRotation;
			}
		}
	}
}
