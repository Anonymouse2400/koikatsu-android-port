using System.Collections;
using System.Linq;
using Illusion;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame
{
	public class PatrolMover : MonoBehaviour
	{
		private NavMeshAgent _agent;

		private Transform[] _route;

		[SerializeField]
		protected int _nowPosIndex;

		[SerializeField]
		private float _arrivedDistance = 0.5f;

		protected Vector3 basePos;

		protected Vector3 nextPos;

		protected float timer;

		protected Transform cachedTransform;

		private bool isSet;

		public NavMeshAgent agent
		{
			get
			{
				return _agent;
			}
			set
			{
				_agent = value;
			}
		}

		public Transform[] route
		{
			get
			{
				return _route;
			}
			set
			{
				_route = value;
			}
		}

		public int nowPosIndex
		{
			get
			{
				return _nowPosIndex;
			}
		}

		public float arrivedDistance
		{
			get
			{
				return _arrivedDistance;
			}
			set
			{
				_arrivedDistance = value;
			}
		}

		protected bool isAgentActive
		{
			get
			{
				return _agent != null && _agent.enabled && _agent.isOnNavMesh;
			}
		}

		protected bool isArrived
		{
			get
			{
				return (!isAgentActive) ? ((nextPos - cachedTransform.position).sqrMagnitude < _arrivedDistance * _arrivedDistance) : (_agent.remainingDistance < _arrivedDistance);
			}
		}

		public void SetDestination(Vector3 next)
		{
			isSet = true;
			basePos = cachedTransform.position;
			nextPos = next;
			timer = 0f;
			if (isAgentActive)
			{
				_agent.SetDestination(next);
			}
		}

		public void SetDestination(int index)
		{
			_nowPosIndex = index;
			SetDestination(_route[index].position);
		}

		protected virtual void Move()
		{
			timer += Time.deltaTime * _agent.speed;
			if (isArrived)
			{
				SetDestination(_nowPosIndex % _route.Length);
				_nowPosIndex++;
			}
			if (!isAgentActive)
			{
				cachedTransform.SetPositionAndRotation(Utils.Math.MoveSpeedPositionEnter(new Vector3[2] { basePos, nextPos }, timer), Quaternion.LookRotation(nextPos - cachedTransform.position));
			}
		}

		private void Awake()
		{
			cachedTransform = base.transform;
		}

		protected virtual IEnumerator Start()
		{
			base.enabled = false;
			if (!(_agent == null) && !_route.IsNullOrEmpty())
			{
				yield return new WaitUntil(() => isSet);
				base.enabled = true;
			}
		}

		protected virtual void Update()
		{
			Move();
		}

		private void OnDrawGizmos()
		{
			if (_route != null)
			{
				Gizmos.color = Color.red;
				Utils.Gizmos.PointLine(_route.Select((Transform t) => t.position + Vector3.up * 0.1f).ToArray(), true);
			}
		}
	}
}
