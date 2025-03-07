using UnityEngine;

namespace ADV.Commands.Chara
{
	public class CoordinateChange : CommandBase
	{
		private int no;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "No", "Bundle", "Asset" };
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
			no = int.Parse(args[num++]);
			string assetBundleName = args[num++];
			string assetName = args[num++];
			TextAsset textAsset = CommonLib.LoadAsset<TextAsset>(assetBundleName, assetName, false, string.Empty);
			if (textAsset != null)
			{
				ChaControl chaCtrl = base.scenario.commandController.GetChara(no).chaCtrl;
				chaCtrl.nowCoordinate.LoadFile(textAsset);
				chaCtrl.Reload(false, true, true, true);
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
		}
	}
}
