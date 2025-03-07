using System;
using System.Linq;
using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class StandFindPosition : CommandBase
	{
		public enum Type
		{
			World = 0,
			Tag = 1,
			Null = 2,
			EventCG = 3
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "No", "Type", "Name", "Child" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					int.MaxValue.ToString(),
					Type.World.ToString(),
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			string self = args[num++];
			int num2 = self.Check(true, Enum.GetNames(typeof(Type)));
			string findName = args[num++];
			string childName = string.Empty;
			args.SafeProc(num++, delegate(string s)
			{
				childName = s;
			});
			Transform transform = base.scenario.commandController.GetChara(no).transform;
			Transform stand = null;
			switch ((Type)num2)
			{
			case Type.World:
				GameObject.Find(findName).SafeProc(delegate(GameObject p)
				{
					stand = p.transform;
				});
				break;
			case Type.Tag:
			{
				GameObject gameObject = GameObject.FindGameObjectWithTag(findName);
				stand = gameObject.transform;
				if (!childName.IsNullOrEmpty())
				{
					stand = gameObject.GetComponentsInChildren<Transform>().FirstOrDefault((Transform t) => t.name.Compare(childName, true));
				}
				break;
			}
			case Type.Null:
				stand = base.scenario.commandController.NullDic[findName];
				if (!childName.IsNullOrEmpty())
				{
					stand = stand.GetComponentsInChildren<Transform>().FirstOrDefault((Transform t) => t.name.Compare(childName, true));
				}
				break;
			case Type.EventCG:
				stand = base.scenario.commandController.EventCGRoot.Children().Find((Transform p) => p.name.Compare(findName, true));
				if (!childName.IsNullOrEmpty())
				{
					stand = stand.Children().Find((Transform t) => t.name.Compare(childName, true));
				}
				break;
			}
			transform.SetPositionAndRotation(stand.position, stand.rotation);
		}
	}
}
