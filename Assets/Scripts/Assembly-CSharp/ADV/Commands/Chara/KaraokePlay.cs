using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.Game;
using Manager;
using Sound;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ADV.Commands.Chara
{
	public class KaraokePlay : CommandBase
	{
		private abstract class Setting
		{
			public string bundle;

			public string asset;

			public float fadeTime = 0.8f;

			public int settingNo = -1;

			public AudioSource source;
		}

		private class AudioSetting : Setting
		{
			public float time;

			public float panStereo;

			public AudioPlayer Load()
			{
				AssetBundleData assetBundleData = new AssetBundleData(bundle, asset);
				if (!assetBundleData.isFile)
				{
					return new AudioPlayer(this, null);
				}
				source = Utils.Sound.Get(Manager.Sound.Type.BGM, assetBundleData);
				AudioSource audioSource = UnityEngine.Object.Instantiate(source, source.transform.parent);
				audioSource.name = source.name;
				audioSource.panStereo = panStereo;
				Singleton<Manager.Sound>.Instance.AudioSettingData(audioSource, settingNo);
				return new AudioPlayer(this, audioSource);
			}
		}

		private class VoiceSetting : Setting
		{
			public int no;

			public bool is2D;

			public Transform voiceTrans;

			public VoicePlayer Load(ChaControl chaCtrl, TextScenario scenario)
			{
				AssetBundleData assetBundleData = new AssetBundleData(bundle, asset);
				if (!assetBundleData.isFile)
				{
					return new VoicePlayer(this, null);
				}
				source = Utils.Voice.Get(no, assetBundleData);
				AudioSource audioSource = UnityEngine.Object.Instantiate(source, source.transform.parent);
				if (settingNo < 0)
				{
					if (!is2D)
					{
						audioSource.spatialBlend = ((voiceTrans != null) ? 1 : 0);
					}
					else
					{
						audioSource.spatialBlend = 0f;
					}
				}
				audioSource.name = source.name;
				audioSource.volume = Singleton<Voice>.Instance.GetVolume(no);
				Singleton<Manager.Sound>.Instance.AudioSettingData3DOnly(audioSource, 1);
				if (chaCtrl != null)
				{
					chaCtrl.SetVoiceTransform((!scenario.info.audio.isNotMoveMouth) ? audioSource.transform : null);
				}
				return new VoicePlayer(this, audioSource);
			}
		}

		private abstract class Player : IDisposable
		{
			protected AudioSource source;

			protected Setting setting;

			protected bool isPlayed;

			protected bool isDisposed;

			public bool isSuccess
			{
				get
				{
					return source != null && source.clip != null;
				}
			}

			public bool isOK
			{
				get
				{
					if (source.clip.loadState != AudioDataLoadState.Loaded)
					{
						return false;
					}
					if (!source.isActiveAndEnabled)
					{
						return false;
					}
					return true;
				}
			}

			public Player(Setting setting, AudioSource source)
			{
				this.setting = setting;
				this.source = source;
			}

			public abstract void Play();

			public abstract void Dispose();
		}

		private class AudioPlayer : Player
		{
			private bool isReleased;

			public AudioSetting audioSetting
			{
				get
				{
					return setting as AudioSetting;
				}
			}

			public AudioPlayer(AudioSetting setting, AudioSource source)
				: base(setting, source)
			{
			}

			public override void Play()
			{
				if (base.isSuccess)
				{
					Singleton<Manager.Sound>.Instance.SetParent(Manager.Sound.Type.BGM, source.transform);
					GameObject currentBGM = Singleton<Manager.Sound>.Instance.currentBGM;
					Singleton<Manager.Sound>.Instance.currentBGM = source.gameObject;
					source.time = audioSetting.time;
					Manager.Sound.PlayFade(currentBGM, source, setting.fadeTime);
					isPlayed = true;
					source.OnDestroyAsObservable().Subscribe(delegate
					{
						Dispose();
					});
				}
			}

			public override void Dispose()
			{
				if (isDisposed)
				{
					return;
				}
				isDisposed = true;
				if (source == null)
				{
					Release();
					return;
				}
				FadePlayer component = source.GetComponent<FadePlayer>();
				if (component == null)
				{
					UnityEngine.Object.Destroy(source.gameObject);
					Release();
					return;
				}
				component.Stop(setting.fadeTime);
				component.OnDestroyAsObservable().Subscribe(delegate
				{
					Release();
				});
			}

			private void Release()
			{
				if (!isReleased)
				{
					isReleased = true;
					Utils.Sound.Remove(Manager.Sound.Type.BGM, setting.bundle, setting.asset);
				}
			}
		}

		private class VoicePlayer : Player, TextScenario.IVoice
		{
			private bool isReleased;

			private bool isReleaseForce;

			int TextScenario.IVoice.personality
			{
				get
				{
					return vs.no;
				}
			}

			string TextScenario.IVoice.bundle
			{
				get
				{
					return vs.bundle;
				}
			}

			string TextScenario.IVoice.asset
			{
				get
				{
					return vs.asset;
				}
			}

			public VoiceSetting vs
			{
				get
				{
					return setting as VoiceSetting;
				}
			}

			public VoicePlayer(VoiceSetting vs, AudioSource source)
				: base(vs, source)
			{
				if (!base.isSuccess)
				{
					return;
				}
				source.UpdateAsObservable().Subscribe(delegate
				{
					source.volume = Singleton<Voice>.Instance.GetVolume(vs.no);
				});
				Transform vt = source.transform;
				Transform voiceTrans = vs.voiceTrans;
				if (voiceTrans != null)
				{
					source.UpdateAsObservable().TakeUntilDestroy(voiceTrans).Subscribe(delegate
					{
						vt.SetPositionAndRotation(voiceTrans.position, voiceTrans.rotation);
					});
				}
			}

			public override void Play()
			{
				if (!base.isSuccess)
				{
					return;
				}
				Manager.Sound.PlayFade(null, source, setting.fadeTime);
				if (!isPlayed)
				{
					Singleton<Voice>.Instance.SetParent(vs.no, source.transform);
					(from _ in source.UpdateAsObservable()
						select source).SkipWhile((AudioSource audio) => audio.isPlaying).Take(1).Subscribe(delegate(AudioSource audio)
					{
						UnityEngine.Object.Destroy(audio.gameObject);
					});
					source.OnDestroyAsObservable().Subscribe(delegate
					{
						Dispose();
					});
				}
				isPlayed = true;
			}

			public override void Dispose()
			{
				if (isDisposed)
				{
					return;
				}
				isDisposed = true;
				if (source == null)
				{
					Release();
					return;
				}
				FadePlayer fadePlayer = null;
				if (isReleaseForce || (fadePlayer = source.GetComponent<FadePlayer>()) == null)
				{
					UnityEngine.Object.Destroy(source.gameObject);
					Release();
					return;
				}
				fadePlayer.Stop(setting.fadeTime);
				fadePlayer.OnDestroyAsObservable().Subscribe(delegate
				{
					Release();
				});
			}

			private void Release()
			{
				if (!isReleased)
				{
					isReleased = true;
					Utils.Voice.Remove(vs.no, setting.bundle, setting.asset);
				}
			}

			void TextScenario.IVoice.Convert2D()
			{
			}

			bool TextScenario.IVoice.Wait()
			{
				if (!base.isSuccess)
				{
					return false;
				}
				if (!isPlayed)
				{
					return true;
				}
				return source.isPlaying;
			}

			AudioSource TextScenario.IVoice.Play()
			{
				if (!isPlayed)
				{
					return source;
				}
				isReleaseForce = true;
				Dispose();
				VoiceSetting voiceSetting = vs;
				Utils.Voice.Setting setting = new Utils.Voice.Setting();
				setting.no = voiceSetting.no;
				setting.assetBundleName = base.setting.bundle;
				setting.assetName = base.setting.asset;
				setting.voiceTrans = null;
				setting.isAsync = false;
				setting.pitch = 1f;
				setting.is2D = false;
				Utils.Voice.Setting s = setting;
				return Utils.Voice.Play(s).GetComponent<AudioSource>();
			}
		}

		private List<Player> playList = new List<Player>();

		public override string[] ArgsLabel
		{
			get
			{
				return new string[10] { "No", "BundleBG", "AssetBG", "BundleVoice", "AssetVoice", "Fade", "Time", "PanStereo", "VoiceNo", "is2D" };
			}
		}

		public override string[] ArgsDefault
		{
			get
			{
				return new string[10]
				{
					int.MaxValue.ToString(),
					string.Empty,
					string.Empty,
					string.Empty,
					string.Empty,
					"0.8",
					"0",
					"0",
					string.Empty,
					bool.FalseString
				};
			}
		}

		public override void Do()
		{
			base.Do();
			int num = 0;
			int no = int.Parse(args[num++]);
			string bundle = args[num++];
			string asset = args[num++];
			string bundle2 = args[num++];
			string asset2 = args[num++];
			float fadeTime = float.Parse(args[num++]);
			float time = float.Parse(args[num++]);
			float panStereo = float.Parse(args[num++]);
			int voiceNo = 0;
			bool flag = args.SafeProc(num++, delegate(string s)
			{
				voiceNo = int.Parse(s);
			});
			bool is2D = bool.Parse(args[num++]);
			AudioSetting audioSetting = new AudioSetting();
			audioSetting.bundle = bundle;
			audioSetting.asset = asset;
			audioSetting.fadeTime = fadeTime;
			audioSetting.panStereo = panStereo;
			audioSetting.time = time;
			AudioSetting audioSetting2 = audioSetting;
			VoiceSetting voiceSetting = new VoiceSetting();
			voiceSetting.bundle = bundle2;
			voiceSetting.asset = asset2;
			voiceSetting.fadeTime = fadeTime;
			voiceSetting.no = voiceNo;
			voiceSetting.is2D = is2D;
			VoiceSetting voiceSetting2 = voiceSetting;
			CharaData chara = base.scenario.commandController.GetChara(no);
			if (chara != null)
			{
				if (!flag)
				{
					voiceSetting2.no = chara.voiceNo;
				}
				voiceSetting2.voiceTrans = chara.voiceTrans;
			}
			AudioPlayer item = audioSetting2.Load();
			VoicePlayer voicePlayer = voiceSetting2.Load(chara.chaCtrl, base.scenario);
			playList.Add(item);
			playList.Add(voicePlayer);
			base.scenario.karaokeList.ForEach(delegate(IDisposable p)
			{
				p.Dispose();
			});
			base.scenario.karaokeList.Clear();
			base.scenario.karaokeList.Add(voicePlayer);
			base.scenario.currentCharaData.karaokePlayer = voicePlayer;
		}

		public override bool Process()
		{
			base.Process();
			if (playList.Any((Player p) => !p.isSuccess))
			{
				return true;
			}
			if (!playList.All((Player p) => p.isOK))
			{
				return false;
			}
			playList.ForEach(delegate(Player p)
			{
				p.Play();
			});
			return true;
		}

		public override void Result(bool processEnd)
		{
			base.Result(processEnd);
			if (processEnd)
			{
			}
		}
	}
}
