using UnityEngine;

namespace ADV.Commands.Chara
{
	public class StandPosition : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Stand" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					int.MaxValue.ToString(),
					"Center"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			Transform transform = base.scenario.commandController.GetChara(no).transform;
			string key = args[num++];
			Transform transform2 = base.scenario.commandController.characterStandNulls[key];
			transform.SetPositionAndRotation(transform2.position, transform2.rotation);
		}
	}
}
