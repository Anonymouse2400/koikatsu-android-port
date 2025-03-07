using System;
using Illusion.Extensions;

namespace ADV.Commands.Chara
{
	public class Coordinate : CommandBase
	{
		private int no;

		private ChaFileDefine.CoordinateType type;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "No", "Type" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"0",
					ChaFileDefine.CoordinateType.School01.ToString()
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			no = int.Parse(args[num++]);
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(ChaFileDefine.CoordinateType)));
			type = (ChaFileDefine.CoordinateType)num2;
			base.scenario.commandController.GetChara(no).chaCtrl.ChangeCoordinateTypeAndReload(type);
		}
	}
}
