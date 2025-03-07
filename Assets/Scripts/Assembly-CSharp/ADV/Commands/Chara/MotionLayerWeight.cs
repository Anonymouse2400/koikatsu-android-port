namespace ADV.Commands.Chara
{
	public class MotionLayerWeight : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "LayerNo", "Weight" };
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
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int layerIndex = int.Parse(args[num++]);
			float weight = float.Parse(args[num++]);
			base.scenario.commandController.GetChara(no).chaCtrl.animBody.SetLayerWeight(layerIndex, weight);
		}
	}
}
