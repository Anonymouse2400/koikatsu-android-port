using System;
using System.Collections.Generic;
using ActionGame;
using Manager;

namespace ADV.Commands.Game
{
	public class CycleChange : CommandBase
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
			Cycle.Type type = Cycle.ConvertType(args[0]);
			if (actionScene != null)
			{
				actionScene.Cycle.Change(type);
				{
					foreach (KeyValuePair<string, Func<string>> aDVVariable in actionScene.Cycle.ADVVariables)
					{
						base.scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(string)));
					}
					return;
				}
			}
			base.scenario.advScene.Map.nowCycle = type;
			base.scenario.advScene.Map.SetCycleToSunLight();
		}
	}
}
