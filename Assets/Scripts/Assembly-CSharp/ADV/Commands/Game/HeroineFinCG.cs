using Illusion.Game;
using Manager;

namespace ADV.Commands.Game
{
	public class HeroineFinCG : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[3] { "isActive", "Bundle", "Asset" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[3]
				{
					bool.FalseString,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			bool flag = bool.Parse(args[num++]);
			base.scenario.FilterImage.enabled = flag;
			base.scenario.FilterImage.sprite = null;
			if (flag)
			{
				base.scenario.currentHeroine.isTaked = true;
				Singleton<Manager.Game>.Instance.glSaveData.fixCharaTaked.Add(base.scenario.currentHeroine.fixCharaID);
				string assetBundleName = args[num++];
				string assetName = args[num++];
				Utils.Bundle.LoadSprite(assetBundleName, assetName, base.scenario.FilterImage, false);
			}
		}
	}
}
