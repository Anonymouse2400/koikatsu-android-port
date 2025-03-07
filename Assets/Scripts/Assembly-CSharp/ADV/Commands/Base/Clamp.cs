using UnityEngine;

namespace ADV.Commands.Base
{
	public class Clamp : CommandBase
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
				return new string[4] { "Answer", "0", "0", "0" };
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
			base.scenario.Vars[answer] = new ValData(Mathf.Clamp(float.Parse(args[num++]), float.Parse(args[num++]), float.Parse(args[num++])));
		}
	}
}
