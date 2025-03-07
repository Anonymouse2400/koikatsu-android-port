using System;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public class YureMotion : AssetBundleData
	{
		public YureCtrl yureCtrl { get; private set; }

		public YureMotion()
		{
		}

		public YureMotion(string bundle, string asset)
			: base(bundle, asset)
		{
		}

		public void Create(ChaControl chaCtrl, YureCtrl yureCtrl = null)
		{
			if (yureCtrl != null)
			{
				this.yureCtrl = yureCtrl;
				return;
			}
			this.yureCtrl = new YureCtrl();
			this.yureCtrl.Init(chaCtrl);
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
			bool flag = false;
			if (yureCtrl == null)
			{
				Create(chaCtrl);
			}
			if (!asset.IsNullOrEmpty())
			{
				base.asset = asset;
				if (!bundle.IsNullOrEmpty())
				{
					base.bundle = bundle;
				}
				yureCtrl.LoadExcel(base.bundle, base.asset);
				flag = true;
			}
			if (!state.IsNullOrEmpty())
			{
				flag = true;
			}
			if (flag)
			{
				yureCtrl.Proc(state);
			}
		}
	}
}
