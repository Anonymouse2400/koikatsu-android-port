using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ADV;
using ActionGame.Chara;
using ActionGame.Chara.Backup;
using ActionGame.Chara.Mover;
using Illusion;
using Manager;
using UnityEngine;

namespace ActionGame
{
	public class CharaPatrolMover : PatrolMover
	{
		private NPC npc;

		private AgentSpeeder.Mode backupMode;

		public AgentSpeeder.Mode mode { private get; set; }

		protected override void Move()
		{
			if (!Singleton<Game>.Instance.IsRegulate(true) && !Program.isADVActionActive)
			{
				timer += Time.deltaTime * base.agent.speed;
			}
			else
			{
				base.agent.velocity = Vector3.zero;
			}
			if (base.isArrived)
			{
				SetDestination(_nowPosIndex % base.route.Length);
				_nowPosIndex++;
			}
			if (!base.isAgentActive)
			{
				cachedTransform.SetPositionAndRotation(Utils.Math.MoveSpeedPositionEnter(new Vector3[2] { basePos, nextPos }, timer), Quaternion.LookRotation(nextPos - cachedTransform.position));
			}
			else
			{
				npc.move.animParamSpeed = base.agent.velocity.magnitude;
				npc.corners = base.agent.path.corners;
			}
		}

		protected override IEnumerator Start()
		{
			npc = GetComponent<NPC>();
			base.agent = npc.agent;
			base.agent.avoidancePriority = 99;
			backupMode = npc.move.agentSpeeder.mode;
			npc.move.agentSpeeder.mode = mode;
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
		}

		protected override void Update()
		{
			if (!npc.move.isStop)
			{
				Move();
			}
		}

		private void OnDestroy()
		{
			if (npc != null)
			{
				NavMeshAgent bkAgent = npc.bkAgent;
				base.agent.avoidancePriority = bkAgent.priority;
				npc.move.agentSpeeder.mode = backupMode;
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
