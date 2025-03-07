using System.Linq;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class ItemCreate : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[7] { "No", "ItemNo", "Bundle", "Asset", "Root", "isWorldPositionStays", "Manifest" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[7]
				{
					int.MaxValue.ToString(),
					"0",
					string.Empty,
					string.Empty,
					string.Empty,
					bool.FalseString,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			int key = int.Parse(args[num++]);
			string bundle = args[num++];
			string asset = args[num++];
			string root = args[num++];
			bool worldPositionStays = bool.Parse(args[num++]);
			string manifest = args.SafeGet(num++);
			CharaData chara = base.scenario.commandController.GetChara(no);
			Transform transform = null;
			if (!root.IsNullOrEmpty() && chara.chaCtrl != null)
			{
				transform = chara.chaCtrl.transform.GetComponentsInChildren<Transform>(true).FirstOrDefault((Transform p) => p.name == root);
			}
			if (transform == null)
			{
				transform = base.scenario.advScene.transform;
			}
			CharaData.CharaItem value;
			if (chara.itemDic.TryGetValue(key, out value))
			{
				value.Delete();
			}
			value = new CharaData.CharaItem();
			value.LoadObject(bundle, asset, transform, worldPositionStays, manifest);
			chara.itemDic[key] = value;
		}
	}
}
