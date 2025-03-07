using System;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Object
{
	public class AnimeParam : CommandBase
	{
		private enum Type
		{
			Float = 0,
			Int = 1,
			Bool = 2,
			Trigger = 3,
			Weight = 4
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Name", "Type", "Param1", "Param2" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					string.Empty,
					Type.Float.ToString(),
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
			Animator component = base.scenario.commandController.Objects[key].GetComponent<Animator>();
			string self = args[num++];
			switch ((Type)self.Check(true, Enum.GetNames(typeof(Type))))
			{
			case Type.Float:
				component.SetFloat(args[num++], float.Parse(args[num++]));
				break;
			case Type.Int:
				component.SetInteger(args[num++], int.Parse(args[num++]));
				break;
			case Type.Bool:
				component.SetBool(args[num++], bool.Parse(args[num++]));
				break;
			case Type.Trigger:
				component.SetTrigger(args[num++]);
				break;
			case Type.Weight:
				component.SetLayerWeight(int.Parse(args[num++]), float.Parse(args[num++]));
				break;
			}
		}
	}
}
