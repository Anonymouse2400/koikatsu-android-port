using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	[Serializable]
	public class Chase : State
	{
		public enum Type
		{
			None = 0,
			Side = 1
		}

		[SerializeField]
		private Type _type;

		private Player _target;

		[Range(0f, 1f)]
		[SerializeField]
		private float _targetLength = 0.4f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _dirLength = 0.75f;

		[SerializeField]
		private float _moveSpeed = 5f;

		[SerializeField]
		private float _smooth = 5f;

		private const float lerpSpeed = 5f;

		private const float WALK_IN_RANGE = 1.5f;

		private const float RUN_OUT_RANGE = 3f;

		private NPCMover mover { get; set; }

		private NavMeshAgent agent
		{
			get
			{
				return chara.agent;
			}
		}

		private NPC chara
		{
			get
			{
				return mover.chara;
			}
		}

		private Transform transform
		{
			get
			{
				return chara.cachedTransform;
			}
		}

		public Type type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public Player target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
			}
		}

		public float targetLength
		{
			get
			{
				return _targetLength;
			}
			set
			{
				_targetLength = value;
			}
		}

		public float dirLength
		{
			get
			{
				return _dirLength;
			}
			set
			{
				_dirLength = value;
			}
		}

		public float moveSpeed
		{
			get
			{
				return _moveSpeed;
			}
			set
			{
				_moveSpeed = value;
			}
		}

		public float smooth
		{
			get
			{
				return _smooth;
			}
			set
			{
				_smooth = value;
			}
		}

		public Chase(NPCMover mover)
			: base(mover)
		{
			this.mover = mover;
		}

		public override IEnumerator Initialize()
		{
			agent.avoidancePriority = 99;
			while (!mover.chara.isActive)
			{
				yield return null;
			}
			mover.MoveUpdate();
			base.initialized = true;
		}

		public override void Release()
		{
			chara.bkAgent.Restore();
		}

		public override void Update()
		{
			Vector3 vector = GetVelocity();
			moveSpeed = (_target.position - chara.position).magnitude * 1.5f;
			Vector3 vector2 = vector.normalized * moveSpeed;
			bool flag = vector.sqrMagnitude > vector2.sqrMagnitude;
			if (flag)
			{
				vector = vector2;
			}
			Vector3 vector3 = vector.normalized;
			float num = vector.magnitude;
			if (num < _targetLength)
			{
				vector = Vector3.zero;
				num = 0f;
				vector3 = _target.cachedTransform.forward;
			}
			agent.SetDestination(chara.position + num * vector3);
			chara.corners = agent.path.corners;
			if (agent.remainingDistance < 1.5f)
			{
				mover.agentSpeeder.mode = AgentSpeeder.Mode.Walk;
			}
			if (agent.remainingDistance > 3f)
			{
				mover.agentSpeeder.mode = AgentSpeeder.Mode.Run;
			}
			if (flag)
			{
				mover.animParamSpeed = Mathf.InverseLerp(0f, 5f, num);
			}
			else
			{
				mover.animParamSpeed = Mathf.Max(_target.move.animParamSpeed, num);
			}
		}

		private Vector3 GetVelocity()
		{
			Vector3 result = _target.position - chara.position;
			if (_type == Type.Side)
			{
				Vector3 vector = _target.cachedTransform.right * _dirLength;
				if (Vector3.Dot(vector.normalized, result.normalized) > 0f)
				{
					vector *= -1f;
				}
				if (result.sqrMagnitude < targetLength * targetLength)
				{
					result = transform.forward;
				}
				result += vector;
			}
			return result;
		}
	}
}
