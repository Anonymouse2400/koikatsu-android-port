namespace ADV.Commands.Base
{
	public class TaskWait : CommandBase
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
		}

		public override bool Process()
		{
			base.Process();
			return !base.scenario.isBackGroundCommandProcessing;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (processEnd)
			{
			}
		}
	}
}
