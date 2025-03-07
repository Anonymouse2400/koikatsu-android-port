using UnityEngine;

namespace ADV.Commands.Object
{
	public class Delete : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[1] { "Name" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { string.Empty };
			}
		}

		public override void Do()
		{
			base.Do();
			string key = args[0];
			GameObject obj = base.scenario.commandController.Objects[key];
			UnityEngine.Object.Destroy(obj);
			base.scenario.commandController.Objects.Remove(key);
		}
	}
}
