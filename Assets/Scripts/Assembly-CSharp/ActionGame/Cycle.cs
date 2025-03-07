using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame.Chara;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Localize.Translate;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame
{
	public class Cycle : MonoBehaviour
	{
		public enum Type
		{
			WakeUp = 0,
			Morning = 1,
			GotoSchool = 2,
			HR1 = 3,
			Lesson1 = 4,
			LunchTime = 5,
			Lesson2 = 6,
			HR2 = 7,
			StaffTime = 8,
			AfterSchool = 9,
			GotoMyHouse = 10,
			MyHouse = 11
		}

		public enum Week
		{
			Monday = 0,
			Tuesday = 1,
			Wednesday = 2,
			Thursday = 3,
			Friday = 4,
			Saturday = 5,
			Holiday = 6
		}

		private class TypeComparer : IEqualityComparer<Type>
		{
			public bool Equals(Type x, Type y)
			{
				return x == y;
			}

			public int GetHashCode(Type obj)
			{
				return (int)obj;
			}
		}

		private class MyRoomInHeroineEventCheck
		{
			public const string VariableH = "isH";

			public bool isH;

			public SaveData.Heroine heroine { get; private set; }

			public MyRoomInHeroineEventCheck(SaveData.Heroine heroine, bool isH)
			{
				this.heroine = heroine;
				this.isH = isH;
			}
		}

		private class CreateChara
		{
			public class Data
			{
				public int sex;

				public int coordinate = -1;

				public Data(int sex, int coordinate)
				{
					this.sex = sex;
					this.coordinate = coordinate;
				}

				public List<Program.Transfer> Create()
				{
					List<Program.Transfer> list = new List<Program.Transfer>();
					switch (sex)
					{
					case 0:
						list.Add(Program.Transfer.Create(false, Command.CharaCreate, "-1", "-1"));
						if (coordinate != -1)
						{
							list.Add(Program.Transfer.Create(false, Command.CharaCoordinate, "-1", Enum.GetNames(typeof(ChaFileDefine.CoordinateType))[coordinate]));
						}
						break;
					case 1:
						list.Add(Program.Transfer.Create(false, Command.CharaCreate, "0", "-2"));
						if (coordinate != -1)
						{
							list.Add(Program.Transfer.Create(false, Command.CharaCoordinate, "0", Enum.GetNames(typeof(ChaFileDefine.CoordinateType))[coordinate]));
						}
						break;
					}
					return list;
				}
			}

			private Data player { get; set; }

			private Data heroine { get; set; }

			public void CreateHeroine(int coordinate = -1)
			{
				heroine = new Data(1, coordinate);
			}

			public void CreatePlayer(int coordinate = -1)
			{
				player = new Data(0, coordinate);
			}

			public List<Program.Transfer> Create()
			{
				List<Program.Transfer> list = new List<Program.Transfer>();
				if (heroine != null)
				{
					list.AddRange(heroine.Create());
				}
				if (player != null)
				{
					list.AddRange(player.Create());
				}
				return list;
			}
		}

		private Dictionary<Type, Func<CancellationToken, IEnumerator>> typeDic = new Dictionary<Type, Func<CancellationToken, IEnumerator>>(new TypeComparer());

		private bool isOpening;

		public const string ClassScheduleDefaultName = "教室";

		[SerializeField]
		private Canvas _timeCanvas;

		[SerializeField]
		private TimeUIControl timeCtrl;

		[SerializeField]
		private TimeZoenCutIn _timeZoenCutIn;

		private float _timer;

		public const int TIME_LIMIT = 500;

		public const int EVENT_LIMIT = 499;

		private SaveData.Heroine _withHeroine;

		private SaveData.Heroine _dateHeroine;

		private MyRoomInHeroineEventCheck myRoomInHeroineEventCheck;

		private ActionScene _actScene;

		private IDisposable typeChangeDisposable;

		private IDisposable nowCycleDisposable;

		private SaveData.Heroine gotoSchoolRemoveChara;

		private static readonly string[] _wakeUpWeekdayWords = new string[6] { "「今日の運勢はバッチリ！\u3000良い一日になりそうだ」", "「よし、今日も積極的に行こう！」", "「さてと、今日もコイカツしちゃいますか！」", "「今日も良い日になりそうだ」", "「よしっ！\u3000今日も一日がんばりますか」", "「快調快調、今日も元気にいってみよう！」" };

		private static readonly string[] _wakeUpHolidayWords = new string[2] { "「今日は家事に精を出すかな」", "「誰が来ても良いように掃除しておこう」" };

		private static readonly string[] _wakeUpDateWords = new string[1] { "「今日はあの子とデートする日だ」" };

		private static readonly string[] _wakeUpSaturdayWords = new string[2] { "「今日は私服の女の子と会えるかも」", "「今日は誰か学園に登校してるかな」" };

		public const string ADVVariablePatternH = "patternH";

		public const string ADVVariableCycle = "cycle";

		public const string ADVVariableKokanForceInsert = "KokanForceInsert";

		public Dictionary<string, Func<string>> ADVVariables { get; private set; }

		public Dictionary<int, Dictionary<Week, string[]>> classScheduleDic { get; private set; }

		public bool timerVisible
		{
			get
			{
				return _timeCanvas.enabled;
			}
			set
			{
				_timeCanvas.enabled = value;
			}
		}

		public Type nowType
		{
			get
			{
				return _nowType;
			}
		}

		public Week nowWeek
		{
			get
			{
				return _nowWeek;
			}
		}

		private Type _nowType { get; set; }

		private Week _nowWeek { get; set; }

		public bool isActionEnd
		{
			get
			{
				return _timer >= 500f;
			}
		}

		public float timer
		{
			get
			{
				return _timer;
			}
		}

		public Dictionary<int, List<FixEventSchedule.Param>> fixEventSchedule { get; private set; }

		public bool isAction
		{
			get
			{
				Type type = nowType;
				if (type == Type.LunchTime || type == Type.StaffTime || type == Type.AfterSchool)
				{
					return true;
				}
				return false;
			}
		}

		public SaveData.Heroine withHeroine
		{
			get
			{
				return this.GetCache(ref _withHeroine, () => Singleton<Game>.Instance.saveData.withHeroine.Get(Singleton<Game>.Instance.HeroineList));
			}
			set
			{
				_withHeroine = value;
				Singleton<Game>.Instance.saveData.withHeroine.Set(_withHeroine);
			}
		}

		public SaveData.Heroine dateHeroine
		{
			get
			{
				return this.GetCache(ref _dateHeroine, () => Singleton<Game>.Instance.saveData.dateHeroine.Get(Singleton<Game>.Instance.HeroineList));
			}
			set
			{
				_dateHeroine = value;
				Singleton<Game>.Instance.saveData.dateHeroine.Set(_dateHeroine);
			}
		}

		private bool isShufflePoped { get; set; }

		private ActionScene actScene
		{
			get
			{
				return this.GetComponentCache(ref _actScene);
			}
		}

		private bool isNextCall { get; set; }

		private Texture2D startTexture
		{
			get
			{
				int index = -1;
				switch (nowType)
				{
				case Type.LunchTime:
					index = ((nowWeek == Week.Saturday) ? 3 : 0);
					break;
				case Type.StaffTime:
					index = 1;
					break;
				case Type.AfterSchool:
					index = ((nowWeek != Week.Saturday) ? 2 : 4);
					break;
				}
				Localize.Translate.Data.Param param = actScene.uiTranslater.Get(2).Values.FindTags("CycleStart").ToArray().SafeGet(index);
				return (param != null) ? param.Load<Texture2D>(true) : null;
			}
		}

		private string[] wakeUpWeekdayWords
		{
			get
			{
				string[] words = GetWords("WakeUp");
				return (!words.Any()) ? _wakeUpWeekdayWords : words;
			}
		}

		private string[] wakeUpHolidayWords
		{
			get
			{
				string[] words = GetWords("Holiday");
				return (!words.Any()) ? _wakeUpHolidayWords : words;
			}
		}

		private string[] wakeUpDateWords
		{
			get
			{
				string[] words = GetWords("Date");
				return (!words.Any()) ? _wakeUpDateWords : words;
			}
		}

		private string[] wakeUpSaturdayWords
		{
			get
			{
				string[] words = GetWords("Saturday");
				return (!words.Any()) ? _wakeUpSaturdayWords : words;
			}
		}

		public static Type ConvertType(string type)
		{
			foreach (Type item in Illusion.Utils.Enum<Type>.Enumerate())
			{
				if (item.GetName() == type)
				{
					return item;
				}
			}
			return Illusion.Utils.Enum<Type>.Cast(type);
		}

		public static Week ConvertWeek(string week)
		{
			foreach (Week item in Illusion.Utils.Enum<Week>.Enumerate())
			{
				if (item.GetName() == week)
				{
					return item;
				}
			}
			return Illusion.Utils.Enum<Week>.Cast(week);
		}

		public void Initialize()
		{
			isOpening = Singleton<Game>.Instance.saveData.isOpening;
			Type type = ((!isOpening) ? Type.MyHouse : Type.WakeUp);
			typeDic.Add(Type.WakeUp, WakeUp);
			typeDic.Add(Type.Morning, Morning);
			typeDic.Add(Type.GotoSchool, GotoSchool);
			typeDic.Add(Type.HR1, HR1);
			typeDic.Add(Type.Lesson1, Lesson1);
			typeDic.Add(Type.LunchTime, LunchTime);
			typeDic.Add(Type.Lesson2, Lesson2);
			typeDic.Add(Type.HR2, HR2);
			typeDic.Add(Type.StaffTime, StaffTime);
			typeDic.Add(Type.AfterSchool, AfterSchool);
			typeDic.Add(Type.GotoMyHouse, GotoMyHouse);
			typeDic.Add(Type.MyHouse, MyHouse);
			int week = Singleton<Game>.Instance.saveData.week;
			_nowWeek = Illusion.Utils.Enum<Week>.Cast(week);
			_nowType = type;
			timeCtrl.dayOfTheWeek = week;
			timeCtrl.timeZone = TimeZoonCast(type);
			classScheduleDic = new Dictionary<int, Dictionary<Week, string[]>>();
			AssetBundleLoadAssetOperation assetBundleLoadAssetOperation = AssetBundleManager.LoadAllAsset("action/list/classschedule.unity3d", typeof(ClassSchedule));
			ClassSchedule[] allAssets = assetBundleLoadAssetOperation.GetAllAssets<ClassSchedule>();
			foreach (ClassSchedule classSchedule in allAssets)
			{
				Dictionary<Week, string[]> dictionary = new Dictionary<Week, string[]>();
				foreach (ClassSchedule.Param item in classSchedule.param)
				{
					dictionary[Illusion.Utils.Enum<Week>.Cast(item.Week)] = new string[2] { item.Lesson1, item.Lesson2 }.Select((string s) => (!s.IsNullOrWhiteSpace()) ? s : "教室").ToArray();
				}
				classScheduleDic[Game.ClassRoomNameIndexPair[classSchedule.name]] = dictionary;
			}
			AssetBundleManager.UnloadAssetBundle("action/list/classschedule.unity3d", false);
			fixEventSchedule = new Dictionary<int, List<FixEventSchedule.Param>>();
			CommonLib.GetAssetBundleNameListFromPath("action/list/fixevent/", true).ForEach(delegate(string file)
			{
				FixEventSchedule[] allAssets2 = AssetBundleManager.LoadAllAsset(file, typeof(FixEventSchedule)).GetAllAssets<FixEventSchedule>();
				foreach (FixEventSchedule fixEventSchedule in allAssets2)
				{
					int key = int.Parse(fixEventSchedule.name);
					List<FixEventSchedule.Param> value;
					if (!this.fixEventSchedule.TryGetValue(key, out value))
					{
						value = (this.fixEventSchedule[key] = new List<FixEventSchedule.Param>());
					}
					value.AddRange(fixEventSchedule.param);
				}
				AssetBundleManager.UnloadAssetBundle(file, false);
			});
		}

		public void ActionEnd()
		{
			_timer = 500f;
		}

		public void AddTimer(float value)
		{
			_timer = Mathf.Clamp(_timer + 500f * value, 0f, 500f);
		}

		public static string GetClassRoomName(int schoolClass)
		{
			foreach (KeyValuePair<string, int> item in Game.ClassRoomNameIndexPair)
			{
				if (item.Value == schoolClass)
				{
					return item.Key;
				}
			}
			return string.Empty;
		}

		public static string GetClassRoomMapName(int schoolClass)
		{
			return "教室" + GetClassRoomName(schoolClass);
		}

		public int GetClassRoomMapNo(int schoolClass)
		{
			return actScene.Map.ConvertMapNo(GetClassRoomMapName(schoolClass));
		}

		public string[] GetNowLessones(int schoolClass)
		{
			string[] value = null;
			Dictionary<Week, string[]> value2;
			if (classScheduleDic.TryGetValue(schoolClass, out value2))
			{
				value2.TryGetValue(nowWeek, out value);
			}
			return value;
		}

		public string[] GetNowLessonesRename(int schoolClass)
		{
			return (from lesson in GetNowLessones(schoolClass)
				select LessonRename(lesson, schoolClass)).ToArray();
		}

		public static string LessonRename(string lesson, int schoolClass)
		{
			return (!(lesson != "教室")) ? GetClassRoomMapName(schoolClass) : lesson;
		}

		private int TimeZoonCast(Type type)
		{
			switch (type)
			{
			case Type.WakeUp:
			case Type.Morning:
			case Type.GotoSchool:
			case Type.HR1:
			case Type.Lesson1:
				return 0;
			case Type.LunchTime:
			case Type.Lesson2:
			case Type.HR2:
				return 1;
			case Type.StaffTime:
				return 2;
			case Type.AfterSchool:
			case Type.GotoMyHouse:
				return 3;
			case Type.MyHouse:
				return 4;
			default:
				return -1;
			}
		}

		public void Change(Type type)
		{
			if (!isNextCall && nowType > type)
			{
				NextWeek();
			}
			_nowType = type;
			timeCtrl.timeZone = TimeZoonCast(type);
			isNextCall = false;
			if (actScene.Map.no != -1)
			{
				actScene.Map.SetCycleToSunLight();
			}
			if (typeChangeDisposable != null)
			{
				typeChangeDisposable.Dispose();
			}
			typeChangeDisposable = Observable.FromCoroutine((CancellationToken _) => Program.ADVProcessingCheck()).Subscribe(delegate
			{
				if (nowCycleDisposable != null)
				{
					nowCycleDisposable.Dispose();
				}
				nowCycleDisposable = Observable.FromCoroutine(typeDic[nowType]).Subscribe();
				actScene.MiniMapAndCameraActive = isAction;
			});
		}

		public void Change(Week week)
		{
			int length = Illusion.Utils.Enum<Week>.Length;
			int cnt = 0;
			int num = (int)_nowWeek;
			while (num != (int)week)
			{
				num = ++num % length;
				cnt++;
			}
			if (cnt == 0)
			{
				cnt = length;
			}
			_nowWeek = week;
			timeCtrl.dayOfTheWeek = (int)week;
			Singleton<Game>.Instance.saveData.week = (int)week;
			Singleton<Game>.Instance.HeroineList.ForEach(delegate(SaveData.Heroine p)
			{
				for (int i = 0; i < cnt; i++)
				{
					p.AddMenstruationsDay();
				}
				if (p.fixCharaID <= -5)
				{
					p.EventAfterDayAdd(cnt);
				}
			});
		}

		public void NextWeek()
		{
			NextWeek(1);
		}

		public void NextWeek(int plus)
		{
			int num = (int)nowWeek;
			num = (num + plus) % Illusion.Utils.Enum<Week>.Length;
			Change((Week)num);
		}

		public void Next()
		{
			StartCoroutine(_Next());
		}

		private IEnumerator _Next()
		{
			if (!Scene.isReturnTitle && !Scene.isGameEnd)
			{
				actScene.SetAllCharaActive(false);
				actScene.ShortcutKeyEnable(false);
				yield return new WaitUntil(() => Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty());
				yield return new WaitWhile(() => actScene.Map.isMapLoading);
				if (Singleton<Scene>.Instance.sceneFade._Fade == SimpleFade.Fade.Out)
				{
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
				}
				actScene.ShortcutKeyEnable(true);
				int next = (int)nowType;
				next = (next + 1) % Illusion.Utils.Enum<Type>.Length;
				isNextCall = true;
				Change((Type)next);
				PlayerAction.isNextStatic = false;
			}
		}

		private void Awake()
		{
			ADVVariables = new Dictionary<string, Func<string>>
			{
				{
					"NowMap",
					() => actScene.Map.no.ToString()
				},
				{
					"NowMapName",
					() => (actScene.Map.no != -1) ? actScene.Map.ConvertMapName(actScene.Map.no) : "なし"
				},
				{
					"NowCycle",
					() => nowType.ToString()
				},
				{
					"NowCycleName",
					() => nowType.GetName()
				},
				{
					"NowWeek",
					() => nowWeek.ToString()
				},
				{
					"NowWeekName",
					() => nowWeek.GetName()
				}
			};
		}

		private void OnDestroy()
		{
			if (typeChangeDisposable != null)
			{
				typeChangeDisposable.Dispose();
			}
			if (nowCycleDisposable != null)
			{
				nowCycleDisposable.Dispose();
			}
		}

		private static void ChimeSE()
		{
			Illusion.Game.Utils.Sound.Setting setting = new Illusion.Game.Utils.Sound.Setting(Manager.Sound.Type.GameSE2D);
			setting.assetName = "se_school_chime";
			Illusion.Game.Utils.Sound.Play(setting);
		}

		private IEnumerator MapChangeFadeIn(string mapName)
		{
			if (Scene.isReturnTitle || Scene.isGameEnd)
			{
				yield break;
			}
			int no = actScene.Map.ConvertMapNo(mapName);
			if (no == actScene.Map.no)
			{
				if (Singleton<Scene>.Instance.sceneFade._Fade == SimpleFade.Fade.Out)
				{
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
				}
			}
			else
			{
				actScene.Map.Change(no, Illusion.Game.Utils.Scene.SafeFadeIn());
				yield return new WaitWhile(() => actScene.Map.isMapLoading);
			}
		}

		private IEnumerator EventADVStart(int advNo, SaveData.Heroine heroine, CreateChara createChara, List<Program.Transfer> addList = null)
		{
			timerVisible = false;
			Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
			if (fadeType == Scene.Data.FadeType.In)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
			actScene.AdvScene.Scenario.BackCamera.fieldOfView = int.Parse("23");
			bool isOpenADV = false;
			yield return StartCoroutine(Program.Open(new ADV.Data
			{
				fadeInTime = 0f,
				position = Vector3.zero,
				rotation = Quaternion.identity,
				camera = null,
				heroineList = new List<SaveData.Heroine> { heroine },
				scene = actScene,
				transferList = EventADV(advNo, heroine, createChara, addList)
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.Wait(string.Empty);
			heroine.talkEvent.Add(advNo);
			Singleton<Scene>.Instance.SetFadeColorDefault();
		}

		private List<Program.Transfer> EventADV(int advNo, SaveData.Heroine heroine, CreateChara createChara, List<Program.Transfer> addList)
		{
			List<Program.Transfer> list = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, heroine, list);
			list.Add(Program.Transfer.Create(false, Command.CameraSetFov, "23"));
			list.AddRange(createChara.Create());
			if (!addList.IsNullOrEmpty())
			{
				list.AddRange(addList);
			}
			list.Add(Program.Transfer.Open(Program.FindADVBundleFilePath(advNo, heroine), advNo.ToString(), bool.TrueString));
			return list;
		}

		private IEnumerator FixCharaEventStart(SaveData.Heroine fixHeroine, FixEventScheduler.Result result)
		{
			timerVisible = false;
			Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
			if (fadeType == Scene.Data.FadeType.In)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
			bool isOpenADV = false;
			FixEventSchedule.Param param = result.param;
			List<Program.Transfer> transferList = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, fixHeroine, transferList);
			transferList.Add(Program.Transfer.Create(false, Command.CameraSetFov, "23"));
			transferList.Add(Program.Transfer.Open(param.Bundle, param.Asset, bool.TrueString));
			yield return StartCoroutine(Program.Open(new ADV.Data
			{
				fadeInTime = 0f,
				position = Vector3.zero,
				rotation = Quaternion.identity,
				camera = null,
				heroineList = new List<SaveData.Heroine> { fixHeroine },
				scene = actScene,
				transferList = transferList
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.Wait(string.Empty);
			fixHeroine.eventAfterDay = 0;
			Singleton<Scene>.Instance.SetFadeColorDefault();
			if (!Scene.isReturnTitle && !Scene.isGameEnd && Singleton<Scene>.Instance.sceneFade._Fade == SimpleFade.Fade.Out)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
		}

		private IEnumerator MapMove(CancellationToken cancel)
		{
			timerVisible = true;
			BoolReactiveProperty visible = new BoolReactiveProperty(true);
			visible.Subscribe(delegate(bool isOn)
			{
				timerVisible = isOn;
			});
			_timer = 0f;
			timeCtrl.timePass = 0f;
			yield return Observable.FromCoroutine((CancellationToken _) => actScene.NPCLoadAll(!isShufflePoped)).StartAsCoroutine(cancel);
			if (isOpening)
			{
				isOpening = false;
				if (Illusion.Game.Utils.Scene.OpenTutorial(0))
				{
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
					yield return new WaitWhile(Illusion.Game.Utils.Scene.IsTutorial);
					if (Scene.isReturnTitle || Scene.isGameEnd)
					{
						yield break;
					}
					Singleton<Scene>.Instance.sceneFade.FadeSet(SimpleFade.Fade.In);
					Singleton<Scene>.Instance.sceneFade.ForceEnd();
				}
			}
			isShufflePoped = true;
			actScene.Player.HitGateReset();
			Transform wpt = actScene.Map.warpPointTransform;
			if (wpt != null)
			{
				actScene.Player.SetPositionAndRotation(wpt);
				actScene.CameraState.SetAngle(wpt.eulerAngles);
			}
			actScene.Player.ChangeNowCoordinate();
			actScene.SetAllCharaActive(true);
			foreach (Transform item in (from p in actScene.npcList.OfType<Base>().Concat(new Base[2] { actScene.Player, actScene.fixChara })
				where p != null && p.initialized
				select p.chaCtrl into chaCtrl
				where chaCtrl != null && chaCtrl.loadEnd
				select chaCtrl).SelectMany((ChaControl chaCtrl) => new Transform[2]
			{
				chaCtrl.transform,
				chaCtrl.objTop.transform
			}))
			{
				item.localPosition = Vector3.zero;
				item.localEulerAngles = Vector3.zero;
			}
			yield return Observable.FromCoroutine((CancellationToken _) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out)).StartAsCoroutine(cancel);
			_timeZoenCutIn.FadeStart(nowType, startTexture);
			_timeCanvas.enabled = true;
			do
			{
				if (actScene.isCursorLock && !Singleton<Game>.Instance.IsRegulate(true))
				{
					_timer += Time.deltaTime;
				}
				timeCtrl.timePass = _timer / 500f;
				bool result = true;
				if (Program.isADVProcessing || Singleton<Scene>.Instance.NowSceneNames.Any((string s) => s == "Talk" || s == "H"))
				{
					result = false;
				}
				visible.Value = result;
				yield return null;
			}
			while (!isActionEnd);
			timeCtrl.timePass = 1f;
			_timeZoenCutIn.Stop();
			visible.Value = true;
			actScene.SetAllCharaActive(false);
			yield return new WaitUntil(() => Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty());
			yield return new WaitWhile(() => actScene.Map.isMapLoading);
		}

		private string[] GetWords(string tag)
		{
			return actScene.uiTranslater.Get(6).Values.ToArray(tag);
		}

		private IEnumerator WakeUp(CancellationToken cancel)
		{
			myRoomInHeroineEventCheck = null;
			timerVisible = true;
			string mapName = "自室";
			actScene.SetAllCharaActive(false);
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MorningMenu));
			bool isNext = true;
			yield return StartCoroutine(WakeUpEvent(mapName, delegate(bool b)
			{
				isNext = b;
			}));
			if (isNext)
			{
				Next();
			}
		}

		private IEnumerator WakeUpEvent(string mapName, Action<bool> isNext)
		{
			bool isEvent = false;
			gotoSchoolRemoveChara = withHeroine;
			if (withHeroine != null)
			{
				isEvent = true;
				yield return StartCoroutine(WakeUpWithHeroineEvent(mapName, withHeroine));
			}
			withHeroine = null;
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				isEvent = true;
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else if (nowWeek == Week.Holiday)
			{
				SaveData.Heroine dateChara = dateHeroine;
				if (dateChara != null && !dateChara.isDate)
				{
					dateHeroine = null;
					dateChara = null;
				}
				if (dateChara == null)
				{
					dateChara = Singleton<Game>.Instance.HeroineList.Shuffle().FirstOrDefault((SaveData.Heroine p) => p.isDate);
				}
				if (dateChara != null)
				{
					bool isSkipCycle = false;
					string bundle = "action/menu/mapselectdatemenu.unity3d";
					if (!Game.isAdd20 || Manager.Config.AddData.DateMapSelectNoneEvent || !AssetBundleCheck.IsFile(bundle, string.Empty) || !dateChara.isGirlfriend || !dateChara.talkEvent.Contains(57))
					{
						yield return StartCoroutine(WakeUpMonologueEvent(mapName, Tuple.Create("[P名]", wakeUpDateWords.Shuffle().First())));
						yield return StartCoroutine(WakeUpHolidayEvent(mapName, dateChara));
					}
					else
					{
						Type backup = _nowType;
						yield return StartCoroutine(WakeUpHolidayMapSelectEvent(bundle, "MapSelectDateMenu", dateChara));
						_nowType = backup;
						isSkipCycle = true;
					}
					isEvent = true;
					if (isSkipCycle || myRoomInHeroineEventCheck != null)
					{
						Change(Type.GotoMyHouse);
						isNext(false);
						yield break;
					}
				}
			}
			if (!Scene.isReturnTitle && !Scene.isGameEnd && !isEvent)
			{
				string word = ((nowWeek == Week.Holiday) ? wakeUpHolidayWords.Shuffle().First() : ((nowWeek != Week.Saturday) ? wakeUpWeekdayWords.Shuffle().First() : wakeUpSaturdayWords.Shuffle().First()));
				yield return StartCoroutine(WakeUpMonologueEvent(mapName, Tuple.Create("[P名]", word)));
			}
		}

		private IEnumerator WakeUpHolidayEvent(string mapName, SaveData.Heroine dateChara)
		{
			Singleton<Game>.Instance.rankSaveData.dateCount++;
			int advNo = 57;
			if (dateChara.talkEvent.Contains(advNo))
			{
				switch (dateChara.relation)
				{
				case 1:
					advNo = 58;
					break;
				case 2:
					advNo = 61;
					break;
				}
			}
			CreateChara createChara = new CreateChara();
			createChara.CreateHeroine(5);
			yield return StartCoroutine(EventADVStart(advNo, dateChara, createChara));
			bool isOpenH = false;
			ValData valData;
			if (actScene.AdvScene.Scenario.Vars.TryGetValue("isH", out valData))
			{
				isOpenH = (bool)valData.o;
			}
			if (isOpenH)
			{
				myRoomInHeroineEventCheck = new MyRoomInHeroineEventCheck(dateChara, true);
			}
			foreach (SaveData.Heroine item in Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine p) => p.isDate))
			{
				item.isDate = false;
				if (item != dateChara)
				{
					item.isAnger = true;
					item.anger += 100;
					item.favor = Mathf.Max(0, item.favor - 30);
				}
			}
			dateHeroine = null;
		}

		private IEnumerator WakeUpWithHeroineEvent(string mapName, SaveData.Heroine heroine)
		{
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			int advNo = 17;
			advNo = ((nowWeek == Week.Holiday) ? 16 : ((nowWeek == Week.Saturday) ? new int[3] { 14, 15, 16 }.Shuffle().First() : ((!heroine.talkEvent.Contains(17)) ? 17 : ((!heroine.talkEvent.Contains(15)) ? 15 : ((!heroine.talkEvent.Contains(14)) ? 14 : (heroine.talkEvent.Contains(16) ? new int[3] { 14, 15, 16 }.Shuffle().First() : 16))))));
			CreateChara createChara = new CreateChara();
			int coordinate = 6;
			createChara.CreateHeroine(coordinate);
			if (advNo == 14 && heroine.personality == 7)
			{
				createChara.CreatePlayer(coordinate);
			}
			else if (advNo == 16)
			{
				createChara.CreatePlayer(coordinate);
			}
			yield return StartCoroutine(EventADVStart(advNo, heroine, createChara));
			if (advNo == 16)
			{
				string nullName = null;
				ValData valData;
				if (actScene.AdvScene.Scenario.Vars.TryGetValue("hPos", out valData))
				{
					nullName = (string)valData.o;
				}
				int nullNo = 0;
				if (actScene.AdvScene.Scenario.Vars.TryGetValue("nullNo", out valData))
				{
					nullNo = (int)valData.o;
				}
				Vector3 pos;
				Quaternion rot;
				Program.GetNull(nullNo, mapName, nullName, actScene.Map, out pos, out rot);
				Cycle cycle = this;
				ActionScene actionScene = actScene;
				Vector3 position = pos;
				Quaternion rotation = rot;
				int appoint = 0;
				Tuple.Create(coordinate, heroine);
				yield return cycle.StartCoroutine(actionScene.ChangeHEvent(position, rotation, appoint, false, Tuple.Create(coordinate, heroine)));
			}
		}

		private IEnumerator WakeUpMonologueEvent(string mapName, params Tuple<string, string>[] words)
		{
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			List<Program.Transfer> transferList = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, transferList);
			transferList.Add(Program.Transfer.Create(false, Command.NullLoad, "0", mapName));
			transferList.Add(Program.Transfer.Create(false, Command.NullSet, "advPos_NoHeroine", "Camera"));
			transferList.Add(Program.Transfer.Create(false, Command.SceneFade, "out", "1"));
			for (int i = 0; i < words.Length; i++)
			{
				Tuple<string, string> tuple = words[i];
				transferList.Add(Program.Transfer.Text(tuple.Item1, tuple.Item2));
			}
			transferList.Add(Program.Transfer.Close());
			bool isOpenADV = false;
			yield return StartCoroutine(Program.Open(new ADV.Data
			{
				scene = actScene,
				camera = new OpenData.CameraData(),
				transferList = transferList
			}, new Program.OpenDataProc
			{
				onLoad = delegate
				{
					isOpenADV = true;
				}
			}));
			yield return new WaitUntil(() => isOpenADV);
			yield return Program.ADVProcessingCheck();
		}

		private void SetADVCycle(ref string cycle)
		{
			ValData value;
			if (!actScene.AdvScene.Scenario.Vars.TryGetValue("cycle", out value))
			{
				return;
			}
			cycle = (string)value.o;
			if (cycle != null)
			{
				if (!(cycle == "昼"))
				{
					if (!(cycle == "夕方"))
					{
						if (cycle == "夜")
						{
							ActionMap map = actScene.Map;
							Type nowCycle = (_nowType = Type.MyHouse);
							map.nowCycle = nowCycle;
						}
					}
					else
					{
						ActionMap map2 = actScene.Map;
						Type nowCycle = (_nowType = Type.GotoMyHouse);
						map2.nowCycle = nowCycle;
					}
				}
				else
				{
					ActionMap map3 = actScene.Map;
					Type nowCycle = (_nowType = Type.WakeUp);
					map3.nowCycle = nowCycle;
				}
			}
			actScene.Map.SetCycleToSunLight();
		}

		private IEnumerator WakeUpHolidayMapSelectEvent(string sceneBundle, string levelName, SaveData.Heroine dateChara)
		{
			CreateChara createChara = new CreateChara();
			createChara.CreateHeroine(5);
			yield return StartCoroutine(EventADVStart(dateChara.GetShuffleEventNo(501, 502), dateChara, createChara));
			int? advNo = null;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				assetBundleName = sceneBundle,
				levelName = levelName,
				isAdd = true,
				onLoad = delegate
				{
					MapSelectDateMenuScene rootComponent = Scene.GetRootComponent<MapSelectDateMenuScene>(levelName);
					if (!(rootComponent == null))
					{
						rootComponent.target = dateChara;
						rootComponent.result.Subscribe(delegate(int no)
						{
							advNo = no;
						});
					}
				}
			}, false);
			yield return new WaitUntil(() => advNo.HasValue);
			Singleton<Scene>.Instance.UnLoad();
			if (Scene.isReturnTitle || Scene.isGameEnd)
			{
				yield break;
			}
			Singleton<Game>.Instance.rankSaveData.dateCount++;
			yield return StartCoroutine(EventADVStart(advNo.Value, dateChara, createChara));
			yield return StartCoroutine(PatternHEventStart(dateChara, createChara));
			foreach (SaveData.Heroine item in Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine p) => p.isDate))
			{
				item.isDate = false;
				if (item != dateChara)
				{
					item.isAnger = true;
					item.anger += 100;
					item.favor = Mathf.Max(0, item.favor - 30);
				}
			}
			dateHeroine = null;
		}

		private IEnumerator PatternHEventStart(SaveData.Heroine dateChara, CreateChara createChara)
		{
			string cycle = null;
			SetADVCycle(ref cycle);
			int? patternH = null;
			ValData value;
			if (actScene.AdvScene.Scenario.Vars.TryGetValue("patternH", out value))
			{
				patternH = (int)value.o;
			}
			bool isStation = actScene.AdvScene.Scenario.LoadAssetName != "500";
			string mapName = null;
			int appoint = -1;
			bool isH = false;
			if (patternH.HasValue && patternH.HasValue)
			{
				switch (patternH.Value)
				{
				case 0:
					myRoomInHeroineEventCheck = new MyRoomInHeroineEventCheck(dateChara, true);
					isStation = false;
					break;
				case 100:
					mapName = "公園";
					isH = true;
					appoint = 7;
					break;
				case 252:
				case 253:
					mapName = "満員電車";
					isH = true;
					appoint = ((patternH != 252) ? 9 : 8);
					break;
				case 300:
					mapName = "カラオケ";
					isH = true;
					appoint = 10;
					break;
				case 600:
					mapName = "ラブホテル";
					isStation = false;
					isH = true;
					break;
				}
			}
			if (isH)
			{
				bool isKokanForceInsert = false;
				ValData value2;
				if (actScene.AdvScene.Scenario.Vars.TryGetValue("KokanForceInsert", out value2))
				{
					isKokanForceInsert = (bool)value2.o;
				}
				mapName = mapName ?? "自室";
				yield return StartCoroutine(HolidayHEvent(mapName, appoint, isKokanForceInsert, dateChara));
				SetADVCycle(ref cycle);
			}
			if (isStation)
			{
				yield return StartCoroutine(StationSplitEvent(cycle, dateChara, createChara));
				yield return StartCoroutine(PatternHEventStart(dateChara, createChara));
			}
		}

		private IEnumerator StationSplitEvent(string cycle, SaveData.Heroine dateChara, CreateChara createChara)
		{
			if (!Scene.isReturnTitle && !Scene.isGameEnd)
			{
				Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
				if (fadeType == Scene.Data.FadeType.In)
				{
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
				}
				List<Program.Transfer> addList = new List<Program.Transfer> { Program.Transfer.VAR("string", "cycle", cycle ?? "昼") };
				int advNo = 500;
				yield return StartCoroutine(EventADVStart(advNo, dateChara, createChara, addList));
			}
		}

		private IEnumerator HolidayHEvent(string mapName, int appoint, bool isKokanForceInsert, SaveData.Heroine heroine)
		{
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			Vector3 pos = Vector3.zero;
			Quaternion rot = Quaternion.identity;
			bool useNull = false;
			Dictionary<string, ValData> vars = actScene.AdvScene.Scenario.Vars;
			ValData value;
			if (vars.TryGetValue("hPos", out value))
			{
				string text = (string)value.o;
				if (!text.IsNullOrEmpty() && vars.TryGetValue("nullNo", out value))
				{
					int version = (int)value.o;
					useNull = Program.GetNull(version, mapName, text, actScene.Map, out pos, out rot);
				}
			}
			if (!useNull)
			{
				Transform transform = actScene.Map.mapRoot.transform.Find("h_free") ?? actScene.Map.mapRoot.transform;
				pos = transform.position;
				rot = transform.rotation;
			}
			yield return StartCoroutine(actScene.ChangeHEvent(pos, rot, appoint, isKokanForceInsert, Tuple.Create(5, heroine)));
		}

		private IEnumerator Morning(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MorningMenu));
			yield return StartCoroutine(MapChangeFadeIn("自室"));
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			Next();
		}

		private IEnumerator GotoSchool(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				gotoSchoolRemoveChara = null;
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else
			{
				if (nowWeek == Week.Holiday)
				{
					gotoSchoolRemoveChara = null;
					Next();
					yield break;
				}
				if (nowWeek == Week.Saturday)
				{
					gotoSchoolRemoveChara = null;
					Next();
					yield break;
				}
				Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MorningMenu));
				SaveData.Heroine greeting = Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine p) => p.fixCharaID == 0).Shuffle().FirstOrDefault(delegate(SaveData.Heroine p)
				{
					if (p.isAnger)
					{
						return false;
					}
					if (p == gotoSchoolRemoveChara)
					{
						return false;
					}
					switch (p.relation)
					{
					case 2:
						return Illusion.Utils.ProbabilityCalclator.DetectFromPercent(15f);
					case 1:
						return Illusion.Utils.ProbabilityCalclator.DetectFromPercent(10f);
					case 0:
						return Illusion.Utils.ProbabilityCalclator.DetectFromPercent(5f);
					default:
						return false;
					}
				});
				gotoSchoolRemoveChara = null;
				if (greeting != null)
				{
					int advNo = -1;
					switch (greeting.relation)
					{
					case 0:
						advNo = 18;
						break;
					case 1:
						advNo = 19;
						break;
					case 2:
						advNo = ((greeting.schoolClass != Singleton<Game>.Instance.Player.schoolClass) ? 20 : 21);
						if (greeting.isNickNameEvent && Illusion.Utils.ProbabilityCalclator.DetectFromPercent(30f))
						{
							advNo = 91;
						}
						break;
					}
					CreateChara createChara = new CreateChara();
					createChara.CreateHeroine(0);
					yield return StartCoroutine(EventADVStart(advNo, greeting, createChara));
				}
			}
			Next();
		}

		private IEnumerator HR1(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else if (nowWeek == Week.Holiday)
			{
				Next();
				yield break;
			}
			Next();
		}

		private IEnumerator Lesson1(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else if (nowWeek == Week.Holiday)
			{
				Next();
				yield break;
			}
			Next();
		}

		private IEnumerator LunchTime(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			if (nowWeek == Week.Holiday)
			{
				FixEventScheduler.Result result;
				SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
				if (fixHeroine != null)
				{
					yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
				}
				Next();
				yield break;
			}
			bool isSaturday = nowWeek > Week.Friday;
			if (!isSaturday)
			{
				yield return StartCoroutine(MapChangeFadeIn(GetNowLessonesRename(Singleton<Game>.Instance.Player.schoolClass)[0]));
			}
			else
			{
				yield return StartCoroutine(MapChangeFadeIn("中庭"));
			}
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MapMoveDay));
			if (!isSaturday)
			{
				ChimeSE();
			}
			yield return Observable.FromCoroutine(MapMove).StartAsCoroutine(cancel);
			ChimeSE();
			Next();
		}

		private IEnumerator Lesson2(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else if (nowWeek == Week.Holiday)
			{
				Next();
				yield break;
			}
			Next();
		}

		private IEnumerator HR2(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			else if (nowWeek == Week.Holiday)
			{
				Next();
				yield break;
			}
			Next();
		}

		private IEnumerator StaffTime(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			if (nowWeek == Week.Holiday)
			{
				FixEventScheduler.Result result;
				SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
				if (fixHeroine != null)
				{
					yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
				}
				Next();
			}
			else
			{
				Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MapMoveDay));
				yield return StartCoroutine(MapChangeFadeIn("部室"));
				yield return Observable.FromCoroutine(MapMove).StartAsCoroutine(cancel);
				ChimeSE();
				Next();
			}
		}

		private IEnumerator AfterSchool(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			if (nowWeek == Week.Holiday)
			{
				FixEventScheduler.Result result;
				SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
				if (fixHeroine != null)
				{
					yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
				}
				Next();
				yield break;
			}
			if (nowWeek <= Week.Friday)
			{
				yield return StartCoroutine(MapChangeFadeIn("教室2-1"));
			}
			else
			{
				yield return StartCoroutine(MapChangeFadeIn("部室"));
			}
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.MapMoveEve));
			yield return Observable.FromCoroutine(MapMove).StartAsCoroutine(cancel);
			Next();
		}

		private IEnumerator GotoMyHouse(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			string mapName = "自室";
			Type backup = _nowType;
			if (myRoomInHeroineEventCheck != null)
			{
				withHeroine = myRoomInHeroineEventCheck.heroine;
				yield return StartCoroutine(GotoMyHouseMyRoomInHeroineEvent(mapName, myRoomInHeroineEventCheck.heroine, true));
			}
			else if (withHeroine != null)
			{
				yield return StartCoroutine(GotoMyHouseWithHeroine(mapName, withHeroine));
				if (myRoomInHeroineEventCheck != null)
				{
					yield return StartCoroutine(GotoMyHouseMyRoomInHeroineEvent(mapName, myRoomInHeroineEventCheck.heroine, false));
				}
			}
			else
			{
				FixEventScheduler.Result result;
				SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
				if (fixHeroine != null)
				{
					yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
				}
			}
			myRoomInHeroineEventCheck = null;
			_nowType = backup;
			Next();
		}

		private IEnumerator GotoMyHouseMyRoomInHeroineEvent(string mapName, SaveData.Heroine heroine, bool isDate)
		{
			heroine.myRoomCount++;
			Singleton<Game>.Instance.rankSaveData.myRoomHCount++;
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			int advNo = 63;
			if (heroine.talkEvent.Contains(advNo))
			{
				advNo = 64;
			}
			CreateChara createChara = new CreateChara();
			int coordinate = ((!isDate && nowWeek <= Week.Friday) ? 1 : 5);
			createChara.CreateHeroine(coordinate);
			string cycle = null;
			SetADVCycle(ref cycle);
			yield return StartCoroutine(EventADVStart(advNo, heroine, createChara));
			Transform hPos = actScene.Map.mapRoot.transform.Find("h_free");
			Cycle cycle2 = this;
			ActionScene actionScene = actScene;
			Vector3 position = hPos.position;
			Quaternion rotation = hPos.rotation;
			Tuple.Create(coordinate, heroine);
			yield return cycle2.StartCoroutine(actionScene.ChangeHEvent(position, rotation, -1, false, Tuple.Create(coordinate, heroine)));
		}

		private IEnumerator GotoMyHouseWithHeroine(string mapName, SaveData.Heroine heroine)
		{
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			int advNo = 65;
			if (heroine.talkEvent.Contains(advNo))
			{
				switch (heroine.relation)
				{
				case 2:
					advNo = 67;
					if (heroine.talkEvent.Contains(advNo) && !Manager.Config.AddData.ADVEventNotOmission)
					{
						advNo = 62;
					}
					break;
				case 1:
					advNo = 66;
					if (heroine.talkEvent.Contains(advNo) && !Manager.Config.AddData.ADVEventNotOmission)
					{
						advNo = 68;
					}
					break;
				default:
					advNo = 68;
					break;
				}
			}
			CreateChara createChara = new CreateChara();
			createChara.CreateHeroine((nowWeek <= Week.Friday) ? 1 : 5);
			yield return StartCoroutine(EventADVStart(advNo, heroine, createChara));
			bool isOpenH = false;
			ValData valData;
			if (actScene.AdvScene.Scenario.Vars.TryGetValue("isH", out valData))
			{
				isOpenH = (bool)valData.o;
			}
			if (isOpenH)
			{
				myRoomInHeroineEventCheck = new MyRoomInHeroineEventCheck(heroine, true);
			}
			else
			{
				withHeroine = null;
			}
		}

		private IEnumerator MyHouse(CancellationToken cancel)
		{
			actScene.SetAllCharaActive(false);
			FixEventScheduler.Result result;
			SaveData.Heroine fixHeroine = actScene.FindFixEventHeroine(out result);
			if (fixHeroine != null)
			{
				yield return StartCoroutine(FixCharaEventStart(fixHeroine, result));
			}
			timerVisible = true;
			string mapName = "自室";
			yield return StartCoroutine(MapChangeFadeIn(mapName));
			Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(BGM.NightMenu));
			Program.SetNull(Camera.main.transform, 0, mapName, "advPos_NightMenu", actScene.Map);
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			if (Game.isAdd20 && Illusion.Game.Utils.Scene.OpenTutorial(7))
			{
				yield return new WaitWhile(Illusion.Game.Utils.Scene.IsTutorial);
			}
			bool isOpenMenu = false;
			string levelName = "NightMenu";
			bool isLoadSceneOpenFile = false;
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				assetBundleName = "action/menu/night.unity3d",
				levelName = levelName,
				isAdd = true,
				onLoad = delegate
				{
					NightMenuScene rootComponent = Scene.GetRootComponent<NightMenuScene>(levelName);
					if (!(rootComponent == null))
					{
						rootComponent.onLoadSubject.TakeUntilDestroy(actScene).Subscribe(delegate
						{
							isLoadSceneOpenFile = true;
							int week = Singleton<Game>.Instance.saveData.week;
							timeCtrl.dayOfTheWeek = week;
							_nowWeek = Illusion.Utils.Enum<Week>.Cast(week);
						});
						rootComponent.OnEnableAsObservable().TakeUntilDestroy(actScene).Subscribe(delegate
						{
							timerVisible = true;
						});
						rootComponent.OnDisableAsObservable().TakeUntilDestroy(actScene).Subscribe(delegate
						{
							timerVisible = false;
						});
						isOpenMenu = true;
					}
				}
			}, false);
			yield return new WaitUntil(() => isOpenMenu);
			yield return new WaitWhile(() => Singleton<Scene>.Instance.NowSceneNames.Contains(levelName));
			if (Scene.isReturnTitle || Scene.isGameEnd)
			{
				yield break;
			}
			if (isLoadSceneOpenFile)
			{
				if (actScene.Player != null)
				{
					actScene.Player.Replace(Singleton<Game>.Instance.Player);
				}
				actScene.paramUI.UpdatePlayer();
				actScene.actCtrl.Refresh();
			}
			isShufflePoped = false;
			Singleton<Game>.Instance.ParameterCorrectValues();
			_withHeroine = null;
			_dateHeroine = null;
			bool isPlayerCreate = actScene.Player == null;
			if (isPlayerCreate)
			{
				Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
				if (fadeType == Scene.Data.FadeType.In)
				{
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
				}
				actScene.PlayerCreate();
			}
			yield return new WaitUntil(() => actScene.Player != null && actScene.Player.initialized);
			if (isPlayerCreate && actScene.CameraState.Mode == CameraMode.FPS)
			{
				actScene.Player.chaCtrl.visibleAll = false;
			}
			Singleton<Game>.Instance.Player.actionCount = 5;
			NextWeek();
			actScene.saturdayTeachers.Clear();
			Next();
		}
	}
}
