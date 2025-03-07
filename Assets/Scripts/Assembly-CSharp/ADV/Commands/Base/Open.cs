namespace ADV.Commands.Base
{
	public class Open : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Bundle", "Asset", "isAdd", "isClearCheck", "isNext" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					string.Empty,
					string.Empty,
					bool.FalseString,
					bool.TrueString,
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			string text2 = args[num++];
			bool flag = bool.Parse(args[num++]);
			bool isClearCheck = bool.Parse(args[num++]);
			bool isNext = bool.Parse(args[num++]);
			if (text.IsNullOrEmpty())
			{
				text = base.scenario.LoadBundleName;
			}
			if (!AssetBundleCheck.IsFile(text, string.Empty))
			{
				text = Program.ScenarioBundle(text);
			}
			base.scenario.Vars["BundleFile"] = new ValData(text);
			base.scenario.Vars["AssetFile"] = new ValData(text2);
			if (!flag)
			{
				(base.scenario as MainScenario).SafeProc(delegate(MainScenario main)
				{
					main.BackLog.Clear();
				});
			}
			base.scenario.LoadFile(text, text2, !flag, isClearCheck, isNext);
		}
	}
}
