using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ActionGame.MapSound;
using Illusion.Game;
using Manager;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace ActionGame
{
	public class EnvArea3D : MonoBehaviour
	{
		[Serializable]
		public class EnvironmentInfo3D : EnvironmentInfo
		{
			[SerializeField]
			private Threshold _dampingScale = new Threshold(1f, 500f);

			[SerializeField]
			private CycleLayer _targetCycle = CycleLayer.Day | CycleLayer.Evening;

			[SerializeField]
			private Transform _transform;

			public Threshold DampingScale
			{
				get
				{
					return _dampingScale;
				}
				set
				{
					_dampingScale = value;
				}
			}

			public CycleLayer TargetCycle
			{
				get
				{
					return _targetCycle;
				}
				set
				{
					_targetCycle = value;
				}
			}

			public Transform Transform
			{
				get
				{
					return _transform;
				}
			}
		}

		[Serializable]
		public class PlayInfo3D : PlayInfo
		{
			[SerializeField]
			private Transform _transform;

			[SerializeField]
			private Threshold _dampingThreshold = new Threshold(1f, 500f);

			[SerializeField]
			private CycleLayer _targetCycle = CycleLayer.Day | CycleLayer.Club | CycleLayer.Evening;

			public Transform Transform
			{
				get
				{
					return _transform;
				}
				set
				{
					_transform = value;
				}
			}

			public Threshold DampingThreshold
			{
				get
				{
					return _dampingThreshold;
				}
				set
				{
					_dampingThreshold = value;
				}
			}

			public CycleLayer TargetCycle
			{
				get
				{
					return _targetCycle;
				}
				set
				{
					_targetCycle = value;
				}
			}

			protected override void Play()
			{
				ActionScene actScene = Singleton<Game>.Instance.actScene;
				if (actScene == null || !actScene.Cycle.isAction)
				{
					return;
				}
				CycleLayer cycleLayer = (CycleLayer)0;
				switch (actScene.Cycle.nowType)
				{
				case Cycle.Type.LunchTime:
					cycleLayer = CycleLayer.Day;
					break;
				case Cycle.Type.StaffTime:
					cycleLayer = CycleLayer.Club;
					break;
				case Cycle.Type.AfterSchool:
					cycleLayer = CycleLayer.Evening;
					break;
				}
				if (TargetCycle == (TargetCycle | cycleLayer) && cycleLayer != 0)
				{
					if (_playingSource != null)
					{
						_playingSource.Stop();
					}
					if (!(base.Clip == null))
					{
						_playingSource = Utils.Sound.Play(Manager.Sound.Type.ENV, base.Clip);
						_playingSource.transform.position = Transform.position;
						_playingSource.loop = false;
						_playingSource.minDistance = _dampingThreshold.min;
						_playingSource.maxDistance = _dampingThreshold.max;
						_playingSource.rolloffMode = AudioRolloffMode.Linear;
					}
				}
			}

			public static bool operator ==(PlayInfo3D a, EnvironmentInfo3D b)
			{
				bool flag = a.Clip == Utils.Sound.EnvClipTable[b.ClipID];
				bool flag2 = a._range.min == b.Range.min && a._range.max == b.Range.max;
				bool flag3 = a.Transform == b.Transform;
				bool flag4 = a.DampingThreshold.min == b.DampingScale.min && a.DampingThreshold.max == b.DampingScale.max;
				bool flag5 = a.TargetCycle == b.TargetCycle;
				return flag && flag2 && flag3 && flag4 && flag5;
			}

			public static bool operator !=(PlayInfo3D a, EnvironmentInfo3D b)
			{
				bool flag = a.Clip == Utils.Sound.EnvClipTable[b.ClipID];
				bool flag2 = a._range.min == b.Range.min && a._range.max == b.Range.max;
				bool flag3 = a.Transform == b.Transform;
				bool flag4 = a.DampingThreshold.min == b.DampingScale.min && a.DampingThreshold.max == b.DampingScale.max;
				bool flag5 = a.TargetCycle == b.TargetCycle;
				return !flag || !flag2 || !flag3 || !flag4 || !flag5;
			}

			public override int GetHashCode()
			{
				int hashCode = base.Clip.GetHashCode();
				hashCode ^= _range.GetHashCode() << 2;
				hashCode ^= Transform.GetHashCode() << 2;
				hashCode ^= DampingThreshold.GetHashCode() << 2;
				return hashCode ^ (TargetCycle.GetHashCode() << 2);
			}

			public override bool Equals(object obj)
			{
				if (!(obj is PlayInfo3D))
				{
					return false;
				}
				PlayInfo3D playInfo3D = obj as PlayInfo3D;
				return base.Clip.Equals(playInfo3D.Clip) && base.Range.Equals(playInfo3D.Range) && Transform.Equals(playInfo3D.Transform) && DampingThreshold.Equals(playInfo3D.DampingThreshold) && TargetCycle.Equals(playInfo3D.TargetCycle);
			}
		}

		[Serializable]
		public class EnvironmentInfo
		{
			[SerializeField]
			protected int _clipID;

			[SerializeField]
			protected Threshold _range = default(Threshold);

			public int ClipID
			{
				get
				{
					return _clipID;
				}
				set
				{
					_clipID = value;
				}
			}

			public Threshold Range
			{
				get
				{
					return _range;
				}
				set
				{
					_range = value;
				}
			}
		}

		[Serializable]
		public class PlayInfo
		{
			[SerializeField]
			private AudioClip _clip;

			[SerializeField]
			private float _delay;

			[SerializeField]
			private float _elapsedTime;

			[SerializeField]
			[HideInInspector]
			protected Threshold _range = default(Threshold);

			[SerializeField]
			[HideInInspector]
			protected AudioSource _playingSource;

			public AudioClip Clip
			{
				get
				{
					return _clip;
				}
				set
				{
					_clip = value;
				}
			}

			public float Delay
			{
				get
				{
					return _delay;
				}
				private set
				{
					_delay = value;
				}
			}

			public float ElapsedTime
			{
				get
				{
					return _elapsedTime;
				}
				set
				{
					_elapsedTime = value;
				}
			}

			public Threshold Range
			{
				get
				{
					return _range;
				}
				set
				{
					_range = value;
				}
			}

			public virtual void Update(bool enablePlayback = true)
			{
				if (enablePlayback)
				{
					if (!(_playingSource != null) || !_playingSource.isPlaying)
					{
						ElapsedTime += Time.deltaTime;
						if (!(ElapsedTime < Delay))
						{
							Play();
							ElapsedTime = 0f;
							ResetDelay();
						}
					}
				}
				else if (_playingSource != null && _playingSource.isPlaying)
				{
					_playingSource.Stop();
				}
			}

			protected virtual void Play()
			{
				if (_playingSource != null)
				{
					_playingSource.Stop();
				}
				_playingSource = Utils.Sound.Play(Manager.Sound.Type.ENV, Clip);
				_playingSource.loop = false;
			}

			public void ResetDelay()
			{
				Delay = Range.RandomValue;
			}

			public void Release()
			{
				if (_playingSource != null && _playingSource.isPlaying)
				{
					_playingSource.Stop();
				}
			}
		}

		[Serializable]
		public struct Threshold
		{
			public float min;

			public float max;

			public float RandomValue
			{
				get
				{
					return UnityEngine.Random.Range(min, max);
				}
			}

			public Threshold(float minValue, float maxValue)
			{
				min = minValue;
				max = maxValue;
			}
		}

		private const string _assetbundleDirectory = "action/list/sound/se/env/";

		private const string _clipTag = "clip";

		private const string _playlistTag = "playlist";

		private static Dictionary<int, EnvironmentInfo[]> _playlistTable = new Dictionary<int, EnvironmentInfo[]>();

		private static bool _initialized = false;

		[SerializeField]
		[FormerlySerializedAs("_playlist")]
		private EnvironmentInfo3D[] _playlist;

		[SerializeField]
		[HideInInspector]
		private PlayInfo3D[] _playdataSolid;

		[HideInInspector]
		[SerializeField]
		private bool _showAllGizmo;

		public IEnumerable<EnvironmentInfo3D> PlayList
		{
			get
			{
				return _playlist;
			}
		}

		public IEnumerable<PlayInfo3D> PlayDataSolid
		{
			get
			{
				return _playdataSolid;
			}
		}

		public bool ShowAllGizmo
		{
			get
			{
				return _showAllGizmo;
			}
			set
			{
				_showAllGizmo = value;
			}
		}

		public static void Initialize()
		{
			if (_initialized)
			{
				return;
			}
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/sound/se/env/");
			assetBundleNameListFromPath.Sort();
			Regex regex = new Regex("#([a-zA-Z]+)", RegexOptions.IgnoreCase);
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
				string text2 = string.Empty;
				for (int j = 1; j < count; j++)
				{
					ExcelData.Param param = asset.list[j];
					int count2 = param.list.Count;
					for (int k = 0; k < count2; k++)
					{
						int num = 0;
						string text3 = param.list[num++];
						Match match = regex.Match(text3);
						if (match.Success)
						{
							text2 = match.Groups[1].Value;
						}
						else
						{
							if (text2 == null || !(text2 == "playlist"))
							{
								continue;
							}
							int result;
							if (!int.TryParse(text3, out result))
							{
								continue;
							}
							string text4 = param.list[num++];
							string text5 = param.list[num++];
							string text6 = param.list[num++];
							string text7 = param.list[num++];
							string assetBundleName = text5;
							string assetName = text6;
							string manifestName = text7;
							ExcelData listExcel = CommonLib.LoadAsset<ExcelData>(assetBundleName, assetName, false, manifestName);
							if (listExcel == null)
							{
								continue;
							}
							EnvironmentInfo[] value = (from x in Enumerable.Range(1, listExcel.MaxCell - 1).Select(delegate(int lr)
								{
									List<string> row = listExcel.GetRow(lr);
									int num2 = 0;
									int result2;
									if (!int.TryParse(row[num2++], out result2))
									{
										return (EnvironmentInfo)null;
									}
									string text8 = row[num2++];
									float result3;
									if (!float.TryParse(row[num2++], out result3))
									{
										return (EnvironmentInfo)null;
									}
									float result4;
									return (!float.TryParse(row[num2++], out result4)) ? null : new EnvironmentInfo
									{
										ClipID = result2,
										Range = new Threshold(result3, result4)
									};
								})
								where x != null
								select x).ToArray();
							_playlistTable[result] = value;
						}
					}
				}
			}
			_initialized = true;
		}

		private void Awake()
		{
			Initialize();
		}

		private void Start()
		{
			InitiateComponent();
		}

		private void InitiateComponent()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			_playdataSolid = _playlist.Select(delegate(EnvironmentInfo3D x)
			{
				PlayInfo3D playInfo3D = new PlayInfo3D
				{
					Clip = Utils.Sound.EnvClipTable[x.ClipID],
					Range = x.Range,
					DampingThreshold = x.DampingScale,
					TargetCycle = x.TargetCycle
				};
				if (x.Transform != null)
				{
					playInfo3D.Transform = x.Transform;
				}
				else
				{
					playInfo3D.Transform = base.transform;
				}
				return playInfo3D;
			}).ToArray();
			PlayInfo3D[] playdataSolid = _playdataSolid;
			foreach (PlayInfo3D playInfo3D2 in playdataSolid)
			{
				playInfo3D2.ResetDelay();
			}
		}

		private void Update()
		{
			if (!Singleton<Character>.IsInstance())
			{
				return;
			}
			bool flag = false;
			foreach (KeyValuePair<int, ChaControl> item in Singleton<Character>.Instance.dictEntryChara)
			{
				if (!(item.Value == null))
				{
					flag |= item.Value.hiPoly;
				}
			}
			if (_playdataSolid != null)
			{
				PlayInfo3D[] playdataSolid = _playdataSolid;
				foreach (PlayInfo3D playInfo3D in playdataSolid)
				{
					playInfo3D.Update(!flag);
				}
			}
		}

		private void OnDestroy()
		{
			if (!LinqExtensions.IsNullOrEmpty(_playdataSolid))
			{
				PlayInfo3D[] playdataSolid = _playdataSolid;
				foreach (PlayInfo3D playInfo3D in playdataSolid)
				{
					playInfo3D.Release();
				}
			}
		}

		public void LoadFromExcelData(ExcelData excelData)
		{
			_playlist = (from x in Enumerable.Range(1, excelData.MaxCell - 1).Select(delegate(int lr)
				{
					List<string> row = excelData.GetRow(lr);
					int num = 0;
					int result;
					if (!int.TryParse(row[num++], out result))
					{
						return (EnvironmentInfo3D)null;
					}
					string text = row[num++];
					float result2;
					float result3;
					if (!float.TryParse(row.ElementAtOrDefault(num++), out result2) || !float.TryParse(row.ElementAtOrDefault(num++), out result3))
					{
						return (EnvironmentInfo3D)null;
					}
					float result4;
					if (!float.TryParse(row.ElementAtOrDefault(num++), out result4))
					{
						return (EnvironmentInfo3D)null;
					}
					float result5;
					if (!float.TryParse(row.ElementAtOrDefault(num++), out result5))
					{
						return (EnvironmentInfo3D)null;
					}
					int result6;
					if (!int.TryParse(row.ElementAtOrDefault(num++), out result6))
					{
						return (EnvironmentInfo3D)null;
					}
					int result7 = -1;
					if (result6 > -1)
					{
						string s = Convert.ToString(result6, 2);
						if (!int.TryParse(s, out result7))
						{
							return (EnvironmentInfo3D)null;
						}
					}
					return new EnvironmentInfo3D
					{
						ClipID = result,
						DampingScale = new Threshold(result2, result3),
						Range = new Threshold(result4, result5),
						TargetCycle = (CycleLayer)result7
					};
				})
				where x != null
				select x).ToArray();
		}
	}
}
