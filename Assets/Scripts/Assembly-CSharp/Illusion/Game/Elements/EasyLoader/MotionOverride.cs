using System;
using UnityEngine;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class MotionOverride : Motion
	{
		public AssetBundleData overrideMotion = new AssetBundleData();

		public MotionOverride()
		{
		}

		public MotionOverride(string bundle, string asset)
			: base(bundle, asset)
		{
		}

		public MotionOverride(string bundle, string asset, string state)
			: base(bundle, asset, state)
		{
		}

		public override bool Setting(Animator animator)
		{
			return Setting(animator, bundle, asset, overrideMotion.bundle, overrideMotion.asset, state, false);
		}

		public bool Setting(Animator animator, string bundle, string asset, string overrideBundle, string overrideAsset, string state, bool isCheck)
		{
			bool flag = Setting(animator, bundle, asset, state, isCheck);
			if ((flag || !isCheck || overrideMotion.Check(overrideBundle, overrideAsset)) && !overrideAsset.IsNullOrEmpty())
			{
				overrideMotion.asset = overrideAsset;
				if (!overrideBundle.IsNullOrEmpty())
				{
					overrideMotion.bundle = overrideBundle;
				}
				UnloadBundle();
				overrideMotion.UnloadBundle();
				animator.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(animator.runtimeAnimatorController, overrideMotion.GetAsset<RuntimeAnimatorController>());
				flag = true;
			}
			return flag;
		}
	}
}
