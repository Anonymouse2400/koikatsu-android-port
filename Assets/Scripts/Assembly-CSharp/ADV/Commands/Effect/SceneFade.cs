using Illusion.Extensions;
using Manager;
using UnityEngine;

namespace ADV.Commands.Effect
{
	public class SceneFade : CommandBase
	{
		private SimpleFade fade;

		private string assetBundleName;

		private float bkFadeTime;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "Fade", "Time", "Color", "Bundle", "Asset" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[5]
				{
					"in",
					string.Empty,
					string.Empty,
					string.Empty,
					"none"
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			fade = Singleton<Scene>.Instance.sceneFade;
			fade._Fade = ((!args[num++].Compare("in", true)) ? SimpleFade.Fade.Out : SimpleFade.Fade.In);
			bkFadeTime = fade._Time;
			args.SafeProc(num++, delegate(string s)
			{
				fade._Time = float.Parse(s);
			});
			string self = args[num++];
			if (!self.IsNullOrEmpty())
			{
				fade._Color = self.GetColor();
			}
			string self2 = args[num++];
			if (!self2.IsNullOrEmpty())
			{
				fade._Texture = AssetBundleManager.LoadAsset(self2, args[num++], typeof(Texture2D)).GetAsset<Texture2D>();
			}
			fade.Init();
		}

		public override bool Process()
		{
			base.Process();
			return fade.IsEnd;
		}

		public override void Result(bool processEnd)
		{
			if (fade._Fade == SimpleFade.Fade.Out && !assetBundleName.IsNullOrEmpty())
			{
				AssetBundleManager.UnloadAssetBundle(assetBundleName, true);
			}
			fade._Time = bkFadeTime;
			fade.ForceEnd();
		}
	}
}
