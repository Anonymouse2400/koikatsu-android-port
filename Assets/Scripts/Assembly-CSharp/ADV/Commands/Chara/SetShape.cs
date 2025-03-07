namespace ADV.Commands.Chara
{
	public class SetShape : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Index", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3] { "0", "0", "0.5" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int index = int.Parse(args[num++]);
			float value = float.Parse(args[num++]);
			base.scenario.commandController.GetChara(no).chaCtrl.SetShapeBodyValue(index, value);
		}
	}
}
