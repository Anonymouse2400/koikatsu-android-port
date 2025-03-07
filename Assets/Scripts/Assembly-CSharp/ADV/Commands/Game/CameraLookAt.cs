using UnityEngine;

namespace ADV.Commands.Game
{
	public class CameraLookAt : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "X", "Y", "Z" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { string.Empty };
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			Vector3 pos;
			if (!base.scenario.commandController.GetV3Dic(args[cnt], out pos))
			{
				CommandBase.CountAddV3(args, ref cnt, ref pos);
			}
			base.scenario.AdvCamera.transform.LookAt(pos);
		}
	}
}
