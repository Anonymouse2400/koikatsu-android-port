namespace ADV.Commands.Game
{
	public class HeroineParam : CommandBase
	{
		private string variable;

		private string value;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Variable", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			variable = args[0];
		}

		public override void Do()
		{
			base.Do();
			value = args[1];
			base.scenario.currentHeroine.SetADVParam(base.scenario, variable, value);
		}
	}
}
