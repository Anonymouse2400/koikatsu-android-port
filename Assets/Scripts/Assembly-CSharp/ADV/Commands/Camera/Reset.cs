using UnityEngine;

namespace ADV.Commands.Camera
{
	public class Reset : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "ResetIndex" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				int result = 0;
				int.TryParse(args[0], out result);
				if (baseCamCtrl.enabled)
				{
					baseCamCtrl.Reset(result);
				}
				else
				{
					baseCamCtrl.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				}
			}
		}
	}
}
