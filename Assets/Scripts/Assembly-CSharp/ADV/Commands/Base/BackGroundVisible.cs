namespace ADV.Commands.Base
{
	public class BackGroundVisible : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isActive" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { bool.TrueString };
			}
		}

		public override void Do()
		{
			base.Do();
			bool flag = bool.Parse(args[0]);
			base.scenario.BGParam.visibleAll = flag;
			if (base.scenario.AdvCamera != null)
			{
				CameraEffectorColorMask componentInChildren = base.scenario.AdvCamera.GetComponentInChildren<CameraEffectorColorMask>();
				if (componentInChildren != null)
				{
					componentInChildren.Enabled = flag;
				}
			}
		}
	}
}
