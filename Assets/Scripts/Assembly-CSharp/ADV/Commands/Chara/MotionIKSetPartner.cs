using System.Linq;

namespace ADV.Commands.Chara
{
	public class MotionIKSetPartner : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Partner" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					int.MaxValue.ToString(),
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			int no = int.Parse(args[cnt++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			MotionIK[] partners = (from s in CommandBase.RemoveArgsEmpty(GetArgToSplitLast(cnt))
				select base.scenario.commandController.GetChara(int.Parse(s)) into charaData
				select charaData.ikMotion.motionIK).ToArray();
			chara.ikMotion.motionIK.SetPartners(partners);
		}
	}
}
