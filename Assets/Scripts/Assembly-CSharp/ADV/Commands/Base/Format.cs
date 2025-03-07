using System.Collections.Generic;

namespace ADV.Commands.Base
{
	public class Format : CommandBase
	{
		public string name;

		public string format;

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
			int num = 0;
			name = args[num++];
			format = args[num++];
			string[] argToSplitLast = GetArgToSplitLast(num++);
			Dictionary<string, ValData> vars = base.scenario.Vars;
			int num2 = -1;
			while (++num2 < argToSplitLast.Length)
			{
				ValData value;
				if (vars.TryGetValue(argToSplitLast[num2], out value))
				{
					parameters.Add(value.o);
				}
				else
				{
					parameters.Add(argToSplitLast[num2]);
				}
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.Vars[name] = new ValData(string.Format(format, parameters.ToArray()));
		}
	}
}
