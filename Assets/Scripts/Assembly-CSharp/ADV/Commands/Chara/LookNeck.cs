namespace ADV.Commands.Chara
{
	public class LookNeck : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Ptn", "Rate" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
					"0",
					"1"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int ptn = int.Parse(args[num++]);
			float rate = float.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			chara.chaCtrl.ChangeLookNeckPtn(ptn, rate);
		}
	}
}
