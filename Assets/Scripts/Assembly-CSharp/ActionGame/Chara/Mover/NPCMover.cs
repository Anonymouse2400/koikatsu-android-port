using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Illusion.Game.Elements.EasyLoader;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	public class NPCMover : Base
	{
		private Illusion.Game.Elements.EasyLoader.Motion motion;

		private const string IdleState = "Idle";

		public override Animator animator
		{
			get
			{
				return chara.animator;
			}
		}

		public NPC chara { get; private set; }

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
				case "Locomotion":
				case "Locomotion 0":
				case "Locomotion_Anger":
				case "Escape":
					return true;
				default:
					if (chara.isTalking)
					{
						return true;
					}
					return false;
				}
			}
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
				if (chara.isTalking)
				{
					return;
				}
				motion.state = "Idle";
			}
			else
			{
				SetAnimeSpeed(speed);
				if (chara.AI.actionNo == 20)
				{
					motion.state = "Escape";
				}
				else if (chara.heroine.isAnger)
				{
					motion.state = "Locomotion_Anger";
				}
				else if (!isPlay || motion.state == "Idle" || chara.isTalking)
				{
					motion.state = ((!(Random.value < 0.5f)) ? "Locomotion 0" : "Locomotion");
				}
			}
			if (isPlay)
			{
				animator.Play(motion.state);
				chara.UpdateMotionSpeed();
			}
		}

		protected override void Awake()
		{
			base.Awake();
			chara = GetComponent<NPC>();
			motion = chara.motion;
			state = new ReactiveProperty<State>(new None(this));
		}

		protected override IEnumerator Start()
		{
			yield return new WaitWhile(() => chara.agent == null);
			yield return StartCoroutine(_003CStart_003E__BaseCallProxy0());
			(from _ in _animParamSpeed.TakeUntilDestroy(animator)
				where !isParamReg
				where animator.runtimeAnimatorController != null
				where animator.gameObject.activeInHierarchy
				select _).Subscribe(delegate
			{
				if (!chara.isArrival)
				{
					MoveUpdate();
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
