using Illusion.Component;
using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Point
{
	public class Door : TriggerEnterExitEvent
	{
		private enum Event
		{
			None = 0,
			Open = 1,
			Close = 2
		}

		private enum State
		{
			Standby = 0,
			Open = 1,
			Close = 2
		}

		private static int closeState = Animator.StringToHash("Base Layer.close");

		private static int openState = Animator.StringToHash("Base Layer.open");

		private static int closeIdleStateShort = Animator.StringToHash("close_Idle");

		private static int openIdleStateShort = Animator.StringToHash("open_Idle");

		[Range(0f, 2f)]
		[SerializeField]
		[Header("規制:0=>無し,1=>開く,2=>閉める")]
		private int regulate;

		[SerializeField]
		private Animator animator;

		[SerializeField]
		private NavMeshObstacle obstacle;

		[SerializeField]
		[ButtonRegulate("Open", "開く", false, new object[] { })]
		private bool openButton;

		[SerializeField]
		[ButtonRegulate("Close", "閉じる", false, new object[] { })]
		private bool closeButton;

		[SerializeField]
		private Event ev;

		private State state;

		private const string NPC = "NPC";

		private const string Player = "Player";

		private AnimatorStateInfo info
		{
			get
			{
				return animator.GetCurrentAnimatorStateInfo(0);
			}
		}

		public Animator GetAnimator()
		{
			return animator;
		}

		public NavMeshObstacle GetObstacle()
		{
			return obstacle;
		}

		public bool IsOpen(AnimatorStateInfo? info = null)
		{
			if (!info.HasValue)
			{
				info = this.info;
			}
			return info.Value.shortNameHash == openIdleStateShort;
		}

		public bool IsClose(AnimatorStateInfo? info = null)
		{
			if (!info.HasValue)
			{
				info = this.info;
			}
			return info.Value.shortNameHash == closeIdleStateShort;
		}

		public void OpenForce()
		{
			animator.Play(openState);
			state = State.Open;
		}

		public void CloseForce()
		{
			animator.Play(closeState);
			state = State.Close;
		}

		private void Open()
		{
			if (regulate != 1)
			{
				animator.Play(openState);
				state = State.Open;
			}
		}

		private void Close()
		{
			if (regulate != 2)
			{
				animator.Play(closeState);
				state = State.Close;
			}
		}

		private void Start()
		{
			base.onTriggerEnter += delegate(Collider col)
			{
				if (col.name.StartsWith("NPC"))
				{
					switch (ev)
					{
					case Event.Open:
						if (IsClose(null))
						{
							Open();
						}
						break;
					case Event.Close:
						if (IsOpen(null))
						{
							Close();
						}
						break;
					}
				}
			};
			base.onTriggerExit += delegate(Collider col)
			{
				if (!(col == null) && animator.isActiveAndEnabled && col.name.StartsWith("NPC"))
				{
					switch (ev)
					{
					case Event.Open:
						if (IsOpen(null))
						{
							Close();
						}
						break;
					case Event.Close:
						if (IsClose(null))
						{
							Open();
						}
						break;
					}
				}
			};
		}

		private void Update()
		{
			switch (state)
			{
			case State.Standby:
				if (obstacle != null)
				{
					AnimatorStateInfo value = info;
					if (IsOpen(value))
					{
						obstacle.enabled = false;
					}
					else if (IsClose(value))
					{
						obstacle.enabled = true;
					}
				}
				break;
			case State.Open:
			case State.Close:
				state = State.Standby;
				break;
			}
		}
	}
}
