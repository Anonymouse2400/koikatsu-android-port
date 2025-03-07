namespace ADV.Commands.Base
{
	public class BackGroundLoad : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Bundle", "Asset" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			string assetName = args[num++];
			base.scenario.BGParam.Load(text, assetName);
			if (text.IsNullOrEmpty())
			{
				return;
			}
			base.scenario.BGParam.visibleAll = true;
			if (base.scenario.AdvCamera != null)
			{
				CameraEffectorColorMask componentInChildren = base.scenario.AdvCamera.GetComponentInChildren<CameraEffectorColorMask>();
				if (componentInChildren != null)
				{
					componentInChildren.Enabled = true;
				}
			}
		}
	}
}
