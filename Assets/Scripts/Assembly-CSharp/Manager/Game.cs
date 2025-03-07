using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ADV;
using Localize.Translate;
using UnityEngine;

namespace Manager
{
	public sealed class Game : Singleton<Game>
	{
		public class AnimePack
		{
			public RuntimeAnimatorController ac { get; private set; }

			public MotionIKData ikData { get; private set; }

			public string defaultName { get; private set; }

			public AnimePack(RuntimeAnimatorController ac, MotionIKData ikData, string defaultName)
			{
				this.ac = ac;
				this.ikData = ikData;
				this.defaultName = defaultName;
			}

			public void SetDefalut(ChaControl chaCtrl)
			{
				MotionIK motionIK;
				SetDefalut(chaCtrl, out motionIK);
			}

			public void SetDefalut(ChaControl chaCtrl, out MotionIK motionIK)
			{
				chaCtrl.animBody.runtimeAnimatorController = ac;
				chaCtrl.animBody.Play(defaultName);
				motionIK = new MotionIK(chaCtrl, ikData.Copy());
				motionIK.Calc(defaultName);
			}
		}

		public class MemorialChara
		{
			public int personality;

			public SaveData.Heroine heroine { get; private set; }

			public MemorialChara(SaveData.Heroine heroine)
			{
				this.heroine = heroine;
			}

			public void OverrideHeroine()
			{
				heroine.parameter.personality = personality;
			}
		}

		private class Initializable : InitializeSolution.IInitializable
		{
			private readonly Game game;

			private bool initialized;

			bool InitializeSolution.IInitializable.initialized
			{
				get
				{
					return initialized;
				}
			}

			public Initializable(Game game)
			{
				this.game = game;
			}

			void InitializeSolution.IInitializable.Initialize()
			{
				if (initialized)
				{
					return;
				}
				initialized = true;
				if (Localize.Translate.Manager.isTranslate)
				{
					Dictionary<int, Localize.Translate.Data.Param> category = Localize.Translate.Manager.GetCategory(Localize.Translate.Manager.SCENE_ID.SET_UP, 1);
					foreach (KeyValuePair<int, ClubInfo.Param> clubInfo in ClubInfos)
					{
						clubInfo.Value.Name = category.SafeGetText(clubInfo.Key) ?? clubInfo.Value.Name;
					}
				}
				List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath(Localize.Translate.Manager.AdvScenarioPath + "common_param/", true);
				assetBundleNameListFromPath.Sort();
				assetBundleNameListFromPath.ForEach(delegate(string file)
				{
					ScenarioData[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(ScenarioData)).GetAllAssets<ScenarioData>();
					foreach (ScenarioData scenarioData in allAssets)
					{
						game.scenarioParameterDic[scenarioData.name] = scenarioData.list;
					}
					AssetBundleManager.UnloadAssetBundle(file, false);
				});
			}
		}

		public class Expression
		{
			public class Pattern : ICloneable
			{
				public int ptn;

				public bool blend;

				public Pattern()
				{
					Initialize();
				}

				public Pattern(string arg, bool isThrow = false)
				{
					Initialize();
					if (arg.IsNullOrEmpty())
					{
						return;
					}
					string[] array = arg.Split(',');
					int num = 0;
					try
					{
						array.SafeProc(num++, delegate(string s)
						{
							ptn = int.Parse(s);
						});
						array.SafeProc(num++, delegate(string s)
						{
							blend = bool.Parse(s);
						});
					}
					catch (Exception)
					{
						if (isThrow)
						{
							throw new Exception("Expression Pattern:" + string.Join(",", array));
						}
					}
				}

				public void Initialize()
				{
					ptn = 0;
					blend = true;
				}

				public object Clone()
				{
					return MemberwiseClone();
				}
			}

			public Pattern eyebrow = new Pattern();

			public Pattern eyes = new Pattern();

			public Pattern mouth = new Pattern();

			public float eyebrowOpen = 1f;

			public float eyesOpen = 1f;

			public float mouthOpen = 1f;

			public int eyesLook;

			public float hohoAkaRate;

			public bool isHighlight = true;

			public int tearsLv;

			public bool isBlink = true;

			protected bool useEyebrow;

			protected bool useEyes;

			protected bool useMouth;

			protected bool useEyebrowOpen;

			protected bool useEyesOpen;

			protected bool useMouthOpen;

			protected bool useEyesLook;

			protected bool useHohoAkaRate;

			protected bool useIsHighlight;

			protected bool useTearsLv;

