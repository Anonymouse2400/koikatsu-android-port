using Manager;

namespace ADV.Commands.Game
{
	public class MapUnload : CommandBase
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
			Singleton<Scene>.Instance.UnloadBaseScene();
		}
	}
}
