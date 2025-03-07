using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionGame.Chara;
using ActionGame.MapSound;
using Illusion.Game;
using Manager;
using Sirenix.OdinInspector;
using StrayTech;
using UnityEngine;

namespace ActionGame
{
	public class FootStepSEDefault : MonoBehaviour
	{
		private const string _footstepAssetBundleCommonPath = "action/list/sound/se/footstep/";

		private const string _mapSeAssetBundleCommonPath = "action/list/sound/se/action/";

		private static Dictionary<int, AudioClip[]> _audioClips = new Dictionary<int, AudioClip[]>();

		private static Dictionary<int, AudioClip> _mapSEClipTable = new Dictionary<int, AudioClip>();

		private static bool _initialized = false;

		private List<AudioSource> _audioSources = new List<AudioSource>();

		private Transform[] _playableBases = new Transform[0];

		[SerializeField]
		[MinValue(0.0)]
		private int _playableNPCCount = 3;

		public static void Initialize()
		{
			if (!_initialized)
			{
				InitFootStep();
				InitMapSE();
				_initialized = true;
			}
		}

		private static void InitFootStep()
		{
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/se/footstep/");
			assetBundleNameListFromPath.Sort();
			for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
			{
				string text = assetBundleNameListFromPath[i];
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (!AssetBundleCheck.IsFile(text, fileNameWithoutExtension))
				{
					continue;
				}
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(assetBundleNameListFromPath[i], fileNameWithoutExtension, typeof(ExcelData));
				if (assetBundleLoadAssetOperation == null)
				{
					continue;
				}
				ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
				if (asset == null)
				{
					continue;
				}
				int count = asset.list.Count;
				for (int j = 1; j < count; j++)
				{
					ExcelData.Param param = asset.list[j];
					int count2 = param.list.Count;
					for (int k = 0; k < count2; k++)
					{
						int num = 0;
						int result;
						if (!int.TryParse(param.list[num++], out result))
						{
							continue;
						}
						num++;
						string text2 = param.list[num++];
						string text3 = param.list[num++];
						string text4 = param.list[num++];
						string assetBundleName = text2;
						string assetName = text3;
						string manifestName = text4;
						ExcelData listExcel = CommonLib.LoadAsset<ExcelData>(assetBundleName, assetName, false, manifestName);
						if (!(listExcel == null))
						{
							AudioClip[] value = Enumerable.Range(1, listExcel.MaxCell - 1).Select(delegate(int lr)
							{
								List<string> row = listExcel.GetRow(lr);
								int num2 = 1;
								string text5 = row[num2++];
								string text6 = row[num2++];
								string text7 = row[num2++];
								string assetBundleName2 = text5;
								string assetName2 = text6;
								string manifestName2 = text7;
								AudioClip result2 = CommonLib.LoadAsset<AudioClip>(assetBundleName2, assetName2, false, manifestName2);
								AssetBundleManager.UnloadAssetBundle(text5, true);
								return result2;
							}).ToArray();
							_audioClips[result] = value;
							AssetBundleManager.UnloadAssetBundle(text2, true);
						}
					}
				}
				AssetBundleManager.UnloadAssetBundle(text, true);
			}
		}

		private static void InitMapSE()
		{
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/se/action/");
			assetBundleNameListFromPath.Sort();
			for (int i = 0; i < assetBundleNameListFromPath.Count; i++)
			{
				string text = assetBundleNameListFromPath[i];
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				if (!AssetBundleCheck.IsFile(text, fileNameWithoutExtension))
				{
					continue;
				}
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAsset(text, fileNameWithoutExtension, typeof(ExcelData));
				if (assetBundleLoadAssetOperation.IsEmpty())
				{
					AssetBundleManager.UnloadAssetBundle(text, true);
					continue;
				}
				ExcelData asset = assetBundleLoadAssetOperation.GetAsset<ExcelData>();
				if (asset == null)
				{
					continue;
				}
				int maxCell = asset.MaxCell;
				for (int j = 0; j < maxCell; j++)
				{
					List<string> row = asset.GetRow(j);
					int num = 0;
					int result;
					if (int.TryParse(row.ElementAtOrDefault(num++), out result))
					{
						string text2 = row.ElementAtOrDefault(num++);
						string text3 = row.ElementAtOrDefault(num++);
						string text4 = row.ElementAtOrDefault(num++);
						string text5 = row.ElementAtOrDefault(num++);
						string assetBundleName = text3;
						string assetName = text4;
						string manifestName = text5;
						AudioClip value = CommonLib.LoadAsset<AudioClip>(assetBundleName, assetName, false, manifestName);
						_mapSEClipTable[result] = value;
					}
				}
			}
		}

