using Illusion.Extensions;
using UnityEngine;

namespace ADV.Commands.Effect
{
	public class ImageLoad : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[4] { "Bundle", "Asset", "Type", "isMain" };
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
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			string assetName = args[num++];
			string self = args[num++];
			if (!self.IsNullOrEmpty())
			{
				bool isFront = !self.Compare("back", true);
				ADVFade advFade = base.scenario.advScene.AdvFade;
				advFade.LoadSprite(isFront, text, assetName);
				return;
			}
			EMTransition eMTFade = base.scenario.advScene.EMTFade;
			Texture2D asset = AssetBundleManager.LoadAsset(text, assetName, typeof(Texture2D)).GetAsset<Texture2D>();
			if (bool.Parse(args[num++]))
			{
				eMTFade.SetTexture(asset);
			}
			else
			{
				eMTFade.SetGradationTexture(asset);
			}
			AssetBundleManager.UnloadAssetBundle(text, false);
		}
	}
}
