using System.Collections.Generic;

namespace ADV.Commands.Base
{
	public class FormatVAR : CommandBase
	{
		public string name;

		private List<object> parameters = new List<object>();

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Variable", "Format", "Args" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					string.Empty,
					"{0:00}",
					"1"
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			name = args[0];
			string[] argToSplitLast = GetArgToSplitLast(2);
			Dictionary<string, ValData> vars = base.scenario.Vars;
			int num = -1;
			while (++num < argToSplitLast.Length)
			{
				ValData value;
				if (vars.TryGetValue(argToSplitLast[num], out value))
				{
					parameters.Add(value.o);
				}
				else
				{
					parameters.Add(argToSplitLast[num]);
				}
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.Vars[name] = new ValData(string.Format(args[1], parameters.ToArray()));
		}
	}
}
