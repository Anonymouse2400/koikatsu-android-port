using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADV;
using Manager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace WorkSpace.ADV
{
	[RequireComponent(typeof(BaseMap))]
	public class ADVTest : BaseLoader
	{
		public enum ScenarioType
		{
			Works = 0,
			Main = 1
		}

		public class ADVParam : SceneParameter
		{
			public bool isInVisibleChara = true;

			public ADVParam(MonoBehaviour scene)
				: base(scene)
			{
			}

			public override void Init(Data data)
			{
				ADVScene aDVScene = SceneParameter.advScene;
				TextScenario scenario = aDVScene.Scenario;
				scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
				scenario.LoadBundleName = data.bundleName;
				scenario.LoadAssetName = data.assetName;
				aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
				scenario.heroineList = data.heroineList;
				scenario.transferList = data.transferList;
				if (!data.heroineList.IsNullOrEmpty())
				{
					scenario.currentChara = new CharaData(data.heroineList[0], scenario, null);
				}
				if (data.camera != null)
				{
					scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
				}
				float fadeInTime = data.fadeInTime;
				if (fadeInTime > 0f)
				{
					aDVScene.fadeTime = fadeInTime;
				}
				else
				{
					isInVisibleChara = false;
				}
			}

			public override void Release()
			{
				ADVScene aDVScene = SceneParameter.advScene;
				aDVScene.gameObject.SetActive(false);
			}

			public override void WaitEndProc()
			{
			}
		}

		public ScenarioData scenarioData;

		private string assetBundle = string.Empty;

		private string assetPath = string.Empty;

		private Dictionary<string, string> dicScenario;

		private Dictionary<string, List<string>> dicScenarioMain;

		[SerializeField]
		private Canvas canvas;

		[SerializeField]
		private Button buttonStart;

		[SerializeField]
		private Dropdown dropdownMap;

		[SerializeField]
		private BaseMap baseMap;

		[SerializeField]
		private Dropdown dropdownScenario;

		[SerializeField]
		private Dropdown[] dropdownScenarioMain;

		[SerializeField]
		private Toggle togglePlayer;

		[SerializeField]
		private Toggle toggleHeroine;

		[SerializeField]
		private Toggle[] togglesType;

		private readonly Version m_Version = new Version(0, 1, 4);

		private readonly string m_SavePath = "advtest";

		private readonly string m_SaveFile = "backup.bin";

		public static string mapName { get; set; }

		public static string scenarioName { get; set; }

		public static bool isPlayer { get; set; }

		public static bool isHeroine { get; set; }

		public static ScenarioType scenarioType { get; set; }

		public static string scenarioBundle { get; set; }

		private void SetPath(string _path = "")
		{
		}

		private  IEnumerator Wait()
		{
			canvas.enabled = false;
			yield return null;
            yield return new WaitWhile(() => Program.isADVScene);
            canvas.enabled = true;
		}

		private void Save()
		{
			string path = DebugData.Create(m_SavePath) + m_SaveFile;
			using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(output))
				{
					binaryWriter.Write(m_Version.ToString());
					binaryWriter.Write(mapName);
					binaryWriter.Write(scenarioName);
					binaryWriter.Write(isPlayer);
					binaryWriter.Write(isHeroine);
					binaryWriter.Write((int)scenarioType);
					binaryWriter.Write(scenarioBundle);
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
					mapName = binaryReader.ReadString();
					if (version.CompareTo(new Version(0, 1, 1)) >= 0)
					{
						scenarioName = binaryReader.ReadString();
					}
					if (version.CompareTo(new Version(0, 1, 2)) >= 0)
					{
						isPlayer = binaryReader.ReadBoolean();
					}
					if (version.CompareTo(new Version(0, 1, 3)) >= 0)
					{
						isHeroine = binaryReader.ReadBoolean();
					}
					if (version.CompareTo(new Version(0, 1, 4)) >= 0)
					{
						scenarioType = (ScenarioType)binaryReader.ReadInt32();
					}
					if (version.CompareTo(new Version(0, 1, 4)) >= 0)
					{
						scenarioBundle = binaryReader.ReadString();
					}
				}
			}
		}

		private void Reset()
		{
			baseMap = GetComponent<BaseMap>();
		}

		protected override void Awake()
		{
			base.Awake();
			ParameterList.Add(new ADVParam(this));
			Load();
		}

		private void OnDestroy()
		{
			ParameterList.Remove(this);
		}

		private IEnumerator Start()
		{
			SetPath(string.Empty);
			this.ObserveEveryValueChanged((ADVTest v) => !v.assetBundle.IsNullOrEmpty() && !v.assetPath.IsNullOrEmpty()).SubscribeToInteractable(buttonStart);
			yield return null;
			togglePlayer.isOn = isPlayer;
			toggleHeroine.isOn = isHeroine;
			dropdownMap.options = new Dropdown.OptionData[1]
			{
				new Dropdown.OptionData("なし")
			}.Concat(baseMap.infoDic.Select((KeyValuePair<int, MapInfo.Param> v) => new Dropdown.OptionData(v.Value.MapName))).ToList();
			int idx = dropdownMap.options.FindIndex((Dropdown.OptionData v) => v.text == mapName);
			if (idx != -1)
			{
				dropdownMap.value = idx;
			}
			dropdownScenario.options = dicScenario.Select((KeyValuePair<string, string> v) => new Dropdown.OptionData(v.Key)).ToList();
			dropdownScenario.onValueChanged.AddListener(delegate
			{
				scenarioName = dropdownScenario.options[dropdownScenario.value].text;
				SetPath(dicScenario[scenarioName]);
			});
			if (scenarioType == ScenarioType.Works)
			{
				idx = dropdownScenario.options.FindIndex((Dropdown.OptionData v) => v.text == scenarioName);
				if (idx != -1)
				{
					dropdownScenario.value = idx;
				}
			}
			dicScenarioMain = new Dictionary<string, List<string>>();
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("adv/scenario/", true);
			foreach (string item in assetBundleNameListFromPath)
			{
				dicScenarioMain.Add(item.Replace("adv/scenario/", string.Empty), (from path in AssetBundleCheck.GetAllAssetName(item)
					where Path.GetExtension(path) == ".asset"
					select path).Select(Path.GetFileNameWithoutExtension).ToList());
			}
			dropdownScenarioMain[0].options = dicScenarioMain.Select((KeyValuePair<string, List<string>> v) => new Dropdown.OptionData(v.Key)).ToList();
			dropdownScenarioMain[0].onValueChanged.AddListener(delegate
			{
				string text = dropdownScenarioMain[0].options[dropdownScenarioMain[0].value].text;
				dropdownScenarioMain[1].options = dicScenarioMain[text].Select((string v) => new Dropdown.OptionData(v)).ToList();
				dropdownScenarioMain[1].value = 0;
				dropdownScenarioMain[1].interactable = true;
			});
			dropdownScenarioMain[1].onValueChanged.AddListener(delegate
			{
				assetBundle = "adv/scenario/" + dropdownScenarioMain[0].options[dropdownScenarioMain[0].value].text;
				assetPath = dropdownScenarioMain[1].options[dropdownScenarioMain[1].value].text;
				scenarioBundle = assetBundle;
				scenarioName = assetPath;
			});
			if (scenarioType == ScenarioType.Main)
			{
				string searchName = scenarioBundle.Replace("adv/scenario/", string.Empty);
				idx = dropdownScenarioMain[0].options.FindIndex((Dropdown.OptionData v) => v.text == searchName);
				if (idx != -1)
				{
					dropdownScenarioMain[0].value = idx;
					idx = dropdownScenarioMain[1].options.FindIndex((Dropdown.OptionData v) => v.text == scenarioName);
					if (idx != -1)
					{
						dropdownScenarioMain[1].value = idx;
						assetBundle = "adv/scenario/" + dropdownScenarioMain[0].options[dropdownScenarioMain[0].value].text;
						assetPath = dropdownScenarioMain[1].options[dropdownScenarioMain[1].value].text;
						scenarioBundle = assetBundle;
						scenarioName = assetPath;
					}
				}
			}
			togglePlayer.onValueChanged.AddListener(delegate(bool value)
			{
				isPlayer = value;
			});
			toggleHeroine.onValueChanged.AddListener(delegate(bool value)
			{
				isHeroine = value;
			});
			togglesType[0].onValueChanged.AddListener(delegate(bool value)
			{
				dropdownScenario.interactable = value;
				scenarioType = ((!value) ? scenarioType : ScenarioType.Works);
			});
			togglesType[1].onValueChanged.AddListener(delegate(bool value)
			{
				dropdownScenarioMain[0].interactable = value;
				if (!value)
				{
					dropdownScenarioMain[1].interactable = false;
				}
				scenarioType = (value ? ScenarioType.Main : scenarioType);
			});
			togglesType[(int)scenarioType].isOn = true;
			buttonStart.onClick.AddListener(delegate
			{
				StartCoroutine(Wait());
				mapName = dropdownMap.options[dropdownMap.value].text;
				SaveData.Heroine heroine = new SaveData.Heroine(true);
				string[] array = new string[2] { "custom/presets_m_00.unity3d", "custom/presets_f_00.unity3d" };
				string[] array2 = new string[2] { "ill_Default_Male", "ill_Default_Female" };
				int num = 1;
				ChaFileControl chaFileControl = new ChaFileControl();
				chaFileControl.LoadFromAssetBundle(array[num], array2[num]);
				heroine.SetCharFile(chaFileControl);
				List<Program.Transfer> list = new List<Program.Transfer>();
				Program.SetParam(heroine, list);
				if (Singleton<Game>.IsInstance())
				{
					num = 0;
					chaFileControl = new ChaFileControl();
					chaFileControl.LoadFromAssetBundle(array[num], array2[num]);
					Singleton<Game>.Instance.Player.SetCharFile(chaFileControl);
					Program.SetParam(Singleton<Game>.Instance.Player, list);
				}
				list.Add(Program.Transfer.Create(true, Command.CameraLock, bool.TrueString));
				if (mapName != "なし")
				{
					list.Add(Program.Transfer.Create(true, Command.MapChange, mapName, bool.FalseString));
				}
				if (togglePlayer.isOn)
				{
					list.Add(Program.Transfer.Create(true, Command.CharaCreate, "-1", "-1"));
				}
				if (toggleHeroine.isOn)
				{
					list.Add(Program.Transfer.Create(true, Command.CharaCreate, "0", "-2"));
				}
				list.Add(Program.Transfer.Create(true, Command.CameraSetFov, "30"));
				list.Add(Program.Transfer.Open(assetBundle, Path.GetFileNameWithoutExtension(assetPath)));
				StartCoroutine(Program.Open(new Data
				{
					fadeInTime = 0f,
					position = Vector3.zero,
					rotation = Quaternion.identity,
					scene = this,
					transferList = list,
					heroineList = new List<SaveData.Heroine> { heroine }
				}));
				Save();
			});
		}
	}
}
