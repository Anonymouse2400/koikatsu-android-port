using Manager;

namespace ADV.Commands.Chara
{
	public class VoiceWaitAll : CommandBase
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

		public override bool Process()
		{
			base.Process();
			return !Singleton<Voice>.Instance.IsVoiceCheck();
		}
	}
}
