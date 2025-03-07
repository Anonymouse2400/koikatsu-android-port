using ActionGame.Point;

namespace ADV.Commands.Game
{
	public class GetGatePos : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "ID", "ValueName" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MinValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int key = int.Parse(args[num++]);
			string key2 = args[num++];
			GateInfo gateInfo = base.scenario.advScene.Map.gateInfoDic[key];
			base.scenario.commandController.V3Dic[key2] = gateInfo.pos;
		}
	}
}