		private void Awake()
		{
			Initialize();
			if (Utils.Sound.FootStepPlayCall == null)
			{
				Utils.Sound.FootStepPlayCall = delegate(int index, Transform t)
				{
					PlayFootStep(index, t);
				};
			}
			if (Utils.Sound.SEPlayCall == null)
			{
				Utils.Sound.SEPlayCall = (Transform t, int id, bool loop, Threshold threshold) => PlaySE(t, id, loop, threshold);
			}
			if (Utils.Sound.SEStopCall == null)
			{
				Utils.Sound.SEStopCall = delegate(AudioSource x)
				{
					StopSE(x);
				};
			}
		}

		private void Update()
		{
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene == null)
			{
				return;
			}
			NPC[] array = actScene.npcList.Where((NPC x) => x.isActive).ToArray();
			if (array.IsNullOrEmpty())
			{
				return;
			}
			List<Transform> list = ListPool<Transform>.Get();
			Vector3 cameraPosition = MonoBehaviourSingleton<CameraSystem>.Instance.CurrentCamera.transform.position;
			while (list.Count < _playableNPCCount)
			{
				float min = array.Min((NPC x) => Vector3.Distance(x.transform.position, cameraPosition));
				NPC npc = array.FirstOrDefault((NPC x) => Vector3.Distance(x.transform.position, cameraPosition) == min);
				if (npc == null)
				{
					break;
				}
				list.Add(npc.transform);
				array = array.Where((NPC x) => x != npc).ToArray();
				if (array.IsNullOrEmpty())
				{
					break;
				}
			}
			_playableBases = list.ToArray();
			ListPool<Transform>.Release(list);
		}

		private void PlayFootStep(int index, Transform t)
		{
			if (!Singleton<Game>.IsInstance())
			{
				return;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene == null)
			{
				return;
			}
			FootStepSE value;
			if (!Utils.Sound.FootStepAreaTypes.TryGetValue(index, out value))
			{
				FootStepSE footStepAreaDefaultType = Utils.Sound.FootStepAreaDefaultType;
				Utils.Sound.FootStepAreaTypes[index] = footStepAreaDefaultType;
				value = footStepAreaDefaultType;
			}
			AudioClip[] array = _audioClips[(int)value];
			if (!array.IsNullOrEmpty())
			{
				int num = Random.Range(0, array.Length);
				AudioClip clip = array[num];
				_audioSources.RemoveAll((AudioSource x) => x == null || x.gameObject == null || x.isPlaying);
				if (!(t != actScene.Player.transform) || _playableBases.Any((Transform x) => x == t))
				{
					AudioSource audioSource = Utils.Sound.Play(Manager.Sound.Type.GameSE3D, clip);
					audioSource.transform.position = t.position;
					audioSource.rolloffMode = AudioRolloffMode.Linear;
					audioSource.minDistance = 1f;
					audioSource.maxDistance = 7.5f;
					_audioSources.Add(audioSource);
				}
			}
		}

		private AudioSource PlaySE(Transform t, int id, bool loop, Threshold threshold)
		{
			if (t == null)
			{
				return null;
			}
			AudioClip value;
			if (!_mapSEClipTable.TryGetValue(id, out value))
			{
				return null;
			}
			AudioSource audioSource = Utils.Sound.Play(Manager.Sound.Type.GameSE3D, value);
			audioSource.transform.position = t.position;
			audioSource.loop = loop;
			audioSource.rolloffMode = AudioRolloffMode.Linear;
			audioSource.minDistance = threshold.min;
			audioSource.maxDistance = threshold.max;
			return audioSource;
		}

		private void StopSE(AudioSource source)
		{
			if (!(source == null))
			{
				source.Stop();
			}
		}

		private void OnDestroy()
		{
			foreach (AudioSource audioSource in _audioSources)
			{
				if (!(audioSource == null) && !(audioSource.gameObject == null) && audioSource.isPlaying)
				{
					audioSource.Stop();
				}
			}
			foreach (KeyValuePair<int, AudioClip[]> audioClip in _audioClips)
			{
				AudioClip[] value = audioClip.Value;
				AudioClip[] array = value;
				foreach (AudioClip assetToUnload in array)
				{
					Resources.UnloadAsset(assetToUnload);
				}
			}
			foreach (KeyValuePair<int, AudioClip> item in _mapSEClipTable)
			{
				if (!(item.Value == null))
				{
					AudioClip value2 = item.Value;
					Resources.UnloadAsset(value2);
				}
			}
		}
	}
}
