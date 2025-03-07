namespace ADV.Commands.CameraEffect
{
	public class CrossFade : CommandBase
	{
		private float time;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Time" };
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
			time = float.Parse(args[0]);
			base.scenario.crossFade.FadeStart(time);
		}

		public override bool Process()
		{
			base.Process();
			return base.scenario.crossFade.isEnd;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (!processEnd)
			{
				base.scenario.crossFade.End();
			}
		}
	}
}
