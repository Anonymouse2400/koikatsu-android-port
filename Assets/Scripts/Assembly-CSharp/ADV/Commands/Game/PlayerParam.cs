using Manager;

namespace ADV.Commands.Game
{
	public class PlayerParam : CommandBase
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
			Singleton<Manager.Game>.Instance.Player.SetADVParam(base.scenario, variable, value);
		}
	}
}
