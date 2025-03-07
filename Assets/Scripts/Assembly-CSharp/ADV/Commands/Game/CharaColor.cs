using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Game
{
	public class CharaColor : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "isActive", "Color" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
					bool.FalseString,
					null
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			ChaControl chaCtrl = chara.chaCtrl;
			ChaFileStatus status = chaCtrl.fileStatus;
			status.visibleSimple = bool.Parse(args[num++]);
			args.SafeProc(num++, delegate(string colorStr)
			{
				Color? colorCheck = colorStr.GetColorCheck();
				status.simpleColor = colorCheck.Value;
			});
		}
	}
}
