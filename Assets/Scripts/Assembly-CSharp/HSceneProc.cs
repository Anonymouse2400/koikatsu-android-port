using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame;
using ActionGame.H;
using ActionGame.MapObject;
using ActionGame.Point;
using ChaCustom;
using Config;
using H;
using HSceneUtility;
using Illusion;
using Illusion.Component;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Localize.Translate;
using Manager;
using RootMotion.FinalIK;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HSceneProc : BaseLoader
{
	public enum EExperience
	{
		funare = 0,
		nare = 1
	}

	[Serializable]
	public class PathName
	{
		public string assetpath;

		public string file;
	}

	[Serializable]
	public class Category
	{
		public int category;

		public string fileMove;
	}

	[Serializable]
	public class MaleParameter
	{
		public PathName path = new PathName();

		public List<int> lstIdLayer = new List<int>();

		public bool isHitObject;

		public string fileHit;

		public string fileHitObject;

		public string fileLookAt;

		public string fileMotionNeck;

		public string fileMetaballCtrl;
	}

	[Serializable]
	public class FemaleParameter
	{
		public PathName path = new PathName();

		public List<int> lstIdLayer = new List<int>();

		public List<int> lstFrontAndBehind = new List<int>();

		public bool isHitObject;

		public string fileHit;

		public string fileHitObject;

		public bool isYure;

		public string fileYure;

		public string fileDynamicBoneRef;

		public string fileMotionNeck;

		public string fileSe;

		public string fileSiruPaste;

		public string fileAibuEnableMotion;

		public string fileReaction;

		public string fileItem;

		public bool isAnal;

		public string nameCamera;
	}

	[Serializable]
	public class AnimationListInfo
	{
		public int id;

		public HFlag.EMode mode = HFlag.EMode.none;

		public string nameAnimation;

		public List<Category> lstCategory = new List<Category>();

		public int posture;

		public int kindHoushi;

		public int isExperience;

		public PathName pathMapObjectNull = new PathName();

		public int useDesk;

		public int useChair;

		public PathName pathMaleBase = new PathName();

		public MaleParameter paramMale = new MaleParameter();

		public PathName pathFemaleBase = new PathName();

		public FemaleParameter paramFemale = new FemaleParameter();

		public PathName pathFemaleBase1 = new PathName();

		public FemaleParameter paramFemale1 = new FemaleParameter();

		public int numCtrl = -1;

		public int sysTaii;

		public string nameCameraIdle;

		public string nameCameraKiss;

		public List<int> lstAibuSpecialItem = new List<int>();

		public bool isFemaleInitiative;

		public int houshiLoopActionW = -1;

		public int houshiLoopActionS = -1;

		public bool isSplash;

		public int numMainVoiceID = -1;

		public int numSubVoiceID = -1;

		public int numVoiceKindID = -1;

		public int numMaleSon = -1;

		public int numFemaleUpperCloth = -1;

		public int numFemaleLowerCloth = -1;

		public int numFemaleUpperCloth1 = -1;

		public int numFemaleLowerCloth1 = -1;

		public bool isRelease;

		public int stateRestriction;

		public int[] mainHoushi3PShortVoicePtns = new int[4];

		public int[] subHoushi3PShortVoicePtns = new int[4];
	}

	[Serializable]
	public class MapObjectNullInfo
	{
		public GameObject obj;

		public Vector3 pos;

		public Quaternion rot;

		public int layer;
	}

	[Serializable]
	public class MapObjectNull
	{
		public List<MapObjectNullInfo> lstDesk = new List<MapObjectNullInfo>();

		public List<MapObjectNullInfo> lstChair = new List<MapObjectNullInfo>();
	}

	public class MaleConfig
	{
		public bool visibleSimple;

		public Color color;

		public bool cloth;

		public bool accessoryMain;

		public bool accessorySub;

		public bool shoes;
	}

	public class LightData
	{
		public Light light;

		public Color initColor = Color.white;

		public Vector2 initRot = Vector2.zero;

		public float initIntensity = 1f;

		public Vector2 calcRot = Vector2.zero;
	}

	public class MapDependent
	{
		public int socks = -1;

		public int shoes = -1;

		public int glove = -1;
	}

	public enum VoicePlayShuffleKind
	{
		PoseChange = 0
	}

	public int finishADV;

	public HFlag flags;

	public CrossFade crossfade;

	public HSprite sprite;

	public ParentObjectCtrl parentObjectBaseFemale;

	public ParentObjectCtrl parentObjectBaseMale;

	public ParentObjectCtrl parentObjectBaseFemale1;

	public ParentObjectCtrl parentObjectFemale;

	public ParentObjectCtrl parentObjectMale;

	public ParentObjectCtrl parentObjectFemale1;

	public HandCtrl hand;

	public HandCtrl hand1;

	public Lookat_dan lookDan;

	public YureCtrl yure;

	public YureCtrl yure1;

	public GameObject objMetaball;

	public SubjectiveChangeSystem subjective;

	public HitCollisionEnableCtrl hitcollisionFemale;

	public HitCollisionEnableCtrl hitcollisionFemale1;

	public HitCollisionEnableCtrl hitcollisionMale;

	public HitReaction hitreaction;

	public HitReaction hitreaction1;

	public SiruPasteCtrl siru;

	public SiruPasteCtrl siru1;

	public FaceListCtrl face;

	public FaceListCtrl face1;

	public HVoiceCtrl voice;

	public HParticleCtrl particle;

	public HParticleCtrl particle1;

	public HMotionEyeNeckFemale eyeneckFemale;

	public HMotionEyeNeckFemale eyeneckFemale1;

	public HMotionEyeNeckMale eyeneckMale;

	public AutoRely rely;

	public HSeCtrl se;

	public DynamicBoneReferenceCtrl dynamicCtrl;

	public DynamicBoneReferenceCtrl dynamicCtrl1;

	public AnimatorLayerCtrl alCtrl;

	public AnimatorLayerCtrl alCtrl1;

	public GameObject objMoveAxis;

	public HSceneGuideObject guideObject;

	public PhysicsRaycaster raycaster;

	public string nameMap = "教室1-1";

	public Cycle.Type defType;

	public OpenHData.Data dataH;

	public int countOrg;

	public clothesFileControl clothCustomCtrl;

	public Light lightCamera;

	public HScene.AddParameter addParameter = new HScene.AddParameter();

	public GameScreenShot gss;

	public HExpSprite spriteExp;

	[Header("---< アニメーション変更 >---")]
	public List<int> categorys = new List<int>();

	public HFlag.EMode modeChange;

	public int idList;

	[Button("ChangeAnimatorTest", "アニメーション変更", new object[] { })]
	public int ChangeAnimatorButton;

	[Button("ChangeCloth", "服全部消す", new object[] { })]
	[Space]
	public int ChangeClothButton;

	[Button("MapObjectDelete", "椅子とか？消す", new object[] { })]
	[Space]
	public int MapObjectDeleteButton;

	[Header("時間帯")]
	[Button("SetDayTime", "昼間に設定", new object[] { })]
	public int SetDayTimeButton;

	[Button("SetEvening", "夕方に設定", new object[] { })]
	public int SetEveningButton;

	[Button("SetNight", "夜に設定", new object[] { })]
	public int SetNightButton;

	[Label("取得したいマップオブジェクトの名前")]
	[Space]
	public string nameMapObject;

	[Button("GetMapObjectKind", "マップオブジェクトの取得", new object[] { })]
	public int GetMapObjectKindButton;

	private List<ChaControl> lstFemale = new List<ChaControl>();

	private List<MotionIK> lstMotionIK = new List<MotionIK>();

	private ChaControl male;

	private MetaballCtrl meta;

	private ItemObject item = new ItemObject();

	private ActionMap map;

	private Kind kindMap;

	private List<AnimationListInfo>[] lstAnimInfo = new List<AnimationListInfo>[8];

	private List<AnimationListInfo>[] lstUseAnimInfo = new List<AnimationListInfo>[8];

	private List<HActionBase> lstProc = new List<HActionBase>();

	private Dictionary<int, List<Collider>> dicCharaCollder = new Dictionary<int, List<Collider>>();

	[SerializeField]
	private MapObjectNull mapObjectNull = new MapObjectNull();

	private MaleConfig maleConfig = new MaleConfig();

	private List<bool> lstOldFemaleVisible = new List<bool>();

	private List<bool> lstOldMaleVisible = new List<bool>();

	private Vector3 initPos = Vector3.zero;

	private Quaternion initRot = Quaternion.identity;

	private List<int> lstInitCategory = new List<int>();

	private bool isInvited;

	private Vector3 loadLocalPositon = Vector3.zero;

	private Vector3 loadLocalRotation = Vector3.zero;

	private LightData lightData = new LightData();

	private bool isEasyPlace;

	private int appointAction = -1;

	private List<int> lstCoordinateType = new List<int> { 0 };

	private List<List<int>> clothStates;

	private List<List<bool>> accessoryStates;

	private int maleCoordinateType;

	private bool isFirstPlayMasturbation;

	private int hScenePlayCount;

	private ShuffleRand[] voicePlayShuffle = new ShuffleRand[5];

	private int progressAdd = 7;

	private Dictionary<int, Dictionary<int, Data.Param>> _uiTranslater;

	public bool isEnd { get; private set; }

	private Dictionary<int, Dictionary<int, Data.Param>> uiTranslater
	{
		get
		{
			return this.GetCache(ref _uiTranslater, () => base.gameObject.LoadTranslater(Localize.Translate.Manager.SCENE_ID.H_POSTURE));
		}
	}

	public void OnBack()
	{
		Singleton<Scene>.Instance.UnLoad();
	}

	public void OnTitle()
	{
		Observable.FromCoroutine((CancellationToken _) => Illusion.Game.Utils.Scene.ReturnTitle()).StartAsCoroutine();
	}

	private IEnumerator Start()
	{
		ActionScene actscene = Singleton<Game>.Instance.actScene;
		flags.isDebug = false;
		flags.isUseImmediatelyFinishButton = false;
		base.enabled = false;
		if (actscene == null)
		{
			map = base.gameObject.AddComponent<ActionMap>();
		}
		else
		{
			map = actscene.GetComponent<ActionMap>();
		}
		yield return new WaitUntil(() => map.infoDic != null);
		flags.firstHEasy = Manager.Config.AddData.firstHEasy;
		Singleton<Character>.Instance.enableCorrectHandSize = false;
		lstInitCategory.Clear();
		int freeHMapNo = -1;
		int freeHTimeZone = -1;
		int freeHStatus = -1;
		int freeHStage1 = -1;
		int freeHStage2 = -1;
		bool isLoadPeepMap = false;
		if (dataH != null)
		{
			flags.lstHeroine = dataH.lstFemale;
			flags.player = dataH.player;
			flags.newHeroine = dataH.newHeroione;
			initPos = dataH.position;
			initRot = dataH.rotation;
			flags.isFreeH = dataH.isFreeH;
			isInvited = dataH.isInvited;
			flags.isMasturbationFound = dataH.isFound;
			freeHMapNo = dataH.mapNoFreeH;
			freeHTimeZone = dataH.timezoneFreeH;
			freeHStatus = dataH.statusFreeH;
			freeHStage1 = dataH.stageFreeH1;
			freeHStage2 = dataH.stageFreeH2;
			isEasyPlace = dataH.isEasyPlace;
			isLoadPeepMap = dataH.isLoadPeepLoom;
			appointAction = dataH.appoint;
			isFirstPlayMasturbation = dataH.isFirstPlayMasturbation;
			lstCoordinateType.Clear();
			lstCoordinateType.AddRange(dataH.lstFemaleCoordinateType);
			clothStates = dataH.clothStates;
			accessoryStates = dataH.accessoryStates;
			maleCoordinateType = dataH.maleCoordinateType;
			hScenePlayCount = dataH.hScenePlayCount;
			categorys.Clear();
			if (dataH.peepCategory.Any((int c) => MathfEx.IsRange(1100, c, 1199, true)))
			{
				categorys.AddRange(dataH.peepCategory);
			}
			else if (dataH.kind != null)
			{
				kindMap = dataH.kind;
				categorys.AddRange(kindMap.categoryes);
			}
			else if (dataH.peepCategory.Any())
			{
				categorys.AddRange(dataH.peepCategory);
			}
			else if (appointAction != -1)
			{
				if (MathfEx.IsRange(0, appointAction, 4, true) || appointAction == 7 || appointAction == 10)
				{
					categorys.Add(0);
				}
				else if (MathfEx.IsRange(5, appointAction, 6, true))
				{
					categorys.Add(4);
				}
				else if (appointAction == 8)
				{
					categorys.Add(1205);
				}
				else if (appointAction == 9)
				{
					categorys.Add(1202);
				}
			}
			else if (dataH.state != -1)
			{
				int[] array = new int[6] { 0, 4, 0, 9, 7, 8 };
				categorys.Add(array[dataH.state]);
			}
			else
			{
				GlobalMethod.DebugLog("カテゴリーが取得できなかった", 1);
				categorys.Add(1);
			}
		}
		bool isWaitPointPoisitionSet = false;
		if (flags.lstHeroine.Count != 0 && (bool)flags.lstHeroine[0].charaBase && flags.lstHeroine[0].charaBase.isArrival && map.no == flags.lstHeroine[0].charaBase.mapNo && appointAction == -1 && hScenePlayCount == 0)
		{
			WaitPoint wp = flags.lstHeroine[0].charaBase.wpData.wp;
			initPos = wp.transform.TransformPoint(wp.offsetHPos);
			initRot = wp.transform.rotation * Quaternion.Euler(wp.offsetHAngle);
			if ((bool)map.mapObjectGroup)
			{
				wp.kindList.ForEach(delegate(WaitPoint.KindMover kind)
				{
					Kind[] mapObjects = map.mapObjects;
					foreach (Kind kind2 in mapObjects)
					{
						if (kind.name == kind2.name)
						{
							kind2.transform.Translate(wp.offsetHPos);
							kind2.transform.rotation *= Quaternion.Euler(wp.offsetHAngle);
						}
					}
				});
			}
			isWaitPointPoisitionSet = true;
		}
		else if (dataH != null)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.SetPositionAndRotation(initPos, initRot);
			initPos = gameObject.transform.TransformPoint(dataH.offsetPos);
			initRot = gameObject.transform.rotation * Quaternion.Euler(dataH.offsetAngle);
			UnityEngine.Object.Destroy(gameObject);
		}
		lstInitCategory.AddRange(categorys);
		if (flags.isFreeH)
		{
			map.Change(freeHMapNo, Scene.Data.FadeType.None);
			yield return new WaitUntil(() => map.mapRoot != null);
			GameObject objStartPosition = map.mapRoot.transform.FindLoop("h_free");
			if ((bool)objStartPosition)
			{
				initPos = objStartPosition.transform.position;
				initRot = objStartPosition.transform.rotation;
			}
			if (freeHTimeZone != -1)
			{
				map.sunLightInfo.Set((SunLightInfo.Info.Type)freeHTimeZone, flags.ctrlCamera.thisCmaera);
				map.UpdateCameraFog();
			}
			if (freeHStage1 != -1)
			{
				SetState(flags.lstHeroine[0], freeHStage1);
			}
			if (flags.lstHeroine.Count > 1)
			{
				SetState(flags.lstHeroine[1], freeHStage2);
			}
			switch (freeHStatus)
			{
			case 0:
				flags.lstHeroine[0].SetMenstruationsDay(0);
				break;
			default:
				flags.lstHeroine[0].SetMenstruationsDay(4);
				break;
			case -1:
				break;
			}
		}
		else if ((bool)actscene)
		{
			actscene.Map.SetCycleToSunLight();
		}
		int startMapNo = -1;
		string startObjName = string.Empty;
		if (map.no == 0)
		{
			startObjName = "h_free";
		}
		if (appointAction == 0)
		{
			List<GameObject> list = GlobalMethod.LoadAllFolder<GameObject>("h/common/", "HPoint_0");
			if (list.Count != 0)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(list[list.Count - 1]);
				GameObject gameObject3 = gameObject2.transform.FindLoop("yuka00 (4)");
				if ((bool)gameObject3)
				{
					initPos = gameObject3.transform.position;
					initRot = gameObject3.transform.rotation;
					startObjName = string.Empty;
				}
				UnityEngine.Object.Destroy(gameObject2);
			}
		}
		else if (appointAction == 8)
		{
			GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
			hPointAppointNullDatail.mode = HFlag.EMode.sonyu;
			hPointAppointNullDatail.idMap = (map ? map.no : 0);
			hPointAppointNullDatail.kindGet = 0;
			hPointAppointNullDatail.nameGetNull = "kabe_00";
			GlobalMethod.HPointAppointNullDatail detail = hPointAppointNullDatail;
			GlobalMethod.HPointAppointNullGetData hPointAppointNull = GlobalMethod.GetHPointAppointNull(detail);
			initPos = hPointAppointNull.pos;
			initRot = hPointAppointNull.rot;
			startObjName = string.Empty;
			isWaitPointPoisitionSet = true;
		}
		else if (appointAction == 9)
		{
			GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
			hPointAppointNullDatail.mode = HFlag.EMode.houshi;
			hPointAppointNullDatail.idMap = (map ? map.no : 0);
			hPointAppointNullDatail.kindGet = 0;
			hPointAppointNullDatail.nameGetNull = "kabe_00";
			GlobalMethod.HPointAppointNullDatail detail2 = hPointAppointNullDatail;
			GlobalMethod.HPointAppointNullGetData hPointAppointNull2 = GlobalMethod.GetHPointAppointNull(detail2);
			initPos = hPointAppointNull2.pos;
			initRot = hPointAppointNull2.rot;
			startObjName = string.Empty;
			isWaitPointPoisitionSet = true;
		}
		if (categorys.Any((int c) => c >= 2000))
		{
			LoadSpecialMapStartPosition(out startMapNo, out startObjName);
		}
		else if (isLoadPeepMap)
		{
			startMapNo = ((map.no == 14 || map.no == 15 || map.no == 16) ? 51 : ((map.no != 45) ? 53 : 52));
			startObjName = "h_free";
		}
		else if (!flags.isFreeH)
		{
			LoadSpecialMapStartPosition(out startMapNo, out startObjName);
		}
		else
		{
			startMapNo = freeHMapNo;
			startObjName = "h_free";
		}
		if (flags.isFreeH)
		{
			if (categorys.Any((int c) => MathfEx.IsRange(1010, c, 1099, true) || MathfEx.IsRange(1100, c, 1199, true)))
			{
				bool isLesbian = categorys.Any((int c) => MathfEx.IsRange(1100, c, 1199, true));
				List<GameObject> objs = GlobalMethod.LoadAllFolder<GameObject>("h/common/", "HPoint_Add_" + map.no);
				if (objs.Count != 0)
				{
					GameObject objPointFree = UnityEngine.Object.Instantiate(objs[objs.Count - 1]);
					HPointData[] datas = objPointFree.GetComponentsInChildren<HPointData>(true);
					yield return null;
					HPointData[] array2 = datas;
					foreach (HPointData hPointData in array2)
					{
						if (hPointData.category.Any((int c) => (!isLesbian) ? (c == 1012) : (c == 1100)))
						{
							hPointData.MoveOffset();
							initPos = hPointData.transform.position;
							initRot = hPointData.transform.rotation;
							startObjName = string.Empty;
							break;
						}
					}
					UnityEngine.Object.Destroy(objPointFree);
				}
			}
			else if (categorys.Any((int c) => MathfEx.IsRange(3000, c, 3099, true)))
			{
				GlobalMethod.HPointAppointNullDatail hPointAppointNullDatail = new GlobalMethod.HPointAppointNullDatail();
				hPointAppointNullDatail.mode = HFlag.EMode.houshi3P;
				hPointAppointNullDatail.idMap = (map ? map.no : 0);
				hPointAppointNullDatail.kindGet = 0;
				hPointAppointNullDatail.nameGetNull = "3P_00";
				GlobalMethod.HPointAppointNullDatail detail3 = hPointAppointNullDatail;
				GlobalMethod.HPointAppointNullGetData hPointAppointNull3 = GlobalMethod.GetHPointAppointNull(detail3);
				initPos = hPointAppointNull3.pos;
				initRot = hPointAppointNull3.rot;
				startObjName = string.Empty;
			}
		}
		if (startMapNo != -1)
		{
			map.Change(startMapNo, Scene.Data.FadeType.None);
			yield return new WaitUntil(() => !map.isMapLoading);
			flags.ctrlCamera.VisibleFroceVanish(true);
			flags.ctrlCamera.CleraVanishCollider();
		}
		if (!startObjName.IsNullOrEmpty())
		{
			GameObject gameObject4 = map.mapRoot.transform.FindLoop(startObjName);
			if ((bool)gameObject4)
			{
				initPos = gameObject4.transform.position;
				initRot = gameObject4.transform.rotation;
			}
		}
		lightData.light = lightCamera;
		lightData.initColor = lightCamera.color;
		lightData.initRot = new Vector2(lightCamera.transform.localEulerAngles.x, lightCamera.transform.localEulerAngles.y);
		lightData.initIntensity = lightCamera.intensity;
		GetMapObject();
		AllDoorOpenClose(false);
		if (flags.lstHeroine.IsNullOrEmpty())
		{
			flags.lstHeroine = new List<SaveData.Heroine>(new SaveData.Heroine[1]
			{
				new SaveData.Heroine(false)
			});
		}
		flags.player = flags.player ?? new SaveData.Player(false);
		Singleton<Scene>.Instance.DrawImageAndProgress(0f);
		GameObject parent = Scene.GetRootGameObjects("H")[0];
		yield return StartCoroutine(CreateCharacter(parent, flags.lstHeroine, flags.player));
		yield return null;
		subjective.transform.SetParent(parent.transform, false);
		ChaControl female = lstFemale[0];
		ChaControl female2 = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		SaveData.Heroine heroine = flags.lstHeroine[0];
		SaveData.Heroine heroine2 = ((flags.lstHeroine.Count <= 1) ? null : flags.lstHeroine[1]);
		female.transform.SetPositionAndRotation(initPos, initRot);
		if ((bool)female2)
		{
			female2.transform.SetPositionAndRotation(initPos, initRot);
		}
		male.transform.SetPositionAndRotation(initPos, initRot);
		SetCharacterPositon(!isWaitPointPoisitionSet);
		if ((bool)objMoveAxis)
		{
			objMoveAxis.transform.SetPositionAndRotation(female.transform.position, female.transform.rotation);
		}
		for (int k = 0; k < lstFemale.Count; k++)
		{
			LoadCharaTagLayer(k);
		}
		flags.mode = HFlag.EMode.none;
		flags.experience = ((heroine.HExperience == SaveData.Heroine.HExperienceKind.慣れ || heroine.HExperience == SaveData.Heroine.HExperienceKind.淫乱) ? EExperience.nare : EExperience.funare);
		CreateListAnimationFileName();
		lstMotionIK.Add(new MotionIK(female));
		lstMotionIK.Add(new MotionIK(male));
		if ((bool)female2)
		{
			lstMotionIK.Add(new MotionIK(female2));
		}
		lstMotionIK.ForEach(delegate(MotionIK motionIK)
		{
			motionIK.SetPartners(lstMotionIK);
		});
		lookDan.SetMale(male);
		hitreaction.ik = female.animBody.GetComponent<FullBodyBipedIK>();
		hitreaction1.ik = ((!female2) ? null : female2.animBody.GetComponent<FullBodyBipedIK>());
		hand.Init(female, male.chaFile.custom.body.skinMainColor, 0);
		hand.hitReaction = hitreaction;
		hand1.Init(female2, male.chaFile.custom.body.skinMainColor, 1);
		hand1.hitReaction = hitreaction1;
		int progressIndex = 2;
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + progressIndex++) / (float)(lstFemale.Count + progressAdd));
		yield return null;
		sprite.Init(lstFemale, heroine, male, rely, lstUseAnimInfo, categorys, voice);
		sprite.SetLightData(lightData);
		sprite.localMoveInitAction = InitLocalPosition;
		sprite.CreatePeepingFemaleList(flags.lstHeroine);
		sprite.InitSpriteButton();
		lookDan.SetFemale(new ChaControl[2] { female, female2 });
		yure = new YureCtrl();
		yure.Init(female);
		yure1 = new YureCtrl();
		yure1.Init(female2);
		hand.yure = yure;
		hand1.yure = yure1;
		meta = new MetaballCtrl(objMetaball, male.objBodyBone, female);
		siru.Init(female);
		siru1.Init(female2);
		face.LoadText(heroine.ChaName);
		if (heroine2 != null)
		{
			face1.LoadText(heroine2.ChaName);
		}
		particle.Init("h/list/", female.objBodyBone, flags);
		particle.Load(male.objBodyBone, 0);
		if ((bool)female2)
		{
			particle1.Init("h/list/", female2.objBodyBone, flags);
			particle1.Load(male.objBodyBone, 0);
		}
		meta.SetParticle(particle);
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + progressIndex++) / (float)(lstFemale.Count + progressAdd));
		yield return null;
		voice.Init(heroine.ChaName, (heroine2 == null) ? string.Empty : heroine2.ChaName, "h/list/");
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + progressIndex++) / (float)(lstFemale.Count + progressAdd));
		yield return null;
		eyeneckFemale.Init(female, male, heroine);
		eyeneckFemale.SetPartner(male.objBodyBone);
		if ((bool)female2)
		{
			eyeneckFemale1.Init(female2, male, heroine2);
			eyeneckFemale1.SetPartner(male.objBodyBone);
			eyeneckFemale.SetFemalePartner(female2.objBodyBone);
			eyeneckFemale1.SetFemalePartner(female.objBodyBone);
		}
		eyeneckMale.Init(male, female);
		eyeneckMale.SetPartner(female.objBodyBone);
		dynamicCtrl.Init(female);
		dynamicCtrl1.Init(female2);
		alCtrl.Init(female.animBody);
		if ((bool)female2)
		{
			alCtrl1.Init(female2.animBody);
		}
		HFlag.DeliveryMember mem = new HFlag.DeliveryMember
		{
			chaFemale = female,
			chaFemale1 = female2,
			chaMale = male,
			ctrlFlag = flags,
			fade = crossfade,
			sprite = sprite,
			lstMotionIK = lstMotionIK,
			hand = hand,
			hand1 = hand1,
			yure = yure,
			yure1 = yure1,
			item = item,
			hitcolFemale = hitcollisionFemale,
			hitcolFemale1 = hitcollisionFemale1,
			hitcolMale = hitcollisionMale,
			meta = meta,
			parentObjectFemale = parentObjectFemale,
			parentObjectFemale1 = parentObjectFemale1,
			parentObjectMale = parentObjectMale,
			voice = voice,
			se = se,
			particle = particle,
			alCtrl = alCtrl,
			alCtrl1 = alCtrl1
		};
		lstProc.Add(new HAibu(mem));
		lstProc.Add(new HHoushi(mem));
		(lstProc[1] as HHoushi).SetRely(rely);
		lstProc.Add(new HSonyu(mem));
		lstProc.Add(new HMasturbation(mem));
		lstProc.Add(new HPeeping(mem));
		(lstProc[4] as HPeeping).SetMapObject(map.mapRoot);
		lstProc.Add(new HLesbian(mem));
		lstProc.Add(new H3PHoushi(mem));
		(lstProc[6] as H3PHoushi).SetRely(rely);
		lstProc.Add(new H3PSonyu(mem));
		parentObjectBaseFemale.Init("h/list/", "parent_object_base_female", female.objBodyBone, female, flags.hashAssetBundle, true);
		if ((bool)female2)
		{
			parentObjectBaseFemale1.Init("h/list/", "parent_object_base_female", female2.objBodyBone, female2, flags.hashAssetBundle, true);
		}
		parentObjectBaseMale.Init("h/list/", "parent_object_base_male", male.objBodyBone, male, flags.hashAssetBundle, true);
		parentObjectFemale.Init("h/list/", "parent_object_female", female.objBodyBone, female, flags.hashAssetBundle, false);
		if ((bool)female2)
		{
			parentObjectFemale1.Init("h/list/", "parent_object_female", female2.objBodyBone, female2, flags.hashAssetBundle, false);
		}
		parentObjectMale.Init("h/list/", "parent_object_male", male.objBodyBone, male, flags.hashAssetBundle, false);
		hand.InitHitReactionCollider();
		hand1.InitHitReactionCollider();
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + progressIndex++) / (float)(lstFemale.Count + progressAdd));
		yield return null;
		HSceneProc hSceneProc = this;
		HSceneProc hSceneProc2 = this;
		int num = appointAction;
		hSceneProc.ChangeAnimator(hSceneProc2.LoadAnimationListInfo(-1, num), true);
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + progressIndex++) / (float)(lstFemale.Count + progressAdd));
		yield return null;
		sprite.InitCategoryActionToggle();
		sprite.InitPointMenuAndHelp(categorys);
		subjective.SetSubjectiveObject(parentObjectBaseMale.GetObject("SubjectiveBase"));
		subjective.SetFemale(female);
		subjective.SetMale(male);
		flags.transVoiceMouth[0] = ((!female.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth)) ? null : female.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth).transform);
		if ((bool)female2)
		{
			flags.transVoiceMouth[1] = ((!female2.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth)) ? null : female2.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth).transform);
		}
		voice.voiceTrans[0] = flags.transVoiceMouth[0];
		voice.voiceTrans[1] = flags.transVoiceMouth[1];
		LoadMapDependent(map.no);
		if (map.Info.State == 2)
		{
			flags.lstHeroine.ForEach(delegate(SaveData.Heroine h)
			{
				h.chaCtrl.fileStatus.shoesType = 1;
			});
			male.fileStatus.shoesType = 1;
		}
		female.SetAccessoryStateCategory(1, false);
		if ((bool)female2)
		{
			female2.SetAccessoryStateCategory(1, false);
		}
		if (map.no == 52)
		{
			female.SetAccessoryStateAll(false);
			if ((bool)female2)
			{
				female2.SetAccessoryStateAll(false);
			}
		}
		female.hideMoz = false;
		if ((bool)female2)
		{
			female2.hideMoz = false;
		}
		flags.isInsertOK[0] = (dataH != null && dataH.isKokanForceInsert) || flags.IsNamaInsertOK();
		flags.isInsertOK[1] = flags.lstHeroine.Count > 1 && HFlag.NamaInsertCheck(flags.lstHeroine[1]);
		int analRate = (int)heroine.hAreaExps[3];
		GlobalMethod.RatioRand rand = new GlobalMethod.RatioRand();
		rand.Add(0, analRate);
		if (100 - analRate != 0)
		{
			rand.Add(1, 100 - analRate);
		}
		flags.isAnalInsertOK = rand.Random() == 0 || heroine.denial.anal;
		flags.voice.MemberInit();
		flags.isCondom = false;
		if (appointAction == 4 && !flags.isInsertOK[0])
		{
			sprite.CondomClick();
		}
		bool isPeeping = categorys.Any((int c) => MathfEx.IsRange(2000, c, 2999, true));
		Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(isPeeping ? BGM.HScenePeep : ((heroine.HExperience != SaveData.Heroine.HExperienceKind.淫乱) ? BGM.HSceneGentle : BGM.HScene)));
		if (MathfEx.IsRange(0, appointAction, 6, true))
		{
			flags.voice.playVoices[0] = -1;
		}
		else if (flags.mode == HFlag.EMode.masturbation)
		{
			flags.voice.playVoices[0] = 400;
		}
		else if (categorys.Any((int c) => c >= 1000 && c < 2000))
		{
			if (flags.mode == HFlag.EMode.aibu)
			{
				flags.voice.playVoices[0] = 144;
			}
			else if (flags.mode == HFlag.EMode.houshi)
			{
				flags.voice.playVoices[0] = 210;
			}
			else if (flags.mode == HFlag.EMode.sonyu)
			{
				flags.voice.playVoices[0] = 337 + ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0);
			}
		}
		else if (categorys.Any((int c) => c < 2000))
		{
			flags.voice.playVoices[0] = 0;
		}
		HashSet<int> saveID;
		Singleton<Game>.Instance.saveData.clubContents.TryGetValue(2, out saveID);
		if (saveID != null && saveID.Contains(1))
		{
			flags.potion = flags.addPtion;
		}
		if (heroine.hCount == 0 && !isPeeping && !flags.isFreeH && !female2)
		{
			sprite.SetHelpSprite(flags.firstHEasy ? 2 : 0);
		}
		flags.ctrlCamera.LoadVanish("h/list/", "map_col_" + map.no.ToString("00"), map.mapRoot);
		Singleton<Manager.Sound>.Instance.Listener = flags.ctrlCamera.transform;
		if (isEasyPlace)
		{
			Singleton<Game>.Instance.rankSaveData.easyPlaceHCount++;
		}
		if (flags.mode == HFlag.EMode.masturbation && !flags.isFreeH)
		{
			Singleton<Game>.Instance.rankSaveData.peepOnanismCount++;
		}
		MapSameObjectDisable();
		SetShortcutKey();
		sprite.InitTrespassingHelp();
		Singleton<Scene>.Instance.DrawImageAndProgress(1f);
		yield return null;
		Singleton<Scene>.Instance.DrawImageAndProgress();
		female.ResetDynamicBoneAll();
		if ((bool)female2)
		{
			female2.ResetDynamicBoneAll();
		}
		male.ResetDynamicBoneAll();
		SetConfig(true);
		for (int l = 0; l < voicePlayShuffle.Length; l++)
		{
			voicePlayShuffle[l] = new ShuffleRand();
			voicePlayShuffle[l].Init(2);
		}
		yield return StartCoroutine(Singleton<Scene>.Instance.Fade(SimpleFade.Fade.Out));
		base.enabled = true;
	}

	private void Update()
	{
		SetConfig();
		ChaControl female = lstFemale[0];
		ChaControl female2 = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		face.SafeProc(delegate(FaceListCtrl f)
		{
			f.OpenCtrl(female);
		});
		face1.SafeProc(delegate(FaceListCtrl f)
		{
			f.OpenCtrl(female2);
		});
		float num = flags.gaugeFemale * 0.01f;
		if (flags.rateNip < num)
		{
			flags.rateNip = num;
		}
		float rate = Mathf.Lerp(0f, Manager.Config.EtcData.nipMax, flags.rateNip);
		female.ChangeNipRate(rate);
		if ((bool)female2)
		{
			female2.ChangeNipRate(rate);
		}
		if (flags.isHSceneEnd)
		{
			switch (flags.numEnd)
			{
			case 0:
				EndProc();
				flags.numEnd = 1;
				break;
			case 1:
				break;
			case 2:
				NewHeroineEndProc();
				flags.numEnd = 3;
				break;
			case 3:
				break;
			}
			return;
		}
		AnimatorStateInfo animatorStateInfo = female.getAnimatorStateInfo(0);
		eyeneckFemale.Proc(animatorStateInfo, flags.voice.eyenecks[0], flags.ctrlCamera.thisCmaera.gameObject, Manager.Config.EtcData.FemaleEyesCamera, Manager.Config.EtcData.FemaleNeckCamera);
		if ((bool)female2)
		{
			eyeneckFemale1.Proc(animatorStateInfo, flags.voice.eyenecks[1], flags.ctrlCamera.thisCmaera.gameObject, Manager.Config.EtcData.FemaleEyesCamera1, Manager.Config.EtcData.FemaleNeckCamera1);
		}
		eyeneckMale.Proc(animatorStateInfo, flags.ctrlCamera.thisCmaera.gameObject);
		if (!(Singleton<Scene>.Instance.NowSceneNames[0] != "HProc"))
		{
			HSprite.FadeKindProc fadeKindProc = sprite.GetFadeKindProc();
			if (fadeKindProc != HSprite.FadeKindProc.OutEnd)
			{
				voice.Proc(animatorStateInfo, lstFemale);
			}
			lstProc.SafeProc((int)flags.mode, delegate(HActionBase proc)
			{
				proc.Proc();
			});
			if (meta != null)
			{
				meta.Proc(animatorStateInfo, flags, flags.isInsideFinish);
			}
			siru.Proc(animatorStateInfo);
			siru1.Proc(animatorStateInfo);
			se.Proc(animatorStateInfo, lstFemale.ToArray());
			if (flags.selectAnimationListInfo != null && !flags.voiceWait)
			{
				ChangeAnimator(flags.selectAnimationListInfo);
				flags.selectAnimationListInfo = null;
			}
			ShortCut();
			if (flags.click == HFlag.ClickKind.pointmove)
			{
				GotoPointMoveScene();
			}
			if ((bool)clothCustomCtrl && clothCustomCtrl.gameObject.activeSelf)
			{
				GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, false);
			}
			flags.click = HFlag.ClickKind.none;
		}
	}

	private void LateUpdate()
	{
		if (Singleton<Scene>.Instance.NowSceneNames[0] != "HProc")
		{
			if ((bool)dynamicCtrl)
			{
				dynamicCtrl.Proc();
			}
			if ((bool)dynamicCtrl1)
			{
				dynamicCtrl1.Proc();
			}
			return;
		}
		lstProc.SafeProc((int)flags.mode, delegate(HActionBase proc)
		{
			proc.LateProc();
		});
		if (item != null)
		{
			item.LateUpdate();
		}
		dynamicCtrl.Proc();
		dynamicCtrl1.Proc();
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].SetPosition(lstFemale[i].gameObject.transform.InverseTransformVector(guideObject.amount.position));
			lstFemale[i].SetRotation(guideObject.amount.rotation);
		}
		if (lstFemale.Count != 0 && (bool)lstFemale[0])
		{
			item.SetTransform(lstFemale[0].objTop.transform);
		}
		if ((bool)male)
		{
			male.SetPosition(male.gameObject.transform.InverseTransformVector(guideObject.amount.position));
			male.SetRotation(guideObject.amount.rotation);
		}
		LateShortCut();
	}

	private void OnDestroy()
	{
		if ((bool)map && (bool)map.mapRoot)
		{
			map.mapRoot.SetActive(true);
		}
		if (Singleton<Voice>.IsInstance())
		{
			Singleton<Voice>.Instance.StopAll();
		}
		if (Singleton<Manager.Sound>.IsInstance())
		{
			Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.GameSE3D);
		}
	}

	private IEnumerator CreateCharacter(GameObject parent, List<SaveData.Heroine> heroineList, SaveData.Player player)
	{
		int count = heroineList.Count + progressAdd;
		int person = 0;
		foreach (SaveData.Heroine heroine in heroineList)
		{
			ChaFileControl chaFile = new ChaFileControl();
			ChaFile.CopyChaFile(chaFile, (!heroine.chaCtrl) ? heroine.charFile : heroine.chaCtrl.chaFile);
			ChaControl chara = Singleton<Character>.Instance.CreateFemale(parent, 0, chaFile);
			heroine.SetRoot(chara.gameObject);
			heroine.lewdness = Mathf.Clamp(heroine.lewdness, 0, 100);
			chara.ChangeCoordinateType((ChaFileDefine.CoordinateType)lstCoordinateType[person]);
			Singleton<Character>.Instance.loading.Load(0, chara);
			yield return new WaitUntil(() => Singleton<Character>.Instance.loading.IsEnd(chara));
			if (clothStates.Count > person)
			{
				for (int i = 0; i < clothStates[person].Count; i++)
				{
					chara.SetClothesState(i, (byte)clothStates[person][i]);
				}
			}
			if (accessoryStates.Count > person)
			{
				for (int j = 0; j < accessoryStates[person].Count; j++)
				{
					chara.SetAccessoryState(j, accessoryStates[person][j]);
				}
			}
			lstFemale.Add(chara);
			Singleton<Scene>.Instance.DrawImageAndProgress((float)lstFemale.Count / (float)count);
			person++;
		}
		ChaFileControl chaFileControl = new ChaFileControl();
		ChaFile.CopyChaFile(chaFileControl, (!player.chaCtrl) ? player.charFile : player.chaCtrl.chaFile);
		male = Singleton<Character>.Instance.CreateMale(parent, 0, chaFileControl);
		player.SetRoot(male.gameObject);
		male.ChangeCoordinateType((ChaFileDefine.CoordinateType)maleCoordinateType);
		Singleton<Character>.Instance.loading.Load(0, male);
		yield return new WaitUntil(() => Singleton<Character>.Instance.loading.IsEnd(male));
		male.visibleAll = false;
		Singleton<Scene>.Instance.DrawImageAndProgress((float)(lstFemale.Count + 1) / (float)count);
	}

	public IEnumerator CreateFemaleCharacterAppoint(SaveData.Heroine _heroine, int _coordinateType)
	{
		GameObject parent = Scene.GetRootGameObjects("H")[0];
		ChaFileControl chaFile = new ChaFileControl();
		ChaFile.CopyChaFile(chaFile, (!_heroine.chaCtrl) ? _heroine.charFile : _heroine.chaCtrl.chaFile);
		ChaControl chara = Singleton<Character>.Instance.CreateFemale(parent, 0, chaFile);
		_heroine.SetRoot(chara.gameObject);
		_heroine.lewdness = Mathf.Clamp(_heroine.lewdness, 0, 100);
		chara.ChangeCoordinateType((ChaFileDefine.CoordinateType)_coordinateType);
		Singleton<Character>.Instance.loading.Load(0, chara);
		yield return new WaitUntil(() => Singleton<Character>.Instance.loading.IsEnd(chara));
		lstFemale.Add(chara);
	}

	private void ChangeAnimator(AnimationListInfo _nextAinmInfo, bool _isForceCameraReset = false)
	{
		HFlag.EMode mode = flags.nowAnimationInfo.mode;
		if (_nextAinmInfo == null)
		{
			return;
		}
		flags.mode = _nextAinmInfo.mode;
		ChaControl chaControl = lstFemale[0];
		ChaControl chaControl2 = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		int num = 0;
		if (_nextAinmInfo.mode == HFlag.EMode.houshi3P || _nextAinmInfo.mode == HFlag.EMode.sonyu3P)
		{
			lstMotionIK.ForEach(delegate(MotionIK motionIK)
			{
				motionIK.Release();
			});
			lstMotionIK.Clear();
			if (_nextAinmInfo.id % 2 == 0)
			{
				lstMotionIK.Add(new MotionIK(chaControl));
				lstMotionIK.Add(new MotionIK(male));
				if ((bool)chaControl2)
				{
					lstMotionIK.Add(new MotionIK(chaControl2));
				}
			}
			else
			{
				if ((bool)chaControl2)
				{
					lstMotionIK.Add(new MotionIK(chaControl2));
				}
				lstMotionIK.Add(new MotionIK(male));
				lstMotionIK.Add(new MotionIK(chaControl));
				num = 1;
			}
			lstMotionIK.ForEach(delegate(MotionIK motionIK)
			{
				motionIK.SetPartners(lstMotionIK);
				motionIK.Reset();
			});
		}
		FemaleParameter paramFemale = _nextAinmInfo.paramFemale;
		FemaleParameter paramFemale2 = flags.nowAnimationInfo.paramFemale;
		if (paramFemale2.lstIdLayer.Count != 0 && paramFemale2.lstIdLayer[0] != -1)
		{
			chaControl.setLayerWeight(0f, paramFemale2.lstIdLayer[0]);
		}
		if (flags.mode != mode || flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P || (!_nextAinmInfo.pathFemaleBase.assetpath.IsNullOrEmpty() && paramFemale.path.assetpath.IsNullOrEmpty()))
		{
			chaControl.LoadAnimation(_nextAinmInfo.pathFemaleBase.assetpath, _nextAinmInfo.pathFemaleBase.file, string.Empty);
		}
		if (paramFemale.lstIdLayer[0] != -1)
		{
			chaControl.setLayerWeight(1f, paramFemale.lstIdLayer[0]);
		}
		PathName path = paramFemale.path;
		if (!path.assetpath.IsNullOrEmpty())
		{
			Animator chaAnimator = chaControl.animBody;
			CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
			{
				chaAnimator.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator.runtimeAnimatorController, rac);
			});
			AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
		}
		else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
		{
			GlobalMethod.DebugLog(string.Concat("女の", flags.mode, "の", _nextAinmInfo.id.ToString(), "番に", flags.experience, "なんてないよ"), 2);
		}
		if (paramFemale.isHitObject)
		{
			chaControl.LoadHitObject();
			hitcollisionFemale.Init(chaControl, chaControl.objHitHead);
			hitcollisionFemale.LoadText("h/list/", paramFemale.fileHit);
		}
		else
		{
			chaControl.ReleaseHitObject();
			hitcollisionFemale.Release();
		}
		lstMotionIK[(num != 0) ? 2 : 0].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
		alCtrl.Load("h/list/", path.file);
		FemaleParameter paramFemale3 = _nextAinmInfo.paramFemale1;
		if ((bool)chaControl2)
		{
			paramFemale2 = flags.nowAnimationInfo.paramFemale1;
			if (paramFemale2.lstIdLayer.Count != 0 && paramFemale2.lstIdLayer[0] != -1)
			{
				chaControl2.setLayerWeight(0f, paramFemale2.lstIdLayer[0]);
			}
			if (flags.mode != mode || flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P || (!_nextAinmInfo.pathFemaleBase.assetpath.IsNullOrEmpty() && paramFemale3.path.assetpath.IsNullOrEmpty()))
			{
				chaControl2.LoadAnimation(_nextAinmInfo.pathFemaleBase1.assetpath, _nextAinmInfo.pathFemaleBase1.file, string.Empty);
			}
			if (paramFemale3.lstIdLayer[0] != -1)
			{
				chaControl2.setLayerWeight(1f, paramFemale3.lstIdLayer[0]);
			}
			path = paramFemale3.path;
			if (!path.assetpath.IsNullOrEmpty())
			{
				Animator chaAnimator2 = chaControl2.animBody;
				CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
				{
					chaAnimator2.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator2.runtimeAnimatorController, rac);
				});
				AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
			}
			else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
			{
				GlobalMethod.DebugLog(string.Concat("女の", flags.mode, "の", _nextAinmInfo.id.ToString(), "番に", flags.experience, "なんてないよ"), 2);
			}
			if (paramFemale3.isHitObject)
			{
				chaControl2.LoadHitObject();
				hitcollisionFemale1.Init(chaControl2, chaControl2.objHitHead);
				hitcollisionFemale1.LoadText("h/list/", paramFemale3.fileHit);
			}
			else
			{
				chaControl2.ReleaseHitObject();
				hitcollisionFemale1.Release();
			}
			lstMotionIK[(num == 0) ? 2 : 0].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
			alCtrl1.Load("h/list/", path.file);
		}
		MaleParameter paramMale = _nextAinmInfo.paramMale;
		path = paramMale.path;
		if (path.assetpath != string.Empty)
		{
			MaleParameter paramMale2 = flags.nowAnimationInfo.paramMale;
			if (paramMale2.lstIdLayer.Any() && paramMale2.lstIdLayer[0] != -1)
			{
				male.setLayerWeight(0f, paramMale2.lstIdLayer[0]);
			}
			male.chaFile.status.visibleBodyAlways = true;
			male.visibleAll = true;
			if (flags.mode != mode || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P)
			{
				male.LoadAnimation(_nextAinmInfo.pathMaleBase.assetpath, _nextAinmInfo.pathMaleBase.file, string.Empty);
			}
			if (paramMale.lstIdLayer[0] != -1)
			{
				male.setLayerWeight(1f, paramMale.lstIdLayer[0]);
			}
			if (!path.assetpath.IsNullOrEmpty())
			{
				Animator chaAnimator3 = male.animBody;
				CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
				{
					chaAnimator3.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator3.runtimeAnimatorController, rac);
				});
				AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
			}
			else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
			{
				GlobalMethod.DebugLog(string.Concat("男の", flags.mode, "の", _nextAinmInfo.id.ToString(), "番に", flags.experience, "なんてないよ"), 2);
			}
			lstMotionIK[1].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
			if (paramMale.isHitObject)
			{
				male.LoadHitObject();
				hitcollisionMale.Init(male, male.objHitHead);
				hitcollisionMale.LoadText("h/list/", paramMale.fileHit);
			}
		}
		else if ((bool)male.objBodyBone)
		{
			male.visibleAll = false;
			male.chaFile.status.visibleBodyAlways = true;
			male.chaFile.status.visibleHeadAlways = true;
			male.ReleaseHitObject();
			hitcollisionMale.Release();
		}
		lstMotionIK.Where((MotionIK motionIK) => motionIK.ik != null).ToList().ForEach(delegate(MotionIK motionIK)
		{
			motionIK.Calc("Idle");
		});
		lookDan.LoadList("h/list/", paramMale.fileLookAt);
		yure.LoadAllExcel("h/list/", paramFemale.fileYure);
		yure1.LoadAllExcel("h/list/", paramFemale3.fileYure);
		hand.LoadMotionEnableState(paramFemale.fileAibuEnableMotion);
		hand.LoadReactionList(paramFemale.fileReaction);
		hand1.LoadMotionEnableState(paramFemale3.fileAibuEnableMotion);
		hand1.LoadReactionList(paramFemale3.fileReaction);
		meta.Clear();
		meta.Load("h/list/", paramMale.fileMetaballCtrl, male.objBodyBone, null, chaControl2, null);
		item.LoadItem(paramFemale.fileItem, male.objBodyBone, lstFemale.ToArray(), null, flags.hashAssetBundle);
		siru.Load("h/list/", paramFemale.fileSiruPaste);
		siru1.Load("h/list/", paramFemale3.fileSiruPaste);
		parentObjectFemale.LoadText("h/list/", paramFemale.fileHitObject);
		parentObjectFemale1.LoadText("h/list/", paramFemale3.fileHitObject);
		parentObjectMale.LoadText("h/list/", paramMale.fileHitObject);
		eyeneckFemale.Load("h/list/", paramFemale.fileMotionNeck);
		eyeneckFemale1.Load("h/list/", paramFemale3.fileMotionNeck);
		eyeneckMale.Load("h/list/", paramMale.fileMotionNeck);
		se.Load("h/list/", paramFemale.fileSe, male.objBodyBone, chaControl.objBodyBone, map.mapRoot);
		dynamicCtrl.Load("h/list/", paramFemale.fileDynamicBoneRef);
		dynamicCtrl1.Load("h/list/", paramFemale3.fileDynamicBoneRef);
		male.chaFile.status.visibleSon = _nextAinmInfo.numMaleSon == 1;
		List<int> useItemNumber = hand.GetUseItemNumber();
		foreach (int item in useItemNumber)
		{
			hand.DetachItemByUseItem(item);
		}
		hand.ForceFinish(false);
		int num2 = ((flags.click == HFlag.ClickKind.insert) ? 1 : ((flags.click == HFlag.ClickKind.insert_voice) ? 2 : 0));
		if (flags.mode == HFlag.EMode.houshi || flags.mode == HFlag.EMode.houshi3P)
		{
			lstProc[(int)flags.mode].MotionChange(num2);
		}
		else if (flags.mode == HFlag.EMode.peeping)
		{
			lstProc[(int)flags.mode].MotionChange(_nextAinmInfo.numCtrl);
		}
		else if (flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.masturbation)
		{
			lstProc[(int)flags.mode].MotionChange(1);
		}
		else
		{
			lstProc[(int)flags.mode].MotionChange(0);
		}
		SetLocalPosition(_nextAinmInfo);
		string strfile = paramFemale.nameCamera;
		if (flags.mode == HFlag.EMode.houshi && num2 == 0)
		{
			strfile = _nextAinmInfo.nameCameraIdle;
		}
		if (_isForceCameraReset || Manager.Config.EtcData.HInitCamera)
		{
			GlobalMethod.LoadCamera(flags.ctrlCamera, "h/list/", strfile);
		}
		else
		{
			GlobalMethod.LoadResetCamera(flags.ctrlCamera, "h/list/", strfile);
		}
		flags.SpeedRelationClear();
		SetMapObject(_nextAinmInfo.useChair, _nextAinmInfo.useDesk, _nextAinmInfo.pathMapObjectNull.assetpath, _nextAinmInfo.pathMapObjectNull.file);
		if (_nextAinmInfo.mode != HFlag.EMode.masturbation && _nextAinmInfo.mode != HFlag.EMode.lesbian)
		{
			if (flags.nowAnimationInfo.mode == _nextAinmInfo.mode)
			{
				flags.voice.playVoices[0] = -1;
				if (_nextAinmInfo.mode == HFlag.EMode.aibu)
				{
					flags.voice.playVoices[0] = 9;
				}
				else if (_nextAinmInfo.mode == HFlag.EMode.sonyu)
				{
					flags.voice.playVoices[0] = 10;
				}
				else if (_nextAinmInfo.mode == HFlag.EMode.sonyu3P)
				{
					flags.voice.playVoices[voicePlayShuffle[0].Get()] = 21;
				}
			}
			else
			{
				flags.voice.playVoices[0] = ((_nextAinmInfo.mode == HFlag.EMode.aibu) ? 100 : ((_nextAinmInfo.mode != HFlag.EMode.houshi) ? (300 + ((Game.isAdd20 && _nextAinmInfo.isFemaleInitiative) ? 38 : 0)) : 200));
			}
		}
		bool flag = (_nextAinmInfo.mode == HFlag.EMode.houshi || _nextAinmInfo.mode == HFlag.EMode.houshi3P) && _nextAinmInfo.mode == flags.nowAnimationInfo.mode;
		flags.nowAnimationInfo = _nextAinmInfo;
		if (!flag)
		{
			sprite.MainSpriteChange(_nextAinmInfo);
		}
		sprite.InitHeroine(flags.lstHeroine);
		SetClothStateStartMotion(0);
		SetClothStateStartMotion(1);
		dynamicCtrl.Proc();
		dynamicCtrl1.Proc();
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL).InitLocalPosition();
			lstFemale[i].getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR).InitLocalPosition();
		}
		GlobalMethod.HGlobalSaveData((int)_nextAinmInfo.mode, _nextAinmInfo.id);
		Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.GameSE3D);
		Resources.UnloadUnusedAssets();
	}

	private void CreateListAnimationFileName(bool _isAnimListCreate = true, int _list = -1)
	{
		if (_isAnimListCreate)
		{
			CreateAllAnimationList();
		}
		SaveData saveData = Singleton<Game>.Instance.saveData;
		Dictionary<int, HashSet<int>> clubContents = saveData.clubContents;
		HashSet<int> value;
		clubContents.TryGetValue(1, out value);
		if (value == null)
		{
			value = new HashSet<int>();
		}
		Dictionary<int, HashSet<int>> playHList = Singleton<Game>.Instance.glSaveData.playHList;
		bool flag = categorys.Any((int c) => MathfEx.IsRange(1010, c, 1099, true) || MathfEx.IsRange(1100, c, 1199, true) || MathfEx.IsRange(3000, c, 3099, true));
		for (int i = 0; i < lstAnimInfo.Length; i++)
		{
			lstUseAnimInfo[i] = new List<AnimationListInfo>();
			if (_list != -1 && i != _list)
			{
				continue;
			}
			HashSet<int> value2;
			if (!playHList.TryGetValue(i, out value2))
			{
				value2 = new HashSet<int>();
			}
			if (flags.isFreeH && value2.Count == 0 && !flag)
			{
				continue;
			}
			for (int j = 0; j < lstAnimInfo[i].Count; j++)
			{
				if (!lstAnimInfo[i][j].lstCategory.Any((Category c) => categorys.Contains(c.category)))
				{
					continue;
				}
				if (!flags.isFreeH)
				{
					if ((lstAnimInfo[i][j].isRelease && !value.Contains(i * 1000 + lstAnimInfo[i][j].id)) || (lstAnimInfo[i][j].isExperience != 2 && lstAnimInfo[i][j].isExperience > (int)flags.experience))
					{
						continue;
					}
				}
				else if (lstAnimInfo[i][j].stateRestriction > (int)flags.lstHeroine[0].HExperience || (!value2.Contains(lstAnimInfo[i][j].id) && !flag))
				{
					continue;
				}
				lstUseAnimInfo[i].Add(lstAnimInfo[i][j]);
			}
		}
	}

	private void CreateAllAnimationList()
	{
		for (int i = 0; i < lstAnimInfo.Length; i++)
		{
			string text2 = GlobalMethod.LoadAllListText("h/list/", "AnimationInfo_" + i.ToString("00"));
			lstAnimInfo[i] = new List<AnimationListInfo>();
			if (text2.IsNullOrEmpty())
			{
				continue;
			}
			string[,] data;
			GlobalMethod.GetListString(text2, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				int num = 0;
				int id = 0;
				int.TryParse(data[j, 1], out id);
				AnimationListInfo info = lstAnimInfo[i].Find((AnimationListInfo l) => l.id == id);
				if (info == null)
				{
					lstAnimInfo[i].Add(new AnimationListInfo());
					info = lstAnimInfo[i][lstAnimInfo[i].Count - 1];
				}
				info.nameAnimation = data[j, num++];
				uiTranslater.Get(i).SafeGetText(id).SafeProc(delegate(string text)
				{
					info.nameAnimation = text;
				});
				num++;
				info.id = id;
				info.mode = (HFlag.EMode)i;
				info.lstCategory.Clear();
				string[] array = data[j, num++].Split('/');
				string[] array2 = array;
				foreach (string text3 in array2)
				{
					if (!text3.IsNullOrEmpty())
					{
						info.lstCategory.Add(new Category
						{
							category = int.Parse(text3)
						});
					}
				}
				int.TryParse(data[j, num++], out info.posture);
				int.TryParse(data[j, num++], out info.kindHoushi);
				int.TryParse(data[j, num++], out info.isExperience);
				string text4 = data[j, num++];
				if (!text4.IsNullOrEmpty())
				{
					array = text4.Split('/');
					if (array.Length == info.lstCategory.Count)
					{
						for (int m = 0; m < array.Length; m++)
						{
							if (!array[m].IsNullOrEmpty())
							{
								info.lstCategory[m].fileMove = array[m];
							}
						}
					}
					else
					{
						GlobalMethod.DebugLog("カテゴリーの数とローカル移動リストの数が合わない", 1);
					}
				}
				info.pathMapObjectNull.assetpath = data[j, num++];
				info.pathMapObjectNull.file = data[j, num++];
				info.useDesk = int.Parse(data[j, num++]);
				info.useChair = int.Parse(data[j, num++]);
				info.pathMaleBase.assetpath = data[j, num++];
				info.pathMaleBase.file = data[j, num++];
				MaleParameter paramMale = info.paramMale;
				paramMale.lstIdLayer.Clear();
				text4 = data[j, num++];
				if (!text4.IsNullOrEmpty())
				{
					string[] array3 = text4.Split('/');
					foreach (string text5 in array3)
					{
						if (!text5.IsNullOrEmpty())
						{
							paramMale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text5, -1));
						}
					}
				}
				else
				{
					paramMale.lstIdLayer.Add(-1);
				}
				paramMale.path.assetpath = data[j, num++];
				paramMale.path.file = data[j, num++];
				paramMale.isHitObject = data[j, num++] == "1";
				paramMale.fileHit = data[j, num++];
				paramMale.fileHitObject = data[j, num++];
				paramMale.fileLookAt = data[j, num++];
				paramMale.fileMotionNeck = data[j, num++];
				paramMale.fileMetaballCtrl = data[j, num++];
				info.pathFemaleBase.assetpath = data[j, num++];
				info.pathFemaleBase.file = data[j, num++];
				FemaleParameter paramFemale = info.paramFemale;
				paramFemale.lstIdLayer.Clear();
				text4 = data[j, num++];
				if (!text4.IsNullOrEmpty())
				{
					string[] array4 = text4.Split('/');
					foreach (string text6 in array4)
					{
						if (!text6.IsNullOrEmpty())
						{
							paramFemale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text6, -1));
						}
					}
				}
				else
				{
					paramFemale.lstIdLayer.Add(-1);
				}
				paramFemale.lstFrontAndBehind.Clear();
				string[] array5 = data[j, num++].Split('/');
				foreach (string text7 in array5)
				{
					if (!text7.IsNullOrEmpty())
					{
						paramFemale.lstFrontAndBehind.Add(GlobalMethod.GetIntTryParse(text7));
					}
				}
				paramFemale.path.assetpath = data[j, num++];
				paramFemale.path.file = data[j, num++];
				paramFemale.isHitObject = data[j, num++] == "1";
				paramFemale.fileHit = data[j, num++];
				paramFemale.fileHitObject = data[j, num++];
				paramFemale.isYure = data[j, num++] == "1";
				paramFemale.fileYure = data[j, num++];
				paramFemale.fileDynamicBoneRef = data[j, num++];
				paramFemale.fileMotionNeck = data[j, num++];
				paramFemale.fileSe = data[j, num++];
				paramFemale.fileSiruPaste = data[j, num++];
				paramFemale.fileAibuEnableMotion = data[j, num++];
				paramFemale.fileReaction = data[j, num++];
				paramFemale.fileItem = data[j, num++];
				paramFemale.isAnal = data[j, num++] == "1";
				paramFemale.nameCamera = data[j, num++];
				int.TryParse(data[j, num++], out info.numCtrl);
				int.TryParse(data[j, num++], out info.sysTaii);
				info.nameCameraIdle = data[j, num++];
				info.nameCameraKiss = data[j, num++];
				info.lstAibuSpecialItem.Clear();
				array = data[j, num++].Split(',');
				string[] array6 = array;
				foreach (string text8 in array6)
				{
					if (!text8.IsNullOrEmpty())
					{
						info.lstAibuSpecialItem.Add(int.Parse(text8));
					}
				}
				info.isFemaleInitiative = data[j, num++] == "1";
				array = data[j, num++].Split('/');
				if (array.Length == 2)
				{
					int.TryParse(array[0], out info.houshiLoopActionW);
					int.TryParse(array[1], out info.houshiLoopActionS);
				}
				info.isSplash = data[j, num++] == "1";
				array = data[j, num++].Split(',');
				if (array.Length == 2)
				{
					int.TryParse(array[0], out info.numMainVoiceID);
					int.TryParse(array[1], out info.numSubVoiceID);
				}
				else
				{
					info.numMainVoiceID = -1;
					info.numSubVoiceID = -1;
				}
				info.numVoiceKindID = GlobalMethod.GetIntTryParse(data[j, num++]);
				info.numMaleSon = GlobalMethod.GetIntTryParse(data[j, num++]);
				info.numFemaleUpperCloth = GlobalMethod.GetIntTryParse(data[j, num++]);
				info.numFemaleLowerCloth = GlobalMethod.GetIntTryParse(data[j, num++]);
				info.isRelease = data[j, num++] == "1";
				info.stateRestriction = GlobalMethod.GetIntTryParse(data[j, num++]);
				if (num >= length2)
				{
					continue;
				}
				num++;
				num++;
				info.pathFemaleBase1.assetpath = data[j, num++];
				info.pathFemaleBase1.file = data[j, num++];
				paramFemale = info.paramFemale1;
				paramFemale.lstIdLayer.Clear();
				text4 = data[j, num++];
				if (!text4.IsNullOrEmpty())
				{
					string[] array7 = text4.Split('/');
					foreach (string text9 in array7)
					{
						if (!text9.IsNullOrEmpty())
						{
							paramFemale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text9, -1));
						}
					}
				}
				else
				{
					paramFemale.lstIdLayer.Add(-1);
				}
				paramFemale.lstFrontAndBehind.Clear();
				string[] array8 = data[j, num++].Split('/');
				foreach (string text10 in array8)
				{
					if (!text10.IsNullOrEmpty())
					{
						paramFemale.lstFrontAndBehind.Add(GlobalMethod.GetIntTryParse(text10));
					}
				}
				paramFemale.path.assetpath = data[j, num++];
				paramFemale.path.file = data[j, num++];
				paramFemale.isHitObject = data[j, num++] == "1";
				paramFemale.fileHit = data[j, num++];
				paramFemale.fileHitObject = data[j, num++];
				paramFemale.isYure = data[j, num++] == "1";
				paramFemale.fileYure = data[j, num++];
				paramFemale.fileDynamicBoneRef = data[j, num++];
				paramFemale.fileMotionNeck = data[j, num++];
				paramFemale.fileSe = data[j, num++];
				paramFemale.fileSiruPaste = data[j, num++];
				paramFemale.fileAibuEnableMotion = data[j, num++];
				paramFemale.fileReaction = data[j, num++];
				paramFemale.fileItem = data[j, num++];
				paramFemale.isAnal = data[j, num++] == "1";
				info.numFemaleUpperCloth1 = GlobalMethod.GetIntTryParse(data[j, num++]);
				info.numFemaleLowerCloth1 = GlobalMethod.GetIntTryParse(data[j, num++]);
				if (num >= length2)
				{
					continue;
				}
				info.mainHoushi3PShortVoicePtns = new int[4];
				array = data[j, num++].Split('/');
				if (array.Length == 4)
				{
					for (int num7 = 0; num7 < array.Length; num7++)
					{
						info.mainHoushi3PShortVoicePtns[num7] = GlobalMethod.GetIntTryParse(array[num7], -1);
					}
				}
				info.subHoushi3PShortVoicePtns = new int[4];
				array = data[j, num++].Split('/');
				if (array.Length == 4)
				{
					for (int num8 = 0; num8 < array.Length; num8++)
					{
						info.subHoushi3PShortVoicePtns[num8] = GlobalMethod.GetIntTryParse(array[num8], -1);
					}
				}
			}
		}
	}

	private void LoadLocalMoveList(string _file, out Vector3 _pos, out Vector3 _rot)
	{
		string text = GlobalMethod.LoadAllListText("h/list/", _file);
		_pos = Vector3.zero;
		_rot = Vector3.zero;
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			_pos.x = float.Parse(data[i, num++]);
			_pos.y = float.Parse(data[i, num++]);
			_pos.z = float.Parse(data[i, num++]);
			_rot.x = float.Parse(data[i, num++]);
			_rot.y = float.Parse(data[i, num++]);
			_rot.z = float.Parse(data[i, num++]);
		}
	}

	private void LoadCharaTagLayer(int _id)
	{
		List<Collider> value;
		if (!dicCharaCollder.TryGetValue(_id, out value))
		{
			value = (dicCharaCollder[_id] = new List<Collider>());
		}
		value.Clear();
		List<string> list2 = GlobalMethod.LoadAllListTextFromList("h/list/", "h_bone_tag_layer");
		if (!list2.Any() || lstFemale.Count <= _id)
		{
			return;
		}
		Transform transform = lstFemale[_id].objBodyBone.transform;
		string[,] data;
		GlobalMethod.GetListString(list2[list2.Count - 1], out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			GameObject gameObject = transform.FindLoop(data[i, num++]);
			if (!(gameObject == null))
			{
				gameObject.tag = data[i, num++];
				gameObject.layer = LayerMask.NameToLayer(data[i, num++]);
				Collider component = gameObject.GetComponent<Collider>();
				if (!(component == null))
				{
					component.enabled = true;
					value.Add(component);
				}
			}
		}
	}

	private void ShortCut()
	{
		if (sprite.isFade || sprite.GetFadeKindProc() == HSprite.FadeKindProc.OutEnd)
		{
			return;
		}
		if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
		{
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Manager.Config.EtcData.FemaleEyesCamera = !Manager.Config.EtcData.FemaleEyesCamera;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Manager.Config.EtcData.FemaleNeckCamera = !Manager.Config.EtcData.FemaleNeckCamera;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				bool[] source = new bool[5]
				{
					lstFemale[0].GetSiruFlags(ChaFileDefine.SiruParts.SiruKao) != 0,
					lstFemale[0].GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontUp) != 0,
					lstFemale[0].GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontDown) != 0,
					lstFemale[0].GetSiruFlags(ChaFileDefine.SiruParts.SiruBackUp) != 0,
					lstFemale[0].GetSiruFlags(ChaFileDefine.SiruParts.SiruBackDown) != 0
				};
				byte lv = (byte)((!source.Any((bool v) => v)) ? 2u : 0u);
				for (ChaFileDefine.SiruParts siruParts = ChaFileDefine.SiruParts.SiruKao; siruParts < (ChaFileDefine.SiruParts)5; siruParts++)
				{
					lstFemale[0].SetSiruFlags(siruParts, lv);
				}
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			Manager.Config.EtcData.FemaleEyesCamera1 = !Manager.Config.EtcData.FemaleEyesCamera1;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			Manager.Config.EtcData.FemaleNeckCamera1 = !Manager.Config.EtcData.FemaleNeckCamera1;
		}
		if (Input.GetKeyDown(KeyCode.Alpha5) && lstFemale.Count > 1)
		{
			bool[] source2 = new bool[5]
			{
				lstFemale[1].GetSiruFlags(ChaFileDefine.SiruParts.SiruKao) != 0,
				lstFemale[1].GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontUp) != 0,
				lstFemale[1].GetSiruFlags(ChaFileDefine.SiruParts.SiruFrontDown) != 0,
				lstFemale[1].GetSiruFlags(ChaFileDefine.SiruParts.SiruBackUp) != 0,
				lstFemale[1].GetSiruFlags(ChaFileDefine.SiruParts.SiruBackDown) != 0
			};
			byte lv2 = (byte)((!source2.Any((bool v) => v)) ? 2u : 0u);
			for (ChaFileDefine.SiruParts siruParts2 = ChaFileDefine.SiruParts.SiruKao; siruParts2 < (ChaFileDefine.SiruParts)5; siruParts2++)
			{
				lstFemale[1].SetSiruFlags(siruParts2, lv2);
			}
		}
	}

	private void LateShortCut()
	{
		if (!sprite.isFade && sprite.GetFadeKindProc() != HSprite.FadeKindProc.OutEnd)
		{
			GlobalMethod.CameraKeyCtrl(flags.ctrlCamera, lstFemale.ToArray());
		}
	}

	private void GotoPointMoveScene()
	{
		if (!hand.IsAction() && !flags.voiceWait && !categorys.Any((int c) => MathfEx.IsRange(2000, c, 2999, true)) && (flags.mode != HFlag.EMode.masturbation || flags.isFreeH))
		{
			lstOldFemaleVisible.Clear();
			for (int i = 0; i < lstFemale.Count; i++)
			{
				lstOldFemaleVisible.Add(lstFemale[i].visibleAll);
				lstFemale[i].visibleAll = false;
			}
			lstOldMaleVisible.Clear();
			lstOldMaleVisible.Add(male.visibleAll);
			male.visibleAll = false;
			item.SetVisible(false);
			hand.SceneChangeItemEnable(false);
			GameObject commonSpace = Singleton<Scene>.Instance.commonSpace;
			if (commonSpace != null)
			{
				DeliveryHPointData deliveryHPointData = commonSpace.AddComponent<DeliveryHPointData>();
				deliveryHPointData.actionSelect = ChangeCategory;
				deliveryHPointData.actionBack = CancelForHPointMove;
				deliveryHPointData.IDMap = map.no;
				deliveryHPointData.cam = flags.ctrlCamera;
				deliveryHPointData.lstCategory = lstInitCategory;
				deliveryHPointData.initPos = initPos;
				deliveryHPointData.initRot = initRot;
				deliveryHPointData.isFreeH = flags.isFreeH;
				deliveryHPointData.status = flags.lstHeroine[0].HExperience;
				deliveryHPointData.lstAnimInfo = lstAnimInfo;
				deliveryHPointData.isDebug = false;
				deliveryHPointData.flags = flags;
			}
			sprite.gameObject.SetActive(false);
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			UndoMapObject();
			raycaster.enabled = false;
			guideObject.gameObject.SetActive(false);
			meta.Clear();
			AllDoorOpenClose(true);
			CameraEffectorConfig component = flags.ctrlCamera.GetComponent<CameraEffectorConfig>();
			component.useDOF = false;
			CameraEffector component2 = flags.ctrlCamera.GetComponent<CameraEffector>();
			component2.dof.enabled = false;
			crossfade.FadeStart();
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "HPointMove",
				isAdd = true,
				isFade = false
			}, false);
		}
	}

	private void BackToCharaCollder(int _id)
	{
		List<Collider> value;
		if (!dicCharaCollder.TryGetValue(_id, out value))
		{
			return;
		}
		foreach (Collider item in value)
		{
			if (item != null)
			{
				item.enabled = false;
			}
		}
	}

	private bool GetMapObject()
	{
		UndoMapObject();
		mapObjectNull.lstChair.Clear();
		mapObjectNull.lstDesk.Clear();
		if (!kindMap)
		{
			return false;
		}
		List<GameObject> list = new List<GameObject>();
		list.Add(kindMap.gameObject);
		List<GameObject> list2 = list;
		list2.AddRange(kindMap.groups.Select((Transform t) => t.gameObject));
		list2.AddRange(kindMap.targets.Select((Transform t) => t.gameObject));
		foreach (GameObject item in list2)
		{
			item.SetActive(false);
			MeshCollider component = item.GetComponent<MeshCollider>();
			if ((bool)component)
			{
				component.isTrigger = false;
				component.convex = false;
			}
			if (item.name.Contains("isu") || item.name.Contains("chair"))
			{
				mapObjectNull.lstChair.Add(new MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			else if (item.name.Contains("desk"))
			{
				mapObjectNull.lstDesk.Add(new MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			item.layer = LayerMask.NameToLayer("HScene/MetaballB");
		}
		return true;
	}

	private bool GetMapObject(HPointData _data)
	{
		UndoMapObject();
		mapObjectNull.lstChair.Clear();
		mapObjectNull.lstDesk.Clear();
		if (!_data)
		{
			return false;
		}
		List<GameObject> list = new List<GameObject>();
		if ((bool)_data.selfTransform)
		{
			list.Add(_data.selfTransform.gameObject);
		}
		list.AddRange(_data.objGroups.Select((Transform t) => t.gameObject));
		list.AddRange(_data.objTargets.Select((Transform t) => t.gameObject));
		foreach (GameObject item in list)
		{
			item.SetActive(false);
			MeshCollider component = item.GetComponent<MeshCollider>();
			if (component != null)
			{
				component.isTrigger = false;
				component.convex = false;
			}
			if (item.name.Contains("isu") || item.name.Contains("chair"))
			{
				mapObjectNull.lstChair.Add(new MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			else if (item.name.Contains("desk"))
			{
				mapObjectNull.lstDesk.Add(new MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			item.layer = LayerMask.NameToLayer("HScene/MetaballB");
		}
		return true;
	}

	private bool SetMapObject(int _useChair, int _useDesk, string _pathAssetNullObject, string _fileNullObject)
	{
		Transform transform = lstFemale[0].transform;
		for (int i = 0; i < mapObjectNull.lstChair.Count; i++)
		{
			MapObjectNullInfo mapObjectNullInfo = mapObjectNull.lstChair[i];
			if ((bool)mapObjectNullInfo.obj)
			{
				mapObjectNullInfo.obj.SetActive(_useChair > i);
				mapObjectNullInfo.obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
			}
		}
		for (int j = 0; j < mapObjectNull.lstDesk.Count; j++)
		{
			MapObjectNullInfo mapObjectNullInfo2 = mapObjectNull.lstDesk[j];
			if ((bool)mapObjectNullInfo2.obj)
			{
				mapObjectNullInfo2.obj.SetActive(_useDesk > j);
				mapObjectNullInfo2.obj.transform.SetPositionAndRotation(transform.position, transform.rotation);
			}
		}
		if (_pathAssetNullObject.IsNullOrEmpty() || _fileNullObject.IsNullOrEmpty())
		{
			return false;
		}
		GameObject gameObject = CommonLib.LoadAsset<GameObject>(_pathAssetNullObject, _fileNullObject, false, string.Empty);
		flags.hashAssetBundle.Add(_pathAssetNullObject);
		if (!gameObject)
		{
			return false;
		}
		gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
		HSceneSpriteObjectCategory component = gameObject.GetComponent<HSceneSpriteObjectCategory>();
		if (!component)
		{
			return false;
		}
		for (int k = 0; k < _useChair && k < mapObjectNull.lstChair.Count; k++)
		{
			MapObjectNullInfo mapObjectNullInfo3 = mapObjectNull.lstChair[k];
			Transform transform2 = component.GetObject(k).transform;
			mapObjectNullInfo3.obj.transform.SetPositionAndRotation(transform2.position, transform2.rotation);
		}
		for (int l = 0; l < _useDesk && l < mapObjectNull.lstDesk.Count; l++)
		{
			MapObjectNullInfo mapObjectNullInfo4 = mapObjectNull.lstDesk[l];
			Transform transform3 = component.GetObject(l).transform;
			mapObjectNullInfo4.obj.transform.SetPositionAndRotation(transform3.position, transform3.rotation);
		}
		return true;
	}

	private bool SetCharacterPositon(bool _set)
	{
		if ((bool)kindMap && _set)
		{
			Transform transform = kindMap.transform;
			kindMap.MoveOffset();
			for (int i = 0; i < lstFemale.Count; i++)
			{
				lstFemale[i].transform.SetPositionAndRotation(transform.position, transform.rotation);
			}
			kindMap.ResetPosition();
			male.transform.SetPositionAndRotation(transform.position, transform.rotation);
		}
		return true;
	}

	private void UndoMapObject()
	{
		foreach (MapObjectNullInfo item in mapObjectNull.lstDesk)
		{
			if ((bool)item.obj)
			{
				item.obj.SetActive(true);
				item.obj.transform.SetPositionAndRotation(item.pos, item.rot);
				item.obj.layer = item.layer;
				MeshCollider component = item.obj.GetComponent<MeshCollider>();
				if ((bool)component)
				{
					component.convex = true;
					component.isTrigger = true;
				}
			}
		}
		foreach (MapObjectNullInfo item2 in mapObjectNull.lstChair)
		{
			if ((bool)item2.obj)
			{
				item2.obj.SetActive(true);
				item2.obj.transform.SetPositionAndRotation(item2.pos, item2.rot);
				item2.obj.layer = item2.layer;
				MeshCollider component2 = item2.obj.GetComponent<MeshCollider>();
				if ((bool)component2)
				{
					component2.convex = true;
					component2.isTrigger = true;
				}
			}
		}
	}

	public void ChangeCategory(HPointData _data, int _category)
	{
		ReturnVisibleForHPointMove();
		if (_data == null)
		{
			return;
		}
		_data.MoveOffset();
		Transform transform = _data.transform;
		ChaControl chaControl = lstFemale[0];
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].transform.SetPositionAndRotation(transform.position, transform.rotation);
		}
		male.transform.SetPositionAndRotation(transform.position, transform.rotation);
		if ((bool)objMoveAxis)
		{
			objMoveAxis.transform.SetPositionAndRotation(chaControl.transform.position, chaControl.transform.rotation);
		}
		_data.ResetPosition();
		_data.GetKindObject(map.mapObjectGroup);
		GetMapObject(_data);
		if ((flags.mode != HFlag.EMode.lesbian && !categorys.SymmetricExcept(_data.category).Any()) || (flags.mode == HFlag.EMode.lesbian && flags.nowAnimationInfo.lstCategory.Any((Category c) => c.category == _category)))
		{
			SetLocalPosition(flags.nowAnimationInfo);
			SetMapObject(flags.nowAnimationInfo.useChair, flags.nowAnimationInfo.useDesk, flags.nowAnimationInfo.pathMapObjectNull.assetpath, flags.nowAnimationInfo.pathMapObjectNull.file);
			int num = ((flags.click == HFlag.ClickKind.insert) ? 1 : ((flags.click == HFlag.ClickKind.insert_voice) ? 2 : 0));
			string strfile = flags.nowAnimationInfo.paramFemale.nameCamera;
			if (flags.mode == HFlag.EMode.houshi && num == 0)
			{
				strfile = flags.nowAnimationInfo.nameCameraIdle;
			}
			if (Manager.Config.EtcData.HInitCamera)
			{
				GlobalMethod.LoadCamera(flags.ctrlCamera, "h/list/", strfile);
			}
			else
			{
				GlobalMethod.LoadResetCamera(flags.ctrlCamera, "h/list/", strfile);
			}
		}
		else
		{
			if (flags.isFreeH || categorys.Count != 1 || (categorys.Count > 0 && categorys[0] < 1000))
			{
				categorys.Clear();
				categorys.AddRange(_data.category);
			}
			flags.mode = HFlag.EMode.none;
			CreateListAnimationFileName(false);
			sprite.Init(lstFemale, flags.lstHeroine[0], male, rely, lstUseAnimInfo, categorys, voice);
			ChangeAnimator(LoadAnimationListInfo(_category));
			sprite.InitCategoryActionToggle();
			sprite.InitPointMenuAndHelp(categorys, false);
		}
	}

	public void ReturnVisibleForHPointMove()
	{
		for (int i = 0; i < lstOldFemaleVisible.Count; i++)
		{
			lstFemale[i].visibleAll = lstOldFemaleVisible[i];
		}
		lstOldMaleVisible.SafeProc(0, delegate(bool visible)
		{
			male.visibleAll = visible;
		});
		AnimatorStateInfo animatorStateInfo = lstFemale[0].getAnimatorStateInfo(0);
		item.SetVisible(true);
		item.SyncPlay(animatorStateInfo.fullPathHash, animatorStateInfo.normalizedTime);
		sprite.gameObject.SetActive(true);
		hand.SceneChangeItemEnable(true);
		raycaster.enabled = true;
		guideObject.gameObject.SetActive(sprite.axis.tglDraw.isOn);
		AllDoorOpenClose(false);
		CameraEffectorConfig component = flags.ctrlCamera.GetComponent<CameraEffectorConfig>();
		component.useDOF = true;
	}

	public void CancelForHPointMove()
	{
		ReturnVisibleForHPointMove();
		crossfade.FadeStart();
		SetMapObject(flags.nowAnimationInfo.useChair, flags.nowAnimationInfo.useDesk, flags.nowAnimationInfo.pathMapObjectNull.assetpath, flags.nowAnimationInfo.pathMapObjectNull.file);
	}

	public IEnumerator ResetScene()
	{
		if ((bool)hand)
		{
			hand.DetachAllItem();
		}
		if (finishADV >= 200)
		{
			lstFemale[0].ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)lstCoordinateType[0]);
			for (ChaFileDefine.SiruParts siruParts = ChaFileDefine.SiruParts.SiruKao; siruParts < (ChaFileDefine.SiruParts)5; siruParts++)
			{
				lstFemale[0].SetSiruFlags(siruParts, 0);
			}
			lstFemale[0].SetClothesStateAll(0);
			lstFemale[0].SetAccessoryStateAll(true);
		}
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].mouthCtrl.SafeProc(delegate(FBSCtrlMouth m)
			{
				m.OpenMin = 0f;
			});
			lstFemale[i].ChangeLookNeckTarget(0, flags.ctrlCamera.thisCmaera.transform);
			lstFemale[i].ChangeLookEyesTarget(0, flags.ctrlCamera.thisCmaera.transform);
			lstFemale[i].ChangeLookNeckPtn(0);
			lstFemale[i].ChangeLookEyesPtn(0);
			lstFemale[i].SetPosition(Vector3.zero);
			lstFemale[i].SetRotation(Vector3.zero);
		}
		map.mapRoot.SetActive(true);
		dynamicCtrl.InitDynamicBoneReferenceBone();
		dynamicCtrl.IsInit = false;
		yure.Release();
		yure.ResetShape();
		if ((bool)particle)
		{
			particle.ReleaseObject(0);
			particle.ReleaseObject(1);
		}
		if ((bool)particle1)
		{
			particle1.ReleaseObject(0);
			particle1.ReleaseObject(1);
		}
		flags.lstHeroine.ForEach(delegate(SaveData.Heroine heroine)
		{
			heroine.charFile.status.visibleBodyAlways = true;
			heroine.chaCtrl.hideMoz = true;
			heroine.chaCtrl.SetPosition(Vector3.zero);
			heroine.chaCtrl.SetRotation(Vector3.zero);
		});
		flags.player.chaCtrl.fileStatus.visibleBodyAlways = true;
		flags.player.chaCtrl.fileStatus.visibleSon = true;
		flags.player.charFile.status.visibleBodyAlways = true;
		flags.player.charFile.status.visibleSon = true;
		if ((bool)parentObjectBaseFemale)
		{
			parentObjectBaseFemale.ReleaseObject();
		}
		if ((bool)parentObjectBaseFemale1)
		{
			parentObjectBaseFemale1.ReleaseObject();
		}
		if ((bool)parentObjectBaseMale)
		{
			parentObjectBaseMale.ReleaseObject();
		}
		if ((bool)parentObjectFemale)
		{
			parentObjectFemale.ReleaseObject();
		}
		if ((bool)parentObjectFemale1)
		{
			parentObjectFemale1.ReleaseObject();
		}
		if ((bool)parentObjectMale)
		{
			parentObjectMale.ReleaseObject();
		}
		item.ReleaseItem();
		UndoMapObject();
		flags.ctrlCamera.VisibleFroceVanish(true);
		flags.ctrlCamera.CleraVanishCollider();
		flags.ctrlCamera.isOutsideTargetTex = false;
		flags.ctrlCamera.Reset(0);
		InitLocalPosition(3);
		InitLocalPosition(7);
		sprite.LightColorAllInit();
		sprite.LightDirInit();
		foreach (KeyValuePair<int, List<Collider>> item in dicCharaCollder)
		{
			BackToCharaCollder(item.Key);
		}
		foreach (string item2 in flags.hashAssetBundle)
		{
			AssetBundleManager.UnloadAssetBundle(item2, true);
		}
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		Singleton<Voice>.Instance.StopAll();
		Singleton<Manager.Sound>.Instance.Stop(Manager.Sound.Type.GameSE3D);
		AllDoorOpenClose(true);
		if (flags.isFreeH)
		{
			yield return Singleton<Scene>.Instance.UnloadBaseSceneCoroutine("HProc");
		}
		if ((bool)male)
		{
			male.gameObject.SetActive(false);
		}
		Singleton<Character>.Instance.enableCorrectHandSize = true;
	}

	private AnimationListInfo LoadAnimationListInfo(int _category = -1, int _appointAction = -1)
	{
		switch (_appointAction)
		{
		case 0:
		case 3:
		{
			AnimationListInfo animationListInfo2 = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 8);
			if (animationListInfo2 != null)
			{
				return animationListInfo2;
			}
			break;
		}
		case 1:
		{
			AnimationListInfo animationListInfo4 = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 1);
			if (animationListInfo4 != null)
			{
				return animationListInfo4;
			}
			break;
		}
		case 2:
		{
			AnimationListInfo animationListInfo8 = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 5);
			if (animationListInfo8 != null)
			{
				return animationListInfo8;
			}
			break;
		}
		case 4:
		{
			AnimationListInfo animationListInfo7 = lstUseAnimInfo[2].Find((AnimationListInfo a) => a.id == 4);
			if (animationListInfo7 != null)
			{
				return animationListInfo7;
			}
			break;
		}
		case 5:
		{
			AnimationListInfo animationListInfo6 = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 17);
			if (animationListInfo6 != null)
			{
				return animationListInfo6;
			}
			break;
		}
		case 6:
		{
			AnimationListInfo animationListInfo3 = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 27);
			if (animationListInfo3 != null)
			{
				return animationListInfo3;
			}
			break;
		}
		case 8:
		{
			AnimationListInfo animationListInfo5 = lstUseAnimInfo[2].Find((AnimationListInfo a) => a.id == 28);
			if (animationListInfo5 != null)
			{
				return animationListInfo5;
			}
			break;
		}
		case 9:
		{
			AnimationListInfo animationListInfo = lstUseAnimInfo[1].Find((AnimationListInfo a) => a.id == 52);
			if (animationListInfo != null)
			{
				return animationListInfo;
			}
			break;
		}
		}
		if (_category != -1)
		{
			for (int i = 0; i < lstUseAnimInfo.Length; i++)
			{
				AnimationListInfo animationListInfo9 = lstUseAnimInfo[i].Find((AnimationListInfo a) => a.lstCategory.Any((Category c) => c.category == _category));
				if (animationListInfo9 != null)
				{
					return animationListInfo9;
				}
			}
		}
		for (int j = 0; j < lstUseAnimInfo.Length; j++)
		{
			AnimationListInfo animationListInfo10 = lstUseAnimInfo[j].Find((AnimationListInfo a) => a.lstCategory.Any((Category c) => c.category >= 1000));
			if (animationListInfo10 != null)
			{
				return animationListInfo10;
			}
		}
		for (int k = 0; k < lstUseAnimInfo.Length; k++)
		{
			if (lstUseAnimInfo[k].Any())
			{
				return lstUseAnimInfo[k][0];
			}
		}
		return null;
	}

	private bool SetLocalPosition(AnimationListInfo _info)
	{
		Vector3 _pos = Vector3.zero;
		Vector3 _rot = Vector3.zero;
		for (int i = 0; i < _info.lstCategory.Count; i++)
		{
			if (categorys.Contains(_info.lstCategory[i].category) && !_info.lstCategory[i].fileMove.IsNullOrEmpty())
			{
				LoadLocalMoveList(_info.lstCategory[i].fileMove, out _pos, out _rot);
				break;
			}
		}
		for (int j = 0; j < lstFemale.Count; j++)
		{
			lstFemale[j].SetPosition(_pos);
			lstFemale[j].SetRotation(_rot);
		}
		male.SetPosition(_pos);
		male.SetRotation(_rot);
		Transform worldBase = lstFemale[0].objTop.transform;
		item.SetTransform(worldBase);
		flags.ctrlCamera.SetWorldBase(worldBase);
		Vector3 position = lstFemale[0].gameObject.transform.TransformVector(_pos);
		if ((bool)guideObject)
		{
			guideObject.amount.position = position;
			guideObject.amount.rotation = _rot;
		}
		loadLocalPositon = position;
		loadLocalRotation = _rot;
		return true;
	}

	private void InitLocalPosition(int _init)
	{
		if (!(guideObject == null))
		{
			switch (_init)
			{
			case 0:
				guideObject.amount.position.x = loadLocalPositon.x;
				break;
			case 1:
				guideObject.amount.position.y = loadLocalPositon.y;
				break;
			case 2:
				guideObject.amount.position.z = loadLocalPositon.z;
				break;
			case 3:
				guideObject.amount.position = loadLocalPositon;
				break;
			case 4:
				guideObject.amount.rotation.x = loadLocalRotation.x;
				break;
			case 5:
				guideObject.amount.rotation.y = loadLocalRotation.y;
				break;
			case 6:
				guideObject.amount.rotation.z = loadLocalRotation.z;
				break;
			case 7:
				guideObject.amount.rotation = loadLocalRotation;
				break;
			}
		}
	}

	private void EndProc()
	{
		isEnd = true;
		SaveData.Heroine heroine = flags.lstHeroine[0];
		SaveData.Heroine heroine2 = ((flags.lstHeroine.Count <= 1) ? null : flags.lstHeroine[1]);
		ActionScene actScene = Singleton<Game>.Instance.actScene;
		if ((bool)actScene && actScene.isPenetration && (flags.mode == HFlag.EMode.houshi3P || flags.mode == HFlag.EMode.sonyu3P))
		{
			if (isFirstPlayMasturbation)
			{
				finishADV = 103;
			}
			else
			{
				finishADV = 101;
			}
		}
		else if (MathfEx.IsRange(7, appointAction, 10, true))
		{
			int[] array = new int[4] { 200, 201, 202, 203 };
			finishADV = array[appointAction - 7];
		}
		else if (heroine.fixCharaID < 0)
		{
			countOrg = flags.GetOrgCount();
			if (heroine.hCount == 0)
			{
				finishADV = 12;
			}
			else
			{
				finishADV = ((countOrg != 0) ? 14 : 13);
			}
		}
		else if (flags.count.isInsertKokan && heroine.isVirgin)
		{
			finishADV = 0;
			flags.player.AddHPeopleCount();
			Singleton<Game>.Instance.rankSaveData.expNumCount++;
		}
		else if (flags.GetOrgCount() == 0 && !categorys.Contains(1200))
		{
			finishADV = ((heroine.HExperience != SaveData.Heroine.HExperienceKind.淫乱) ? 6 : 7);
		}
		else if (flags.GetInsideAndOutsideCount() == 0 && !categorys.Contains(1002) && !categorys.Contains(1003))
		{
			finishADV = 8;
		}
		else if (isInvited)
		{
			finishADV = 11;
		}
		else if (isEasyPlace || flags.GetOrgCount() >= 3)
		{
			GlobalMethod.ShuffleRand shuffleRand = new GlobalMethod.ShuffleRand();
			List<int> list = new List<int>();
			if (isEasyPlace)
			{
				int num = ((heroine.lewdness < 80 || flags.GetOrgCount() == 0) ? 9 : 10);
				for (int i = 0; i < 3; i++)
				{
					list.Add(num);
				}
			}
			if (flags.GetOrgCount() >= 3)
			{
				for (int j = 0; j < 2; j++)
				{
					list.Add(4);
				}
			}
			shuffleRand.Init(list);
			finishADV = shuffleRand.Get();
			if (finishADV == -999999)
			{
				finishADV = (isEasyPlace ? 9 : ((flags.GetOrgCount() < 3) ? 10 : 4));
			}
		}
		else
		{
			GlobalMethod.ShuffleRand shuffleRand2 = new GlobalMethod.ShuffleRand();
			List<int> list2 = new List<int>();
			int[] array2 = new int[4] { 1, 1, 2, 3 };
			int[] array3 = new int[4] { 1, 1, 1, 2 };
			if (array2[(int)heroine.HExperience] != -1)
			{
				for (int k = 0; k < array3[(int)heroine.HExperience]; k++)
				{
					list2.Add(array2[(int)heroine.HExperience]);
				}
			}
			if (heroine.favor >= 70 && heroine.isGirlfriend)
			{
				for (int l = 0; l < 3; l++)
				{
					list2.Add(5);
				}
			}
			shuffleRand2.Init(list2);
			finishADV = shuffleRand2.Get();
			if (finishADV == -999999)
			{
				finishADV = 1;
			}
		}
		if (flags.mode != HFlag.EMode.peeping && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian)
		{
			heroine.AddHCount();
			if (heroine2 != null)
			{
				heroine2.AddHCount();
			}
		}
		if (heroine.isVirgin && flags.count.isInsertKokan)
		{
			heroine.isVirgin = false;
		}
		if (heroine.isAnalVirgin && flags.count.isInsertAnal)
		{
			heroine.isAnalVirgin = false;
		}
		for (int m = 0; m < heroine.massageExps.Length; m++)
		{
			heroine.massageExps[m] = Mathf.Min(heroine.massageExps[m] + flags.count.selectMassages[m], 100f);
		}
		if (!heroine.isKiss && flags.count.kiss != 0)
		{
			heroine.isKiss = true;
		}
		if (flags.count.isNameInsertKokan)
		{
			heroine.countNamaInsert = Mathf.Min(heroine.countNamaInsert + 1, 1000);
		}
		if (flags.count.isHoushiPlay)
		{
			heroine.countHoushi = Mathf.Min(heroine.countHoushi + 1, 1000);
		}
		CalcParameter(addParameter);
		heroine.hAreaExps[0] = Mathf.Min(heroine.hAreaExps[0] + flags.count.selectAreas[0], 100f);
		if (!flags.isUseImmediatelyFinishButton)
		{
			Singleton<Game>.Instance.rankSaveData.orgasmCount += (uint)flags.GetOrgCount();
			Singleton<Game>.Instance.rankSaveData.inEjaculationCount += (uint)(flags.count.sonyuInside + flags.count.sonyuCondomInside);
			Singleton<Game>.Instance.rankSaveData.inEjaculationKokanNamaCount += (uint)flags.count.sonyuInside;
			Singleton<Game>.Instance.rankSaveData.inEjaculationAnalCount += (uint)(flags.count.sonyuAnalInside + flags.count.sonyuAnalCondomInside);
			Singleton<Game>.Instance.rankSaveData.outEjaculationCount += (uint)(flags.count.sonyuOutside + flags.count.sonyuAnalOutside);
		}
		Singleton<Game>.Instance.rankSaveData.bustGauge += (uint)flags.count.selectAreasGlobal[1];
		Singleton<Game>.Instance.rankSaveData.hipGauge += (uint)flags.count.selectAreasGlobal[4];
		Singleton<Game>.Instance.rankSaveData.asokoGauge += (uint)flags.count.selectAreasGlobal[2];
		Singleton<Game>.Instance.rankSaveData.adultToyGauge += (uint)flags.count.selectHobby;
		if (flags.mode != HFlag.EMode.peeping && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian)
		{
			flags.player.AddHCount();
		}
		flags.player.AddOrgCount(flags.GetOrgCount());
		SaveData saveData = Singleton<Game>.Instance.saveData;
		saveData.ClubPointAdd(100);
	}

	private void NewHeroineEndProc()
	{
		isEnd = true;
		SaveData.Heroine heroine = flags.lstHeroine[0];
		if (flags.mode == HFlag.EMode.masturbation)
		{
			finishADV = ((!flags.isMasturbationFound || heroine.HExperience != SaveData.Heroine.HExperienceKind.淫乱) ? 104 : 105);
		}
		else if (flags.mode == HFlag.EMode.lesbian)
		{
			finishADV = 102;
		}
		else
		{
			finishADV = 100;
		}
		if (flags.mode != HFlag.EMode.peeping && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian)
		{
			heroine.AddHCount();
		}
		if (heroine.isAnalVirgin && flags.count.isInsertAnal)
		{
			heroine.isAnalVirgin = false;
		}
		for (int i = 0; i < heroine.massageExps.Length; i++)
		{
			heroine.massageExps[i] = Mathf.Min(heroine.massageExps[i] + flags.count.selectMassages[i], 100f);
		}
		if (!heroine.isKiss && flags.count.kiss != 0)
		{
			heroine.isKiss = true;
		}
		if (flags.count.isNameInsertKokan)
		{
			heroine.countNamaInsert = Mathf.Min(heroine.countNamaInsert + 1, 1000);
		}
		if (flags.count.isHoushiPlay)
		{
			heroine.countHoushi = Mathf.Min(heroine.countHoushi + 1, 1000);
		}
		heroine.hAreaExps[0] = Mathf.Min(heroine.hAreaExps[0] + flags.count.selectAreas[0], 100f);
		if (!flags.isUseImmediatelyFinishButton)
		{
			Singleton<Game>.Instance.rankSaveData.orgasmCount += (uint)flags.GetOrgCount();
			Singleton<Game>.Instance.rankSaveData.inEjaculationCount += (uint)(flags.count.sonyuInside + flags.count.sonyuCondomInside);
			Singleton<Game>.Instance.rankSaveData.inEjaculationKokanNamaCount += (uint)flags.count.sonyuInside;
			Singleton<Game>.Instance.rankSaveData.inEjaculationAnalCount += (uint)(flags.count.sonyuAnalInside + flags.count.sonyuAnalCondomInside);
			Singleton<Game>.Instance.rankSaveData.outEjaculationCount += (uint)(flags.count.sonyuOutside + flags.count.sonyuAnalOutside);
		}
		Singleton<Game>.Instance.rankSaveData.bustGauge += (uint)flags.count.selectAreasGlobal[1];
		Singleton<Game>.Instance.rankSaveData.hipGauge += (uint)flags.count.selectAreasGlobal[4];
		Singleton<Game>.Instance.rankSaveData.asokoGauge += (uint)flags.count.selectAreasGlobal[2];
		Singleton<Game>.Instance.rankSaveData.adultToyGauge += (uint)flags.count.selectHobby;
		if (flags.mode != HFlag.EMode.peeping && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian)
		{
			flags.player.AddHCount();
		}
		flags.player.AddOrgCount(flags.GetOrgCount());
	}

	private void CalcParameter(HScene.AddParameter _add)
	{
		SaveData.Heroine heroine = flags.lstHeroine[0];
		float num = ((!IsAddTaiiAllGet()) ? 1f : 1.25f);
		for (int i = 0; i < _add.aibus.Length; i++)
		{
			_add.aibus[i] += Mathf.Min(1000f, flags.count.selectAreas[i + 1] * num * flags.expCheat) * 0.1f;
			_add.aibus[i] = Mathf.Min(1000f, _add.aibus[i]);
		}
		int num2 = flags.count.houshiInside + flags.count.houshiOutside;
		for (int j = 0; j < num2; j++)
		{
			switch (j)
			{
			case 0:
				_add.houshi += 15f;
				break;
			case 1:
				_add.houshi += 10f;
				break;
			case 2:
				_add.houshi += 5f;
				break;
			default:
				_add.houshi += 1f;
				break;
			}
		}
		if (heroine.hCount <= 1 && _add.houshi < 15f && flags.firstHEasy)
		{
			_add.houshi = 15f;
		}
		_add.houshi = Mathf.Min(100f, _add.houshi * num * flags.expCheat);
		_add.sonyus[0] += (flags.count.isInsertKokan ? 5 : 0);
		num2 = flags.count.sonyuCondomInside + flags.count.sonyuOrg + flags.count.sonyuInside + flags.count.sonyuOutside;
		for (int k = 0; k < num2; k++)
		{
			switch (k)
			{
			case 0:
				_add.sonyus[0] += 15f;
				break;
			case 1:
				_add.sonyus[0] += 10f;
				break;
			case 2:
				_add.sonyus[0] += 5f;
				break;
			default:
				_add.sonyus[0] += 1f;
				break;
			}
		}
		_add.sonyus[0] = Mathf.Min(100f, _add.sonyus[0] * num * flags.expCheat);
		_add.sonyus[1] += (flags.count.isInsertAnal ? 5 : 0);
		num2 = flags.count.sonyuAnalCondomInside + flags.count.sonyuAnalOrg + flags.count.sonyuAnalInside + flags.count.sonyuAnalOutside;
		for (int l = 0; l < num2; l++)
		{
			switch (l)
			{
			case 0:
				_add.sonyus[1] += 15f;
				break;
			case 1:
				_add.sonyus[1] += 10f;
				break;
			case 2:
				_add.sonyus[1] += 5f;
				break;
			default:
				_add.sonyus[1] += 1f;
				break;
			}
		}
		_add.sonyus[1] = Mathf.Min(100f, _add.sonyus[1] * num * flags.expCheat);
	}

	private void SetConfig(bool _force = false)
	{
		if ((bool)male)
		{
			if (maleConfig.visibleSimple != Manager.Config.EtcData.SimpleBody || _force)
			{
				male.ChangeSimpleBodyDraw(Manager.Config.EtcData.SimpleBody);
				maleConfig.visibleSimple = Manager.Config.EtcData.SimpleBody;
			}
			if (maleConfig.color != Manager.Config.EtcData.SilhouetteColor || _force)
			{
				male.ChangeSimpleBodyColor(Manager.Config.EtcData.SilhouetteColor);
				maleConfig.color = Manager.Config.EtcData.SilhouetteColor;
			}
			if (maleConfig.cloth != Manager.Config.EtcData.IsMaleClothes || _force)
			{
				for (ChaFileDefine.ClothesKind clothesKind = ChaFileDefine.ClothesKind.top; clothesKind < ChaFileDefine.ClothesKind.shoes_inner; clothesKind++)
				{
					male.SetClothesState((int)clothesKind, (byte)((!Manager.Config.EtcData.IsMaleClothes) ? 3u : 0u));
				}
				maleConfig.cloth = Manager.Config.EtcData.IsMaleClothes;
			}
			if (maleConfig.accessoryMain != Manager.Config.EtcData.IsMaleAccessoriesMain || _force)
			{
				male.SetAccessoryStateCategory(0, Manager.Config.EtcData.IsMaleAccessoriesMain);
				maleConfig.accessoryMain = Manager.Config.EtcData.IsMaleAccessoriesMain;
			}
			if (maleConfig.accessorySub != Manager.Config.EtcData.IsMaleAccessoriesSub || _force)
			{
				male.SetAccessoryStateCategory(1, Manager.Config.EtcData.IsMaleAccessoriesSub);
				maleConfig.accessorySub = Manager.Config.EtcData.IsMaleAccessoriesSub;
			}
			if (maleConfig.shoes != Manager.Config.EtcData.IsMaleShoes || _force)
			{
				male.SetClothesState(7, (byte)((!Manager.Config.EtcData.IsMaleShoes) ? 3u : 0u));
				maleConfig.shoes = Manager.Config.EtcData.IsMaleShoes;
			}
		}
		map.mapRoot.SafeProc(delegate(GameObject o)
		{
			o.SetActive(Manager.Config.EtcData.Map);
		});
		if (map.no == 65)
		{
			flags.ctrlCamera.thisCmaera.clearFlags = (Manager.Config.EtcData.Map ? CameraClearFlags.Skybox : CameraClearFlags.Color);
		}
		flags.ctrlCamera.ConfigVanish = Manager.Config.EtcData.Shield;
		flags.ctrlCamera.isConfigTargetTex = Manager.Config.EtcData.Look;
		flags.ctrlCamera.thisCmaera.backgroundColor = Manager.Config.EtcData.BackColor;
	}

	private void SetClothStateStartMotion(int _cha)
	{
		if (lstFemale.Count > _cha)
		{
			GlobalMethod.SetAllClothState(lstFemale[_cha], true, (_cha != 0) ? flags.nowAnimationInfo.numFemaleUpperCloth1 : flags.nowAnimationInfo.numFemaleUpperCloth);
			GlobalMethod.SetAllClothState(lstFemale[_cha], false, (_cha != 0) ? flags.nowAnimationInfo.numFemaleLowerCloth1 : flags.nowAnimationInfo.numFemaleLowerCloth);
		}
	}

	private void LoadSpecialMapStartPosition(out int _no, out string _objName)
	{
		_objName = string.Empty;
		_no = -1;
		string text = GlobalMethod.LoadAllListText("h/list/", "SpecialMapStartPosition");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int num2 = int.Parse(data[i, num++]);
			if (categorys.Contains(num2))
			{
				_no = int.Parse(data[i, num++]);
				_objName = data[i, num++];
			}
		}
	}

	private bool SetState(SaveData.Heroine _heroine, int _status)
	{
		if (_heroine == null)
		{
			return false;
		}
		int[] array = new int[4] { 0, 1, 5, 5 };
		float[] array2 = new float[4] { 0f, 1f, 100f, 100f };
		float[] array3 = new float[4] { 0f, 1f, 100f, 100f };
		bool[] array4 = new bool[4] { true, false, false, false };
		bool[] array5 = new bool[4] { true, false, false, false };
		int[] array6 = new int[4] { 0, 1, 100, 100 };
		int[] array7 = new int[4] { 0, 1, 100, 100 };
		bool[] array8 = new bool[4] { false, true, true, true };
		int[] array9 = new int[4] { 0, 1, 100, 100 };
		int[] array10 = new int[4] { 0, 1, 100, 100 };
		int[] array11 = new int[4] { 0, 0, 0, 100 };
		_heroine.hCount = array[_status];
		for (int i = 0; i < _heroine.hAreaExps.Length; i++)
		{
			_heroine.hAreaExps[i] = array2[_status];
		}
		for (int j = 0; j < _heroine.massageExps.Length; j++)
		{
			_heroine.massageExps[j] = array3[_status];
		}
		_heroine.isVirgin = array4[_status];
		_heroine.isAnalVirgin = array5[_status];
		_heroine.countKokanH = array6[_status];
		_heroine.countAnalH = array7[_status];
		_heroine.isKiss = array8[_status];
		_heroine.countNamaInsert = array9[_status];
		_heroine.countHoushi = array10[_status];
		_heroine.lewdness = array11[_status];
		_heroine.favor = 100;
		return true;
	}

	private void LoadMapDependent(int _map)
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "h_map_dependent");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		Dictionary<int, MapDependent> dictionary = new Dictionary<int, MapDependent>();
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int key = int.Parse(data[i, num++]);
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new MapDependent());
			}
			dictionary[key].socks = int.Parse(data[i, num++]);
			dictionary[key].shoes = int.Parse(data[i, num++]);
			dictionary[key].glove = int.Parse(data[i, num++]);
		}
		if (!dictionary.ContainsKey(_map))
		{
			return;
		}
		if (dictionary[_map].glove != -1)
		{
			byte state = (byte)((dictionary[_map].glove == 1) ? 3u : 0u);
			for (int j = 0; j < lstFemale.Count; j++)
			{
				lstFemale[j].SetClothesState(4, state);
			}
			male.SetClothesState(4, state);
		}
		if (dictionary[_map].socks != -1)
		{
			byte state2 = (byte)((dictionary[_map].socks == 1) ? 3u : 0u);
			for (int k = 0; k < lstFemale.Count; k++)
			{
				lstFemale[k].SetClothesState(6, state2);
			}
			male.SetClothesState(6, state2);
		}
		if (dictionary[_map].shoes != -1)
		{
			byte state3 = (byte)((dictionary[_map].shoes == 1) ? 3u : 0u);
			for (int l = 0; l < lstFemale.Count; l++)
			{
				lstFemale[l].SetClothesState(7, state3);
				lstFemale[l].SetClothesState(8, state3);
			}
			male.SetClothesState(7, state3);
			male.SetClothesState(8, state3);
		}
	}

	private void AllDoorOpenClose(bool _isOpen)
	{
		if (map == null || map.mapRoot == null)
		{
			return;
		}
		Door[] componentsInChildren = map.mapRoot.transform.GetComponentsInChildren<Door>(true);
		Door[] array = componentsInChildren;
		foreach (Door door in array)
		{
			if (_isOpen)
			{
				door.OpenForce();
			}
			else
			{
				door.CloseForce();
			}
		}
	}

	private void SetShortcutKey()
	{
		ShortcutKey shortcutkey = base.gameObject.GetOrAddComponent<ShortcutKey>();
		ShortcutKey.Proc proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Escape;
		proc.enabled = true;
		ShortcutKey.Proc proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			Singleton<GameCursor>.Instance.SetCursorLock(false);
			GameCursor.isDraw = true;
			hand.ForceFinish();
			shortcutkey._GameEnd();
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F1;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			Singleton<GameCursor>.Instance.SetCursorLock(false);
			GameCursor.isDraw = true;
			hand.ForceFinish();
			shortcutkey._OpenConfig();
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F2;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			Singleton<GameCursor>.Instance.SetCursorLock(false);
			GameCursor.isDraw = true;
			hand.ForceFinish();
			shortcutkey._OpenShortcutKey();
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F5;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			Singleton<GameCursor>.Instance.SetCursorLock(false);
			GameCursor.isDraw = true;
			hand.ForceFinish();
			shortcutkey._OpenTutorial();
		});
		shortcutkey.procList.Add(proc2);
		if (!flags.isFreeH && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.peeping)
		{
			proc = new ShortcutKey.Proc();
			proc.keyCode = KeyCode.F6;
			proc.enabled = true;
			proc2 = proc;
			proc2.call.AddListener(delegate
			{
				Singleton<GameCursor>.Instance.SetCursorTexture(-1);
				Singleton<GameCursor>.Instance.SetCursorLock(false);
				GameCursor.isDraw = true;
				hand.ForceFinish();
				HScene.AddParameter addParameter = new HScene.AddParameter();
				CalcParameter(addParameter);
				SaveData.Heroine heroine = flags.lstHeroine[0];
				AdditionalFunctionsSystem addData = Manager.Config.AddData;
				int num = ((!addData.expH.isON) ? 1 : addData.expH.property);
				for (int i = 1; i < heroine.hAreaExps.Length; i++)
				{
					addParameter.aibus[i - 1] = Mathf.Min(heroine.hAreaExps[i] + addParameter.aibus[i - 1] * (float)num, 100f);
				}
				addParameter.houshi = Mathf.Min(heroine.houshiExp + addParameter.houshi * (float)num, 100f);
				addParameter.sonyus[0] = Mathf.Min(heroine.countKokanH + addParameter.sonyus[0] * (float)num, 100f);
				addParameter.sonyus[1] = Mathf.Min(heroine.countAnalH + addParameter.sonyus[1] * (float)num, 100f);
				spriteExp.SetHeroine(heroine, addParameter);
				spriteExp.gameObject.SetActive(true);
			});
			shortcutkey.procList.Add(proc2);
		}
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F11;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			gss.Capture(string.Empty);
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha1;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.Look = !Manager.Config.EtcData.Look;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha4;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.SemenType = (Manager.Config.EtcData.SemenType + 1) % 3;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha6;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.Map = !Manager.Config.EtcData.Map;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha7;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.Shield = !Manager.Config.EtcData.Shield;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha8;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			sprite.axis.tglDraw.isOn = !sprite.axis.tglDraw.isOn;
			sprite.MoveAxisDraw(sprite.axis.tglDraw.isOn);
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha9;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			if (sprite.moveAxizInteractive.lstTgl[0].isActiveAndEnabled)
			{
				sprite.moveAxizInteractive.lstTgl.First((Toggle t) => !t.isOn).isOn = true;
			}
			else
			{
				sprite.moveAxizInteractive.lstTgl.ForEach(delegate(Toggle t)
				{
					t.isOn = !t.isOn;
				});
			}
			sprite.ChangeMoveAxis((!sprite.moveAxizInteractive.lstTgl[0].isOn) ? 1 : 0);
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Z;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.VisibleBody = !Manager.Config.EtcData.VisibleBody;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.X;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.VisibleSon = !Manager.Config.EtcData.VisibleSon;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.C;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.SimpleBody = !Manager.Config.EtcData.SimpleBody;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.V;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.IsMaleClothes = !Manager.Config.EtcData.IsMaleClothes;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.B;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.IsMaleAccessoriesMain = !Manager.Config.EtcData.IsMaleAccessoriesMain;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.N;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.IsMaleAccessoriesSub = !Manager.Config.EtcData.IsMaleAccessoriesSub;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.M;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.IsMaleShoes = !Manager.Config.EtcData.IsMaleShoes;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.A;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			flags.isAibuSelect = !flags.isAibuSelect;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.S;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.HInitCamera = !Manager.Config.EtcData.HInitCamera;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.D;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			sprite.OnFemaleGaugeLockOnGauge();
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			sprite.OnMaleGaugeLockOnGauge();
		});
		shortcutkey.procList.Add(proc2);
	}

	private bool IsAddTaiiAllGet()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "HAddTaii");
		if (text.IsNullOrEmpty())
		{
			return false;
		}
		Dictionary<int, List<int>>[] array = new Dictionary<int, List<int>>[6];
		for (int j = 0; j < array.Length; j++)
		{
			array[j] = new Dictionary<int, List<int>>();
		}
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		int length2 = data.GetLength(1);
		for (int k = 0; k < length; k++)
		{
			int num = 0;
			int num2 = int.Parse(data[k, num++]);
			int key = int.Parse(data[k, num++]);
			if (!array[num2].ContainsKey(key))
			{
				array[num2].Add(key, new List<int>());
			}
			List<int> list = array[num2][key];
			list.Clear();
			while (length2 > num)
			{
				string text2 = data[k, num++];
				if (text2.IsNullOrEmpty())
				{
					break;
				}
				list.Add(int.Parse(text2));
			}
		}
		List<int> list2 = new List<int>();
		Dictionary<int, List<int>>[] array2 = array;
		foreach (Dictionary<int, List<int>> dictionary in array2)
		{
			foreach (List<int> value in dictionary.Values)
			{
				list2.AddRange(value);
			}
		}
		Dictionary<int, HashSet<int>> clubContents = Singleton<Game>.Instance.saveData.clubContents;
		HashSet<int> saveHas;
		if (!clubContents.TryGetValue(1, out saveHas))
		{
			clubContents[1] = (saveHas = new HashSet<int>());
		}
		return !list2.Any((int i) => !saveHas.Contains(i));
	}

	private void MapSameObjectDisable()
	{
		if (!map || map.no != 17)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < map.mapRoot.transform.childCount; i++)
		{
			Transform child = map.mapRoot.transform.GetChild(i);
			if (!(child.name != "p_o_koi_map17_00"))
			{
				num++;
				if (num >= 2)
				{
					child.gameObject.SetActive(false);
				}
			}
		}
	}
}
