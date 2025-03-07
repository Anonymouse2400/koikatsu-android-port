using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetPosition : Base
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
			UnityEngine.Camera advCamera = base.scenario.AdvCamera;
			if (!(advCamera == null))
			{
				int cnt = 0;
				Vector3 v = advCamera.transform.position;
				Vector3 pos;
				if (base.scenario.commandController.GetV3Dic(args[cnt], out pos))
				{
					v = pos;
				}
				else
				{
					CommandBase.CountAddV3(args, ref cnt, ref v);
				}
				BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
				if (baseCamCtrl != null)
				{
					baseCamCtrl.SetCamera(v, baseCamCtrl.CameraAngle, Quaternion.Euler(baseCamCtrl.CameraAngle), baseCamCtrl.CameraDir);
				}
				else
				{
					advCamera.transform.position = v;
				}
			}
		}
	}
}
