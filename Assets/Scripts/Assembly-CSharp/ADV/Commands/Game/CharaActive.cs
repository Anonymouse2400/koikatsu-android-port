using ActionGame.Chara;
using UnityEngine;

namespace ADV.Commands.Game
{
	public class CharaActive : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "isActive", "Stand" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
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
			bool flag = bool.Parse(args[num++]);
			args.SafeProc(num++, delegate(string findName)
			{
				Transform transform = base.scenario.commandController.characterStandNulls[findName];
				chara.transform.SetPositionAndRotation(transform.position, transform.rotation);
			});
			bool flag2 = false;
			if (chara.root != null)
			{
				ActionGame.Chara.Base component = chara.root.GetComponent<ActionGame.Chara.Base>();
				if (component != null)
				{
					component.Visible = flag;
					flag2 = true;
				}
			}
			if (!flag2)
			{
				chara.chaCtrl.visibleAll = flag;
			}
		}
	}
}
