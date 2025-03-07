using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ADV;
using ActionGame.Chara.Backup;
using ActionGame.Chara.Mover;
using ActionGame.Place;
using Illusion;
using Illusion.Component;
using Illusion.Extensions;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara
{
	public sealed class NPC : Base, IHitable
	{
		public enum ActIcon
		{
			NoActiveTalkOK = -2,
			NoActive = -1,
			TalkOK = 0,
			Sleep = 1,
			PleaseTalk = 2,
			PleaseTalkActive = 3,
			PleaseH = 4,
			PleaseHActive = 5,
			Anger = 6
		}

		public const string TalkState = "talk";

		public const string ListenState = "talk_uke";

		private const string height1 = "height1";

		private const string breast = "Breast";

		private const string breast1 = "Breast1";

		public const string HesitantlyWaitState = "hesitantly";

		private static IDisposable summoningDisposable;

		private static IDisposable summoningTimeOutDisposable;

		private static bool isFailed;

		private const float summoningTimeLimit = 60f;

		private bool _isPause;

		private BoolReactiveProperty _isPlayerAction = new BoolReactiveProperty();

		[SerializeField]
		private SpritesAnim _spritesAnim;

		[SerializeField]
		private float _inSightDistance = 10f;

		[SerializeField]
		private TriggerEnterExitEvent sightArea;

		private bool isPhysicsInSight;

		[SerializeField]
		private bl_MiniMapItem miniMapIcon;

		[SerializeField]
		private SpeakChara _speak;

		[SerializeField]
		private TriggerEnterExitEvent noticeArea;

		private bool _isDefenseSpeak;

		private HashSet<Area> _placeAreaList = new HashSet<Area>();

		private MotionVoice motionVoice = new MotionVoice();

		private BoolReactiveProperty _isLesH = new BoolReactiveProperty(false);

		public override bool isAction
		{
			get
			{
				ActIcon actIcon = actIconNo;
				return actIcon != ActIcon.NoActive && actIcon != ActIcon.Sleep;
			}
		}

		public bool isPause
		{
			get
			{
				return _isPause;
			}
			set
			{
				_isPause = value;
			}
		}

		public bool isPlayerAction
		{
			get
			{
				return _isPlayerAction.Value;
			}
			set
			{
				_isPlayerAction.Value = value;
			}
		}

		public bool isEasyPlace
		{
			get
			{
				if (base.actScene.Map.Info.isOutdoors)
				{
					return true;
				}
				float radius = 1f;
				return otherPeopleList.Any((Base p) => (p.position - base.actScene.Player.position).sqrMagnitude < radius * radius);
			}
		}

		public bool isOtherPeople
		{
			get
			{
				return otherPeopleList.Any();
			}
		}

		public List<Base> otherPeopleList
		{
			get
			{
				List<Base> list = new List<Base>();
				foreach (NPC item in from p in base.actScene.npcList
					where p != this
					where p.mapNo == base.mapNo
					select p)
				{
					list.Add(item);
				}
				Fix fixChara = base.actScene.fixChara;
				if (fixChara != null && fixChara.isCharaLoad && fixChara.mapNo == base.mapNo)
				{
					list.Add(fixChara);
				}
				return list;
			}
		}

		public override bool isBodyForTarget
		{
			get
			{
				if (AI.isPatrol)
				{
					return false;
				}
				return base.isBodyForTarget;
			}
		}

		public SpeakChara speak
		{
			get
			{
				return _speak;
			}
		}

		public Vector3 calcPosition { get; set; }

		public UnityEngine.AI.NavMeshAgent agent { get; private set; }

		public ActionGame.Chara.Backup.NavMeshAgent bkAgent { get; private set; }

		public override Vector3 position
		{
			get
			{
				return base.cachedTransform.localPosition;
			}
			set
			{
				Transform obj = base.cachedTransform;
				Vector3 localPosition = (calcPosition = value);
				obj.localPosition = localPosition;
				agent.nextPosition = value;
			}
		}

		public AI AI { get; private set; }

		public Vector3[] corners { get; set; }

		public ActIcon actIconNo
		{
			get
			{
				ActIcon actIcon = ActIcon.NoActive;
				switch (AI.actionNo)
				{
				case -1:
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 6:
				case 7:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 18:
				case 19:
				case 21:
				case 25:
					actIcon = ActIcon.TalkOK;
					break;
				case 22:
					actIcon = (base.isArrival ? ActIcon.Sleep : ActIcon.TalkOK);
					break;
				case 5:
				case 8:
				case 23:
				case 24:
					actIcon = (AI.isArrival ? ActIcon.NoActive : ActIcon.TalkOK);
					break;
				case 26:
					actIcon = ((base.isArrival && base.motion.state == base.wpData.motion.motion) ? ActIcon.NoActive : ActIcon.TalkOK);
					break;
				case 27:
					actIcon = (base.isArrival ? ActIcon.NoActive : ActIcon.TalkOK);
					break;
				case 28:
					actIcon = (_isPlayerAction.Value ? ActIcon.PleaseTalkActive : ActIcon.PleaseTalk);
					break;
				case 29:
					actIcon = (_isPlayerAction.Value ? ActIcon.PleaseHActive : ActIcon.PleaseH);
					break;
				case 30:
					actIcon = ActIcon.NoActiveTalkOK;
					break;
				}
				if (actIcon != ActIcon.NoActive && actIcon != ActIcon.Sleep)
				{
					if (base.motion.state.CompareParts("sleep", true))
					{
						actIcon = ActIcon.Sleep;
					}
					else if (base.heroine.isAnger)
					{
						actIcon = ActIcon.Anger;
					}
				}
				return actIcon;
			}
		}

		public override bool isLookForTarget
		{
			get
			{
				if (base.heroine.isAnger || (AI.isArrival && AI.ActionNoCheck(4, 27)))
				{
					return false;
				}
				return base.isLookForTarget;
			}
		}

		public bool isOnanism
		{
			get
			{
				return AI.isArrival && AI.actionNo == 4;
			}
		}

		public bool isOnanismInSight { get; private set; }

		public bool isLesbian
		{
			get
			{
				return AI.actionNo == 27 && base.isArrival;
			}
		}

		public bool isDefenseSpeak
		{
			get
			{
				return _isDefenseSpeak;
			}
			set
			{
				_isDefenseSpeak = value;
			}
		}

		public bool isSightInPlayer
		{
			get
			{
				if (!isPhysicsInSight && !sightArea.isHit)
				{
					return false;
				}
				RaycastHit[] array = InSightHits();
				return array.Any() && array[0].collider.CompareTag("Player");
			}
		}

		public bool isNotice
		{
			get
			{
				return noticeArea.isHit;
			}
		}

		public string[] PlaceNames
		{
			get
			{
				return _placeAreaList.Where((Area p) => p != null).SelectMany((Area p) => p.names).Concat(new string[1] { base.actScene.Map.Info.MapName })
					.Distinct()
					.ToArray();
			}
		}

		public bool isLesH
		{
			get
			{
				return _isLesH.Value;
			}
		}

		public bool isTalking
		{
			get
			{
				return new string[3] { "talk", "talk_uke", "hesitantly" }.Contains(base.motion.state);
			}
		}

		public static NPC Create(Transform parent, SaveData.Heroine heroine)
		{
			GameObject asset = AssetBundleManager.LoadAsset("action/chara.unity3d", "NPC", typeof(GameObject)).GetAsset<GameObject>();
			NPC component = UnityEngine.Object.Instantiate(asset, parent, false).GetComponent<NPC>();
			component.charaData = heroine;
			component.name = GetObjectName(heroine);
			component.gameObject.SetActive(true);
			return component;
		}

		public static NPC Summon(SaveData.Heroine heroine, ActionScene actScene, int? mapNo = null)
		{
			Action<NPC> summoning = delegate(NPC npc)
			{
				isFailed = false;
				if (summoningDisposable != null)
				{
					summoningDisposable.Dispose();
				}
				if (summoningTimeOutDisposable != null)
				{
					summoningTimeOutDisposable.Dispose();
				}
				actScene.actionChangeUI.Set(ActionChangeUI.ActionType.SummonEvent);
				summoningTimeOutDisposable = (from _ in npc.UpdateAsObservable()
					where false
					select _).Timeout(TimeSpan.FromSeconds(60.0)).Catch((TimeoutException ex) => Observable.Empty<Unit>()).Take(1)
					.Subscribe((Action<Unit>)delegate
					{
					}, (Action)delegate
					{
						if (actScene != null && actScene.Player.isActionNow)
						{
							npc.AI.NextActionNoTarget();
							actScene.Player.isActionNow = false;
						}
						isFailed = true;
					});
				summoningDisposable = npc.UpdateAsObservable().TakeUntilDestroy(actScene).TakeWhile((Unit _) => npc.AI.actionNo == 24 && !isFailed)
					.Subscribe((Action<Unit>)delegate
					{
						actScene.ShortcutKeyRegulate(false);
						Vector3 vector = npc.position;
						actScene.CameraState.SetAngle(Quaternion.LookRotation(vector - actScene.Player.position).eulerAngles);
					}, (Action)delegate
					{
						if (actScene != null)
						{
							if (!isFailed && summoningTimeOutDisposable != null)
							{
								summoningTimeOutDisposable.Dispose();
							}
							actScene.actionChangeUI.Remove(ActionChangeUI.ActionType.SummonEvent);
							actScene.ShortcutKeyRegulate(true);
						}
					});
			};
			actScene.Player.ChaserEnd();
			actScene.Player.isActionNow = true;
			NPC target = heroine.charaBase as NPC;
			if (target == null)
			{
				target = Create(actScene.transform, heroine);
				target.gameObject.SetActive(true);
				target.LoadStart(true);
				actScene.npcList.Add(target);
				Observable.FromCoroutine((CancellationToken _) => new WaitUntil(() => target.initialized)).TakeUntilDestroy(target).Subscribe(delegate
				{
					target.isPopOK = true;
					target.ItemClear();
					target.AI.FirstAction();
					actScene.Map.PlayerMapNearWarp(target);
					target.ReStart();
					target.AI.TogetherAction(mapNo);
					target.AI.GotoTarget(actScene.Player);
					summoning(target);
				});
			}
			else
			{
				if (!target.initialized || target.AI.result == null || target.AI.actionNo == 24)
				{
					return target;
				}
				if (target.mapNo != actScene.Player.mapNo)
				{
					actScene.Map.PlayerMapNearWarp(target);
				}
				target.AI.TogetherAction(mapNo);
				target.AI.GotoTarget(actScene.Player);
				summoning(target);
			}
			return target;
		}

		private static string GetObjectName(SaveData.Heroine heroine)
		{
			return string.Format("NPC({0})_{1}_{2}", heroine.FixCharaIDOrPersonality, Cycle.GetClassRoomName(heroine.schoolClass), heroine.schoolClassIndex);
		}

		public override void PlayAnimation()
		{
			if (AI.actionNo == 27 && AI.isArrival && base.isActive)
			{
				if (base.actScene != null && base.actScene.Player != null && base.actScene.Player.isLesMotionPlay)
				{
					if (AI.target != null && AI.target.isArrival && otherPeopleList.Count == 1)
					{
						base.chaCtrl.hideMoz = false;
						base.chaCtrl.ChangeToiletStateLowPoly();
						base.wpData = AI.target.wpData;
						Vector3 pos = base.wpData.wp.position + base.wpData.motion.offsetPos;
						Vector3 ang = base.wpData.wp.angle + base.wpData.motion.offsetAngle;
						SetPositionAndRotation(pos, ang);
						WaitPoint.Parameter.Motion motion = base.wpData.motion;
						base.motion.state = motion.motion + "_sub";
						base.state = motion.state;
						ItemClear();
						motion.ItemSet(base.itemObjList, base.chaCtrl);
						SetParameterHeightAnimation();
						LesParam();
						base.animator.SetFloat("height1", AI.target.Height);
						base.animator.SetFloat("Breast1", AI.target.BustSize);
						base.motion.normalizedTime = 0f;
						base.motion.Play(base.chaCtrl.animBody);
						return;
					}
				}
				else if (base.isArrival)
				{
					AI.TimeEnd();
				}
			}
			ItemClear();
			ResetKind();
			if (base.isActive && base.isArrival && AI.target == null)
			{
				bool isNormalMotion = true;
				NPC target = null;
				if (AI.actionNo == 26)
				{
					isNormalMotion = false;
					if (LesMotion(out target))
					{
						return;
					}
				}
				PlayAnimationItemKindSet(isNormalMotion);
				if (AI.actionNo == 4 && base.motion.state == "Masturbation4")
				{
					foreach (GameObject itemObj in base.itemObjList)
					{
						if (itemObj.name == "p_item_hardle")
						{
							Animator component = itemObj.GetComponent<Animator>();
							if (component != null)
							{
								component.SetFloat(Base.hashHeight, base.Height);
							}
						}
					}
				}
				if (target != null && isBodyForTarget)
				{
					base.motion.state = "talk_uke";
				}
			}
			else
			{
				base.move.AnimeUpdate();
				base.state = 0;
			}
			SetParameterHeightAnimation();
			if (isTalking)
			{
				ItemClear();
				base.motion.normalizedTime = 0f;
			}
			else
			{
				base.motion.normalizedTime = Mathf.Min(UnityEngine.Random.value, 0.9f);
			}
			base.motion.Play(base.chaCtrl.animBody);
			UpdateMotionSpeed();
		}

		public override void LoadAnimator()
		{
			CharaInfo.Param param = base.actScene.charaInfoDic[base.charaData.personality];
			base.motion.bundle = param.MotionBundle;
			base.motion.asset = param.MotionAsset;
			base.motion.LoadAnimator(base.animator);
		}

		public override void ChangeNowCoordinate()
		{
			base.chaCtrl.ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)base.heroine.coordinates.SafeGet(0));
		}

		public override void Replace(SaveData.CharaData charaData)
		{
			base.Replace(charaData);
			base.name = GetObjectName(base.heroine);
			miniMapIcon.iconItem.text = charaData.Name;
			motionVoice.Init(base.heroine);
		}

		public void ReStart()
		{
			if (base.heroine.coordinates.SafeGet(0) != base.chaCtrl.fileStatus.coordinateType)
			{
				ChangeNowCoordinate();
			}
			SetActive(base.actScene.Map.IsPop(this));
			AI.Start();
		}

		public void Pause(bool isPause)
		{
			if (isPause)
			{
				if (AI.isRunning)
				{
					_isPause = true;
					AI.Pause();
				}
			}
			else if (_isPause)
			{
				AI.Start();
				_isPause = false;
			}
		}

		public void Stop()
		{
			SetActive(false);
			AI.Reset(true);
			base.move.animParamSpeed = 0f;
		}

		public void SynchroCoordinate(bool isRemove)
		{
			if (!(base.chaCtrl == null) && Singleton<Character>.IsInstance())
			{
				int nowCoordinate = base.heroine.NowCoordinate;
				int coordinateType = base.chaCtrl.fileStatus.coordinateType;
				if (coordinateType != nowCoordinate)
				{
					Singleton<Character>.Instance.enableCharaLoadGCClear = false;
					base.chaCtrl.ChangeCoordinateTypeAndReload((ChaFileDefine.CoordinateType)nowCoordinate);
					Singleton<Character>.Instance.enableCharaLoadGCClear = true;
				}
				if (isRemove)
				{
					base.chaCtrl.RandomChangeOfClothesLowPoly(base.heroine.lewdness);
				}
			}
		}

		public void Backup(UnityEngine.AI.NavMeshAgent agent)
		{
			bkAgent = new ActionGame.Chara.Backup.NavMeshAgent(agent);
		}

		void IHitable.Enter(Area area)
		{
			_placeAreaList.Add(area);
		}

		void IHitable.Exit(Area area)
		{
			_placeAreaList.Remove(area);
		}

		public void Speak(string bundleName, string assetName)
		{
			if (bundleName != null)
			{
				if (speak.isOpen)
				{
					speak.Close();
				}
				int result;
				if (int.TryParse(assetName, out result))
				{
					base.heroine.talkEvent.Add(result);
				}
				speak.bundleName = bundleName;
				speak.assetName = assetName;
				speak.Open();
			}
		}

		public void UpdateMotionSpeed()
		{
			if (!base.animator.gameObject.activeInHierarchy)
			{
				return;
			}
			float value = 1f;
			if (base.heroine.motionSpeed.TryGetValue(base.motion.state, out value))
			{
				bool flag = Singleton<Manager.Communication>.IsInstance();
				if (flag && Singleton<Manager.Communication>.Instance.CheckMoveMotion(base.motion.state, base.heroine.personality))
				{
					float shapeBodyValue = base.heroine.chaCtrl.GetShapeBodyValue(0);
					value *= ((!(shapeBodyValue >= 0.5f)) ? Mathf.Lerp(0.8f, 1f, Mathf.InverseLerp(0f, 0.5f, shapeBodyValue)) : Mathf.Lerp(1f, 1.2f, Mathf.InverseLerp(0.5f, 1f, shapeBodyValue)));
				}
				base.animator.SetFloat("MotionSpeed", value);
				if (flag && Singleton<Manager.Communication>.Instance.CheckChangeMoveSpeed(base.motion.state, base.heroine.personality) && base.heroine.motionSpeed.TryGetValue(base.motion.state, out value))
				{
					float shapeBodyValue2 = base.chaCtrl.GetShapeBodyValue(0);
					value *= ((!(shapeBodyValue2 >= 0.5f)) ? Mathf.Lerp(0.8f, 1f, Mathf.InverseLerp(0f, 0.5f, shapeBodyValue2)) : Mathf.Lerp(1f, 1.2f, Mathf.InverseLerp(0.5f, 1f, shapeBodyValue2)));
					AI.speeder.walkRev = value;
					AI.speeder.runRev = value;
				}
			}
		}

		public int GetOnanismID(bool isMotion, bool isCheck = true)
		{
			if (isCheck && !isOnanism)
			{
				return -1;
			}
			switch (isMotion ? base.motion.state : GetOnanismMotion())
			{
			case "Masturbation":
				return 1012;
			case "Masturbation2":
				return 1011;
			case "Masturbation3":
				return 1010;
			case "Masturbation4":
				return 1013;
			default:
				return 1012;
			}
		}

		public string GetOnanismMotion()
		{
			switch (base.state)
			{
			case 1:
				return "Masturbation2";
			case 6:
				return "Masturbation3";
			case 7:
				return "Masturbation4";
			default:
				return "Masturbation";
			}
		}

		public void LesHEndForce()
		{
			_isLesH.Value = false;
		}

		public int GetLesbianID(bool isMotion, bool isCheck = true)
		{
			if (isCheck && !isLesbian)
			{
				return -1;
			}
			string text = (isMotion ? base.motion.state : GetLesbianMotion());
			switch (text.Replace("_sub", string.Empty))
			{
			case "Lesbian":
				return 1100;
			case "Lesbian2":
				return 1101;
			case "Lesbian3":
				return 1102;
			default:
				return 1100;
			}
		}

		public string GetLesbianMotion()
		{
			switch (base.state)
			{
			case 1:
				return "Lesbian2";
			case 2:
				return "Lesbian";
			default:
				return "Lesbian3";
			}
		}

		public void SetTalker()
		{
			base.motion.state = "talk";
			base.SetWaitPoint(null);
			corners = null;
		}

		public void SetListener()
		{
			base.motion.state = "talk_uke";
			if (AI.actionNo == 26)
			{
				PlayAnimation();
			}
			else
			{
				base.SetWaitPoint(null);
			}
			corners = null;
		}

		public void SetHesitantly()
		{
			base.motion.state = "hesitantly";
			base.SetWaitPoint(null);
			corners = null;
		}

		public override void SetWaitPoint(WaitPointData wpData)
		{
			base.SetWaitPoint(wpData);
			corners = null;
			AI.isArrival = wpData != null;
			if (base.isActive)
			{
				if (wpData != null && wpData.wp.isNavMeshOffsetPoint)
				{
					agent.enabled = false;
				}
				else
				{
					agent.enabled = true;
				}
			}
		}

		public void AgentVelocityMoveAnimeUpdate()
		{
			float animParamSpeed = base.move.animParamSpeed;
			base.move.animParamSpeed = agent.velocity.magnitude;
			if (Mathf.Approximately(animParamSpeed, base.move.animParamSpeed))
			{
				base.move.MoveUpdate();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			base.move = GetComponent<NPCMover>();
			Backup(agent);
			AI = new AI(this);
		}

		protected override IEnumerator Start()
		{
			yield return new WaitWhile(() => base.actScene.Player == null);
			yield return new WaitWhile(() => base.actScene.Player.chaCtrl == null);
			yield return new WaitUntil(() => base.actScene.Player.initialized);
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
			_mapNo.Subscribe(delegate
			{
				corners = null;
			});
			ActionChangeUI changeUI = base.actScene.actionChangeUI;
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where base.isActive
				select _).Subscribe(delegate
			{
				if (base.isArrival && AI.actionNo == 4 && base.motion.state == "Masturbation4")
				{
					base.chaCtrl.ChangeLookEyesPtn(0);
					base.chaCtrl.ChangeLookNeckPtn(0);
				}
				else
				{
					Base @base;
					if (_isPlayerAction.Value && base.heroine.relation >= 1)
					{
						@base = base.actScene.Player;
					}
					else
					{
						@base = (AI.isArrival ? AI.target : null);
						@base = @base ?? AI.TargetReceiver((AI p) => p.isArrival);
					}
					bool isForce = false;
					if (@base is NPC)
					{
						isForce = AI.actionNo == 26;
					}
					LookForTarget(@base, isForce);
				}
			});
			GameObject ag = _actionIcon.gameObject;
			Transform at = _actionIcon.transform;
			(from active in base.OnActiveChangeObservable.TakeUntilDestroy(base.actScene)
				where !active
				select active).Subscribe(delegate(bool active)
			{
				if (base.actScene.Player.actionTarget == this)
				{
					base.actScene.Player.actionTarget = null;
					if (isOnanismInSight)
					{
						changeUI.Remove(ActionChangeUI.ActionType.Onanism);
					}
				}
				ag.SetActiveIfDifferent(active);
			}).AddTo(disposables);
			_isPlayerAction.Subscribe(delegate(bool isOn)
			{
				ag.SetActiveIfDifferent(isOn);
				if (isOnanism && isOn)
				{
					changeUI.Set(ActionChangeUI.ActionType.Onanism);
				}
				if (!isOn)
				{
					changeUI.Remove(ActionChangeUI.ActionType.Onanism);
				}
			}).AddTo(disposables);
			(from active in base.OnActiveChangeObservable
				where !active
				where AI.isPlayerResWaiting
				select active).Subscribe(delegate
			{
				AI.TimeEnd();
			});
			(from _ in this.UpdateAsObservable()
				where base.isActive && !_isPlayerAction.Value
				select AI.isPlayerResWaiting).Subscribe(delegate(bool isOn)
			{
				ag.SetActiveIfDifferent(isOn);
			});
			(from _ in this.UpdateAsObservable()
				where ag.activeSelf
				select _).Subscribe(delegate
			{
				Vector3 headPos = base.HeadPos;
				headPos.y += 0.5f;
				at.position = headPos;
				int num = (int)actIconNo;
				int num2 = _spritesAnim.names.Length;
				if ((uint)num < num2)
				{
					_spritesAnim.isEnable = true;
					_spritesAnim.Play(num);
				}
				else
				{
					_spritesAnim.isEnable = false;
					_actionIcon.sprite = ((num >= 0) ? _sprites[num - num2] : null);
				}
			});
			_isPlayerAction.Subscribe(delegate(bool value)
			{
				if (AI.isTalkSuccess && !(AI.TargetReceiver((AI p) => p.isArrival) != null))
				{
					if (!value || base.heroine.isAnger || AI.isPatrol || base.actScene.Map.Info.isWarning || (base.heroine.talkTime <= 0 && Manager.Config.AddData.TalkTimeNoneWalkStop))
					{
						bool isStop = false;
						if (AI.isPlayerResWaiting)
						{
							isStop = true;
						}
						base.move.isStop = isStop;
						AI.Start();
					}
					else if (base.heroine.relation >= 1)
					{
						base.move.isStop = true;
						AI.Pause();
					}
				}
			});
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				if (active)
				{
					if (base.isArrival && base.wpData.wp.isNavMeshOffsetPoint)
					{
						agent.enabled = false;
					}
					else
					{
						agent.enabled = true;
					}
				}
				else
				{
					agent.enabled = false;
				}
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where base.isActive
				where agent.enabled
				where !agent.isOnNavMesh
				where !base.actScene.Map.isMapLoading
				select _).Subscribe(delegate
			{
				agent.enabled = false;
				agent.enabled = true;
				AI.isOnNavMeshReset = true;
			});
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				if (active)
				{
					position = calcPosition;
				}
				else
				{
					calcPosition = position;
				}
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Subscribe(delegate
			{
				SynchroCoordinate(false);
			}).AddTo(disposables);
			yield return new WaitWhile(() => miniMapIcon.iconItem == null);
			miniMapIcon.iconItem.text = base.charaData.Name;
			Sprites minimapIconSprites = GetComponents<Sprites>()[1];
			(from _ in this.UpdateAsObservable()
				where base.heroine != null
				select base.heroine.relation into relation
				select Mathf.Max(0, relation)).DistinctUntilChanged().Subscribe(delegate(int index)
			{
				miniMapIcon.iconItem.SetIcon(minimapIconSprites[index]);
			});
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				miniMapIcon.SetVisible(active);
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				speak.Close();
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				sightArea.enabled = active;
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				noticeArea.enabled = active;
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				isPhysicsInSight = false;
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				isOnanismInSight = false;
			}).AddTo(disposables);
			(from _ in base.OnActiveChangeObservable.TakeUntilDestroy(base.actScene)
				where isOnanism || isLesbian
				select _).Subscribe(delegate(bool active)
			{
				base.actScene.OtherAIPause(this, active);
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				_isDefenseSpeak = false;
			}).AddTo(disposables);
			base.actScene.VisibleList.Add(base.chaCtrl);
			Transform sightAreaTransform = sightArea.transform;
			(from _ in sightArea.UpdateAsObservable()
				where sightArea.enabled
				select _).Subscribe(delegate
			{
				Transform head = base.Head;
				if (head != null)
				{
					Vector3 vector = head.position;
					vector.y -= sightAreaTransform.localScale.y;
					sightAreaTransform.SetPositionAndRotation(vector, head.rotation);
				}
				else
				{
					sightAreaTransform.SetPositionAndRotation(Vector3.zero, base.rotation);
				}
			});
			motionVoice.Init(base.heroine);
			(from active in base.OnActiveChangeObservable
				where !active
				select active into _
				where Singleton<Voice>.IsInstance()
				select _).Subscribe(delegate
			{
				Singleton<Voice>.Instance.Stop(base.Head);
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable()
				where base.isActive
				where !Singleton<Game>.Instance.IsRegulate(true)
				where !Program.isADVProcessing
				where !speak.isOpen
				select _).Subscribe(delegate
			{
				motionVoice.Proc();
			});
			(from _ in this.UpdateAsObservable()
				where base.isActive
				where AI.isArrival
				where !base.actScene.isEventNow
				select _).Subscribe(delegate
			{
				switch (AI.actionNo)
				{
				case 4:
					if (isOtherPeople)
					{
						changeUI.Remove(ActionChangeUI.ActionType.Onanism);
						AI.NextActionNoTarget();
					}
					else if (isSightInPlayer)
					{
						isPhysicsInSight = true;
						if (!isOnanismInSight)
						{
							isOnanismInSight = true;
							if (base.heroine.HExperience <= SaveData.Heroine.HExperienceKind.不慣れ || base.heroine.lewdness < 80)
							{
								motionVoice.actionKind = MotionVoice.ActionKind.masturbation_found;
								changeUI.Remove(ActionChangeUI.ActionType.Onanism);
								AI.EscapeAction(false);
							}
						}
					}
					break;
				case 26:
					if (base.motion.state == base.wpData.motion.motion)
					{
						bool flag = false;
						foreach (Base otherPeople in otherPeopleList)
						{
							if (otherPeople is Fix)
							{
								flag = true;
								break;
							}
							NPC nPC = otherPeople as NPC;
							if (nPC.AI.actionNo != 27 || nPC.AI.target != this)
							{
								flag = true;
								break;
							}
						}
						if (flag)
						{
							NPC nPC2 = AI.TargetReceiver((AI p) => p.actionNo == 27);
							if (nPC2 != null)
							{
								nPC2.AI.SetLesbianDesire(AI.LesbianResult.Success);
								nPC2.AI.NextActionNoTarget();
							}
							AI.SetLesbianDesire(AI.LesbianResult.Success);
							AI.NextActionNoTarget();
						}
						else if (isSightInPlayer)
						{
							isPhysicsInSight = true;
							NPC nPC3 = AI.TargetReceiver((AI p) => p.actionNo == 27);
							if (nPC3 != null)
							{
								nPC3.AI.EscapeAction(false);
							}
							AI.EscapeAction(false);
						}
					}
					break;
				case 27:
					if (base.isArrival)
					{
						if (otherPeopleList.Any((Base p) => p != AI.target))
						{
							AI aI = (AI.target as NPC).AI;
							aI.SetLesbianDesire(AI.LesbianResult.Success);
							AI.SetLesbianDesire(AI.LesbianResult.Success);
							aI.NextActionNoTarget();
							AI.NextActionNoTarget();
						}
						else if (isSightInPlayer)
						{
							isPhysicsInSight = true;
							(AI.target as NPC).AI.EscapeAction(false);
							AI.EscapeAction(false);
						}
					}
					break;
				}
			});
			int defenseNo = 25;
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where !base.actScene.isEventNow
				where base.actScene.Map.Info.isWarning
				where !_isDefenseSpeak
				where base.isActive
				where !AI.isArrival || AI.actionNo != 4
				where AI.actionNo != 26 || !base.isArrival || !(base.wpData.motion.motion == base.motion.state)
				where AI.actionNo != 27 || !base.isArrival
				where isAction
				where !AI.ActionNoCheck(20, 23, defenseNo)
				where !AI.isPlayerResWaiting
				select _).Subscribe(delegate
			{
				if (isSightInPlayer)
				{
					isPhysicsInSight = true;
					AI.ShyAction();
					int encounterADVNo = GetEncounterADVNo();
					Speak(Program.FindADVBundleFilePath(encounterADVNo, base.heroine), encounterADVNo.ToString());
					_isDefenseSpeak = true;
				}
			});
			(from _ in this.UpdateAsObservable()
				where base.isActive
				where AI.actionNo == defenseNo
				select _).Subscribe(delegate
			{
				base.move.isStop = true;
				Utils.Math.TargetFor(base.cachedTransform, base.actScene.Player.cachedTransform);
			});
			(from active in base.OnActiveChangeObservable
				where !active
				select active into _
				where AI.actionNo == defenseNo
				select _).Subscribe(delegate
			{
				AI.NextActionNoTarget();
			}).AddTo(disposables);
			_isLesH.Subscribe(delegate(bool isOn)
			{
				if (isOn)
				{
					changeUI.Set(ActionChangeUI.ActionType.Lesbian);
				}
				else
				{
					changeUI.Remove(ActionChangeUI.ActionType.Lesbian);
				}
			});
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				_isLesH.Value = false;
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where _isLesH.Value
				where ActionInput.isAction
				where !Singleton<Game>.Instance.IsRegulate(true)
				where !Program.isADVProcessing
				select _).Subscribe(delegate
			{
				base.actScene.SceneEvent(this);
			});
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(base.actScene)
				where base.isActive && isLesbian
				select Vector3.Distance(position, base.actScene.Player.position) < base.actScene.lesbianDistance).DistinctUntilChanged().Subscribe(delegate(bool isOn)
			{
				_isLesH.Value = isOn;
			});
			AI.ActionStart();
			base.initialized = true;
		}

		private void OnDrawGizmos()
		{
			if (!base.isActive || !(base.chaCtrl != null) || !(base.Head != null))
			{
				return;
			}
			if (isPhysicsInSight)
			{
				RaycastHit[] source = InSightHits();
				if (source.Any())
				{
					Gizmos.DrawRay(base.HeadPos, (base.actScene.Player.HeadPos - base.HeadPos).normalized * _inSightDistance);
				}
			}
			if (AI.actionNo == 27 && base.isArrival)
			{
				Color cyan = Color.cyan;
				cyan.a = 0.5f;
				Gizmos.color = cyan;
				Gizmos.DrawSphere(position, base.actScene.lesbianDistance);
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (base.actScene != null && base.actScene.VisibleList != null)
			{
				base.actScene.VisibleList.Remove(base.chaCtrl);
			}
		}

		public int GetEncounterADVNo()
		{
			if (base.heroine.isAnger)
			{
				return 36;
			}
			switch (base.heroine.relation)
			{
			case 2:
				if (isOtherPeople)
				{
					return 37;
				}
				switch (base.mapNo)
				{
				case 14:
				case 15:
				case 16:
					return 40;
				case 45:
					return 39;
				case 46:
					return 38;
				default:
					return 37;
				}
			case 1:
				return 37;
			default:
				return 35;
			}
		}

		private RaycastHit[] InSightHits()
		{
			Vector3 headPos = base.HeadPos;
			Vector3 headPos2 = base.actScene.Player.HeadPos;
			float num = Vector3.Dot(base.Head.forward, (headPos2 - headPos).normalized);
			if (num < 0f)
			{
				return new RaycastHit[0];
			}
			return (from p in Physics.RaycastAll(headPos, (headPos2 - headPos).normalized, _inSightDistance, 10240)
				where p.collider.gameObject.layer == 11 || p.collider.CompareTag("Player")
				orderby p.distance
				select p).ToArray();
		}

		private bool LesMotion(out NPC target)
		{
			target = AI.TargetReceiver((AI p) => p.actionNo == 27 && p.isArrival);
			if (target == null)
			{
				return false;
			}
			if (base.actScene == null || base.actScene.Player == null || !base.actScene.Player.isLesMotionPlay)
			{
				return false;
			}
			if (otherPeopleList.Count != 1)
			{
				return false;
			}
			Vector3 pos = base.wpData.wp.position + base.wpData.motion.offsetPos;
			Vector3 ang = base.wpData.wp.angle + base.wpData.motion.offsetAngle;
			SetPositionAndRotation(pos, ang);
			PlayAnimationItemKindSet(true);
			SetParameterHeightAnimation();
			LesParam();
			base.animator.SetFloat("height1", target.Height);
			base.animator.SetFloat("Breast1", target.BustSize);
			base.motion.normalizedTime = 0f;
			base.motion.Play(base.chaCtrl.animBody);
			base.chaCtrl.hideMoz = false;
			base.chaCtrl.ChangeToiletStateLowPoly();
			return true;
		}

		private void LesParam()
		{
			base.animator.SetFloat("Breast", base.BustSize);
			float weight = 0f;
			float weight2 = 0f;
			if (base.motion.state.IndexOf("Lesbian3") != -1)
			{
				if (base.motion.state.IndexOf("_sub") == -1)
				{
					weight = 1f;
					weight2 = 0f;
				}
				else
				{
					weight = 0f;
					weight2 = 1f;
				}
			}
			base.animator.SetLayerWeight(1, weight);
			base.animator.SetLayerWeight(2, weight2);
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private IEnumerator _003CStart_003E__BaseCallProxy0()
		{
			return base.Start();
		}
	}
}
