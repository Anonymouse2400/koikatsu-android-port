namespace ADV.Commands.Game
{
	public class CharaParam : CommandBase
	{
		private string variable;

		private string value;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Variable", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
					null,
					null
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			variable = args[1];
		}

		public override void Do()
		{
			base.Do();
			int no = int.Parse(args[0]);
			value = args[2];
			base.scenario.commandController.GetChara(no).data.SetADVParam(base.scenario, variable, value);
		}
	}
}
