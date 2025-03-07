using UnityEngine;

namespace ADV.Commands.Chara
{
	public class SetPositionLocal : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[7] { "No", "X", "Y", "Z", "Pitch", "Yaw", "Roll" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			int no = int.Parse(args[cnt++]);
			Transform transform = base.scenario.commandController.GetChara(no).transform;
			Vector3 pos;
			if (!base.scenario.commandController.GetV3Dic(args.SafeGet(cnt), out pos))
			{
				pos = transform.localPosition;
				CommandBase.CountAddV3(args, ref cnt, ref pos);
			}
			else
			{
				CommandBase.CountAddV3(ref cnt);
			}
			transform.localPosition = pos;
			Vector3 pos2;
			if (!base.scenario.commandController.GetV3Dic(args.SafeGet(cnt), out pos2))
			{
				pos2 = transform.localEulerAngles;
				CommandBase.CountAddV3(args, ref cnt, ref pos2);
			}
			else
			{
				CommandBase.CountAddV3(ref cnt);
			}
			transform.localEulerAngles = pos2;
		}
	}
}
