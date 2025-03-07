using UnityEngine;

namespace ADV.Commands.H
{
	public class LookAtDankonRemove : CommandBase
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
			UnityEngine.Object.Destroy(base.scenario.GetComponent<Lookat_dan>());
		}
	}
}
