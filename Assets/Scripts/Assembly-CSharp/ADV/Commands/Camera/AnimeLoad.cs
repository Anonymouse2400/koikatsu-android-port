using UnityEngine;

namespace ADV.Commands.Camera
{
	public class AnimeLoad : Base
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
			string assetBundleName = args[num++];
			string assetName = args[num++];
			RuntimeAnimatorController runtimeAnimatorController = CommonLib.LoadAsset<RuntimeAnimatorController>(assetBundleName, assetName, false, string.Empty);
			if (runtimeAnimatorController != null)
			{
				base.scenario.AdvCamera.GetOrAddComponent<Animator>().runtimeAnimatorController = runtimeAnimatorController;
			}
			AssetBundleManager.UnloadAssetBundle(assetBundleName, false);
		}
	}
}