			protected bool useIsBlink;

			public bool isChangeSkip { private get; set; }

			public Expression()
			{
			}

			public Expression(Expression other)
			{
				Copy(other, this);
			}

			public Expression(string[] args, ref int cnt)
			{
				Initialize(args, ref cnt);
			}

			public Expression(string[] args)
			{
				int cnt = 0;
				Initialize(args, ref cnt);
			}

			public static void Copy(Expression src, Expression dest)
			{
				if (!src.eyebrow.SafeProc(delegate(Pattern p)
				{
					dest.eyebrow = (Pattern)p.Clone();
				}))
				{
					dest.eyebrow = new Pattern();
				}
				if (!src.eyes.SafeProc(delegate(Pattern p)
				{
					dest.eyes = (Pattern)p.Clone();
				}))
				{
					dest.eyes = new Pattern();
				}
				if (!src.mouth.SafeProc(delegate(Pattern p)
				{
					dest.mouth = (Pattern)p.Clone();
				}))
				{
					dest.mouth = new Pattern();
				}
				dest.eyebrowOpen = src.eyebrowOpen;
				dest.eyesOpen = src.eyesOpen;
				dest.mouthOpen = src.mouthOpen;
				dest.eyesLook = src.eyesLook;
				dest.hohoAkaRate = src.hohoAkaRate;
				dest.isHighlight = src.isHighlight;
				dest.tearsLv = src.tearsLv;
				dest.isBlink = src.isBlink;
				dest.isChangeSkip = src.isChangeSkip;
				dest.useEyebrow = src.useEyebrow;
				dest.useEyes = src.useEyes;
				dest.useMouth = src.useMouth;
				dest.useEyebrowOpen = src.useEyebrowOpen;
				dest.useEyesOpen = src.useEyesOpen;
				dest.useMouthOpen = src.useMouthOpen;
				dest.useEyesLook = src.useEyesLook;
				dest.useHohoAkaRate = src.useHohoAkaRate;
				dest.useIsHighlight = src.useIsHighlight;
				dest.useTearsLv = src.useTearsLv;
				dest.useIsBlink = src.useIsBlink;
			}

			public virtual void Initialize(string[] args, ref int cnt, bool isThrow = false)
			{
				try
				{
					useEyebrow = args.SafeProc(cnt++, delegate(string s)
					{
						eyebrow = new Pattern(s, true);
					});
					useEyes = args.SafeProc(cnt++, delegate(string s)
					{
						eyes = new Pattern(s, true);
					});
					useMouth = args.SafeProc(cnt++, delegate(string s)
					{
						mouth = new Pattern(s, true);
					});
					useEyebrowOpen = args.SafeProc(cnt++, delegate(string s)
					{
						eyebrowOpen = float.Parse(s);
					});
					useEyesOpen = args.SafeProc(cnt++, delegate(string s)
					{
						eyesOpen = float.Parse(s);
					});
					useMouthOpen = args.SafeProc(cnt++, delegate(string s)
					{
						mouthOpen = float.Parse(s);
					});
					useEyesLook = args.SafeProc(cnt++, delegate(string s)
					{
						eyesLook = int.Parse(s);
					});
					useHohoAkaRate = args.SafeProc(cnt++, delegate(string s)
					{
						hohoAkaRate = float.Parse(s);
					});
					useIsHighlight = args.SafeProc(cnt++, delegate(string s)
					{
						isHighlight = bool.Parse(s);
					});
					useTearsLv = args.SafeProc(cnt++, delegate(string s)
					{
						tearsLv = int.Parse(s);
					});
					useIsBlink = args.SafeProc(cnt++, delegate(string s)
					{
						isBlink = bool.Parse(s);
					});
				}
				catch (Exception)
				{
					if (isThrow)
					{
						throw new Exception("Expression:" + string.Join(",", args));
					}
				}
			}

