using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using ActionGame.Chara.Mover;
using ActionGame.Point;
using Illusion.Component;
using Illusion.Game.Elements.EasyLoader;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara
{
	public class Player : Base
	{
		private bool _isActionNow;

		private ReactiveProperty<Base> _actionTarget = new ReactiveProperty<Base>();

		[SerializeField]
		private TriggerEnterExitEvent noticeArea;

		private bool _isChaseWarning;

		private HashSet<Component> _actionPointList = new HashSet<Component>();

		[SerializeField]
		private List<Item.Data> itemList = new List<Item.Data>();

		private float height;

		public override bool isAction
		{
			get
			{
				return !_isActionNow && base.actScene.isCursorLock;
			}
		}

		public bool isActionNow
		{
			get
			{
				return _isActionNow || !base.actScene.isCursorLock;
			}
			set
			{
				_isActionNow = value;
			}
		}

		public bool isActionNow_Origin
		{
			get
			{
				return _isActionNow;
			}
		}

		public Base actionTarget
		{
			get
			{
				return _actionTarget.Value;
			}
			set
			{
				_actionTarget.Value = value;
			}
		}

		public NPC chaser { get; private set; }

		public bool isChaseWarning
		{
			get
			{
				bool result = _isChaseWarning;
				_isChaseWarning = false;
				return result;
			}
		}

		public bool isActionPointHit
		{
			get
			{
				return isActionPointToPlayerHit || isActionPointToMapHit;
			}
		}

		public bool isActionPointToPlayerHit
		{
			get
			{
				return _actionPointList.Where((Component o) => o != null).Any((Component o) => o.GetComponent<PlayerActionPoint>() != null);
			}
		}

		public bool isActionPointToMapHit
		{
			get
			{
				return (from o in _actionPointList
					where o != null
					select o.GetComponent<ActionPoint>() into o
					where o != null
					select o).Any((ActionPoint o) => o.isIconDraw);
			}
		}

		public HashSet<Component> actionPointList
		{
			get
			{
				return _actionPointList;
			}
		}

		public bool isLesMotionPlay { get; set; }

		public NavMeshAgent agent { get; private set; }

		public override Vector3 position
		{
			get
			{
				return base.cachedTransform.localPosition;
			}
			set
			{
				base.cachedTransform.localPosition = value;
				agent.nextPosition = value;
			}
		}

		public Vector3 fpsPos
		{
			get
			{
				Vector3 result = position;
				result.y += height;
				return result;
			}
		}

		public static Player Create(Transform parent, SaveData.Player player)
		{
			GameObject asset = AssetBundleManager.LoadAsset("action/chara.unity3d", "Player", typeof(GameObject)).GetAsset<GameObject>();
			Player component = Object.Instantiate(asset, parent, false).GetComponent<Player>();
			component.charaData = player;
			component.name = asset.name;
			component.gameObject.SetActive(true);
			return component;
		}

		public override void Replace(SaveData.CharaData charaData)
		{
			base.Replace(charaData);
		}

		public override void PlayAnimation()
		{
			base.move.AnimeUpdate();
			base.motion.Play(base.animator);
		}

		public override void LoadAnimator()
		{
			base.motion.bundle = "action/animator/00.unity3d";
			base.motion.asset = "player";
			base.motion.LoadAnimator(base.animator);
		}

		public override void ChangeNowCoordinate()
		{
			ChaFileDefine.CoordinateType type = ChaFileDefine.CoordinateType.School01;
			if (base.player.changeClothesType < 0)
			{
				bool flag = false;
				if (base.actScene.Cycle.nowWeek != Cycle.Week.Saturday)
				{
					switch (base.actScene.Cycle.nowType)
					{
					case Cycle.Type.LunchTime:
						switch (base.actScene.Cycle.GetNowLessones(base.charaData.schoolClass)[0])
						{
						case "グラウンド":
						case "体育館":
							type = ChaFileDefine.CoordinateType.Gym;
							break;
						case "プール":
							type = ChaFileDefine.CoordinateType.Swim;
							break;
						default:
							type = ChaFileDefine.CoordinateType.School01;
							break;
						}
						break;
					case Cycle.Type.StaffTime:
						type = ChaFileDefine.CoordinateType.Club;
						break;
					case Cycle.Type.AfterSchool:
						type = ChaFileDefine.CoordinateType.School02;
						break;
					}
				}
				else
				{
					switch (base.actScene.Cycle.nowType)
					{
					case Cycle.Type.LunchTime:
						type = ChaFileDefine.CoordinateType.Plain;
						break;
					case Cycle.Type.StaffTime:
						type = ChaFileDefine.CoordinateType.Club;
						break;
					case Cycle.Type.AfterSchool:
						type = ChaFileDefine.CoordinateType.Plain;
						break;
					}
				}
			}
			else
			{
				type = (ChaFileDefine.CoordinateType)base.player.changeClothesType;
			}
			base.chaCtrl.ChangeCoordinateTypeAndReload(type);
		}

		public void ChaserSet(NPC npc)
		{
			ChaserEnd();
			chaser = npc;
			npc.AI.Stop();
			npc.move.Change(new Chase(npc.move as NPCMover)
			{
				target = this
			});
			npc.AI.ChaseAction();
			npc.move.isStop = false;
		}

		public void ChaserEnd()
		{
			if (chaser != null)
			{
				chaser.move.Change(new None(chaser.move));
				chaser.AI.Start();
				chaser.AI.NextActionNoTarget();
			}
			chaser = null;
		}

		public bool isChaseCancel(int mapNo)
		{
			_isChaseWarning = false;
			if (chaser == null)
			{
				return false;
			}
			SaveData.Heroine heroine = chaser.heroine;
			if (heroine.relation <= 0)
			{
				_isChaseWarning = true;
				return true;
			}
			if (heroine.relation == 1)
			{
				List<Base> list = new List<Base>();
				foreach (NPC item in from p in base.actScene.npcList
					where p != chaser
					where p.mapNo == mapNo
					select p)
				{
					list.Add(item);
				}
				Fix fixChara = base.actScene.fixChara;
				if (fixChara != null && fixChara.isCharaLoad && fixChara.mapNo == mapNo)
				{
					list.Add(fixChara);
				}
				if (list.Any())
				{
					_isChaseWarning = true;
					return true;
				}
			}
			return false;
		}

		public void SetPhone()
		{
			ItemClear();
			string stateName = "call";
			base.motion.state = stateName;
			base.itemObjList.Add(itemList[0].Load(base.chaCtrl));
			base.motion.Play(base.animator);
			(from _ in this.UpdateAsObservable()
				where base.motion.state != stateName
				select _).Take(1).Subscribe(delegate
			{
				ItemClear();
			});
		}

		protected override void Awake()
		{
			base.Awake();
			agent = GetComponent<NavMeshAgent>();
			base.move = GetComponent<PlayerMover>();
		}

		protected override IEnumerator Start()
		{
			base.enabled = false;
			base.actScene.follow.followTarget = base.cachedTransform;
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				agent.enabled = active;
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => active).Subscribe(delegate
			{
				base.actScene.follow.Sync();
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Subscribe(delegate(bool active)
			{
				noticeArea.enabled = active;
			}).AddTo(disposables);
			(from active in base.OnActiveChangeObservable.TakeUntilDestroy(base.actScene)
				where !active
				select active).Subscribe(delegate
			{
				base.actScene.actionChangeUI.Set(ActionChangeUI.ActionType.None);
			}).AddTo(disposables);
			base.OnActiveChangeObservable.Where((bool active) => !active).Subscribe(delegate
			{
				_actionPointList.Clear();
			}).AddTo(disposables);
			noticeArea.onTriggerEnter += delegate(Collider collider)
			{
				Base component = collider.GetComponent<Base>();
				_actionTarget.Value = component;
			};
			noticeArea.onTriggerExit += delegate(Collider collider)
			{
				Base component2 = collider.GetComponent<Base>();
				if (_actionTarget.Value == component2)
				{
					_actionTarget.Value = null;
				}
				if (base.isActive && _actionTarget.Value == null && noticeArea.hitList.Any())
				{
					noticeArea.hitList.RemoveWhere((Collider p) => p == null);
					Base[] source = (from col in noticeArea.hitList
						select col.GetComponent<Base>() into p
						where p.isActive
						select p).ToArray();
					_actionTarget.Value = source.OrderBy((Base p) => (p.position - position).sqrMagnitude).FirstOrDefault();
				}
			};
			_actionTarget.Scan(delegate(Base prev, Base current)
			{
				if (prev is NPC)
				{
					(prev as NPC).isPlayerAction = false;
				}
				else if (prev is Fix)
				{
					(prev as Fix).isPlayerAction = false;
				}
				return current;
			}).Subscribe(delegate(Base target)
			{
				LookForTarget(target);
				if (target != null)
				{
					if (target is NPC)
					{
						(target as NPC).isPlayerAction = true;
					}
					else if (target is Fix)
					{
						(target as Fix).isPlayerAction = true;
					}
				}
				if (base.actScene != null)
				{
					base.actScene.paramUI.SetHeroine((!(target == null)) ? target.heroine : null);
				}
			}).AddTo(disposables);
			this.UpdateAsObservable().Subscribe(delegate
			{
				if (!isActionNow)
				{
					if (ActionInput.isCrouch)
					{
						base.move.agentSpeeder.mode = AgentSpeeder.Mode.Crouch;
					}
					else if (ActionInput.isWalk)
					{
						base.move.agentSpeeder.mode = AgentSpeeder.Mode.Walk;
					}
					else
					{
						base.move.agentSpeeder.mode = AgentSpeeder.Mode.Run;
					}
				}
			});
			height = base.Head.position.y - position.y;
			(from _ in this.LateUpdateAsObservable()
				select base.move.agentSpeeder.mode == AgentSpeeder.Mode.Crouch).DistinctUntilChanged().Subscribe(delegate
			{
				height = base.Head.position.y - position.y;
			});
			this.UpdateAsObservable().Subscribe(delegate
			{
				base.actScene.MiniMapTarget.SetPositionAndRotation(position, base.actScene.cameraTransform.rotation);
			});
			(from _ in this.UpdateAsObservable()
				where base.isActive
				where base.actScene.Map.no >= 0 && base.actScene.Map.Info.isWarning
				select _).Subscribe(delegate
			{
				Collider[] source2 = Physics.OverlapSphere(position, (base.baseCollider as CapsuleCollider).radius, 2048);
				foreach (Collider item in source2.Where((Collider p) => p.name.IndexOf("DoorCloseEvent") != -1))
				{
					if (!(item == null))
					{
						Door component3 = item.GetComponent<Door>();
						if (component3 != null)
						{
							if (component3.IsClose(null))
							{
								component3.OpenForce();
							}
							break;
						}
					}
				}
			});
			base.enabled = true;
			base.initialized = true;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (base.actScene != null)
			{
				ChaserEnd();
			}
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private IEnumerator _003CStart_003E__BaseCallProxy0()
		{
			return base.Start();
		}
	}
}
