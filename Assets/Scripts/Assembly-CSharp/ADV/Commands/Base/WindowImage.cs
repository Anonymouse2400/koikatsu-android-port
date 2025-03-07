namespace ADV.Commands.Base
{
	public class WindowImage : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Visible" };
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
			base.scenario.isWindowImage = bool.Parse(args[0]);
		}
	}
}
