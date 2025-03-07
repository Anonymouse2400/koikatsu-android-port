namespace ADV.Commands.Camera
{
	public class SetTargetFront : Base
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Dir", "isReset" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"0",
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			BaseCameraControl baseCamCtrl = base.scenario.BaseCamCtrl;
			if (baseCamCtrl != null)
			{
				int num = 0;
				float result = 0f;
				bool flag = float.TryParse(args.SafeGet(num++), out result);
				bool isReset = bool.Parse(args[num++]);
				if (flag)
				{
					baseCamCtrl.FrontTarget(baseCamCtrl.targetObj, isReset, result);
				}
				else
				{
					baseCamCtrl.FrontTarget(baseCamCtrl.targetObj, isReset);
				}
			}
		}
	}
}
