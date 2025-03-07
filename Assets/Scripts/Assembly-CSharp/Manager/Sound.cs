using System.Collections.Generic;
using System.Linq;
using Illusion;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Sound;
using UnityEngine;
using UnityEngine.Audio;

namespace Manager
{
	public sealed class Sound : Singleton<Sound>
	{
		public enum Type
		{
			BGM = 0,
			ENV = 1,
			SystemSE = 2,
			GameSE2D = 3,
			GameSE3D = 4
		}

		public class OutputSettingData
		{
			public float delayTime;
		}

		[NotEditable]
		[SerializeField]
		private Transform _listener;

		[NotEditable]
		[SerializeField]
		private GameObject _currentBGM;

		[SerializeField]
		[NotEditable]
		private GameObject oldBGM;

		[SerializeField]
		[NotEditable]
		private Transform listener;

		[SerializeField]
		[NotEditable]
		private Transform rootSetting;

		[NotEditable]
		[SerializeField]
		private Transform rootPlay;

		[NotEditable]
		[SerializeField]
		private Transform ASCacheRoot;

		[SerializeField]
		[NotEditable]
		private Transform[] typeObjects;

		[NotEditable]
		[SerializeField]
		private GameObject[] settingObjects;

		private Dictionary<int, Dictionary<string, AudioSource>> dicASCache;

		[SerializeField]
		[NotEditable]
		private List<AudioClip> useAudioClipList = new List<AudioClip>();

		public static AudioMixer Mixer { get; private set; }

		public AudioListener AudioListener
		{
			get
			{
				return listener.GetComponent<AudioListener>();
			}
		}

		public Transform Listener
		{
			get
			{
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		public GameObject currentBGM
		{
			get
			{
				return _currentBGM;
			}
			set
			{
				if (oldBGM != null)
				{
					Object.Destroy(oldBGM);
				}
				oldBGM = _currentBGM;
				_currentBGM = value;
			}
		}

		public List<SoundSettingData.Param> settingDataList { get; private set; }

		public List<Sound3DSettingData.Param> setting3DDataList { get; private set; }

		public static GameObject PlayFade(GameObject fadeOut, AudioSource audio, float fadeTime = 0f)
		{
			if (fadeOut != null)
			{
				fadeOut.GetComponent<FadePlayer>().SafeProc(delegate(FadePlayer p)
				{
					p.Stop(fadeTime);
				});
			}
			GameObject gameObject = audio.gameObject;
			gameObject.AddComponent<FadePlayer>().Play(fadeTime);
			return gameObject;
		}

		public void Register(AudioClip clip)
		{
			useAudioClipList.Add(clip);
		}

		public void Remove(AudioClip clip)
		{
			if (!useAudioClipList.Remove(clip) || useAudioClipList.Count((AudioClip p) => p == clip) == 0)
			{
				Resources.UnloadAsset(clip);
			}
		}

		public OutputSettingData AudioSettingData(AudioSource audio, int settingNo)
		{
			if (settingNo < 0)
			{
				return null;
			}
			SoundSettingData.Param audioSettingData = GetAudioSettingData(settingNo);
			if (audioSettingData == null)
			{
				return null;
			}
			audio.volume = audioSettingData.Volume;
			audio.pitch = audioSettingData.Pitch;
			audio.panStereo = audioSettingData.Pan;
			audio.spatialBlend = audioSettingData.Level3D;
			audio.priority = audioSettingData.Priority;
			audio.playOnAwake = audioSettingData.PlayAwake;
			audio.loop = audioSettingData.Loop;
			AudioSettingData3DOnly(audio, audioSettingData);
			OutputSettingData outputSettingData = new OutputSettingData();
			outputSettingData.delayTime = audioSettingData.DelayTime;
			return outputSettingData;
		}

		public SoundSettingData.Param GetAudioSettingData(int settingNo)
		{
			return (settingNo >= 0) ? settingDataList[settingNo] : null;
		}

		public void AudioSettingData3DOnly(AudioSource audio, int settingNo)
		{
			AudioSettingData3DOnly(audio, GetAudioSettingData(settingNo));
		}

		private void AudioSettingData3DOnly(AudioSource audio, SoundSettingData.Param param)
		{
			if (param != null && param.Setting3DNo >= 0)
			{
				Sound3DSettingData.Param param2 = setting3DDataList[param.Setting3DNo];
				if (param2 != null)
				{
					audio.dopplerLevel = param2.DopplerLevel;
					audio.spread = param2.Spread;
					audio.minDistance = param2.MinDistance;
					audio.maxDistance = param2.MaxDistance;
					audio.rolloffMode = (AudioRolloffMode)param2.AudioRolloffMode;
				}
			}
		}

		public AudioSource Create(Type type, bool isCache = false)
		{
			GameObject gameObject = Object.Instantiate(settingObjects[(int)type], (!isCache) ? typeObjects[(int)type] : ASCacheRoot, false);
			gameObject.SetActive(true);
			return gameObject.GetComponent<AudioSource>();
		}

		public void SetParent(Type type, Transform t)
		{
			t.SetParent(typeObjects[(int)type], false);
		}

		public Transform SetParent(Transform parent, LoadAudioBase script, GameObject settingObject)
		{
			GameObject gameObject = Object.Instantiate(settingObject, parent, false);
			gameObject.SetActive(true);
			script.Init(gameObject.GetComponent<AudioSource>());
			return gameObject.transform;
		}

		public void Bind(LoadSound script)
		{
			if (script.audioSource == null)
			{
				int type = (int)script.type;
				SetParent(typeObjects[type], script, settingObjects[type]);
			}
			AudioSource audioSource = script.audioSource;
			audioSource.clip = script.clip;
			Register(script.clip);
			audioSource.name = script.clip.name;
			OutputSettingData outputSettingData = AudioSettingData(audioSource, script.settingNo);
			if (outputSettingData != null && script.delayTime <= 0f)
			{
				script.delayTime = outputSettingData.delayTime;
			}
		}

		public List<AudioSource> GetPlayingList(Type type)
		{
			List<AudioSource> list = new List<AudioSource>();
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				list.Add(transform.GetChild(i).GetComponent<AudioSource>());
			}
			return list;
		}

		public bool IsPlay(Type type, string playName = null)
		{
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				if (playName.IsNullOrEmpty() || !(playName != transform.GetChild(i).name))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPlay(Transform trans)
		{
			for (int i = 0; i < typeObjects.Length; i++)
			{
				if (IsPlay((Type)i, trans))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsPlay(Type type, Transform trans)
		{
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child == trans)
				{
					return true;
				}
			}
			return false;
		}

		public Transform FindAsset(Type type, string assetName, string assetBundleName = null)
		{
			if (typeObjects == null)
			{
				return null;
			}
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				LoadAudioBase componentInChildren = child.GetComponentInChildren<LoadAudioBase>();
				if (componentInChildren != null && componentInChildren.clip != null && componentInChildren.assetName == assetName && (assetBundleName == null || componentInChildren.assetBundleName == assetBundleName))
				{
					if (type != 0)
					{
						return child;
					}
					if (child.gameObject != oldBGM)
					{
						return child;
					}
				}
			}
			return null;
		}

		public Transform Play(Type type, string assetBundleName, string assetName, float delayTime = 0f, float fadeTime = 0f, bool isAssetEqualPlay = true, bool isAsync = true, int settingNo = -1, bool isBundleUnload = true)
		{
			LoadSound loadSound = new GameObject("Sound Loading").AddComponent<LoadSound>();
			loadSound.assetBundleName = assetBundleName;
			loadSound.assetName = assetName;
			loadSound.type = type;
			loadSound.delayTime = delayTime;
			loadSound.fadeTime = fadeTime;
			loadSound.isAssetEqualPlay = isAssetEqualPlay;
			loadSound.isAsync = isAsync;
			loadSound.settingNo = settingNo;
			loadSound.isBundleUnload = isBundleUnload;
			return SetParent(typeObjects[(int)type], loadSound, settingObjects[(int)type]);
		}

		public void Stop(Type type)
		{
			List<GameObject> list = new List<GameObject>();
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				list.Add(transform.GetChild(i).gameObject);
			}
			list.ForEach(delegate(GameObject p)
			{
				Object.Destroy(p);
			});
		}

