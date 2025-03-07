using System.Linq;

namespace ADV.Commands.EtcData
{
	public class MotionLoad : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Bundle", "Asset" };
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
			int num = 0;
			string text = args[num++];
			string assetName = args[num++];
			if (text.IsNullOrEmpty())
			{
				text = base.scenario.LoadBundleName;
			}
			ExcelData asset = AssetBundleManager.LoadAsset(text, assetName, typeof(ExcelData)).GetAsset<ExcelData>();
			foreach (ExcelData.Param item in asset.list)
			{
				base.scenario.commandController.motionDic[item.list[0]] = item.list.Skip(1).ToArray();
			}
			AssetBundleManager.UnloadAssetBundle(text, false);
		}
	}
}
