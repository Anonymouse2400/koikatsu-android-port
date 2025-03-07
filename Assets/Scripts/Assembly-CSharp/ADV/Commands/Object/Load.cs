using UnityEngine;

namespace ADV.Commands.Object
{
	public class Load : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Name", "Bundle", "Asset", "Manifest" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[4]
				{
					string.Empty,
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
			string text = args[num++];
			string assetBundleName = args[num++];
			string assetName = args[num++];
			string manifestAssetBundleName = args[num++];
			GameObject asset = AssetBundleManager.LoadAsset(assetBundleName, assetName, typeof(GameObject), manifestAssetBundleName).GetAsset<GameObject>();
			GameObject gameObject = UnityEngine.Object.Instantiate(asset);
			if (!text.IsNullOrEmpty())
			{
				gameObject.name = text;
			}
			else
			{
				gameObject.name = asset.name;
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, false, manifestAssetBundleName);
			base.scenario.commandController.SetObject(gameObject);
		}
	}
}
