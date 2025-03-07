using ADV.EventCG;

namespace ADV.Commands.EventCG
{
	public class Next : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "No" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			int index = int.Parse(args[0]);
			base.scenario.CrossFadeStart();
			base.scenario.commandController.EventCGRoot.GetChild(0).GetComponent<ADV.EventCG.Data>().Next(index, base.scenario.commandController.Characters);
		}
	}
}
