using System;
using System.Collections.Generic;
using ActionGame;
using Manager;

namespace ADV.Commands.Game
{
	public class WeekChange : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Name" };
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
			ActionScene actionScene = null;
			if (Singleton<Manager.Game>.IsInstance() && Singleton<Manager.Game>.Instance.actScene != null)
			{
				actionScene = Singleton<Manager.Game>.Instance.actScene;
			}
			if (!(actionScene != null))
			{
				return;
			}
			actionScene.Cycle.Change(Cycle.ConvertWeek(args[0]));
			foreach (KeyValuePair<string, Func<string>> aDVVariable in actionScene.Cycle.ADVVariables)
			{
				base.scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(string)));
			}
		}
	}
}
