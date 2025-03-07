using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Illusion;

namespace ADV.Commands.Base
{
	public class Calc : CommandBase
	{
		public enum Formula1
		{
			Equal = 0,
			PlusEqual = 1,
			MinusEqual = 2,
			AstaEqual = 3,
			SlashEqual = 4
		}

		public enum Formula2
		{
			Plus = 0,
			Minus = 1,
			Asta = 2,
			Slash = 3
		}

		private string answer;

		private string arg1;

		private List<string> argsList = new List<string>();

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Answer", "Formula", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					string.Empty,
					string.Empty,
					"0"
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			int num = 0;
			answer = args[num++];
			arg1 = args[num++];
			while (num < args.Length)
			{
				argsList.Add(args[num++]);
			}
		}

		public override void Do()
		{
			base.Do();
			Dictionary<string, ValData> Vars = base.scenario.Vars;
			ValData answerVal;
			if (!Vars.TryGetValue(answer, out answerVal))
			{
				string text = argsList[0];
				ValData value;
				if (Vars.TryGetValue(text, out value))
				{
					text = value.o.ToString();
				}
				int result;
				float result2;
				bool result3;
				if (int.TryParse(text, out result))
				{
					answerVal = new ValData(0);
				}
				else if (float.TryParse(text, out result2))
				{
					answerVal = new ValData(0f);
				}
				else if (bool.TryParse(text, out result3))
				{
					answerVal = new ValData(false);
				}
				else
				{
					answerVal = new ValData(string.Empty);
				}
			}
			ValData value2;
			Func<string, ValData> func = (string s) => new ValData(ValData.Cast((!Vars.TryGetValue(s, out value2)) ? s : value2.o, answerVal.o.GetType()));
			int num = 0;
			ValData valData = func(argsList[num++]);
			while (num < argsList.Count)
			{
				valData = Calculate(valData, Formula2to1((Formula2)int.Parse(argsList[num++])), func(argsList[num++]));
			}
			answerVal = Calculate(answerVal, (Formula1)int.Parse(arg1), valData);
			Vars[answer] = answerVal;
		}

		private static Formula1 Formula2to1(Formula2 f2)
		{
			switch (f2)
			{
			case Formula2.Plus:
				return Formula1.PlusEqual;
			case Formula2.Minus:
				return Formula1.MinusEqual;
			case Formula2.Asta:
				return Formula1.AstaEqual;
			case Formula2.Slash:
				return Formula1.SlashEqual;
			default:
				return Formula1.Equal;
			}
		}

		private static ValData Calculate(ValData a, Formula1 f1, ValData b)
		{
			ValData result = a;
			switch (f1)
			{
			case Formula1.Equal:
				result = b;
				break;
			case Formula1.PlusEqual:
				result += b;
				break;
			case Formula1.MinusEqual:
				result -= b;
				break;
			case Formula1.AstaEqual:
				result *= b;
				break;
			case Formula1.SlashEqual:
				result /= b;
				break;
			}
			return result;
		}

		[Conditional("ADV_DEBUG")]
		private void DBTEST(ValData answerVal)
		{
			Dictionary<string, ValData> Vars = base.scenario.Vars;
			ValData dbOutputVal = answerVal;
			List<string> list = new List<string>();
			ValData value;
			Func<string, ValData> func = (string s) => (!Vars.TryGetValue(s, out value)) ? new ValData(dbOutputVal.Convert(s)) : new ValData(value.o);
			int num = 0;
			ValData valData = func(argsList[num++]);
			list.Add(valData.o.ToString());
			while (num < argsList.Count)
			{
				Formula2 formula = (Formula2)int.Parse(argsList[num++]);
				ValData valData2 = func(argsList[num++]);
				list.Add(Cast(formula));
				list.Add(valData2.o.ToString());
				valData = Calculate(valData, Formula2to1(formula), valData2);
			}
			dbOutputVal = Calculate(dbOutputVal, (Formula1)int.Parse(arg1), valData);
			Formula1 formula2 = (Formula1)int.Parse(arg1);
			int num2 = 0;
			if (formula2 != 0)
			{
				list.Insert(num2++, answerVal.o.ToString());
			}
			list.Insert(num2++, Cast(formula2));
		}

		public static string Cast(Formula1 formula)
		{
			switch (formula)
			{
			case Formula1.Equal:
				return "=";
			case Formula1.PlusEqual:
				return "+=";
			case Formula1.MinusEqual:
				return "-=";
			case Formula1.AstaEqual:
				return "*=";
			case Formula1.SlashEqual:
				return "/=";
			default:
				return string.Empty;
			}
		}

		public static string Cast(Formula2 formula)
		{
			switch (formula)
			{
			case Formula2.Plus:
				return "+";
			case Formula2.Minus:
				return "-";
			case Formula2.Asta:
				return "*";
			case Formula2.Slash:
				return "/";
			default:
				return string.Empty;
			}
		}

		public override void Convert(string fileName, ref string[] args)
		{
			int cnt = 0;
			StringBuilder stringBuilder = new StringBuilder();
			if (args.IsNullOrEmpty(cnt++))
			{
				stringBuilder.AppendLine("Answer none");
			}
			if (!Formula1Cast(ref args[cnt]))
			{
				stringBuilder.AppendLine("Formula1 Cast Error");
			}
			Action action = delegate
			{
				cnt += 2;
			};
			action();
			while (!args.IsNullOrEmpty(cnt))
			{
				if (!Formula2Cast(ref args[cnt]))
				{
					stringBuilder.AppendLine("Formula2 Cast Error");
				}
				action();
				if (args.IsNullOrEmpty(cnt - 1))
				{
					stringBuilder.AppendLine("Formula2 Value none");
				}
			}
			if (stringBuilder.Length <= 0)
			{
			}
		}

		private static bool Formula1Cast(ref string arg)
		{
			string temp = arg;
			int num = Utils.Value.Check(Utils.Enum<Formula1>.Length, (int index) => temp == Cast((Formula1)index));
			bool result = true;
			if (num == -1)
			{
				num = 0;
				result = false;
			}
			arg = num.ToString();
			return result;
		}

		private static bool Formula2Cast(ref string arg)
		{
			string temp = arg;
			int num = Utils.Value.Check(Utils.Enum<Formula2>.Length, (int index) => temp == Cast((Formula2)index));
			bool result = true;
			if (num == -1)
			{
				num = 0;
				result = false;
			}
			arg = num.ToString();
			return result;
		}
	}
}
