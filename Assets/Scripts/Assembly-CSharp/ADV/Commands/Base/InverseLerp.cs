using UnityEngine;

namespace ADV.Commands.Base
{
	public class InverseLerp : CommandBase
	{
		private string answer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Answer", "A", "B", "Value" };
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
			base.scenario.Vars[answer] = new ValData(Mathf.InverseLerp(float.Parse(args[num++]), float.Parse(args[num++]), float.Parse(args[num++])));
		}
	}
}
