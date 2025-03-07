using Illusion;
using UnityEngine;

namespace ADV.Commands.Object
{
	public class Create : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Name", "Component" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			GameObject gameObject = new GameObject(args[cnt++]);
			string[] array = CommandBase.RemoveArgsEmpty(GetArgToSplitLast(cnt));
			foreach (string typeName in array)
			{
				gameObject.AddComponent(Utils.Type.Get(typeName));
			}
			base.scenario.commandController.SetObject(gameObject);
		}
	}
}
