using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame;
using ActionGame.Communication;
using Illusion;
using Illusion.Extensions;
using Illusion.Game;
using Illusion.Game.Elements.EasyLoader;
using IllusionUtility.GetUtility;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TalkScene : BaseLoader
{
	private class GaugeInfo
	{
		public Image image { get; protected set; }

		public int min { get; protected set; }

		public int max { get; protected set; }

		public int now { get; protected set; }

		public int target { get; set; }

		public bool isUpdate
		{
			get
			{
				return now != target;
			}
		}

		public float inverseLerp
		{
			get
			{
				return Mathf.InverseLerp(min, max, now);
			}
		}

		public SingleAssignmentDisposable interval { get; set; }

		public SingleAssignmentDisposable changed { get; set; }

		public GaugeInfo(Image _image, int _min, int _max, int _now)
		{
			image = _image;
			min = _min;
			max = _max;
			now = _now;
			target = _now;
			interval = new SingleAssignmentDisposable();
			changed = new SingleAssignmentDisposable();
			image.fillAmount = inverseLerp;
		}

		public virtual void Update()
		{
			int num = Mathf.Clamp(Mathf.Abs(target - now) / 10, 1, 5);
			now = Mathf.Clamp((target <= now) ? (now - num) : (now + num), min, max);
			image.fillAmount = inverseLerp;
		}

		public void Dispose()
		{
			if (changed.Disposable != null)
			{
				changed.Dispose();
			}
		}
	}

	private class FavorGaugeInfo : GaugeInfo
	{
		public Image[] imageArray { get; protected set; }

		public int stage { get; protected set; }

		public FavorGaugeInfo(Image[] _imageArray, int _stage, int _min, int _max, int _now)
			: base(_imageArray[_stage], _min, _max, _now)
		{
			imageArray = _imageArray;
			SetStage(_stage);
		}

		public override void Update()
		{
			base.Update();
		}

		public void SetStage(int _stage)
		{
			base.image = imageArray[_stage];
			stage = _stage;
			switch (stage)
			{
			case 1:
				imageArray[0].fillAmount = 1f;
				imageArray[2].fillAmount = 0f;
				break;
			case 2:
				imageArray[0].fillAmount = 0f;
				imageArray[1].fillAmount = 1f;
				break;
			default:
				imageArray[1].fillAmount = 0f;
				imageArray[2].fillAmount = 0f;
				break;
			}
			Update();
		}
	}

	private class LeaveAloneDisposableInfo
	{
		private SingleAssignmentDisposable[] arrayDisposable;

		public SingleAssignmentDisposable timer
		{
			get
			{
				return arrayDisposable[0];
			}
			set
			{
				arrayDisposable[0] = value;
			}
		}

		public SingleAssignmentDisposable wait
		{
			get
			{
				return arrayDisposable[1];
			}
			set
			{
				arrayDisposable[1] = value;
			}
		}

		public LeaveAloneDisposableInfo()
		{
			arrayDisposable = new SingleAssignmentDisposable[2];
		}

		public void End()
		{
			for (int i = 0; i < 2; i++)
			{
				if (arrayDisposable[i] != null)
				{
					arrayDisposable[i].Dispose();
				}
				arrayDisposable[i] = null;
			}
		}
	}

	private class TouchPointInfo
	{
		private bool enableCol;

		public GameObject obj { get; private set; }

		public Collider col { get; private set; }

		public string tag { get; private set; }

		public int layer { get; private set; }

		public TouchPointInfo(GameObject _obj, Collider _col, string _tag, int _layer)
		{
			obj = _obj;
			col = _col;
			enableCol = col.enabled;
			col.enabled = true;
			tag = obj.tag;
			obj.tag = _tag;
			layer = obj.layer;
			obj.layer = _layer;
		}

		private void Reset()
		{
			col.enabled = enableCol;
			obj.tag = tag;
			obj.layer = layer;
		}
	}

	private delegate void touchFunc(string _kind, Vector3 _pos);

	private delegate void enterExitFunc(string _kind);

	private class ColDisposableInfo
	{
		private touchFunc touchFunc;

		private enterExitFunc enterFunc;

		private enterExitFunc exitFunc;

		public Collider col { get; private set; }

		private SingleAssignmentDisposable disposableTouch { get; set; }

		private SingleAssignmentDisposable disposableEnter { get; set; }

		private SingleAssignmentDisposable disposableExit { get; set; }

		private string name
		{
			get
			{
				return col.tag.Replace("Com/Hit/", string.Empty);
			}
		}

		private int mode { get; set; }

		private TalkScene talkScene { get; set; }

		public ColDisposableInfo(Collider _col, touchFunc _touchFunc, enterExitFunc _enterFunc, enterExitFunc _exitFunc, int _mode, TalkScene _talk)
		{
			col = _col;
			touchFunc = _touchFunc;
			enterFunc = _enterFunc;
			exitFunc = _exitFunc;
			mode = _mode;
			talkScene = _talk;
		}

		public void Start()
		{
            //End();
            //disposableTouch = new SingleAssignmentDisposable();
            //disposableTouch.Disposable = (from _ in col.OnPointerDownAsObservable()
            //	where !EventSystem.current.IsPointerOverGameObject()
            //	where ((1 << talkScene.touchMode) & mode) != 0
            //	select _).Subscribe(delegate
            //{
            //RaycastHit hitInfo;
            //  bool flag = col.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 10f);
            //touchFunc(name, (!flag) ? Vector3.zero : hitInfo.point);
            //}).AddTo(col);
            //disposableEnter = new SingleAssignmentDisposable();
            //disposableEnter.Disposable = (from _ in col.OnMouseEnterAsObservable()
            //where !EventSystem.current.IsPointerOverGameObject()
            //where ((1 << talkScene.touchMode) & mode) != 0
            //select _).Subscribe(delegate
            //{
            //enterFunc(name);
            //}).AddTo(col);
            //disposableExit = new SingleAssignmentDisposable();
            //disposableExit.Disposable = (from _ in col.OnMouseExitAsObservable()
            //where !EventSystem.current.IsPointerOverGameObject()
            //where ((1 << talkScene.touchMode) & mode) != 0
            //select _).Subscribe(delegate
            //{
            //exitFunc(name);
            //}).AddTo(col);
            }

        public void End()
		{
			if (disposableTouch != null)
			{
				disposableTouch.Dispose();
			}
			disposableTouch = null;
			if (disposableEnter != null)
			{
				disposableEnter.Dispose();
			}
			disposableEnter = null;
			if (disposableExit != null)
			{
				disposableExit.Dispose();
			}
			disposableExit = null;
		}
	}

	private class TouchDisposableInfo
	{
		public SingleAssignmentDisposable wait { get; set; }

		public bool check
		{
			get
			{
				return wait != null;
			}
		}

		public void End()
		{
			if (wait != null)
			{
				wait.Dispose();
			}
			wait = null;
		}
	}

	[Serializable]
	private class CharaCorrectHeightCamera
	{
		[Serializable]
		private struct Pair
		{
			public Vector3 min;

			public Vector3 max;
		}

		[SerializeField]
		private Pair pos;

		[SerializeField]
		private Pair ang;

		public bool Calculate(float _shape, out Vector3 pos, out Vector3 ang)
		{
			pos = MotionIK.GetShapeLerpPositionValue(_shape, this.pos.min, this.pos.max);
			ang = MotionIK.GetShapeLerpAngleValue(_shape, this.ang.min, this.ang.max);
			return true;
		}
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
				for (int i = 0; i < data.heroineList.Count; i++)
				{
					SaveData.Heroine heroine = data.heroineList[i];
					CharaData.MotionReserver motionReserver = data.motionReserverList.SafeGet(i);
					scenario.commandController.AddChara(i, new CharaData(heroine, heroine.chaCtrl, scenario, motionReserver));
				}
				scenario.ChangeCurrentChara(0);
			}
			if (data.camera != null)
			{
				scenario.BackCamera.transform.SetPositionAndRotation(data.camera.position, data.camera.rotation);
			}
			TalkScene talkScene = base.mono as TalkScene;
			foreach (KeyValuePair<string, Func<int>> aDVVariable in talkScene.ADVVariables)
			{
				scenario.Vars[aDVVariable.Key] = new ValData(ValData.Convert(aDVVariable.Value(), typeof(int)));
			}
			foreach (KeyValuePair<string, Func<string>> aDVVariableStr in talkScene.ADVVariableStrs)
			{
				scenario.Vars[aDVVariableStr.Key] = new ValData(ValData.Convert(aDVVariableStr.Value(), typeof(string)));
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
			if ((base.mono as TalkScene).cvInfo is ChangeValueSelectInfo)
			{
				((base.mono as TalkScene).cvInfo as ChangeValueSelectInfo).select = (int)aDVScene.Scenario.Vars["Result"].o;
			}
			(base.mono as TalkScene).selectNo = (int)aDVScene.Scenario.Vars["Result"].o;
		}

		public override void WaitEndProc()
		{
		}
	}

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Canvas canvasFemale;

	[SerializeField]
	private Button buttonTalk;

	[SerializeField]
	private Button buttonListen;

	[SerializeField]
	private Button buttonEvent;

	[SerializeField]
	private Sprite[] spriteAction;

	[SerializeField]
	private Button[] buttonTalkContents;

	[SerializeField]
	private GameObject objTalkContentsRoot;

	[SerializeField]
	private Button buttonEnd;

	[SerializeField]
	private Button[] buttonEventContents;

	[SerializeField]
	private GameObject objEventContentsRoot;

	[SerializeField]
	private Image imageTime;

	[SerializeField]
	private Image[] imageFavor;

	[SerializeField]
	private Image imageLewdness;

	[SerializeField]
	private Sprite[] spriteLewdness;

	[SerializeField]
	private Camera cameraMap;

	[SerializeField]
	private RawImage rawImageMap;

	[SerializeField]
	private Button[] buttonTouch;

	[SerializeField]
	private Sprite[] spriteTouch;

	[SerializeField]
	private Canvas canvasBack;

	[SerializeField]
	private Texture2D[] texCursor;

	[SerializeField]
	private RawImage rawFace;

	[SerializeField]
	private Image imageState;

	[SerializeField]
	private Sprite[] spriteState;

	private SaveData.Heroine m_TargetHeroine;

	public const string KEY_NAME = "Result";

	private ChangeValueAbstractInfo m_CVInfo;

	public int selectNo = -1;

	private int baseMask;

	private bool isIntroductionOnly;

	private Illusion.Game.Elements.EasyLoader.Motion motion = new Illusion.Game.Elements.EasyLoader.Motion();

	private IKMotion ikmotion = new IKMotion();

	private Camera nowCamera;

	private bool oldAnger;

	private int heroineEvent = -1;

	private Transform transVoice;

	private LeaveAloneDisposableInfo ladInfo;

	private Vector3 oldPos = Vector3.zero;

	private List<TouchPointInfo> lstTouchPoint;

	private List<GameObject> lstAddCol;

	private List<ColDisposableInfo> lstColDisposable;

	private int m_touchMode;

	private Dictionary<int, int> dicTouchCount = new Dictionary<int, int>();

	private TouchDisposableInfo touchDisposableInfo = new TouchDisposableInfo();

	private CrossFade crossFade;

	[SerializeField]
	private CharaCorrectHeightCamera correctCamera = new CharaCorrectHeightCamera();

	[SerializeField]
	private Transform[] cameraTransform;

	[SerializeField]
	private float fieldOfView = 23f;

	private float backupFOV = 45f;

	private bool isUpdateCamera = true;

	[SerializeField]
	private int addFavor;

	[SerializeField]
	private int addLewdness;

	private bool isSuccess;

	private int endADVNo = -1;

	private RenderTexture renderTextureMap;

	private CameraEffectorConfig cameraEffectorConfig;

	private GaugeInfo timeGaugeInfo;

	private FavorGaugeInfo favorGaugeInfo;

	private GaugeInfo lewdnessGaugeInfo;

	private bool isDesire;

	public SaveData.Heroine targetHeroine
	{
		get
		{
			return m_TargetHeroine;
		}
		set
		{
			m_TargetHeroine = value;
		}
	}

	public NecessaryInfo necessaryInfo { get; set; }

	public ResultInfo resultInfo { get; private set; }

	public IKMotion resultIK
	{
		get
		{
			return ikmotion;
		}
	}

	public PassingInfo passingInfo
	{
		get
		{
			PassingInfo passingInfo = new PassingInfo();
			passingInfo.player = Singleton<Game>.Instance.Player;
			passingInfo.heroine = targetHeroine;
			passingInfo.info = necessaryInfo;
			passingInfo.map = ((!(Singleton<Game>.Instance.actScene != null)) ? GetComponent<BaseMap>() : Singleton<Game>.Instance.actScene.Map);
			return passingInfo;
		}
	}

	private ActionGame.Communication.Info info { get; set; }

	private bool activeTalkContents
	{
		get
		{
			return objTalkContentsRoot.activeSelf;
		}
		set
		{
			objTalkContentsRoot.SetActive(value);
			buttonTalk.image.sprite = spriteAction[value ? 1 : 0];
		}
	}

	private bool activeEventContents
	{
		get
		{
			return objEventContentsRoot.activeSelf;
		}
		set
		{
			objEventContentsRoot.SetActive(value);
			buttonEvent.image.sprite = spriteAction[4 + (value ? 1 : 0)];
		}
	}

	public Dictionary<string, Func<int>> ADVVariables { get; private set; }

	public Dictionary<string, Func<string>> ADVVariableStrs { get; private set; }

	public ChangeValueAbstractInfo cvInfo
	{
		get
		{
			return m_CVInfo;
		}
	}

	public int touchMode
	{
		get
		{
			return m_touchMode;
		}
		private set
		{
			m_touchMode = value;
			for (int i = 0; i < buttonTouch.Length; i++)
			{
				buttonTouch[i].image.sprite = spriteTouch[((i == value) ? 1 : 0) + i * 2];
			}
		}
	}

	public event Action otherInitialize = delegate
	{
	};

	protected override void Awake()
	{
		base.Awake();
		ADVVariables = new Dictionary<string, Func<int>> { 
		{
			"Result",
			() => int.MaxValue
		} };
		ADVVariableStrs = new Dictionary<string, Func<string>>
		{
			{
				"NowMap",
				() => (!(Singleton<Game>.Instance.actScene == null)) ? Singleton<Game>.Instance.actScene.Map.no.ToString() : string.Empty
			},
			{
				"NowMapName",
				() => (Singleton<Game>.Instance.actScene == null) ? string.Empty : ((Singleton<Game>.Instance.actScene.Map.no != -1) ? Singleton<Game>.Instance.actScene.Map.ConvertMapName(Singleton<Game>.Instance.actScene.Map.no) : "なし")
			},
			{
				"NowCycle",
				() => (!(Singleton<Game>.Instance.actScene == null)) ? Singleton<Game>.Instance.actScene.Cycle.nowType.ToString() : string.Empty
			},
			{
				"NowCycleName",
				() => (!(Singleton<Game>.Instance.actScene == null)) ? Singleton<Game>.Instance.actScene.Cycle.nowType.GetName() : string.Empty
			},
			{
				"NowWeek",
				() => (!(Singleton<Game>.Instance.actScene == null)) ? Singleton<Game>.Instance.actScene.Cycle.nowWeek.ToString() : string.Empty
			},
			{
				"NowWeekName",
				() => (!(Singleton<Game>.Instance.actScene == null)) ? Singleton<Game>.Instance.actScene.Cycle.nowWeek.GetName() : string.Empty
			}
		};
		ParameterList.Add(new ADVParam(this));
	}

	private void OnDestroy()
	{
		ParameterList.Remove(this);
		if (!isIntroductionOnly)
		{
			if (nowCamera != null)
			{
				nowCamera.cullingMask = baseMask;
				nowCamera.fieldOfView = backupFOV;
			}
			if (renderTextureMap != null)
			{
				cameraMap.targetTexture = null;
				rawImageMap.texture = null;
				UnityEngine.Object.DestroyImmediate(renderTextureMap);
			}
			cameraEffectorConfig.SetFog(true);
			cameraEffectorConfig.SetDOF(true);
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
		}
	}

	private IEnumerator Start()
	{
		resultInfo = new ResultInfo();
		canvas.enabled = false;
		canvasBack.enabled = false;
		cameraMap.enabled = false;
		yield return new WaitWhile(() => targetHeroine == null);
		if (Illusion.Game.Utils.Scene.OpenTutorial(1))
		{
			yield return new WaitWhile(Illusion.Game.Utils.Scene.IsTutorial);
		}
		if (necessaryInfo.state == 0 && Singleton<Game>.Instance.Player.charaBase != null && targetHeroine.charaBase != null)
		{
			Vector3 position = Singleton<Game>.Instance.Player.charaBase.position;
			Vector3 position2 = targetHeroine.charaBase.position;
			position.y = position2.y;
			targetHeroine.charaBase.rotation = Quaternion.LookRotation(position - position2, Vector3.up);
		}
		SaveData.Heroine heroine = targetHeroine;
		oldAnger = heroine.isAnger;
		info = new ActionGame.Communication.Info();
		info.Init(passingInfo);
		if (Camera.main != null)
		{
			nowCamera = Camera.main;
		}
		else
		{
			GameObject gameObject = Load<GameObject>("camera/action.unity3d", "ActionCamera", true);
			nowCamera = gameObject.GetComponent<Camera>();
			AssetBundleManager.UnloadAssetBundle("camera/action.unity3d", false);
		}
		renderTextureMap = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
		renderTextureMap.antiAliasing = 8;
		cameraMap.targetTexture = renderTextureMap;
		rawImageMap.texture = renderTextureMap;
		Singleton<Manager.Sound>.Instance.Listener = nowCamera.transform;
		if (!necessaryInfo.isHAttack)
		{
			int _eventNo = -1;
			List<Program.Transfer> introductionADV = info.GetIntroductionADV(ref isIntroductionOnly, ref _eventNo, out m_CVInfo, true);
			if (isIntroductionOnly)
			{
				StartADV(introductionADV);
				ReflectChangeValue();
				Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
				yield break;
			}
		}
		Singleton<Scene>.Instance.SetFadeColor(Color.black);
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
		canvasFemale.enabled = true;
		Texture2D texFace = new Texture2D(240, 320);
		texFace.LoadImage(heroine.charFile.facePngData);
		rawFace.texture = texFace;
		rawFace.enabled = true;
		UpdateUI(true);
		this.otherInitialize();
		Transform root = heroine.transform;
		GameObject parent = ((!(Singleton<Game>.Instance.actScene != null)) ? base.gameObject : Singleton<Game>.Instance.actScene.gameObject);
		ChaFileControl chaFile = new ChaFileControl();
		ChaFile.CopyChaFile(chaFile, (!(heroine.chaCtrl == null)) ? heroine.chaCtrl.chaFile : heroine.charFile);
		ChaControl female = Singleton<Character>.Instance.CreateFemale(parent, 0, chaFile);
		heroine.SetRoot(female.gameObject);
		Singleton<Character>.Instance.loading.Load(0, female);
		yield return new WaitUntil(() => Singleton<Character>.Instance.loading.IsEnd(female));
		if (root != null)
		{
			female.transform.SetPositionAndRotation(root.position, root.rotation);
		}
		yield return new WaitWhile(() => !Singleton<Communication>.Instance.isInit);
		int bgm = ((!heroine.isAnger) ? Mathf.Clamp(heroine.relation, 0, 2) : 3);
		Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM((BGM)(4 + bgm)));
		heroine.chaCtrl.animBody.runtimeAnimatorController = Singleton<Game>.Instance.advAnimePack.ac;
		AnimePlay((!heroine.isAnger) ? "通常立ち" : "怒り立ち");
		Singleton<Communication>.Instance.ChangeExpression(heroine.FixCharaIDOrPersonality, (!heroine.isAnger) ? "笑顔" : "怒り１", heroine.chaCtrl);
		heroine.chaCtrl.ChangeLookNeckPtn(3, 0f);
		ladInfo = new LeaveAloneDisposableInfo();
		InitTouch();
		canvasBack.enabled = true;
		cameraMap.enabled = true;
		baseMask = nowCamera.cullingMask;
		nowCamera.cullingMask = 1 << LayerMask.NameToLayer("Chara");
		(from _ in this.UpdateAsObservable()
			where isUpdateCamera
			select _).Subscribe(delegate
		{
			cameraTransform[1].SetPositionAndRotation(heroine.transform.position, heroine.transform.rotation);
			Vector3 pos;
			Vector3 ang;
			correctCamera.Calculate(heroine.chaCtrl.GetShapeBodyValue(0), out pos, out ang);
			cameraTransform[0].position = cameraTransform[0].position + pos;
			cameraTransform[0].eulerAngles = cameraTransform[0].eulerAngles + ang;
			nowCamera.transform.SetPositionAndRotation(cameraTransform[2].position, cameraTransform[2].rotation);
			cameraTransform[0].localPosition = Vector3.zero;
			cameraTransform[0].localEulerAngles = Vector3.zero;
			cameraMap.transform.SetPositionAndRotation(nowCamera.transform.position, nowCamera.transform.rotation);
		});
		backupFOV = nowCamera.fieldOfView;
		nowCamera.fieldOfView = fieldOfView;
		cameraEffectorConfig = Singleton<Game>.Instance.cameraEffector.config;
		cameraEffectorConfig.SetFog(false);
		cameraEffectorConfig.SetDOF(false);
		heroine.chaCtrl.ChangeLookEyesTarget(0, nowCamera.transform);
		heroine.chaCtrl.ChangeLookNeckTarget(0, nowCamera.transform);
		crossFade = nowCamera.GetComponent<CrossFade>();
		buttonTalk.OnClickAsObservable().Subscribe(delegate
		{
			activeTalkContents = !activeTalkContents;
			activeEventContents = false;
		});
		buttonListen.OnClickAsObservable().Subscribe(delegate
		{
			CommandFunc(3);
		});
		buttonEvent.OnClickAsObservable().Subscribe(delegate
		{
			activeEventContents = !activeEventContents;
			activeTalkContents = false;
		});
		buttonTalkContents[0].OnClickAsObservable().Subscribe(delegate
		{
			CommandFunc(0);
		});
		buttonTalkContents[1].OnClickAsObservable().Subscribe(delegate
		{
			CommandFunc(1);
		});
		buttonTalkContents[2].OnClickAsObservable().Subscribe(delegate
		{
			CommandFunc(2);
		});
		buttonEnd.OnClickAsObservable().Subscribe(delegate
		{
			CommandFunc(-1);
		});
		for (int i = 0; i < buttonEventContents.Length; i++)
		{
			int id = i + 10;
			buttonEventContents[i].OnClickAsObservable().Subscribe(delegate
			{
				CommandFunc(id);
			});
		}
		buttonTouch[0].OnClickAsObservable().Subscribe(delegate
		{
			touchMode = 0;
		});
		buttonTouch[1].OnClickAsObservable().Subscribe(delegate
		{
			touchMode = 1;
		});
		touchMode = 0;
		timeGaugeInfo = new GaugeInfo(imageTime, 0, targetHeroine.talkTimeMax, targetHeroine.talkTime);
		(from _ in Observable.Interval(TimeSpan.FromMilliseconds(25.0))
			where timeGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			timeGaugeInfo.Update();
		}).AddTo(this);
		timeGaugeInfo.changed.Disposable = this.ObserveEveryValueChanged((TalkScene x) => x.targetHeroine.talkTime).Subscribe(delegate(int time)
		{
			timeGaugeInfo.target = time;
		}).AddTo(this);
		favorGaugeInfo = new FavorGaugeInfo(imageFavor, Mathf.Max(0, heroine.relation), 0, 100, targetHeroine.favor);
		(from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where favorGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			favorGaugeInfo.Update();
		}).AddTo(this);
		favorGaugeInfo.changed.Disposable = this.ObserveEveryValueChanged((TalkScene x) => x.targetHeroine.favor).Subscribe(delegate(int x)
		{
			favorGaugeInfo.target = x;
		}).AddTo(this);
		lewdnessGaugeInfo = new GaugeInfo(imageLewdness, 0, 100, targetHeroine.lewdness);
		(from _ in Observable.Interval(TimeSpan.FromMilliseconds(50.0))
			where lewdnessGaugeInfo.isUpdate
			select _).Subscribe(delegate
		{
			lewdnessGaugeInfo.Update();
			imageLewdness.sprite = spriteLewdness[(!(lewdnessGaugeInfo.inverseLerp < 1f)) ? 1u : 0u];
		}).AddTo(this);
		lewdnessGaugeInfo.changed.Disposable = this.ObserveEveryValueChanged((TalkScene x) => x.targetHeroine.lewdness).Subscribe(delegate(int x)
		{
			lewdnessGaugeInfo.target = x;
		}).AddTo(this);
		yield return null;
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
		Observable.FromCoroutine(Introduction).Subscribe().AddTo(this);
	}

	private void InitTouch()
	{
		lstColDisposable = new List<ColDisposableInfo>();
		SetCharaTagLayer();
		string[,] array = new string[3, 3]
		{
			{ "cf_j_hand_L", "communication/hit_00.unity3d", "com_hit_hand_L" },
			{ "cf_j_hand_R", "communication/hit_00.unity3d", "com_hit_hand_R" },
			{ "cf_j_head", "communication/hit_00.unity3d", "com_hit_head" }
		};
		int[] array2 = new int[3] { 1, 1, 3 };
		ChaControl chaCtrl = targetHeroine.chaCtrl;
		Transform transform = chaCtrl.objBodyBone.transform;
		lstAddCol = new List<GameObject>();
		for (int i = 0; i < array.GetLength(0); i++)
		{
			GameObject gameObject = transform.FindLoop(array[i, 0]);
			if (gameObject == null)
			{
				continue;
			}
			GameObject gameObject2 = CommonLib.LoadAsset<GameObject>(array[i, 1], array[i, 2], true, string.Empty);
			if (!(gameObject2 == null))
			{
				gameObject2.transform.SetParent(gameObject.transform, false);
				lstAddCol.Add(gameObject2);
				Collider[] componentsInChildren = gameObject2.GetComponentsInChildren<Collider>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					lstColDisposable.Add(new ColDisposableInfo(componentsInChildren[j], TouchFunc, EnterFunc, ExitFunc, array2[i], this));
				}
			}
		}
		dicTouchCount = new Dictionary<int, int>
		{
			{ 0, 0 },
			{ 1, 0 },
			{ 2, 0 },
			{ 3, 0 },
			{ 10, 0 },
			{ 11, 0 }
		};
	}

	private void SetCharaTagLayer()
	{
		string[,] array = new string[2, 3]
		{
			{ "cf_hit_bust02_L", "Com/Hit/MuneL", "Chara" },
			{ "cf_hit_bust02_R", "Com/Hit/MuneR", "Chara" }
		};
		int[] array2 = new int[2] { 3, 3 };
		ChaControl chaCtrl = targetHeroine.chaCtrl;
		Transform transform = chaCtrl.objBodyBone.transform;
		int length = array.GetLength(0);
		lstTouchPoint = new List<TouchPointInfo>();
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			GameObject gameObject = transform.FindLoop(array[i, num++]);
			if (!(gameObject == null))
			{
				Collider component = gameObject.GetComponent<Collider>();
				if (!(component == null))
				{
					TouchPointInfo item = new TouchPointInfo(gameObject, component, array[i, num++], LayerMask.NameToLayer(array[i, num++]));
					lstTouchPoint.Add(item);
					lstColDisposable.Add(new ColDisposableInfo(component, TouchFunc, EnterFunc, ExitFunc, array2[i], this));
				}
			}
		}
	}

	private IEnumerator Introduction()
	{
		SaveData.Heroine heroine = targetHeroine;
		bool isIntroductionOnly = false;
		bool isFirst = !heroine.talkEvent.Contains(0);
		if (!necessaryInfo.introSkip)
		{
			if (necessaryInfo.isHAttack)
			{
				StartADV(info.GetEventFromHeroineADV(3, out m_CVInfo, ref heroineEvent));
				Observable.FromCoroutine(HeroineEventWait).Subscribe().AddTo(this);
				if (isFirst)
				{
					heroine.talkEvent.Add(0);
					heroine.talkEvent.Add(1);
				}
				yield break;
			}
			if (isFirst && !heroine.isAnger)
			{
				StartADV(info.GetEncounterADV(Singleton<Game>.Instance.saveData.metPersonality.Contains(heroine.personality) && !Manager.Config.AddData.ADVEventNotOmission));
				heroine.talkEvent.Add(0);
				heroine.talkEvent.Add(1);
				Singleton<Game>.Instance.saveData.metPersonality.Add(heroine.personality);
			}
			else
			{
				int _eventNo = -1;
				StartADV(info.GetIntroductionADV(ref isIntroductionOnly, ref _eventNo, out m_CVInfo));
				if (isFirst)
				{
					heroine.talkEvent.Add(0);
					heroine.talkEvent.Add(1);
				}
			}
		}
		isUpdateCamera = false;
		yield return null;
		yield return Program.Wait("Talk");
		if (isIntroductionOnly)
		{
			Singleton<Scene>.Instance.UnLoad();
			yield break;
		}
		heroine.chaCtrl.ChangeLookNeckPtn(3, 0f);
		StartSelection();
	}

	private void UpdateUI(bool _gauge = false)
	{
		SaveData.Heroine heroine = targetHeroine;
		if (heroine.isAnger)
		{
			int[] source = new int[2]
			{
				0,
				heroine.isGirlfriend ? 2 : 0
			};
			for (int i = 0; i < buttonEventContents.Length; i++)
			{
				buttonEventContents[i].gameObject.SetActive(source.Contains(i));
			}
		}
		else
		{
			int[] source2 = info.ConfirmEvent();
			for (int j = 0; j < buttonEventContents.Length; j++)
			{
				buttonEventContents[j].gameObject.SetActive(source2.Contains(j));
			}
		}
		int num = Mathf.Max(heroine.relation, 0);
		imageState.enabled = true;
		imageState.sprite = spriteState[num * 2 + (heroine.isAnger ? 1 : 0)];
		if (_gauge)
		{
			imageTime.fillAmount = Mathf.InverseLerp(0f, targetHeroine.talkTimeMax, targetHeroine.talkTime);
			imageFavor[0].fillAmount = Mathf.InverseLerp(0f, 100f, targetHeroine.favor);
			imageLewdness.fillAmount = Mathf.InverseLerp(0f, 100f, targetHeroine.lewdness);
		}
	}

	private void StartSelection()
	{
		isUpdateCamera = true;
		UpdateUI();
		canvas.enabled = true;
		SetLeaveAloneTimer();
		foreach (ColDisposableInfo item in lstColDisposable)
		{
			item.Start();
		}
	}

	private void SetLeaveAloneTimer()
	{
		ladInfo.End();
		oldPos = Input.mousePosition;
		IObservable<bool> other = from _ in (from _ in this.UpdateAsObservable()
				select oldPos == Input.mousePosition).Do(delegate
			{
				oldPos = Input.mousePosition;
			})
			where _
			select _;
		IObservable<bool> other2 = from _ in this.UpdateAsObservable()
			select oldPos != Input.mousePosition into _
			where _
			select _;
		ladInfo.timer = new SingleAssignmentDisposable();
		ladInfo.timer.Disposable = Observable.Timer(TimeSpan.FromSeconds(30.0)).SkipUntil(other).TakeUntil(other2)
			.Repeat()
			.Subscribe(delegate
			{
				ladInfo.timer.Dispose();
				if (!canvas.enabled)
				{
					ladInfo.End();
				}
				else
				{
					LeaveAloneReactionInfo leaveAloneVoice = info.GetLeaveAloneVoice();
					LoadVoice(leaveAloneVoice.assetbundle, leaveAloneVoice.file, leaveAloneVoice.facialExpression, leaveAloneVoice.pose);
					ladInfo.wait = new SingleAssignmentDisposable();
					ladInfo.wait.Disposable = Observable.FromCoroutine(LeaveAloneEnd).Subscribe().AddTo(this);
				}
			})
			.AddTo(this);
	}

	private void CommandFunc(int _kind)
	{
		bool flag = true;
		if (touchDisposableInfo.check)
		{
			AnimePlay((!targetHeroine.isAnger) ? "通常立ち" : "怒り立ち");
		}
		switch (_kind)
		{
		case 0:
		case 1:
		case 2:
			StartADV(ActionGame.Communication.Info.Group.Talk, _kind);
			Singleton<Communication>.Instance.DecreaseTalkTime(targetHeroine, 1);
			break;
		case 3:
			StartADV(ActionGame.Communication.Info.Group.Listen);
			Singleton<Communication>.Instance.DecreaseTalkTime(targetHeroine, 1);
			break;
		case -1:
			StartADV(info.GetEndConversationADV(targetHeroine.isAnger, true, out endADVNo));
			canvas.enabled = false;
			Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
			flag = false;
			break;
		case 10:
		{
			bool isAnger = targetHeroine.isAnger;
			targetHeroine.anger = Mathf.Max(0, targetHeroine.anger - UnityEngine.Random.Range(30, 50));
			targetHeroine.isAnger = targetHeroine.anger != 0;
			StartADV(ActionGame.Communication.Info.Group.Apologize);
			Singleton<Communication>.Instance.DecreaseTalkTime(targetHeroine, 1);
			if (isAnger != targetHeroine.isAnger)
			{
				Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM((BGM)(4 + Mathf.Clamp(targetHeroine.relation, 0, 2))));
			}
			break;
		}
		default:
		{
			Singleton<Communication>.Instance.DecreaseTalkTime(targetHeroine, 1);
			int _derive = -1;
			int command = _kind - 10;
			StartADV(info.GetEventADV(command, ref _derive, ref resultInfo.isFirst));
			int[] array = new int[10] { 1, 3, 4, 5, 6, 7, 8, 9, 10, 2 };
			targetHeroine.talkCommand.Add(Array.FindIndex(array, (int v) => v == command));
			if (_derive != -1)
			{
				resultInfo.result = (ResultEnum)_derive;
				switch (resultInfo.result)
				{
				case ResultEnum.H:
					targetHeroine.talkCommand.Remove(1);
					break;
				case ResultEnum.Study:
					Singleton<Game>.Instance.Player.intellect = Mathf.Clamp(Singleton<Game>.Instance.Player.intellect + RandomValue(1, 1, 1, 1, 1, 1, 2, 2, 2, 3), 0, 100);
					break;
				case ResultEnum.Exercise:
					Singleton<Game>.Instance.Player.physical = Mathf.Clamp(Singleton<Game>.Instance.Player.physical + RandomValue(1, 1, 1, 1, 1, 1, 2, 2, 2, 3), 0, 100);
					break;
				}
				Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
				flag = false;
				if (resultInfo.result == ResultEnum.Lunch)
				{
					targetHeroine.isLunch = true;
					Singleton<Game>.Instance.actScene.actCtrl.SetDesire(targetHeroine, 3);
				}
			}
			else if (command == 1 && targetHeroine.isGirlfriend)
			{
				favorGaugeInfo.SetStage(2);
			}
			break;
		}
		}
		ladInfo.End();
		foreach (ColDisposableInfo item in lstColDisposable)
		{
			item.End();
		}
		touchDisposableInfo.End();
		if (transVoice != null)
		{
			Singleton<Manager.Voice>.Instance.Stop(transVoice);
			transVoice = null;
		}
		activeTalkContents = false;
		activeEventContents = false;
		if (flag)
		{
			Observable.FromCoroutine(CommandWait).Subscribe().AddTo(this);
		}
	}

	private void TouchFunc(string _kind, Vector3 _pos)
	{
		bool flag = touchMode == 0;
		int num = -1;
		int num2 = ((!flag) ? 10 : 0);
		switch (_kind)
		{
		case "Head":
			num = (flag ? 2 : 0);
			break;
		case "Cheek":
			num = (flag ? 3 : 0);
			break;
		case "HandL":
		case "HandR":
			num = ((!flag) ? (-1) : 0);
			break;
		case "MuneL":
		case "MuneR":
			num = 1;
			break;
		}
		if (num == -1)
		{
			return;
		}
		dicTouchCount[num + num2]++;
		int num3 = dicTouchCount[num + num2];
		float num4 = 0f;
		float percent = Mathf.Clamp((float)(num3 - 1) * 50f - num4, 0f, 100f);
		int num5 = ((num3 != 1) ? ((num3 >= 4) ? 2 : (Illusion.Utils.ProbabilityCalclator.DetectFromPercent(percent) ? 1 : 0)) : 0);
		if (num5 == 1)
		{
			dicTouchCount[num + num2] += 2;
		}
		TouchReactionInfo touchVoice = info.GetTouchVoice(flag, num, num5, out m_CVInfo);
		ReflectChangeValue();
		List<Program.Transfer> endConversationADV = info.GetEndConversationADV(oldAnger, false, out endADVNo);
		if (endConversationADV != null)
		{
			canvas.enabled = false;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
			if (transVoice != null)
			{
				Singleton<Manager.Voice>.Instance.Stop(transVoice);
				transVoice = null;
			}
			StartADV(endConversationADV);
			Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
			return;
		}
		Singleton<Manager.Voice>.Instance.Stop(targetHeroine.voiceNo);
		LoadVoice(touchVoice.assetbundle, touchVoice.file, touchVoice.facialExpression, touchVoice.pose);
		ladInfo.End();
		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
		foreach (ColDisposableInfo item in lstColDisposable)
		{
			item.End();
		}
		touchDisposableInfo.End();
		touchDisposableInfo.wait = new SingleAssignmentDisposable();
		touchDisposableInfo.wait.Disposable = Observable.FromCoroutine(TouchEnd).Subscribe().AddTo(this);
	}

	private void EnterFunc(string _kind)
	{
		Cursor.SetCursor((touchMode != 0) ? texCursor[1] : texCursor[0], new Vector2(24f, 24f), CursorMode.ForceSoftware);
	}

	private void ExitFunc(string _kind)
	{
		Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
	}

	private int RandomValue(params int[] _array)
	{
		return _array.Shuffle().First();
	}

	private void ReflectChangeValue()
	{
		if (isDesire)
		{
			ActionControl actCtrl = Singleton<Game>.Instance.actScene.actCtrl;
			actCtrl.AddDesire(4, targetHeroine, RandomValue(0, 1, 1, 1, 1, 2, 2, 2, 3, 3));
			actCtrl.AddDesire(5, targetHeroine, RandomValue(0, 1, 1, 1, 1, 2, 2, 2, 3, 3) * targetHeroine.relation);
			int value = ((!targetHeroine.parameter.attribute.likeGirls) ? RandomValue(0, 0, 0, 0, 0, 1, 1, 1, 2, 2) : RandomValue(2, 2, 2, 2, 3, 3, 3, 4, 4, 5));
			actCtrl.AddDesire(26, targetHeroine, value);
			isDesire = false;
		}
		if (m_CVInfo != null)
		{
			SaveData.Heroine heroine = targetHeroine;
			isSuccess |= Singleton<Communication>.Instance.ChangeParam(targetHeroine, m_CVInfo);
			if (m_CVInfo is ChangeValueSelectInfo)
			{
				ChangeValueSelectInfo changeValueSelectInfo = m_CVInfo as ChangeValueSelectInfo;
				Singleton<Manager.Sound>.Instance.Play(Manager.Sound.Type.SystemSE, "sound/data/systemse/00.unity3d", (!changeValueSelectInfo.isSuccess) ? "sse_01_01" : "sse_01_00");
			}
		}
	}

	private void LoadVoice(string _assetbundle, string _file, string _expression, string _pose, bool _isCheck = false)
	{
		SaveData.Heroine heroine = targetHeroine;
		transVoice = Illusion.Game.Utils.Voice.Play(new Illusion.Game.Utils.Voice.Setting
		{
			assetBundleName = _assetbundle,
			assetName = _file,
			no = heroine.voiceNo,
			pitch = heroine.voicePitch
		});
		heroine.chaCtrl.SetVoiceTransform(transVoice);
		Singleton<Communication>.Instance.ChangeExpression(heroine.FixCharaIDOrPersonality, _expression, heroine.chaCtrl);
		AnimePlay(_pose, _isCheck);
	}

	private void AnimePlay(string _pose, bool _isCheck = false)
	{
		if (_pose.IsNullOrEmpty())
		{
			return;
		}
		SaveData.Heroine heroine = targetHeroine;
		string[] poseInfo = Singleton<Communication>.Instance.GetPoseInfo(heroine.FixCharaIDOrPersonality, _pose);
		if (!poseInfo.IsNullOrEmpty())
		{
			if (motion.Setting(heroine.chaCtrl.animBody, poseInfo[0], poseInfo[1], poseInfo[2], _isCheck))
			{
				motion.Play(heroine.chaCtrl.animBody);
				heroine.chaCtrl.animBody.Update(0f);
			}
			ikmotion.Setting(heroine.chaCtrl, poseInfo.SafeGet(3), poseInfo.SafeGet(4), poseInfo[2], _isCheck);
			if ((bool)crossFade)
			{
				crossFade.FadeStart();
			}
		}
	}

	private void StartADV(ActionGame.Communication.Info.Group _group, int _command = -1)
	{
		StartADV(info.GetADV(out m_CVInfo, _group, _command));
		if (_group == ActionGame.Communication.Info.Group.Talk && _command == 2)
		{
			isDesire = true;
		}
	}

	private void StartADV(List<Program.Transfer> _list)
	{
		Observable.FromCoroutine((CancellationToken _) => new WaitUntil(() => Singleton<Scene>.Instance.NowSceneNames[0] == "Talk")).Subscribe(delegate
		{
			Transform transform = ((!isIntroductionOnly) ? cameraTransform[2] : nowCamera.transform);
			StartCoroutine(Program.Open(new Data
			{
				fadeInTime = 0f,
				position = Vector3.zero,
				rotation = Quaternion.identity,
				camera = new OpenData.CameraData
				{
					position = transform.position,
					rotation = transform.rotation
				},
				scene = this,
				transferList = _list,
				heroineList = new List<SaveData.Heroine> { targetHeroine },
				motionReserverList = new List<CharaData.MotionReserver>
				{
					new CharaData.MotionReserver
					{
						ikMotion = ikmotion
					}
				}
			}));
		}).AddTo(this);
	}

	private IEnumerator CommandWait()
	{
		isUpdateCamera = false;
		yield return Program.Wait("Talk");
		ReflectChangeValue();
		List<Program.Transfer> heroineInvitation = info.GetEventFromHeroineADV(out m_CVInfo, ref heroineEvent);
		if (heroineInvitation != null)
		{
			StartADV(heroineInvitation);
			Observable.FromCoroutine(HeroineEventWait).Subscribe().AddTo(this);
			yield break;
		}
		List<Program.Transfer> end = info.GetEndConversationADV(oldAnger, false, out endADVNo);
		if (end != null)
		{
			StartADV(end);
			canvas.enabled = false;
			Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
			yield break;
		}
		List<Program.Transfer> heroineE = info.GetEventFromHeroineADV(out m_CVInfo, ref heroineEvent);
		if (heroineE != null)
		{
			StartADV(heroineE);
			Observable.FromCoroutine(HeroineEventWait).Subscribe().AddTo(this);
			yield break;
		}
		if (!Singleton<Manager.Voice>.Instance.IsVoiceCheck(targetHeroine.voiceNo))
		{
			InterludeReactionInfo interludeVoice = info.GetInterludeVoice(m_CVInfo);
			if (interludeVoice.isInterlude)
			{
				LoadVoice(interludeVoice.assetbundle, interludeVoice.file, interludeVoice.facialExpression, interludeVoice.pose);
			}
		}
		m_CVInfo = null;
		StartSelection();
	}

	private IEnumerator HeroineEventWait()
	{
		isUpdateCamera = false;
		yield return null;
		yield return Program.Wait("Talk");
		int[] array = new int[7] { 1, 2, 3, 4, 6, 7, 9 };
		int idx = Array.IndexOf(array, heroineEvent);
		ResultEnum[] res = new ResultEnum[7]
		{
			ResultEnum.H,
			ResultEnum.Lunch,
			ResultEnum.Club,
			ResultEnum.GoHome,
			ResultEnum.Study,
			ResultEnum.Exercise,
			ResultEnum.Divorce
		};
		resultInfo.result = ((idx >= 0) ? res[idx] : ResultEnum.None);
		targetHeroine.talkCommand.Add(heroineEvent);
		switch (heroineEvent)
		{
		case 0:
			if (selectNo == 0)
			{
				targetHeroine.isGirlfriend = true;
				favorGaugeInfo.SetStage(2);
				if (!targetHeroine.isFirstGirlfriend)
				{
					targetHeroine.isFirstGirlfriend = true;
					Singleton<Game>.Instance.rankSaveData.girlfriendCount++;
				}
			}
			else
			{
				targetHeroine.talkTime = 0;
			}
			targetHeroine.confessed = true;
			break;
		case 1:
			resultInfo.result = ((selectNo == 0) ? ResultEnum.H : ResultEnum.None);
			if (resultInfo.result == ResultEnum.H)
			{
				targetHeroine.talkCommand.Remove(1);
				targetHeroine.talkEvent.Add(12);
				targetHeroine.talkEvent.Add(8);
				Singleton<Scene>.Instance.UnLoad();
				yield break;
			}
			if (necessaryInfo.isHAttack)
			{
				Singleton<Scene>.Instance.UnLoad();
				yield break;
			}
			break;
		case 4:
			if (selectNo == 0)
			{
				yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
				Singleton<Scene>.Instance.UnLoad();
				yield break;
			}
			resultInfo.result = ResultEnum.None;
			break;
		case 5:
			if (selectNo == 0)
			{
				targetHeroine.isDate = true;
				targetHeroine.talkEvent.Add(9);
				targetHeroine.talkEvent.Add(13);
				Singleton<Game>.Instance.actScene.Cycle.dateHeroine = targetHeroine;
			}
			break;
		case 8:
			targetHeroine.isStaff = true;
			Singleton<Game>.Instance.rankSaveData.staffCount++;
			Singleton<Game>.Instance.saveData.clubReport.staffAdd = 1;
			break;
		case 9:
			targetHeroine.isGirlfriend = false;
			targetHeroine.talkTime = 0;
			Singleton<Scene>.Instance.UnLoad();
			yield break;
		default:
			if (selectNo == 0)
			{
				if (resultInfo.result == ResultEnum.Lunch)
				{
					resultInfo.isFirst = !targetHeroine.talkEvent.Contains(79);
					targetHeroine.isLunch = true;
					Singleton<Game>.Instance.actScene.actCtrl.SetDesire(targetHeroine, 3);
				}
				switch (resultInfo.result)
				{
				case ResultEnum.Lunch:
				case ResultEnum.Club:
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
					break;
				case ResultEnum.Study:
					Singleton<Game>.Instance.Player.intellect = Mathf.Clamp(Singleton<Game>.Instance.Player.intellect + RandomValue(1, 1, 1, 1, 1, 1, 2, 2, 2, 3), 0, 100);
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
					break;
				case ResultEnum.Exercise:
					Singleton<Game>.Instance.Player.physical = Mathf.Clamp(Singleton<Game>.Instance.Player.physical + RandomValue(1, 1, 1, 1, 1, 1, 2, 2, 2, 3), 0, 100);
					yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
					break;
				}
				Singleton<Scene>.Instance.UnLoad();
				yield break;
			}
			resultInfo.result = ResultEnum.None;
			break;
		}
		ReflectChangeValue();
		List<Program.Transfer> end = info.GetEndConversationADV(oldAnger, false, out endADVNo);
		if (end != null)
		{
			StartADV(end);
			Observable.FromCoroutine(TalkEnd).Subscribe().AddTo(this);
		}
		else
		{
			StartSelection();
		}
	}

	private IEnumerator InterludeWait()
	{
		isUpdateCamera = false;
		yield return null;
		yield return Program.Wait("Talk");
		StartSelection();
	}

	private IEnumerator TalkEnd()
	{
		isUpdateCamera = false;
		yield return null;
		yield return Program.Wait("Talk");
		if (timeGaugeInfo != null)
		{
			timeGaugeInfo.Dispose();
		}
		if (favorGaugeInfo != null)
		{
			favorGaugeInfo.Dispose();
		}
		if (lewdnessGaugeInfo != null)
		{
			lewdnessGaugeInfo.Dispose();
		}
		if (!targetHeroine.isTalkPoint && isSuccess)
		{
			targetHeroine.isTalkPoint = true;
			Singleton<Game>.Instance.saveData.ClubPointAdd(Singleton<Communication>.Instance.correctionInfo.club[1]);
		}
		int num = endADVNo;
		if (num == 10)
		{
			targetHeroine.talkEvent.Add(2);
			targetHeroine.favor = 0;
		}
		switch (resultInfo.result)
		{
		case ResultEnum.Lunch:
		case ResultEnum.Club:
		case ResultEnum.GoHome:
		case ResultEnum.Study:
		case ResultEnum.Exercise:
			yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.In));
			break;
		}
		Singleton<Scene>.Instance.UnLoad();
	}

	private IEnumerator LeaveAloneEnd()
	{
		isUpdateCamera = false;
		yield return null;
		yield return new WaitWhile(() => transVoice != null);
		SetLeaveAloneTimer();
	}

	private IEnumerator TouchEnd()
	{
		isUpdateCamera = false;
		SaveData.Heroine heroine = targetHeroine;
		yield return null;
		yield return new WaitWhile(() => transVoice != null);
		AnimePlay((!heroine.isAnger) ? "通常立ち" : "怒り立ち");
		m_CVInfo = null;
		StartSelection();
	}
}
