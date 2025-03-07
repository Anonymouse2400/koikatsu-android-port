namespace ADV.Commands.Base
{
	public class InfoAudioEco : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Use", "Delay", "DecayRatio", "WetMix", "DryMix" };
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
			Info.Audio.Eco eco = base.scenario.info.audio.eco;
			args.SafeProc(num++, delegate(string s)
			{
				eco.use = bool.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				eco.delay = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				eco.decayRatio = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				eco.wetMix = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				eco.dryMix = float.Parse(s);
			});
		}
	}
}
