using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using Manager;
using UnityEngine;

public class HFlag : BaseLoader
{
	public class DeliveryMember
	{
		public HFlag ctrlFlag;

		public ChaControl chaFemale;

		public ChaControl chaFemale1;

		public ChaControl chaMale;

		public CrossFade fade;

		public HSprite sprite;

		public List<MotionIK> lstMotionIK;

		public HandCtrl hand;

		public HandCtrl hand1;

		public YureCtrl yure;

		public YureCtrl yure1;

		public ItemObject item;

		public HitCollisionEnableCtrl hitcolFemale;

		public HitCollisionEnableCtrl hitcolFemale1;

		public HitCollisionEnableCtrl hitcolMale;

		public MetaballCtrl meta;

		public ParentObjectCtrl parentObjectFemale;

		public ParentObjectCtrl parentObjectFemale1;

		public ParentObjectCtrl parentObjectMale;

		public HVoiceCtrl voice;

		public HSeCtrl se;

		public HParticleCtrl particle;

		public AnimatorLayerCtrl alCtrl;

		public AnimatorLayerCtrl alCtrl1;
	}

	[Serializable]
	public class TimeWait
	{
		public float timeIdleCalc;

		public float timeIdle;

		public float timeMin = 15f;

		public float timeMax = 25f;

		public float timeIdleMin = 15f;

		public float timeIdleMax = 25f;

		public float time70Min = 3f;

		public float time70Max = 3f;

		public bool isIdle = true;

		public void MemberInit()
		{
			SetWaitTime(true);
			SetIdleTime();
		}

		public void SetIdleTime()
		{
			timeIdleCalc = 0f;
			timeIdle = UnityEngine.Random.Range(timeMin, timeMax);
		}

		public void SetWaitTime(bool _isIdle)
		{
			timeMin = ((!_isIdle) ? time70Min : timeIdleMin);
			timeMax = ((!_isIdle) ? time70Max : timeIdleMax);
			isIdle = _isIdle;
		}

		public bool IsIdleTime()
		{
			timeIdleCalc += Time.deltaTime;
			if (timeIdle > timeIdleCalc)
			{
				return false;
			}
			SetIdleTime();
			return true;
		}
	}

	public enum EMode
	{
		none = -1,
		aibu = 0,
		houshi = 1,
		sonyu = 2,
		masturbation = 3,
		peeping = 4,
		lesbian = 5,
		houshi3P = 6,
		sonyu3P = 7
	}

	public enum ClickKind
	{
		none = 0,
		insert = 1,
		insert_voice = 2,
		insert_anal = 3,
		insert_anal_voice = 4,
		speedup = 5,
		motionchange = 6,
		fast = 7,
		slow = 8,
		inside = 9,
		outside = 10,
		drink = 11,
		vomit = 12,
		pull = 13,
		again = 14,
		mouth = 15,
		muneL = 16,
		muneR = 17,
		kokan = 18,
		anal = 19,
		siriL = 20,
		siriR = 21,
		de_muneL = 22,
		de_muneR = 23,
		de_kokan = 24,
		de_anal = 25,
		de_siriL = 26,
		de_siriR = 27,
		modeChange = 28,
		actionChange = 29,
		end = 30,
		anal_dislikes = 31,
		massage_mune_dislikes = 32,
		massage_kokan_dislikes = 33,
		pointmove = 34,
		peeping_restart = 35,
		orgW = 36,
		sameW = 37,
		orgS = 38,
		sameS = 39,
		Idle = 40,
		stop_Idle = 41,
		OLoop = 42,
		Front_Dislikes = 43,
		Back_Dislikes = 44
	}

	public enum FinishKind
	{
		none = 0,
		inside = 1,
		outside = 2,
		orgW = 3,
		sameW = 4,
		orgS = 5,
		sameS = 6
	}

	[Serializable]
	public class VoiceFlag
	{
		public int[] playVoices = new int[2] { -1, -1 };

		public int[] playShorts = new int[2] { -1, -1 };

		public bool[] isShortsPlayTouchWeak = new bool[2];

		public bool[] gentles = new bool[2];

		public bool speedMotion;

		public bool speedItem;

		public bool loopMotionAorB;

		public int[] eyenecks = new int[2] { -1, -1 };

		public bool isFemale70PercentageVoicePlay;

		public bool isMale70PercentageVoicePlay;

		public bool isAfterVoicePlay;

		[SerializeField]
		private TimeWait timeAibu = new TimeWait();

		[SerializeField]
		private TimeWait timeHoushi = new TimeWait();

		[SerializeField]
		private TimeWait timeSonyu = new TimeWait();

