namespace ADV.Commands.Chara
{
	public class CreateEmpty : CommandBase
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
				return new string[1] { "-100" };
			}
		}

		public override void Do()
		{
			base.Do();
			SaveData.Heroine heroine = new SaveData.Heroine(false);
			heroine.fixCharaID = -100;
			base.scenario.commandController.AddChara(int.Parse(args[0]), new CharaData(heroine, base.scenario));
		}
	}
}
