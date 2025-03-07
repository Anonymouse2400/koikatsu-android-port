using Illusion;
using Illusion.Extensions;

namespace ADV.Commands.Base
{
	public class IF : CommandBase
	{
		private const string compStr = "check";

		private string left;

		private string center;

		private string right;

		private string jumpTrue;

		private string jumpFalse;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Left", "Center", "Right", "True", "False" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					"a",
					string.Empty,
					"b",
					"tagA",
					"tagB"
				};
			}
		}

		public override void Convert(string fileName, ref string[] args)
		{
			if (Cast(ref args[1]))
			{
			}
		}

		private static bool Cast(ref string arg)
		{
			int num = Utils.Comparer.STR.Check(arg);
			bool result = true;
			if (num == -1)
			{
				if (arg.Compare("check", true))
				{
					num = Utils.Enum<Utils.Comparer.Type>.Length;
				}
				else
				{
					num = 0;
					result = false;
				}
			}
			arg = num.ToString();
			return result;
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			int num = 0;
			left = args[num++];
			center = args[num++];
			right = args[num++];
		}

		public override void Do()
		{
			base.Do();
			ValData value = null;
			ValData value2 = null;
			int num = int.Parse(center);
			if (num < Utils.Enum<Utils.Comparer.Type>.Length)
			{
				if (!base.scenario.Vars.TryGetValue(left, out value))
				{
					value = new ValData(VAR.CheckLiterals(left));
				}
				if (!base.scenario.Vars.TryGetValue(right, out value2))
				{
					value2 = new ValData(value.Convert(right));
				}
			}
			bool flag = false;
			switch ((Utils.Comparer.Type)num)
			{
			case Utils.Comparer.Type.Equal:
				flag = value.o.Equals(value2.o);
				break;
			case Utils.Comparer.Type.NotEqual:
				flag = !value.o.Equals(value2.o);
				break;
			case Utils.Comparer.Type.Greater:
				flag = value > value2;
				break;
			case Utils.Comparer.Type.Lesser:
				flag = value < value2;
				break;
			case Utils.Comparer.Type.Over:
				flag = value >= value2;
				break;
			case Utils.Comparer.Type.Under:
				flag = value <= value2;
				break;
			default:
				flag = base.scenario.Vars.ContainsKey(left);
				break;
			}
			jumpTrue = args[3];
			jumpFalse = args[4];
			string jump = ((!flag) ? jumpFalse : jumpTrue);
			base.scenario.SearchTagJumpOrOpenFile(jump, base.localLine);
		}
	}
}
