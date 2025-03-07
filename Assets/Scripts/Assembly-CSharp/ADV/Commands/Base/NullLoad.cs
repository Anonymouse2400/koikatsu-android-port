using UnityEngine;

namespace ADV.Commands.Base
{
	public class NullLoad : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Version", "Name" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					"0",
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int num2 = int.Parse(args[num++]);
			string text = args[num++];
			text = ((!(text == "デフォルト")) ? base.scenario.advScene.Map.GetParam(text).AssetName.ToLower() : "default");
			base.scenario.commandController.ReleaseNull();
			string assetBundleName = string.Format("{0}{1:00}/{2}{3}", "map/advpos/", num2, text, ".unity3d");
			GameObject[] allAssets = AssetBundleManager.LoadAllAsset(assetBundleName, typeof(GameObject)).GetAllAssets<GameObject>();
			foreach (GameObject gameObject in allAssets)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
				gameObject2.name = gameObject.name;
				base.scenario.commandController.SetNull(gameObject2.transform);
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, false);
		}
	}
}