			public void Change(ChaControl chaCtrl)
			{
				if (!isChangeSkip || useEyebrow)
				{
					chaCtrl.ChangeEyebrowPtn(eyebrow.ptn, eyebrow.blend);
				}
				if (!isChangeSkip || useEyes)
				{
					chaCtrl.ChangeEyesPtn(eyes.ptn, eyes.blend);
				}
				if (!isChangeSkip || useMouth)
				{
					chaCtrl.ChangeMouthPtn(mouth.ptn, mouth.blend);
					int ptn = mouth.ptn;
					bool disable = ((ptn == 21 || ptn == 22) ? true : false);
					chaCtrl.DisableShapeMouth(disable);
				}
				if (!isChangeSkip || useEyebrowOpen)
				{
					chaCtrl.ChangeEyebrowOpenMax(eyebrowOpen);
				}
				if (!isChangeSkip || useEyesOpen)
				{
					chaCtrl.ChangeEyesOpenMax(eyesOpen);
				}
				if (!isChangeSkip || useMouthOpen)
				{
					chaCtrl.ChangeMouthOpenMax(mouthOpen);
				}
				if ((!isChangeSkip || useEyesLook) && eyesLook != -1)
				{
					chaCtrl.ChangeLookEyesPtn(eyesLook);
				}
				if (!isChangeSkip || useHohoAkaRate)
				{
					chaCtrl.ChangeHohoAkaRate(hohoAkaRate);
				}
				if (!isChangeSkip || useIsHighlight)
				{
					chaCtrl.HideEyeHighlight(!isHighlight);
				}
				if (!isChangeSkip || useTearsLv)
				{
					chaCtrl.chaFile.status.tearsLv = (byte)tearsLv;
				}
				if (!isChangeSkip || useIsBlink)
				{
					chaCtrl.ChangeEyesBlinkFlag(isBlink);
				}
			}

			public void Copy(Expression dest)
			{
				Copy(this, dest);
			}
		}

		public static readonly Version Version = new Version(5, 1);

		private static bool? _isAdd20 = null;

		public const string SaveFileHeader = "file";

		public const string SaveFileExtension = ".dat";

		public static readonly Dictionary<string, int> ClassRoomNameIndexPair = new Dictionary<string, int>
		{
			{ "1-1", 0 },
			{ "2-1", 1 },
			{ "2-2", 2 },
			{ "3-1", 3 }
		};

		public static readonly ClubInfo.Param KoikatuClubParam = new ClubInfo.Param
		{
			ID = -1,
			Name = "恋活部",
			Place = "部室"
		};

		private static Dictionary<int, ClubInfo.Param> clubInfoDic = new Dictionary<int, ClubInfo.Param>();

		private CameraEffector _cameraEffector;

		private Camera _nowCamera;

		private bool isCameraChanged;

		private static string _SaveFileName = string.Empty;

		private const string AutoSaveFileName = "auto.dat";

		private Dictionary<int, string> heroinePersonalitys;

		public readonly Dictionary<string, List<ScenarioData.Param>> scenarioParameterDic = new Dictionary<string, List<ScenarioData.Param>>();

		public Dictionary<int, Dictionary<string, Expression>> expCharaDic = new Dictionary<int, Dictionary<string, Expression>>();

		public static bool isAdd20
		{
			get
			{
				bool? flag = _isAdd20;
				bool value;
				if (flag.HasValue)
				{
					value = flag.Value;
				}
				else
				{
					bool? flag2 = (_isAdd20 = AssetBundleCheck.IsManifest("add20"));
					value = flag2.Value;
				}
				return value;
			}
		}

		public static string SaveFileName
		{
			get
			{
				return _SaveFileName;
			}
			set
			{
				_SaveFileName = value;
			}
		}

		public static Dictionary<int, ClubInfo.Param> ClubInfos
		{
			get
			{
				return clubInfoDic;
			}
		}

		public static string[] SaveFiles
		{
			get
			{
				return Directory.Exists(SaveData.Path) ? Directory.GetFiles(SaveData.Path, "*.dat") : new string[0];
			}
		}

		public static bool IsLoadCheck
		{
			get
			{
				return !SaveFiles.IsNullOrEmpty();
			}
		}

		public GlobalSaveData glSaveData { get; private set; }

		public RankingData rankSaveData { get; private set; }

		public WeddingData weddingData { get; private set; }

		public TutorialData tutorialData { get; private set; }

		public SaveData saveData { get; private set; }

		public SaveData.Player Player
		{
			get
			{
				return saveData.player;
			}
		}

		public List<SaveData.Heroine> HeroineList
		{
			get
			{
				return saveData.heroineList;
			}
		}

		public ActionScene actScene { get; set; }

		public CameraEffector cameraEffector
		{
			get
			{
				Camera camera = nowCamera;
				if (camera == null)
				{
					return null;
				}
				if (isCameraChanged)
				{
					_cameraEffector = camera.GetComponent<CameraEffector>();
				}
				return _cameraEffector;
			}
		}

		public Camera nowCamera
		{
			get
			{
				if (_nowCamera != Camera.main)
				{
					_nowCamera = Camera.main;
					isCameraChanged = true;
				}
				return _nowCamera;
			}
		}

