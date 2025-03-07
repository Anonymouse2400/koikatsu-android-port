namespace ADV.Commands.H
{
	public class HNamaNG : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Result" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "0", "IsNamaNG" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			CharaData chara = base.scenario.commandController.GetChara(int.Parse(args[num++]));
			base.scenario.Vars[args[num++]] = new ValData(!HFlag.NamaInsertCheck(chara.heroine));
		}
	}
}
