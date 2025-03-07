namespace ADV.Commands.Chara
{
	public class MotionDefault : CommandBase
	{
		private int no;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "No" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
					"0",
					"1"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			no = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			ChaControl chaCtrl = chara.chaCtrl;
			MotionIK motionIK = chara.MotionDefault();
			motionIK.SetPartners(chara.ikMotion.motionIK.partners);
			chara.ikMotion.Create(chaCtrl, motionIK);
		}
	}
}
