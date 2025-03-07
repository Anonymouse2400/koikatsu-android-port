namespace ADV.Commands.Game
{
	public class HeroineCallNameChange : CommandBase
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
			base.scenario.currentHeroine.CallNameChange(base.scenario);
		}
	}
}