		public AnimePack advAnimePack { get; private set; }

		public Dictionary<int, string> HeroinePersonalitys
		{
			get
			{
				return this.GetCache(ref heroinePersonalitys, () => Singleton<Voice>.Instance.voiceInfoList.ToDictionary((VoiceInfo.Param v) => v.No, (VoiceInfo.Param v) => v.Personality));
			}
		}

		public static byte CharaDataToSex(SaveData.CharaData charaData)
		{
			return (!(charaData is SaveData.Player)) ? ((byte)1) : ((byte)0);
		}

		public static int SchoolYear(int schoolClass)
		{
			switch (schoolClass)
			{
			case 0:
				return 1;
			case 1:
			case 2:
				return 2;
			case 3:
				return 3;
			default:
				return 0;
			}
		}

		public static ClubInfo.Param GetClubInfo(SaveData.CharaData charaData, bool isOrigine)
		{
			if (charaData is SaveData.Player)
			{
				return KoikatuClubParam;
			}
			if (charaData.schoolClass < 0)
			{
				return new ClubInfo.Param();
			}
			if (!isOrigine)
			{
				SaveData.Heroine heroine = charaData as SaveData.Heroine;
				if (heroine != null && heroine.isStaff)
				{
					return KoikatuClubParam;
				}
			}
			ClubInfo.Param value;
			clubInfoDic.TryGetValue(charaData.clubActivities, out value);
			return value;
		}

		public static void LoadFromTextAsset(SaveData.Player player)
		{
			if (!player.charFileInitialized)
			{
				player.charFileInitialized = true;
				string path = "action/fixchara/00.unity3d";
				Localize.Translate.Manager.DefaultData.GetBundlePath(ref path);
				AssetBundleData assetBundleData = new AssetBundleData(path, "player");
				player.charFile.LoadFromTextAsset(assetBundleData.GetAsset<TextAsset>());
				player.ChaFileUpdate();
				assetBundleData.UnloadBundle();
			}
		}

		public static void LoadFromTextAsset(int fixID, SaveData.Heroine heroine, TextAsset ta)
		{
			if (!heroine.charFileInitialized)
			{
				heroine.charFileInitialized = true;
				heroine.charFile.LoadFromTextAsset(ta, true);
				heroine.ChaFileUpdate();
				heroine.fixCharaID = fixID;
			}
		}

