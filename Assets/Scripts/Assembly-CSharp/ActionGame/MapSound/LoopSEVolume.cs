using System;
using System.Collections.Generic;
using System.Linq;
using ActionGame.Chara;
using Illusion.Game;
using Manager;
using UnityEngine;

namespace ActionGame.MapSound
{
	public class LoopSEVolume : MonoBehaviour
	{
		[Serializable]
		public class Preset
		{
			[SerializeField]
			private CycleLayer _targetCycle = (CycleLayer)(-1);

			[SerializeField]
			private int _clipID = -1;

			[SerializeField]
			private Threshold _dampingThreshold = new Threshold(1f, 7.5f);

			[SerializeField]
			private Transform _base;

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

			public Transform Base
			{
				get
				{
					return _base;
				}
			}
		}

		[Serializable]
		public class RuntimeData
		{
			[SerializeField]
			private CycleLayer _targetCycle = (CycleLayer)(-1);

			[SerializeField]
			private AudioClip _clip;

			[SerializeField]
			private Threshold _dampingThreshold = new Threshold(1f, 7.5f);

			[SerializeField]
			private Transform _base;

			[HideInInspector]
			[SerializeField]
			private AudioSource _source;

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

			public Transform Base
			{
				get
				{
					return _base;
				}
				set
				{
					_base = value;
				}
			}

			public void Play(Manager.Sound.Type type)
			{
				if ((!(_source != null) || !_source.isPlaying) && !(_clip == null))
				{
					_source = Utils.Sound.Play(type, _clip);
					_source.transform.position = Base.position;
					_source.loop = true;
					_source.minDistance = _dampingThreshold.min;
					_source.maxDistance = _dampingThreshold.max;
					_source.rolloffMode = AudioRolloffMode.Linear;
				}
			}

			public void Stop()
			{
				if (!(_source == null))
				{
					_source.Stop();
				}
			}

			public static bool EqualsAgainstPreset(Manager.Sound.Type soundType, RuntimeData runtime, Preset preset)
			{
				Dictionary<int, AudioClip> dictionary = ((soundType != Manager.Sound.Type.GameSE3D) ? Utils.Sound.EnvClipTable : Utils.Sound.SEClipTable);
				bool flag = runtime.Clip == dictionary[preset.ClipID];
				bool flag2 = runtime.DampingThreshold == preset.DampingThreshold;
				bool flag3 = !(preset.Base != null) || runtime.Base == preset.Base;
				bool flag4 = runtime.TargetCycle == preset.TargetCycle;
				return flag && flag2 && flag3;
			}

			public override bool Equals(object obj)
			{
				if (!(obj is RuntimeData))
				{
					return false;
				}
				RuntimeData runtimeData = obj as RuntimeData;
				return Clip == runtimeData.Clip && DampingThreshold == runtimeData.DampingThreshold && Base == runtimeData.Base;
			}

			public override int GetHashCode()
			{
				int hashCode = Clip.GetHashCode();
				hashCode ^= Base.GetHashCode();
				return hashCode ^ DampingThreshold.GetHashCode();
			}
		}

		[SerializeField]
		private Preset[] _presets;

		[SerializeField]
		private RuntimeData[] _runtimeDataList;

		[SerializeField]
		private bool _ignoreCondition;

		[HideInInspector]
		[SerializeField]
		private bool _showAllGizmo;

		[SerializeField]
		private Manager.Sound.Type _soundType = Manager.Sound.Type.GameSE3D;

		[SerializeField]
		private int _mapID;

		[SerializeField]
		private int _actionID;

		public IEnumerable<Preset> Presets
		{
			get
			{
				return _presets;
			}
			set
			{
				_presets = value.ToArray();
			}
		}

		public IEnumerable<RuntimeData> RuntimeDataList
		{
			get
			{
				return _runtimeDataList;
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

		public int MapID
		{
			get
			{
				return _mapID;
			}
			set
			{
				_mapID = value;
			}
		}

		public int ActionID
		{
			get
			{
				return _actionID;
			}
			set
			{
				_actionID = value;
			}
		}

		private void Start()
		{
			InitializeComponent();
		}

		private void OnDestroy()
		{
			RuntimeData[] runtimeDataList = _runtimeDataList;
			foreach (RuntimeData runtimeData in runtimeDataList)
			{
				runtimeData.Stop();
			}
		}

		private void InitializeComponent()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			_runtimeDataList = _presets.Select(delegate(Preset x)
			{
				RuntimeData runtimeData = new RuntimeData
				{
					TargetCycle = x.TargetCycle,
					DampingThreshold = x.DampingThreshold
				};
				if (_soundType == Manager.Sound.Type.GameSE3D)
				{
					runtimeData.Clip = Utils.Sound.SEClipTable[x.ClipID];
				}
				else
				{
					runtimeData.Clip = Utils.Sound.EnvClipTable[x.ClipID];
				}
				if (x.Base != null)
				{
					runtimeData.Base = x.Base;
				}
				else
				{
					runtimeData.Base = base.transform;
				}
				return runtimeData;
			}).ToArray();
		}

		private void Update()
		{
			if (!Singleton<Character>.IsInstance() || !Singleton<Game>.IsInstance())
			{
				return;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if (actScene == null || (!_ignoreCondition && actScene.Map.no != _mapID))
			{
				return;
			}
			bool flag = false;
			foreach (KeyValuePair<int, ChaControl> item in Singleton<Character>.Instance.dictEntryChara)
			{
				flag |= item.Value.hiPoly;
				if (flag)
				{
					break;
				}
			}
			if (!_ignoreCondition)
			{
				if ((from x in actScene.npcList
					where x.isActive
					where x.mapNo == _mapID
					select x).Any((NPC x) => x.isArrival && x.AI.actionNo == _actionID) && !flag)
				{
					RuntimeData[] runtimeDataList = _runtimeDataList;
					foreach (RuntimeData runtimeData in runtimeDataList)
					{
						runtimeData.Play(_soundType);
					}
				}
				else
				{
					RuntimeData[] runtimeDataList2 = _runtimeDataList;
					foreach (RuntimeData runtimeData2 in runtimeDataList2)
					{
						runtimeData2.Stop();
					}
				}
			}
			else if (!flag)
			{
				RuntimeData[] runtimeDataList3 = _runtimeDataList;
				foreach (RuntimeData runtimeData3 in runtimeDataList3)
				{
					runtimeData3.Play(_soundType);
				}
			}
			else
			{
				RuntimeData[] runtimeDataList4 = _runtimeDataList;
				foreach (RuntimeData runtimeData4 in runtimeDataList4)
				{
					runtimeData4.Stop();
				}
			}
		}
	}
}
