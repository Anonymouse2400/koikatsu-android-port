namespace ADV.Commands.Chara
{
	public class CreateDummy : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "ReadNo" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "-100", "-2" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int no2 = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no2);
			base.scenario.commandController.AddChara(no, new CharaData(chara.data, base.scenario));
		}
	}
}
