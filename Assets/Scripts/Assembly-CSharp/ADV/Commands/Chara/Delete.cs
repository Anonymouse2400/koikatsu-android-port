namespace ADV.Commands.Chara
{
	public class Delete : CommandBase
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
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.commandController.RemoveChara(int.Parse(args[0]));
		}
	}
}
