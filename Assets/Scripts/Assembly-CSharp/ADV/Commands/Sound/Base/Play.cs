using Illusion.Game;
using Manager;
using UnityEngine;

namespace ADV.Commands.Sound.Base
{
	public abstract class Play : CommandBase
	{
		private Manager.Sound.Type type;

		private float delayTime;

		private bool isWait;

		private bool isStop;

		private Transform transform;

		private float timer;

		private Vector3? move;

		private float? stopTime;

		public override string[] ArgsLabel
		{
			get
			{
				return new string[18]
				{
					"Bundle", "Asset", "Delay", "Fade", "isName", "isAsync", "SettingNo", "isWait", "isStop", "isLoop",
					"Pitch", "PanStereo", "SpatialBlend", "Time", "Volume", "Pos", "Move", "Stop"
				};
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[18]
				{
					string.Empty,
					string.Empty,
					"0",
					"0",
					bool.TrueString,
					bool.TrueString,
					"-1",
					bool.FalseString,
					bool.FalseString,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty
				};
			}
		}

		public Play(Manager.Sound.Type type)
		{
			this.type = type;
		}

		public override void Do()
		{
			base.Do();
			Utils.Sound.Setting setting = new Utils.Sound.Setting(type);
			int num = 0;
			setting.assetBundleName = args[num++];
			setting.assetName = args[num++];
			delayTime = (setting.delayTime = float.Parse(args[num++]));
			setting.fadeTime = float.Parse(args[num++]);
			setting.isAssetEqualPlay = bool.Parse(args[num++]);
			setting.isAsync = bool.Parse(args[num++]);
			setting.settingNo = int.Parse(args[num++]);
			isWait = bool.Parse(args[num++]);
			isStop = bool.Parse(args[num++]);
			transform = Utils.Sound.Play(setting);
			AudioSource audioSource = transform.GetComponent<AudioSource>();
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.loop = bool.Parse(s);
			});
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
				audioSource.spatialBlend = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.time = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				audioSource.volume = float.Parse(s);
			});
			args.SafeProc(num++, delegate(string s)
			{
				Vector3 value;
				if (!base.scenario.commandController.V3Dic.TryGetValue(s, out value))
				{
					int cnt = 0;
					CommandBase.CountAddV3(s.Split(','), ref cnt, ref value);
				}
				transform.position = value;
			});
			args.SafeProc(num++, delegate(string s)
			{
				Vector3 value2;
				if (!base.scenario.commandController.V3Dic.TryGetValue(s, out value2))
				{
					int cnt2 = 0;
					CommandBase.CountAddV3(s.Split(','), ref cnt2, ref value2);
				}
				move = value2;
			});
			args.SafeProc(num++, delegate(string s)
			{
				stopTime = float.Parse(s);
			});
		}

		public override bool Process()
		{
			base.Process();
			if (!isWait)
			{
				return true;
			}
			if (timer >= delayTime)
			{
				if (!Singleton<Manager.Sound>.Instance.IsPlay(type, transform))
				{
					return true;
				}
			}
			else
			{
				timer += Time.deltaTime;
			}
			if (move.HasValue)
			{
				transform.Translate(move.Value * Time.deltaTime);
			}
			if (stopTime.HasValue && timer >= stopTime.Value)
			{
				return true;
			}
			return false;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (isStop)
			{
				Singleton<Manager.Sound>.Instance.Stop(transform);
			}
		}
	}
}
