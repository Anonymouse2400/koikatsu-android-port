using System;
using Illusion.Extensions;

namespace ADV.Commands.Chara
{
	public class SiruState : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Parts", "State" };
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
				result = text.Check(true, Enum.GetNames(typeof(ChaFileDefine.SiruParts)));
			}
			string text2 = args[num++];
			int result2;
			if (!int.TryParse(text2, out result2))
			{
				result2 = text2.Check(true, "なし", "少ない", "多い");
			}
			base.scenario.commandController.GetChara(no).chaCtrl.SetSiruFlags((ChaFileDefine.SiruParts)result, (byte)result2);
		}
	}
}
