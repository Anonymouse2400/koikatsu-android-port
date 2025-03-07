using System;
using Illusion.Extensions;

namespace ADV.Commands.Chara
{
	public class ClothState : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Kind", "State" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					"0",
					ChaFileDefine.ClothesKind.top.ToString(),
					"0"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			string text = args[num++];
			int result;
			if (!int.TryParse(text, out result))
			{
				result = text.Check(true, Enum.GetNames(typeof(ChaFileDefine.ClothesKind)));
			}
			int num2 = int.Parse(args[num++]);
			base.scenario.commandController.GetChara(no).chaCtrl.SetClothesState(result, (byte)num2);
		}
	}
}