		public void MemberInit()
		{
			playVoices[0] = -1;
			playVoices[1] = -1;
			playShorts[0] = -1;
			playShorts[1] = -1;
			isShortsPlayTouchWeak[0] = false;
			isShortsPlayTouchWeak[1] = false;
			gentles[0] = false;
			gentles[1] = false;
			speedMotion = false;
			speedItem = false;
			loopMotionAorB = false;
			timeAibu.MemberInit();
			timeHoushi.MemberInit();
			timeSonyu.MemberInit();
		}

		public void SetAibuIdleTime()
		{
			timeAibu.SetIdleTime();
		}

		public bool IsAibuIdleTime()
		{
			return timeAibu.IsIdleTime();
		}

		public void SetHoushiIdleTime()
		{
			timeHoushi.SetIdleTime();
		}

		public bool IsHoushiIdleTime()
		{
			return timeHoushi.IsIdleTime();
		}

		public void SetSonyuIdleTime()
		{
			timeSonyu.SetIdleTime();
		}

		public bool IsSonyuIdleTime()
		{
			return timeSonyu.IsIdleTime();
		}

		public void SetSonyuWaitTime(bool _isIdle)
		{
			timeSonyu.SetWaitTime(_isIdle);
		}

		public bool IsSonyuIdleFlag()
		{
			return timeSonyu.isIdle;
		}
	}

	[Serializable]
	public class MouseAction
	{
		public bool click;

		public bool drag;
	}

	[Serializable]
	public class Count
	{
		public int aibuOrg;

		public int kiss;

		public int notKiss;

		public MouseAction[,] mouseAction = new MouseAction[10, 7];

		public int notPowerful;

		public int dontTouchAnal;

		public int dontTouchMassage;

		public float[] selectMassages = new float[2];

		public float[] selectAreas = new float[6];

		public float[] selectAreasGlobal = new float[6];

		public float selectHobby;

		public int houshiOutside;

		public int houshiInside;

		public int handFinish;

		public int nameFinish;

		public int kuwaeFinish;

		public int paizuriFinish;

		public int paizurinameFinish;

		public int paizurikuwaeFinish;

		public int houshiDrink;

		public int houshiVomit;

		public int splash;

		public int sonyuCondomInside;

		public int sonyuOrg;

		public int sonyuInside;

		public int sonyuOutside;

		public int sonyuSame;

		public int sonyuCondomSame;

		public int sonyuTare;

		public int sonyuAnalCondomInside;

		public int sonyuAnalOrg;

		public int sonyuAnalInside;

		public int sonyuAnalOutside;

		public int sonyuAnalSame;

		public int sonyuAnalCondomSame;

		public int sonyuAnalTare;

		public int notCondomPlay;

		public int notAnalPlay;

		public bool isInsertKokan;

		public bool isInsertAnal;

		public bool isNameInsertKokan;

		public bool isHoushiPlay;

		public bool isInsertKokanVoiceCondition;

		public bool isInsertAnalVoiceCondition;

		public int sonyuKokanPlay;

		public int sonyuAnalPlay;
	}

	public enum MenstruationType
	{
		安全日 = 0,
		危険日 = 1
	}

	[Header("---< 全体 >---")]
	[Label("ゴムなし挿入可能")]
	public bool[] isInsertOK = new bool[2];

	[Label("アナル挿入可能")]
	public bool isAnalInsertOK;

	[Label("アナル挿入中")]
	public bool isAnalPlay;

	[RangeLabel("女快感値", 0f, 100f)]
	public float gaugeFemale;

	[Label("女快感値固定")]
	public bool lockGugeFemale;

	[RangeLabel("男快感値", 0f, 100f)]
	public float gaugeMale;

	[Label("男快感値固定")]
	public bool lockGugeMale;

	[Label("クリック率または自動")]
	public float rateClickGauge = 0.1f;

	[Label("ドラッグ率")]
	public float rateDragGauge = 0.1f;

	[Label("減少率")]
	public float rateDecreaseGauge = 0.1f;

	[Label("絶頂時に快感値がなくなるまでの時間")]
	public float timeDecreaseGauge = 0.3f;

	[Label("快感値がなくなるまでの経過時間")]
	public float timeDecreaseGaugeCalc;

	[Label("弱点時の快感値倍率")]
	public float rateWeakPoint = 1.5f;

	[RangeLabel("乳首立ちの最大比率", 0f, 1f)]
	public float rateMaxNip;

	[RangeLabel("乳首立ち比率", 0f, 1f)]
	public float rateNip;

