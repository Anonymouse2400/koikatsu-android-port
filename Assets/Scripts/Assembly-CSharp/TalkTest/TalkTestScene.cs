using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ActionGame;
using ActionGame.Communication;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace TalkTest
{
	public class TalkTestScene : BaseLoader
	{
		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private Button buttonStart;

		[SerializeField]
		private Toggle toggleAnger;

		[SerializeField]
		private Toggle toggleResetTime;

		[SerializeField]
		private Dropdown dropdownStage;

		[SerializeField]
		private Dropdown dropdownCycle;

		[SerializeField]
		private Dropdown dropdownMap;

		[SerializeField]
		private Slider sliderFavor;

		[SerializeField]
		private Text textFavor;

		[SerializeField]
		private Slider sliderLeewdness;

		[SerializeField]
		private Text textLeewdness;

		[SerializeField]
		private Toggle toggleOtherPeople;

		[SerializeField]
		private Toggle toggleChase;

		[SerializeField]
		private Toggle toggleChasePossible;

		[SerializeField]
		private Toggle toggleHPossible;

		[SerializeField]
		private Toggle toggleNotice;

		[SerializeField]
		private Dropdown dropdownState;

		[SerializeField]
		private BaseMap baseMap;

		[SerializeField]
		private Dropdown dropdownPersonality;

		private SaveData.Heroine heroine;

		private Dictionary<int, MapInfo.Param> dicMap;

		private readonly Version m_Version = new Version(0, 1, 1);

		private readonly string m_SavePath = "talktest";

		private readonly string m_SaveFile = "backup.bin";

		private bool isAttack;

		private bool isHAttack;

		private bool exposure;

		private bool femaleOnly;

		private bool introSkip;

		private IDisposable talkTimeDisposable;

		private int stage { get; set; }

		private int cycle { get; set; }

		private int map { get; set; }

		private float favor { get; set; }

		private float leewdness { get; set; }

		private bool anger { get; set; }

		private bool otherPeople { get; set; }

		private bool chase { get; set; }

		private bool chasePossible { get; set; }

		private bool hPossible { get; set; }

		private bool notice { get; set; }

		private int state { get; set; }

		private int personality { get; set; }

		private IEnumerator StartTalk()
		{
			canvas.enabled = false;
			if (heroine == null)
			{
				heroine = new SaveData.Heroine(true);
				string[] array = new string[2] { "custom/presets_m_00.unity3d", "custom/presets_f_00.unity3d" };
				string[] array2 = new string[2] { "ill_Default_Male", "ill_Default_Female" };
				int num = 1;
				ChaFileControl chaFileControl = new ChaFileControl();
				chaFileControl.LoadFromAssetBundle(array[num], array2[num]);
				heroine.SetCharFile(chaFileControl);
				if (Singleton<Game>.IsInstance())
				{
					num = 0;
					chaFileControl = new ChaFileControl();
					chaFileControl.LoadFromAssetBundle(array[num], array2[num]);
					Singleton<Game>.Instance.Player.SetCharFile(chaFileControl);
				}
			}
			heroine.charFile.parameter.personality = dropdownPersonality.value;
			switch (dropdownStage.value)
			{
			case 0:
				heroine.talkEvent.Remove(0);
				heroine.talkEvent.Remove(2);
				heroine.isGirlfriend = false;
				break;
			case 1:
				heroine.talkEvent.Add(0);
				heroine.talkEvent.Remove(2);
				heroine.isGirlfriend = false;
				break;
			case 2:
				heroine.talkEvent.Add(0);
				heroine.talkEvent.Add(2);
				heroine.isGirlfriend = false;
				break;
			case 3:
				heroine.talkEvent.Add(0);
				heroine.talkEvent.Add(2);
				heroine.isGirlfriend = true;
				break;
			}
			Cycle.Type[] cycle = new Cycle.Type[3]
			{
				Cycle.Type.LunchTime,
				Cycle.Type.StaffTime,
				Cycle.Type.AfterSchool
			};
			heroine.favor = (int)sliderFavor.value * 50;
			heroine.lewdness = (int)sliderLeewdness.value * 50;
			if (!heroine.isAnger && toggleAnger.isOn)
			{
				heroine.anger = 100;
			}
			heroine.isAnger = toggleAnger.isOn;
			if (talkTimeDisposable != null)
			{
				talkTimeDisposable.Dispose();
				talkTimeDisposable = null;
			}
			if (toggleResetTime.isOn)
			{
				talkTimeDisposable = this.UpdateAsObservable().Subscribe(delegate
				{
					heroine.talkTime = heroine.talkTimeMax;
				});
			}
			Save();
			TalkScene talk = null;
			bool isWait = true;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				assetBundleName = "action/menu/talk.unity3d",
				levelName = "Talk",
				isAdd = true,
				onLoad = delegate
				{
					talk = Scene.GetRootComponent<TalkScene>("Talk");
					talk.targetHeroine = heroine;
					talk.necessaryInfo = new NecessaryInfo(dicMap.Keys.ToArray()[dropdownMap.value], cycle[dropdownCycle.value], toggleOtherPeople.isOn, toggleChase.isOn, toggleChasePossible.isOn, toggleHPossible.isOn, toggleNotice.isOn, dropdownState.value, isAttack, isHAttack, exposure, femaleOnly, introSkip, new string[0]);
					isWait = false;
				}
			}, false);
			yield return new WaitWhile(() => isWait);
			yield return new WaitWhile(() => talk != null);
			canvas.enabled = true;
		}

		private void Save()
		{
			stage = dropdownStage.value;
			cycle = dropdownCycle.value;
			map = dropdownMap.value;
			favor = sliderFavor.value;
			leewdness = sliderLeewdness.value;
			state = dropdownState.value;
			anger = toggleAnger.isOn;
			otherPeople = toggleOtherPeople.isOn;
			chase = toggleChase.isOn;
			chasePossible = toggleChasePossible.isOn;
			hPossible = toggleHPossible.isOn;
			notice = toggleNotice.isOn;
			personality = dropdownPersonality.value;
			string path = DebugData.Create(m_SavePath) + m_SaveFile;
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write(m_Version.ToString());
					binaryWriter.Write(stage);
					binaryWriter.Write(cycle);
					binaryWriter.Write(map);
					binaryWriter.Write(favor);
					binaryWriter.Write(leewdness);
					binaryWriter.Write(anger);
					binaryWriter.Write(otherPeople);
					binaryWriter.Write(chase);
					binaryWriter.Write(chasePossible);
					binaryWriter.Write(hPossible);
					binaryWriter.Write(notice);
					binaryWriter.Write(state);
					binaryWriter.Write(personality);
				}
			}
		}

		private void Load()
		{
			string path = DebugData.Create(m_SavePath) + m_SaveFile;
			if (!File.Exists(path))
			{
				return;
			}
			using (FileStream input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (BinaryReader binaryReader = new BinaryReader(input))
				{
					Version version = new Version(binaryReader.ReadString());
					stage = binaryReader.ReadInt32();
					cycle = binaryReader.ReadInt32();
					map = binaryReader.ReadInt32();
					favor = binaryReader.ReadInt32();
					leewdness = binaryReader.ReadInt32();
					anger = binaryReader.ReadBoolean();
					otherPeople = binaryReader.ReadBoolean();
					chase = binaryReader.ReadBoolean();
					chasePossible = binaryReader.ReadBoolean();
					hPossible = binaryReader.ReadBoolean();
					notice = binaryReader.ReadBoolean();
					state = binaryReader.ReadInt32();
					if (version.CompareTo(new Version(0, 1, 1)) >= 0)
					{
						personality = binaryReader.ReadInt32();
					}
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			Load();
		}

		private IEnumerator Start()
		{
			GameObject cameraObject = Load<GameObject>("camera/action.unity3d", "ActionCamera", true);
			cameraObject.transform.SetParent(base.transform, false);
			AssetBundleManager.UnloadAssetBundle("camera/action.unity3d", false);
			yield return null;
			buttonStart.interactable = false;
			yield return new WaitUntil(() => Singleton<Communication>.Instance.isInit);
			buttonStart.interactable = true;
			buttonStart.OnClickAsObservable().Subscribe(delegate
			{
				StartCoroutine(StartTalk());
			});
			dropdownPersonality.options = Singleton<Game>.Instance.HeroinePersonalitys.Select((KeyValuePair<int, string> v) => new Dropdown.OptionData(v.Value)).ToList();
			dropdownPersonality.value = personality;
			Dropdown.OptionData[] source = new Dropdown.OptionData[4]
			{
				new Dropdown.OptionData("初回"),
				new Dropdown.OptionData("知り合い"),
				new Dropdown.OptionData("友人"),
				new Dropdown.OptionData("恋人")
			};
			dropdownStage.options = source.ToList();
			dropdownStage.value = stage;
			Dropdown.OptionData[] source2 = new Dropdown.OptionData[3]
			{
				new Dropdown.OptionData("お昼"),
				new Dropdown.OptionData("部活時間"),
				new Dropdown.OptionData("放課後")
			};
			dropdownCycle.options = source2.ToList();
			dropdownCycle.value = cycle;
			dicMap = new Dictionary<int, MapInfo.Param>();
			CommonLib.GetAssetBundleNameListFromPath("map/list/mapinfo/", true).ForEach(delegate(string file)
			{
				AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset(file, typeof(MapInfo));
				foreach (List<MapInfo.Param> item in from p in assetBundleLoadAssetOperation.GetAllAssets<MapInfo>()
					select p.param)
				{
					foreach (MapInfo.Param item2 in item)
					{
						dicMap[item2.No] = item2;
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			dropdownMap.options = dicMap.Select((KeyValuePair<int, MapInfo.Param> v) => new Dropdown.OptionData(v.Value.MapName)).ToList();
			dropdownMap.value = map;
			sliderFavor.OnValueChangedAsObservable().Subscribe(delegate(float v)
			{
				textFavor.text = (v * 50f).ToString();
			});
			sliderFavor.value = favor;
			sliderLeewdness.OnValueChangedAsObservable().Subscribe(delegate(float v)
			{
				textLeewdness.text = (v * 50f).ToString();
			});
			sliderLeewdness.value = leewdness;
			Dropdown.OptionData[] state = new Dropdown.OptionData[3]
			{
				new Dropdown.OptionData("立ち"),
				new Dropdown.OptionData("座り"),
				new Dropdown.OptionData("寝ている")
			};
			dropdownState.options = state.ToList();
			dropdownState.value = this.state;
			toggleAnger.isOn = anger;
			toggleOtherPeople.isOn = otherPeople;
			toggleChase.isOn = chase;
			toggleChasePossible.isOn = chasePossible;
			toggleHPossible.isOn = hPossible;
			toggleNotice.isOn = notice;
		}
	}
}
