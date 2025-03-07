using System.Collections.Generic;

namespace ADV.Commands.Chara2D
{
	public class Delete : CommandBase
	{
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
				return new string[1] { "0" };
			}
		}

		public override void Do()
		{
			base.Do();
			int key = int.Parse(args[0]);
			Dictionary<int, CharaData2D> characters2D = base.scenario.commandController.Characters2D;
			characters2D[key].Release();
			characters2D.Remove(key);
		}
	}
}
