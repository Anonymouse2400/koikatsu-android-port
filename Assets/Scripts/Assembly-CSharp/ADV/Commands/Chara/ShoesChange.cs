using System;
using Illusion.Extensions;

namespace ADV.Commands.Chara
{
	public class ShoesChange : CommandBase
	{
		private enum Type
		{
			In = 0,
			Out = 1
		}

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
					Type.In.ToString()
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
				result = text.Check(true, Enum.GetNames(typeof(Type)));
			}
			base.scenario.commandController.GetChara(no).chaCtrl.fileStatus.shoesType = (byte)result;
		}
	}
}
