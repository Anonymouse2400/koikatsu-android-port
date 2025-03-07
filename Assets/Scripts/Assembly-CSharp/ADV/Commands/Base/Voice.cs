using System;
using System.Collections.Generic;
using Illusion.Extensions;
using Illusion.Game;
using Manager;
using UnityEngine;

namespace ADV.Commands.Base
{
	public class Voice : CommandBase
	{
		public class Data : TextScenario.IVoice
		{
			public int no { get; private set; }

			public string bundle { get; set; }

			public string asset { get; private set; }

			public int personality { get; set; }

			public float pitch { get; set; }

			public ChaControl chaCtrl { get; set; }

			public Transform transform { get; set; }

			public Info.Audio.Eco eco { get; set; }

			public bool is2D { get; set; }

			public bool isNotMoveMouth { get; set; }

			public bool usePersonality { get; private set; }

			public bool usePitch { get; private set; }

			public AudioSource audio { get; private set; }

			public Data(string[] args, ref int cnt)
			{
				pitch = 1f;
				try
				{
					no = int.Parse(args[cnt++]);
					bundle = args.SafeGet(cnt++);
					asset = args.SafeGet(cnt++);
					usePersonality = args.SafeProc(cnt++, delegate(string s)
					{
						personality = int.Parse(s);
					});
					usePitch = args.SafeProc(cnt++, delegate(string s)
					{
						pitch = float.Parse(s);
					});
				}
				catch (Exception)
				{
				}
			}

			public void Convert2D()
			{
				transform = null;
			}

			public AudioSource Play()
			{
				Utils.Voice.Setting setting = new Utils.Voice.Setting();
				setting.no = personality;
				setting.assetBundleName = bundle;
				setting.assetName = asset;
				setting.voiceTrans = this.transform;
				setting.isAsync = false;
				setting.pitch = pitch;
				setting.is2D = is2D;
				Utils.Voice.Setting s = setting;
				Transform transform = Utils.Voice.Play(s);
				if (eco != null)
				{
					AudioEchoFilter audioEchoFilter = transform.gameObject.AddComponent<AudioEchoFilter>();
					audioEchoFilter.delay = eco.delay;
					audioEchoFilter.decayRatio = eco.decayRatio;
					audioEchoFilter.wetMix = eco.wetMix;
					audioEchoFilter.dryMix = eco.dryMix;
				}
				if (chaCtrl != null && this.transform != null)
				{
					chaCtrl.SetVoiceTransform((!isNotMoveMouth) ? transform : null);
				}
				audio = transform.GetComponent<AudioSource>();
				if (audio != null && this.transform != null)
				{
					Singleton<Manager.Sound>.Instance.AudioSettingData3DOnly(audio, 1);
				}
				return audio;
			}

			public bool Wait()
			{
				return Singleton<Manager.Voice>.Instance.IsVoiceCheck(personality, transform, false);
			}
		}

		public override string[] ArgsLabel
		{
			get
			{
				return new string[5] { "No", "Bundle", "Asset", "Personality", "Pitch" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[1] { int.MaxValue.ToString() };
			}
		}

		public override void Do()
		{
			base.Do();
			TextScenario.CurrentCharaData currentCharaData = base.scenario.currentCharaData;
			currentCharaData.CreateVoiceList();
			List<Data> list = new List<Data>();
			if (args.Length > 1)
			{
				int cnt = 0;
				while (!args.IsNullOrEmpty(cnt))
				{
					Data data = new Data(args, ref cnt);
					CharaData chara = base.scenario.commandController.GetChara(data.no);
					if (chara != null)
					{
						data.transform = chara.voiceTrans;
						data.chaCtrl = chara.chaCtrl;
						if (!data.usePersonality)
						{
							data.personality = chara.voiceNo;
						}
						if (!data.usePitch)
						{
							data.pitch = chara.voicePitch;
						}
					}
					data.is2D = base.scenario.info.audio.is2D;
					data.isNotMoveMouth = base.scenario.info.audio.isNotMoveMouth;
					if (base.scenario.info.audio.eco.use)
					{
						data.eco = base.scenario.info.audio.eco.DeepCopy();
					}
					list.Add(data);
				}
			}
			currentCharaData.voiceList.Add(list.ToArray());
			foreach (Data item in list)
			{
				if (item.bundle.IsNullOrEmpty())
				{
					string value;
					if (base.scenario.currentCharaData.bundleVoices.TryGetValue(item.personality, out value))
					{
						item.bundle = value;
					}
				}
				else
				{
					base.scenario.currentCharaData.bundleVoices[item.personality] = item.bundle;
				}
			}
		}
	}
}
