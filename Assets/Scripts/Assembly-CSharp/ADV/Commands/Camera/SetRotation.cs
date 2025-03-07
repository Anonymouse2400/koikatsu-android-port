using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetRotation : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Pitch", "Yaw", "Roll" };
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
			UnityEngine.Camera advCamera = base.scenario.AdvCamera;
			if (!(advCamera == null))
			{
				int cnt = 0;
				Vector3 pos = advCamera.transform.rotation.eulerAngles;
				if (!base.scenario.commandController.GetV3Dic(args[cnt], out pos))
				{
					CommandBase.CountAddV3(args, ref cnt, ref pos);
				}
				BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
				if (baseCamCtrl != null)
				{
					baseCamCtrl.SetCamera(advCamera.transform.position, pos, Quaternion.Euler(pos), baseCamCtrl.CameraDir);
				}
				else
				{
					advCamera.transform.rotation = Quaternion.Euler(pos);
				}
			}
		}
	}
}