		public static Dictionary<int, TextAsset> GetFixCharaTextAsset()
		{
			Dictionary<int, TextAsset> ret = new Dictionary<int, TextAsset>();
			List<string> assetBundleNameListFromPath = Localize.Translate.Manager.DefaultData.GetAssetBundleNameListFromPath("action/fixchara/", true);
			assetBundleNameListFromPath.Sort();
			assetBundleNameListFromPath.ForEach(delegate(string file)
			{
				TextAsset[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(TextAsset)).GetAllAssets<TextAsset>();
				foreach (TextAsset textAsset in allAssets)
				{
					int result;
					if (int.TryParse(textAsset.name.Replace("c", string.Empty), out result))
					{
						ret[result] = textAsset;
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			return ret;
		}

		public static List<SaveData.Heroine> CreateFixCharaList(params int[] removeFixCharaIDs)
		{
			List<SaveData.Heroine> list = (from p in GetFixCharaTextAsset()
				where !removeFixCharaIDs.Contains(p.Key)
				select p).Select(delegate(KeyValuePair<int, TextAsset> p)
			{
				SaveData.Heroine heroine = new SaveData.Heroine(false);
				LoadFromTextAsset(p.Key, heroine, p.Value);
				return heroine;
			}).ToList();
			list.Sort((SaveData.Heroine p1, SaveData.Heroine p2) => p2.fixCharaID.CompareTo(p1.fixCharaID));
			return list;
		}

		public bool IsRegulate(bool isAddSceneCheck)
		{
			bool flag = false;
			if (Singleton<Scene>.IsInstance())
			{
				flag |= Singleton<Scene>.Instance.IsNowLoadingFade;
				if (isAddSceneCheck)
				{
					flag |= !Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty();
				}
			}
			if (actScene != null && actScene.Map != null)
			{
				flag |= actScene.Map.isMapLoading;
			}
			return flag;
		}

		public void Save()
		{
			Save(SaveFileName);
		}

		public void Save(string fileName)
		{
			saveData.Save(fileName);
		}

		public void Load()
		{
			Load(SaveFileName);
		}

		public void Load(string fileName)
		{
			saveData.Load(fileName);
		}

		public void NewGame()
		{
			saveData = new SaveData();
			saveData.player.Randomize();
			actScene = null;
			SaveFileName = string.Empty;
		}

		public void ParameterCorrectValues()
		{
			int min = 0;
			int num = 100;
			SaveData.Player player = saveData.player;
			player.physical = Mathf.Clamp(player.physical, min, num);
			player.intellect = Mathf.Clamp(player.intellect, min, num);
			player.hentai = Mathf.Clamp(player.hentai, min, num);
			foreach (SaveData.Heroine heroine in saveData.heroineList)
			{
				heroine.favor = Mathf.Clamp(heroine.favor, min, num);
				heroine.lewdness = Mathf.Clamp(heroine.lewdness, min, num);
				heroine.intimacy = Mathf.Clamp(heroine.intimacy, min, heroine.isGirlfriend ? num : 20);
			}
		}

		protected override void Awake()
		{
			if (!CheckInstance())
			{
				return;
			}
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			glSaveData = new GlobalSaveData();
			rankSaveData = new RankingData();
			weddingData = new WeddingData();
			tutorialData = new TutorialData();
			SaveData.InitializeCall();
			glSaveData.Load();
			rankSaveData.Load();
			weddingData.Load();
			tutorialData.Load();
			foreach (int datum in tutorialData.data)
			{
				glSaveData.tutorialHash.Add(datum);
			}
			GameContentData.SetClubContents(glSaveData.clubContents);
			GameContentData.SetPlayHList(glSaveData.playHList);
			MainScenario.LoadReadInfo();
			List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/clubinfo/", true);
			assetBundleNameListFromPath.Sort();
			assetBundleNameListFromPath.ForEach(delegate(string file)
			{
				ClubInfo[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(ClubInfo)).GetAllAssets<ClubInfo>();
				foreach (ClubInfo clubInfo in allAssets)
				{
					foreach (ClubInfo.Param item in clubInfo.param)
					{
						clubInfoDic[item.ID] = item;
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			List<string> assetBundleNameListFromPath2 = CommonLib.GetAssetBundleNameListFromPath("etcetra/list/exp/", true);
			assetBundleNameListFromPath2.Sort();
			assetBundleNameListFromPath2.ForEach(delegate(string file)
			{
				ExcelData[] allAssets2 = AssetBundleManager.LoadAllAsset(file, typeof(ExcelData)).GetAllAssets<ExcelData>();
				foreach (ExcelData excelData in allAssets2)
				{
					int key = int.Parse(excelData.name.Replace("c", string.Empty));
					Dictionary<string, Expression> value;
					if (!expCharaDic.TryGetValue(key, out value))
					{
						value = new Dictionary<string, Expression>();
					}
					LoadExpExcelData(value, excelData);
					expCharaDic[key] = value;
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			Localize.Translate.Manager.initializeSolution.Add(new Initializable(this));
			NewGame();
		}

		private void OnApplicationQuit()
		{
			tutorialData.Save();
		}

		public static void LoadExpExcelData(Dictionary<string, Expression> dic, ExcelData excelData)
		{
			foreach (ExcelData.Param item in excelData.list)
			{
				if (item.list.Any())
				{
					Expression expression = new Expression(item.list.Skip(1).ToArray());
					expression.isChangeSkip = true;
					dic[item.list[0]] = expression;
				}
			}
		}

		public static Expression GetExpression(Dictionary<string, Expression> dic, string key)
		{
			Expression value;
			dic.TryGetValue(key, out value);
			return value;
		}

		public static Dictionary<string, Expression> ExpCharaListLoadData(int personality)
		{
			Dictionary<string, Expression> dic = new Dictionary<string, Expression>();
			CommonLib.GetAssetBundleNameListFromPath("etcetra/list/exp/", true).ForEach(delegate(string file)
			{
				ExcelData[] allAssets = AssetBundleManager.LoadAllAsset(file, typeof(ExcelData)).GetAllAssets<ExcelData>();
				foreach (ExcelData excelData in allAssets)
				{
					if (int.Parse(excelData.name.Replace("c", string.Empty)) == personality)
					{
						LoadExpExcelData(dic, excelData);
					}
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
			return dic;
		}

		public Expression GetExpression(int personality, string key)
		{
			Dictionary<string, Expression> value;
			if (!expCharaDic.TryGetValue(personality, out value))
			{
				return null;
			}
			return GetExpression(value, key);
		}
	}
}
