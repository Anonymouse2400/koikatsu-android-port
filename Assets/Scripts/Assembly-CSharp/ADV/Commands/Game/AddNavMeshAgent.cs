using UnityEngine.AI;

namespace ADV.Commands.Game
{
	public class AddNavMeshAgent : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return null;
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.currentChara.transform.gameObject.AddComponent<NavMeshAgent>();
		}
	}
}
