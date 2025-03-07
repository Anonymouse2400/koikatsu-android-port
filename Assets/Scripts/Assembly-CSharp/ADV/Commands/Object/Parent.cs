using UnityEngine;

namespace ADV.Commands.Object
{
	public class Parent : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Name", "FindType", "ChildName", "RootName" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string key = args[num++];
			string findType = args[num++];
			string childName = args[num++];
			string otherRootName = args[num++];
			Transform parent = ObjectEx.FindGet(findType, childName, otherRootName, base.scenario.commandController);
			base.scenario.commandController.Objects[key].transform.SetParent(parent, false);
		}
	}
}