	[Label("媚薬設定倍率 読込み時のみ")]
	public float addPtion = 1.5f;

	[Label("媚薬倍率")]
	public float potion = 1f;

	[Label("男感度倍率")]
	public float feelMaleCheat = 1f;

	[Label("経験値チート率")]
	public float expCheat = 1f;

	[Label("モーションスピード")]
	[Header("---< モーション関連 >---")]
	public float speed;

	[Label("モーションアイテムスピード")]
	public float speedItem;

	[RangeLabel("モーションスピード係数", 0f, 1f)]
	public float speedCalc;

	[Label("挿入スピードアニメーション")]
	public AnimationCurve speedSonyuCurve;

	[Label("奉仕スピードアニメーション")]
	public AnimationCurve speedHoushiCurve;

	[Label("愛撫時身体最大モーションスピード")]
	public float speedMaxAibuBody = 1.5f;

	[Label("愛撫時アイテム最大モーションスピード")]
	public float speedMaxAibuItem = 1.5f;

	[Label("身体最大モーションスピード")]
	public float speedMaxBody;

	[Label("アイテム最大モーションスピード")]
	public float speedMaxItem;

	[Label("声の体速さ切り替え割合")]
	public float speedVoiceChangeSpeedRate = 0.6f;

	[Label("声のアイテム速さ切り替え割合")]
	public float speedVoiceChangeSpeedRateItem = 0.6f;

	[RangeLabel("モーション揺らぎ", 0f, 1f)]
	public float motion;

	[RangeLabel("モーション揺らぎ二人目", 0f, 1f)]
	public float motion1;

	[Label("揺らぎモーション発動までの最小時間")]
	public float timeAutoMotionMin;

	[Label("揺らぎモーション発動までの最大時間")]
	public float timeAutoMotionMax;

	[Label("揺らぎモーションを変更している最小時間")]
	public float timeMotionMin;

	[Label("揺らぎモーションを変更している最大時間")]
	public float timeMotionMax;

	[Label("揺らぎモーションを最低でもこんだけ進める")]
	public float rateMotionMin;

	[Label("揺らぎモーション変更しているときのリープアニメーション")]
	public AnimationCurve curveMotion;

	[Label("レズの最大スピード 1 + この値")]
	public float speedLesbian = 0.4f;

	[Label("加速時間")]
	[Header("---< スピード操作関連 >---")]
	[Header("●上がり")]
	public float timeSpeedUpStart = 0.5f;

	[Label("愛撫加速時間")]
	public float timeSpeedUpStartAibu = 0.1f;

	[Label("アイテム加速時間")]
	public float timeSpeedUpStartItem = 0.1f;

	[Label("加速経過時間")]
	public float timeSpeedUpStartCalc;

	[Label("アイテム加速経過時間")]
	public float timeSpeedUpStartItemCalc;

	[Tooltip("クリックしたときに、Xに今のスピード Yに今のスピード＋上げる割合をいれてスピードを徐々に上げるための値")]
	[Label("加速計算")]
	public Vector2 speedUpClac = default(Vector2);

	[Tooltip("クリックしたときに、Xに今のスピード Yに今のスピード＋上げる割合をいれてスピードを徐々に上げるための値")]
	[Label("アイテム加速計算")]
	public Vector2 speedUpItemClac = default(Vector2);

	[Label("1クリックで上げる割合")]
	public float rateSpeedUp = 0.1f;

	[Label("愛撫1クリックで上げる割合")]
	public float rateSpeedUpAibu = 0.01f;

	[Label("愛撫1クリックでアイテムのスピードを上げる割合")]
	public float rateSpeedUpItem = 0.01f;

	[Label("1ホイールで上げる割合")]
	public float rateWheelSpeedUp = 0.05f;

	[Label("おしっぱで上げる割合")]
	public float rateStateSpeedUp = 0.008f;

	[Label("ドラッグで上げる割合")]
	public float rateDragSpeedUp = 0.01f;

	[Label("アイテムをドラッグで上げる割合")]
	public float rateDragSpeedUpItem = 0.1f;

	[Label("減速時間")]
	[Header("●下がり")]
	public float timeSpeedSubStart = 1f;

	[Label("減速経過時間")]
	public float timeSpeedSubStartCalc;

	[Tooltip("Xの値からYの値まで減速する割合が時間によって変化する")]
	[Label("減速割合")]
	public Vector2 rateSpeedSub = new Vector2(0f, 0.5f);

	[Label("アイテム減速割合")]
	public float rateSpeedSubItem = 0.5f;

	[Label("この時間内にクリックした？")]
	[Header("●クリック判定")]
	public float timeNextClick = 1f;

