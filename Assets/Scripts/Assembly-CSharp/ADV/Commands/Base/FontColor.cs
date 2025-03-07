using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class FontColor : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Color", "Target" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"white",
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			string target = string.Empty;
			args.SafeProc(num++, delegate(string s)
			{
				target = s;
			});
			Color? colorCheck = text.GetColorCheck();
			if (colorCheck.HasValue)
			{
				base.scenario.commandController.fontColor.Set(target, colorCheck.Value);
				return;
			}
			int num2 = -1;
			switch (text.ToLower())
			{
			default:
				return;
			case "color0":
				num2 = 0;
				break;
			case "color1":
			case "color2":
				num2 = 1;
				break;
			}
			base.scenario.commandController.fontColor.Set(target, num2);
		}
	}
}
