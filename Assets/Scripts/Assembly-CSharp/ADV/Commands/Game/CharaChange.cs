namespace ADV.Commands.Game
{
	public class CharaChange : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "isParamUpdate" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					int.MaxValue.ToString(),
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			base.scenario.ChangeCurrentChara(no);
			if (bool.Parse(args[num++]))
			{
				base.scenario.currentChara.data.SetADVParam(base.scenario);
			}
		}
	}
}
