using System.Collections.Generic;
using System.Linq;
using ActionGame;
using ActionGame.Chara;
using ActionGame.Point;
using Illusion;
using Illusion.Extensions;
using Manager;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.AI;

namespace NodeCanvas.Tasks.Actions
{
	[Category("✫ Illusion/ActionGame/NPC/Map")]
	[Description("マップ移動中")]
	public class MapMove : ActionTask
	{
		private const float NavMeshArriveDistance = 0.1f;

		private const float SqrNavMeshArriveDistance = 0.010000001f;

		public BBParameter<int> targetMapNo = new BBParameter<int>
		{
			value = -1
		};

		private ActionScene actScene;

		private ActionMap actMap;

		private List<int> routeIDList;

		private Vector3[] routePoints;

		private Vector3[] corners;

		private float gateInTimer;

		private float nextCornerDistance;

		private NPC npc;

		private bool lastNavigate;

		private bool isArrived;

		private Vector3 position
		{
			get
			{
				return npc.calcPosition;
			}
			set
			{
				npc.calcPosition = value;
			}
		}

		protected override void OnExecute()
		{
			lastNavigate = false;
			isArrived = false;
			npc = base.agent.GetComponent<NPC>();
			if (npc == null)
			{
				EndAction();
				return;
			}
			position = npc.position;
			gateInTimer = 0f;
			actScene = npc.actScene;
			actMap = actScene.Map;
			routePoints = null;
			corners = null;
			nextCornerDistance = float.MaxValue;
			if (targetMapNo.value == -1)
			{
				EndAction();
				return;
			}
			npc.agent.avoidancePriority = 99;
			routeIDList = npc.AI.route;
			if (npc.mapNo == targetMapNo.value || routeIDList.IsNullOrEmpty())
			{
				LastCalculatePosition();
			}
			else
			{
				NaviGate(false);
			}
		}

		protected override void OnStop()
		{
			if (npc == null)
			{
				return;
			}
			npc.HitGateReset();
			npc.move.animParamSpeed = 0f;
			npc.agent.avoidancePriority = npc.bkAgent.priority;
			if (!isArrived)
			{
				if (actMap.no != npc.mapNo)
				{
					npc.position = position;
				}
			}
			else if (npc.AI.target == null)
			{
				npc.AI.isArrival = true;
			}
			else if (actMap.no != npc.mapNo)
			{
				Vector3 vector = ((!(npc.AI.target is NPC)) ? npc.AI.target.position : (npc.AI.target as NPC).AI.position);
				Vector3 forward = Vector3.forward;
				Vector3 vector2 = npc.AI.target.rotation * forward;
				npc.position = vector + vector2;
				npc.AI.isArrival = true;
			}
		}

		protected override void OnUpdate()
		{
			if (routePoints.IsNullOrEmpty())
			{
				EndAction();
				return;
			}
			if (npc == null || npc.agent == null)
			{
				EndAction();
				return;
			}
			if (!Singleton<Game>.Instance.IsRegulate(true) && !npc.move.isReglateMove && !npc.move.isStop)
			{
				gateInTimer += Time.deltaTime;
			}
			bool flag = false;
			if (npc.AI.isOnNavMeshReset)
			{
				if (npc.agent.enabled && !npc.agent.isOnNavMesh)
				{
					npc.position = routePoints[routePoints.Length - 1];
					corners = null;
					flag = true;
				}
				npc.AI.isOnNavMeshReset = false;
			}
			if (flag || !NaviMeshCalclater())
			{
				if (!corners.IsNullOrEmpty())
				{
					gateInTimer = 0f;
					position = npc.position;
					int a = Utils.Math.MinDistanceRouteIndex(corners, position) + 1;
					routePoints = new Vector3[1] { position }.Concat(corners.Skip(Mathf.Min(a, corners.Length - 1))).ToArray();
				}
				corners = null;
				if (flag || (gateInTimer > 0f && npc.AI.MoveEnter(routePoints, gateInTimer)))
				{
					Arrive();
				}
			}
		}

