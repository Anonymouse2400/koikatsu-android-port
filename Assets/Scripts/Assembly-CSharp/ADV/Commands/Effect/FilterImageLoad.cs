using Illusion.Game;

namespace ADV.Commands.Effect
{
	public class FilterImageLoad : CommandBase
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
			Utils.Bundle.LoadSprite(assetBundleName, assetName, base.scenario.FilterImage, false);
			base.scenario.FilterImage.enabled = true;
		}
	}
}
