namespace ADV.Commands.Base
{
	public class InfoAudio : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "is2D", "isMoveMouth" };
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
			int num = 0;
			Info.Audio audio = base.scenario.info.audio;
			args.SafeProc(num++, delegate(string s)
			{
				audio.is2D = bool.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audio.isNotMoveMouth = bool.Parse(s);
			});
		}
	}
}
