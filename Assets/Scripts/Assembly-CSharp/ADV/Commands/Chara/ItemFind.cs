using System.Linq;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class ItemFind : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "ItemNo", "Name" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					int.MaxValue.ToString(),
					"0",
					"Find"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int key = int.Parse(args[num++]);
			string findName = args[num++];
			CharaData chara = base.scenario.commandController.GetChara(no);
			Transform transform = chara.chaCtrl.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform p) => p.name == findName);
			if (transform != null)
			{
				chara.itemDic[key] = new CharaData.CharaItem(transform.gameObject);
			}
		}
	}
}
