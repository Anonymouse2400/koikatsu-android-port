namespace ADV.Commands.Base
{
	public class NullRelease : CommandBase
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
			base.scenario.commandController.ReleaseNull();
		}
	}
}
