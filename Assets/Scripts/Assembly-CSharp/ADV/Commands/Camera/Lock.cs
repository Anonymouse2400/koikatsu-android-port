namespace ADV.Commands.Camera
{
	public class Lock : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isLock" };
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
			base.scenario.isCameraLock = bool.Parse(args[0]);
		}
	}
}
