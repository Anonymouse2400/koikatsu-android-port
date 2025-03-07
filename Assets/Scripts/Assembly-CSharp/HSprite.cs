using System;
using System.Collections.Generic;
using System.Linq;
using ChaCustom;
using H;
using HSceneUtility;
using Illusion.Component.UI;
using Illusion.CustomAttributes;
using Illusion.Game;
using IllusionUtility.GetUtility;
using Manager;
using SceneAssist;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HSprite : MonoBehaviour
{
	public enum FadeKind
	{
		Out = 0,
		In = 1,
		OutIn = 2
	}

	public enum FadeKindProc
	{
		None = 0,
		Out = 1,
		OutEnd = 2,
		In = 3,
		InEnd = 4,
		OutIn = 5,
		OutInEnd = 6
	}

	public class AnimationInfoComponent : MonoBehaviour
	{
		public HSceneProc.AnimationListInfo info;
	}

	[Serializable]
	public class SpriteBase
	{
		[Label("ID")]
		public int id;

		[Label("親オブジェクト")]
		public GameObject mainObj;
	}

	[Serializable]
	public class AibuSprite : SpriteBase
	{
	}

	[Serializable]
	public class HoushiSprite : SpriteBase
	{
		[Label("アクションボタン(吹き出し)")]
		public HSceneSpriteCategory categoryActionButton;

		[Label("オートフィニッシュ")]
		public Toggle tglAutoFinish;

		[Label("パッド")]
		public SpriteClickChangeCtrl clickPadCtrl;

		[Label("パッドImage")]
		public Image imagePad;

		[Label("おまかせ")]
		public Toggle tglRely;

		[Label("パッドImageCtrl")]
		public SpriteChangeCtrl imageCtrlPad;
	}

	[Serializable]
	public class SonyuSprite : SpriteBase
	{
		[Label("アクションボタン(吹き出し)")]
		public HSceneSpriteCategory categoryActionButton;

		[Label("オートフィニッシュ")]
		public Toggle tglAutoFinish;

		[Label("パッド")]
		public SpriteClickChangeCtrl clickPadCtrl;

		[Label("パッドImage")]
		public Image imagePad;

		[Label("コンドームボタン")]
		public Button buttonCondom;

		[Label("コンドームのスプライト変更")]
		public ButtonSpriteChangeCtrl buttonCondomChange;

		[Label("コンドームの状態")]
		public bool isCondom;

		[Label("パッドImageCtrl")]
		public SpriteChangeCtrl imageCtrlPad;
	}

	[Serializable]
	public class MasturbationSprite : SpriteBase
	{
	}

	[Serializable]
	public class PeepingSprite : SpriteBase
	{
		[Label("覗きリスタートチェックImage")]
		public Image imageReplayCheck;

		[Label("着替えリスト親オブジェクト")]
		public GameObject objPeepingFemaleList;

		[Label("content")]
		public GameObject objPeepingFemaleListContentParent;

		[Label("FemaleListNode")]
		public GameObject objPeepingFemaleListNameNode;

		[Label("着替えYesボタン")]
		public Button buttonClothChangeYes;
	}

	[Serializable]
	public class Houshi3PSprite : SpriteBase
	{
		[Label("アクションボタン(吹き出し)")]
		public HSceneSpriteCategory categoryActionButton;

		[Label("オートフィニッシュ")]
		public Toggle tglAutoFinish;

		[Label("パッド")]
		public SpriteClickChangeCtrl clickPadCtrl;

		[Label("パッドImage")]
		public Image imagePad;

		[Label("おまかせ")]
		public Toggle tglRely;

		[Label("パッドImageCtrl")]
		public SpriteChangeCtrl imageCtrlPad;
	}

	[Serializable]
	public class Sonyu3PSprite : SpriteBase
	{
		[Label("アクションボタン(吹き出し)")]
		public HSceneSpriteCategory categoryActionButton;

		[Label("オートフィニッシュ")]
		public Toggle tglAutoFinish;

		[Label("パッド")]
		public SpriteClickChangeCtrl clickPadCtrl;

		[Label("パッドImage")]
		public Image imagePad;

		[Label("コンドームボタン")]
		public Button buttonCondom;

		[Label("コンドームのスプライト変更")]
		public ButtonSpriteChangeCtrl buttonCondomChange;

		[Label("コンドームの状態")]
		public bool isCondom;

		[Label("パッドImageCtrl")]
		public SpriteChangeCtrl imageCtrlPad;
	}

	[Serializable]
	public class GaugeSprite
	{
		[Label("スライダー")]
		public Image imgBar;

		[Label("ロックToggle")]
		public Toggle tglLock;
	}

	[Serializable]
	public class Axis
	{
		public HSceneGuideObject guideObj;

		public HSceneSpriteObjectCategory categoryAxis;

		public Slider sliderSpeed;

		public Slider sliderScale;

		public Toggle tglDraw;
	}

	[Serializable]
	public class LightSlider
	{
		public Slider r;

		public Slider g;

		public Slider b;

		public Slider a;

		public Slider x;

		public Slider y;
	}

	[Serializable]
	public class MoveAxizInteractive
	{
		public List<Toggle> lstTgl = new List<Toggle>();

		public List<Button> lstBtn = new List<Button>();

		public List<Slider> lstSld = new List<Slider>();

		public void SetInteractive(bool _interactive)
		{
			lstTgl.ForEach(delegate(Toggle t)
			{
				t.interactable = _interactive;
			});
			lstBtn.ForEach(delegate(Button b)
			{
				b.interactable = _interactive;
			});
			lstSld.ForEach(delegate(Slider s)
			{
				s.interactable = _interactive;
			});
		}
	}

	[Serializable]
	public class MultipleFemaleSubGroup
	{
		public List<GameObject> lstGroup = new List<GameObject>();
	}

	[Serializable]
	public class FemaleDressButtonCategory
	{
		public HSceneSpriteCategory dress;

		public HSceneSpriteCategory dressAll;

		public HSceneSpriteCategory accessory;

		public HSceneSpriteCategory accessoryAll;

		public HSceneSpriteCategory coordinate;
	}

	private enum VoicePlayShuffleKind
	{
		ActionChange = 0,
		PoseChange = 1
	}

	private delegate bool AutoFinishDelegate(bool _force = false);

	private delegate bool SpriteStateProcDelegate();

	[Label("フラグ管理スクリプト")]
	public HFlag flags;

	[Label("スピードスライダー親オブジェクト")]
	public GameObject objSpeed;

	[Label("スピードスライダー愛撫下地オブジェクト")]
	public GameObject objSpeedAibuBase;

	[Label("スピードスライダーバーimage")]
	public Image imageSpeed;

	[Label("HandCtrlスクリプト")]
	public HandCtrl hand;

	public AibuSprite aibu;

	public HoushiSprite houshi;

	public SonyuSprite sonyu;

	public MasturbationSprite masturbation;

	public PeepingSprite peeping;

	public Houshi3PSprite houshi3P;

	public Sonyu3PSprite sonyu3P;

	public RawImage[] commonAibuIcons = new RawImage[3];

	public GaugeSprite[] gauge = new GaugeSprite[2];

	[Label("メインメニューHSceneSpriteCategory")]
	public HSceneSpriteCategory menuMain;

	[Label("アクションメニューHSceneSpriteCategory")]
	public HSceneSpriteCategory menuAction;

	[Label("サブメニューHSceneSpriteCategory")]
	public HSceneSpriteObjectCategory menuSub;

	[Label("アクションサブメニューHSceneSpriteCategory")]
	public HSceneSpriteObjectCategory menuActionSub;

	public GameObject[] objMenuSubSubs = new GameObject[3];

	[Label("服装HSceneSpriteCategory")]
	public HSceneSpriteCategory categoryDress;

	[Label("服HSceneSpriteCategory")]
	public HSceneSpriteCategory categoryCloth;

	[Label("服まとめHSceneSpriteCategory")]
	public HSceneSpriteCategory categoryClothAll;

	[Label("アクセサリーHSceneSpriteCategory")]
	public HSceneSpriteCategory categoryAccessory;

	[Label("アクセサリーまとめHSceneSpriteCategory")]
	public HSceneSpriteCategory categoryAccessoryAll;

	public SelectUI[] selectUIPads;

	[Label("奉仕行為Toggleカテゴリー")]
	public HSceneSpriteToggleCategory categoryToggleHoushi;

	[Label("モーションリストNodeオブジェクト")]
	public GameObject objMotionListNode;

	[Label("Hシーン終了ボタン")]
	public Button btnEnd;

	[Label("愛撫時の7割スライダーcover")]
	public Image imageSpeedSlliderCover70;

	public Axis axis;

	public clothesFileControl clothCusutomCtrl;

	public LightSlider lightSlider = new LightSlider();

	[Label("愛撫アイコンの親オブジェクト")]
	public GameObject objCommonAibuIcon;

	[Label("愛撫アイコンカテゴリー")]
	public HSceneSpriteObjectCategory categoryAibuIcon;

	public MoveAxizInteractive moveAxizInteractive = new MoveAxizInteractive();

	[Label("初HHelp親オブジェクト")]
	public GameObject objFirstHHelpBase;

	[Label("ヘルプ自動消去")]
	public HSpriteAutoDisable autoDisableFirstHHelp;

	[Label("ヘルプスプライト変更")]
	public SpriteChangeCtrl helpSpriteChange;

	[Label("ヘルプテキスト変更")]
	public TextChangeCtrl helpTextChange;

	[Label("生理周期カテゴリー")]
	public HSceneSpriteObjectCategory categoryMenstruation;

	[Label("慣れカテゴリー")]
	public HSceneSpriteObjectCategory categoryExperience;

	[Label("顔RawImage")]
	public RawImage rawFace;

	[Label("MainMenuの親オブジェクト")]
	public GameObject objBaseMainMenu;

	[Label("MainMenuOpenButtonObject")]
	public GameObject objMainMenuOpen;

	[Label("AibuItemIconOpenオブジェクト")]
	public GameObject objAibuItemIconOpen;

	public List<HSceneSpriteCategory> lstMultipleFemaleGroup = new List<HSceneSpriteCategory>();

	public List<MultipleFemaleSubGroup> lstMultipleFemaleSubGroup = new List<MultipleFemaleSubGroup>();

	public List<FemaleDressButtonCategory> lstMultipleFemaleDressButton = new List<FemaleDressButtonCategory>();

	[Label("イキそうボタン親")]
	public GameObject objImmediatelyFinish;

	[Label("イキそうボタン女")]
	public Button btnImmediatelyFinishFemale;

	[Label("イキそうボタン男")]
	public Button btnImmediatelyFinishMale;

	[Label("乱入ボタン")]
	public Button btnTrespassing;

	[Label("乱入ボタンImage変更スクリプト")]
	public SpriteChangeCtrl helpSpriteTrespassing;

	[Label("乱入ボタンText変更スクリプト")]
	public TextChangeCtrl helpTextTrespassing;

	[Label("乱入ボタンヘルプ自動消去")]
	public HSpriteAutoDisable autoDisableTrespassingHelp;

	public UnityAction<int> localMoveInitAction;

	[Label("フェード用RawImage")]
	[Header("フェード")]
	public RawImage imageFade;

	[Label("基本フェード時間")]
	public float timeFadeBase;

	[Label("フェード処理をしてるか")]
	public bool isFade;

	[Label("フェードアニメーション")]
	public AnimationCurve fadeAnimation;

	private List<ChaControl> females = new List<ChaControl>();

	private ChaControl male;

	private SaveData.Heroine heroine;

	private AutoRely rely;

	private List<HSceneProc.AnimationListInfo>[] lstUseAnimInfo = new List<HSceneProc.AnimationListInfo>[8];

	private List<HSceneProc.AnimationListInfo>[] lstHoushiAnimInfo = new List<HSceneProc.AnimationListInfo>[3];

	private List<HSceneProc.AnimationListInfo>[] lst3PHoushiAnimInfo = new List<HSceneProc.AnimationListInfo>[3];

	[SerializeField]
	private List<int> lstHoushiPose = new List<int>();

	private Dictionary<int, string> dicHoushiPoseName = new Dictionary<int, string>();

	private FadeKind kindFade;

	private FadeKindProc kindFadeProc;

	private float timeFade;

	private float timeFadeTime;

	private ReactiveProperty<SaveData.Heroine> resultPeepingNode = new ReactiveProperty<SaveData.Heroine>();

	private HSceneProc.LightData lightData;

	private ShuffleRand[] voicePlayShuffle = new ShuffleRand[5];

	protected HVoiceCtrl voice;

	private void Start()
	{
		LoadHoushiPoseName();
		axis.guideObj.SetScale();
		resultPeepingNode.Select((SaveData.Heroine node) => node != null).SubscribeToInteractable(peeping.buttonClothChangeYes);
		peeping.buttonClothChangeYes.OnClickAsObservable().Subscribe(delegate
		{
			flags.click = HFlag.ClickKind.peeping_restart;
		});
		moveAxizInteractive.SetInteractive(false);
		for (int i = 0; i < lstMultipleFemaleGroup.Count; i++)
		{
			HSceneSpriteCategory hSceneSpriteCategory = lstMultipleFemaleGroup[i];
			int female = i;
			int count = hSceneSpriteCategory.GetCount();
			for (int j = 0; j < count; j++)
			{
				int num = j;
				hSceneSpriteCategory.SetAction(j, delegate
				{
					OnFemaleDressSubMenu(female, num);
				});
			}
		}
		for (int k = 0; k < voicePlayShuffle.Length; k++)
		{
			voicePlayShuffle[k] = new ShuffleRand();
			voicePlayShuffle[k].Init(2);
		}
	}

	private void Update()
	{
		if (flags.mode != HFlag.EMode.none)
		{
			if ((bool)imageSpeed)
			{
				if (flags.mode == HFlag.EMode.aibu)
				{
					imageSpeed.fillAmount = Mathf.InverseLerp(0f, flags.speedMaxAibuBody, flags.speed);
				}
				else
				{
					imageSpeed.fillAmount = flags.speedCalc;
				}
			}
			SpriteStateProcDelegate[] array = new SpriteStateProcDelegate[8] { AibuProc, HoushiProc, SonyuProc, MasturbationProc, PeepingProc, LesbianProc, Houshi3PProc, Sonyu3PProc };
			array[(int)flags.mode]();
		}
		if (flags.lstHeroine.Count != 0 && flags.lstHeroine[0].hCount == 0)
		{
			bool flag = flags.count.aibuOrg != 0 && flags.count.houshiOutside + flags.count.houshiInside != 0 && (flags.count.sonyuOrg != 0 || flags.count.sonyuOutside + flags.count.sonyuInside + flags.count.sonyuCondomInside != 0);
			if (flags.firstHEasy && (flags.count.sonyuOrg != 0 || flags.count.sonyuOutside + flags.count.sonyuInside + flags.count.sonyuCondomInside != 0))
			{
				flag = true;
			}
			if (flags.mode == HFlag.EMode.masturbation || flags.mode == HFlag.EMode.peeping || flags.mode == HFlag.EMode.lesbian || flags.isFreeH || flags.isDebug)
			{
				flag = true;
			}
			if (btnEnd.gameObject.activeSelf != flag)
			{
				btnEnd.gameObject.SetActive(flag);
			}
		}
		if ((bool)gauge[0].imgBar)
		{
			gauge[0].imgBar.fillAmount = Mathf.InverseLerp(0f, 100f, flags.gaugeFemale);
		}
		if ((bool)gauge[1].imgBar)
		{
			gauge[1].imgBar.fillAmount = Mathf.InverseLerp(0f, 100f, flags.gaugeMale);
		}
		for (int i = 0; i < females.Count; i++)
		{
			int num = 0;
			for (int j = 0; j < 7; j++)
			{
				if (females[i].IsClothesStateKind(j))
				{
					num++;
				}
			}
			if (females[i].IsShoesStateKind())
			{
				num++;
			}
			if (females.Count == 1)
			{
				categoryDress.SetActive(num != 0, 0);
			}
			else if (females.Count > 1)
			{
				lstMultipleFemaleGroup[i].SetActive(num != 0, 0);
			}
			int num2 = 0;
			for (int k = 0; k < 20; k++)
			{
				if (females[i].IsAccessory(k))
				{
					num2++;
				}
			}
			if (females.Count == 1)
			{
				categoryDress.SetActive(num2 != 0, 1);
			}
			else if (females.Count > 1)
			{
				lstMultipleFemaleGroup[i].SetActive(num2 != 0, 1);
			}
		}
		if ((bool)objImmediatelyFinish && objImmediatelyFinish.activeSelf != Manager.Config.AddData.immediatelyFinish)
		{
			objImmediatelyFinish.SetActive(Manager.Config.AddData.immediatelyFinish);
		}
		FadeProc();
	}

	public bool IsCursorOnPad()
	{
		return selectUIPads.Any((SelectUI s) => s.isSelect);
	}

	public void CloseAllMenu()
	{
		for (int i = 0; i < menuSub.GetCount(); i++)
		{
			if (menuSub.GetActive(i))
			{
				menuSub.SetActive(false, i);
			}
		}
		GameObject[] array = objMenuSubSubs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		foreach (HSceneSpriteCategory item in lstMultipleFemaleGroup)
		{
			item.gameObject.SetActive(false);
		}
		foreach (MultipleFemaleSubGroup item2 in lstMultipleFemaleSubGroup)
		{
			foreach (GameObject item3 in item2.lstGroup)
			{
				item3.SetActive(false);
			}
		}
	}

	public void CloseMotionSubMenu()
	{
		menuSub.SetActive(false, 1);
		menuSub.SetActive(false, 2);
		menuSub.SetActive(false, 3);
		menuSub.SetActive(false, 4);
		menuSub.SetActive(false, 5);
	}

	public void ForceCloseAllMenu()
	{
		for (int i = 0; i < menuSub.GetCount(); i++)
		{
			menuSub.SetActive(false);
		}
		GameObject[] array = objMenuSubSubs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		foreach (HSceneSpriteCategory item in lstMultipleFemaleGroup)
		{
			item.gameObject.SetActive(false);
		}
		foreach (MultipleFemaleSubGroup item2 in lstMultipleFemaleSubGroup)
		{
			foreach (GameObject item3 in item2.lstGroup)
			{
				item3.SetActive(false);
			}
		}
	}

	public void OnSpeedUpClick()
	{
		if (!IsSpriteAciotn())
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			flags.click = HFlag.ClickKind.speedup;
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			if ((flags.mode == HFlag.EMode.houshi || flags.mode == HFlag.EMode.houshi3P) && animatorStateInfo.IsName("Idle"))
			{
				flags.voiceWait = true;
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			flags.click = HFlag.ClickKind.motionchange;
		}
		if (Input.GetMouseButtonDown(2))
		{
			flags.click = HFlag.ClickKind.modeChange;
		}
	}

	public void OnInsertClick()
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		int num = 0;
		int num2 = ((flags.mode >= HFlag.EMode.houshi3P) ? (flags.nowAnimationInfo.id % 2) : 0);
		num = ((flags.mode == HFlag.EMode.sonyu3P) ? ((!flags.nowAnimationInfo.isFemaleInitiative) ? 500 : 538) : ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0));
		if (flags.isInsertOK[num2] || flags.isDebug)
		{
			flags.click = HFlag.ClickKind.insert;
			flags.voice.playVoices[num2] = 301 + num;
			Utils.Sound.Play(SystemSE.sel);
		}
		else if (flags.isCondom)
		{
			flags.click = HFlag.ClickKind.insert;
			flags.voice.playVoices[num2] = 301 + num;
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			flags.AddNotCondomPlay();
			flags.voice.playVoices[num2] = 302 + num;
			flags.voice.SetSonyuIdleTime();
			flags.isDenialvoiceWait = true;
		}
		if (flags.mode >= HFlag.EMode.houshi3P)
		{
			int num3 = num2 ^ 1;
			if (voice.nowVoices[num3].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num3]))
			{
				Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3]);
			}
		}
	}

	public void OnInsertNoVoiceClick()
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		int num = ((flags.mode >= HFlag.EMode.houshi3P) ? (flags.nowAnimationInfo.id % 2) : 0);
		if (flags.isInsertOK[num] || flags.isDebug)
		{
			flags.click = HFlag.ClickKind.insert_voice;
			Utils.Sound.Play(SystemSE.sel);
			return;
		}
		if (flags.isCondom)
		{
			flags.click = HFlag.ClickKind.insert_voice;
			Utils.Sound.Play(SystemSE.sel);
			return;
		}
		flags.AddNotCondomPlay();
		int num2 = 0;
		num2 = ((flags.mode == HFlag.EMode.sonyu3P) ? ((!flags.nowAnimationInfo.isFemaleInitiative) ? 500 : 538) : ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0));
		flags.voice.playVoices[num] = 302 + num2;
		flags.voice.SetSonyuIdleTime();
		flags.isDenialvoiceWait = true;
		if (flags.mode >= HFlag.EMode.houshi3P)
		{
			int num3 = num ^ 1;
			if (voice.nowVoices[num3].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num3]))
			{
				Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3]);
			}
		}
	}

	public void OnInsertAnalClick()
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		int num = ((flags.mode >= HFlag.EMode.houshi3P) ? (flags.nowAnimationInfo.id % 2) : 0);
		int num2 = 0;
		num2 = ((flags.mode == HFlag.EMode.sonyu3P) ? ((!flags.nowAnimationInfo.isFemaleInitiative) ? 500 : 538) : ((Game.isAdd20 && flags.nowAnimationInfo.isFemaleInitiative) ? 38 : 0));
		if (flags.isAnalInsertOK || flags.isDebug)
		{
			flags.click = HFlag.ClickKind.insert_anal;
			flags.voice.playVoices[num] = 304 + num2;
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			flags.AddNotAnalPlay();
			flags.voice.playVoices[num] = 305 + num2;
			flags.voice.SetSonyuIdleTime();
		}
		if (flags.mode >= HFlag.EMode.houshi3P)
		{
			int num3 = num ^ 1;
			if (voice.nowVoices[num3].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num3]))
			{
				Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3]);
			}
		}
	}

	public void OnInsertAnalNoVoiceClick()
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		if (flags.isAnalInsertOK || flags.isDebug)
		{
			flags.click = HFlag.ClickKind.insert_anal_voice;
			Utils.Sound.Play(SystemSE.sel);
			return;
		}
		flags.AddNotAnalPlay();
		int num = ((flags.mode >= HFlag.EMode.houshi3P) ? (flags.nowAnimationInfo.id % 2) : 0);
		int num2 = 0;
		num2 = ((flags.mode == HFlag.EMode.sonyu3P) ? ((!flags.nowAnimationInfo.isFemaleInitiative) ? 500 : 538) : (flags.nowAnimationInfo.isFemaleInitiative ? 38 : 0));
		flags.voice.playVoices[num] = 305 + num2;
		flags.voice.SetSonyuIdleTime();
		if (flags.mode >= HFlag.EMode.houshi3P)
		{
			int num3 = num ^ 1;
			if (voice.nowVoices[num3].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num3]))
			{
				Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num3]);
			}
		}
	}

	public void OnPullClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.pull;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnAutoFinish()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			AutoFinishDelegate[] array = new AutoFinishDelegate[8] { SetAibuAutoFinish, SetHoushiAutoFinish, null, null, null, null, SetHoushi3PAutoFinish, null };
			if (array[(int)flags.mode] != null)
			{
				array[(int)flags.mode]();
			}
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnCondomClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			CondomClick();
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void CondomClick()
	{
		if (flags.mode == HFlag.EMode.sonyu)
		{
			SetCondom(!sonyu.isCondom);
			flags.isCondom = sonyu.isCondom;
		}
		else
		{
			SetCondom3P(!sonyu3P.isCondom);
			flags.isCondom = sonyu3P.isCondom;
		}
		if ((bool)male && male.chaFile != null)
		{
			male.chaFile.status.visibleGomu = flags.isCondom;
		}
	}

	public void OnInsideClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.inside;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnOutsideClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.outside;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnFastMouseDown()
	{
		if (Input.GetMouseButtonDown(0) && IsSpriteAciotn())
		{
			flags.speedHoushi = 1;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnSlowMouseDown()
	{
		if (Input.GetMouseButtonDown(0) && IsSpriteAciotn())
		{
			flags.speedHoushi = 2;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnSpeedMouseUp()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.speedHoushi = 0;
		}
	}

	public void OnRelyClick(bool _isON)
	{
		if (IsSpriteAciotn())
		{
			flags.rely = _isON;
			if (!_isON)
			{
				rely.InitTimer();
			}
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnDrinkClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.drink;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnRePlayClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.again;
			flags.voiceWait = true;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnVomitClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.vomit;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnFemaleGaugeLock(bool _isON)
	{
		if (IsSpriteAciotn())
		{
			flags.lockGugeFemale = _isON;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnFemaleGaugeLockOnGauge()
	{
		gauge[0].tglLock.isOn = !gauge[0].tglLock.isOn;
	}

	public void OnMaleGaugeLock(bool _isON)
	{
		if (IsSpriteAciotn())
		{
			flags.lockGugeMale = _isON;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnMaleGaugeLockOnGauge()
	{
		gauge[1].tglLock.isOn = !gauge[1].tglLock.isOn;
	}

	public void OnClickPeepingRestart()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.peeping_restart;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnMainMenu(int _kind)
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		_kind = ((_kind != 0 || (flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.houshi3P && flags.mode != HFlag.EMode.sonyu3P)) ? _kind : 4);
		if (!menuSub.IsEmpty(_kind))
		{
			if (menuSub.GetActive(_kind))
			{
				menuSub.SetActive(false);
			}
			else
			{
				menuSub.SetActiveToggle(_kind);
				MainMenuProc(_kind);
				Utils.Sound.Play(SystemSE.sel);
			}
		}
		else
		{
			MainMenuProc(_kind);
		}
		GameObject[] array = objMenuSubSubs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		foreach (HSceneSpriteCategory item in lstMultipleFemaleGroup)
		{
			item.gameObject.SetActive(false);
		}
		foreach (MultipleFemaleSubGroup item2 in lstMultipleFemaleSubGroup)
		{
			foreach (GameObject item3 in item2.lstGroup)
			{
				item3.SetActive(false);
			}
		}
	}

	public void OnActionMenu(int _kind)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			CreateMotionList(_kind);
		}
	}

	public void OnActionHoushiMenuHand(bool _isON)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			OnActionHoushiMenu(_isON, 0);
		}
	}

	public void OnActionHoushiMenuMouth(bool _isON)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			OnActionHoushiMenu(_isON, 1);
		}
	}

	public void OnActionHoushiMenuBreast(bool _isON)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			OnActionHoushiMenu(_isON, 2);
		}
	}

	public void OnActionHoushiMenu(bool _isON, int _kind)
	{
		if (!menuActionSub.GetActive(_kind + 1) && _isON)
		{
			for (int i = 1; i < 4; i++)
			{
				if (!menuActionSub.IsEmpty(i))
				{
					menuActionSub.SetActive(_kind + 1 == i, i);
				}
			}
			LoadMotionList(lstHoushiAnimInfo[_kind], menuActionSub.GetObject(_kind + 1));
			GameObject @object = menuActionSub.GetObject(4);
			GameObject object2 = categoryToggleHoushi.GetObject(_kind);
			RectTransform rectTransform = @object.transform as RectTransform;
			RectTransform rectTransform2 = object2.transform as RectTransform;
			object2 = menuActionSub.GetObject(_kind + 1);
			RectTransform rectTransform3 = object2.transform as RectTransform;
			Vector2 anchoredPosition = rectTransform3.anchoredPosition;
			anchoredPosition.y = rectTransform2.anchoredPosition.y + rectTransform.anchoredPosition.y;
			rectTransform3.anchoredPosition = anchoredPosition;
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			menuActionSub.SetActive(false, _kind + 1);
		}
	}

	public void OnSubMenu(int _kind)
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		Animator component = objMenuSubSubs[_kind].GetComponent<Animator>();
		if (objMenuSubSubs[_kind].activeSelf)
		{
			component.SetBool("on", false);
			MenuStateMachineBehaviour behaviour = component.GetBehaviour<MenuStateMachineBehaviour>();
			behaviour.obj = objMenuSubSubs[_kind];
			return;
		}
		objMenuSubSubs[_kind].SetActive(true);
		for (int i = 0; i < objMenuSubSubs.Length; i++)
		{
			if (i != _kind)
			{
				objMenuSubSubs[i].SetActive(false);
			}
		}
		component.SetBool("on", true);
		SubMenuProc(_kind);
		Utils.Sound.Play(SystemSE.sel);
	}

	public void OnClickCloth(int _cloth)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].SetClothesStateNext(_cloth);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllCloth(int _cloth)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].SetClothesStateAll((byte)_cloth);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAccessory(int _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].chaFile.status.showAccessory[_accessory] = !females[0].chaFile.status.showAccessory[_accessory];
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessory(bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].SetAccessoryStateAll(_accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessoryGroup1(bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].SetAccessoryStateCategory(0, _accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessoryGroup2(bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].SetAccessoryStateCategory(1, _accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickCoordinateChange(int _coordinate)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[0].ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)_coordinate);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClothChange(int _female)
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		clothCusutomCtrl.chaCtrl = females[_female];
		clothCusutomCtrl.gameObject.SetActive(true);
		Utils.Sound.Play(SystemSE.window_o);
		GameObject[] array = objMenuSubSubs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(false);
		}
		foreach (HSceneSpriteCategory item in lstMultipleFemaleGroup)
		{
			item.gameObject.SetActive(false);
		}
		foreach (MultipleFemaleSubGroup item2 in lstMultipleFemaleSubGroup)
		{
			foreach (GameObject item3 in item2.lstGroup)
			{
				item3.SetActive(false);
			}
		}
	}

	public void OnFemaleClick(int _female)
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		if (lstMultipleFemaleGroup[_female].gameObject.activeSelf)
		{
			lstMultipleFemaleGroup[_female].gameObject.SetActive(false);
		}
		else
		{
			for (int i = 0; i < lstMultipleFemaleGroup.Count; i++)
			{
				lstMultipleFemaleGroup[i].gameObject.SetActive(i == _female);
			}
			Utils.Sound.Play(SystemSE.sel);
		}
		foreach (MultipleFemaleSubGroup item in lstMultipleFemaleSubGroup)
		{
			foreach (GameObject item2 in item.lstGroup)
			{
				item2.SetActive(false);
			}
		}
	}

	public void OnFemaleDressSubMenu(int _female, int _kind)
	{
		if (!Input.GetMouseButtonUp(0) || !IsSpriteAciotn())
		{
			return;
		}
		Animator component = lstMultipleFemaleSubGroup[_female].lstGroup[_kind].GetComponent<Animator>();
		if (lstMultipleFemaleSubGroup[_female].lstGroup[_kind].activeSelf)
		{
			component.SetBool("on", false);
			MenuStateMachineBehaviour behaviour = component.GetBehaviour<MenuStateMachineBehaviour>();
			behaviour.obj = lstMultipleFemaleSubGroup[_female].lstGroup[_kind];
			return;
		}
		for (int i = 0; i < lstMultipleFemaleSubGroup[_female].lstGroup.Count; i++)
		{
			lstMultipleFemaleSubGroup[_female].lstGroup[i].SetActive(i == _kind);
		}
		component.SetBool("on", true);
		FemaleDressSubMenuProc(_female, _kind);
		Utils.Sound.Play(SystemSE.sel);
	}

	public void OnClickCloth(int _female, int _cloth)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].SetClothesStateNext(_cloth);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllCloth(int _female, int _cloth)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].SetClothesStateAll((byte)_cloth);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAccessory(int _female, int _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].chaFile.status.showAccessory[_accessory] = !females[_female].chaFile.status.showAccessory[_accessory];
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessory(int _female, bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].SetAccessoryStateAll(_accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessoryGroup1(int _female, bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].SetAccessoryStateCategory(0, _accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickAllAccessoryGroup2(int _female, bool _accessory)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].SetAccessoryStateCategory(1, _accessory);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickCoordinateChange(int _female, int _coordinate)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			females[_female].ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)_coordinate);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnClickHSceneEnd()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			Utils.Sound.Play(SystemSE.cancel);
			flags.click = HFlag.ClickKind.end;
			flags.isHSceneEnd = true;
			flags.numEnd = 0;
		}
	}

	public void OnClickTrespassing()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			Utils.Sound.Play(SystemSE.sel);
			flags.click = HFlag.ClickKind.end;
			flags.isHSceneEnd = true;
			flags.numEnd = 2;
			if (flags.mode == HFlag.EMode.masturbation && flags.isMasturbationFound)
			{
				flags.lstHeroine[0].lewdness = 100;
			}
			ActionScene actScene = Singleton<Game>.Instance.actScene;
			if ((bool)actScene)
			{
				actScene.isPenetration = true;
			}
		}
	}

	public void OnClickConfig()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			Utils.Sound.Play(SystemSE.ok_s);
			Singleton<Scene>.Instance.LoadReserve(new Scene.Data
			{
				levelName = "Config",
				isAdd = true
			}, false);
		}
	}

	public void OnValueRChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			Color color = lightData.light.color;
			color.r = _value;
			lightData.light.color = color;
		}
	}

	public void OnValueGChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			Color color = lightData.light.color;
			color.g = _value;
			lightData.light.color = color;
		}
	}

	public void OnValueBChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			Color color = lightData.light.color;
			color.b = _value;
			lightData.light.color = color;
		}
	}

	public void OnClickLightColorInit(int _num)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			Color color = lightData.light.color;
			switch (_num)
			{
			case 0:
				lightSlider.r.value = lightData.initColor.r;
				color.r = lightData.initColor.r;
				break;
			case 1:
				lightSlider.g.value = lightData.initColor.g;
				color.g = lightData.initColor.g;
				break;
			case 2:
				lightSlider.b.value = lightData.initColor.b;
				color.b = lightData.initColor.b;
				break;
			case 3:
				lightData.light.intensity = lightData.initIntensity;
				lightSlider.a.value = lightData.initIntensity;
				color.a = lightData.initIntensity;
				break;
			}
			lightData.light.color = color;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void LightColorAllInit()
	{
		lightData.light.color = lightData.light.color;
	}

	public void OnValueVerticalChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			lightData.calcRot.x = _value;
			float x = _value * 360f + lightData.initRot.x;
			float y = lightData.calcRot.y * 360f + lightData.initRot.y;
			lightData.light.transform.localRotation = Quaternion.Euler(x, y, 0f);
		}
	}

	public void OnValueHorizonChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			lightData.calcRot.y = _value;
			float x = lightData.calcRot.x * 360f + lightData.initRot.x;
			float y = _value * 360f + lightData.initRot.y;
			lightData.light.transform.localRotation = Quaternion.Euler(x, y, 0f);
		}
	}

	public void OnValuePowerChanged(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			lightData.light.intensity = _value;
		}
	}

	public void OnClickLightDirInit(int _num)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			if (_num == 0)
			{
				lightData.calcRot.x = 0f;
				lightData.light.transform.localRotation = Quaternion.Euler(lightData.initRot.x, lightSlider.y.value * 360f, 0f);
				lightSlider.x.value = 0f;
			}
			else
			{
				lightData.calcRot.y = 0f;
				lightData.light.transform.localRotation = Quaternion.Euler(lightSlider.x.value * 360f, lightData.initRot.y, 0f);
				lightSlider.y.value = 0f;
			}
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void LightDirInit()
	{
		lightData.calcRot = Vector2.zero;
		lightData.light.transform.localRotation = Quaternion.Euler(lightData.initRot.x, lightData.initRot.y, 0f);
	}

	public void OnClickMoveAxisDraw(bool _isOn)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			axis.guideObj.gameObject.SetActive(_isOn);
			moveAxizInteractive.SetInteractive(_isOn);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void MoveAxisDraw(bool _isOn)
	{
		axis.guideObj.gameObject.SetActive(_isOn);
		moveAxizInteractive.SetInteractive(_isOn);
	}

	public void OnClickChangeMoveAxis(int _change)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn() && moveAxizInteractive.lstTgl[_change].interactable)
		{
			axis.guideObj.SetMode(_change);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void ChangeMoveAxis(int _change)
	{
		axis.guideObj.SetMode(_change);
	}

	public void OnClickInitMoveAxis(int _init)
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			localMoveInitAction(_init);
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnValueMoveAxisSpeedChange(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			axis.guideObj.speedMove = _value;
		}
	}

	public void OnClickMoveAxisInitSpeed()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			axis.sliderSpeed.value = 1f;
			axis.guideObj.speedMove = 1f;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnValueMoveAxisScaleChange(float _value)
	{
		if (Input.GetMouseButton(0) && IsSpriteAciotn())
		{
			axis.guideObj.scaleAxis = _value;
			axis.guideObj.SetScale();
		}
	}

	public void OnClickMoveAxisInitScale()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			axis.sliderScale.value = 1.5f;
			axis.guideObj.scaleAxis = 1.5f;
			axis.guideObj.SetScale();
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnMouseDownSlider()
	{
		GlobalMethod.SetCameraMoveFlag(flags.ctrlCamera, false);
	}

	public void OnAibuIconOpenAndClose(bool _open)
	{
		categoryAibuIcon.SetActive(_open);
		objAibuItemIconOpen.SetActive(!_open);
		Utils.Sound.Play(SystemSE.sel);
	}

	public void OnMainMenuOpenAndClose(bool _close)
	{
		objBaseMainMenu.SetActive(!_close);
		objMainMenuOpen.SetActive(_close);
		if (_close)
		{
			CloseAllMenu();
		}
		Utils.Sound.Play(SystemSE.sel);
	}

	public void OnClickHelp()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			if ((bool)autoDisableFirstHHelp)
			{
				autoDisableFirstHHelp.FadeStart();
			}
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnDetachItemClick(int _num)
	{
		if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		if (_num != 3)
		{
			int num = hand.DetachItemByUseItem(_num);
			if (num != -1)
			{
				flags.click = (HFlag.ClickKind)(num + 22);
			}
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			hand.DetachAllItem();
			flags.click = HFlag.ClickKind.de_muneL;
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnImmediatelyFinishMale()
	{
		if (Input.GetMouseButtonUp(0))
		{
			bool flag = false;
			if (flags.mode == HFlag.EMode.houshi || flags.mode == HFlag.EMode.houshi3P)
			{
				flag = true;
			}
			else if (flags.mode == HFlag.EMode.sonyu || flags.mode == HFlag.EMode.sonyu3P)
			{
				flag = true;
			}
			if (flags.gaugeMale < 70f && flag && !flags.lockGugeMale)
			{
				flags.gaugeMale = 70f;
			}
			Utils.Sound.Play(SystemSE.sel);
			flags.isUseImmediatelyFinishButton = true;
		}
	}

	public void OnImmediatelyFinishFemale()
	{
		if (Input.GetMouseButtonUp(0))
		{
			bool flag = false;
			flag = flags.mode != HFlag.EMode.sonyu || true;
			if (flags.gaugeFemale < 70f && flag && !flags.lockGugeFemale)
			{
				flags.gaugeFemale = 70f;
			}
			Utils.Sound.Play(SystemSE.sel);
			flags.isUseImmediatelyFinishButton = true;
		}
	}

	public void OnTrespassingHelp()
	{
		autoDisableTrespassingHelp.FadeStart();
	}

	public void OnChangeMotionClick()
	{
		if (Input.GetMouseButtonUp(0) && IsSpriteAciotn())
		{
			flags.click = HFlag.ClickKind.motionchange;
		}
	}

	public void OnOrgWClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.orgW;
		}
	}

	public void OnSameWClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.sameW;
		}
	}

	public void OnOrgSClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.orgS;
		}
	}

	public void OnSameSClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.sameS;
		}
	}

	public void OnIdleClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.Idle;
		}
	}

	public void OnStopIdleClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.stop_Idle;
		}
	}

	public void OnLoopClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.insert;
		}
	}

	public void OnOLoopClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.OLoop;
		}
	}

	public void OnFrontDislikesClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.Front_Dislikes;
		}
	}

	public void OnBackDislikesClick()
	{
		if (Input.GetMouseButtonUp(0))
		{
			flags.click = HFlag.ClickKind.Back_Dislikes;
		}
	}

	public void NickNamePlay()
	{
		SaveData.CallFileData callFileData = SaveData.FindCallFileData(flags.lstHeroine[0].personality, 0);
		Utils.Voice.Setting setting = new Utils.Voice.Setting();
		setting.no = flags.lstHeroine[0].voiceNo;
		setting.assetBundleName = callFileData.bundle;
		setting.assetName = callFileData.GetFileName(2);
		setting.pitch = flags.lstHeroine[0].voicePitch;
		setting.voiceTrans = flags.transVoiceMouth[0];
		Utils.Voice.Setting s = setting;
		females[0].SetVoiceTransform(Utils.Voice.OnecePlayChara(s));
	}

	private void LoadHoushiPoseName()
	{
		string text = GlobalMethod.LoadAllListText("h/list/", "houshi_pose_name");
		string[,] data;
		GlobalMethod.GetListString(text, out data);
		int length = data.GetLength(0);
		for (int i = 0; i < length; i++)
		{
			int key = int.Parse(data[i, 0]);
			string value = data[i, 1];
			if (!dicHoushiPoseName.ContainsKey(key))
			{
				dicHoushiPoseName.Add(key, value);
			}
			else
			{
				dicHoushiPoseName[key] = value;
			}
		}
	}

	public void Init(List<ChaControl> _female, SaveData.Heroine _heroine, ChaControl _male, AutoRely _rely, List<HSceneProc.AnimationListInfo>[] _lstAnimation, List<int> _category, HVoiceCtrl _voice)
	{
		females = _female;
		heroine = _heroine;
		male = _male;
		rely = _rely;
		lstUseAnimInfo = _lstAnimation;
		voice = _voice;
		bool flag = _category.Any((int c) => MathfEx.IsRange(3000, c, 3999, true));
		int[] array = new int[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = lstUseAnimInfo[i].Count;
			bool active = array[i] >= 1;
			menuAction.SetActive(active, i);
		}
		if (flag)
		{
			menuAction.SetActive(false, 0);
			for (int j = 1; j < 3; j++)
			{
				array[j] = lstUseAnimInfo[j + 5].Count;
				bool active2 = array[j] >= 1;
				menuAction.SetActive(active2, j);
			}
		}
		menuAction.gameObject.SetActive(array.Count((int c) => c >= 1) >= 2);
		lstHoushiAnimInfo[0] = new List<HSceneProc.AnimationListInfo>();
		lstHoushiAnimInfo[1] = new List<HSceneProc.AnimationListInfo>();
		lstHoushiAnimInfo[2] = new List<HSceneProc.AnimationListInfo>();
		int k;
		for (k = 0; k < lstHoushiAnimInfo.Length; k++)
		{
			lstHoushiAnimInfo[k] = lstUseAnimInfo[1].Where((HSceneProc.AnimationListInfo l) => l.kindHoushi == k).ToList();
		}
		lst3PHoushiAnimInfo[0] = new List<HSceneProc.AnimationListInfo>();
		lst3PHoushiAnimInfo[1] = new List<HSceneProc.AnimationListInfo>();
		lst3PHoushiAnimInfo[2] = new List<HSceneProc.AnimationListInfo>();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (HSceneProc.AnimationListInfo item in lstUseAnimInfo[1])
		{
			hashSet.Add(item.posture);
		}
		lstHoushiPose = new List<int>(hashSet);
		if (lstHoushiPose.Count != 0)
		{
			flags.poseHoushi = lstHoushiPose[0];
		}
		array = new int[3];
		for (int m = 0; m < 3; m++)
		{
			array[m] = lstHoushiAnimInfo[m].Count;
			bool active3 = array[m] >= 1;
			menuActionSub.SetActive(active3, m + 1);
			categoryToggleHoushi.SetActive(active3, m);
		}
		SetHoushiStart();
		SetSonyuStart();
		clothCusutomCtrl.chaCtrl = females[0];
		categoryMenstruation.SetActiveToggle((HFlag.GetMenstruation(heroine.MenstruationDay) != 0) ? 1 : 0);
		categoryExperience.SetActiveToggle((int)heroine.HExperience);
		Texture2D texture2D = new Texture2D(240, 320);
		texture2D.LoadImage(heroine.charFile.facePngData);
		rawFace.texture = texture2D;
		menuActionSub.SetActive(false);
		CreateActionList();
	}

	public void InitHeroine(List<SaveData.Heroine> _heroines)
	{
		if (flags.mode == HFlag.EMode.houshi3P || flags.mode == HFlag.EMode.sonyu3P)
		{
			int index = flags.nowAnimationInfo.id % 2;
			heroine = _heroines[index];
			clothCusutomCtrl.chaCtrl = females[index];
			categoryMenstruation.SetActiveToggle((HFlag.GetMenstruation(heroine.MenstruationDay) != 0) ? 1 : 0);
			categoryExperience.SetActiveToggle((int)heroine.HExperience);
			Texture2D texture2D = new Texture2D(240, 320);
			texture2D.LoadImage(heroine.charFile.facePngData);
			if ((bool)rawFace.texture)
			{
				UnityEngine.Object.Destroy(rawFace.texture);
			}
			rawFace.texture = texture2D;
		}
	}

	public void InitPointMenuAndHelp(List<int> _category, bool _isFirst = true)
	{
		menuMain.SetEnable(!_category.Any((int c) => c >= 2000) && ((flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian) || flags.isFreeH), 2);
		if ((bool)objFirstHHelpBase && _isFirst)
		{
			objFirstHHelpBase.SetActive(heroine.hCount == 0 && !flags.isFreeH && !_category.Any((int c) => c >= 2000) && flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian && flags.mode != HFlag.EMode.sonyu3P && flags.mode != HFlag.EMode.houshi3P);
		}
		categoryExperience.gameObject.SetActive(flags.mode != HFlag.EMode.lesbian);
		if (!_isFirst)
		{
			return;
		}
		ActionScene actScene = Singleton<Game>.Instance.actScene;
		bool flag = (bool)actScene && !actScene.isPenetration && flags.mode != HFlag.EMode.peeping && !flags.isFreeH && Game.isAdd20;
		if (flag)
		{
			if (flags.mode == HFlag.EMode.masturbation)
			{
				if (flags.lstHeroine[0].isVirgin || flags.lstHeroine[0].isAnger)
				{
					flag = false;
				}
			}
			else if (flags.mode == HFlag.EMode.lesbian)
			{
				if (flags.lstHeroine[0].HExperience < SaveData.Heroine.HExperienceKind.慣れ || flags.lstHeroine[1].HExperience < SaveData.Heroine.HExperienceKind.慣れ || flags.lstHeroine[0].isAnger || flags.lstHeroine[1].isAnger)
				{
					flag = false;
				}
			}
			else if ((flags.mode == HFlag.EMode.aibu || flags.mode == HFlag.EMode.houshi || flags.mode == HFlag.EMode.sonyu) && (_category.Any((int c) => c >= 1000) || flags.newHeroine == null || flags.lstHeroine[0].HExperience < SaveData.Heroine.HExperienceKind.慣れ))
			{
				flag = false;
			}
		}
		if ((bool)btnTrespassing)
		{
			btnTrespassing.gameObject.SetActive(flag);
		}
	}

	public void InitSpriteButton()
	{
		for (int i = 0; i < lstMultipleFemaleDressButton.Count; i++)
		{
			HSceneSpriteCategory dress = lstMultipleFemaleDressButton[i].dress;
			int female = i;
			int count = dress.GetCount();
			for (int j = 0; j < count; j++)
			{
				int cloth = j;
				dress.SetAction(j, delegate
				{
					OnClickCloth(female, cloth);
				});
			}
			dress = lstMultipleFemaleDressButton[i].dressAll;
			count = dress.GetCount();
			for (int k = 0; k < count; k++)
			{
				int num = k;
				int[] cloths = new int[3] { 0, 1, 3 };
				dress.SetAction(k, delegate
				{
					OnClickAllCloth(female, cloths[num]);
				});
			}
			dress = lstMultipleFemaleDressButton[i].accessory;
			count = dress.GetCount();
			for (int l = 0; l < count; l++)
			{
				int cloth2 = l;
				dress.SetAction(l, delegate
				{
					OnClickAccessory(female, cloth2);
				});
			}
			dress = lstMultipleFemaleDressButton[i].accessoryAll;
			dress.SetAction(0, delegate
			{
				OnClickAllAccessory(female, true);
			});
			dress.SetAction(1, delegate
			{
				OnClickAllAccessory(female, false);
			});
			dress.SetAction(2, delegate
			{
				OnClickAllAccessoryGroup1(female, true);
			});
			dress.SetAction(3, delegate
			{
				OnClickAllAccessoryGroup1(female, false);
			});
			dress.SetAction(4, delegate
			{
				OnClickAllAccessoryGroup2(female, true);
			});
			dress.SetAction(5, delegate
			{
				OnClickAllAccessoryGroup2(female, false);
			});
			dress = lstMultipleFemaleDressButton[i].coordinate;
			count = dress.GetCount();
			for (int m = 0; m < count; m++)
			{
				int cloth3 = m;
				dress.SetAction(m, delegate
				{
					OnClickCoordinateChange(female, cloth3);
				});
			}
		}
	}

	public void InitTrespassingHelp()
	{
		int num = -1;
		if (flags.mode == HFlag.EMode.masturbation)
		{
			num = 0;
		}
		else if (flags.mode == HFlag.EMode.lesbian)
		{
			num = 1;
		}
		else if (btnTrespassing.gameObject.activeSelf)
		{
			num = 2;
		}
		if (num != -1)
		{
			helpSpriteTrespassing.OnChangeValue(num);
			helpTextTrespassing.OnChangeValue(num);
			autoDisableTrespassingHelp.FadeStart();
		}
	}

	public void SetLightData(HSceneProc.LightData _lightdata)
	{
		lightData = _lightdata;
		if (lightData != null)
		{
			lightSlider.r.value = lightData.initColor.r;
			lightSlider.g.value = lightData.initColor.g;
			lightSlider.b.value = lightData.initColor.b;
			lightSlider.a.value = lightData.initIntensity;
			lightSlider.x.value = 0f;
			lightSlider.y.value = 0f;
		}
	}

	public void InitCategoryActionToggle()
	{
	}

	public void CreatePeepingFemaleList(List<SaveData.Heroine> _listHeroine)
	{
		foreach (SaveData.Heroine item in _listHeroine)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(peeping.objPeepingFemaleListNameNode);
			gameObject.transform.SetParent(peeping.objPeepingFemaleListContentParent.transform, false);
			gameObject.SetActive(true);
			HSpritePeepingNode nodeComp = gameObject.GetComponent<HSpritePeepingNode>();
			nodeComp.heroine = item;
			nodeComp.text.text = item.Name;
			nodeComp.tgl.OnValueChangedAsObservable().TakeUntilDestroy(gameObject).Subscribe(delegate(bool isOn)
			{
				if (isOn)
				{
					resultPeepingNode.Value = nodeComp.heroine;
				}
			});
		}
	}

	public SaveData.Heroine GetPeepingFemale()
	{
		return resultPeepingNode.Value;
	}

	public void MainSpriteChange(HSceneProc.AnimationListInfo _info)
	{
		SpriteBase[] array = new SpriteBase[8] { aibu, houshi, sonyu, masturbation, peeping, null, houshi3P, sonyu3P };
		SpriteBase[] array2 = array;
		foreach (SpriteBase spriteBase in array2)
		{
			if (spriteBase != null && (bool)spriteBase.mainObj)
			{
				spriteBase.mainObj.SetActive(spriteBase.id == (int)_info.mode);
			}
		}
		bool active = _info.mode != HFlag.EMode.peeping && _info.mode != HFlag.EMode.masturbation && _info.mode != HFlag.EMode.lesbian;
		objSpeed.SetActive(active);
		menuAction.gameObject.SetActive(active);
		objSpeedAibuBase.SetActive(_info.mode == HFlag.EMode.aibu);
		RectTransform rectTransform = objSpeed.transform as RectTransform;
		Vector2 anchoredPosition = rectTransform.anchoredPosition;
		anchoredPosition.x = ((_info.mode == HFlag.EMode.aibu) ? 200 : 0);
		rectTransform.anchoredPosition = anchoredPosition;
		SpriteStateProcDelegate[] array3 = new SpriteStateProcDelegate[8] { SetAibuStart, SetHoushiStart, SetSonyuStart, SetMasturbationStart, SetPeepingStart, SetLesbianStart, SetHoushi3PStart, SetSonyu3PStart };
		array3[(int)_info.mode]();
	}

	private void MainMenuProc(int _kind)
	{
		if (_kind == 2)
		{
			flags.click = HFlag.ClickKind.pointmove;
		}
	}

	public void CreateActionList()
	{
		if (flags.lstHeroine[0].hCount == 0 && !flags.isFreeH && !flags.firstHEasy)
		{
			menuAction.SetActive(flags.count.aibuOrg != 0 || flags.isDebug, 1);
			menuAction.SetActive(flags.count.houshiInside + flags.count.houshiOutside != 0 || flags.isDebug, 2);
		}
	}

	private void CreateMotionList(int _kind)
	{
		bool flag = flags.mode == HFlag.EMode.houshi3P || flags.mode == HFlag.EMode.sonyu3P;
		if (_kind != 1)
		{
			int num = ((_kind != 0) ? 5 : 0);
			if (!menuActionSub.GetActive(num))
			{
				menuActionSub.SetActiveToggle(num);
				LoadMotionList(lstUseAnimInfo[(_kind != 0) ? ((!flag) ? 2 : 7) : 0], menuActionSub.GetObject(num));
				GameObject @object = menuAction.GetObject(_kind);
				RectTransform rectTransform = @object.transform as RectTransform;
				@object = menuActionSub.GetObject(num);
				RectTransform rectTransform2 = @object.transform as RectTransform;
				Vector2 anchoredPosition = rectTransform2.anchoredPosition;
				anchoredPosition.y = rectTransform.anchoredPosition.y;
				rectTransform2.anchoredPosition = anchoredPosition;
				Utils.Sound.Play(SystemSE.sel);
			}
			else
			{
				menuActionSub.SetActive(false, num);
			}
		}
		else if (flag)
		{
			if (!menuActionSub.GetActive(6))
			{
				menuActionSub.SetActiveToggle(6);
				LoadMotionList(lstUseAnimInfo[6], menuActionSub.GetObject(6));
				GameObject object2 = menuAction.GetObject(_kind);
				RectTransform rectTransform3 = object2.transform as RectTransform;
				object2 = menuActionSub.GetObject(6);
				RectTransform rectTransform4 = object2.transform as RectTransform;
				Vector2 anchoredPosition2 = rectTransform4.anchoredPosition;
				anchoredPosition2.y = rectTransform3.anchoredPosition.y;
				rectTransform4.anchoredPosition = anchoredPosition2;
				Utils.Sound.Play(SystemSE.sel);
			}
			else
			{
				menuActionSub.SetActive(false, 6);
			}
		}
		else if (!menuActionSub.GetActive(4))
		{
			menuActionSub.SetActiveToggle(4);
			GameObject object3 = menuAction.GetObject(_kind);
			RectTransform rectTransform5 = object3.transform as RectTransform;
			object3 = menuActionSub.GetObject(4);
			RectTransform rectTransform6 = object3.transform as RectTransform;
			Vector2 anchoredPosition3 = rectTransform6.anchoredPosition;
			anchoredPosition3.y = rectTransform5.anchoredPosition.y;
			rectTransform6.anchoredPosition = anchoredPosition3;
			categoryToggleHoushi.SetToggleAll(false);
			Utils.Sound.Play(SystemSE.sel);
		}
		else
		{
			for (int i = 1; i < 6; i++)
			{
				menuActionSub.SetActive(false, i);
			}
		}
	}

	private void SubMenuProc(int _kind)
	{
		Action[] array = new Action[3] { ClothProc, AccessoryProc, CoordinateProc };
		array[_kind]();
	}

	private void ClothProc()
	{
		for (int i = 0; i < 7; i++)
		{
			bool active = females[0].IsClothesStateKind(i);
			categoryCloth.SetActive(active, i);
		}
		categoryCloth.SetActive(females[0].IsShoesStateKind(), 7);
	}

	private void AccessoryProc()
	{
		for (int i = 0; i < 20; i++)
		{
			bool flag = females[0].IsAccessory(i);
			categoryAccessory.SetActive(flag, i);
			if (flag)
			{
				ListInfoComponent component = females[0].objAccessory[i].GetComponent<ListInfoComponent>();
				GameObject @object = categoryAccessory.GetObject(i);
				TextMeshProUGUI textMeshProUGUI = null;
				if ((bool)@object)
				{
					textMeshProUGUI = @object.GetComponentInChildren<TextMeshProUGUI>(true);
				}
				if ((bool)component && (bool)textMeshProUGUI)
				{
					textMeshProUGUI.text = component.data.Name;
				}
			}
		}
		bool active = females[0].GetAccessoryCategoryCount(0) == 0 || females[0].GetAccessoryCategoryCount(1) == 0;
		categoryAccessoryAll.SetActive(active, 2);
	}

	private void LightProc()
	{
	}

	private void MoveProc()
	{
	}

	private void CoordinateProc()
	{
	}

	private bool LoadMotionList(List<HSceneProc.AnimationListInfo> _lstAnimInfo, GameObject _objParent)
	{
		for (int i = 0; i < _objParent.transform.childCount; i++)
		{
			Transform child = _objParent.transform.GetChild(i);
			UnityEngine.Object.Destroy(child.gameObject);
		}
		ToggleGroup component = _objParent.GetComponent<ToggleGroup>();
		if (_lstAnimInfo == null)
		{
			return true;
		}
		for (int j = 0; j < _lstAnimInfo.Count; j++)
		{
			GameObject objClone = UnityEngine.Object.Instantiate(objMotionListNode);
			AnimationInfoComponent animationInfoComponent = objClone.AddComponent<AnimationInfoComponent>();
			animationInfoComponent.info = _lstAnimInfo[j];
			objClone.transform.SetParent(_objParent.transform, false);
			GameObject gameObject = objClone.transform.FindLoop("TextMeshPro Text");
			if ((bool)gameObject)
			{
				gameObject.GetComponent<TextMeshProUGUI>().text = animationInfoComponent.info.nameAnimation;
			}
			Toggle component2 = objClone.GetComponent<Toggle>();
			if ((bool)component2 && (bool)component)
			{
				component2.group = component;
				component2.enabled = false;
				component2.enabled = true;
			}
			GameObject gameObject2 = objClone.transform.FindLoop("New");
			if ((bool)gameObject2)
			{
				Dictionary<int, HashSet<int>> playHList = Singleton<Game>.Instance.glSaveData.playHList;
				HashSet<int> value;
				if (!playHList.TryGetValue((int)animationInfoComponent.info.mode, out value))
				{
					value = (playHList[(int)animationInfoComponent.info.mode] = new HashSet<int>());
				}
				gameObject2.SetActive(!value.Contains(animationInfoComponent.info.id));
			}
			objClone.GetComponent<PointerAction>().listClickAction.Add(delegate
			{
				OnChangePlaySelect(objClone);
			});
			objClone.SetActive(true);
			if (_lstAnimInfo[j] == flags.nowAnimationInfo)
			{
				objClone.GetComponent<Toggle>().isOn = true;
			}
		}
		return true;
	}

	public void OnChangePlaySelect(GameObject objClick)
	{
		if (null == objClick)
		{
			return;
		}
		Toggle component = objClick.GetComponent<Toggle>();
		if ((bool)component && !component.interactable)
		{
			return;
		}
		AnimationInfoComponent component2 = objClick.GetComponent<AnimationInfoComponent>();
		if (null == component2)
		{
			GlobalMethod.DebugLog("モーションボタンにモーションクラスが入っていない", 1);
		}
		else
		{
			if (component2.info.mode == flags.nowAnimationInfo.mode && component2.info.id == flags.nowAnimationInfo.id)
			{
				return;
			}
			GameObject gameObject = objClick.transform.FindLoop("New");
			if ((bool)gameObject && gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
			flags.selectAnimationListInfo = component2.info;
			flags.voiceWait = true;
			if (flags.mode != HFlag.EMode.masturbation && flags.mode != HFlag.EMode.lesbian)
			{
				if (flags.mode == flags.selectAnimationListInfo.mode)
				{
					if ((flags.mode == HFlag.EMode.houshi && flags.selectAnimationListInfo.mode == HFlag.EMode.houshi) || (flags.mode == HFlag.EMode.houshi3P && flags.selectAnimationListInfo.mode == HFlag.EMode.houshi3P))
					{
						flags.click = HFlag.ClickKind.insert;
					}
					else
					{
						flags.click = HFlag.ClickKind.actionChange;
					}
					int num = ((component2.info.mode >= HFlag.EMode.houshi3P) ? voicePlayShuffle[1].Get() : 0);
					if (component2.info.mode < HFlag.EMode.houshi3P)
					{
						flags.voice.playVoices[num] = ((component2.info.mode == HFlag.EMode.aibu) ? 6 : ((component2.info.mode != HFlag.EMode.houshi) ? 8 : 7));
					}
					else if (component2.info.mode != HFlag.EMode.none)
					{
						if (component2.info.id % 2 == flags.nowAnimationInfo.id % 2)
						{
							flags.voice.playVoices[num] = ((component2.info.mode != HFlag.EMode.houshi3P) ? 18 : 17);
						}
						else
						{
							flags.voice.playVoices[num] = ((component2.info.mode != HFlag.EMode.houshi3P) ? 20 : 19);
						}
						int num2 = num ^ 1;
						if (voice.nowVoices[num2].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num2]))
						{
							Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num2]);
						}
					}
				}
				else
				{
					flags.click = HFlag.ClickKind.actionChange;
					int[] array = new int[8] { 3, 4, 5, -1, -1, -1, 15, 16 };
					int num3 = ((component2.info.mode >= HFlag.EMode.houshi3P) ? voicePlayShuffle[0].Get() : 0);
					if (component2.info.mode != HFlag.EMode.none)
					{
						flags.voice.playVoices[num3] = array[(int)component2.info.mode];
					}
					if (component2.info.mode < HFlag.EMode.houshi3P)
					{
						flags.voice.playVoices[num3] = array[(int)component2.info.mode];
					}
					else if (component2.info.mode != HFlag.EMode.none)
					{
						if (component2.info.id % 2 == flags.nowAnimationInfo.id % 2)
						{
							flags.voice.playVoices[num3] = ((component2.info.mode != HFlag.EMode.houshi3P) ? 16 : 15);
						}
						else
						{
							flags.voice.playVoices[num3] = ((component2.info.mode != HFlag.EMode.houshi3P) ? 23 : 22);
						}
						int num4 = num3 ^ 1;
						if (voice.nowVoices[num4].state == HVoiceCtrl.VoiceKind.voice && Singleton<Voice>.Instance.IsVoiceCheck(flags.transVoiceMouth[num4]))
						{
							Singleton<Voice>.Instance.Stop(flags.transVoiceMouth[num4]);
						}
					}
				}
			}
			Utils.Sound.Play(SystemSE.sel);
		}
	}

	public void OnChangeHoushiPose(GameObject objClick, int _pose)
	{
		if (null != objClick)
		{
			flags.poseHoushi = _pose;
		}
	}

	public bool IsSpriteAciotn()
	{
		if (Singleton<Scene>.Instance.IsNowLoading || Singleton<Scene>.Instance.IsNowLoadingFade || (Singleton<Scene>.Instance.AddSceneName != string.Empty && Singleton<Scene>.Instance.AddSceneName != "HProc"))
		{
			return false;
		}
		return true;
	}

	private bool SetSubMenuEnable(bool _enable)
	{
		for (int i = 0; i < menuActionSub.GetCount(); i++)
		{
			if (i == 4)
			{
				continue;
			}
			GameObject @object = menuActionSub.GetObject(i);
			for (int j = 0; j < @object.transform.childCount; j++)
			{
				Toggle component = @object.transform.GetChild(j).GetComponent<Toggle>();
				if ((bool)component)
				{
					component.interactable = _enable;
				}
			}
		}
		return true;
	}

	public void SetHelpSprite(int _num)
	{
		if ((bool)autoDisableFirstHHelp)
		{
			autoDisableFirstHHelp.FadeStart();
		}
		if ((bool)helpSpriteChange)
		{
			helpSpriteChange.OnChangeValue(_num);
		}
		if ((bool)helpTextChange)
		{
			helpTextChange.OnChangeValue(_num);
		}
	}

	private void FemaleDressSubMenuProc(int _female, int _kind)
	{
		Action<int>[] array = new Action<int>[3] { FemaleDressSubMenuClothProc, FemaleDressSubMenuAccessoryProc, FemaleDressSubMenuCoordinateProc };
		array[_kind](_female);
	}

	private void FemaleDressSubMenuClothProc(int _female)
	{
		for (int i = 0; i < 7; i++)
		{
			bool active = females[_female].IsClothesStateKind(i);
			lstMultipleFemaleDressButton[_female].dress.SetActive(active, i);
		}
		lstMultipleFemaleDressButton[_female].dress.SetActive(females[_female].IsShoesStateKind(), 7);
	}

	private void FemaleDressSubMenuAccessoryProc(int _female)
	{
		for (int i = 0; i < 20; i++)
		{
			bool flag = females[_female].IsAccessory(i);
			lstMultipleFemaleDressButton[_female].accessory.SetActive(flag, i);
			if (flag)
			{
				ListInfoComponent component = females[_female].objAccessory[i].GetComponent<ListInfoComponent>();
				GameObject @object = lstMultipleFemaleDressButton[_female].accessory.GetObject(i);
				TextMeshProUGUI textMeshProUGUI = null;
				if ((bool)@object)
				{
					textMeshProUGUI = @object.GetComponentInChildren<TextMeshProUGUI>(true);
				}
				if ((bool)component && (bool)textMeshProUGUI)
				{
					textMeshProUGUI.text = component.data.Name;
				}
			}
		}
		bool active = females[_female].GetAccessoryCategoryCount(0) == 0 || females[_female].GetAccessoryCategoryCount(1) == 0;
		lstMultipleFemaleDressButton[_female].accessoryAll.SetActive(active, 2);
	}

	private void FemaleDressSubMenuCoordinateProc(int _female)
	{
	}

	public void FadeState(FadeKind _kind, float _timeFade = -1f)
	{
		isFade = true;
		timeFadeTime = 0f;
		imageFade.enabled = true;
		if (_timeFade < 0f)
		{
			timeFade = ((_kind == FadeKind.OutIn) ? (timeFadeBase * 2f) : timeFadeBase);
		}
		else
		{
			timeFade = ((_kind == FadeKind.OutIn) ? (_timeFade * 2f) : _timeFade);
		}
		kindFade = _kind;
		switch (kindFade)
		{
		case FadeKind.Out:
			kindFadeProc = FadeKindProc.Out;
			break;
		case FadeKind.In:
			kindFadeProc = FadeKindProc.In;
			break;
		case FadeKind.OutIn:
			kindFadeProc = FadeKindProc.OutIn;
			break;
		}
	}

	public FadeKindProc GetFadeKindProc()
	{
		return kindFadeProc;
	}

	private bool FadeProc()
	{
		if (!imageFade)
		{
			return false;
		}
		if (!isFade)
		{
			return false;
		}
		timeFadeTime += Time.deltaTime;
		Color color = imageFade.color;
		float time = Mathf.Clamp01(timeFadeTime / timeFade);
		time = fadeAnimation.Evaluate(time);
		switch (kindFade)
		{
		case FadeKind.Out:
			color.a = time;
			break;
		case FadeKind.In:
			color.a = 1f - time;
			break;
		case FadeKind.OutIn:
			color.a = Mathf.Sin((float)Math.PI / 180f * Mathf.Lerp(0f, 180f, time));
			break;
		}
		imageFade.color = color;
		if (time >= 1f)
		{
			isFade = false;
			switch (kindFade)
			{
			case FadeKind.Out:
				kindFadeProc = FadeKindProc.OutEnd;
				break;
			case FadeKind.In:
				kindFadeProc = FadeKindProc.InEnd;
				imageFade.enabled = false;
				break;
			case FadeKind.OutIn:
				kindFadeProc = FadeKindProc.OutInEnd;
				imageFade.enabled = false;
				break;
			}
		}
		return true;
	}

	public bool SetHoushi3PStart()
	{
		houshi3P.categoryActionButton.SetActive(false);
		objCommonAibuIcon.SetActive(false);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(false);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(true);
		}
		return true;
	}

	public bool SetHoushi3PActionButtonActive(bool _active, int _array = -1)
	{
		houshi3P.categoryActionButton.SetActive(_active, _array);
		return true;
	}

	public bool SetHoushi3PAutoFinish(bool _force = false)
	{
		if (!females[0].getAnimatorStateInfo(0).IsName("OLoop") && !_force)
		{
			return false;
		}
		int num = ((flags.selectAnimationListInfo == null) ? flags.nowAnimationInfo.numCtrl : flags.selectAnimationListInfo.numCtrl);
		houshi3P.categoryActionButton.SetActive((num == 1 || num == 3) && !houshi3P.tglAutoFinish.isOn, 0);
		houshi3P.categoryActionButton.SetActive((num != 1 && num != 3) || houshi3P.tglAutoFinish.isOn, 1);
		return true;
	}

	public bool Is3PHoushiAutoFinish()
	{
		return houshi3P.tglAutoFinish.isOn;
	}

	public void Houshi3PInitRely()
	{
		houshi3P.tglRely.isOn = false;
	}

	public bool Houshi3PProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			flag = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("Drink_A") || animatorStateInfo.IsName("Vomit_A")) && !flags.voiceWait;
			SetSubMenuEnable(flag);
			btnEnd.interactable = flag;
			houshi3P.tglAutoFinish.interactable = !flags.voiceWait;
			houshi3P.categoryActionButton.SetEnable(!flags.voiceWait);
			menuMain.SetEnable(flag, 2);
			houshi3P.tglAutoFinish.gameObject.SetActive(flags.nowAnimationInfo.numCtrl == 1 || flags.nowAnimationInfo.numCtrl == 3);
			bool flag2 = animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop");
			Color color = houshi3P.imagePad.color;
			color.a = ((!flag2) ? 0.3f : 1f);
			houshi3P.imagePad.color = color;
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishMale.interactable = flag;
		}
		return true;
	}

	public bool SetSonyu3PStart()
	{
		sonyu3P.categoryActionButton.SetActive(false);
		int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
		sonyu3P.categoryActionButton.SetActive(true, num);
		sonyu3P.categoryActionButton.SetActive(true, 1 + num);
		bool active = ((flags.selectAnimationListInfo == null) ? flags.nowAnimationInfo.paramFemale.isAnal : flags.selectAnimationListInfo.paramFemale.isAnal);
		sonyu3P.categoryActionButton.SetActive(active, 2 + num);
		sonyu3P.categoryActionButton.SetActive(active, 3 + num);
		objCommonAibuIcon.SetActive(false);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(true);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(true);
		}
		return true;
	}

	public bool SetSonyu3PActionButtonActive(bool _active, int _array = -1)
	{
		int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
		sonyu3P.categoryActionButton.SetActive(_active, (_array != -1) ? (_array + num) : _array);
		return true;
	}

	public bool IsSonyu3PAutoFinish()
	{
		return sonyu3P.tglAutoFinish.isOn;
	}

	public bool SetCondom3P(bool _isOn)
	{
		sonyu3P.isCondom = _isOn;
		sonyu3P.buttonCondomChange.OnChangeValue(_isOn);
		return true;
	}

	public bool Sonyu3PProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			flag = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("InsertIdle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_Idle") || animatorStateInfo.IsName("A_InsertIdle") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop") || animatorStateInfo.IsName("A_IN_A") || animatorStateInfo.IsName("A_OUT_A")) && !flags.voiceWait;
			SetSubMenuEnable(flag);
			btnEnd.interactable = flag;
			sonyu3P.categoryActionButton.SetEnable(!flags.voiceWait);
			sonyu3P.tglAutoFinish.interactable = !flags.voiceWait;
			menuMain.SetEnable(flag, 2);
			sonyu3P.buttonCondom.interactable = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_Idle") || animatorStateInfo.IsName("A_OUT_A")) && !flags.voiceWait;
			int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
			bool flag2 = ((animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop")) && flags.gaugeMale >= 70f) || flags.isDebug;
			sonyu3P.categoryActionButton.SetActive(flag2 && !sonyu3P.tglAutoFinish.isOn, 4 + num);
			sonyu3P.categoryActionButton.SetActive(flag2 && sonyu3P.tglAutoFinish.isOn, 5 + num);
			bool flag3 = animatorStateInfo.IsName("InsertIdle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("A_InsertIdle") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop") || animatorStateInfo.IsName("A_IN_A");
			Color color = sonyu3P.imagePad.color;
			color.a = ((!flag3) ? 0.3f : 1f);
			sonyu3P.imagePad.color = color;
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.interactable = flag;
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.interactable = flag;
		}
		return true;
	}

	public bool SetAibuStart()
	{
		objCommonAibuIcon.SetActive(true);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(true);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(false);
		}
		return true;
	}

	public bool SetAibuActionButtonActive(bool _active, int _array = -1)
	{
		return true;
	}

	public bool SetAibuAutoFinish(bool _force = false)
	{
		return true;
	}

	public bool AibuProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			flag = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("M_Idle") || animatorStateInfo.IsName("S_Idle") || animatorStateInfo.IsName("A_Idle") || animatorStateInfo.IsName("Orgasm_A")) && !flags.voiceWait;
			SetSubMenuEnable(flag);
			menuMain.SetEnable(flag, 2);
			btnEnd.interactable = flag;
			if ((bool)btnTrespassing)
			{
				btnTrespassing.interactable = flag;
			}
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.interactable = flag;
		}
		return true;
	}

	public bool SetHoushiStart()
	{
		houshi.categoryActionButton.SetActive(false);
		objCommonAibuIcon.SetActive(true);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(false);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(true);
		}
		return true;
	}

	public bool SetHoushiActionButtonActive(bool _active, int _array = -1)
	{
		houshi.categoryActionButton.SetActive(_active, _array);
		return true;
	}

	public bool SetHoushiAutoFinish(bool _force = false)
	{
		if (!females[0].getAnimatorStateInfo(0).IsName("OLoop") && !_force)
		{
			return false;
		}
		int num = ((flags.selectAnimationListInfo == null) ? flags.nowAnimationInfo.numCtrl : flags.selectAnimationListInfo.numCtrl);
		houshi.categoryActionButton.SetActive(num == 1 && !houshi.tglAutoFinish.isOn, 0);
		houshi.categoryActionButton.SetActive(num != 1 || houshi.tglAutoFinish.isOn, 1);
		return true;
	}

	public bool IsHoushiAutoFinish()
	{
		return houshi.tglAutoFinish.isOn;
	}

	public void HoushiInitRely()
	{
		houshi.tglRely.isOn = false;
	}

	public bool HoushiProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			flag = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("Drink_A") || animatorStateInfo.IsName("Vomit_A")) && !flags.voiceWait;
			SetSubMenuEnable(flag);
			btnEnd.interactable = flag;
			houshi.tglAutoFinish.interactable = !flags.voiceWait;
			houshi.categoryActionButton.SetEnable(!flags.voiceWait);
			if ((bool)btnTrespassing)
			{
				btnTrespassing.interactable = flag;
			}
			menuMain.SetEnable(flag, 2);
			houshi.tglAutoFinish.gameObject.SetActive(flags.nowAnimationInfo.numCtrl == 1);
			bool flag2 = animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop");
			Color color = houshi.imagePad.color;
			color.a = ((!flag2) ? 0.3f : 1f);
			houshi.imagePad.color = color;
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishMale.interactable = flag;
		}
		return true;
	}

	public bool SetLesbianStart()
	{
		objCommonAibuIcon.SetActive(false);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(flags.isFreeH);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(false);
		}
		return true;
	}

	public bool LesbianProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			if (flags.isFreeH)
			{
				AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
				flag = (animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("MLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop") || animatorStateInfo.IsName("Orgasm_B")) && !flags.voiceWait;
				menuMain.SetEnable(flag, 2);
			}
			btnEnd.interactable = true;
			if ((bool)btnTrespassing)
			{
				btnTrespassing.interactable = true;
			}
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.interactable = flag;
		}
		return true;
	}

	public bool SetMasturbationStart()
	{
		objCommonAibuIcon.SetActive(false);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(flags.isFreeH);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(false);
		}
		return true;
	}

	public bool MasturbationProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			if (flags.isFreeH)
			{
				AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
				flag = (animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("MLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("OLoop") || animatorStateInfo.IsName("Orgasm_B")) && !flags.voiceWait;
				menuMain.SetEnable(flag, 2);
			}
			btnEnd.interactable = true;
			if ((bool)btnTrespassing)
			{
				btnTrespassing.interactable = true;
			}
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.interactable = flag;
		}
		return true;
	}

	public bool SetPeepingStart()
	{
		objCommonAibuIcon.SetActive(false);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(false);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(false);
		}
		return true;
	}

	public bool PeepingProc()
	{
		if (females.Count > 0 && (bool)females[0])
		{
			SetSubMenuEnable(true);
			btnEnd.interactable = true;
		}
		return true;
	}

	public bool SetSonyuStart()
	{
		sonyu.categoryActionButton.SetActive(false);
		int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
		sonyu.categoryActionButton.SetActive(true, num);
		sonyu.categoryActionButton.SetActive(true, 1 + num);
		bool active = ((flags.selectAnimationListInfo == null) ? flags.nowAnimationInfo.paramFemale.isAnal : flags.selectAnimationListInfo.paramFemale.isAnal);
		sonyu.categoryActionButton.SetActive(active, 2 + num);
		sonyu.categoryActionButton.SetActive(active, 3 + num);
		objCommonAibuIcon.SetActive(true);
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.gameObject.SetActive(true);
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.gameObject.SetActive(true);
		}
		return true;
	}

	public bool SetSonyuActionButtonActive(bool _active, int _array = -1)
	{
		int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
		sonyu.categoryActionButton.SetActive(_active, (_array != -1) ? (_array + num) : _array);
		return true;
	}

	public bool IsSonyuAutoFinish()
	{
		return sonyu.tglAutoFinish.isOn;
	}

	public bool SetCondom(bool _isOn)
	{
		sonyu.isCondom = _isOn;
		sonyu.buttonCondomChange.OnChangeValue(_isOn);
		return true;
	}

	public bool SonyuProc()
	{
		bool flag = false;
		if (females.Count > 0 && (bool)females[0])
		{
			AnimatorStateInfo animatorStateInfo = females[0].getAnimatorStateInfo(0);
			flag = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("InsertIdle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_Idle") || animatorStateInfo.IsName("A_InsertIdle") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop") || animatorStateInfo.IsName("A_IN_A") || animatorStateInfo.IsName("A_OUT_A")) && !flags.voiceWait;
			SetSubMenuEnable(flag);
			btnEnd.interactable = flag;
			sonyu.categoryActionButton.SetEnable(!flags.voiceWait);
			sonyu.tglAutoFinish.interactable = !flags.voiceWait;
			if ((bool)btnTrespassing)
			{
				btnTrespassing.interactable = flag;
			}
			menuMain.SetEnable(flag, 2);
			sonyu.buttonCondom.interactable = (animatorStateInfo.IsName("Idle") || animatorStateInfo.IsName("OUT_A") || animatorStateInfo.IsName("A_Idle") || animatorStateInfo.IsName("A_OUT_A")) && !flags.voiceWait;
			int num = ((flags.selectAnimationListInfo != null) ? (flags.selectAnimationListInfo.isFemaleInitiative ? 1 : 0) : (flags.nowAnimationInfo.isFemaleInitiative ? 1 : 0)) * 7;
			bool flag2 = ((animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop")) && flags.gaugeMale >= 70f) || flags.isDebug;
			sonyu.categoryActionButton.SetActive(flag2 && !sonyu.tglAutoFinish.isOn, 4 + num);
			sonyu.categoryActionButton.SetActive(flag2 && sonyu.tglAutoFinish.isOn, 5 + num);
			bool flag3 = animatorStateInfo.IsName("InsertIdle") || animatorStateInfo.IsName("WLoop") || animatorStateInfo.IsName("SLoop") || animatorStateInfo.IsName("IN_A") || animatorStateInfo.IsName("A_InsertIdle") || animatorStateInfo.IsName("A_WLoop") || animatorStateInfo.IsName("A_SLoop") || animatorStateInfo.IsName("A_IN_A");
			Color color = sonyu.imagePad.color;
			color.a = ((!flag3) ? 0.3f : 1f);
			sonyu.imagePad.color = color;
		}
		if ((bool)btnImmediatelyFinishFemale)
		{
			btnImmediatelyFinishFemale.interactable = flag;
		}
		if ((bool)btnImmediatelyFinishMale)
		{
			btnImmediatelyFinishMale.interactable = flag;
		}
		return true;
	}
}
