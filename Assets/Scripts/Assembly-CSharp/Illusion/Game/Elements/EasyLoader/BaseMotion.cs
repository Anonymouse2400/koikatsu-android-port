using System;

namespace Illusion.Game.Elements.EasyLoader
{
	[Serializable]
	public abstract class BaseMotion : AssetBundleData
	{
		public string state;

		public new bool isEmpty
		{
			get
			{
				return base.isEmpty || state.IsNullOrEmpty();
			}
		}

		public BaseMotion()
		{
		}

		public BaseMotion(string bundle, string asset)
			: base(bundle, asset)
		{
		}

		public BaseMotion(string bundle, string asset, string state)
			: base(bundle, asset)
		{
			this.state = state;
		}

		public bool Check(string bundle, string asset, string state)
		{
			if (!state.IsNullOrEmpty() && this.state != state)
			{
				return true;
			}
			return Check(bundle, asset);
		}
	}
}
