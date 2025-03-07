using Manager;

namespace ADV.Commands.CameraEffect
{
	public class SepiaEffect : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isActive" };
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
			Singleton<Manager.Game>.Instance.cameraEffector.useSepia = bool.Parse(args[0]);
		}
	}
}
