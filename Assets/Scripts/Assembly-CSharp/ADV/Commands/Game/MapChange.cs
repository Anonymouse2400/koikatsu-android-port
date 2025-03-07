using System;
using System.Collections.Generic;
using Illusion;
using Illusion.Extensions;
using Manager;
using UnityEngine;

namespace ADV.Commands.Game
{
	public class MapChange : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Fade" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2] { "0", "1" };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			int result;
			if (!int.TryParse(text, out result))
			{
				result = base.scenario.advScene.Map.ConvertMapNo(text);
			}
			string text2 = args[num++];
			int num2 = 1;
			if (!text2.IsNullOrEmpty())
			{
				string[] names = Utils.Enum<Scene.Data.FadeType>.Names;
				int result2 = text2.Check(true, names);
				if (result2 == -1)
				{
					bool result3;
					if (bool.TryParse(text2, out result3))
					{
						if (!result3)
						{
							num2 = 0;
						}
					}
					else if (int.TryParse(text2, out result2))
					{
						num2 = Mathf.Clamp(num2, 0, names.Length - 1);
					}
				}
				else
				{
					num2 = result2;
				}
			}
			base.scenario.advScene.Map.Change(result, Utils.Enum<Scene.Data.FadeType>.Cast(num2));
			if (!Singleton<Manager.Game>.IsInstance() || !(Singleton<Manager.Game>.Instance.actScene != null))
			{
				return;
			}
			foreach (KeyValuePair<string, Func<string>> aDVVariable in Singleton<Manager.Game>.Instance.actScene.Cycle.ADVVariables)
			{
				base.scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(string)));
			}
		}
	}
}