		public void Stop(Type type, Transform trans)
		{
			Transform transform = typeObjects[(int)type];
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child == trans)
				{
					Object.Destroy(child.gameObject);
					break;
				}
			}
		}

		public void Stop(Transform trans)
		{
			for (int i = 0; i < typeObjects.Length; i++)
			{
				Transform transform = typeObjects[i];
				for (int j = 0; j < transform.childCount; j++)
				{
					Transform child = transform.GetChild(j);
					if (child == trans)
					{
						if (i == 0)
						{
							Stop(Type.BGM);
						}
						else
						{
							Object.Destroy(child.gameObject);
						}
						return;
					}
				}
			}
		}

		public void PlayBGM(float fadeTime = 0f)
		{
			if (currentBGM != null)
			{
				currentBGM.GetComponent<FadePlayer>().SafeProc(delegate(FadePlayer p)
				{
					p.Play(fadeTime);
				});
			}
		}

		public void PauseBGM()
		{
			if (currentBGM != null)
			{
				currentBGM.GetComponent<FadePlayer>().SafeProc(delegate(FadePlayer p)
				{
					p.Pause();
				});
			}
		}

		public void StopBGM(float fadeTime = 0f)
		{
			if (currentBGM != null)
			{
				currentBGM.GetComponent<FadePlayer>().SafeProc(delegate(FadePlayer p)
				{
					p.Stop(fadeTime);
				});
			}
			(from item in typeObjects[0].gameObject.Children()
				where item != currentBGM
				select item).ToList().ForEach(Object.Destroy);
		}

		public AudioSource Play(Type type, AudioClip clip, float fadeTime = 0f)
		{
			AudioSource audioSource = Create(type);
			audioSource.clip = clip;
			audioSource.GetOrAddComponent<FadePlayer>().SafeProc(delegate(FadePlayer p)
			{
				p.Play(fadeTime);
			});
			return audioSource;
		}

		public AudioSource CreateCache(Type type, AssetBundleData data)
		{
			return CreateCache(type, data.bundle, data.asset);
		}

		public AudioSource CreateCache(Type type, AssetBundleManifestData data)
		{
			return CreateCache(type, data.bundle, data.asset, data.manifest);
		}

		public AudioSource CreateCache(Type type, string bundle, string asset, string manifest = null)
		{
			Dictionary<string, AudioSource> value;
			if (!dicASCache.TryGetValue((int)type, out value))
			{
				Dictionary<string, AudioSource> dictionary = new Dictionary<string, AudioSource>();
				dicASCache[(int)type] = dictionary;
				value = dictionary;
			}
			AudioSource value2;
			if (!value.TryGetValue(asset, out value2))
			{
				value2 = Create(type, true);
				value2.name = asset;
				value2.clip = new AssetBundleManifestData(bundle, asset, manifest).GetAsset<AudioClip>();
				Register(value2.clip);
				value.Add(asset, value2);
			}
			return value2;
		}

		public void ReleaseCache(Type type, string bundle, string asset, string manifest = null)
		{
			Dictionary<string, AudioSource> value;
			if (dicASCache.TryGetValue((int)type, out value))
			{
				AudioSource value2;
				if (value.TryGetValue(asset, out value2))
				{
					Remove(value2.clip);
					Object.Destroy(value2.gameObject);
					value.Remove(asset);
					AssetBundleManager.UnloadAssetBundle(bundle, false, manifest);
				}
				if (!value.Any())
				{
					dicASCache.Remove((int)type);
				}
			}
		}

		private void LoadSettingData()
		{
			settingDataList = new List<SoundSettingData.Param>();
			AssetBundleData assetBundleData = new AssetBundleData("sound/setting/soundsettingdata/00.unity3d", null);
			foreach (List<SoundSettingData.Param> item in from p in assetBundleData.GetAllAssets<SoundSettingData>()
				select p.param)
			{
				settingDataList.AddRange(item);
			}
			settingDataList.Sort((SoundSettingData.Param a, SoundSettingData.Param b) => a.No.CompareTo(b.No));
			assetBundleData.UnloadBundle(true);
			setting3DDataList = new List<Sound3DSettingData.Param>();
			assetBundleData = new AssetBundleData("sound/setting/sound3dsettingdata/00.unity3d", null);
			foreach (List<Sound3DSettingData.Param> item2 in from p in assetBundleData.GetAllAssets<Sound3DSettingData>()
				select p.param)
			{
				setting3DDataList.AddRange(item2);
			}
			setting3DDataList.Sort((Sound3DSettingData.Param a, Sound3DSettingData.Param b) => a.No.CompareTo(b.No));
			assetBundleData.UnloadBundle(true);
		}

		private void LoadSetting(Type type, int settingNo = -1)
		{
			string text = type.ToString();
			AssetBundleData assetBundleData = new AssetBundleData("sound/setting/object/00.unity3d", text.ToLower());
			GameObject gameObject = Object.Instantiate(assetBundleData.GetAsset<GameObject>(), rootSetting, false);
			gameObject.name = text + "_Setting";
			AudioSource component = gameObject.GetComponent<AudioSource>();
			AudioSettingData(component, settingNo);
			if (text.CompareParts("gamese", true))
			{
				text = "GameSE";
			}
			settingObjects[(int)type] = gameObject;
			assetBundleData.UnloadBundle(true);
		}

		protected override void Awake()
		{
			if (CheckInstance())
			{
				Object.DontDestroyOnLoad(base.gameObject);
				Mixer = new AssetBundleData("sound/data/mixer/00.unity3d", "master").GetAsset<AudioMixer>();
				rootSetting = new GameObject("SettingObject").transform;
				rootSetting.SetParent(base.transform, false);
				rootPlay = new GameObject("PlayObject").transform;
				rootPlay.SetParent(base.transform, false);
				LoadSettingData();
				settingObjects = new GameObject[Utils.Enum<Type>.Length];
				typeObjects = new Transform[settingObjects.Length];
				for (int i = 0; i < settingObjects.Length; i++)
				{
					Type type = (Type)i;
					LoadSetting(type);
					Transform transform = new GameObject(type.ToString()).transform;
					transform.SetParent(rootPlay, false);
					typeObjects[i] = transform;
				}
				dicASCache = new Dictionary<int, Dictionary<string, AudioSource>>();
				ASCacheRoot = new GameObject("AudioSourceCache").transform;
				ASCacheRoot.SetParent(rootPlay, false);
				listener = new GameObject("Listener", typeof(AudioListener)).transform;
				listener.SetParent(base.transform, false);
				if (Camera.main != null)
				{
					_listener = Camera.main.transform;
				}
			}
		}

		private void Update()
		{
			if (_listener != null)
			{
				listener.SetPositionAndRotation(_listener.position, _listener.rotation);
			}
			else
			{
				listener.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
			}
		}
	}
}
