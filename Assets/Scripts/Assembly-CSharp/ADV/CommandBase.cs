using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace ADV
{
	public abstract class CommandBase : ICommand
	{
		public string[] args;

		public const int currentCharaDefaultIndex = int.MaxValue;

		public int localLine { get; set; }

		public TextScenario scenario { get; private set; }

		public Command command { get; private set; }

		public abstract string[] ArgsLabel { get; }

		public abstract string[] ArgsDefault { get; }

		public void Set(Command command)
		{
			this.command = command;
		}

		public static string[] RemoveArgsEmpty(string[] args)
		{
			return (args != null) ? args.Where((string s) => !s.IsNullOrEmpty()).ToArray() : null;
		}

		public string[] RemoveArgsEmpty()
		{
			return RemoveArgsEmpty(args);
		}

		public static string[] GetArgToSplit(int cnt, params string[] args)
		{
			string[] ret = null;
			args.SafeProc(cnt, delegate(string s)
			{
				ret = s.Split(',');
			});
			return ret;
		}

		public string[] GetArgToSplit(int cnt)
		{
			return GetArgToSplit(cnt, args);
		}

		public static string[] GetArgToSplitLast(int cnt, params string[] args)
		{
			List<string> list = new List<string>();
			while (true)
			{
				string[] argToSplit = GetArgToSplit(cnt++, args);
				if (argToSplit.IsNullOrEmpty())
				{
					break;
				}
				list.AddRange(argToSplit);
			}
			return list.ToArray();
		}

		public string[] GetArgToSplitLast(int cnt)
		{
			return GetArgToSplitLast(cnt, args);
		}

		public void Initialize(TextScenario scenario, Command command, string[] args)
		{
			this.scenario = scenario;
			this.command = command;
			string[] argsDefault = ArgsDefault;
			if (argsDefault != null)
			{
				int a = argsDefault.Length;
				int num = args.Length;
				int num2 = Mathf.Min(a, num);
				for (int i = 0; i < num2; i++)
				{
					if (!args[i].IsNullOrEmpty())
					{
						argsDefault[i] = args[i];
					}
				}
				List<string> list = new List<string>(argsDefault);
				for (int j = list.Count; j < num; j++)
				{
					list.Add(args[j]);
				}
				this.args = list.ToArray();
			}
			else
			{
				this.args = args.ToArray();
			}
		}

		public virtual void Convert(string fileName, ref string[] args)
		{
		}

		public virtual void ConvertBeforeArgsProc()
		{
		}

		protected static void CountAddV3(string[] args, ref int cnt, ref Vector3 v)
		{
			if (args == null)
			{
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				float result;
				if (float.TryParse(args.SafeGet(cnt++), out result))
				{
					v[i] = result;
				}
			}
		}

		protected static void CountAddV3(ref int cnt)
		{
			cnt += 3;
		}

		protected static Vector3 LerpV3(Vector3 start, Vector3 end, float t)
		{
			return Vector3.Lerp(start, end, t);
		}

		protected static Vector3 LerpAngleV3(Vector3 start, Vector3 end, float t)
		{
			Vector3 zero = Vector3.zero;
			for (int i = 0; i < 3; i++)
			{
				zero[i] = Mathf.LerpAngle(start[i], end[i], t);
			}
			return zero;
		}

		public virtual void Do()
		{
		}

		public virtual bool Process()
		{
			return true;
		}

		public virtual void Result(bool processEnd)
		{
		}

		[Conditional("ADV_DEBUG")]
		protected void ErrorCheckLog(bool isError, string message)
		{
			if (!isError)
			{
			}
		}

		[Conditional("ADV_DEBUG")]
		private void dbPrint(string procName, string[] command)
		{
		}

		[Conditional("__DEBUG_PROC__")]
		private void dbPrintDebug(string procName, string[] command)
		{
		}
	}
}
