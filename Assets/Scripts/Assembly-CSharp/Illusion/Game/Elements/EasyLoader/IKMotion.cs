using System;
using UnityEngine;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class IKMotion : AssetBundleData
	{
		public MotionIK motionIK { get; private set; }

		public IKMotion()
		{
		}

		public IKMotion(string bundle, string asset)
			: base(bundle, asset)
		{
		}

		public void Create(ChaControl chaCtrl, MotionIK motionIK = null, params MotionIK[] partners)
		{
			if (motionIK != null)
			{
				this.motionIK = motionIK;
				return;
			}
			this.motionIK = new MotionIK(chaCtrl);
			this.motionIK.SetPartners(partners);
		}

		public void Setting(ChaControl chaCtrl, string state)
		{
			Setting(chaCtrl, bundle, asset, state, false);
		}

		public void Setting(ChaControl chaCtrl, string bundle, string asset, string state, bool isCheck)
		{
			if (isCheck && !Check(bundle, asset))
			{
				return;
			}
			if (motionIK == null)
			{
				Create(chaCtrl, null);
			}
			bool flag = false;
			if (!asset.IsNullOrEmpty())
			{
				base.asset = asset;
				if (!bundle.IsNullOrEmpty())
				{
					base.bundle = bundle;
				}
				motionIK.LoadData(GetAsset<TextAsset>());
				UnloadBundle();
				flag = true;
			}
			if (!state.IsNullOrEmpty())
			{
				flag = true;
			}
			motionIK.enabled = flag && motionIK.GetNowState(state) != null;
			if (motionIK.enabled)
			{
				motionIK.Calc(state);
			}
		}
	}
}
