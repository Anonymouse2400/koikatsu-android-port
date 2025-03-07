namespace ADV.Commands.Effect
{
	public class SceneFadeRegulate : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Flag" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { bool.TrueString };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.isSceneFadeRegulate = bool.Parse(args[0]);
		}
	}
}
