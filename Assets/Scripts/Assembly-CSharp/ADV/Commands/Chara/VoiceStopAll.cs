using Manager;

namespace ADV.Commands.Chara
{
	public class VoiceStopAll : CommandBase
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

		public override void Do()
		{
			base.Do();
			Singleton<Voice>.Instance.StopAll();
		}
	}
}
