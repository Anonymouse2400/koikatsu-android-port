namespace ADV.Commands.Camera
{
	public class GetFov : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Variable" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "Fov" };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.Vars[args[0]] = new ValData(base.scenario.AdvCamera.fieldOfView);
		}
	}
}
