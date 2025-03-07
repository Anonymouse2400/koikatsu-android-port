namespace ADV.Commands.Camera
{
	public class SetFov : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.AdvCamera.fieldOfView = float.Parse(args[0]);
		}
	}
}
