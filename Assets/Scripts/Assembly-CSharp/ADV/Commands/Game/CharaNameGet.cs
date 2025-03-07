using System;
using Illusion.Extensions;

namespace ADV.Commands.Game
{
	public class CharaNameGet : CommandBase
	{
		private string variable;

		private string value;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "ValueName", "NameType" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					SaveData.CharaData.NameType.First.ToString()
				};
			}
		}

		public override void ConvertBeforeArgsProc()
		{
			base.ConvertBeforeArgsProc();
			int num = 0;
			variable = args[num++];
			SaveData.CharaData data = base.scenario.currentChara.data;
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(SaveData.CharaData.NameType)));
			value = (new string[4] { data.firstname, data.lastname, data.Name, data.nickname })[num2];
		}

		public override void Do()
		{
			base.Do();
			base.scenario.Vars[variable] = new ValData(value);
		}
	}
}
