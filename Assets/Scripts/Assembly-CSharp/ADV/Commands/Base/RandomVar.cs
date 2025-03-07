using System;
using System.Collections.Generic;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class RandomVar : CommandBase
	{
		private Type type;

		private string name;

		private string min;

		private string max;

		private int refMinCnt;

		private int refMaxCnt;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Type", "Variable", "Min", "Max" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					"int",
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Convert(string fileName, ref string[] args)
		{
			new VAR().Convert(fileName, ref args);
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			int num = 0;
			type = Type.GetType(args[num++]);
			name = args[num++];
			if (type != typeof(bool))
			{
				min = args.SafeGet(num++);
				max = args.SafeGet(num++);
				refMinCnt = VAR.RefCheck(ref min);
				refMaxCnt = VAR.RefCheck(ref max);
			}
		}

		public override void Do()
		{
			base.Do();
			Dictionary<string, ValData> vars = base.scenario.Vars;
			VAR.RefGet(type, refMinCnt, min, vars).SafeProc(delegate(string s)
			{
				min = s;
			});
			VAR.RefGet(type, refMaxCnt, max, vars).SafeProc(delegate(string s)
			{
				max = s;
			});
			if (type == typeof(int))
			{
				vars[name] = new ValData(ValData.Convert(UnityEngine.Random.Range(int.Parse(min), int.Parse(max) + 1), type));
			}
			else if (type == typeof(float))
			{
				vars[name] = new ValData(ValData.Convert(UnityEngine.Random.Range(float.Parse(min), float.Parse(max)), type));
			}
			else if (type == typeof(string))
			{
				vars[name] = new ValData(ValData.Convert((UnityEngine.Random.Range(0, 2) != 1) ? max : min, type));
			}
			else if (type == typeof(bool))
			{
				vars[name] = new ValData(ValData.Convert(UnityEngine.Random.Range(0, 2) == 1, type));
			}
		}
	}
}
