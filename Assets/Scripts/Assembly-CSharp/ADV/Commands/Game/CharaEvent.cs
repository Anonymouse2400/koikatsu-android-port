using System;
using Illusion.Extensions;

namespace ADV.Commands.Game
{
	public class CharaEvent : CommandBase
	{
		private enum Type
		{
			Check = 0,
			Add = 1,
			Sub = 2
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Type", "No", "Event", "TrueName", "FalseName" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					Type.Check.ToString(),
					int.MaxValue.ToString(),
					"0",
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(Type)));
			int no = int.Parse(args[num++]);
			CharaData chara = base.scenario.commandController.GetChara(no);
			int item = int.Parse(args[num++]);
			switch ((Type)num2)
			{
			case Type.Check:
			{
				string text = args[num++];
				string text2 = args[num++];
				base.scenario.SearchTagJumpOrOpenFile((!chara.heroine.talkEvent.Contains(item)) ? text2 : text, base.localLine);
				break;
			}
			case Type.Add:
				chara.heroine.talkEvent.Add(item);
				break;
			case Type.Sub:
				chara.heroine.talkEvent.Remove(item);
				break;
			}
		}
	}
}
