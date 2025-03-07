using Illusion;
using Illusion.Game;
using UnityEngine;

namespace ADV.Commands.Sound.BGM
{
	public class Play : CommandBase
	{
		public override string[] ArgsLabel
		{
			get
			{
				return new string[9] { "Bundle", "Asset", "Delay", "Fade", "isAsync", "Pitch", "PanStereo", "Time", "Volume" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[9]
				{
					string.Empty,
					string.Empty,
					"0",
					string.Empty,
					bool.TrueString,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			string text = args[num++];
			Illusion.Game.Utils.Sound.SettingBGM setting;
			int result;
			if (int.TryParse(text, out result))
			{
				setting = new Illusion.Game.Utils.Sound.SettingBGM(result);
			}
			else
			{
				result = Illusion.Utils.Enum<Illusion.Game.BGM>.FindIndex(text, true);
				if (result != -1)
				{
					setting = new Illusion.Game.Utils.Sound.SettingBGM(result);
				}
				else
				{
					setting = new Illusion.Game.Utils.Sound.SettingBGM(text);
				}
			}
			args.SafeProc(num++, delegate(string s)
			{
				setting.assetName = s;
			});
			setting.delayTime = float.Parse(args[num++]);
			args.SafeProc(num++, delegate(string s)
			{
				setting.fadeTime = float.Parse(s);
			});
			setting.isAsync = bool.Parse(args[num++]);
			AudioSource audioSource = Illusion.Game.Utils.Sound.Play(setting).GetComponent<AudioSource>();
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.pitch = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.panStereo = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.time = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.volume = float.Parse(s);
			});
		}
	}
}
