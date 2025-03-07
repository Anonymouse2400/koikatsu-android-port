using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetDirection : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Dir" };
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
				baseCamCtrl.CameraDir = new Vector3(0f, 0f, float.Parse(args[0]));
			}
		}
	}
}