	[Label("愛撫この時間内にクリックした？")]
	public float timeNextClickAibu = 0.2f;

	[Label("アイテムこの時間内にクリックした？")]
	public float timeNextClickItem = 0.2f;

	[Label("クリックしていない時間")]
	public float timeNoClick;

	[Label("クリックしていない時間アイテム用")]
	public float timeNoClickItem;

	[Label("奉仕のときのスピード変更フラグ")]
	public int speedHoushi;

	[Label("押している時間")]
	[Header("---< ドラッグ関連 >---")]
	public float timeDragCalc;

	[Label("押している判定する時間")]
	public float timeDrag = 0.5f;

	[Label("ドラッグしてる")]
	public bool drag;

	[Label("DynamicBoneでいじっているときのマウスの速度")]
	public float speedDynamicBoneMove = 0.001f;

	[Label("XYでいじっているときのマウスの速度")]
	public float speedXYMove = 0.001f;

	[Label("Drag時の限界移動量")]
	public float forceLength = 1f;

	[Header("---< アニメーションパラメーター >---")]
	public Vector2[] xy = new Vector2[6];

	[Header("---< 声関連パラメーター >---")]
	public VoiceFlag voice = new VoiceFlag();

	[Header("---< 回数関連パラメーター >---")]
	public Count count = new Count();

	[Label("フリーH")]
	[Header("---< その他 >---")]
	public bool isFreeH;

	[Label("初回H緩和")]
	public bool firstHEasy;

	[Label("イキそうボタン使用した")]
	public bool isUseImmediatelyFinishButton;

	[Label("オナニー見つかる")]
	public bool isMasturbationFound;

	[Label("アニメーションステート名")]
	public string nowAnimStateName = "Idle";

	[Label("クリック状態")]
	public ClickKind click;

	[Label("どんな絶頂")]
	public FinishKind finish;

	[Label("assetbundleパス")]
	public HashSet<string> hashAssetBundle = new HashSet<string>();

	[Label("CameraControl")]
	public CameraControl_Ver2 ctrlCamera;

	public TimeWait timeWaitHoushi = new TimeWait();

	public TimeWait timeWaitSonyu = new TimeWait();

	public TimeWait timeMasturbation = new TimeWait();

	public TimeWait timeLesbian = new TimeWait();

	[Label("愛撫の選択")]
	public bool isAibuSelect = true;

	[Label("奉仕の姿勢")]
	public int poseHoushi;

	[Label("コンドームの有無")]
	public bool isCondom;

	public HSceneProc.AnimationListInfo nowAnimationInfo;

	[Label("音声待ち")]
	public bool voiceWait;

	[Label("拒否音声待ち")]
	public bool isDenialvoiceWait;

	[Label("Hシーン終了")]
	public bool isHSceneEnd;

	[Label("Hシーン終了順序")]
	public int numEnd;

	[Label("おまかせ")]
	public bool rely;

	[Label("中出しした？")]
	public bool isInsideFinish;

	public Transform[] transVoiceMouth = new Transform[2];

	public List<SaveData.Heroine> lstHeroine = new List<SaveData.Heroine>();

	public SaveData.Player player;

	public SaveData.Heroine newHeroine;

	private HSceneProc.AnimationListInfo _selectAnimationListInfo;

	[DisabledGroup("慣れ状態")]
	public HSceneProc.EExperience experience;

	public static byte[] menstruations = new byte[15]
	{
		0, 0, 0, 0, 1, 1, 1, 1, 1, 0,
		0, 0, 0, 0, 0
	};

	[Header("---< デバッグ >---")]
	[Label("デバッグモード")]
	public bool isDebug;

	[Label("ループ回数")]
	public int debugForceLoop = 1;

	public EMode mode { get; set; }

	public HSceneProc.AnimationListInfo selectAnimationListInfo
	{
		get
		{
			return _selectAnimationListInfo;
		}
		set
		{
			_selectAnimationListInfo = value;
		}
	}

