using System.Linq;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class Max : CommandBase
	{
		private string answer;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Answer", "A", "B" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3] { "Answer", "0", "0" };
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
			int cnt = 1;
			float num = Mathf.Max(GetArgToSplitLast(cnt).Select(float.Parse).ToArray());
			base.scenario.Vars[answer] = new ValData(num);
		}
	}
}
