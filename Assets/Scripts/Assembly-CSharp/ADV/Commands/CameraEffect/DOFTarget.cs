namespace ADV.Commands.CameraEffect
{
	public class DOFTarget : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "No" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			base.scenario.advScene.cameraEffector.SetDOFTarget(base.scenario.commandController.GetChara(int.Parse(args[num++])).transform);
		}
	}
}
