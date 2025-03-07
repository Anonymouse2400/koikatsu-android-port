using Manager;

namespace ADV.Commands.Chara
{
	public class VoiceWait : CommandBase
	{
		private CharaData chara;

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
			chara = base.scenario.commandController.GetChara(int.Parse(args[0]));
		}

		public override bool Process()
		{
			base.Process();
			return !Singleton<Voice>.Instance.IsVoiceCheck(chara.voiceNo, chara.voiceTrans);
		}
	}
}
