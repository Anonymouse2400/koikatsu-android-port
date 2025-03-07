using System;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	public class AgentSpeeder : MonoBehaviour
	{
		public enum Mode
		{
			Walk = 0,
			Run = 1,
			Crouch = 2
		}

		[Serializable]
		public class ModeReactiveProperty : ReactiveProperty<Mode>
		{
			public ModeReactiveProperty()
			{
			}

			public ModeReactiveProperty(Mode initialValue)
				: base(initialValue)
			{
			}
		}

		private float _walkRev = 1f;

		private float _runRev = 1f;

		[SerializeField]
		private ModeReactiveProperty _mode = new ModeReactiveProperty(Mode.Walk);

		[Header("歩き速度")]
		[SerializeField]
		private float _walkSpeed = 0.5f;

		[SerializeField]
		[Header("走り速度")]
		private float _runSpeed = 2.5f;

		[SerializeField]
		[Header("しゃがみ速度")]
		private float _crouchSpeed = 0.25f;

		[Header("旋回速度")]
		[SerializeField]
		private float _rotateSpeed = 2f;

		public Mode mode
		{
			get
			{
				return _mode.Value;
			}
			set
			{
				_mode.Value = value;
			}
		}

		public float walkSpeed
		{
			get
			{
				return _walkSpeed;
			}
		}

		public float runSpeed
		{
			get
			{
				return _runSpeed;
			}
		}

		public float crouchSpeed
		{
			get
			{
				return _crouchSpeed;
			}
		}

		public float rotateSpeed
		{
			get
			{
				return _rotateSpeed;
			}
		}

		public NavMeshAgent agent { get; private set; }

		public float walkRev
		{
			get
			{
				return _walkRev;
			}
			set
			{
				_walkRev = value;
				if (mode == Mode.Walk)
				{
					ModeUpdate(mode);
				}
			}
		}

		public float runRev
		{
			get
			{
				return _runRev;
			}
			set
			{
				_runRev = value;
				if (mode == Mode.Run)
				{
					ModeUpdate(mode);
				}
			}
		}

		private void Start()
		{
			_walkRev = 1f;
			_runRev = 1f;
			agent = GetComponent<NavMeshAgent>();
			if (agent != null)
			{
				_mode.Subscribe(delegate(Mode m)
				{
					ModeUpdate(m);
				});
			}
		}

		public float GetSpeed(Mode mode)
		{
			float result = 0f;
			switch (mode)
			{
			case Mode.Walk:
				result = _walkSpeed * _walkRev;
				break;
			case Mode.Run:
				result = _runSpeed * _runRev;
				break;
			case Mode.Crouch:
				result = _crouchSpeed;
				break;
			}
			return result;
		}

		public void ModeUpdate(Mode mode)
		{
			agent.speed = GetSpeed(mode);
		}
	}
}
