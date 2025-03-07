using System;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Object
{
	public class Scale : CommandBase
	{
		private enum Type
		{
			Add = 0,
			Set = 1
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Name", "Type", "X", "Y", "Z" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					string.Empty,
					Type.Set.ToString(),
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int cnt = 0;
			string key = args[cnt++];
			string self = args[cnt++];
			int num = self.Check(true, Enum.GetNames(typeof(Type)));
			Vector3 v = Vector3.zero;
			CommandBase.CountAddV3(args, ref cnt, ref v);
			switch ((Type)num)
			{
			case Type.Add:
				base.scenario.commandController.Objects[key].transform.localScale += v;
				break;
			case Type.Set:
				base.scenario.commandController.Objects[key].transform.localScale = v;
				break;
			}
		}
	}
}
