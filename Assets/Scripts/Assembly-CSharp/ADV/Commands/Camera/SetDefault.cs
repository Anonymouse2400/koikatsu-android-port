using UnityEngine;

namespace ADV.Commands.Camera
{
	public class SetDefault : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return null;
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
			UnityEngine.Camera backCamera = base.scenario.BackCamera;
			if (advCamera != null && backCamera != null)
			{
				Transform transform = backCamera.transform;
				advCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
				advCamera.fieldOfView = backCamera.fieldOfView;
			}
		}
	}
}
