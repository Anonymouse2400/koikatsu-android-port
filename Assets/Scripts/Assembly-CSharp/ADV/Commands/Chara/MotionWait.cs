namespace ADV.Commands.Chara
{
	public class MotionWait : CommandBase
	{
		private int layerNo;

		private float time;

		private ChaControl chaCtrl;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "LayerNo", "Time" };
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
			int no = int.Parse(args[num++]);
			layerNo = int.Parse(args[num++]);
			time = float.Parse(args[num++]);
			chaCtrl = base.scenario.commandController.GetChara(no).chaCtrl;
		}

		public override bool Process()
		{
			base.Process();
			return chaCtrl.getAnimatorStateInfo(layerNo).normalizedTime >= time;
		}
	}
}
