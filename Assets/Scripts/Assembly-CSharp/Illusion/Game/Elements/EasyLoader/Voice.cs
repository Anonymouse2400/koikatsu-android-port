using System;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class Voice : AssetBundleData
	{
		public int personality;

		public virtual void Setting(ChaControl chaCtrl, int personality, string bundle, string asset)
		{
			bool flag = false;
			if (!asset.IsNullOrEmpty())
			{
				base.asset = asset;
				if (!bundle.IsNullOrEmpty())
				{
					base.bundle = bundle;
				}
				flag = true;
			}
			if (flag)
			{
				chaCtrl.SetVoiceTransform(Utils.Voice.OnecePlayChara(new Utils.Voice.Setting
				{
					no = personality,
					assetBundleName = base.bundle,
					assetName = base.asset,
					pitch = chaCtrl.chaFile.parameter.voicePitch,
					voiceTrans = chaCtrl.GetReferenceInfo(ChaReference.RefObjKey.HeadParent).transform
				}));
			}
		}

		public virtual void Setting(ChaControl chaCtrl)
		{
			Setting(chaCtrl, personality, bundle, asset);
		}
	}
}
