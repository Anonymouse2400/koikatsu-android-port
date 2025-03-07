using Localize.Translate;

namespace ADV.Commands.Base
{
	public class ReplaceLanguage : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Src", "Dst" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return null;
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string key = args[num++];
			string dst = string.Empty;
			num += Localize.Translate.Manager.Language;
			args.SafeProc(num++, delegate(string s)
			{
				dst = s;
			});
			base.scenario.Replaces[key] = dst;
		}
	}
}
