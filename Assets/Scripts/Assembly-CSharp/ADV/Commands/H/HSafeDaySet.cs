namespace ADV.Commands.H
{
	public class HSafeDaySet : CommandBase
	{
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
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			CharaData chara = base.scenario.commandController.GetChara(int.Parse(args[num++]));
			HFlag.SetMenstruation(chara.heroine, HFlag.MenstruationType.安全日);
		}
	}
}
