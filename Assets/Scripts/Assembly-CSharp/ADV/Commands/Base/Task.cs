namespace ADV.Commands.Base
{
	public class Task : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isTask" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { bool.FalseString };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.isBackGroundCommanding = bool.Parse(args[0]);
		}
	}
}
