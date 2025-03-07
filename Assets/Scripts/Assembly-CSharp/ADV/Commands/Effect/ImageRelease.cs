using Illusion.Extensions;

namespace ADV.Commands.Effect
{
	public class ImageRelease : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[2] { "Type", "isMain" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[2]
				{
					string.Empty,
					bool.TrueString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string self = args[num++];
			if (!self.IsNullOrEmpty())
			{
				bool isFront = self.Compare("front", true);
				ADVFade advFade = base.scenario.advScene.AdvFade;
				advFade.ReleaseSprite(isFront);
				return;
			}
			EMTransition eMTFade = base.scenario.advScene.EMTFade;
			if (bool.Parse(args[num++]))
			{
				eMTFade.SetTexture(null);
			}
			else
			{
				eMTFade.SetGradationTexture(null);
			}
		}
	}
}
