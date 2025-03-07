using UnityEngine;

namespace ADV.Commands.Camera
{
	public class Active : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "isActive", "isEnabled" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					bool.TrueString,
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			UnityEngine.Camera advCamera = base.scenario.AdvCamera;
			if (advCamera != null)
			{
				int num = 0;
				bool active = bool.Parse(args[num++]);
				advCamera.gameObject.SetActive(active);
				BaseCameraControl component = advCamera.gameObject.GetComponent<BaseCameraControl>();
				bool result = false;
				if (!bool.TryParse(args[num++], out result))
				{
					component.enabled = result;
				}
			}
		}
	}
}
