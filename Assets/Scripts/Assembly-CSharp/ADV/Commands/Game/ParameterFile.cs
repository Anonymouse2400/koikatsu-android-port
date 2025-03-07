using System.Collections.Generic;
using Manager;

namespace ADV.Commands.Game
{
	public class ParameterFile : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Asset" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { string.Empty };
			}
		}

		public override void Do()
		{
			base.Do();
			List<ScenarioData.Param> value;
			if (Singleton<Manager.Game>.Instance.scenarioParameterDic.TryGetValue(args[0], out value))
			{
				base.scenario.CommandPacks.InsertRange(base.scenario.CurrentLine + 1, value);
			}
		}
	}
}
