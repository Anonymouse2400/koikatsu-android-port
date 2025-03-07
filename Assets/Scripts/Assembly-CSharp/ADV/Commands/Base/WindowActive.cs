namespace ADV.Commands.Base
{
	public class WindowActive : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "isActive", "Level" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					bool.TrueString,
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			bool flag = bool.Parse(args[num++]);
			if (int.Parse(args[num++]) == 0)
			{
				base.scenario.isWindowImage = flag;
			}
			else
			{
				base.scenario.MessageWindow.gameObject.SetActive(flag);
			}
		}
	}
}
