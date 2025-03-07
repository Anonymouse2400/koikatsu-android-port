using UnityEngine;

namespace ADV.Commands.Base
{
	public class Reset : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "Type", "Pos", "Ang" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
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
			int num2 = int.Parse(args[num++]);
			int cnt = 0;
			Vector3 v = Vector3.zero;
			CommandBase.CountAddV3(GetArgToSplit(num++), ref cnt, ref v);
			Vector3 v2 = Vector3.zero;
			cnt = 0;
			CommandBase.CountAddV3(GetArgToSplit(num++), ref cnt, ref v2);
			switch (num2)
			{
			case 0:
				base.scenario.commandController.BasePositon.SetPositionAndRotation(v, Quaternion.Euler(v2));
				break;
			case 1:
				base.scenario.commandController.Character.SetPositionAndRotation(v, Quaternion.Euler(v2));
				break;
			}
		}
	}
}