	private void Start()
	{
		timeWaitHoushi.MemberInit();
		timeWaitSonyu.MemberInit();
		timeMasturbation.MemberInit();
		timeLesbian.MemberInit();
		for (int i = 0; i < 10; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				count.mouseAction[i, j] = new MouseAction();
			}
		}
	}

	public bool SpeedUpClick(float _rateSpeedUp, float _rateSpeedMax)
	{
		speedUpClac.x = speedCalc;
		speedUpClac.y = Mathf.Clamp(((timeNoClick != 0f) ? speedUpClac.y : speedCalc) + _rateSpeedMax * _rateSpeedUp, 0f, _rateSpeedMax);
		timeSpeedUpStartCalc = 0f;
		timeNoClick = timeNextClick;
		timeSpeedSubStartCalc = 0f;
		speedMaxBody = _rateSpeedMax;
		return true;
	}

	public bool WaitSpeedProc(bool _isLock, AnimationCurve _curve)
	{
		timeNoClick = Mathf.Max(0f, timeNoClick - Time.deltaTime);
		if (timeNoClick == 0f)
		{
			if (!_isLock)
			{
				timeSpeedSubStartCalc = Mathf.Clamp(timeSpeedSubStartCalc + Time.deltaTime, 0f, timeSpeedSubStart);
				float t = ((!(timeSpeedSubStartCalc < 0f)) ? (timeSpeedSubStartCalc / timeSpeedSubStart) : 1f);
				t = Mathf.Lerp(rateSpeedSub.x, rateSpeedSub.y, t);
				speedCalc = Mathf.Max(0f, speedCalc - t * Time.deltaTime);
			}
		}
		else
		{
			timeSpeedUpStartCalc = Mathf.Clamp(timeSpeedUpStartCalc + Time.deltaTime, 0f, timeSpeedUpStart);
			float t2 = ((!(timeSpeedUpStartCalc < 0f)) ? (timeSpeedUpStartCalc / timeSpeedUpStart) : 1f);
			speedCalc = Mathf.Lerp(speedUpClac.x, speedUpClac.y, t2);
		}
		float num = speedMaxBody * speedVoiceChangeSpeedRate;
		if (!voice.speedMotion && speedCalc > num)
		{
			voice.speedMotion = true;
		}
		else if (voice.speedMotion && speedCalc < speedMaxBody - num)
		{
			voice.speedMotion = false;
		}
		speed = _curve.Evaluate(speedCalc);
		return true;
	}

	public bool SpeedUpClickAibu(float _rateSpeedUp, float _rateSpeedMax, bool _drag)
	{
		speedUpClac.x = speed;
		if (!_drag)
		{
			speedUpClac.y = Mathf.Clamp(((timeSpeedUpStartCalc != timeSpeedUpStartAibu) ? speedUpClac.y : speed) + _rateSpeedMax * _rateSpeedUp, 0f, _rateSpeedMax);
			timeSpeedUpStartCalc = 0f;
			timeNoClick = timeNextClickAibu;
			timeSpeedSubStartCalc = 0f;
		}
		else
		{
			speed = Mathf.Clamp(speed + _rateSpeedUp, 0f, _rateSpeedMax);
			timeNoClick = 0f;
			timeSpeedSubStartCalc = timeSpeedSubStart;
		}
		speedMaxBody = _rateSpeedMax;
		return true;
	}

	public bool DragStart()
	{
		drag = true;
		timeDragCalc = 0f;
		return true;
	}

	public bool SpeedUpClickItemAibu(float _rateSpeedUp, float _rateSpeedMax, bool _drag)
	{
		speedUpItemClac.x = speedItem;
		if (!_drag)
		{
			speedUpItemClac.y = Mathf.Clamp(((timeSpeedUpStartItemCalc != timeSpeedUpStartItem) ? speedUpItemClac.y : speedItem) + _rateSpeedMax * _rateSpeedUp, 0f, _rateSpeedMax);
			timeSpeedUpStartItemCalc = 0f;
			timeNoClickItem = timeNextClickItem;
		}
		else
		{
			speedUpItemClac.y = Mathf.Clamp(speedItem + _rateSpeedUp, 0f, _rateSpeedMax);
			timeSpeedUpStartItemCalc = timeSpeedUpStartItem;
			timeNoClickItem = ((_rateSpeedUp != 0f) ? 1 : 0);
		}
		speedMaxItem = _rateSpeedMax;
		return true;
	}

	public bool WaitSpeedProcAibu()
	{
		timeNoClick = Mathf.Max(0f, timeNoClick - Time.deltaTime);
		if (IsClickTimeOver())
		{
			timeSpeedSubStartCalc = Mathf.Clamp(timeSpeedSubStartCalc + Time.deltaTime, 0f, timeSpeedSubStart);
			float t = ((!(timeSpeedSubStartCalc < 0f)) ? (timeSpeedSubStartCalc / timeSpeedSubStart) : 1f);
			t = Mathf.Lerp(rateSpeedSub.x, rateSpeedSub.y, t);
			speed = Mathf.Max(0f, speed - t * Time.deltaTime);
		}
		else
		{
			timeSpeedUpStartCalc = Mathf.Clamp(timeSpeedUpStartCalc + Time.deltaTime, 0f, timeSpeedUpStartAibu);
			float t2 = ((!(timeSpeedUpStartCalc < 0f)) ? (timeSpeedUpStartCalc / timeSpeedUpStartAibu) : 1f);
			speed = Mathf.Lerp(speedUpClac.x, speedUpClac.y, t2);
		}
		float num = speedMaxBody * speedVoiceChangeSpeedRate;
		if (!voice.speedMotion && speed > num)
		{
			voice.speedMotion = true;
		}
		else if (voice.speedMotion && speed < speedMaxBody - num)
		{
			voice.speedMotion = false;
		}
		WaitSpeedProcItem();
		return true;
	}

	public bool WaitSpeedProcItem()
	{
		if (drag)
		{
			timeDragCalc = Mathf.Min(timeDragCalc + Time.deltaTime, timeDrag);
		}
		timeNoClickItem = Mathf.Max(0f, timeNoClickItem - Time.deltaTime);
		if (!IsClickTimeOverItem())
		{
			timeSpeedUpStartItemCalc = Mathf.Clamp(timeSpeedUpStartItemCalc + Time.deltaTime, 0f, timeSpeedUpStartItem);
			float t = ((!(timeSpeedUpStartItemCalc < 0f)) ? (timeSpeedUpStartItemCalc / timeSpeedUpStartItem) : 1f);
			speedItem = Mathf.Lerp(speedUpItemClac.x, speedUpItemClac.y, t);
		}
		speedItem = Mathf.Max(0f, speedItem - Time.deltaTime * rateSpeedSubItem);
		float num = speedMaxItem * speedVoiceChangeSpeedRateItem;
		if (!voice.speedItem && speedItem > num)
		{
			voice.speedItem = true;
		}
		else if (voice.speedItem && speedItem < speedMaxItem - num)
		{
			voice.speedItem = false;
		}
		return true;
	}

	private bool IsClickTimeOverItem()
	{
		return timeNoClickItem == 0f;
	}

	public bool IsClickTimeOver()
	{
		return timeNoClick == 0f;
	}

	public bool FinishDrag()
	{
		drag = false;
		timeDragCalc = 0f;
		return true;
	}

	public bool IsDrag()
	{
		return timeDragCalc == timeDrag;
	}

	public void SpeedRelationClear()
	{
		speed = 0f;
		speedItem = 0f;
		speedUpClac = Vector2.zero;
		speedUpItemClac = Vector2.zero;
		voice.speedMotion = false;
		voice.speedItem = false;
	}

	public void FemaleGaugeUp(float _addPoint, bool _force, bool _isIdle = true)
	{
		if (!lockGugeFemale || _force)
		{
			float num = 100f;
			if (mode == EMode.houshi)
			{
				num = ((!(gaugeFemale < 70f)) ? gaugeFemale : 69f);
			}
			else if (mode == EMode.sonyu)
			{
				num = ((!_isIdle) ? 100f : ((!(gaugeFemale < 70f)) ? gaugeFemale : 69f));
			}
			if (_addPoint > 0f && mode != EMode.masturbation && mode != EMode.lesbian)
			{
				Singleton<Game>.Instance.saveData.clubReport.hAdd += ((!(_addPoint * potion + gaugeFemale >= num)) ? (_addPoint * potion) : (num - gaugeFemale)) * ((!lstHeroine[0].isStaff) ? 1.5f : 2f);
			}
			gaugeFemale += _addPoint * potion;
			gaugeFemale = Mathf.Clamp(gaugeFemale, 0f, num);
		}
	}

	public void MaleGaugeUp(float _addPoint)
	{
		if (!lockGugeMale)
		{
			gaugeMale += _addPoint * feelMaleCheat;
			gaugeMale = Mathf.Clamp(gaugeMale, 0f, 100f);
		}
	}

	public void AddAibuOrg()
	{
		count.aibuOrg = Mathf.Min(count.aibuOrg + 1, 10000);
	}

	public void AddKiss()
	{
		count.kiss = Mathf.Min(count.kiss + 1, 10000);
	}

	public void AddNotKiss()
	{
		count.notKiss = Mathf.Min(count.notKiss + 1, 10000);
	}

	public void SetMouseAction(int _item, int _area, int _action)
	{
		if (count.mouseAction.GetLength(0) <= _item || count.mouseAction.GetLength(1) <= _area)
		{
			return;
		}
		if (_action == 0)
		{
			if (!count.mouseAction[_item, _area].click)
			{
				count.mouseAction[_item, _area].click = true;
			}
		}
		else if (!count.mouseAction[_item, _area].drag)
		{
			count.mouseAction[_item, _area].drag = true;
		}
	}

	public void AddNotPowerful()
	{
		count.notPowerful = Mathf.Min(count.notPowerful + 1, 10000);
	}

	public void AddDontTouchAnal()
	{
		count.dontTouchAnal = Mathf.Min(count.dontTouchAnal + 1, 10000);
	}

	public void AddDontTouchMassage()
	{
		count.dontTouchMassage = Mathf.Min(count.dontTouchMassage + 1, 10000);
	}

	public void AddSelectMassage(float _pointAdd, int _area, bool _isCheck = false)
	{
		if (!_isCheck || !(count.selectMassages[_area] >= 0.01f))
		{
			count.selectMassages[_area] = Mathf.Min(count.selectMassages[_area] + _pointAdd, 30f);
		}
	}

	public void SetSelectArea(int _area, float _pointAdd, bool _isCheck = false)
	{
		if (count.selectAreas.Length > _area && (!_isCheck || !(count.selectAreas[_area] >= 0.01f)))
		{
			count.selectAreas[_area] = Mathf.Min(count.selectAreas[_area] + _pointAdd, 1000f);
			count.selectAreasGlobal[_area] = Mathf.Min(count.selectAreasGlobal[_area] + _pointAdd, float.MaxValue);
		}
	}

	public void SetSelectHobby(float _pointAdd)
	{
		count.selectHobby = Mathf.Min(count.selectHobby + _pointAdd * 0.01f, float.MaxValue);
	}

	public int IsAreaTouchAll()
	{
		if (count.selectAreas.Any((float a) => a > 0.01f))
		{
			return 2;
		}
		if (count.selectAreas.Any((float a) => a < 0.01f))
		{
			return 0;
		}
		return 1;
	}

	public void AddHoushiOutside()
	{
		count.houshiOutside = Mathf.Min(count.houshiOutside + 1, 10000);
	}

	public void AddHoushiInside()
	{
		count.houshiInside = Mathf.Min(count.houshiInside + 1, 10000);
	}

	public void AddHoushiDrink()
	{
		count.houshiDrink = Mathf.Min(count.houshiDrink + 1, 10000);
	}

	public void AddHoushiVomit()
	{
		count.houshiVomit = Mathf.Min(count.houshiVomit + 1, 10000);
	}

	public void AddSplash()
	{
		count.splash = Mathf.Min(count.splash + 1, 10000);
	}

	public void AddSonyuOrg()
	{
		count.sonyuOrg = Mathf.Min(count.sonyuOrg + 1, 10000);
	}

	public void AddSonyuInside()
	{
		count.sonyuInside = Mathf.Min(count.sonyuInside + 1, 10000);
	}

	public void AddSonyuOutside()
	{
		count.sonyuOutside = Mathf.Min(count.sonyuOutside + 1, 10000);
	}

	public void AddSonyuSame()
	{
		count.sonyuSame = Mathf.Min(count.sonyuSame + 1, 10000);
	}

	public void AddSonyuAnalOrg()
	{
		count.sonyuAnalOrg = Mathf.Min(count.sonyuAnalOrg + 1, 10000);
	}

	public void AddSonyuAnalInside()
	{
		count.sonyuAnalInside = Mathf.Min(count.sonyuAnalInside + 1, 10000);
	}

	public void AddSonyuAnalOutside()
	{
		count.sonyuAnalOutside = Mathf.Min(count.sonyuAnalOutside + 1, 10000);
	}

	public void AddSonyuAnalSame()
	{
		count.sonyuAnalSame = Mathf.Min(count.sonyuAnalSame + 1, 10000);
	}

	public void AddSonyuTare()
	{
		count.sonyuTare = Mathf.Min(count.sonyuTare + 1, 10000);
	}

	public void AddSonyuAnalTare()
	{
		count.sonyuAnalTare = Mathf.Min(count.sonyuAnalTare + 1, 10000);
	}

	public void AddNotCondomPlay()
	{
		count.notCondomPlay = Mathf.Min(count.notCondomPlay + 1, 10000);
	}

	public void AddNotAnalPlay()
	{
		count.notAnalPlay = Mathf.Min(count.notAnalPlay + 1, 10000);
	}

	public void SetInsertKokan()
	{
		if (!count.isInsertKokan)
		{
			count.isInsertKokan = true;
		}
	}

	public void SetInsertAnal()
	{
		if (!count.isInsertAnal)
		{
			count.isInsertAnal = true;
		}
	}

	public void SetInsertKokanVoiceCondition()
	{
		if (!count.isInsertKokanVoiceCondition)
		{
			count.isInsertKokanVoiceCondition = true;
		}
	}

	public void SetInsertAnalVoiceCondition()
	{
		if (!count.isInsertAnalVoiceCondition)
		{
			count.isInsertAnalVoiceCondition = true;
		}
	}

	public void SetNameInsert()
	{
		if (!count.isNameInsertKokan)
		{
			count.isNameInsertKokan = true;
		}
	}

	public void SetHoushiPlay()
	{
		if (!count.isHoushiPlay)
		{
			count.isHoushiPlay = true;
		}
	}

	public void AddSonyuKokanPlay()
	{
		count.sonyuKokanPlay = Mathf.Min(count.sonyuKokanPlay + 1, 10000);
	}

	public void AddSonyuAnalPlay()
	{
		count.sonyuAnalPlay = Mathf.Min(count.sonyuAnalPlay + 1, 10000);
	}

	public void AddHandFinsh()
	{
		count.handFinish = Mathf.Min(count.handFinish + 1, 10000);
	}

	public void AddNameFinish()
	{
		count.nameFinish = Mathf.Min(count.nameFinish + 1, 10000);
	}

	public void AddKuwaeFinish()
	{
		count.kuwaeFinish = Mathf.Min(count.kuwaeFinish + 1, 10000);
	}

	public void AddPaizuriFinish()
	{
		count.paizuriFinish = Mathf.Min(count.paizuriFinish + 1, 10000);
	}

	public void AddPaizurinameiFinish()
	{
		count.paizurinameFinish = Mathf.Min(count.paizurinameFinish + 1, 10000);
	}

	public void AddPaizurikuwaeFinish()
	{
		count.paizurikuwaeFinish = Mathf.Min(count.paizurikuwaeFinish + 1, 10000);
	}

	public void AddSonyuCondomInside()
	{
		count.sonyuCondomInside = Mathf.Min(count.sonyuCondomInside + 1, 10000);
	}

	public void AddSonyuCondomSame()
	{
		count.sonyuCondomSame = Mathf.Min(count.sonyuCondomSame + 1, 10000);
	}

	public void AddSonyuAnalCondomInside()
	{
		count.sonyuAnalCondomInside = Mathf.Min(count.sonyuAnalCondomInside + 1, 10000);
	}

	public void AddSonyuAnalCondomSame()
	{
		count.sonyuAnalCondomSame = Mathf.Min(count.sonyuAnalCondomSame + 1, 10000);
	}

	public int GetInsideAndOutsideCount()
	{
		return count.houshiOutside + count.houshiInside + count.sonyuCondomInside + count.sonyuInside + count.sonyuOutside + count.sonyuAnalCondomInside + count.sonyuAnalInside + count.sonyuAnalOutside;
	}

	public int GetOrgCount()
	{
		return count.aibuOrg + count.sonyuOrg + count.sonyuAnalOrg;
	}

	public static MenstruationType GetMenstruation(byte _day)
	{
		if (menstruations.Length <= _day)
		{
			return MenstruationType.安全日;
		}
		if (menstruations[_day] == 0)
		{
			return MenstruationType.安全日;
		}
		if (menstruations[_day] == 1)
		{
			return MenstruationType.危険日;
		}
		return MenstruationType.安全日;
	}

	public static void SetMenstruation(SaveData.Heroine _heroine, MenstruationType _type)
	{
		while (GetMenstruation(_heroine.MenstruationDay) != _type)
		{
			_heroine.AddMenstruationsDay();
		}
	}

	public bool IsNamaInsertOK()
	{
		return NamaInsertCheck(lstHeroine[0]);
	}

	public static bool NamaInsertCheck(SaveData.Heroine heroine)
	{
		if (heroine.denial.notCondom)
		{
			return true;
		}
		bool flag = GetMenstruation(heroine.MenstruationDay) == MenstruationType.安全日 || heroine.countNamaInsert >= 5;
		if (heroine.fixCharaID < 0 && heroine.HExperience == SaveData.Heroine.HExperienceKind.慣れ && flag)
		{
			return true;
		}
		if (heroine.HExperience == SaveData.Heroine.HExperienceKind.慣れ && ((heroine.relation == 1 && heroine.favor >= 80) || heroine.relation == 2) && flag)
		{
			return true;
		}
		if (heroine.HExperience == SaveData.Heroine.HExperienceKind.淫乱)
		{
			if (flag)
			{
				return true;
			}
			if (heroine.countNamaInsert >= 3)
			{
				return true;
			}
		}
		return false;
	}
}
