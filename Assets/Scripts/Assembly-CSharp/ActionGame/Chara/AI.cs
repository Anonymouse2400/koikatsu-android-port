using System;
using System.Collections.Generic;
using System.Linq;
using ADV;
using ActionGame.Chara.Mover;
using ActionGame.Point;
using Illusion;
using Illusion.Extensions;
using Manager;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ActionGame.Chara
{
	public class AI
	{
		public enum LesbianResult
		{
			Success = 0,
			Half_Main = 1,
			Half_Sub = 2,
			Failed_Main = 3,
			Failed_Sub = 4
		}

		public class NodeCanvasAccesser
		{
			public enum Names
			{
				MapNo = 0,
				TargetMapNo = 1
			}

			private Variable[] _variables;

			public int mapNo
			{
				get
				{
					return (int)this[Names.MapNo].value;
				}
				set
				{
					this[Names.MapNo].value = value;
				}
			}

			public int TargetMapNo
			{
				get
				{
					return (int)this[Names.TargetMapNo].value;
				}
				set
				{
					this[Names.TargetMapNo].value = value;
				}
			}

			public Variable this[Names name]
			{
				get
				{
					return _variables[(int)name];
				}
			}

			public BehaviourTreeOwner bt { get; private set; }

			public NodeCanvasAccesser(BehaviourTreeOwner bt)
			{
				this.bt = bt;
				_variables = new Variable[Utils.Enum<Names>.Length];
				foreach (KeyValuePair<string, Variable> variable in bt.blackboard.variables)
				{
					if (Utils.Enum<Names>.Contains(variable.Key))
					{
						_variables[(int)Utils.Enum<Names>.Cast(variable.Key)] = variable.Value;
					}
				}
			}
		}

		private BoolReactiveProperty _isArrival = new BoolReactiveProperty(false);

		private float targetTimer;

		private IDisposable targetDisposable;

		private Base _target;

		private float _actionTimer;

		private ReactiveProperty<ActionControl.ResultInfo> actResult = new ReactiveProperty<ActionControl.ResultInfo>();

		private ActionScene _actScene;

		private AgentSpeeder _speeder;

		private bool _isOnNavMeshReset;

		private NonActiveWaitPointInfo.Param _nonActiveWPParam;

		private CharaPatrolMover patrolMover;

		private bool isDress;

		private BoolReactiveProperty isPatrolReceived = new BoolReactiveProperty(false);

		private bool isFirstActionPlayed;

		private HashSet<AI> targetReceives = new HashSet<AI>();

		private bool isInitialized;

		private int? _togetherActionMapNo;

		public int actionNo
		{
			get
			{
				return (actResult.Value != null) ? actResult.Value.actionNo : (-1);
			}
		}

		public bool isPlayerResWaiting
		{
			get
			{
				int num = actionNo;
				if (num == 28 || num == 29 || num == 30)
				{
					return true;
				}
				return false;
			}
		}

		public bool isArrival
		{
			get
			{
				return _isArrival.Value;
			}
			set
			{
				_isArrival.Value = value;
			}
		}

		public NodeCanvasAccesser accesser { get; private set; }

		public List<int> route { get; private set; }

		public Vector3 position
		{
			get
			{
				return (!npc.isActive) ? npc.calcPosition : npc.position;
			}
		}

		public Vector3 targetPos
		{
			get
			{
				Vector3 vector;
				float num;
				if (_target.isArrival)
				{
					vector = _target.wpData.wp.navPosition;
					num = 0.1f;
				}
				else
				{
					vector = ((!(_target is NPC)) ? _target.position : (_target as NPC).AI.position);
					num = 0.3f;
				}
				return vector + (vector - position).normalized * (0f - num);
			}
		}

		public Base target
		{
			get
			{
				return _target;
			}
			private set
			{
				if (targetDisposable != null)
				{
					targetDisposable.Dispose();
					targetDisposable = null;
				}
				npc.corners = null;
				targetTimer = 0f;
				if (_target is NPC)
				{
					NPC nPC = _target as NPC;
					nPC.AI.targetReceives.Remove(this);
				}
				_target = value;
				if (value is NPC)
				{
					NPC nPC2 = value as NPC;
					nPC2.AI.targetReceives.Add(this);
				}
				if (value != null)
				{
					targetDisposable = value.ObserveEveryValueChanged((Base t) => t.mapNo).Subscribe(delegate(int mapNo)
					{
						Stop();
						accesser.TargetMapNo = mapNo;
						Start();
					});
				}
			}
		}

		public float actionTimer
		{
			get
			{
				return _actionTimer;
			}
		}

		public ActionControl.ResultInfo result
		{
			get
			{
				return actResult.Value;
			}
		}

		private NPC npc { get; set; }

		private ActionScene actScene
		{
			get
			{
				return this.GetCacheObject(ref _actScene, () => npc.actScene);
			}
		}

		private SaveData.Heroine heroine
		{
			get
			{
				return npc.heroine;
			}
		}

		public AgentSpeeder speeder
		{
			get
			{
				return this.GetCacheObject(ref _speeder, () => npc.move.agentSpeeder);
			}
		}

		public bool isOnNavMeshReset
		{
			get
			{
				return _isOnNavMeshReset;
			}
			set
			{
				_isOnNavMeshReset = value;
			}
		}

		public NonActiveWaitPointInfo.Param nonActiveWPParam
		{
			set
			{
				if (value != null)
				{
					_nonActiveWPParam = value;
					_nonActiveWPParam.Set(npc);
					return;
				}
				if (_nonActiveWPParam != null)
				{
					_nonActiveWPParam.Set(null);
				}
				_nonActiveWPParam = null;
			}
		}

		public bool isPatrol
		{
			get
			{
				return patrolMover != null;
			}
		}

		public bool isTalkSuccess
		{
			get
			{
				int num = actionNo;
				if (_isArrival.Value)
				{
					switch (num)
					{
					case 0:
					case 1:
					case 2:
					case 4:
					case 5:
					case 7:
					case 8:
					case 20:
					case 22:
					case 23:
					case 24:
					case 25:
					case 26:
					case 27:
						return false;
					}
				}
				else
				{
					switch (num)
					{
					case 5:
					case 8:
					case 20:
					case 23:
					case 24:
					case 25:
					case 28:
					case 29:
					case 30:
						return false;
					}
				}
				return true;
			}
		}

		private bool isLesSuccess
		{
			get
			{
				NPC nPC = _target as NPC;
				if (nPC == null)
				{
					return false;
				}
				return ActionNoCheck(27) && nPC.AI.ActionNoCheck(26);
			}
		}

		public bool isRunning
		{
			get
			{
				return accesser.bt.isRunning;
			}
		}

		public bool isPaused
		{
			get
			{
				return accesser.bt.isPaused;
			}
		}

		public AI(NPC npc)
		{
			this.npc = npc;
			accesser = new NodeCanvasAccesser(npc.GetComponent<BehaviourTreeOwner>());
			Stop();
		}

		public bool ActionNoCheck(params int[] nums)
		{
			ActionControl.ResultInfo value = actResult.Value;
			if (value == null)
			{
				return false;
			}
			return nums.Contains(value.actionNo);
		}

		public bool MoveEnter(Vector3[] points, float timer)
		{
			if (points.IsNullOrEmpty())
			{
				return true;
			}
			Vector3 calcPosition = npc.calcPosition;
			float speed = npc.agent.speed;
			npc.calcPosition = Utils.Math.MoveSpeedPositionEnter(points, timer * speed);
			float animParamSpeed = 0f;
			if ((npc.calcPosition - calcPosition).sqrMagnitude > 0f)
			{
				calcPosition.y = npc.calcPosition.y;
				npc.cachedTransform.forward = npc.calcPosition - calcPosition;
				animParamSpeed = speed;
			}
			npc.move.animParamSpeed = animParamSpeed;
			return points[points.Length - 1] == npc.calcPosition;
		}

		public void TimeEnd()
		{
			ActionControl.ResultInfo value = actResult.Value;
			if (value != null)
			{
				_actionTimer = Mathf.Max(value.actionTime, value.totalTime);
			}
		}

		public NonActiveWaitPointInfo.Param NonActiveWaitPointSearch(Vector3 pos, out List<NonActiveWaitPointInfo.Param> list)
		{
			if (actScene.PosSet.nonActiveWaitPointDic.TryGetValue(npc.mapNo, out list))
			{
				return (from p in list
					where !p.isReserved
					orderby (p.pos - pos).sqrMagnitude
					select p).FirstOrDefault();
			}
			return null;
		}

		public NPC TargetReceiver(Func<AI, bool> predicate = null)
		{
			AI aI = ((predicate != null) ? targetReceives.FirstOrDefault(predicate) : targetReceives.FirstOrDefault());
			return (aI != null) ? aI.npc : null;
		}

		public void ActionStart()
		{
			if (isInitialized)
			{
				return;
			}
			isInitialized = true;
			(from isOn in isPatrolReceived
				where !isOn
				select isOn into _
				where patrolMover != null
				select _).Subscribe(delegate
			{
				UnityEngine.Object.Destroy(patrolMover);
				patrolMover = null;
			});
			(from arrival in _isArrival.TakeUntilDestroy(npc).SkipWhile((bool _) => npc.chaCtrl == null)
				where arrival
				select arrival into _
				select actResult.Value into result
				where result != null
				select result).Subscribe(delegate(ActionControl.ResultInfo result)
			{
				ArrivalSet(result);
			});
			(from arrival in _isArrival
				where !arrival
				select arrival into _
				where _target is NPC
				select _).Subscribe(delegate
			{
				_target.move.isStop = false;
			});
			actResult.TakeUntilDestroy(npc).Subscribe(delegate(ActionControl.ResultInfo result)
			{
				Result(result);
			});
			(from _ in npc.UpdateAsObservable().TakeUntilDestroy(actScene)
				where !Singleton<Game>.Instance.IsRegulate(true)
				where isRunning && !isPaused
				select actResult.Value into result
				where result != null
				select result).Subscribe(delegate(ActionControl.ResultInfo result)
			{
				float num = ((!_isArrival.Value) ? result.totalTime : result.actionTime);
				if (_actionTimer < num)
				{
					ArrivalUpdate(result);
					_actionTimer = Mathf.Min(_actionTimer + Time.deltaTime, num);
				}
				if (!(_actionTimer < num))
				{
					bool flag = true;
					switch (result.actionNo)
					{
					case 25:
						flag = false;
						EscapeAction(heroine.relation < 1);
						break;
					case 26:
					{
						NPC nPC = TargetReceiver((AI p) => p.actionNo == 27);
						bool flag2 = !(nPC == null) && nPC.isArrival;
						SetLesbianDesire((!flag2 || !(npc.motion.state == npc.wpData.motion.motion)) ? LesbianResult.Failed_Main : LesbianResult.Success);
						break;
					}
					case 27:
						SetLesbianDesire((!npc.isArrival) ? LesbianResult.Failed_Sub : LesbianResult.Success);
						break;
					case 28:
					case 29:
						flag = false;
						NextActionNoTarget();
						break;
					}
					if (flag)
					{
						NextAction();
					}
				}
			});
			(from _ in npc.UpdateAsObservable().TakeUntilDestroy(actScene)
				where isRunning && !isPaused
				where _target != null
				select _target).Subscribe(delegate(Base target)
			{
				TargetUpdate(target);
			});
		}

		public void Reset(bool isResult)
		{
			_isArrival.Value = false;
			accesser.TargetMapNo = -1;
			nonActiveWPParam = null;
			target = null;
			if (isResult)
			{
				targetReceives.Clear();
				actResult.Value = null;
			}
			Stop();
		}

		public ActionControl.ResultInfo FirstAction()
		{
			ActionControl.ResultInfo resultInfo = actScene.actCtrl.FirstAction(heroine);
			CoordinateSetting(resultInfo);
			ActionControl.ResultInfo value = resultInfo;
			actResult.Value = value;
			return value;
		}

		public void NextActionNoTarget()
		{
			NextAction(5, 7, 8);
		}

		public void NextAction(params int[] not)
		{
			if (heroine.isDresses.Check(false) == -1)
			{
				not = not.Union(new int[1]).ToArray();
			}
			actResult.Value = actScene.actCtrl.NextAction(heroine, not);
		}

		public void EscapeAction(bool isAnger)
		{
			switch (actionNo)
			{
			case 26:
				SetLesbianDesire(LesbianResult.Half_Main);
				break;
			case 27:
				SetLesbianDesire(LesbianResult.Half_Sub);
				break;
			}
			actResult.Value = actScene.actCtrl.EscapeAction(heroine, isAnger);
		}

		public void ChaseAction()
		{
			SetAction(23);
		}

		public void TogetherAction(int? mapNo)
		{
			SetAction(24);
			_togetherActionMapNo = mapNo;
		}

		public void ShyAction()
		{
			SetAction(25);
		}

		public void LesbianAction(NPC target)
		{
			target.AI.actResult.Value = actScene.actCtrl.SetLesbian(heroine, target.heroine);
		}

		public void SetLesbianDesire(LesbianResult result)
		{
			int num = 26;
			int value = 0;
			switch (result)
			{
			case LesbianResult.Success:
				value = 0;
				break;
			case LesbianResult.Half_Main:
				value = 50;
				break;
			case LesbianResult.Half_Sub:
				value = (int)((float)actScene.actCtrl.GetDesire(num, npc.heroine) * 0.5f);
				break;
			case LesbianResult.Failed_Main:
				value = 65;
				break;
			case LesbianResult.Failed_Sub:
				value = (int)((float)actScene.actCtrl.GetDesire(num, npc.heroine) * 0.65f);
				break;
			}
			actScene.actCtrl.SetDesire(num, npc.heroine, value);
		}

		public void TalkResponseAction()
		{
			SetAction(28);
			_isArrival.Value = true;
		}

		public void HResponseAction()
		{
			SetAction(29);
			_isArrival.Value = true;
		}

		public void HesitantlyWait()
		{
			SetAction(30);
			_isArrival.Value = true;
		}

		public void SetAction(int no, int mapID = -1)
		{
			actResult.Value = actScene.actCtrl.SetAction(heroine, no, mapID);
		}

		public void Start()
		{
			if (!(npc.move.NowState is Chase) && !isRunning)
			{
				accesser.bt.StartBehaviour();
			}
		}

		public void Pause()
		{
			accesser.bt.PauseBehaviour();
		}

		public void Stop()
		{
			accesser.bt.StopBehaviour();
		}

		public void GotoMap(int mapNo)
		{
			Reset(false);
			Start();
			accesser.TargetMapNo = mapNo;
		}

		public void GotoTarget(Base target)
		{
			Reset(false);
			Start();
			this.target = target;
		}

		private void CoordinateSetting(ActionControl.ResultInfo aiResult)
		{
			SaveData.Heroine heroine = this.heroine;
			List<int> list = new List<int>();
			if (actScene.Cycle.nowWeek == Cycle.Week.Saturday)
			{
				switch (actScene.Cycle.nowType)
				{
				case Cycle.Type.LunchTime:
					list.Add(5);
					break;
				case Cycle.Type.StaffTime:
				{
					ClubInfo.Param clubInfo2 = Game.GetClubInfo(heroine, true);
					if (aiResult.point != null && aiResult.point.MapNo != actScene.Map.GetParam(clubInfo2.Place).No)
					{
						list.Add(5);
					}
					list.Add(4);
					break;
				}
				case Cycle.Type.AfterSchool:
				{
					ClubInfo.Param clubInfo = Game.GetClubInfo(heroine, true);
					if (aiResult.point != null && aiResult.point.MapNo == actScene.Map.GetParam(clubInfo.Place).No)
					{
						list.Add(4);
					}
					list.Add(5);
					break;
				}
				}
				heroine.coordinates = list.ToArray();
				heroine.isDresses = new bool[Mathf.Max(0, list.Count - 1)];
				return;
			}
			switch (actScene.Cycle.nowType)
			{
			case Cycle.Type.LunchTime:
			{
				bool flag = false;
				for (int i = 0; i < 2; i++)
				{
					switch (actScene.Cycle.GetNowLessones(heroine.schoolClass)[i])
					{
					case "グラウンド":
					case "体育館":
						if (!flag && i != 0)
						{
							list.Add(0);
						}
						list.Add(2);
						flag = false;
						break;
					case "プール":
						if (!flag && i != 0)
						{
							list.Add(0);
						}
						list.Add(3);
						flag = false;
						break;
					default:
						if (!flag)
						{
							list.Add(0);
						}
						flag = true;
						break;
					}
				}
				break;
			}
			case Cycle.Type.StaffTime:
			{
				ClubInfo.Param clubInfo4 = Game.GetClubInfo(heroine, true);
				if (aiResult.point != null && aiResult.point.MapNo != actScene.Map.GetParam(clubInfo4.Place).No)
				{
					list.Add(0);
				}
				list.Add(4);
				break;
			}
			case Cycle.Type.AfterSchool:
			{
				ClubInfo.Param clubInfo3 = Game.GetClubInfo(heroine, true);
				if (aiResult.point != null && aiResult.point.MapNo == actScene.Map.GetParam(clubInfo3.Place).No)
				{
					list.Add(4);
				}
				list.Add(1);
				break;
			}
			}
			heroine.coordinates = list.ToArray();
			heroine.isDresses = new bool[Mathf.Max(0, list.Count - 1)];
		}

		public void MapRouteFind(params int[] mapNoArray)
		{
			ActionMap map = actScene.Map;
			ActionControl.ResultInfo resultInfo = result;
			HashSet<int> notMap = ((resultInfo == null) ? null : resultInfo.notMap);
			ActionMap.NavigationInfo navigationInfo = (from no in mapNoArray
				select map.SearchRoute(npc.mapNo, no, position, notMap) into item
				where item != null
				orderby item.distance
				select item).FirstOrDefault();
			route = ((navigationInfo != null) ? navigationInfo.IDs.ToList() : null);
		}

		private void Result(ActionControl.ResultInfo result)
		{
			npc.motion.state = string.Empty;
			if (npc.isActive)
			{
				Singleton<Voice>.Instance.Stop(npc.Head);
			}
			npc.LesHEndForce();
			foreach (AI item in targetReceives.Where((AI p) => p._isArrival.Value || p.actionNo == 27))
			{
				item.TimeEnd();
			}
			npc.move.isStop = false;
			npc.LookForDefault();
			_actionTimer = 0f;
			npc.SetWaitPoint(null);
			if (actScene.otherPauseNPC == npc)
			{
				actScene.OtherAIPause(null, false);
			}
			if (result == null)
			{
				isDress = false;
				isFirstActionPlayed = false;
				isPatrolReceived.Value = false;
				npc.chaCtrl.hideMoz = true;
				npc.chaCtrl.RandomChangeOfClothesLowPolyEnd();
				npc.MapShoesSetting(npc.mapNo);
				return;
			}
			int num = result.actionNo;
			if (num != 25)
			{
				npc.chaCtrl.hideMoz = true;
				npc.chaCtrl.RandomChangeOfClothesLowPolyEnd();
			}
			speeder.mode = result.move;
			if (result.actionNo == 0)
			{
				isDress = true;
			}
			else if (isDress)
			{
				heroine.NextCoordinate();
				isDress = false;
			}
			if ((result.actionNo == 6 || result.actionNo == 18) && result.point != null && result.point.transform.childCount > 0)
			{
				isPatrolReceived.Value = true;
			}
			else if (isPatrolReceived.Value)
			{
				isPatrolReceived.Value = false;
			}
			if (!isFirstActionPlayed)
			{
				if (actScene.Map.no == npc.mapNo)
				{
					actScene.Player.isLesMotionPlay = false;
				}
				result.point.SetWait(result.pointIndex, result.notAction);
				if (result.actionNo == 26)
				{
					result.actionTime = 200f;
				}
			}
			else if (result.point != null)
			{
				GotoMap(result.point.MapNo);
			}
			else if (result.heroine != null)
			{
				Base charaBase = result.heroine.charaBase;
				GotoTarget(charaBase);
				if (charaBase == null)
				{
					_isArrival.Value = true;
					TimeEnd();
				}
			}
			else if (result.actionNo == 25)
			{
				GotoTarget(null);
				_isArrival.Value = true;
			}
			else
			{
				GotoTarget(actScene.Player);
				if (Manager.Config.AddData.AINotPlayerTarget && ActionNoCheck(5, 8))
				{
					TimeEnd();
				}
			}
			isFirstActionPlayed = true;
			npc.MapShoesSetting(npc.mapNo);
		}

		private void ArrivalSet(ActionControl.ResultInfo result)
		{
			_actionTimer = 0f;
			targetTimer = 0f;
			npc.corners = null;
			npc.move.velocity = Vector3.zero;
			if (npc.agent.enabled)
			{
				npc.agent.ResetPath();
			}
			if (result.point != null && result.point.isReserved)
			{
				if (actScene.Map.no == npc.mapNo)
				{
					actScene.Player.isLesMotionPlay = false;
				}
				int index = result.point.SetWait(result.pointIndex, result.notAction);
				int iD = result.point.parameterList[result.pointIndex].motionList[index].ID;
				result.actionTime = actScene.actCtrl.GetActionTime(iD);
				if (result.actionNo == 26)
				{
					result.actionTime = 200f;
				}
			}
			if (_nonActiveWPParam != null)
			{
				npc.PlayAnimation();
			}
			if (isPatrolReceived.Value)
			{
				if (patrolMover == null)
				{
					patrolMover = npc.GetOrAddComponent<CharaPatrolMover>();
				}
				patrolMover.mode = AgentSpeeder.Mode.Run;
				patrolMover.route = result.point.transform.Children().ToArray();
				patrolMover.SetDestination(0);
			}
			switch (result.actionNo)
			{
			case 0:
				isDress = false;
				heroine.NextCoordinate();
				npc.SynchroCoordinate(true);
				npc.chaCtrl.hideMoz = false;
				break;
			case 1:
				npc.chaCtrl.ChangeToiletStateLowPoly();
				npc.chaCtrl.hideMoz = false;
				break;
			case 2:
				npc.chaCtrl.SetClothesStateAll(3);
				npc.chaCtrl.hideMoz = false;
				break;
			case 4:
				if (actScene.Player.mapNo == npc.mapNo || npc.isOtherPeople)
				{
					TimeEnd();
					break;
				}
				npc.chaCtrl.hideMoz = false;
				npc.chaCtrl.ChangeToiletStateLowPoly();
				break;
			case 20:
				TimeEnd();
				break;
			case 26:
			{
				NPC nPC2 = actScene.npcList.Find((NPC target) => target != npc && (target.heroine.parameter.attribute.likeGirls || actScene.actCtrl.GetDesire(26, target.heroine) >= 80) && target.AI.actResult.Value != null && !target.AI.actResult.Value.isPriority && target.AI.isTalkSuccess && !target.AI.isPlayerResWaiting);
				if (nPC2 != null)
				{
					LesbianAction(nPC2);
				}
				else
				{
					result.actionTime = UnityEngine.Random.Range(5, 10);
				}
				break;
			}
			case 27:
			{
				NPC nPC = _target as NPC;
				_actionTimer = 0f;
				nPC.AI._actionTimer = 0f;
				nPC.AI.actResult.Value.actionTime = result.actionTime;
				break;
			}
			}
			if (_target is Player)
			{
				switch (result.actionNo)
				{
				case 5:
				case 8:
				{
					bool flag = false;
					flag = Manager.Config.AddData.AINotPlayerTarget;
					if (!Singleton<Game>.Instance.IsRegulate(true) && !(_target as Player).isActionNow_Origin && !flag)
					{
						if (!Manager.Config.AddData.AINotPlayerTargetCommunication)
						{
							actScene.SceneEvent(npc);
							break;
						}
						bool flag2 = false;
						if (true && actScene.Map.Info.isWarning)
						{
							npc.isDefenseSpeak = true;
							int encounterADVNo = npc.GetEncounterADVNo();
							npc.Speak(Program.FindADVBundleFilePath(encounterADVNo, npc.heroine), encounterADVNo.ToString());
							ShyAction();
							break;
						}
						switch (result.actionNo)
						{
						case 5:
							HResponseAction();
							break;
						case 8:
							TalkResponseAction();
							break;
						}
					}
					else
					{
						TimeEnd();
					}
					break;
				}
				case 23:
					break;
				case 24:
				case 28:
				case 29:
				{
					int advNo = -1;
					switch (result.actionNo)
					{
					case 24:
						advNo = 93;
						break;
					case 28:
						advNo = 98;
						break;
					case 29:
						advNo = 99;
						break;
					}
					if (actScene.Map.Info.isWarning && (result.actionNo == 28 || result.actionNo == 29))
					{
						TimeEnd();
						break;
					}
					Utils.Math.TargetFor(npc.cachedTransform, _target.cachedTransform);
					npc.move.isStop = true;
					npc.Speak(Program.FindADVBundleFilePath(advNo, npc.heroine), advNo.ToString());
					break;
				}
				case 30:
					npc.SetHesitantly();
					result.actionTime = UnityEngine.Random.Range(10, 20);
					break;
				default:
					TimeEnd();
					break;
				}
			}
			else
			{
				if (!(_target is NPC))
				{
					return;
				}
				NPC nPC3 = _target as NPC;
				float num = Vector3.Distance(nPC3.AI.position, position);
				bool flag3 = !isLesSuccess && !nPC3.AI.isTalkSuccess && nPC3.AI.isPlayerResWaiting;
				if (result.actionNo != 27 && !flag3)
				{
					flag3 = !npc.isArrival && (num < 0.5f || num > 1.2f);
				}
				nPC3.AI.targetReceives.Where((AI p) => p != this && p._isArrival.Value).ToList().ForEach(delegate(AI item)
				{
					item.TimeEnd();
					item.target = null;
				});
				if (!flag3 && (nPC3.motion.state.IndexOf("Sleep") != -1 || nPC3.AI.isPatrol))
				{
					flag3 = true;
				}
				if (flag3)
				{
					TimeEnd();
					return;
				}
				npc.move.isStop = true;
				nPC3.move.isStop = true;
				if (result.actionNo == 7)
				{
					SaveData.Heroine[] array = new SaveData.Heroine[2] { npc.heroine, nPC3.heroine };
					if (array.Any((SaveData.Heroine heroine) => heroine.parameter.attribute.likeGirls))
					{
						SaveData.Heroine[] array2 = array;
						foreach (SaveData.Heroine heroine2 in array2)
						{
							actScene.actCtrl.AddDesire(26, heroine2, 5);
						}
					}
				}
				if (actScene.Map.no == npc.mapNo)
				{
					actScene.Player.isLesMotionPlay = false;
				}
				npc.SetTalker();
				if (nPC3.isBodyForTarget)
				{
					nPC3.SetListener();
					Utils.Math.TargetFor(_target.cachedTransform, npc.cachedTransform);
				}
			}
		}

		private void ArrivalUpdate(ActionControl.ResultInfo result)
		{
			if (!_isArrival.Value)
			{
				return;
			}
			switch (result.actionNo)
			{
			case 4:
				if (npc.isOtherPeople)
				{
					TimeEnd();
				}
				else
				{
					if (!npc.isActive || npc.mapNo != 45)
					{
						break;
					}
					bool flag = false;
					Collider[] source = Physics.OverlapSphere(position, (npc.baseCollider as CapsuleCollider).radius, 2048);
					foreach (Collider item in source.Where((Collider p) => p.name.IndexOf("DoorCloseEvent") != -1))
					{
						if (!(item == null))
						{
							Door component = item.GetComponent<Door>();
							if (component != null && component.hitList.Contains(npc.baseCollider))
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						npc.chaCtrl.SetClothesStateAll(3);
					}
				}
				break;
			case 25:
				if (npc.speak.isOpen)
				{
					_actionTimer = 0f;
				}
				break;
			case 28:
			case 29:
				if (npc.speak.isOpen)
				{
					_actionTimer = 0f;
				}
				Utils.Math.TargetFor(npc.cachedTransform, _target.cachedTransform);
				break;
			case 30:
				Utils.Math.TargetFor(npc.cachedTransform, _target.cachedTransform);
				break;
			case 26:
				if (!targetReceives.Any((AI p) => p.actionNo == 27))
				{
					TimeEnd();
				}
				break;
			case 27:
				if (!(_target as NPC).AI.ActionNoCheck(26))
				{
					TimeEnd();
				}
				break;
			}
			if (_target == null)
			{
				if (npc.isTalking && !targetReceives.Any((AI p) => p._isArrival.Value))
				{
					TimeEnd();
				}
			}
			else if (_target is NPC)
			{
				NPC nPC = _target as NPC;
				float num = Vector3.Distance(nPC.AI.position, position);
				bool flag2 = !isLesSuccess && !nPC.AI.isTalkSuccess && nPC.AI.isPlayerResWaiting;
				if (result.actionNo != 27 && !flag2)
				{
					flag2 = !npc.isArrival && (num < 0.5f || num > 1.2f);
				}
				if (!flag2 && !nPC.move.isStop)
				{
					flag2 = true;
				}
				if (flag2)
				{
					TimeEnd();
				}
				else if (!npc.isArrival)
				{
					Utils.Math.TargetFor(npc.cachedTransform, _target.cachedTransform);
				}
			}
			else if (_target is Player && result.actionNo == 24 && !npc.speak.isOpen)
			{
				Player player = _target as Player;
				player.ChaserSet(npc);
				player.isActionNow = false;
				if (_togetherActionMapNo.HasValue)
				{
					npc.mapNo = _togetherActionMapNo.Value;
					actScene.Map.PlayerMapWarp(_togetherActionMapNo.Value);
				}
				_togetherActionMapNo = null;
			}
		}

		private void TargetUpdate(Base target)
		{
			if (target is NPC)
			{
				NPC nPC = target as NPC;
				if (nPC.AI.result != null && !isLesSuccess && nPC.AI.result.isPriority)
				{
					TimeEnd();
					return;
				}
			}
			if (target.mapNo != npc.mapNo || _isArrival.Value || accesser.TargetMapNo != -1)
			{
				return;
			}
			if (!npc.isActive || !npc.agent.enabled || !npc.agent.isOnNavMesh)
			{
				if (!Singleton<Game>.Instance.IsRegulate(true))
				{
					targetTimer += Time.deltaTime;
				}
				bool flag = false;
				if (targetTimer > 0f)
				{
					flag = MoveEnter(npc.corners, targetTimer);
					npc.position = npc.calcPosition;
				}
				if (flag)
				{
					Vector3 vector = ((!(target is NPC)) ? target.position : (target as NPC).AI.position);
					Vector3 forward = Vector3.forward;
					Vector3 vector2 = target.rotation * forward;
					npc.position = vector + vector2;
					_isArrival.Value = true;
				}
				return;
			}
			npc.agent.SetDestination(targetPos);
			if (Singleton<Game>.Instance.IsRegulate(true))
			{
				npc.agent.velocity = Vector3.zero;
			}
			npc.AgentVelocityMoveAnimeUpdate();
			if (!npc.agent.pathPending)
			{
				targetTimer = 0f;
				npc.corners = npc.agent.path.corners;
				npc.calcPosition = npc.position;
				float num = ((target.state != 0) ? 0.52f : 1f);
				if (npc.agent.remainingDistance < num)
				{
					_isArrival.Value = true;
				}
			}
		}
	}
}
