using Manager;

namespace ADV.Commands.EtcData
{
	public class ExpLoad : CommandBase
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
			Manager.Game.LoadExpExcelData(base.scenario.commandController.expDic, AssetBundleManager.LoadAsset(text, assetName, typeof(ExcelData)).GetAsset<ExcelData>());
			AssetBundleManager.UnloadAssetBundle(text, false);
		}
	}
}
