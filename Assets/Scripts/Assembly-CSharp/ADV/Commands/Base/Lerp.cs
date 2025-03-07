using System;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class Lerp : CommandBase
	{
		private string answer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Answer", "A", "B", "T", "isUnclamped" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					"Answer",
					"0",
					"0",
					"0",
					bool.FalseString
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
			Func<float, float, float, bool, float> func = (float a, float b, float t, bool isUnclamped) => (!isUnclamped) ? Mathf.LerpUnclamped(a, b, t) : Mathf.Lerp(a, b, t);
			base.scenario.Vars[answer] = new ValData(func(float.Parse(args[num++]), float.Parse(args[num++]), float.Parse(args[num++]), bool.Parse(args[num++])));
		}
	}
}
