namespace ADV.Commands.Chara
{
	public class FixEyes : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Fix" };
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
			int no = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			args.SafeProc(num++, delegate(string s)
			{
				chara.chaCtrl.ChangeEyesBlinkFlag(!bool.Parse(s));
			});
		}
	}
}
