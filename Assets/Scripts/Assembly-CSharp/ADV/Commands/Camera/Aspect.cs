namespace ADV.Commands.Camera
{
	public class Aspect : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isAspect" };
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
			base.scenario.isAspect = bool.Parse(args[0]);
		}
	}
}
