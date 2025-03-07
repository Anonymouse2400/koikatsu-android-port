namespace ADV.Commands.Base
{
	public class Log : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Type", "Msg" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"Log",
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
		}
	}
}
