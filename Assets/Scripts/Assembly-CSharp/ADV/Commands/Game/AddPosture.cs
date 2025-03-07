using System.Collections.Generic;
using Manager;

namespace ADV.Commands.Game
{
	public class AddPosture : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "ID" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { "-1" };
			}
		}

		public override void Do()
		{
			base.Do();
			int item = int.Parse(args[0]);
			HashSet<int> value;
			if (!Singleton<Manager.Game>.Instance.saveData.clubContents.TryGetValue(1, out value))
			{
				value = (Singleton<Manager.Game>.Instance.saveData.clubContents[1] = new HashSet<int>());
			}
			value.Add(item);
		}
	}
}
