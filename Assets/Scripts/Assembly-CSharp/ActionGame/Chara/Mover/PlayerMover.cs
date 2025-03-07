using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Illusion.Game.Elements.EasyLoader;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	public class PlayerMover : Base
	{
		private string IdleState = "Idle";

		private Illusion.Game.Elements.EasyLoader.Motion motion;

		public override Animator animator
		{
			get
			{
				return chara.animator;
			}
		}

		public Player chara { get; private set; }

		protected override float HeightSpeed
		{
			get
			{
				return 1f;
			}
		}

		protected override NavMeshAgent agent
		{
			get
			{
				return chara.agent;
			}
		}

		public override bool isMoveState
		{
			get
			{
				switch (motion.state)
				{
				case "Idle":
				case "squat_walk":
				case "Locomotion":
				case "squat_loop":
					return true;
				default:
					return false;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			chara = GetComponent<Player>();
			motion = chara.motion;
			state = new ReactiveProperty<State>(new Main(this));
		}

		public override void AnimeUpdate()
		{
			if (!(animator.runtimeAnimatorController == null) && animator.gameObject.activeInHierarchy)
			{
				MoveState(_animParamSpeed.Value, false);
			}
		}

		public override void MoveUpdate()
		{
			MoveState(_animParamSpeed.Value, true);
		}

		private void MoveState(float speed, bool isPlay)
		{
			if (Mathf.Approximately(speed, 0f))
			{
				motion.state = IdleState;
			}
			else
			{
				SetAnimeSpeed(speed);
				if (base.agentSpeeder.mode == AgentSpeeder.Mode.Crouch)
				{
					motion.state = "squat_walk";
				}
				else
				{
					motion.state = "Locomotion";
				}
			}
			if (isPlay)
			{
				animator.Play(motion.state);
				float num = Mathf.Lerp(1f, 1.2f, Mathf.Clamp01(Mathf.InverseLerp(0f, 100f, chara.player.physical)));
				animator.SetFloat("MotionSpeed", num);
				base.agentSpeeder.walkRev = num;
				base.agentSpeeder.runRev = num;
			}
		}

		protected override IEnumerator Start()
		{
			yield return new WaitWhile(() => agent == null);
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
			base.agentSpeeder.ObserveEveryValueChanged((AgentSpeeder speeder) => speeder.mode).Subscribe(delegate(AgentSpeeder.Mode mode)
			{
				motion.Play(animator);
				switch (mode)
				{
				case AgentSpeeder.Mode.Walk:
				case AgentSpeeder.Mode.Run:
					IdleState = "Idle";
					break;
				case AgentSpeeder.Mode.Crouch:
					IdleState = "squat_loop";
					break;
				}
				motion.state = IdleState;
				MoveState(_animParamSpeed.Value, animator.runtimeAnimatorController != null && animator.gameObject.activeInHierarchy);
			});
			(from _ in _animParamSpeed.TakeUntilDestroy(animator)
				where !isParamReg
				where animator.runtimeAnimatorController != null
				where animator.gameObject.activeInHierarchy
				select _).Subscribe(delegate
			{
				MoveUpdate();
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable()
				select chara.isActionNow).DistinctUntilChanged().Subscribe(delegate(bool level)
			{
				if (level)
				{
					if (regulateLevel == -1)
					{
						regulateLevel = 1;
					}
				}
				else if (regulateLevel == 1)
				{
					regulateLevel = -1;
					base.isStop = false;
				}
			}).AddTo(disposables);
		}

		[CompilerGenerated]
		[DebuggerHidden]
		private IEnumerator _003CStart_003E__BaseCallProxy0()
		{
			return base.Start();
		}
	}
}
