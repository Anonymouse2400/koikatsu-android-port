namespace ADV.Commands.EventCG
{
	public class Release : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "isMotionContinue" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { bool.FalseString };
			}
		}

		public override void Do()
		{
			base.Do();
			if (!Common.Release(base.scenario))
			{
				return;
			}
			bool isMotionContinue = false;
			args.SafeProc(0, delegate(string s)
			{
				isMotionContinue = bool.Parse(s);
			});
			if (!isMotionContinue)
			{
				foreach (CharaData value in base.scenario.commandController.Characters.Values)
				{
					if (!(value.chaCtrl == null) && value.chaCtrl.loadEnd && value.data is SaveData.Heroine)
					{
						MotionIK motionIK = value.MotionDefault();
						motionIK.SetPartners(value.ikMotion.motionIK.partners);
						value.ikMotion.Create(value.chaCtrl, motionIK);
					}
				}
			}
			base.scenario.commandController.useCorrectCamera = true;
		}
	}
}
