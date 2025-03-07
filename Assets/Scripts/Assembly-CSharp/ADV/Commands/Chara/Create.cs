namespace ADV.Commands.Chara
{
	public class Create : CommandBase
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
				return new string[2]
				{
					int.MaxValue.ToString(),
					"-2"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int num2 = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(num2);
			base.scenario.commandController.AddChara(no, chara.data, null);
			if (num2 != -1)
			{
				base.scenario.ChangeCurrentChara(no);
			}
		}
	}
}
