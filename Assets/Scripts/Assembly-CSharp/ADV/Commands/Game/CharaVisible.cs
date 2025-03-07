using System;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Game
{
	public class CharaVisible : CommandBase
	{
		private enum Target
		{
			All = 0,
			Head = 1,
			Body = 2,
			Son = 3,
			Gomu = 4
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "No", "Target", "isActive", "Stand" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					int.MaxValue.ToString(),
					Target.All.ToString(),
					bool.TrueString,
					string.Empty
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
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(Target)));
			bool flag = bool.Parse(args[num++]);
			args.SafeProc(num++, delegate(string findName)
			{
				Transform transform = base.scenario.commandController.characterStandNulls[findName];
				chara.transform.SetPositionAndRotation(transform.position, transform.rotation);
			});
			ChaFileStatus fileStatus = chaCtrl.fileStatus;
			switch ((Target)num2)
			{
			case Target.All:
				chaCtrl.visibleAll = flag;
				break;
			case Target.Head:
				fileStatus.visibleHeadAlways = flag;
				break;
			case Target.Body:
				fileStatus.visibleBodyAlways = flag;
				break;
			case Target.Son:
				fileStatus.visibleSonAlways = flag;
				break;
			case Target.Gomu:
				fileStatus.visibleGomu = flag;
				break;
			}
		}
	}
}