		private bool NaviMeshCalclater()
		{
			bool result = false;
			if (!npc.isActive || !npc.agent.isOnNavMesh)
			{
				nextCornerDistance = float.MaxValue;
				return result;
			}
			bool hasPath = npc.agent.hasPath;
			if (corners == null || !hasPath || npc.agent.path.corners.IsNullOrEmpty())
			{
				nextCornerDistance = float.MaxValue;
				NavMeshPath navMeshPath = new NavMeshPath();
				if (npc.agent.CalculatePath(routePoints[routePoints.Length - 1], navMeshPath))
				{
					if (npc.agent.SetPath(navMeshPath))
					{
						npc.corners = (corners = navMeshPath.corners);
						if (npc.agent.path.corners.IsNullOrEmpty())
						{
							npc.agent.Warp(corners[0]);
						}
					}
					else
					{
						npc.agent.Warp(navMeshPath.corners[0]);
					}
				}
				else
				{
					int index = Utils.Math.MinDistanceRouteIndex(routePoints, position);
					npc.agent.Warp(routePoints[index]);
				}
			}
			if (!corners.IsNullOrEmpty())
			{
				result = true;
				npc.AgentVelocityMoveAnimeUpdate();
				position = npc.position;
				if ((corners[corners.Length - 1] - position).sqrMagnitude < 0.010000001f || (!routeIDList.IsNullOrEmpty() && routeIDList[0] == npc.hitGateID))
				{
					Arrive();
				}
				else
				{
					float sqrMagnitude = (npc.agent.steeringTarget - npc.position).sqrMagnitude;
					bool flag = sqrMagnitude < 0.010000001f;
					if (sqrMagnitude <= nextCornerDistance)
					{
						nextCornerDistance = ((!flag) ? sqrMagnitude : float.MaxValue);
					}
					else if (flag)
					{
						nextCornerDistance = float.MaxValue;
					}
					else if (!float.IsInfinity(npc.agent.remainingDistance))
					{
						corners = null;
					}
				}
			}
			return result;
		}

		private void NaviGate(bool gatePositionSet)
		{
			routePoints = null;
			gateInTimer = 0f;
			bool flag = routeIDList.IsNullOrEmpty();
			if (!flag)
			{
				GateInfo gateInfo = actMap.gateInfoDic[routeIDList[0]];
				if (gatePositionSet)
				{
					npc.mapNo = gateInfo.mapNo;
					bool flag2 = actMap.no == npc.mapNo;
					GateInfo gateInfo2 = actMap.gateInfoDic[gateInfo.linkID];
					if (flag2)
					{
						npc.SetPositionAndRotation(gateInfo2.pos, gateInfo2.ang);
						npc.SetActive(true);
					}
					else
					{
						npc.SetActive(false);
						npc.SetPositionAndRotation(gateInfo2.pos, gateInfo2.ang);
					}
					routeIDList.RemoveAt(0);
					if (!routeIDList.Any() || npc.mapNo == targetMapNo.value)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					routePoints = actMap.MapCalcPosition(npc.mapNo, routeIDList[0], position, gatePositionSet ? new int?(gateInfo.linkID) : ((int?)null));
					if (!routePoints.IsNullOrEmpty())
					{
						if (actMap.no == npc.mapNo)
						{
							if (routePoints.Length > 1)
							{
								routePoints = new Vector3[1] { position }.Concat(routePoints.Skip(1)).ToArray();
							}
							else
							{
								routePoints = new Vector3[2]
								{
									position,
									routePoints[0]
								};
							}
						}
						else
						{
							NPC nPC = npc;
							Vector3 vector2 = (position = routePoints[0]);
							nPC.position = vector2;
						}
					}
					else
					{
						routePoints = new Vector3[2] { position, gateInfo.pos };
					}
				}
			}
			if (flag | (npc.mapNo == targetMapNo.value))
			{
				LastCalculatePosition();
			}
		}

		private void LastCalculatePosition()
		{
			lastNavigate = true;
			if (npc.AI.target != null)
			{
				isArrived = true;
				EndAction();
				return;
			}
			Vector3 vector = position;
			Vector3? vector2 = null;
			if (npc.AI.result != null && npc.AI.result.point != null)
			{
				vector2 = npc.AI.result.point.navPosition;
			}
			else
			{
				List<NonActiveWaitPointInfo.Param> list;
				NonActiveWaitPointInfo.Param param = npc.AI.NonActiveWaitPointSearch(vector, out list);
				if (param != null)
				{
					npc.AI.nonActiveWPParam = param;
					vector2 = param.pos;
				}
				else
				{
					if (list != null)
					{
						param = list.Take(10).Shuffle().FirstOrDefault();
					}
					if (param != null)
					{
						float num = Mathf.Lerp(1f, 2f, Random.value);
						Vector2 insideUnitCircle = Random.insideUnitCircle;
						vector2 = vector + new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y) * num;
					}
					else if (npc.isActive)
					{
						Vector3 result;
						if (Utils.NavMesh.GetRandomPosition(vector, out result))
						{
							vector2 = result;
						}
						else
						{
							float num2 = Mathf.Lerp(1f, 4f, Random.value);
							Vector3 vector3 = vector + npc.cachedTransform.forward * num2;
							NavMeshPath navMeshPath = new NavMeshPath();
							if (npc.agent.CalculatePath(vector3, navMeshPath))
							{
								routePoints = navMeshPath.corners;
							}
							else
							{
								vector2 = vector3;
							}
						}
					}
				}
			}
			if (!vector2.HasValue)
			{
				float num3 = Mathf.Lerp(1f, 2f, Random.value);
				vector2 = vector + npc.cachedTransform.forward * num3;
			}
			routePoints = new Vector3[2] { vector, vector2.Value };
		}

		private void Arrive()
		{
			corners = null;
			if (!lastNavigate)
			{
				NaviGate(true);
				return;
			}
			isArrived = true;
			EndAction();
		}
	}
}
