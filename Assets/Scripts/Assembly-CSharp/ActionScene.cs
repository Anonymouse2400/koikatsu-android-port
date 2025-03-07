using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame;
using ActionGame.Chara;
using ActionGame.Communication;
using ActionGame.H;
using ActionGame.MapObject;
using ActionGame.Point;
using DG.Tweening;
using Illusion;
using Illusion.Component;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Elements.EasyLoader;
using Localize.Translate;
using Manager;
using StrayTech;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Cycle))]
public sealed class ActionScene : BaseLoader
{
	public enum Regulate
	{
		Move = 1
	}

	public enum ADVReserveType
	{
		MapWarning = 0
	}

	public class ADVParam : SceneParameter
	{
		private class WarningADV
		{
			public Vector3 pos { get; private set; }

			public Quaternion rot { get; private set; }

			public WarningADV(Vector3 pos, Quaternion rot)
			{
				this.pos = pos;
				this.rot = rot;
			}
		}

		public bool isInVisibleChara = true;

		private WarningADV warningADV;

		public ADVParam(ActionScene scene)
			: base(scene)
		{
		}

		public override void Init(ADV.Data data)
		{
			warningADV = null;
			ADVScene aDVScene = SceneParameter.advScene;
			ActionScene actionScene = base.mono as ActionScene;
			TextScenario scenario = aDVScene.Scenario;
			scenario.LoadBundleName = data.bundleName;
			scenario.LoadAssetName = data.assetName;
			aDVScene.Stand.SetPositionAndRotation(data.position, data.rotation);
			scenario.heroineList = data.heroineList;
			scenario.transferList = data.transferList;
			if (!data.heroineList.IsNullOrEmpty())
			{
				scenario.currentChara = new CharaData(data.heroineList[0], scenario, data.motionReserverList.SafeGet(0), data.isParentChara);
				if (data.isParentChara)
				{
					scenario.commandController.AddChara(0, scenario.currentChara);
				}
			}
			if (data.camera != null)
			{
				scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
				scenario.BackCamera.fieldOfView = Camera.main.fieldOfView;
			}
			foreach (KeyValuePair<string, Func<string>> aDVVariable in actionScene.Cycle.ADVVariables)
			{
				scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(string)));
			}
			aDVScene.Map = actionScene.Map;
			float fadeInTime = data.fadeInTime;
			if (fadeInTime > 0f)
			{
				aDVScene.fadeTime = fadeInTime;
			}
			else
			{
				isInVisibleChara = false;
			}
			Program.Transfer transfer = data.transferList.Find((Program.Transfer p) => p.param.Command == Command.Open);
			if (transfer != null)
			{
				string text = transfer.param.Args[1];
				if (text == "Warning")
				{
					warningADV = new WarningADV(data.camera.position, data.camera.rotation);
				}
			}
		}

		public override void Release()
		{
			ADVScene aDVScene = SceneParameter.advScene;
			ActionScene actScene = base.mono as ActionScene;
			if (isInVisibleChara)
			{
				List<ChaControl> visibleList = actScene.VisibleList;
				if (visibleList != null)
				{
					visibleList.RemoveAll((ChaControl p) => p == null);
					visibleList.ForEach(delegate(ChaControl p)
					{
						p.GetComponent<Base>().chaCtrl.visibleAll = true;
					});
				}
			}
			if (warningADV != null)
			{
				Camera.main.transform.SetPositionAndRotation(warningADV.pos, warningADV.rot);
			}
			bool isPlayerVisible = true;
			MapInfo.Param info = actScene.Map.Info;
			if (!info.isGate || info.is2D || !actScene.Cycle.isAction)
			{
				isPlayerVisible = false;
			}
			Player player = actScene.Player;
			if (player != null)
			{
				if (info.isGate)
				{
					player.mapNo = actScene.Map.no;
				}
				player.SetActive(isPlayerVisible);
			}
			actScene.npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(isPlayerVisible && actScene.Map.no == p.mapNo);
			});
			aDVScene.gameObject.SetActive(false);
		}

		public override void WaitEndProc()
		{
			ActionScene actionScene = base.mono as ActionScene;
			if (isInVisibleChara)
			{
				actionScene.VisibleList.ForEach(delegate(ChaControl p)
				{
					p.GetComponent<Base>().chaCtrl.visibleAll = false;
				});
				actionScene.Player.chaCtrl.visibleAll = false;
			}
		}
	}

	[Serializable]
	private class ActionMenu
	{
		public enum Mode
		{
			ClothChange = 0,
			Status = 1,
			GirlfriendList = 2,
			FloowMyHome = 3,
			GotoMyHome = 4
		}

		private ReactiveProperty<bool> visible;

		[SerializeField]
		private Canvas _menuCanvas;

		private CanvasGroup canvasGroup;

		[SerializeField]
		private ActionMoveUI _actionMoveUI;

		public bool Visible
		{
			get
			{
				return visible.Value;
			}
			set
			{
				visible.Value = value;
			}
		}

		public bool Interactable
		{
			get
			{
				return canvasGroup.interactable;
			}
			set
			{
				canvasGroup.interactable = value;
			}
		}

		public float Alpha
		{
			get
			{
				return canvasGroup.alpha;
			}
			set
			{
				canvasGroup.alpha = value;
			}
		}

		public void Initialize(ActionScene actScene)
		{
			_actionMoveUI.Initialize(actScene);
			canvasGroup = _menuCanvas.GetComponent<CanvasGroup>();
			Button[] componentsInChildren = _menuCanvas.GetComponentsInChildren<Button>();
			ChangePlayerClothesComponent changePlayerClothes = null;
			if (componentsInChildren == null)
			{
				return;
			}
			string[] names = Enum.GetNames(typeof(Mode));
			Button[] array = componentsInChildren;
			foreach (Button item in array)
			{
				if (!item.gameObject.activeSelf)
				{
					continue;
				}
				int i2 = names.Check(item.name);
				if (i2 == -1)
				{
					continue;
				}
				switch ((Mode)i2)
				{
				case Mode.ClothChange:
					changePlayerClothes = item.GetComponentInChildren<ChangePlayerClothesComponent>();
					changePlayerClothes.Initialize();
					changePlayerClothes.Close();
					break;
				case Mode.GirlfriendList:
					(from _ in actScene.UpdateAsObservable()
						select (actScene.Player != null && actScene.Player.isActionNow_Origin) || (actScene.Map.no >= 0 && actScene.Map.Info.isWarning) || actScene.npcList.Any((NPC npc) => npc.AI.actionNo == 24) || actScene.npcList.Any((NPC npc) => npc.isActive && (npc.isOnanism || npc.isLesbian))).DistinctUntilChanged().Subscribe(delegate(bool isOn)
					{
						item.interactable = !isOn;
					});
					(from _ in actScene.UpdateAsObservable()
						select actScene.npcList.Select((NPC npc) => npc.heroine).Any((SaveData.Heroine p) => p.isGirlfriend)).DistinctUntilChanged().Subscribe(delegate(bool active)
					{
						item.gameObject.SetActiveIfDifferent(active);
					});
					break;
				case Mode.FloowMyHome:
					item.gameObject.SetActiveIfDifferent(false);
					break;
				}
				(from _ in item.OnClickAsObservable()
					select Tuple.Create(item, (Mode)i2)).Subscribe(delegate(Tuple<Button, Mode> p)
				{
					Illusion.Game.Utils.Sound.Play(SystemSE.ok_s);
					switch (p.Item2)
					{
					case Mode.ClothChange:
						if (!changePlayerClothes.isOpen)
						{
							changePlayerClothes.Open();
						}
						else
						{
							changePlayerClothes.Close();
						}
						break;
					case Mode.Status:
					{
						Scene.Data data = new Scene.Data
						{
							levelName = "Status",
							isAdd = true,
							isFade = false,
							isAsync = false
						};
						Singleton<Scene>.Instance.LoadReserve(data, false);
						break;
					}
					case Mode.GirlfriendList:
					{
						Scene.Data data2 = new Scene.Data
						{
							assetBundleName = "action/menu/callselect.unity3d",
							levelName = "CallSelect",
							isAdd = true,
							isFade = false,
							isAsync = false
						};
						Singleton<Scene>.Instance.LoadReserve(data2, false);
						actScene.Player.SetPhone();
						break;
					}
					case Mode.FloowMyHome:
						break;
					case Mode.GotoMyHome:
					{
						p.Item1.interactable = false;
						CheckScene.Parameter param = new CheckScene.Parameter();
						param.Yes = delegate
						{
							Singleton<Scene>.Instance.UnLoad();
							Observable.FromCoroutine((CancellationToken _) => Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In)).Subscribe(delegate
							{
								actScene.Cycle.Change(Cycle.Type.MyHouse);
								p.Item1.interactable = true;
							});
						};
						param.No = delegate
						{
							p.Item1.interactable = true;
							Singleton<Scene>.Instance.UnLoad();
						};
						param.Title = Localize.Translate.Manager.OtherData.Get(100).Values.FindTagText("ReturnHome") ?? "家に帰りますか？";
						Observable.FromCoroutine((IObserver<CheckScene> observer) => Illusion.Game.Utils.Scene.Check.Load(param, observer)).Subscribe();
						break;
					}
					}
				});
			}
			visible = new ReactiveProperty<bool>(false);
			visible.TakeUntilDestroy(actScene).Subscribe(delegate(bool value)
			{
				_menuCanvas.enabled = value;
				changePlayerClothes.Close();
			});
		}
	}

	[SerializeField]
	private SmoothDampFollowTargetPublices _follow;

	[SerializeField]
	private ADVScene advScene;

	[SerializeField]
	private Camera nowCamera;

	private Transform _cameraTransform;

	[SerializeField]
	private CameraStateDefinitionChange cameraStateDefinitionChange;

	private bool _isCursorLock;

	[EnumFlags]
	public Regulate regulate;

	private Dictionary<int, CharaInfo.Param> _charaInfoDic;

	private Dictionary<int, FixCharaInfo.Param> _fixCharaInfoDic;

	private Dictionary<int, TalkLookNeck.Param> _talkLookNeckDic;

	private Dictionary<string, TalkLookBody.Param> _talkLookBodyDic;

	[SerializeField]
	private Transform miniMapTarget;

	[SerializeField]
	private bl_MiniMap _miniMap;

	[SerializeField]
	private ActionChangeUI _actionChangeUI;

	private bool _isInChargeBGM;

	private Dictionary<int, Dictionary<int, Localize.Translate.Data.Param>> _uiTranslater;

	public List<Tuple<ADVReserveType, Func<IEnumerator>>> advReserveList = new List<Tuple<ADVReserveType, Func<IEnumerator>>>();

	private ActionControl _actCtrl = new ActionControl();

	[SerializeField]
	private ParamUI _paramUI;

	[SerializeField]
	private GameObject informationH;

	private GameObject nowInformationH;

	[SerializeField]
	private ShortcutKey _systemShortcutKey;

	[SerializeField]
	private ShortcutKey _actionShortcutKey;

	private float _lesbianDistance = 1.5f;

	private HashSet<SaveData.Heroine> _saturdayTeachers = new HashSet<SaveData.Heroine>();

	[SerializeField]
	private ActionMenu actionMenu;

	private SaveData.Heroine hSceneOtherHeroine;

	public List<ChaControl> VisibleList { get; private set; }

	public ActionMap Map { get; private set; }

	public Cycle Cycle { get; private set; }

	public CharacterPosSet PosSet { get; private set; }

	public SmoothDampFollowTargetPublices follow
	{
		get
		{
			return _follow;
		}
	}

	public Player Player { get; private set; }

	public ADVScene AdvScene
	{
		get
		{
			return advScene;
		}
	}

	public Transform cameraTransform
	{
		get
		{
			return nowCamera.GetComponentCache(ref _cameraTransform);
		}
	}

	public CameraStateDefinitionChange CameraState
	{
		get
		{
			return cameraStateDefinitionChange;
		}
	}

	public bool isCursorLock
	{
		get
		{
			return _isCursorLock;
		}
	}

	public List<NPC> npcList { get; private set; }

	public Fix fixChara { get; private set; }

	public Dictionary<int, CharaInfo.Param> charaInfoDic
	{
		get
		{
			return _charaInfoDic;
		}
	}

	public Dictionary<int, FixCharaInfo.Param> fixCharaInfoDic
	{
		get
		{
			return _fixCharaInfoDic;
		}
	}

	public Dictionary<int, TalkLookNeck.Param> talkLookNeckDic
	{
		get
		{
			return _talkLookNeckDic;
		}
	}

	public Dictionary<string, TalkLookBody.Param> talkLookBodyDic
	{
		get
		{
			return _talkLookBodyDic;
		}
	}

	public Transform MiniMapTarget
	{
		get
		{
			return miniMapTarget;
		}
	}

	public bl_MiniMap miniMap
	{
		get
		{
			return _miniMap;
		}
	}

	public ActionChangeUI actionChangeUI
	{
		get
		{
			return _actionChangeUI;
		}
	}

	public bool IsInChargeBGM
	{
		get
		{
			return _isInChargeBGM;
		}
	}

	public Dictionary<int, Dictionary<int, Localize.Translate.Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.ACTION));
		}
	}

	public bool MiniMapAndCameraActive
	{
		get
		{
			return MonoBehaviourSingleton<CameraSystem>.Instance.enabled && miniMap.m_Canvas.enabled;
		}
		set
		{
			miniMap.m_Canvas.enabled = value;
			MonoBehaviourSingleton<CameraSystem>.Instance.enabled = value;
			paramUI.active = value;
			_actionChangeUI.isVisible = value;
		}
	}

	public bool isEventNow { get; private set; }

	private bool isReserveADVProccessing { get; set; }

	public ActionControl actCtrl
	{
		get
		{
			return _actCtrl;
		}
	}

	public ParamUI paramUI
	{
		get
		{
			return _paramUI;
		}
	}

	private bool shortcutKey
	{
		set
		{
			_systemShortcutKey.enabled = value;
			_actionShortcutKey.enabled = value;
		}
	}

	public float lesbianDistance
	{
		get
		{
			return _lesbianDistance;
		}
	}

	public HashSet<SaveData.Heroine> saturdayTeachers
	{
		get
		{
			return _saturdayTeachers;
		}
	}

	public bool isPenetration { get; set; }

	public NPC otherPauseNPC { get; private set; }

	public void PlayerCreate()
	{
		if (!(Player != null))
		{
			Player = Player.Create(base.transform, Singleton<Game>.Instance.Player);
			Player.LoadStart();
		}
	}

	public void PlayerDelete()
	{
		if (!(Player == null))
		{
			UnityEngine.Object.Destroy(Player.gameObject);
			Player = null;
		}
	}

	public void FixCharaDelete()
	{
		if (fixChara != null)
		{
			UnityEngine.Object.Destroy(fixChara.gameObject);
			fixChara = null;
		}
	}

	public SaveData.Heroine FindFixEventHeroine(out FixEventScheduler.Result result)
	{
		result = null;
		if (Cycle.nowWeek == Cycle.Week.Saturday)
		{
			return null;
		}
		foreach (SaveData.Heroine item in Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine p) => p.fixCharaID <= -5).Shuffle())
		{
			if (FixEventScheduler.Check(this, item, Cycle.fixEventSchedule[item.fixCharaID], out result))
			{
				return item;
			}
		}
		return null;
	}

	public void ShortcutKeyEnable(bool isOn)
	{
		shortcutKey = isOn;
	}

	protected override void Awake()
	{
		base.Awake();
		npcList = new List<NPC>();
		VisibleList = new List<ChaControl>();
		Map = GetComponent<ActionMap>();
		Cycle = GetComponent<Cycle>();
		regulate = (Regulate)0;
		PosSet = new CharacterPosSet();
		ParameterList.Add(new ADVParam(this));
	}

	private IEnumerator Start()
	{
		base.enabled = false;
		Singleton<Game>.Instance.actScene = this;
		Cycle.Initialize();
		_actCtrl.Init();
		_actCtrl.Refresh();
		Singleton<Manager.Sound>.Instance.Listener = cameraTransform;
		PlayerCreate();
		actionMenu.Initialize(this);
		PosSet.Initialize();
		_charaInfoDic = new Dictionary<int, CharaInfo.Param>();
		List<string> assetBundleNameListFromPath = CommonLib.GetAssetBundleNameListFromPath("action/list/chara/", true);
		assetBundleNameListFromPath.Sort();
		assetBundleNameListFromPath.ForEach(delegate(string file)
		{
			foreach (CharaInfo item in from p in AssetBundleManager.LoadAllAsset(file, typeof(CharaInfo)).GetAllAssets<CharaInfo>()
				orderby p.name
				select p)
			{
				foreach (CharaInfo.Param item2 in item.param)
				{
					_charaInfoDic[item2.Personality] = item2;
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		_fixCharaInfoDic = new Dictionary<int, FixCharaInfo.Param>();
		List<string> assetBundleNameListFromPath2 = CommonLib.GetAssetBundleNameListFromPath("action/list/fixchara/", true);
		assetBundleNameListFromPath2.Sort();
		assetBundleNameListFromPath2.ForEach(delegate(string file)
		{
			foreach (FixCharaInfo item3 in from p in AssetBundleManager.LoadAllAsset(file, typeof(FixCharaInfo)).GetAllAssets<FixCharaInfo>()
				orderby p.name
				select p)
			{
				foreach (FixCharaInfo.Param item4 in item3.param)
				{
					_fixCharaInfoDic[item4.FixID] = item4;
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		_talkLookNeckDic = new Dictionary<int, TalkLookNeck.Param>();
		List<string> assetBundleNameListFromPath3 = CommonLib.GetAssetBundleNameListFromPath("action/list/talklookneck/", true);
		assetBundleNameListFromPath3.Sort();
		assetBundleNameListFromPath3.ForEach(delegate(string file)
		{
			foreach (TalkLookNeck item5 in from p in AssetBundleManager.LoadAllAsset(file, typeof(TalkLookNeck)).GetAllAssets<TalkLookNeck>()
				orderby p.name
				select p)
			{
				foreach (TalkLookNeck.Param item6 in item5.param)
				{
					_talkLookNeckDic[Animator.StringToHash(item6.StateName)] = item6;
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		_talkLookBodyDic = new Dictionary<string, TalkLookBody.Param>();
		List<string> assetBundleNameListFromPath4 = CommonLib.GetAssetBundleNameListFromPath("action/list/talklookbody/", true);
		assetBundleNameListFromPath4.Sort();
		assetBundleNameListFromPath4.ForEach(delegate(string file)
		{
			foreach (TalkLookBody item7 in from p in AssetBundleManager.LoadAllAsset(file, typeof(TalkLookBody)).GetAllAssets<TalkLookBody>()
				orderby p.name
				select p)
			{
				foreach (TalkLookBody.Param item8 in item7.param)
				{
					_talkLookBodyDic[item8.StateName] = item8;
				}
			}
			AssetBundleManager.UnloadAssetBundle(file, false);
		});
		(from _ in this.UpdateAsObservable()
			select Singleton<Scene>.Instance.IsFadeNow).DistinctUntilChanged().Subscribe(delegate(bool isFade)
		{
			actionMenu.Interactable = !isFade;
		});
		(from isOn in (from _ in this.UpdateAsObservable()
				select ActionInput.isCursorLock && Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty() && MiniMapAndCameraActive && !Singleton<Scene>.Instance.IsNowLoadingFade).DistinctUntilChanged()
			where isOn
			select isOn).Subscribe(delegate
		{
			_isCursorLock = !_isCursorLock;
		});
		(from _ in this.UpdateAsObservable()
			select !Cycle.isAction || Program.isADVProcessing || !Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty() || Singleton<Scene>.Instance.IsFadeNow).DistinctUntilChanged().Subscribe(delegate(bool isOn)
		{
			_isCursorLock = !isOn;
		});
		IObservable<bool> source = from _ in this.UpdateAsObservable()
			select _isCursorLock;
		source.Select((bool isOn) => !isOn && Cycle.isAction && !Program.isADVProcessing && Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty()).DistinctUntilChanged().Subscribe(delegate(bool visible)
		{
			actionMenu.Visible = visible;
		});
		source.DistinctUntilChanged().Subscribe(delegate(bool isOn)
		{
			if (!isOn)
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		});
		source.Where((bool isOn) => isOn).Subscribe((Action<bool>)delegate
		{
			Cursor.lockState = CursorLockMode.Locked;
		}, (Action)delegate
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		});
		Func<bool> isUpdateCheck = delegate
		{
			if (Player == null)
			{
				return false;
			}
			bool flag = ((Cycle.isAction && !Program.isADVProcessing && Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty()) ? true : false);
			MiniMapAndCameraActive = flag;
			return flag && !isEventNow && !Singleton<Game>.Instance.IsRegulate(false);
		};
		CameraEffector cameraEffector = nowCamera.GetComponent<CameraEffector>();
		CameraEffectorConfig cameraEffectorConfig = cameraEffector.config;
		(from _ in this.UpdateAsObservable()
			where base.enabled
			where isUpdateCheck()
			select _).Subscribe(delegate
		{
			if (ActionInput.isAction)
			{
				Base actionTarget = Player.actionTarget;
				if (!Player.isActionPointHit && actionTarget != null && (!Player.isGateHit || actionTarget.heroine.fixCharaID != 0) && !Player.isActionNow && actionTarget.isAction && Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty())
				{
					StartCoroutine(_SceneEvent(actionTarget));
				}
			}
			switch (cameraStateDefinitionChange.Mode)
			{
			case CameraMode.FPS:
				cameraEffectorConfig.SetDOF(false);
				break;
			case CameraMode.TPS:
			case CameraMode.Other:
				cameraEffectorConfig.SetDOF(true);
				break;
			}
			cameraEffector.SetDOFTarget(Player.cachedTransform);
		});
		(from _ in this.UpdateAsObservable()
			where advReserveList.Any()
			where !isReserveADVProccessing
			select _).Subscribe(delegate
		{
			Observable.FromCoroutine((CancellationToken __) => ReserveADV()).TakeUntilDestroy(this).Subscribe();
		});
		yield return new WaitUntil(() => Player != null && Player.initialized);
		_paramUI.Init();
		base.enabled = true;
		if (Singleton<Game>.Instance.saveData.isOpening)
		{
			ADV.Data openData = new ADV.Data
			{
				fadeInTime = 0f,
				scene = this,
				transferList = new List<Program.Transfer>()
			};
			openData.transferList.Add(Program.Transfer.Create(false, Command.SceneFadeRegulate, bool.FalseString));
			Program.SetParam(Singleton<Game>.Instance.Player, openData.transferList);
			openData.transferList.Add(Program.Transfer.Open("op/00", "0", bool.TrueString));
			yield return StartCoroutine(Program.Open(openData));
			yield return Program.Wait(string.Empty);
			Singleton<Game>.Instance.saveData.isOpening = false;
			Singleton<Game>.Instance.glSaveData.isOpeningEnd = true;
			if (Illusion.Game.Utils.Scene.SafeFadeIn() == Scene.Data.FadeType.In)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
		}
		else
		{
			Singleton<Game>.Instance.glSaveData.isOpeningEnd = true;
			Cycle.Change(Cycle.nowType);
		}
		Resources.UnloadUnusedAssets();
	}

	private void OnDestroy()
	{
		if (Singleton<Manager.Character>.IsInstance())
		{
			Singleton<Manager.Character>.Instance.loading.Clear();
		}
		ParameterList.Remove(this);
	}

	private IEnumerator ReserveADV()
	{
		isReserveADVProccessing = true;
		bool isMapWarning = false;
		foreach (Tuple<ADVReserveType, Func<IEnumerator>> item in advReserveList)
		{
			if (item.Item1 == ADVReserveType.MapWarning)
			{
				isMapWarning = true;
			}
			yield return StartCoroutine(item.Item2());
			yield return Program.ADVProcessingCheck();
		}
		advReserveList.Clear();
		isReserveADVProccessing = false;
		if (isMapWarning)
		{
			Player.mapNo = Map.prevNo;
			Map.Change(Map.prevNo);
		}
	}

	private IEnumerator _SceneEvent(Base chara)
	{
		if (Cycle.timer >= 499f || chara == null || isEventNow)
		{
			yield break;
		}
		_isInChargeBGM = true;
		Player.isActionNow = true;
		isEventNow = true;
		Player.isLesMotionPlay = false;
		shortcutKey = false;
		Cycle.Type nowCycle = Cycle.nowType;
		string prevBGM = string.Empty;
		float prevVolume = 1f;
		yield return StartCoroutine(Illusion.Game.Utils.Sound.GetBGMandVolume(delegate(string bgm, float volume)
		{
			prevBGM = bgm;
			prevVolume = volume;
		}));
		yield return null;
		yield return new WaitUntil(() => Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty());
		SaveData.Heroine heroine = chara.heroine;
		if (heroine.fixCharaID < 0)
		{
			Player.ChaserEnd();
			Player.move.isStop = true;
			if (heroine.isTeacher)
			{
				yield return StartCoroutine(_SceneEventTeacher(chara as Fix));
			}
			else
			{
				yield return StartCoroutine(_SceneEventFixChara(chara as Fix));
			}
			Player.move.isStop = false;
		}
		else
		{
			NPC npc = chara as NPC;
			int id = npc.GetOnanismID(false);
			if (id != -1)
			{
				Player.ChaserEnd();
				yield return StartCoroutine(ChangeHSpecial(new List<int> { id }, false, npc.cachedTransform, npc));
			}
			else
			{
				id = npc.GetLesbianID(false);
				if (id != -1)
				{
					Player.ChaserEnd();
					yield return StartCoroutine(ChangeHSpecial(new List<int> { id }, false, npc.cachedTransform, npc.AI.target, npc));
				}
				else
				{
					yield return StartCoroutine(_SceneEventNPC(npc));
				}
			}
		}
		if (_isInChargeBGM && Cycle.isAction && nowCycle == Cycle.nowType)
		{
			yield return StartCoroutine(Illusion.Game.Utils.Sound.GetFadePlayerWhileNull(prevBGM, prevVolume));
		}
		shortcutKey = true;
		_isInChargeBGM = false;
		isEventNow = false;
		Player.isActionNow = false;
	}

	private  IEnumerator _SceneEventTeacher(Fix chara)
	{
		bool isOpenH = false;
		SaveData.Heroine heroine = chara.heroine;
		List<Program.Transfer> transferList = Program.Transfer.NewList();
		Program.SetParam(Singleton<Game>.Instance.Player, heroine, transferList);
		int cnt = 0;
		int[] array = new int[7];
		int num;
		cnt = (num = cnt - 1);
		array[0] = num;
		cnt = (num = cnt - 1);
		array[1] = num;
		cnt = (num = cnt - 1);
		array[2] = num;
		cnt = (num = cnt - 1);
		array[3] = num;
		cnt = (num = cnt - 1);
		array[4] = num;
		cnt = (num = cnt - 1);
		array[5] = num;
		cnt = (num = cnt - 1);
		array[6] = num;
		int[] firstTalks = array;
		int[] array2 = new int[7];
		cnt = (num = cnt - 1);
		array2[0] = num;
		cnt = (num = cnt - 1);
		array2[1] = num;
		cnt = (num = cnt - 1);
		array2[2] = num;
		cnt = (num = cnt - 1);
		array2[3] = num;
		cnt = (num = cnt - 1);
		array2[4] = num;
		cnt = (num = cnt - 1);
		array2[5] = num;
		cnt = (num = cnt - 1);
		array2[6] = num;
		int[] secondTalks = array2;
		int[] array3 = new int[3];
		cnt = (num = cnt - 1);
		array3[0] = num;
		cnt = (num = cnt - 1);
		array3[1] = num;
		num = cnt - 1;
		array3[2] = num;
		int[] lastTalks = array3;
		heroine.talkTime = Mathf.Max(0, heroine.talkTime - 1);
		int advNo = -100;
		bool isForce = false;
		if (!heroine.talkEvent.Contains(0))
		{
			switch (heroine.fixCharaID)
			{
			case -1:
				if (Singleton<Game>.Instance.HeroineList.Find((SaveData.Heroine p) => p.fixCharaID == -5).talkEvent.Contains(10))
				{
					heroine.talkEvent.Add(0);
				}
				break;
			case -2:
				if (Singleton<Game>.Instance.HeroineList.Find((SaveData.Heroine p) => p.fixCharaID == -10).talkEvent.Contains(0))
				{
					heroine.talkEvent.Add(0);
				}
				break;
			}
		}
		if (isForce || heroine.talkTime > 0)
		{
			if (!isForce && !heroine.talkEvent.Contains(0))
			{
				advNo = 0;
				heroine.talkTime = 0;
			}
			else if (!isForce && !heroine.talkEvent.Contains(1))
			{
				advNo = firstTalks.Where((int talk) => !heroine.talkEvent.Contains(talk)).Shuffle().FirstOrDefault();
				if (advNo.IsDefault())
				{
					advNo = 1;
					heroine.talkTime = 0;
				}
			}
			else if (!isForce && !heroine.talkEvent.Contains(2))
			{
				advNo = secondTalks.Where((int talk) => !heroine.talkEvent.Contains(talk)).Shuffle().FirstOrDefault();
				if (advNo.IsDefault())
				{
					advNo = 2;
					heroine.talkTime = 0;
					isOpenH = true;
				}
			}
			else if (!isForce && !heroine.talkEvent.Contains(4))
			{
				advNo = lastTalks.Where((int talk) => !heroine.talkEvent.Contains(talk)).Shuffle().FirstOrDefault();
				if (advNo.IsDefault())
				{
					advNo = 4;
					heroine.talkTime = 0;
				}
			}
			else
			{
				int[] array4 = secondTalks.Concat(lastTalks).ToArray();
				int num2 = array4.Count((int talk) => heroine.talkEvent.Contains(talk));
				if (!isForce && num2 < 3)
				{
					advNo = array4.Where((int talk) => !heroine.talkEvent.Contains(talk)).Shuffle().First();
				}
				else
				{
					advNo = 4;
					heroine.talkTime = 0;
					int[] array5 = array4;
					foreach (int item in array5)
					{
						heroine.talkEvent.Remove(item);
					}
				}
			}
		}
		string bundle = Program.FindADVBundleFilePath(advNo, heroine);
		OpenData.CameraData cameraData;
		if (advNo >= 0)
		{
			cameraData = new OpenData.CameraData
			{
				position = chara.advCamPos.position,
				rotation = chara.advCamPos.rotation
			};
			transferList.Add(Program.Transfer.Create(false, Command.CharaCreate, "0", "-2"));
			transferList.Add(Program.Transfer.Open(bundle, advNo.ToString(), bool.TrueString));
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			chara.SetActive(false);
			Player.SetActive(false);
			npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(false);
			});
			npcList.ForEach(delegate(NPC p)
			{
				p.Pause(true);
			});
		}
		else
		{
			cameraData = new OpenData.CameraData
			{
				position = nowCamera.transform.position,
				rotation = nowCamera.transform.rotation
			};
			transferList.Add(Program.Transfer.Open(bundle, advNo.ToString(), bool.TrueString));
		}
		bool isOpenADV = false;
		yield return StartCoroutine(Program.Open(new ADV.Data
		{
			fadeInTime = 0f,
			position = chara.position,
			rotation = chara.rotation,
			camera = cameraData,
			heroineList = new List<SaveData.Heroine> { heroine },
			scene = this,
			transferList = transferList
		}, new Program.OpenDataProc
		{
			onLoad = delegate
			{
				isOpenADV = true;
			}
		}));
		yield return new WaitUntil(() => isOpenADV);
		shortcutKey = true;
		yield return Program.Wait(string.Empty);
		heroine.talkEvent.Add(advNo);
		ValData value;
		if (advNo == 4 && AdvScene.Scenario.Vars.TryGetValue("isH", out value))
		{
			isOpenH = (bool)value.o;
		}
		if (isOpenH)
		{
			yield return StartCoroutine(ChangeH(null, false, false, chara));
		}
		if (advNo >= 0 && !isOpenH)
		{
			Player.SetActive(true);
			chara.SetActive(true);
			npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(p.mapNo == Map.no);
			});
			npcList.ForEach(delegate(NPC p)
			{
				p.Pause(false);
			});
            yield return new WaitUntil(() => this.MiniMapAndCameraActive);

            if (Illusion.Game.Utils.Scene.IsFadeOutOK)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			}
		}
	}

	private  IEnumerator _SceneEventFixChara(Fix chara)
	{
		Cycle.Type nowCycle = Cycle.nowType;
		SaveData.Heroine heroine = chara.heroine;
		bool isTaked = heroine.isTaked;
		FixEventScheduler.Result result = chara.eventSchedulerResult;
		FixEventSchedule.Param param = result.param;
		List<Program.Transfer> transferList = Program.Transfer.NewList();
		Program.SetParam(Singleton<Game>.Instance.Player, heroine, transferList);
		transferList.Add(Program.Transfer.Create(true, Command.CameraSetFov, "23"));
		transferList.Add(Program.Transfer.Open(param.Bundle, param.Asset, bool.TrueString));
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
		FixCharaDelete();
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC p)
		{
			p.SetActive(false);
		});
		npcList.ForEach(delegate(NPC p)
		{
			p.Pause(true);
		});
		bool isOpenADV = false;
		yield return StartCoroutine(Program.Open(new ADV.Data
		{
			fadeInTime = 0f,
			camera = null,
			heroineList = new List<SaveData.Heroine> { heroine },
			scene = this,
			transferList = transferList
		}, new Program.OpenDataProc
		{
			onLoad = delegate
			{
				isOpenADV = true;
			}
		}));
		yield return new WaitUntil(() => isOpenADV);
		shortcutKey = true;
		yield return Program.Wait(string.Empty);
		heroine.eventAfterDay = 0;
		if (Cycle.isAction && nowCycle == Cycle.nowType)
		{
			Player.SetActive(true);
			npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(p.mapNo == Map.no);
			});
			npcList.ForEach(delegate(NPC p)
			{
				p.Pause(false);
			});
            yield return new WaitUntil(() => this.MiniMapAndCameraActive);

            if (Illusion.Game.Utils.Scene.IsFadeOutOK)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
			}
		}
		if (!isTaked && heroine.isTaked)
		{
			DisplayHInformation(3f);
		}
	}

	private void DisplayHInformation(float time)
	{
		time *= 0.5f;
		if (nowInformationH != null)
		{
			UnityEngine.Object.Destroy(nowInformationH);
		}
		nowInformationH = UnityEngine.Object.Instantiate(informationH, base.transform, false);
		nowInformationH.GetComponent<CanvasGroup>().DOFade(1f, time).SetEase(Ease.OutExpo)
			.SetLoops(2, LoopType.Yoyo)
			.OnComplete(delegate
			{
				UnityEngine.Object.Destroy(nowInformationH);
			});
		Canvas canvas = nowInformationH.GetComponent<Canvas>();
		canvas.enabled = true;
		nowInformationH.UpdateAsObservable().Subscribe(delegate
		{
			canvas.sortingOrder = Singleton<Scene>.Instance.sceneFade.canvas.sortingOrder + 1;
		});
	}

	private  IEnumerator _SceneEventNPC(NPC npc)
	{
		SaveData.Heroine heroine = npc.heroine;
		npc.move.isStop = true;
		npc.move.velocity = Vector3.zero;
		bool isAttack = npc.AI.ActionNoCheck(8, 28);
		bool isHAttack = npc.AI.ActionNoCheck(5, 29);
		bool isTalkSkip = false;
		bool isIntroSkip = false;
		ResultEnum talkSkipResult = ResultEnum.None;
		switch (npc.AI.actionNo)
		{
		case 28:
			isIntroSkip = true;
			break;
		case 29:
			isTalkSkip = true;
			talkSkipResult = ResultEnum.H;
			heroine.talkEvent.Add(8);
			heroine.talkEvent.Add(12);
			break;
		case 30:
			npc.AI.HResponseAction();
			yield break;
		}
		npc.Pause(true);
		string loadSceneName = "Talk";
		TalkScene talk = null;
		bool isNotice = npc.isNotice;
		bool isChase = Player.chaser == npc;
		ResultInfo resultInfo = null;
		bool isOtherInitialized = false;
		IKMotion ikMotion = null;
		if (!isTalkSkip)
		{
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				assetBundleName = "action/menu/talk.unity3d",
				levelName = loadSceneName,
				isAdd = true,
				onLoad = delegate
				{
					shortcutKey = true;
					talk = Scene.GetRootComponent<TalkScene>(loadSceneName);
					talk.targetHeroine = heroine;
					talk.otherInitialize += delegate
					{
						isOtherInitialized = true;
						MiniMapAndCameraActive = false;
						npcList.ForEach(delegate(NPC p)
						{
							p.SetActive(false);
						});
						npcList.ForEach(delegate(NPC p)
						{
							p.Pause(true);
						});
						if (fixChara != null)
						{
							fixChara.SetActive(false);
						}
						Player.SetActive(false);
					};
					int state = ((npc.actIconNo == NPC.ActIcon.Sleep) ? 2 : ((!npc.isBodyForTarget) ? 1 : 0));
					string[] placeNames = npc.PlaceNames;
					talk.necessaryInfo = new NecessaryInfo(Map.no, Cycle.nowType, npc.isOtherPeople, isChase, !Map.Info.is2D, true, isNotice, state, isAttack, isHAttack, npc.isEasyPlace, Player.isChaseWarning, isIntroSkip, placeNames);
					talk.OnDestroyAsObservable().Subscribe(delegate
					{
						resultInfo = talk.resultInfo;
						ikMotion = talk.resultIK;
					});
				}
			}, false);
		}
		else
		{
			resultInfo = new ResultInfo
			{
				result = talkSkipResult
			};
		}
		yield return new WaitWhile(() => resultInfo == null);
		switch (resultInfo.result)
		{
		case ResultEnum.Lunch:
		case ResultEnum.Club:
		case ResultEnum.GoHome:
		case ResultEnum.Study:
		case ResultEnum.Exercise:
			npc.heroine.chaCtrl.RandomChangeOfClothesLowPolyEnd();
			break;
		}
		int advNo = -1;
		switch (resultInfo.result)
		{
		case ResultEnum.Chase:
			Player.ChaserSet(npc);
			break;
		case ResultEnum.Divorce:
			npc.AI.EscapeAction(false);
			break;
		case ResultEnum.H:
		{
			Base chara = ((!isChase) ? ((Base)npc) : ((Base)Player));
			Kind findKind = null;
			KindGroup kindGroup = Map.mapRoot.GetComponent<KindGroup>();
			List<Kind> kindList = new List<Kind>();
			if (kindGroup != null && !kindGroup.kinds.IsNullOrEmpty())
			{
				kindList.AddRange(kindGroup.kinds);
			}
			if (!Map.mapObjects.IsNullOrEmpty())
			{
				kindList.AddRange(Map.mapObjects);
			}
			if (kindList.Any())
			{
				float num = float.MaxValue;
				foreach (Kind item in kindList)
				{
					float sqrMagnitude = (item.transform.position - chara.position).sqrMagnitude;
					if (sqrMagnitude < num)
					{
						num = sqrMagnitude;
						findKind = item;
					}
				}
				float radius = chara.GetComponent<CapsuleCollider>().radius;
				if (num > radius * radius)
				{
					findKind = null;
				}
			}
			yield return StartCoroutine(ChangeH(findKind, isChase, false, npc));
			break;
		}
		case ResultEnum.Lunch:
			if (heroine.relation == 2)
			{
				heroine.talkEvent.Add(79);
				advNo = ((heroine.talkEvent.Contains(81) && !Manager.Config.AddData.ADVEventNotOmission) ? 83 : 81);
			}
			else
			{
				advNo = ((!resultInfo.isFirst) ? ((heroine.talkEvent.Contains(80) && !Manager.Config.AddData.ADVEventNotOmission) ? 82 : 80) : 79);
			}
			break;
		case ResultEnum.Club:
			if (heroine.relation == 2)
			{
				heroine.talkEvent.Add(84);
				advNo = ((heroine.talkEvent.Contains(86) && !Manager.Config.AddData.ADVEventNotOmission) ? 88 : 86);
			}
			else
			{
				advNo = (heroine.talkEvent.Contains(84) ? ((heroine.talkEvent.Contains(85) && !Manager.Config.AddData.ADVEventNotOmission) ? 87 : 85) : 84);
			}
			break;
		case ResultEnum.Study:
		{
			if (resultInfo.isFirst)
			{
				advNo = 69;
				break;
			}
			int relation2 = heroine.relation;
			advNo = ((relation2 == 2) ? ((heroine.talkEvent.Contains(71) && !Manager.Config.AddData.ADVEventNotOmission) ? 73 : 71) : ((heroine.talkEvent.Contains(70) && !Manager.Config.AddData.ADVEventNotOmission) ? 72 : 70));
			break;
		}
		case ResultEnum.Exercise:
		{
			if (resultInfo.isFirst)
			{
				advNo = 74;
				break;
			}
			int relation = heroine.relation;
			advNo = ((relation == 2) ? ((heroine.talkEvent.Contains(76) && !Manager.Config.AddData.ADVEventNotOmission) ? 78 : 76) : ((heroine.talkEvent.Contains(75) && !Manager.Config.AddData.ADVEventNotOmission) ? 77 : 75));
			break;
		}
		case ResultEnum.GoHome:
			Cycle.withHeroine = heroine;
			break;
		}
		if (advNo != -1)
		{
			yield return StartCoroutine(OpenADV(heroine, new CharaData.MotionReserver
			{
				ikMotion = ikMotion
			}, advNo));
		}
		if (resultInfo.result != ResultEnum.Chase && isChase)
		{
			Player.ChaserEnd();
		}
		ResultEnum result = resultInfo.result;
		if (result != ResultEnum.H)
		{
			int hiPolyCoordinates = -1;
			if (heroine.chaCtrl != null && heroine.chaCtrl.hiPoly)
			{
				hiPolyCoordinates = heroine.chaCtrl.fileStatus.coordinateType;
				Singleton<Manager.Character>.Instance.DeleteChara(heroine.chaCtrl);
				yield return new WaitUntil(() => heroine.chaCtrl == null);
			}
			if (hiPolyCoordinates != -1)
			{
				int num2 = heroine.isDresses.Check(false);
				if (num2 == -1)
				{
					num2 = heroine.coordinates.Length - 1;
				}
				heroine.coordinates[num2] = hiPolyCoordinates;
			}
			heroine.SetRoot(npc.gameObject);
			if (resultInfo.result == ResultEnum.GoHome)
			{
				Cycle.ActionEnd();
			}
			else
			{
				bool isNextAction = false;
				if (advNo != -1)
				{
					switch (resultInfo.result)
					{
					case ResultEnum.Lunch:
					case ResultEnum.Club:
					case ResultEnum.Study:
					case ResultEnum.Exercise:
						isNextAction = true;
						if (npc.mapNo != Player.mapNo)
						{
							npc.mapNo = Player.mapNo;
							Vector3 position = Player.position;
							Vector3 forward = Vector3.forward;
							Vector3 vector = Player.rotation * forward;
							npc.SetPositionAndRotation(position + vector, Player.eulerAngles);
						}
						break;
					}
				}
				if (isAttack || isHAttack)
				{
					isNextAction = true;
				}
				if (Player.chaser != null)
				{
					Player.chaser.mapNo = Player.mapNo;
				}
				Player.SetActive(true);
				npc.move.isStop = false;
				npcList.ForEach(delegate(NPC p)
				{
					p.SetActive(p.mapNo == Map.no);
				});
				npcList.ForEach(delegate(NPC p)
				{
					p.Pause(false);
				});
				if (fixChara != null && fixChara.mapNo == Map.no)
				{
					fixChara.SetActive(true);
				}
				if (isNextAction)
				{
					npc.AI.NextActionNoTarget();
				}
				if (isOtherInitialized)
				{
					CameraState.SetAngle(Player.eulerAngles);
				}
				if (advNo != -1)
				{
                    yield return new WaitUntil(() => this.MiniMapAndCameraActive);

                    if (Illusion.Game.Utils.Scene.IsFadeOutOK)
					{
						yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
					}
				}
			}
		}
		npc.isDefenseSpeak = true;
		Singleton<Scene>.Instance.SetFadeColorDefault();
	}

	private IEnumerator OpenADV(SaveData.Heroine heroine, CharaData.MotionReserver motionReserver, params int[] advNos)
	{
		for (int i = 0; i < advNos.Length; i++)
		{
			int advNo = advNos[i];
			Scene.Data.FadeType fadeType = Illusion.Game.Utils.Scene.SafeFadeIn();
			if (fadeType == Scene.Data.FadeType.In)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			}
			List<Program.Transfer> transferList = Program.Transfer.NewList();
			Program.SetParam(Singleton<Game>.Instance.Player, heroine, transferList);
			transferList.Add(Program.Transfer.Create(true, Command.CameraSetFov, "23"));
			string bundle = Program.FindADVBundleFilePath(advNo, heroine);
			transferList.Add(Program.Transfer.Open(bundle, advNo.ToString(), bool.TrueString));
			bool isOpenADV = false;
			yield return StartCoroutine(Program.Open(new ADV.Data
			{
				fadeInTime = 0f,
				camera = null,
				heroineList = new List<SaveData.Heroine> { heroine },
				scene = this,
				transferList = transferList,
				isParentChara = true,
				motionReserverList = new List<CharaData.MotionReserver> { motionReserver }
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
		}
	}

	public IEnumerator ChangeHEvent(Vector3 position, Quaternion rotation, int appoint = -1, bool isKokanForceInsert = false, params Tuple<int, SaveData.Heroine>[] heroines)
	{
		shortcutKey = false;
		for (int i = 0; i < heroines.Length; i++)
		{
			Tuple<int, SaveData.Heroine> tuple = heroines[i];
			tuple.Item2.talkEvent.Add(8);
			tuple.Item2.talkEvent.Add(12);
		}
		OpenHData openData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
		openData.data = new OpenHData.Data
		{
			position = position,
			rotation = rotation,
			maleCoordinateType = heroines.First().Item1,
			lstFemaleCoordinateType = heroines.Select((Tuple<int, SaveData.Heroine> p) => p.Item1).ToList(),
			lstFemale = heroines.Select((Tuple<int, SaveData.Heroine> p) => p.Item2).ToList(),
			state = 0,
			appoint = appoint,
			isKokanForceInsert = isKokanForceInsert
		};
		yield return StartCoroutine(ChangeHCommon(openData, true));
	}

	public IEnumerator ChangeHEventWithActionPoint(Vector3 position, Quaternion rotation, int appoint = -1, params Base[] targets)
	{
		shortcutKey = false;
		OpenHData openData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
		foreach (ChaControl item in from p in targets
			select p.charaData.chaCtrl into chaCtrl
			where chaCtrl != null && chaCtrl.hiPoly
			select chaCtrl)
		{
			Singleton<Manager.Character>.Instance.DeleteChara(item);
		}
		foreach (Base @base in targets)
		{
			@base.charaData.SetRoot(@base.gameObject);
		}
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC p)
		{
			p.SetActive(false);
		});
		npcList.ForEach(delegate(NPC p)
		{
			p.Pause(true);
		});
		if (fixChara != null)
		{
			fixChara.SetActive(false);
		}
		openData.data = new OpenHData.Data
		{
			position = position,
			rotation = rotation,
			lstFemale = targets.Select((Base p) => p.heroine).ToList(),
			maleCoordinateType = Player.chaCtrl.fileStatus.coordinateType,
			lstFemaleCoordinateType = targets.Select((Base p) => p.chaCtrl.fileStatus.coordinateType).ToList(),
			state = 0,
			appoint = appoint
		};
		yield return StartCoroutine(ChangeHCommon(openData, false));
		foreach (Base base2 in targets)
		{
			base2.charaData.SetRoot(base2.gameObject);
		}
	}

	public IEnumerator ChangeH(Kind kind, bool isKindPlayer, bool isLoadPeepLoom, params Base[] targets)
	{
		shortcutKey = false;
		foreach (ChaControl item in from p in targets
			select p.charaData.chaCtrl into chaCtrl
			where chaCtrl != null && chaCtrl.hiPoly
			select chaCtrl)
		{
			Singleton<Manager.Character>.Instance.DeleteChara(item);
		}
		foreach (Base @base in targets)
		{
			@base.charaData.SetRoot(@base.gameObject);
		}
		bool isInvited = false;
		bool isFound = false;
		bool isEasyPlace = false;
		NPC npc = targets.OfType<NPC>().FirstOrDefault();
		if (npc != null)
		{
			isInvited = npc.AI.actionNo == 5;
			isFound = npc.isOnanismInSight;
			isEasyPlace = npc.isEasyPlace;
		}
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC p)
		{
			p.SetActive(false);
		});
		npcList.ForEach(delegate(NPC p)
		{
			p.Pause(true);
		});
		if (fixChara != null)
		{
			fixChara.SetActive(false);
		}
		Base heroine = targets[0];
		Base baseChara = ((!isKindPlayer) ? heroine : Player);
		Vector3 position = baseChara.position;
		Quaternion rotation = baseChara.rotation;
		if (kind != null)
		{
			Collider component = kind.GetComponent<Collider>();
			if (component != null)
			{
				position = component.ClosestPointOnBounds(baseChara.position);
				position.y = baseChara.position.y;
			}
			if (kind.category == 3)
			{
				rotation = Quaternion.LookRotation((position - baseChara.position).normalized);
			}
		}
		OpenHData openData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
		openData.data = new OpenHData.Data
		{
			position = position,
			rotation = rotation,
			kind = kind,
			lstFemale = targets.Select((Base p) => p.heroine).ToList(),
			maleCoordinateType = Player.chaCtrl.fileStatus.coordinateType,
			lstFemaleCoordinateType = targets.Select((Base p) => p.chaCtrl.fileStatus.coordinateType).ToList(),
			state = heroine.state,
			isInvited = isInvited,
			isFound = isFound,
			isEasyPlace = isEasyPlace,
			isLoadPeepLoom = isLoadPeepLoom
		};
		yield return StartCoroutine(ChangeHCommon(openData, false));
		foreach (Base base2 in targets)
		{
			base2.charaData.SetRoot(base2.gameObject);
		}
	}

	public IEnumerator ChangeHSpecial(List<int> peepCategory, bool isLoadPeepLoom, Transform point, params Base[] targets)
	{
		shortcutKey = false;
		if (isLoadPeepLoom)
		{
			_isInChargeBGM = true;
		}
		foreach (ChaControl item in from p in targets
			select p.charaData.chaCtrl into chaCtrl
			where chaCtrl != null && chaCtrl.hiPoly
			select chaCtrl)
		{
			Singleton<Manager.Character>.Instance.DeleteChara(item);
		}
		foreach (Base @base in targets)
		{
			@base.charaData.SetRoot(@base.gameObject);
		}
		bool isInvited = false;
		bool isFound = false;
		bool isEasyPlace = false;
		NPC npc = targets.OfType<NPC>().FirstOrDefault();
		if (npc != null)
		{
			isInvited = npc.AI.actionNo == 5;
			isFound = npc.isOnanismInSight;
			isEasyPlace = npc.isEasyPlace;
		}
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC p)
		{
			p.SetActive(false);
		});
		npcList.ForEach(delegate(NPC p)
		{
			p.Pause(true);
		});
		if (fixChara != null)
		{
			fixChara.SetActive(false);
		}
		OpenHData openData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
		openData.data = new OpenHData.Data
		{
			position = point.position,
			rotation = point.rotation,
			lstFemale = targets.Select((Base p) => p.heroine).ToList(),
			maleCoordinateType = Player.chaCtrl.fileStatus.coordinateType,
			lstFemaleCoordinateType = targets.Select((Base p) => p.chaCtrl.fileStatus.coordinateType).ToList(),
			peepCategory = peepCategory,
			isInvited = isInvited,
			isFound = isFound,
			isEasyPlace = isEasyPlace,
			isLoadPeepLoom = isLoadPeepLoom
		};
		yield return StartCoroutine(ChangeHCommon(openData, false));
		foreach (Base base2 in targets)
		{
			base2.charaData.SetRoot(base2.gameObject);
		}
	}

	public IEnumerator ChangeHSpecial(ActionPoint point, Kind kind, params Base[] targets)
	{
		shortcutKey = false;
		foreach (ChaControl item in from p in targets
			select p.charaData.chaCtrl into chaCtrl
			where chaCtrl != null && chaCtrl.hiPoly
			select chaCtrl)
		{
			Singleton<Manager.Character>.Instance.DeleteChara(item);
		}
		foreach (Base @base in targets)
		{
			@base.charaData.SetRoot(@base.gameObject);
		}
		bool isInvited = false;
		bool isFound = false;
		bool isEasyPlace = false;
		NPC npc = targets.OfType<NPC>().FirstOrDefault();
		if (npc != null)
		{
			isInvited = npc.AI.actionNo == 5;
			isFound = npc.isOnanismInSight;
			isEasyPlace = npc.isEasyPlace;
		}
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC p)
		{
			p.SetActive(false);
		});
		npcList.ForEach(delegate(NPC p)
		{
			p.Pause(true);
		});
		if (fixChara != null)
		{
			fixChara.SetActive(false);
		}
		OpenHData openData = Singleton<Scene>.Instance.commonSpace.GetOrAddComponent<OpenHData>();
		openData.data = new OpenHData.Data
		{
			position = point.transform.position,
			rotation = point.transform.rotation,
			lstFemale = targets.Select((Base p) => p.heroine).ToList(),
			maleCoordinateType = Player.chaCtrl.fileStatus.coordinateType,
			lstFemaleCoordinateType = targets.Select((Base p) => p.chaCtrl.fileStatus.coordinateType).ToList(),
			peepCategory = point.HType,
			kind = kind,
			isInvited = isInvited,
			isFound = isFound,
			isEasyPlace = isEasyPlace,
			isLoadPeepLoom = point.IsLoadPeepLoom,
			offsetPos = point.HOffsetPos,
			offsetAngle = point.HOffsetAngle
		};
		yield return StartCoroutine(ChangeHCommon(openData, false));
		foreach (Base base2 in targets)
		{
			base2.charaData.SetRoot(base2.gameObject);
		}
	}

	private  IEnumerator ChangeHCommon(OpenHData openData, bool isEvent)
	{
		isPenetration = false;
		hSceneOtherHeroine = null;
		Singleton<Scene>.Instance.SetFadeColorDefault();
		OpenHData.Data data = openData.data as OpenHData.Data;
		data.camera = new OpenData.CameraData
		{
			position = cameraTransform.position,
			rotation = cameraTransform.rotation
		};
		data.map = Map.mapRoot;
		data.player = Singleton<Game>.Instance.Player;
		List<NPC> hNPCList = data.lstFemale.Select((SaveData.Heroine p) => p.charaBase).OfType<NPC>().ToList();
		openData.isLoad = false;
		openData.isAsync = true;
		openData.isFade = false;
		openData.fadeType = Scene.Data.FadeType.In;
		int[] peepCategorys = data.peepCategory.ToArray();
		bool isNotPeep = true;
		if (peepCategorys.Any())
		{
			isNotPeep = peepCategorys.Except(Enumerable.Range(2000, 5)).Any();
			if (isNotPeep)
			{
				isNotPeep = peepCategorys.Except(Enumerable.Range(1010, 4)).Any();
				if (isNotPeep)
				{
					isNotPeep = peepCategorys.Except(Enumerable.Range(1100, 3)).Any();
					if (!isEvent && !isNotPeep)
					{
						foreach (NPC item in hNPCList)
						{
							item.AI.SetLesbianDesire(AI.LesbianResult.Success);
						}
					}
				}
			}
			else if (peepCategorys.Contains(2000))
			{
				hNPCList.ForEach(delegate(NPC p)
				{
					p.chaCtrl.RandomChangeOfClothesLowPolyEnd();
				});
			}
		}
		int mapNo = Map.no;
		string sceneName = "H";
		yield return new WaitUntil(() => Singleton<Scene>.Instance.NowSceneNames.Contains(sceneName));
		shortcutKey = true;
		MiniMapAndCameraActive = false;
		nowCamera.gameObject.SetActive(false);
		yield return new WaitWhile(() => Singleton<Scene>.Instance.NowSceneNames.Contains(sceneName));
		yield return new WaitWhile(() => Singleton<Scene>.Instance.IsNowLoading);
		Player.charaData.SetRoot(Player.gameObject);
		if (mapNo != Map.no)
		{
			nowCamera.gameObject.SetActive(true);
			Map.Change(mapNo, Illusion.Game.Utils.Scene.SafeFadeIn());
		}
		while (Map.isMapLoading)
		{
			yield return null;
		}
		if (!nowCamera.gameObject.activeSelf)
		{
			nowCamera.gameObject.SetActive(true);
		}
		while (!Singleton<Scene>.Instance.AddSceneName.IsNullOrEmpty())
		{
			yield return null;
		}
		int frame = 0;
		float time = 0f;
		while (nowCamera != Camera.main && time < 5f)
		{
			time += Time.unscaledDeltaTime;
			yield return null;
		}
		if (isPenetration && !isNotPeep)
		{
			isNotPeep = !Map.Info.isWarning;
		}
		if (!isEvent)
		{
			if (!isNotPeep)
			{
				_isInChargeBGM = false;
				List<GateInfo> source = Map.calcGateDic[Map.no];
				MapInfo.Param value;
				GateInfo gateInfo = (from p in source
					where Map.infoDic.TryGetValue(p.mapNo, out value) && !value.isWarning
					orderby (p.pos - Player.position).sqrMagnitude
					select p).FirstOrDefault();
				if (gateInfo == null)
				{
					gateInfo = source.First();
				}
				Map.gateLinkID = gateInfo.linkID;
				Player.mapNo = gateInfo.mapNo;
				if (Player.chaser != null)
				{
					Player.chaser.mapNo = Player.mapNo;
				}
				Map.Change(gateInfo.mapNo, Scene.Data.FadeType.None);
			}
			yield return new WaitWhile(() => Map.isMapLoading);
			Player.SetActive(true);
			npcList.ForEach(delegate(NPC p)
			{
				p.SetActive(p.mapNo == Map.no);
			});
			npcList.ForEach(delegate(NPC p)
			{
				p.Pause(false);
			});
			npcList.ForEach(delegate(NPC p)
			{
				p.charaData.SetRoot(p.gameObject);
			});
			foreach (NPC item2 in hNPCList.Where((NPC p) => p.AI.actionNo != 23))
			{
				item2.AI.NextActionNoTarget();
			}
			NPC otherNPC = ((hSceneOtherHeroine != null) ? (hSceneOtherHeroine.charaBase as NPC) : null);
			if (otherNPC != null && !hNPCList.Contains(otherNPC))
			{
				otherNPC.AI.NextActionNoTarget();
			}
			if (isNotPeep)
			{
				hNPCList.SafeProc(0, delegate(NPC npc)
				{
					foreach (NPC item3 in from p in GetOtherNPC(10f, npc)
						where p != otherNPC
						where !hNPCList.Contains(p)
						select p)
					{
						if (!item3.AI.isArrival || !item3.AI.ActionNoCheck(1, 2))
						{
							if (item3.heroine.isAnger)
							{
								item3.AI.NextActionNoTarget();
							}
							else
							{
								SaveData.Heroine.HExperienceKind hExperience = item3.heroine.HExperience;
								if (hExperience == SaveData.Heroine.HExperienceKind.淫乱)
								{
									actCtrl.AddDesire(30, item3.heroine, 50);
									if (actCtrl.GetDesire(30, item3.heroine) >= 100)
									{
										if (Player.chaser == item3)
										{
											Player.ChaserEnd();
										}
										item3.AI.HesitantlyWait();
									}
									actCtrl.AddDesire(5, item3.heroine, 10);
									actCtrl.AddDesire(4, item3.heroine, 10);
									if (item3.heroine.parameter.attribute.likeGirls)
									{
										actCtrl.AddDesire(26, item3.heroine, 10);
									}
								}
								else
								{
									if (Player.chaser == item3)
									{
										Player.ChaserEnd();
									}
									item3.AI.EscapeAction(false);
									actCtrl.AddDesire(5, item3.heroine, 5);
									actCtrl.AddDesire(4, item3.heroine, 5);
									if (item3.heroine.parameter.attribute.likeGirls)
									{
										actCtrl.AddDesire(26, item3.heroine, 5);
									}
								}
							}
						}
					}
				});
			}
			if (fixChara != null && fixChara.mapNo == Map.no)
			{
				fixChara.SetActive(true);
			}
            yield return new WaitUntil(() => this.MiniMapAndCameraActive);

        }
        Singleton<Manager.Sound>.Instance.Listener = cameraTransform;
		if (!isEvent && Illusion.Game.Utils.Scene.IsFadeOutOK)
		{
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
		}
	}

	public void SceneEvent(NPC npc)
	{
		StartCoroutine(_SceneEvent(npc));
	}

	public void OtherAIPause(NPC npc, bool stop)
	{
		Func<NPC, List<AI>> func = (NPC my) => (from p in npcList
			where p != my
			select p into item
			select item.AI).ToList();
		if (otherPauseNPC != null && npc != null)
		{
			func(otherPauseNPC).ForEach(delegate(AI item)
			{
				item.Start();
			});
		}
		if (stop)
		{
			func(npc).ForEach(delegate(AI item)
			{
				item.Pause();
			});
		}
		else
		{
			func(npc).ForEach(delegate(AI item)
			{
				item.Start();
			});
		}
		otherPauseNPC = npc;
	}

	public void SetAllCharaActive(bool isActive)
	{
		Player.ChaserEnd();
		if (isActive)
		{
			Player.isActionNow = false;
			Player.mapNo = Map.no;
			Player.position = Map.warpPoint;
			Player.ItemClear();
			Player.isPopOK = true;
			Player.SetActive(Map.IsPop(Player));
			npcList.ForEach(delegate(NPC npc)
			{
				npc.isPopOK = true;
				npc.ItemClear();
				npc.AI.FirstAction();
				npc.ReStart();
			});
			if (fixChara != null)
			{
				fixChara.isPopOK = true;
				fixChara.ItemClear();
				fixChara.SetActive(Map.IsPop(fixChara));
			}
			return;
		}
		Player.isActionNow = true;
		Player.isPopOK = false;
		Player.SetActive(false);
		npcList.ForEach(delegate(NPC npc)
		{
			npc.isPopOK = false;
			npc.Stop();
			if (npc.chaCtrl != null)
			{
				npc.chaCtrl.RandomChangeOfClothesLowPolyEnd();
			}
		});
		if (fixChara != null)
		{
			fixChara.isPopOK = false;
			fixChara.SetActive(false);
		}
	}

	public SaveData.Heroine GetHSceneOtherHeroine(SaveData.Heroine target)
	{
		if (!Game.isAdd20)
		{
			return null;
		}
		if (target.isAnger)
		{
			return null;
		}
		if (target.fixCharaID < 0)
		{
			return null;
		}
		Base chara = target.charaBase;
		if (chara == null)
		{
			return null;
		}
		int mapNo = chara.mapNo;
		if (Map.no != mapNo)
		{
			return null;
		}
		float distance = 10f;
		return hSceneOtherHeroine = (from p in (from p in npcList
				where p != chara
				where !p.heroine.isAnger
				where p.mapNo == mapNo
				where p.heroine.HExperience >= SaveData.Heroine.HExperienceKind.慣れ
				where !p.AI.isArrival || !p.AI.ActionNoCheck(1, 2)
				select new
				{
					heroine = p.heroine,
					sqrDis = (p.position - chara.position).sqrMagnitude
				} into p
				orderby p.sqrDis
				select p).TakeWhile(p => p.sqrDis < distance * distance)
			select p.heroine).FirstOrDefault(delegate(SaveData.Heroine heroine)
		{
			float num = ((heroine.HExperience != SaveData.Heroine.HExperienceKind.慣れ) ? 40 : 20);
			num += (float)actCtrl.GetDesire(5, heroine) * 0.5f;
			num += (float)heroine.lewdness * 0.5f;
			if (heroine.parameter.attribute.bitch)
			{
				num += 10f;
			}
			return Illusion.Utils.ProbabilityCalclator.DetectFromPercent(num);
		});
	}

	public NPC[] GetOtherNPC(float distance, Base chara)
	{
		if (chara == null)
		{
			return new NPC[0];
		}
		return (from p in (from p in npcList
				where p != chara
				where p.mapNo == chara.mapNo
				select new
				{
					chara = p,
					sqrDis = (p.position - chara.position).sqrMagnitude
				} into p
				orderby p.sqrDis
				select p).TakeWhile(p => p.sqrDis < distance * distance)
			select p.chara).ToArray();
	}

	public IEnumerator NPCLoadAll(bool isShuffle)
	{
		Singleton<Manager.Character>.Instance.enableCharaLoadGCClear = false;
		foreach (KeyValuePair<int, List<WaitPoint>> item4 in PosSet.waitPointDic)
		{
			item4.Value.ForEach(delegate(WaitPoint p)
			{
				p.UnReserve();
			});
		}
		Singleton<Game>.Instance.HeroineList.ForEach(delegate(SaveData.Heroine p)
		{
			p.PopInit();
		});
		List<NPC> reloadList = new List<NPC>();
		List<Tuple<NPC, SaveData.Heroine>> replaceList = new List<Tuple<NPC, SaveData.Heroine>>();
		if (!isShuffle)
		{
			reloadList.AddRange(npcList);
		}
		else
		{
			Singleton<Game>.Instance.HeroineList.ForEach(delegate(SaveData.Heroine item)
			{
				item.SetRoot(null);
			});
			List<NPC> list = npcList.ToList();
			List<SaveData.Heroine> list2 = new List<SaveData.Heroine>();
			bool flag = Cycle.nowWeek == Cycle.Week.Saturday;
			IEnumerable<SaveData.Heroine> source = from p in Singleton<Game>.Instance.HeroineList.Shuffle()
				where p.fixCharaID == 0
				select p;
			if (flag)
			{
				source = from p in source
					orderby p.relation descending, p.favor descending
					select p;
			}
			if (!Manager.Config.AddData.OtherClassRegisterMax)
			{
				int playerClass = Singleton<Game>.Instance.Player.schoolClass;
				source = source.Where((SaveData.Heroine p) => p.schoolClass == playerClass || p.schoolClassIndex < 5);
			}
			source = source.Take(Mathf.Min(38, Manager.Config.ActData.MaxCharaNum));
			foreach (SaveData.Heroine item2 in source)
			{
				int num = list.FindIndex((NPC p) => p.charaData == item2);
				if (num != -1)
				{
					reloadList.Add(list[num]);
					list.RemoveAt(num);
				}
				else
				{
					list2.Add(item2);
				}
			}
			int j;
			for (j = 0; j < list.Count && j < list2.Count; j++)
			{
				replaceList.Add(Tuple.Create(list[j], list2[j]));
			}
			if (list.Count != list2.Count)
			{
				if (list.Count > list2.Count)
				{
					for (; j < list.Count; j++)
					{
						NPC nPC = list[j];
						npcList.Remove(nPC);
						UnityEngine.Object.Destroy(nPC.gameObject);
					}
				}
				else if (list.Count < list2.Count)
				{
					npcList.AddRange(from heroine in list2.Skip(j)
						select NPC.Create(base.transform, heroine));
				}
			}
		}
		otherPauseNPC = null;
		npcList.ForEach(delegate(NPC npc)
		{
			npc.AI.Reset(true);
		});
		npcList.ForEach(delegate(NPC npc)
		{
			npc.LoadStart();
		});
		Singleton<Scene>.Instance.DrawImageAndProgress(0f);
		int count = npcList.Count + 1;
		foreach (var item3 in npcList.Select((NPC p, int i) => new { p, i }))
		{
			Singleton<Scene>.Instance.DrawImageAndProgress((float)(item3.i + 1) / (float)count);
			yield return new WaitUntil(() => item3.p.initialized);
			if (reloadList.Contains(item3.p))
			{
				item3.p.charaData.SetRoot(item3.p.gameObject);
				continue;
			}
			foreach (Tuple<NPC, SaveData.Heroine> item5 in replaceList)
			{
				if (item5.Item1 == item3.p)
				{
					item5.Item1.Replace(item5.Item2);
					break;
				}
			}
		}
		if (Cycle.nowWeek == Cycle.Week.Saturday)
		{
			Localize.Translate.Manager.SetCulture(delegate
			{
				npcList = (from p in npcList
					orderby p.heroine.relation descending, p.heroine.favor descending, p.heroine.parameter.fullname
					select p).ToList();
			});
		}
		_actCtrl.NextTime();
		Singleton<PlayerAction>.Instance.NextTime();
		yield return StartCoroutine(FixCharaSetting());
		Singleton<Scene>.Instance.DrawImageAndProgress(1f);
		yield return null;
		Singleton<Scene>.Instance.DrawImageAndProgress();
		Singleton<Manager.Character>.Instance.enableCharaLoadGCClear = true;
	}

	private IEnumerator FixCharaSetting()
	{
		FixEventScheduler.Result result = null;
		SaveData.Heroine fixHeroine = FindFixEventHeroine(out result);
		if (fixHeroine == null)
		{
			if (Cycle.nowWeek != Cycle.Week.Saturday)
			{
				fixHeroine = Singleton<Game>.Instance.HeroineList.Shuffle().FirstOrDefault((SaveData.Heroine p) => p.isTeacher);
			}
			else
			{
				fixHeroine = Singleton<Game>.Instance.HeroineList.Where((SaveData.Heroine p) => p.isTeacher).Shuffle().FirstOrDefault((SaveData.Heroine p) => !_saturdayTeachers.Contains(p));
				_saturdayTeachers.Add(fixHeroine);
			}
		}
		if (fixChara != null)
		{
			if (fixHeroine.isTeacher)
			{
				if (!fixChara.isCharaLoad)
				{
					FixCharaDelete();
				}
			}
			else if (fixChara.isCharaLoad != result.param.isVisible)
			{
				FixCharaDelete();
			}
		}
		yield return StartCoroutine(FixCharaSetPosition(fixHeroine, result, null));
	}

	private IEnumerator FixCharaSetPosition(SaveData.Heroine fixHeroine, FixEventScheduler.Result result, int? mapNo = null)
	{
		if (result == null)
		{
			string layerName;
			switch (fixHeroine.fixCharaID)
			{
			case -1:
				layerName = "FixHealth";
				if (!mapNo.HasValue && fixHeroine.talkEvent.Contains(4) && Illusion.Utils.ProbabilityCalclator.DetectFromPercent(30f))
				{
					mapNo = 20;
				}
				break;
			case -2:
				layerName = "FixMath";
				if (!mapNo.HasValue && fixHeroine.talkEvent.Contains(4) && Illusion.Utils.ProbabilityCalclator.DetectFromPercent(20f))
				{
					mapNo = 36;
				}
				break;
			case -3:
				layerName = "FixPhysical";
				break;
			case -4:
				layerName = "FixHomeRoom";
				break;
			default:
				layerName = null;
				break;
			}
			List<Tuple<WaitPoint, int>> list = new List<Tuple<WaitPoint, int>>();
			foreach (List<WaitPoint> value in PosSet.waitPointDic.Values)
			{
				foreach (WaitPoint item in value)
				{
					int num = item.parameterList.FindIndex((WaitPoint.Parameter p) => p.layer == layerName);
					if (num != -1)
					{
						list.Add(Tuple.Create(item, num));
					}
				}
			}
			Tuple<WaitPoint, int> tuple = list.Shuffle().FirstOrDefault();
			if (mapNo.HasValue)
			{
				foreach (Tuple<WaitPoint, int> item2 in list)
				{
					if (item2.Item1.MapNo == mapNo.GetValueOrDefault() && mapNo.HasValue)
					{
						tuple = item2;
						break;
					}
				}
			}
			result = new FixEventScheduler.Result(tuple.Item1, tuple.Item2, null);
		}
		if (fixChara != null)
		{
			fixChara.SetEventSchedulerResult(result);
			fixChara.Replace(fixHeroine);
		}
		else
		{
			fixChara = Fix.Create(base.transform, fixHeroine);
			fixChara.SetEventSchedulerResult(result);
			if (result.param != null)
			{
				fixChara.isCharaLoad = result.param.isVisible;
			}
		}
		fixChara.LoadStart();
		yield return new WaitUntil(() => fixChara.initialized);
		result.wp.Reserve(fixChara);
		result.wp.SetWait(result.layerIndex);
	}

	public void ShortcutKeyRegulate(bool enable)
	{
		foreach (ShortcutKey.Proc proc in _actionShortcutKey.procList)
		{
			KeyCode keyCode = proc.keyCode;
			if (keyCode == KeyCode.F3)
			{
				proc.enabled = enable;
			}
		}
	}

	public void OpenMapMoveAllMenu()
	{
		string levelName = "MapSelectMenu";
		if (!IsShortcutKey(levelName))
		{
			return;
		}
		OpenMenu("action/menu/mapselect.unity3d", levelName, delegate
		{
			MapSelectMenuScene rootComponent = Scene.GetRootComponent<MapSelectMenuScene>(levelName);
			if (!(rootComponent == null))
			{
				rootComponent.visibleType = MapSelectMenuScene.VisibleType.Route;
				rootComponent.baseMap = Map;
				rootComponent.result = MapSelectMenuScene.ResultType.EnterMapMove;
			}
		});
	}

	public void OpenStatus()
	{
		string levelName = "Status";
		if (IsShortcutKey(levelName))
		{
			OpenMenu(string.Empty, levelName, null);
		}
	}

	public void NextCycle()
	{
		if (IsShortcutKey(string.Empty))
		{
			Cycle.ActionEnd();
		}
	}

	private bool IsShortcutKey(string levelName)
	{
		if (isEventNow)
		{
			return false;
		}
		if (Program.isADVProcessing)
		{
			return false;
		}
		if (ShortcutKey.IsReglate(levelName))
		{
			return false;
		}
		Scene instance = Singleton<Scene>.Instance;
		if (!instance.AddSceneName.IsNullOrEmpty())
		{
			return false;
		}
		if (instance.IsOverlap)
		{
			return false;
		}
		return true;
	}

	private void OpenMenu(string assetBundleName, string levelName, Action onLoad)
	{
		Singleton<Scene>.Instance.LoadReserve(new Scene.Data
		{
			assetBundleName = assetBundleName,
			levelName = levelName,
			isAdd = true,
			onLoad = onLoad
		}, false);
	}
}
