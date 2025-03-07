namespace ADV.Commands.Chara
{
	public class GetShape : CommandBase
	{
		private int no;

		private string name;

		private int index;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Name", "Index" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"0",
					string.Empty,
					"0"
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			name = args[1];
		}

		public override void Do()
		{
			base.Do();
			no = int.Parse(args[0]);
			index = int.Parse(args[2]);
			base.scenario.Vars[name] = new ValData(base.scenario.commandController.GetChara(no).chaCtrl.GetShapeBodyValue(index));
		}
	}
}
