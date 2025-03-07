namespace ADV.Commands.Game
{
	public class MapVisible : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Visible" };
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
			base.scenario.advScene.Map.mapVisibleList.Set(bool.Parse(args[0]));
		}
	}
}
