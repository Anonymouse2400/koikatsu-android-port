using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetNull : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Name" };
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
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				Transform target = GetTarget(args[0].Split(','));
				if (target != null)
				{
					baseCamCtrl.SetCamera(target.position, target.eulerAngles, target.rotation, Vector3.zero);
				}
			}
		}
	}
}
