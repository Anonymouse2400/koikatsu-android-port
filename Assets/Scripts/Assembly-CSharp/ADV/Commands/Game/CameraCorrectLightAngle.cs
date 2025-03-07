using UnityEngine;

namespace ADV.Commands.Game
{
	public class CameraCorrectLightAngle : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "isLocal", "X", "Y" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					bool.TrueString,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			bool flag = bool.Parse(args[cnt++]);
			Vector3 pos;
			if (!base.scenario.commandController.GetV3Dic(args[cnt], out pos))
			{
				CommandBase.CountAddV3(args, ref cnt, ref pos);
			}
			if (flag)
			{
				base.scenario.advScene.correctLightAngle.lightTrans.localRotation = Quaternion.Euler(pos.x, pos.y, 0f);
			}
			else
			{
				base.scenario.advScene.correctLightAngle.lightTrans.rotation = Quaternion.Euler(pos.x, pos.y, 0f);
			}
		}
	}
}
