using System;
using System.Collections.Generic;

namespace ADV.Commands.Base
{
	public class ExpressionIcon : CommandBase
	{
		public class Data : TextScenario.IExpressionIcon, TextScenario.IChara
		{
			public int no { get; private set; }

			public string key { get; private set; }

			public Data(string[] args, ref int cnt)
			{
				try
				{
					no = int.Parse(args[cnt++]);
					key = args.SafeGet(cnt++);
				}
				catch (Exception)
				{
				}
			}

			public void Play(TextScenario scenario)
			{
				GetChara(scenario).faceIcon.Load(key);
			}

			public CharaData GetChara(TextScenario scenario)
			{
				return scenario.commandController.GetChara(no);
			}
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Key" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			base.scenario.currentCharaData.CreateExpressionIconList();
			List<Data> list = new List<Data>();
			if (args.Length > 1)
			{
				int cnt = 0;
				while (!args.IsNullOrEmpty(cnt))
				{
					list.Add(new Data(args, ref cnt));
				}
			}
			base.scenario.currentCharaData.expressionIconList.Add(list.ToArray());
		}
	}
}
