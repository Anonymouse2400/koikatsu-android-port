using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Config;
using Illusion;
using Illusion.CustomAttributes;
using Illusion.Elements.Xml;
using Localize.Translate;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
	public sealed class Voice : Singleton<Voice>
	{
		public enum Type
		{
			PCM = 0
		}

		private class Initializable : InitializeSolution.IInitializable
		{
			private readonly Voice voice;

			private bool initialized;

			bool InitializeSolution.IInitializable.initialized
			{
				get
				{
					return initialized;
				}
			}

			public Initializable(Voice voice)
			{
				this.voice = voice;
			}

			void InitializeSolution.IInitializable.Initialize()
			{
				if (initialized)
				{
					return;
				}
				initialized = true;
				if (!Localize.Translate.Manager.isTranslate)
				{
					return;
				}
				Dictionary<int, Localize.Translate.Data.Param> category = Localize.Translate.Manager.GetCategory(Localize.Translate.Manager.SCENE_ID.SET_UP, 0);
				foreach (KeyValuePair<int, VoiceInfo.Param> item in voice.voiceInfoDic)
				{
					item.Value.Personality = category.SafeGetText(item.Key) ?? item.Value.Personality;
				}
			}
		}

		private Dictionary<int, VoiceInfo.Param> _voiceInfoDic;

		[NotEditable]
		[SerializeField]
		private Transform rootSetting;

		[SerializeField]
		[NotEditable]
		private Transform rootPlay;

		[NotEditable]
		[SerializeField]
		private Transform ASCacheRoot;

		[NotEditable]
		[SerializeField]
		private GameObject[] settingObjects;

		private Dictionary<int, Transform> voiceDic = new Dictionary<int, Transform>();

		private Dictionary<int, Dictionary<string, AudioSource>> dicASCache;

		private const string UserPath = "config";

		private const string FileName = "voice.xml";

		private const string RootName = "Voice";

		private const string ElementName = "Volume";

		private Control xmlCtrl;

		public static AudioMixer Mixer
		{
			get
			{
				return Sound.Mixer;
			}
		}

		public Dictionary<int, VoiceInfo.Param> voiceInfoDic
		{
			get
			{
				return this.GetCache(ref _voiceInfoDic, () => voiceInfoList.ToDictionary((VoiceInfo.Param v) => v.No, (VoiceInfo.Param v) => v));
			}
		}

		public List<VoiceInfo.Param> voiceInfoList { get; private set; }

		public VoiceSystem _Config { get; private set; }

		protected override void Awake()
		{
			if (!CheckInstance())
			{
				return;
			}
			Localize.Translate.Manager.initializeSolution.Add(new Initializable(this));
			Object.DontDestroyOnLoad(base.gameObject);
			rootSetting = new GameObject("SettingObjectPCM").transform;
			rootSetting.SetParent(base.transform, false);
			rootPlay = new GameObject("PlayObjectPCM").transform;
			rootPlay.SetParent(base.transform, false);
			settingObjects = new GameObject[Utils.Enum<Type>.Length];
			for (int i = 0; i < settingObjects.Length; i++)
			{
				LoadSetting((Type)i);
			}
			dicASCache = new Dictionary<int, Dictionary<string, AudioSource>>();
			ASCacheRoot = new GameObject("AudioSourceCache").transform;
			ASCacheRoot.SetParent(rootPlay, false);
			string text = AssetBundleManager.BaseDownloadingURL + "sound/data/pcm/";
			List<VoiceInfo.Param> sortList = new List<VoiceInfo.Param>();
			HashSet<int> distinctCheck = new HashSet<int>();
			CommonLib.GetAssetBundleNameListFromPath("etcetra/list/config/", true).ForEach(delegate(string file)
			{
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(file, typeof(VoiceInfo));
				foreach (List<VoiceInfo.Param> item in from p in assetBundleLoadAssetOperation.GetAllAssets<VoiceInfo>()
					select p.param)
				{
					foreach (VoiceInfo.Param p2 in item.Where((VoiceInfo.Param p) => !distinctCheck.Add(p.No)))
					{
						sortList.Remove(sortList.FirstOrDefault((VoiceInfo.Param l) => l.No == p2.No));
					}
					sortList.AddRange(item);
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			sortList.Sort((VoiceInfo.Param a, VoiceInfo.Param b) => a.Sort.CompareTo(b.Sort));
			voiceInfoList = sortList;
			Dictionary<int, string> dic = new Dictionary<int, string>();
			voiceInfoList.ForEach(delegate(VoiceInfo.Param p)
			{
				dic.Add(p.No, p.FileName);
				Transform transform = new GameObject(p.FileName).transform;
				transform.SetParent(rootPlay, false);
				voiceDic.Add(p.No, transform);
			});
			_Config = new VoiceSystem("Volume", dic);
			xmlCtrl = new Control("config", "voice.xml", "Voice", _Config);
			Load();
		}

		private void OnDestroy()
		{
			Object.Destroy(rootSetting.gameObject);
			Object.Destroy(rootPlay.gameObject);
			voiceDic.Clear();
		}

		[Conditional("__DEBUG_PROC__")]
		private void FileLog(string filePath)
		{
			if (!Directory.Exists(filePath))
			{
				return;
			}
			string[] source = Directory.GetDirectories(filePath).Select(Path.GetFileName).ToArray();
			if (source.Any())
			{
			}
			string[] source2 = source.Where((string file) => voiceInfoList.Find((VoiceInfo.Param p) => p.FileName == file) == null).ToArray();
			if (!source2.Any())
			{
			}
		}

		private void LoadSetting(Type type, int settingNo = -1)
		{
			string text = type.ToString();
			AssetBundleData assetBundleData = new AssetBundleData("sound/setting/object/00.unity3d", text.ToLower());
			GameObject gameObject = Object.Instantiate(assetBundleData.GetAsset<GameObject>(), rootSetting, false);
			gameObject.name = text + "_Setting";
			AudioSource component = gameObject.GetComponent<AudioSource>();
			Singleton<Sound>.Instance.AudioSettingData(component, settingNo);
			settingObjects[(int)type] = gameObject;
			assetBundleData.UnloadBundle(true);
		}

		public void SetParent(int no, Transform t)
		{
			Transform value;
			if (voiceDic.TryGetValue(no, out value))
			{
				t.SetParent(value, false);
			}
		}

		public void Bind(LoadVoice script)
		{
			if (script.audioSource == null)
			{
				Transform value;
				if (!voiceDic.TryGetValue(script.no, out value))
				{
					return;
				}
				Singleton<Sound>.Instance.SetParent(value, script, settingObjects[(int)script.type]);
			}
			AudioSource audioSource = script.audioSource;
			audioSource.clip = script.clip;
			Singleton<Sound>.Instance.Register(script.clip);
			audioSource.name = audioSource.clip.name;
			audioSource.volume = GetVolume(script.no);
			Sound.OutputSettingData outputSettingData = Singleton<Sound>.Instance.AudioSettingData(audioSource, script.settingNo);
			if (outputSettingData != null)
			{
				script.delayTime = outputSettingData.delayTime;
			}
		}

		public List<AudioSource> GetPlayingList(int no)
		{
			List<AudioSource> list = new List<AudioSource>();
			Transform value;
			if (!voiceDic.TryGetValue(no, out value))
			{
				return list;
			}
			for (int i = 0; i < value.childCount; i++)
			{
				list.Add(value.GetChild(i).GetComponent<AudioSource>());
			}
			return list;
		}

		public bool IsVoiceCheck(int no)
		{
			Transform value;
			if (!voiceDic.TryGetValue(no, out value))
			{
				return false;
			}
			return value.childCount != 0;
		}

		public bool IsVoiceCheck(Transform voiceTrans, bool isLoopCheck = true)
		{
			foreach (Transform value in voiceDic.Values)
			{
				for (int i = 0; i < value.childCount; i++)
				{
					LoadVoice componentInChildren = value.GetChild(i).GetComponentInChildren<LoadVoice>();
					if (!(componentInChildren == null) && componentInChildren.voiceTrans == voiceTrans && (isLoopCheck || !(componentInChildren.audioSource != null) || !componentInChildren.audioSource.loop))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsVoiceCheck(int no, Transform voiceTrans, bool isLoopCheck = true)
		{
			Transform value;
			if (!voiceDic.TryGetValue(no, out value))
			{
				return false;
			}
			for (int i = 0; i < value.childCount; i++)
			{
				LoadVoice componentInChildren = value.GetChild(i).GetComponentInChildren<LoadVoice>();
				if (!(componentInChildren == null) && componentInChildren.voiceTrans == voiceTrans && (isLoopCheck || !(componentInChildren.audioSource != null) || !componentInChildren.audioSource.loop))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsVoiceCheck()
		{
			return voiceDic.Values.Any((Transform v) => v.childCount != 0);
		}

		public Transform Play(int no, string assetBundleName, string assetName, float pitch = 1f, float delayTime = 0f, float fadeTime = 0f, bool isAsync = true, Transform voiceTrans = null, Type type = Type.PCM, int settingNo = -1, bool isPlayEndDelete = true, bool isBundleUnload = true, bool is2D = false)
		{
			LoadVoice loadVoice = new GameObject("Voice Loading").AddComponent<LoadVoice>();
			loadVoice.no = no;
			loadVoice.assetBundleName = assetBundleName;
			loadVoice.assetName = assetName;
			loadVoice.pitch = pitch;
			loadVoice.delayTime = delayTime;
			loadVoice.fadeTime = fadeTime;
			loadVoice.isAsync = isAsync;
			loadVoice.voiceTrans = voiceTrans;
			loadVoice.type = type;
			loadVoice.settingNo = settingNo;
			loadVoice.isPlayEndDelete = isPlayEndDelete;
			loadVoice.isBundleUnload = isBundleUnload;
			loadVoice.is2D = is2D;
			Transform value;
			if (!voiceDic.TryGetValue(no, out value))
			{
				return null;
			}
			return Singleton<Sound>.Instance.SetParent(value, loadVoice, settingObjects[(int)type]);
		}

		public Transform OnecePlay(int no, string assetBundleName, string assetName, float pitch = 1f, float delayTime = 0f, float fadeTime = 0f, bool isAsync = true, Transform voiceTrans = null, Type type = Type.PCM, int settingNo = -1, bool isPlayEndDelete = true, bool isBundleUnload = true, bool is2D = false)
		{
			StopAll();
			return Play(no, assetBundleName, assetName, pitch, delayTime, fadeTime, isAsync, voiceTrans, type, settingNo, isPlayEndDelete, isBundleUnload, is2D);
		}

		public Transform OnecePlayChara(int no, string assetBundleName, string assetName, float pitch = 1f, float delayTime = 0f, float fadeTime = 0f, bool isAsync = true, Transform voiceTrans = null, Type type = Type.PCM, int settingNo = -1, bool isPlayEndDelete = true, bool isBundleUnload = true, bool is2D = false)
		{
			if (voiceTrans != null)
			{
				Stop(no, voiceTrans);
			}
			else
			{
				Stop(no);
			}
			return Play(no, assetBundleName, assetName, pitch, delayTime, fadeTime, isAsync, voiceTrans, type, settingNo, isPlayEndDelete, isBundleUnload, is2D);
		}

		public void StopAll(bool isLoopStop = true)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform value in voiceDic.Values)
			{
				for (int i = 0; i < value.childCount; i++)
				{
					Transform child = value.GetChild(i);
					if (!isLoopStop)
					{
						AudioSource componentInChildren = child.GetComponentInChildren<AudioSource>();
						if (componentInChildren != null && componentInChildren.loop)
						{
							continue;
						}
					}
					list.Add(child.gameObject);
				}
			}
			list.ForEach(delegate(GameObject p)
			{
				Object.Destroy(p);
			});
		}

		public void Stop(int no)
		{
			Transform value;
			if (voiceDic.TryGetValue(no, out value))
			{
				List<GameObject> list = new List<GameObject>();
				for (int i = 0; i < value.childCount; i++)
				{
					list.Add(value.GetChild(i).gameObject);
				}
				list.ForEach(delegate(GameObject p)
				{
					Object.Destroy(p);
				});
			}
		}

		public void Stop(Transform voiceTrans)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (Transform value in voiceDic.Values)
			{
				for (int i = 0; i < value.childCount; i++)
				{
					Transform child = value.GetChild(i);
					LoadVoice componentInChildren = child.GetComponentInChildren<LoadVoice>();
					if (!(componentInChildren == null) && componentInChildren.voiceTrans == voiceTrans)
					{
						list.Add(child.gameObject);
					}
				}
			}
			list.ForEach(delegate(GameObject p)
			{
				Object.Destroy(p);
			});
		}

		public void Stop(int no, Transform voiceTrans)
		{
			Transform value;
			if (!voiceDic.TryGetValue(no, out value))
			{
				return;
			}
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < value.childCount; i++)
			{
				Transform child = value.GetChild(i);
				LoadVoice componentInChildren = child.GetComponentInChildren<LoadVoice>();
				if (!(componentInChildren == null) && componentInChildren.voiceTrans == voiceTrans)
				{
					list.Add(child.gameObject);
				}
			}
			list.ForEach(Object.Destroy);
		}

		public float GetVolume(int charaNo)
		{
			VoiceSystem.Voice value;
			if (!_Config.chara.TryGetValue(charaNo, out value))
			{
				return 0f;
			}
			return value.sound.GetVolume();
		}

		public AudioSource CreateCache(int voiceNo, AssetBundleData data)
		{
			return CreateCache(voiceNo, data.bundle, data.asset);
		}

		public AudioSource CreateCache(int voiceNo, AssetBundleManifestData data)
		{
			return CreateCache(voiceNo, data.bundle, data.asset, data.manifest);
		}

		public AudioSource CreateCache(int voiceNo, string bundle, string asset, string manifest = null)
		{
			Dictionary<string, AudioSource> value;
			if (!dicASCache.TryGetValue(voiceNo, out value))
			{
				Dictionary<string, AudioSource> dictionary = new Dictionary<string, AudioSource>();
				dicASCache[voiceNo] = dictionary;
				value = dictionary;
			}
			AudioSource value2;
			if (!value.TryGetValue(asset, out value2))
			{
				GameObject gameObject = Object.Instantiate(settingObjects[0], ASCacheRoot, false);
				gameObject.name = asset;
				gameObject.SetActive(true);
				value2 = gameObject.GetComponent<AudioSource>();
				value2.clip = AssetBundleManager.LoadAsset(bundle, asset, typeof(AudioClip), manifest).GetAsset<AudioClip>();
				Singleton<Sound>.Instance.Register(value2.clip);
				value.Add(asset, value2);
			}
			return value2;
		}

		public void ReleaseCache(int voiceNo, string bundle, string asset, string manifest = null)
		{
			Dictionary<string, AudioSource> value;
			if (dicASCache.TryGetValue(voiceNo, out value))
			{
				AudioSource value2;
				if (value.TryGetValue(asset, out value2))
				{
					Singleton<Sound>.Instance.Remove(value2.clip);
					Object.Destroy(value2.gameObject);
					value.Remove(asset);
					AssetBundleManager.UnloadAssetBundle(bundle, false, manifest);
				}
				if (!value.Any())
				{
					dicASCache.Remove(voiceNo);
				}
			}
		}

		public void Reset()
		{
			if (xmlCtrl != null)
			{
				xmlCtrl.Init();
			}
		}

		public void Load()
		{
			xmlCtrl.Read();
		}

		public void Save()
		{
			xmlCtrl.Write();
		}
	}
}
