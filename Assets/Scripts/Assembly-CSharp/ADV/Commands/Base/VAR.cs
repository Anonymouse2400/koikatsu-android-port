using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class VAR : CommandBase
	{
		private Type type;

		private string name;

		private string value;

		private int refCnt;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Type", "Variable", "Value" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"int",
					string.Empty,
					string.Empty
				};
			}
		}

		public static int RefCheck(ref string variable)
		{
			int num = 0;
			while (!variable.IsNullOrEmpty() && variable[0] == '*')
			{
				num++;
				variable = variable.Remove(0, 1);
			}
			return num;
		}

		public static string RefGet(Type type, int refCount, string variable, Dictionary<string, ValData> Vars)
		{
			ValData valData = null;
			if (refCount-- > 0)
			{
				valData = new ValData(Vars[variable].o);
			}
			while (refCount-- > 0)
			{
				valData = Vars[valData.o.ToString()];
			}
			return (valData != null) ? ValData.Cast(valData.o, type).ToString() : null;
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			int num = 0;
			type = Type.GetType(args[num++]);
			name = args[num++];
			value = args.Skip(num++).Shuffle().FirstOrDefault();
			refCnt = RefCheck(ref value);
		}

		public override void Do()
		{
			base.Do();
			Dictionary<string, ValData> vars = base.scenario.Vars;
			RefGet(type, refCnt, value, vars).SafeProc(delegate(string s)
			{
				value = s;
			});
			vars[name] = new ValData(ValData.Cast(value, type));
		}

		public override void Convert(string fileName, ref string[] args)
		{
			string text = args[0];
			Type type = null;
			switch (text)
			{
			case "int":
				type = typeof(int);
				break;
			case "float":
				type = typeof(float);
				break;
			case "string":
				type = typeof(string);
				break;
			case "bool":
				type = typeof(bool);
				break;
			default:
			{
				object obj = CheckLiterals(args[1]);
				args = new string[3]
				{
					obj.GetType().ToString(),
					args[0],
					obj.ToString()
				};
				return;
			}
			}
			args[0] = type.ToString();
		}

		public static object CheckLiterals(object o)
		{
			string text = o.ToString();
			if (text.Check(true, bool.TrueString, bool.FalseString) != -1)
			{
				return bool.Parse(text);
			}
			int n;
			if (CheckFirst(text, "0x", out n) == 0)
			{
				return System.Convert.ToInt32(Convert(text, n + 1), 16);
			}
			if (CheckFirst(text, "0b", out n) == 0)
			{
				return System.Convert.ToInt32(Convert(text, n + 1), 2);
			}
			if (CheckFirst(text, "0o", out n) == 0)
			{
				return System.Convert.ToInt32(Convert(text, n + 1), 8);
			}
			if (CheckFirst(text, ".", out n) != -1)
			{
				int num = text.Length;
				bool flag = false;
				int n2;
				if (CheckLast(text, "d", out n2) != -1)
				{
					num--;
					flag = true;
				}
				else if (CheckLast(text, "f", out n2) != -1)
				{
					num--;
				}
				if (CheckLast(text, "e", out n) != -1)
				{
					string text2 = Convert(text, n);
					if (num != text.Length)
					{
						text2 = text2.Substring(0, text2.Length - 1);
					}
					float num2 = Mathf.Pow(10f, int.Parse(text2));
					if (flag)
					{
						return double.Parse(text.Substring(0, n)) * (double)num2;
					}
					return float.Parse(text.Substring(0, n)) * num2;
				}
				if (flag)
				{
					return double.Parse((num != text.Length) ? text.Substring(0, n2) : text);
				}
				return float.Parse((num != text.Length) ? text.Substring(0, n2) : text);
			}
			if (CheckLast(text, "d", out n) != -1)
			{
				int length = text.Length;
				length--;
				string s = text.Substring(0, n);
				if (CheckLast(text, "e", out n) != -1)
				{
					s = Convert(text, n);
					s = s.Substring(0, s.Length - 1);
					return double.Parse(text.Substring(0, n)) * (double)Mathf.Pow(10f, int.Parse(s));
				}
				return double.Parse(s);
			}
			if (CheckLast(text, "f", out n) != -1)
			{
				int length2 = text.Length;
				length2--;
				string s2 = text.Substring(0, n);
				if (CheckLast(text, "e", out n) != -1)
				{
					s2 = Convert(text, n);
					s2 = s2.Substring(0, s2.Length - 1);
					return float.Parse(text.Substring(0, n)) * Mathf.Pow(10f, int.Parse(s2));
				}
				return float.Parse(s2);
			}
			if (CheckLastLiterals(text, "ul", out n))
			{
				return ulong.Parse(text.Substring(0, n));
			}
			if (CheckLastLiterals(text, "l", out n))
			{
				return long.Parse(text.Substring(0, n));
			}
			if (CheckLastLiterals(text, "u", out n))
			{
				return uint.Parse(text.Substring(0, n));
			}
			if (CheckLastLiterals(text, "m", out n))
			{
				return decimal.Parse(text.Substring(0, n));
			}
			if (CheckLast(text, "e", out n) != -1)
			{
				return float.Parse(text.Substring(0, n)) * Mathf.Pow(10f, int.Parse(Convert(text, n)));
			}
			if (text[0] == '"' && text[text.Length - 1] == '"')
			{
				return text.Remove(text.Length - 1, 1).Remove(0, 1);
			}
			return (!int.TryParse(text, out n)) ? o : ((object)n);
		}

		private static string Convert(string s, int n)
		{
			int num = n + 1;
			return s.Substring(num, s.Length - num);
		}

		private static int CheckFirst(string s, string c, out int n)
		{
			return n = s.ToLower().IndexOf(c);
		}

		private static int CheckLast(string s, string c, out int n)
		{
			return n = s.ToLower().LastIndexOf(c);
		}

		private static bool CheckLastLiterals(string s, string c, out int n)
		{
			return CheckLast(s, c, out n) != -1 && s.Substring(n, s.Length - n).ToLower() == c;
		}
	}
}
