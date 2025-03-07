using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionGame;
using ActionGame.MapObject;
using ChaCustom;
using H;
using HSceneUtility;
using Illusion;
using Illusion.Component;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HTest : BaseLoader
{
	public enum FixChara
	{
		指定なし = -1,
		養護教諭 = 0,
		数学教師 = 1,
		体育教師 = 2,
		担任 = 3,
		アイドル = 4
	}

	public enum PersonalChara
	{
		セクシーお姉さま = 0,
		お嬢様 = 1,
		タカビー = 2,
		小悪魔っ子 = 3,
		ミステリアス = 4,
		電波 = 5,
		大和撫子 = 6,
		ボーイッシュ = 7,
		純粋無垢な子供 = 8,
		アホの子 = 9,
		邪気眼 = 10,
		母性的お姉さん = 11,
		姉御肌 = 12,
		コギャル = 13,
		不良少女 = 14,
		野性的 = 15,
		意識高い系クール = 16,
		ひねくれ = 17,
		不幸少女 = 18,
		文学少女 = 19,
		もじもじ = 20,
		正統派ヒロイン = 21,
		ミーハー = 22,
		オタク少女 = 23,
		ヤンデレ = 24,
		ダルい系 = 25,
		無口 = 26,
		意地っ張り = 27,
		ロリババア = 28,
		素直クール = 29,
		気さく = 30,
		勝ち気 = 31,
		誠実 = 32,
		艶やか = 33,
		帰国子女 = 34,
		方言娘 = 35,
		Sっ気 = 36,
		無感情 = 37
	}

	public enum CharaState
	{
		初めて = 0,
		不慣れ = 1,
		慣れ = 2,
		淫乱 = 3
	}

	public enum SiruKind
	{
		中出し = 0,
		外出し = 1,
		吐く = 2,
		アナル = 3,
		アナル外出し = 4
	}

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

	public AnimatorLayerCtrl alCtrl;

	public AnimatorLayerCtrl alCtrl1;

	public GameObject objMoveAxis;

	public HSceneGuideObject guideObject;

	public DynamicBoneReferenceCtrl dynamicCtrl;

	public DynamicBoneReferenceCtrl dynamicCtrl1;

	public PhysicsRaycaster raycaster;

	public string nameMap = "教室1-1";

	public SunLightInfo.Info.Type defType;

	public clothesFileControl clothCustomCtrl;

	public Light lightCamera;

	public GameScreenShot gss;

	[Header("---< アニメーション変更 >---")]
	public List<int> categorys = new List<int>();

	public HFlag.EMode modeChange;

	public int idList;

	public HSceneProc.EExperience experience;

	[Button("ChangeAnimatorTest", "アニメーション変更", new object[] { true })]
	public int ChangeAnimatorButton;

	[Header("")]
	[Button("ChangeCategoryB", "カテゴリー変更", new object[] { })]
	public int ChangeCategoryButton;

	[Button("ChangeCloth", "服全部消す", new object[] { })]
	[Header("")]
	public int ChangeClothButton;

	[Button("MapObjectVisible", "椅子とか？消す", new object[] { false })]
	[Header("")]
	public int MapObjectDeleteButton;

	[Button("MapObjectVisible", "椅子とか？付ける", new object[] { true })]
	public int MapObjectVisibleButton;

	private GameObject objMapObjectGroup;

	[Button("SetDayTime", "昼間に設定", new object[] { })]
	[Header("時間帯")]
	public int SetDayTimeButton;

	[Button("SetEvening", "夕方に設定", new object[] { })]
	public int SetEveningButton;

	[Button("SetNight", "夜に設定", new object[] { })]
	public int SetNightButton;

	[Header("マップオブジェクトの取得")]
	[Label("取得したいマップオブジェクトの名前")]
	public string nameMapObject;

	[Button("GetMapObjectKind", "マップオブジェクトの取得", new object[] { })]
	public int GetMapObjectKindButton;

	[Label("女読み込みキャラファイル名")]
	[Header("キャラ指定")]
	public string loadFileName;

	[Label("固定キャラ指定")]
	public FixChara fixChara = FixChara.指定なし;

	[Label("性格指定")]
	public PersonalChara personalChara;

	[Label("状態指定")]
	public CharaState stateChara;

	[Label("読み込みファイルを指定している時、性格を変更する？")]
	public bool isChangePersonal;

	[Label("二人目を読み込むか")]
	[Header("二人目キャラ指定")]
	public bool isSecondFemaleLoad;

	[Label("女読み込みキャラファイル名")]
	public string loadFileName1;

	[Label("固定キャラ指定")]
	public FixChara fixChara1 = FixChara.指定なし;

	[Label("性格指定")]
	public PersonalChara personalChara1;

	[Label("状態指定")]
	public CharaState stateChara1;

	[Label("読み込みファイルを指定している時、性格を変更する？")]
	public bool isChangePersonal1;

	[Button("LoadVoiceList", "音声リスト読み込み", new object[] { })]
	public int LoadVoiceListButton;

	[Space]
	public GameObject objVoice;

	[Label("メタ汁テスト")]
	[Header("")]
	public SiruKind kindSiruTest;

	private List<ChaControl> lstFemale = new List<ChaControl>();

	private List<ChaControl> lstMale = new List<ChaControl>();

	private List<MotionIK> lstMotionIK = new List<MotionIK>();

	private MetaballCtrl meta;

	private ItemObject item = new ItemObject();

	private ActionMap map;

	private Kind kindMap;

	private List<HSceneProc.AnimationListInfo>[] lstAnimInfo = new List<HSceneProc.AnimationListInfo>[8];

	private List<HSceneProc.AnimationListInfo>[] lstUseAnimInfo = new List<HSceneProc.AnimationListInfo>[8];

	private List<HTestActionBase> lstProc = new List<HTestActionBase>();

	private Dictionary<int, List<Collider>> dicCharaCollder = new Dictionary<int, List<Collider>>();

	[SerializeField]
	private HSceneProc.MapObjectNull mapObjectNull = new HSceneProc.MapObjectNull();

	private HSceneProc.MaleConfig maleConfig = new HSceneProc.MaleConfig();

	private List<bool> lstOldFemaleVisible = new List<bool>();

	private List<bool> lstOldMaleVisible = new List<bool>();

	private Vector3 initPos = Vector3.zero;

	private Quaternion initRot = Quaternion.identity;

	private List<int> lstInitCategory = new List<int>();

	private Vector3 loadLocalPositon = Vector3.zero;

	private Vector3 loadLocalRotation = Vector3.zero;

	private HSceneProc.LightData lightData = new HSceneProc.LightData();

	private ShuffleRand[] voicePlayShuffle = new ShuffleRand[5];

	private IEnumerator Start()
	{
		if (Singleton<Game>.Instance.actScene == null)
		{
			map = base.gameObject.AddComponent<ActionMap>();
		}
		else
		{
			map = Singleton<Game>.Instance.actScene.GetComponent<ActionMap>();
		}
		yield return new WaitUntil(() => map.infoDic != null);
		SetShortcutKey();
		SaveData saveData = Singleton<Game>.Instance.saveData;
		Dictionary<int, HashSet<int>> clubContents = saveData.clubContents;
		HashSet<int> list;
		if (!clubContents.TryGetValue(0, out list))
		{
			HashSet<int> value = new HashSet<int>();
			clubContents[0] = value;
		}
		clubContents[0].Add(0);
		clubContents[0].Add(1);
		if (Singleton<Character>.IsInstance())
		{
			Singleton<Character>.Instance.enableCorrectHandSize = false;
		}
		int startMapNo;
		string startObjName;
		LoadSpecialMapStartPosition(out startMapNo, out startObjName);
		if (startMapNo != -1)
		{
			map.Change(startMapNo, Scene.Data.FadeType.None);
			yield return new WaitUntil(() => map.mapRoot != null);
			GameObject objStartPosition = map.mapRoot.transform.FindLoop(startObjName);
			if ((bool)objStartPosition)
			{
				initPos = objStartPosition.transform.position;
				initRot = objStartPosition.transform.rotation;
			}
		}
		else
		{
			map.Change(nameMap);
		}
		yield return new WaitUntil(() => map.mapRoot != null);
		if (defType == SunLightInfo.Info.Type.DayTime)
		{
			SetDayTime();
		}
		else if (defType == SunLightInfo.Info.Type.Evening)
		{
			SetEvening();
		}
		else
		{
			SetNight();
		}
		lightData.light = lightCamera;
		lightData.initColor = lightCamera.color;
		lightData.initRot = new Vector2(lightCamera.transform.localEulerAngles.x, lightCamera.transform.localEulerAngles.y);
		lightData.initIntensity = lightCamera.intensity;
		Load();
		ChaControl male = lstMale[0];
		ChaControl female = lstFemale[0];
		SaveData.Heroine heroine = flags.lstHeroine[0];
		ChaControl female2 = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		SaveData.Heroine heroine2 = ((flags.lstHeroine.Count <= 1) ? null : flags.lstHeroine[1]);
		female.transform.SetPositionAndRotation(initPos, initRot);
		male.transform.SetPositionAndRotation(initPos, initRot);
		if ((bool)objMoveAxis)
		{
			objMoveAxis.transform.SetPositionAndRotation(female.transform.position, female.transform.rotation);
		}
		lstInitCategory.Clear();
		lstInitCategory.AddRange(categorys);
		flags.mode = HFlag.EMode.none;
		CreateListAnimationFileName();
		flags.experience = experience;
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
		if ((bool)female2)
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
		voice.Init(heroine.ChaName, (heroine2 == null) ? string.Empty : heroine2.ChaName, "h/list/");
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
		if ((bool)female2)
		{
			dynamicCtrl1.Init(female2);
		}
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
		lstProc.Add(new HTestAibu(mem));
		lstProc.Add(new HTestHoushi(mem));
		(lstProc[1] as HTestHoushi).SetRely(rely);
		lstProc.Add(new HTestSonyu(mem));
		lstProc.Add(new HTestMasturbation(mem));
		lstProc.Add(new HTestPeeping(mem));
		(lstProc[4] as HTestPeeping).SetMapObject(map.mapRoot);
		lstProc.Add(new HTestLesbian(mem));
		lstProc.Add(new HTest3PHoushi(mem));
		(lstProc[6] as HTest3PHoushi).SetRely(rely);
		lstProc.Add(new HTest3PSonyu(mem));
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
		ChangeAnimatorTest(true);
		sprite.InitCategoryActionToggle();
		sprite.InitPointMenuAndHelp(categorys);
		subjective.SetSubjectiveObject(parentObjectBaseMale.GetObject("SubjectiveBase"));
		subjective.SetFemale(lstFemale[0]);
		subjective.SetMale(lstMale[0]);
		flags.isInsertOK[0] = flags.IsNamaInsertOK();
		List<int> hit = new List<int>();
		int[] hitcount = new int[4] { 1, 3, 5, 8 };
		for (int i = 0; i < hitcount[(int)flags.lstHeroine[0].HExperience]; i++)
		{
			hit.Add(1);
		}
		for (int j = 0; j < 10 - hitcount[(int)flags.lstHeroine[0].HExperience]; j++)
		{
			hit.Add(0);
		}
		GlobalMethod.ShuffleRand rand = new GlobalMethod.ShuffleRand();
		rand.Init(hit);
		int anal = rand.Get();
		flags.isAnalInsertOK = anal == 1 || flags.lstHeroine[0].denial.anal;
		flags.isCondom = false;
		female.hideMoz = false;
		if ((bool)female2)
		{
			female2.hideMoz = false;
		}
		flags.transVoiceMouth[0] = ((!female.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth)) ? null : female.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth).transform);
		if ((bool)female2)
		{
			flags.transVoiceMouth[1] = ((!female2.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth)) ? null : female2.GetReferenceInfo(ChaReference.RefObjKey.a_n_mouth).transform);
		}
		voice.voiceTrans[0] = flags.transVoiceMouth[0];
		voice.voiceTrans[1] = flags.transVoiceMouth[1];
		LoadMapDependent(map.no);
		female.SetAccessoryStateCategory(1, false);
		if ((bool)female2)
		{
			female2.SetAccessoryStateCategory(1, false);
		}
		female.SetVoiceTransform(objVoice.transform);
		if ((bool)female2)
		{
			female2.SetVoiceTransform(objVoice.transform);
		}
		flags.voice.MemberInit();
		Illusion.Game.Utils.Sound.Play(new Illusion.Game.Utils.Sound.SettingBGM(categorys.Any((int c) => c >= 2000) ? BGM.HScenePeep : ((heroine.HExperience != SaveData.Heroine.HExperienceKind.淫乱) ? BGM.HSceneGentle : BGM.HScene)));
		if (flags.mode == HFlag.EMode.masturbation)
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
				flags.voice.playVoices[0] = 337 + (flags.nowAnimationInfo.isFemaleInitiative ? 38 : 0);
			}
		}
		else if (categorys.Any((int c) => c < 2000))
		{
			flags.voice.playVoices[0] = 0;
		}
		flags.ctrlCamera.LoadVanish("h/list/", "map_col_" + map.no.ToString("00"), map.mapRoot);
		Singleton<Manager.Sound>.Instance.Listener = flags.ctrlCamera.transform;
		MapSameObjectDisable();
		for (int k = 0; k < voicePlayShuffle.Length; k++)
		{
			voicePlayShuffle[k] = new ShuffleRand();
			voicePlayShuffle[k].Init(2);
		}
		flags.isDebug = true;
	}

	public void Load()
	{
		SaveData.Heroine heroine;
		if (loadFileName != string.Empty)
		{
			ChaFileControl chaFileControl = new ChaFileControl();
			chaFileControl.LoadCharaFile(UserData.Path + "chara/female/" + loadFileName);
			heroine = new SaveData.Heroine(chaFileControl, false);
			if (isChangePersonal)
			{
				heroine.charFile.parameter.personality = (int)personalChara;
				SetState(heroine);
			}
		}
		else if (fixChara != FixChara.指定なし)
		{
			heroine = new SaveData.Heroine(false);
			string[] array = new string[5] { "c-1", "c-2", "c-3", "c-4", "c-5" };
			int[] array2 = new int[5] { -1, -2, -3, -4, -5 };
			heroine.charFile.LoadFromAssetBundle("action/fixchara/00.unity3d", array[(int)fixChara]);
			heroine.fixCharaID = array2[(int)fixChara];
		}
		else
		{
			List<SaveData.Heroine> list = Game.CreateFixCharaList();
			heroine = list[0];
			heroine.charFile.parameter.personality = (int)personalChara;
			heroine.fixCharaID = 0;
			SetState(heroine);
		}
		ChaControl chaControl = Singleton<Character>.Instance.CreateFemale(null, 0, heroine.charFile);
		heroine.SetRoot(chaControl.gameObject);
		flags.lstHeroine.Add(heroine);
		chaControl.releaseCustomInputTexture = false;
		chaControl.Load();
		Singleton<Game>.Instance.saveData.heroineList.Add(heroine);
		if (isSecondFemaleLoad)
		{
			if (loadFileName1 != string.Empty)
			{
				ChaFileControl chaFileControl2 = new ChaFileControl();
				chaFileControl2.LoadCharaFile(UserData.Path + "chara/female/" + loadFileName1);
				heroine = new SaveData.Heroine(chaFileControl2, false);
				if (isChangePersonal1)
				{
					heroine.charFile.parameter.personality = (int)personalChara1;
					SetState(heroine);
				}
			}
			else if (fixChara1 != FixChara.指定なし)
			{
				heroine = new SaveData.Heroine(false);
				string[] array3 = new string[5] { "c-1", "c-2", "c-3", "c-4", "c-5" };
				int[] array4 = new int[5] { -1, -2, -3, -4, -5 };
				heroine.charFile.LoadFromAssetBundle("action/fixchara/00.unity3d", array3[(int)fixChara1]);
				heroine.fixCharaID = array4[(int)fixChara1];
			}
			else
			{
				List<SaveData.Heroine> list2 = Game.CreateFixCharaList();
				heroine = list2[0];
				heroine.charFile.parameter.personality = (int)personalChara1;
				heroine.fixCharaID = 0;
				SetState(heroine);
			}
			ChaControl chaControl2 = Singleton<Character>.Instance.CreateFemale(null, 0, heroine.charFile);
			heroine.SetRoot(chaControl.gameObject);
			flags.lstHeroine.Add(heroine);
			chaControl2.releaseCustomInputTexture = false;
			chaControl2.Load();
			Singleton<Game>.Instance.saveData.heroineList.Add(heroine);
		}
		lstFemale.Clear();
		foreach (KeyValuePair<int, ChaControl> item in Singleton<Character>.Instance.dictEntryChara)
		{
			if (item.Value.sex == 1)
			{
				lstFemale.Add(item.Value);
			}
		}
		for (int i = 0; i < lstFemale.Count; i++)
		{
			LoadCharaTagLayer(i);
		}
		SaveData.Player player = new SaveData.Player(false);
		Game.LoadFromTextAsset(player);
		ChaControl chaControl3 = Singleton<Character>.Instance.CreateMale(null, 0, player.charFile);
		player.SetRoot(chaControl3.gameObject);
		chaControl3.releaseCustomInputTexture = false;
		chaControl3.Load();
		chaControl3.visibleAll = false;
		flags.player = player;
		lstMale.Clear();
		foreach (KeyValuePair<int, ChaControl> item2 in Singleton<Character>.Instance.dictEntryChara)
		{
			if (item2.Value.sex == 0)
			{
				lstMale.Add(item2.Value);
			}
		}
	}

	public void ReLoad(SaveData.Heroine _rerodeHeroine)
	{
	}

	private bool SetState(SaveData.Heroine _heroine)
	{
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
		int num = (int)stateChara;
		_heroine.hCount = array[num];
		for (int i = 0; i < _heroine.hAreaExps.Length; i++)
		{
			_heroine.hAreaExps[i] = array2[num];
		}
		for (int j = 0; j < _heroine.massageExps.Length; j++)
		{
			_heroine.massageExps[j] = array3[num];
		}
		_heroine.isVirgin = array4[num];
		_heroine.isAnalVirgin = array5[num];
		_heroine.countKokanH = array6[num];
		_heroine.countAnalH = array7[num];
		_heroine.isKiss = array8[num];
		_heroine.countNamaInsert = array9[num];
		_heroine.countHoushi = array10[num];
		_heroine.lewdness = array11[num];
		return true;
	}

	private void Update()
	{
		ChaControl female = ((lstFemale.Count <= 0) ? null : lstFemale[0]);
		ChaControl female2 = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		SetConfig();
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
		if ((bool)female)
		{
			female.ChangeNipRate(rate);
		}
		if ((bool)female2)
		{
			female2.ChangeNipRate(rate);
		}
		AnimatorStateInfo animatorStateInfo = default(AnimatorStateInfo);
		if (lstFemale.Count > 0)
		{
			animatorStateInfo = female.getAnimatorStateInfo(0);
		}
		eyeneckFemale.Proc(animatorStateInfo, flags.voice.eyenecks[0], flags.ctrlCamera.thisCmaera.gameObject, Manager.Config.EtcData.FemaleEyesCamera, Manager.Config.EtcData.FemaleNeckCamera);
		eyeneckFemale1.Proc(animatorStateInfo, flags.voice.eyenecks[1], flags.ctrlCamera.thisCmaera.gameObject, Manager.Config.EtcData.FemaleEyesCamera1, Manager.Config.EtcData.FemaleNeckCamera1);
		eyeneckMale.Proc(animatorStateInfo, flags.ctrlCamera.thisCmaera.gameObject);
		HSprite.FadeKindProc fadeKindProc = sprite.GetFadeKindProc();
		if (fadeKindProc != HSprite.FadeKindProc.OutEnd)
		{
			voice.Proc(animatorStateInfo, lstFemale);
		}
		if (!(Singleton<Scene>.Instance.NowSceneNames[0] != "HTest"))
		{
			if (lstProc.Count > (int)flags.mode)
			{
				lstProc[(int)flags.mode].Proc();
			}
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
		if (Singleton<Scene>.Instance.NowSceneNames[0] != "HTest")
		{
			if ((bool)dynamicCtrl)
			{
				dynamicCtrl.Proc();
			}
			return;
		}
		if (lstProc.Count > (int)flags.mode)
		{
			lstProc[(int)flags.mode].LateProc();
		}
		if (item != null)
		{
			item.LateUpdate();
		}
		dynamicCtrl.Proc();
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].SetPosition(lstFemale[i].gameObject.transform.InverseTransformVector(guideObject.amount.position));
			lstFemale[i].SetRotation(guideObject.amount.rotation);
		}
		if (lstFemale.Count != 0 && (bool)lstFemale[0])
		{
			item.SetTransform(lstFemale[0].objTop.transform);
		}
		if (lstMale.Count != 0 && (bool)lstMale[0])
		{
			lstMale[0].SetPosition(lstMale[0].gameObject.transform.InverseTransformVector(guideObject.amount.position));
			lstMale[0].SetRotation(guideObject.amount.rotation);
		}
		LateShortCut();
	}

	private void OnValidate()
	{
		flags.experience = experience;
	}

	private void ChangeAnimatorTest(bool _isForceCameraReset)
	{
		HFlag.EMode mode = flags.mode;
		flags.mode = modeChange;
		HSceneProc.AnimationListInfo animationListInfo = lstAnimInfo[(int)flags.mode].Find((HSceneProc.AnimationListInfo i) => i.id == idList);
		if (animationListInfo == null)
		{
			GlobalMethod.DebugLog("id:" + idList + "なんてないよ", 2);
		}
		else
		{
			ChangeAnimator(mode, animationListInfo, _isForceCameraReset);
		}
	}

	private HSceneProc.AnimationListInfo LoadAnimationListInfo()
	{
		for (int i = 0; i < lstUseAnimInfo.Length; i++)
		{
			if (lstUseAnimInfo[i].Any())
			{
				return lstUseAnimInfo[i][0];
			}
		}
		return null;
	}

	private void ChangeAnimator(HSceneProc.AnimationListInfo _nextAinmInfo)
	{
		HFlag.EMode mode = flags.nowAnimationInfo.mode;
		if (_nextAinmInfo != null)
		{
			flags.mode = _nextAinmInfo.mode;
			ChangeAnimator(mode, _nextAinmInfo, false);
		}
	}

	private void ChangeAnimator(HFlag.EMode _oldMode, HSceneProc.AnimationListInfo _info, bool _isForceCameraReset)
	{
		ChaControl chaControl = ((lstFemale.Count <= 1) ? null : lstFemale[1]);
		int num = 0;
		if (_info.mode == HFlag.EMode.houshi3P || _info.mode == HFlag.EMode.sonyu3P)
		{
			lstMotionIK.ForEach(delegate(MotionIK motionIK)
			{
				motionIK.Release();
			});
			lstMotionIK.Clear();
			if (_info.id % 2 == 0)
			{
				lstMotionIK.Add(new MotionIK(lstFemale[0]));
				lstMotionIK.Add(new MotionIK(lstMale[0]));
				if ((bool)chaControl)
				{
					lstMotionIK.Add(new MotionIK(chaControl));
				}
			}
			else
			{
				if ((bool)chaControl)
				{
					lstMotionIK.Add(new MotionIK(chaControl));
				}
				lstMotionIK.Add(new MotionIK(lstMale[0]));
				lstMotionIK.Add(new MotionIK(lstFemale[0]));
				num = 1;
			}
			lstMotionIK.ForEach(delegate(MotionIK motionIK)
			{
				motionIK.SetPartners(lstMotionIK);
				motionIK.Reset();
			});
		}
		HSceneProc.FemaleParameter paramFemale = _info.paramFemale;
		HSceneProc.FemaleParameter paramFemale2 = flags.nowAnimationInfo.paramFemale;
		if (paramFemale2.lstIdLayer.Count != 0 && paramFemale2.lstIdLayer[0] != -1)
		{
			lstFemale[0].setLayerWeight(0f, paramFemale2.lstIdLayer[0]);
		}
		if (flags.mode != _oldMode || flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P)
		{
			lstFemale[0].LoadAnimation(_info.pathFemaleBase.assetpath, _info.pathFemaleBase.file, string.Empty);
		}
		if (paramFemale.lstIdLayer[0] != -1)
		{
			lstFemale[0].setLayerWeight(1f, paramFemale.lstIdLayer[0]);
		}
		HSceneProc.PathName path = paramFemale.path;
		if (!path.assetpath.IsNullOrEmpty())
		{
			Animator chaAnimator = lstFemale[0].animBody;
			CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
			{
				chaAnimator.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator.runtimeAnimatorController, rac);
			});
			AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
		}
		else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
		{
			GlobalMethod.DebugLog(string.Concat("女の", flags.mode, "の", _info.id.ToString(), "番に", experience, "なんてないよ"), 2);
		}
		if (paramFemale.isHitObject)
		{
			lstFemale[0].LoadHitObject();
			hitcollisionFemale.Init(lstFemale[0], lstFemale[0].objHitHead);
			hitcollisionFemale.LoadText("h/list/", paramFemale.fileHit);
		}
		else
		{
			lstFemale[0].ReleaseHitObject();
			hitcollisionFemale.Release();
		}
		lstMotionIK[(num != 0) ? 2 : 0].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
		alCtrl.Load("h/list/", path.file);
		HSceneProc.FemaleParameter paramFemale3 = _info.paramFemale1;
		if ((bool)chaControl)
		{
			paramFemale2 = flags.nowAnimationInfo.paramFemale1;
			if (paramFemale2.lstIdLayer.Count != 0 && paramFemale2.lstIdLayer[0] != -1)
			{
				chaControl.setLayerWeight(0f, paramFemale2.lstIdLayer[0]);
			}
			if (flags.mode != _oldMode || flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P)
			{
				chaControl.LoadAnimation(_info.pathFemaleBase1.assetpath, _info.pathFemaleBase1.file, string.Empty);
			}
			if (paramFemale3.lstIdLayer[0] != -1)
			{
				chaControl.setLayerWeight(1f, paramFemale3.lstIdLayer[0]);
			}
			path = paramFemale3.path;
			if (!path.assetpath.IsNullOrEmpty())
			{
				Animator chaAnimator2 = chaControl.animBody;
				CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
				{
					chaAnimator2.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator2.runtimeAnimatorController, rac);
				});
				AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
			}
			else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
			{
				GlobalMethod.DebugLog(string.Concat("女の", flags.mode, "の", _info.id.ToString(), "番に", flags.experience, "なんてないよ"), 2);
			}
			if (paramFemale3.isHitObject)
			{
				chaControl.LoadHitObject();
				hitcollisionFemale1.Init(chaControl, chaControl.objHitHead);
				hitcollisionFemale1.LoadText("h/list/", paramFemale3.fileHit);
			}
			else
			{
				chaControl.ReleaseHitObject();
			}
			lstMotionIK[(num == 0) ? 2 : 0].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
			alCtrl1.Load("h/list/", path.file);
		}
		HSceneProc.MaleParameter paramMale = _info.paramMale;
		path = paramMale.path;
		if (_info.pathMaleBase.assetpath != string.Empty)
		{
			HSceneProc.MaleParameter paramMale2 = flags.nowAnimationInfo.paramMale;
			if (paramMale2.lstIdLayer.Count != 0 && paramMale2.lstIdLayer[0] != -1)
			{
				lstMale[0].setLayerWeight(0f, paramMale2.lstIdLayer[0]);
			}
			lstMale[0].chaFile.status.visibleBodyAlways = true;
			lstMale[0].visibleAll = true;
			if (flags.mode != _oldMode || flags.mode == HFlag.EMode.sonyu3P || flags.mode == HFlag.EMode.houshi3P)
			{
				lstMale[0].LoadAnimation(_info.pathMaleBase.assetpath, _info.pathMaleBase.file, string.Empty);
			}
			if (paramMale.lstIdLayer[0] != -1)
			{
				lstMale[0].setLayerWeight(1f, paramMale.lstIdLayer[0]);
			}
			if (!path.assetpath.IsNullOrEmpty())
			{
				Animator chaAnimator3 = lstMale[0].animBody;
				CommonLib.LoadAsset<RuntimeAnimatorController>(path.assetpath, path.file, false, string.Empty).SafeProc(delegate(RuntimeAnimatorController rac)
				{
					chaAnimator3.runtimeAnimatorController = Illusion.Utils.Animator.SetupAnimatorOverrideController(chaAnimator3.runtimeAnimatorController, rac);
				});
				AssetBundleManager.UnloadAssetBundle(path.assetpath, true);
			}
			else if (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P)
			{
				GlobalMethod.DebugLog(string.Concat("男の", flags.mode, "の", _info.id.ToString(), "番に", flags.experience, "なんてないよ"), 2);
			}
			lstMotionIK[1].LoadData(GlobalMethod.LoadAllFolderInOneFile<TextAsset>("h/list/", path.file));
			if (paramMale.isHitObject)
			{
				lstMale[0].LoadHitObject();
				hitcollisionMale.Init(lstMale[0], lstMale[0].objHitHead);
				hitcollisionMale.LoadText("h/list/", paramMale.fileHit);
			}
		}
		else if ((bool)lstMale[0].objBodyBone)
		{
			lstMale[0].visibleAll = false;
			lstMale[0].chaFile.status.visibleBodyAlways = true;
			lstMale[0].chaFile.status.visibleHeadAlways = true;
			lstMale[0].ReleaseHitObject();
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
		sprite.MainSpriteChange(_info);
		meta.Clear();
		meta.Load("h/list/", paramMale.fileMetaballCtrl, lstMale[0].objBodyBone, null, chaControl, flags.hashAssetBundle);
		item.LoadItem(paramFemale.fileItem, lstMale[0].objBodyBone, lstFemale.ToArray(), null, flags.hashAssetBundle);
		siru.Load("h/list/", paramFemale.fileSiruPaste);
		siru1.Load("h/list/", paramFemale3.fileSiruPaste);
		parentObjectFemale.LoadText("h/list/", paramFemale.fileHitObject);
		parentObjectMale.LoadText("h/list/", paramMale.fileHitObject);
		if ((bool)parentObjectFemale1)
		{
			parentObjectFemale1.LoadText("h/list/", paramFemale3.fileHitObject);
		}
		eyeneckFemale.Load("h/list/", paramFemale.fileMotionNeck);
		eyeneckMale.Load("h/list/", paramMale.fileMotionNeck);
		if ((bool)eyeneckFemale1)
		{
			eyeneckFemale1.Load("h/list/", paramFemale3.fileMotionNeck);
		}
		se.Load("h/list/", paramFemale.fileSe, lstMale[0].objBodyBone, lstFemale[0].objBodyBone, map.mapRoot);
		dynamicCtrl.Load("h/list/", paramFemale.fileDynamicBoneRef);
		if ((bool)dynamicCtrl1)
		{
			dynamicCtrl1.Load("h/list/", paramFemale3.fileDynamicBoneRef);
		}
		lstMale[0].chaFile.status.visibleSon = _info.numMaleSon == 1;
		List<int> useItemNumber = hand.GetUseItemNumber();
		foreach (int item in useItemNumber)
		{
			hand.DetachItemByUseItem(item);
		}
		hand.ForceFinish(false);
		int num2 = ((flags.click == HFlag.ClickKind.insert) ? 1 : ((flags.click == HFlag.ClickKind.insert_voice) ? 2 : 0));
		if (flags.mode == HFlag.EMode.houshi)
		{
			lstProc[(int)flags.mode].MotionChange(num2);
		}
		else if (flags.mode == HFlag.EMode.peeping)
		{
			lstProc[(int)flags.mode].MotionChange(_info.numCtrl);
		}
		else if (flags.mode == HFlag.EMode.lesbian || flags.mode == HFlag.EMode.masturbation)
		{
			lstProc[(int)flags.mode].MotionChange(_isForceCameraReset ? 1 : 0);
		}
		else
		{
			lstProc[(int)flags.mode].MotionChange(0);
		}
		SetLocalPosition(_info);
		string strfile = paramFemale.nameCamera;
		if (flags.mode == HFlag.EMode.houshi && num2 == 0)
		{
			strfile = _info.nameCameraIdle;
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
		SetMapObject(_info.useChair, _info.useDesk, _info.pathMapObjectNull.assetpath, _info.pathMapObjectNull.file);
		if (flags.nowAnimationInfo.mode == _info.mode)
		{
			flags.voice.playVoices[0] = -1;
			if (_info.mode == HFlag.EMode.aibu)
			{
				flags.voice.playVoices[0] = 9;
			}
			else if (_info.mode == HFlag.EMode.sonyu)
			{
				flags.voice.playVoices[0] = 10;
			}
			else if (_info.mode == HFlag.EMode.sonyu3P)
			{
				flags.voice.playVoices[voicePlayShuffle[0].Get()] = 21;
			}
		}
		else
		{
			flags.voice.playVoices[0] = ((_info.mode == HFlag.EMode.aibu) ? 100 : ((_info.mode != HFlag.EMode.houshi) ? (300 + (_info.isFemaleInitiative ? 38 : 0)) : 200));
		}
		flags.nowAnimationInfo = _info;
		sprite.InitHeroine(flags.lstHeroine);
		SetClothStateStartMotion(0);
		SetClothStateStartMotion(1);
		dynamicCtrl.Proc();
		if ((bool)dynamicCtrl1)
		{
			dynamicCtrl1.Proc();
		}
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastL).InitLocalPosition();
			lstFemale[i].getDynamicBoneBust(ChaInfo.DynamicBoneKind.BreastR).InitLocalPosition();
		}
		sprite.CloseMotionSubMenu();
		GlobalMethod.HGlobalSaveData((int)_info.mode, _info.id);
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
		bool flag = categorys.Any((int c) => MathfEx.IsRange(1010, c, 1099, true) || MathfEx.IsRange(1100, c, 1199, true));
		for (int i = 0; i < lstAnimInfo.Length; i++)
		{
			lstUseAnimInfo[i] = new List<HSceneProc.AnimationListInfo>();
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
				if (!lstAnimInfo[i][j].lstCategory.Any((HSceneProc.Category c) => categorys.Contains(c.category)))
				{
					continue;
				}
				if (!flags.isFreeH)
				{
					if ((lstAnimInfo[i][j].isRelease && !value.Contains(i * 1000 + lstAnimInfo[i][j].id) && !flags.isDebug) || (lstAnimInfo[i][j].isExperience != 2 && lstAnimInfo[i][j].isExperience > (int)flags.experience))
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
			string text = GlobalMethod.LoadAllListText("h/list/", "AnimationInfo_" + i.ToString("00"));
			lstAnimInfo[i] = new List<HSceneProc.AnimationListInfo>();
			if (text.IsNullOrEmpty())
			{
				continue;
			}
			string[,] data;
			GlobalMethod.GetListString(text, out data);
			int length = data.GetLength(0);
			int length2 = data.GetLength(1);
			for (int j = 0; j < length; j++)
			{
				int num = 0;
				int id = 0;
				int.TryParse(data[j, 1], out id);
				HSceneProc.AnimationListInfo animationListInfo = lstAnimInfo[i].Find((HSceneProc.AnimationListInfo l) => l.id == id);
				if (animationListInfo == null)
				{
					lstAnimInfo[i].Add(new HSceneProc.AnimationListInfo());
					animationListInfo = lstAnimInfo[i][lstAnimInfo[i].Count - 1];
				}
				animationListInfo.nameAnimation = data[j, num++];
				num++;
				animationListInfo.id = id;
				animationListInfo.mode = (HFlag.EMode)i;
				animationListInfo.lstCategory.Clear();
				string[] array = data[j, num++].Split('/');
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!text2.IsNullOrEmpty())
					{
						animationListInfo.lstCategory.Add(new HSceneProc.Category
						{
							category = int.Parse(text2)
						});
					}
				}
				int.TryParse(data[j, num++], out animationListInfo.posture);
				int.TryParse(data[j, num++], out animationListInfo.kindHoushi);
				int.TryParse(data[j, num++], out animationListInfo.isExperience);
				string text3 = data[j, num++];
				if (!text3.IsNullOrEmpty())
				{
					array = text3.Split('/');
					if (array.Length == animationListInfo.lstCategory.Count)
					{
						for (int m = 0; m < array.Length; m++)
						{
							if (!array[m].IsNullOrEmpty())
							{
								animationListInfo.lstCategory[m].fileMove = array[m];
							}
						}
					}
					else
					{
						GlobalMethod.DebugLog("カテゴリーの数とローカル移動リストの数が合わない", 1);
					}
				}
				animationListInfo.pathMapObjectNull.assetpath = data[j, num++];
				animationListInfo.pathMapObjectNull.file = data[j, num++];
				animationListInfo.useDesk = int.Parse(data[j, num++]);
				animationListInfo.useChair = int.Parse(data[j, num++]);
				animationListInfo.pathMaleBase.assetpath = data[j, num++];
				animationListInfo.pathMaleBase.file = data[j, num++];
				HSceneProc.MaleParameter paramMale = animationListInfo.paramMale;
				paramMale.lstIdLayer.Clear();
				text3 = data[j, num++];
				if (!text3.IsNullOrEmpty())
				{
					string[] array3 = text3.Split('/');
					foreach (string text4 in array3)
					{
						if (!text4.IsNullOrEmpty())
						{
							paramMale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text4, -1));
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
				animationListInfo.pathFemaleBase.assetpath = data[j, num++];
				animationListInfo.pathFemaleBase.file = data[j, num++];
				HSceneProc.FemaleParameter paramFemale = animationListInfo.paramFemale;
				paramFemale.lstIdLayer.Clear();
				text3 = data[j, num++];
				if (!text3.IsNullOrEmpty())
				{
					string[] array4 = text3.Split('/');
					foreach (string text5 in array4)
					{
						if (!text5.IsNullOrEmpty())
						{
							paramFemale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text5, -1));
						}
					}
				}
				else
				{
					paramFemale.lstIdLayer.Add(-1);
				}
				paramFemale.lstFrontAndBehind.Clear();
				string[] array5 = data[j, num++].Split('/');
				foreach (string text6 in array5)
				{
					if (!text6.IsNullOrEmpty())
					{
						paramFemale.lstFrontAndBehind.Add(GlobalMethod.GetIntTryParse(text6));
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
				int.TryParse(data[j, num++], out animationListInfo.numCtrl);
				int.TryParse(data[j, num++], out animationListInfo.sysTaii);
				animationListInfo.nameCameraIdle = data[j, num++];
				animationListInfo.nameCameraKiss = data[j, num++];
				animationListInfo.lstAibuSpecialItem.Clear();
				array = data[j, num++].Split(',');
				string[] array6 = array;
				foreach (string text7 in array6)
				{
					if (!text7.IsNullOrEmpty())
					{
						animationListInfo.lstAibuSpecialItem.Add(int.Parse(text7));
					}
				}
				animationListInfo.isFemaleInitiative = data[j, num++] == "1";
				array = data[j, num++].Split('/');
				if (array.Length == 2)
				{
					int.TryParse(array[0], out animationListInfo.houshiLoopActionW);
					int.TryParse(array[1], out animationListInfo.houshiLoopActionS);
				}
				animationListInfo.isSplash = data[j, num++] == "1";
				array = data[j, num++].Split(',');
				if (array.Length == 2)
				{
					int.TryParse(array[0], out animationListInfo.numMainVoiceID);
					int.TryParse(array[1], out animationListInfo.numSubVoiceID);
				}
				else
				{
					animationListInfo.numMainVoiceID = -1;
					animationListInfo.numSubVoiceID = -1;
				}
				animationListInfo.numVoiceKindID = GlobalMethod.GetIntTryParse(data[j, num++]);
				animationListInfo.numMaleSon = GlobalMethod.GetIntTryParse(data[j, num++]);
				animationListInfo.numFemaleUpperCloth = GlobalMethod.GetIntTryParse(data[j, num++]);
				animationListInfo.numFemaleLowerCloth = GlobalMethod.GetIntTryParse(data[j, num++]);
				animationListInfo.isRelease = data[j, num++] == "1";
				animationListInfo.stateRestriction = GlobalMethod.GetIntTryParse(data[j, num++]);
				if (num >= length2)
				{
					continue;
				}
				num++;
				num++;
				animationListInfo.pathFemaleBase1.assetpath = data[j, num++];
				animationListInfo.pathFemaleBase1.file = data[j, num++];
				paramFemale = animationListInfo.paramFemale1;
				paramFemale.lstIdLayer.Clear();
				text3 = data[j, num++];
				if (!text3.IsNullOrEmpty())
				{
					string[] array7 = text3.Split('/');
					foreach (string text8 in array7)
					{
						if (!text8.IsNullOrEmpty())
						{
							paramFemale.lstIdLayer.Add(GlobalMethod.GetIntTryParse(text8, -1));
						}
					}
				}
				else
				{
					paramFemale.lstIdLayer.Add(-1);
				}
				paramFemale.lstFrontAndBehind.Clear();
				string[] array8 = data[j, num++].Split('/');
				foreach (string text9 in array8)
				{
					if (!text9.IsNullOrEmpty())
					{
						paramFemale.lstFrontAndBehind.Add(GlobalMethod.GetIntTryParse(text9));
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
				animationListInfo.numFemaleUpperCloth1 = GlobalMethod.GetIntTryParse(data[j, num++]);
				animationListInfo.numFemaleLowerCloth1 = GlobalMethod.GetIntTryParse(data[j, num++]);
				if (num >= length2)
				{
					continue;
				}
				animationListInfo.mainHoushi3PShortVoicePtns = new int[4];
				array = data[j, num++].Split('/');
				if (array.Length == 4)
				{
					for (int num7 = 0; num7 < array.Length; num7++)
					{
						animationListInfo.mainHoushi3PShortVoicePtns[num7] = GlobalMethod.GetIntTryParse(array[num7], -1);
					}
				}
				animationListInfo.subHoushi3PShortVoicePtns = new int[4];
				array = data[j, num++].Split('/');
				if (array.Length == 4)
				{
					for (int num8 = 0; num8 < array.Length; num8++)
					{
						animationListInfo.subHoushi3PShortVoicePtns[num8] = GlobalMethod.GetIntTryParse(array[num8], -1);
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
		List<string> list = GlobalMethod.LoadAllListTextFromList("h/list/", "h_bone_tag_layer");
		ChaControl chaControl = lstFemale[_id];
		if (!dicCharaCollder.ContainsKey(_id))
		{
			dicCharaCollder.Add(_id, new List<Collider>());
		}
		List<Collider> list2 = dicCharaCollder[_id];
		list2.Clear();
		if (list.Count == 0)
		{
			return;
		}
		string[,] data;
		GlobalMethod.GetListString(list[list.Count - 1], out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			GameObject gameObject = chaControl.objBodyBone.transform.FindLoop(data[i, num++]);
			if (!(gameObject == null))
			{
				gameObject.tag = data[i, num++];
				gameObject.layer = LayerMask.NameToLayer(data[i, num++]);
				Collider component = gameObject.GetComponent<Collider>();
				if (!(component == null))
				{
					component.enabled = true;
					list2.Add(component);
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
		if (Input.GetKeyDown(KeyCode.J))
		{
			subjective.Play = !subjective.Play;
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			flags.isDebug = !flags.isDebug;
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			flags.lstHeroine[0].massageExps[0] = 100f;
			flags.lstHeroine[0].massageExps[1] = 100f;
			flags.lstHeroine[0].hAreaExps[0] = 1f;
			flags.lstHeroine[0].hAreaExps[1] = 1f;
			flags.count.aibuOrg = Mathf.Max(flags.count.aibuOrg, 1);
			flags.count.houshiInside = Mathf.Max(flags.count.houshiInside, 1);
			flags.count.sonyuOutside = Mathf.Max(flags.count.sonyuOutside, 1);
			flags.count.isInsertKokan = true;
			sprite.objFirstHHelpBase.SetActive(false);
			sprite.CreateActionList();
		}
		if (Input.GetKeyDown(KeyCode.H))
		{
			flags.lstHeroine[0].parameter.weakPoint = -1;
			flags.lstHeroine[0].isGirlfriend = true;
			flags.lstHeroine[0].isVirgin = false;
			flags.isInsertOK[0] = true;
			flags.isAnalInsertOK = true;
		}
		if (meta != null)
		{
			if (Input.GetKeyDown(KeyCode.Space) && meta.ctrlMetaball[(int)kindSiruTest] != null)
			{
				meta.ctrlMetaball[(int)kindSiruTest].ShootMetaBallStart(flags.isCondom);
			}
			if (Input.GetKeyDown(KeyCode.O))
			{
				meta.Clear();
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
			for (int j = 0; j < lstMale.Count; j++)
			{
				lstOldMaleVisible.Add(lstMale[j].visibleAll);
				lstMale[j].visibleAll = false;
			}
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
				deliveryHPointData.initPos = Vector3.zero;
				deliveryHPointData.initRot = Quaternion.identity;
				deliveryHPointData.isFreeH = flags.isFreeH;
				deliveryHPointData.status = flags.lstHeroine[0].HExperience;
				deliveryHPointData.lstAnimInfo = lstAnimInfo;
				deliveryHPointData.isDebug = flags.isDebug;
				deliveryHPointData.flags = flags;
			}
			sprite.ForceCloseAllMenu();
			sprite.gameObject.SetActive(false);
			Singleton<GameCursor>.Instance.SetCursorTexture(-1);
			UndoMapObject();
			raycaster.enabled = false;
			guideObject.gameObject.SetActive(false);
			meta.Clear();
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
		Transform[] groups = kindMap.groups;
		foreach (Transform transform in groups)
		{
			list2.Add(transform.gameObject);
		}
		Transform[] targets = kindMap.targets;
		foreach (Transform transform2 in targets)
		{
			list2.Add(transform2.gameObject);
		}
		foreach (GameObject item in list2)
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
				mapObjectNull.lstChair.Add(new HSceneProc.MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			else if (item.name.Contains("desk"))
			{
				mapObjectNull.lstDesk.Add(new HSceneProc.MapObjectNullInfo
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
		Transform[] objGroups = _data.objGroups;
		foreach (Transform transform in objGroups)
		{
			list.Add(transform.gameObject);
		}
		Transform[] objTargets = _data.objTargets;
		foreach (Transform transform2 in objTargets)
		{
			list.Add(transform2.gameObject);
		}
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
				mapObjectNull.lstChair.Add(new HSceneProc.MapObjectNullInfo
				{
					obj = item,
					pos = item.transform.position,
					rot = item.transform.rotation,
					layer = item.layer
				});
			}
			else if (item.name.Contains("desk"))
			{
				mapObjectNull.lstDesk.Add(new HSceneProc.MapObjectNullInfo
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
		for (int i = 0; i < mapObjectNull.lstChair.Count; i++)
		{
			HSceneProc.MapObjectNullInfo mapObjectNullInfo = mapObjectNull.lstChair[i];
			if ((bool)mapObjectNullInfo.obj)
			{
				mapObjectNullInfo.obj.SetActive(_useChair > i);
				mapObjectNullInfo.obj.transform.position = lstFemale[0].transform.position;
				mapObjectNullInfo.obj.transform.rotation = lstFemale[0].transform.rotation;
			}
		}
		for (int j = 0; j < mapObjectNull.lstDesk.Count; j++)
		{
			HSceneProc.MapObjectNullInfo mapObjectNullInfo2 = mapObjectNull.lstDesk[j];
			if ((bool)mapObjectNullInfo2.obj)
			{
				mapObjectNullInfo2.obj.SetActive(_useDesk > j);
				mapObjectNullInfo2.obj.transform.position = lstFemale[0].transform.position;
				mapObjectNullInfo2.obj.transform.rotation = lstFemale[0].transform.rotation;
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
		gameObject.transform.position = lstFemale[0].transform.position;
		gameObject.transform.rotation = lstFemale[0].transform.rotation;
		HSceneSpriteObjectCategory component = gameObject.GetComponent<HSceneSpriteObjectCategory>();
		if (!component)
		{
			return false;
		}
		for (int k = 0; k < _useChair && k < mapObjectNull.lstChair.Count; k++)
		{
			HSceneProc.MapObjectNullInfo mapObjectNullInfo3 = mapObjectNull.lstChair[k];
			Transform transform = component.GetObject(k).transform;
			mapObjectNullInfo3.obj.transform.position = transform.position;
			mapObjectNullInfo3.obj.transform.rotation = transform.rotation;
		}
		for (int l = 0; l < _useDesk && l < mapObjectNull.lstDesk.Count; l++)
		{
			HSceneProc.MapObjectNullInfo mapObjectNullInfo4 = mapObjectNull.lstDesk[l];
			Transform transform2 = component.GetObject(l).transform;
			mapObjectNullInfo4.obj.transform.position = transform2.position;
			mapObjectNullInfo4.obj.transform.rotation = transform2.rotation;
		}
		return true;
	}

	private bool SetCharacterPositon()
	{
		if ((bool)kindMap)
		{
			kindMap.MoveOffset();
			for (int i = 0; i < lstFemale.Count; i++)
			{
				lstFemale[i].transform.position = kindMap.transform.position;
				lstFemale[i].transform.rotation = kindMap.transform.rotation;
			}
			kindMap.ResetPosition();
		}
		lstMale[0].transform.position = kindMap.transform.position;
		lstMale[0].transform.rotation = kindMap.transform.rotation;
		return true;
	}

	private void UndoMapObject()
	{
		foreach (HSceneProc.MapObjectNullInfo item in mapObjectNull.lstDesk)
		{
			if ((bool)item.obj)
			{
				item.obj.SetActive(true);
				item.obj.transform.position = item.pos;
				item.obj.transform.rotation = item.rot;
				item.obj.layer = item.layer;
				MeshCollider component = item.obj.GetComponent<MeshCollider>();
				if ((bool)component)
				{
					component.convex = true;
					component.isTrigger = true;
				}
			}
		}
		foreach (HSceneProc.MapObjectNullInfo item2 in mapObjectNull.lstChair)
		{
			if ((bool)item2.obj)
			{
				item2.obj.SetActive(true);
				item2.obj.transform.position = item2.pos;
				item2.obj.transform.rotation = item2.rot;
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

	private void ChangeCategoryB()
	{
		flags.mode = HFlag.EMode.none;
		CreateListAnimationFileName(false);
		sprite.Init(lstFemale, flags.lstHeroine[0], lstMale[0], rely, lstUseAnimInfo, categorys, voice);
		ChangeAnimatorTest(false);
		sprite.InitCategoryActionToggle();
		sprite.InitPointMenuAndHelp(categorys, false);
	}

	public void ChangeCategory(HPointData _data, int _category)
	{
		ReturnVisibleForHPointMove();
		if (_data == null)
		{
			return;
		}
		_data.MoveOffset();
		for (int i = 0; i < lstFemale.Count; i++)
		{
			lstFemale[i].transform.position = _data.transform.position;
			lstFemale[i].transform.rotation = _data.transform.rotation;
		}
		lstMale[0].transform.position = _data.transform.position;
		lstMale[0].transform.rotation = _data.transform.rotation;
		if ((bool)objMoveAxis)
		{
			objMoveAxis.transform.SetPositionAndRotation(lstFemale[0].transform.position, lstFemale[0].transform.rotation);
		}
		_data.ResetPosition();
		_data.GetKindObject(map.mapObjectGroup);
		GetMapObject(_data);
		if (!categorys.SymmetricExcept(_data.category).Any())
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
			return;
		}
		categorys.Clear();
		categorys.AddRange(_data.category);
		flags.mode = HFlag.EMode.none;
		CreateListAnimationFileName(false);
		sprite.Init(lstFemale, flags.lstHeroine[0], lstMale[0], rely, lstUseAnimInfo, categorys, voice);
		HSceneProc.AnimationListInfo animationListInfo = null;
		if (_category != -1)
		{
			for (int j = 0; j < lstUseAnimInfo.Length; j++)
			{
				HSceneProc.AnimationListInfo animationListInfo2 = lstUseAnimInfo[j].FindAll((HSceneProc.AnimationListInfo a) => a.lstCategory.Any((HSceneProc.Category c) => c.category == _category)).FirstOrDefault();
				if (animationListInfo2 != null)
				{
					animationListInfo = animationListInfo2;
					break;
				}
			}
		}
		if (animationListInfo == null)
		{
			for (int k = 0; k < lstUseAnimInfo.Length; k++)
			{
				List<HSceneProc.AnimationListInfo> list = lstUseAnimInfo[k].FindAll((HSceneProc.AnimationListInfo a) => a.lstCategory.Any((HSceneProc.Category c) => c.category > 1000));
				if (list.Count != 0)
				{
					animationListInfo = list[0];
					break;
				}
			}
		}
		if (animationListInfo == null)
		{
			if (lstUseAnimInfo[0].Count != 0)
			{
				animationListInfo = lstUseAnimInfo[0][0];
			}
			else if (lstUseAnimInfo[1].Count != 0)
			{
				animationListInfo = lstUseAnimInfo[1][0];
			}
			else if (lstUseAnimInfo[2].Count != 0)
			{
				animationListInfo = lstUseAnimInfo[2][0];
			}
			else if (lstUseAnimInfo[3].Count != 0)
			{
				animationListInfo = lstUseAnimInfo[3][0];
			}
			else if (lstUseAnimInfo[4].Count != 0)
			{
				animationListInfo = lstUseAnimInfo[4][0];
			}
		}
		ChangeAnimator(animationListInfo);
		sprite.InitCategoryActionToggle();
		sprite.InitPointMenuAndHelp(categorys, false);
	}

	public void ReturnVisibleForHPointMove()
	{
		for (int i = 0; i < lstOldFemaleVisible.Count; i++)
		{
			lstFemale[i].visibleAll = lstOldFemaleVisible[i];
		}
		for (int j = 0; j < lstOldMaleVisible.Count; j++)
		{
			lstMale[j].visibleAll = lstOldMaleVisible[j];
		}
		AnimatorStateInfo animatorStateInfo = lstFemale[0].getAnimatorStateInfo(0);
		item.SetVisible(true);
		item.SyncPlay(animatorStateInfo.fullPathHash, animatorStateInfo.normalizedTime);
		sprite.gameObject.SetActive(true);
		hand.SceneChangeItemEnable(true);
		raycaster.enabled = true;
		guideObject.gameObject.SetActive(sprite.axis.tglDraw.isOn);
		CameraEffectorConfig component = flags.ctrlCamera.GetComponent<CameraEffectorConfig>();
		component.useDOF = true;
	}

	public void CancelForHPointMove()
	{
		ReturnVisibleForHPointMove();
		crossfade.FadeStart();
		SetMapObject(flags.nowAnimationInfo.useChair, flags.nowAnimationInfo.useDesk, flags.nowAnimationInfo.pathMapObjectNull.assetpath, flags.nowAnimationInfo.pathMapObjectNull.file);
	}

	private bool SetLocalPosition(HSceneProc.AnimationListInfo _info)
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
		for (int k = 0; k < lstMale.Count; k++)
		{
			lstMale[k].SetPosition(_pos);
			lstMale[k].SetRotation(_rot);
		}
		item.SetTransform(lstFemale[0].objTop.transform);
		flags.ctrlCamera.SetWorldBase(lstFemale[0].objTop.transform);
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

	private void SetConfig()
	{
		if (lstMale.Count != 0)
		{
			if (maleConfig.visibleSimple != Manager.Config.EtcData.SimpleBody)
			{
				lstMale[0].ChangeSimpleBodyDraw(Manager.Config.EtcData.SimpleBody);
				maleConfig.visibleSimple = Manager.Config.EtcData.SimpleBody;
			}
			if (maleConfig.color != Manager.Config.EtcData.SilhouetteColor)
			{
				lstMale[0].ChangeSimpleBodyColor(Manager.Config.EtcData.SilhouetteColor);
				maleConfig.color = Manager.Config.EtcData.SilhouetteColor;
			}
			if (maleConfig.cloth != Manager.Config.EtcData.IsMaleClothes)
			{
				for (ChaFileDefine.ClothesKind clothesKind = ChaFileDefine.ClothesKind.top; clothesKind < ChaFileDefine.ClothesKind.shoes_inner; clothesKind++)
				{
					lstMale[0].SetClothesState((int)clothesKind, (byte)((!Manager.Config.EtcData.IsMaleClothes) ? 3u : 0u));
				}
				maleConfig.cloth = Manager.Config.EtcData.IsMaleClothes;
			}
			if (maleConfig.accessoryMain != Manager.Config.EtcData.IsMaleAccessoriesMain)
			{
				lstMale[0].SetAccessoryStateCategory(0, Manager.Config.EtcData.IsMaleAccessoriesMain);
				maleConfig.accessoryMain = Manager.Config.EtcData.IsMaleAccessoriesMain;
			}
			if (maleConfig.accessorySub != Manager.Config.EtcData.IsMaleAccessoriesSub)
			{
				lstMale[0].SetAccessoryStateCategory(1, Manager.Config.EtcData.IsMaleAccessoriesSub);
				maleConfig.accessorySub = Manager.Config.EtcData.IsMaleAccessoriesSub;
			}
			if (maleConfig.shoes != Manager.Config.EtcData.IsMaleShoes)
			{
				lstMale[0].SetClothesState(7, (byte)((!Manager.Config.EtcData.IsMaleShoes) ? 3u : 0u));
				maleConfig.shoes = Manager.Config.EtcData.IsMaleShoes;
			}
		}
		map.mapRoot.SafeProc(delegate(GameObject o)
		{
			o.SetActive(Manager.Config.EtcData.Map);
		});
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

	private void LoadMapDependent(int _map)
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "h_map_dependent");
		if (text.IsNullOrEmpty())
		{
			return;
		}
		Dictionary<int, HSceneProc.MapDependent> dictionary = new Dictionary<int, HSceneProc.MapDependent>();
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int num = 0;
			int key = int.Parse(data[i, num++]);
			if (!dictionary.ContainsKey(key))
			{
				dictionary.Add(key, new HSceneProc.MapDependent());
			}
			dictionary[key].socks = int.Parse(data[i, num++]);
			dictionary[key].shoes = int.Parse(data[i, num++]);
			dictionary[key].glove = int.Parse(data[i, num++]);
		}
		if (dictionary.ContainsKey(_map))
		{
			if (dictionary[_map].glove != -1)
			{
				byte state = (byte)((dictionary[_map].glove == 1) ? 3u : 0u);
				lstFemale[0].SetClothesState(4, state);
				lstFemale[1].SetClothesState(4, state);
				lstMale[0].SetClothesState(4, state);
			}
			if (dictionary[_map].socks != -1)
			{
				byte state2 = (byte)((dictionary[_map].socks == 1) ? 3u : 0u);
				lstFemale[0].SetClothesState(6, state2);
				lstFemale[1].SetClothesState(6, state2);
				lstMale[0].SetClothesState(6, state2);
			}
			if (dictionary[_map].shoes != -1)
			{
				byte state3 = (byte)((dictionary[_map].shoes == 1) ? 3u : 0u);
				lstFemale[0].SetClothesState(7, state3);
				lstFemale[0].SetClothesState(8, state3);
				lstFemale[1].SetClothesState(7, state3);
				lstFemale[1].SetClothesState(8, state3);
				lstMale[0].SetClothesState(7, state3);
				lstMale[0].SetClothesState(8, state3);
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
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.F11;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Illusion.Game.Utils.Sound.Play(SystemSE.photo);
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
		proc.keyCode = KeyCode.Alpha2;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.FemaleEyesCamera = !Manager.Config.EtcData.FemaleEyesCamera;
		});
		shortcutkey.procList.Add(proc2);
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha3;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
		{
			Manager.Config.EtcData.FemaleNeckCamera = !Manager.Config.EtcData.FemaleNeckCamera;
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
		proc = new ShortcutKey.Proc();
		proc.keyCode = KeyCode.Alpha5;
		proc.enabled = true;
		proc2 = proc;
		proc2.call.AddListener(delegate
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

	private void ChangeCloth()
	{
		if (lstFemale.Count != 0 && !(lstFemale[0].objTop == null))
		{
			GameObject gameObject = lstFemale[0].objTop.transform.FindLoop("clothes_00");
			if ((bool)gameObject)
			{
				gameObject.SetActive(false);
			}
		}
	}

	private void MapObjectVisible(bool _visible)
	{
		if (objMapObjectGroup == null)
		{
			objMapObjectGroup = GameObject.Find("MapObjectGroup");
		}
		if ((bool)objMapObjectGroup)
		{
			objMapObjectGroup.SetActive(_visible);
		}
	}

	private void SetDayTime()
	{
		if (!(map.sunLightInfo == null) && map.sunLightInfo.Set(SunLightInfo.Info.Type.DayTime, flags.ctrlCamera.thisCmaera))
		{
		}
	}

	private void SetEvening()
	{
		if (!(map.sunLightInfo == null) && map.sunLightInfo.Set(SunLightInfo.Info.Type.Evening, flags.ctrlCamera.thisCmaera))
		{
		}
	}

	private void SetNight()
	{
		if (!(map.sunLightInfo == null) && map.sunLightInfo.Set(SunLightInfo.Info.Type.Night, flags.ctrlCamera.thisCmaera))
		{
		}
	}

	private void GetMapObjectKind()
	{
		GameObject gameObject = GameObject.Find(nameMapObject);
		if ((bool)gameObject)
		{
			kindMap = gameObject.GetComponent<Kind>();
			if ((bool)kindMap)
			{
				SetCharacterPositon();
				GetMapObject();
			}
		}
	}

	private void LoadVoiceList()
	{
		voice.Init(flags.lstHeroine[0].ChaName, (flags.lstHeroine.Count <= 1) ? string.Empty : flags.lstHeroine[1].ChaName, "h/list/");
	}
}
