namespace ADV.Commands.Chara
{
	public class LookNeckSkip : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "isSkip" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					int.MaxValue.ToString(),
					bool.FalseString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			CharaData chara = base.scenario.commandController.GetChara(int.Parse(args[num++]));
			chara.chaCtrl.neckLookCtrl.neckLookScript.skipCalc = bool.Parse(args[num++]);
		}
	}
}
