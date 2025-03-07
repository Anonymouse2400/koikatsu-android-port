using System.Collections.Generic;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class LerpAngleV3 : CommandBase
	{
		private string answer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Answer", "Value", "Min", "Max" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					"Answer",
					"0",
					string.Empty,
					string.Empty
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			answer = args[0];
		}

		public override void Do()
		{
			base.Do();
			int num = 1;
			float shape = float.Parse(args[num++]);
			Dictionary<string, Vector3> v3Dic = base.scenario.commandController.V3Dic;
			Vector3 min = v3Dic[args[num++]];
			Vector3 max = v3Dic[args[num++]];
			v3Dic[answer] = MotionIK.GetShapeLerpAngleValue(shape, min, max);
		}
	}
}
