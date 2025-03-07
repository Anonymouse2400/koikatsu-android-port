using System;
using System.Collections;
using Illusion.Extensions;
using Manager;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace ActionGame.Chara.Mover
{
	public abstract class Base : MonoBehaviour
	{
		private AgentSpeeder _agentSpeeder;

		protected int regulateLevel = -1;

		private bool isStoped;

		private bool isNavStoped;

		protected ReactiveProperty<float> _animParamSpeed = new ReactiveProperty<float>(0f);

		protected ReactiveProperty<State> state;

		protected bool isParamReg;

		protected CompositeDisposable disposables = new CompositeDisposable();

		private static int hashSpeed = Animator.StringToHash("Speed");

		public AgentSpeeder agentSpeeder
		{
			get
			{
				return _agentSpeeder;
			}
		}

		public bool isReglateMove
		{
			get
			{
				return regulateLevel != -1;
			}
		}

		public bool isStop
		{
			get
			{
				return isNavStoped;
			}
			set
			{
				isNavStoped = value;
				if (agent.enabled && agent.isOnNavMesh)
				{
					agent.isStopped = value;
				}
				if (value)
				{
					velocity = Vector3.zero;
					isStoped = true;
				}
				else
				{
					_animParamSpeed.Value = velocity.magnitude;
				}
			}
		}

		public Vector3 velocity
		{
			get
			{
				return (!(agent == null) && agent.enabled) ? agent.velocity : Vector3.zero;
			}
			set
			{
				_animParamSpeed.Value = value.magnitude;
				if (!(agent == null) && agent.enabled)
				{
					agent.velocity = value;
				}
			}
		}

		public float animParamSpeed
		{
			get
			{
				return _animParamSpeed.Value;
			}
			set
			{
				_animParamSpeed.Value = value;
			}
		}

		public State NowState
		{
			get
			{
				return state.Value;
			}
		}

		public abstract Animator animator { get; }

		protected abstract float HeightSpeed { get; }

		protected abstract NavMeshAgent agent { get; }

		public abstract bool isMoveState { get; }

		protected ActionScene actScene
		{
			get
			{
				if (!Singleton<Game>.IsInstance() || Singleton<Game>.Instance.actScene == null)
				{
					return null;
				}
				return Singleton<Game>.Instance.actScene;
			}
		}

		public void Change(State state)
		{
			this.state.Value = state;
		}

		public virtual void AnimeUpdate()
		{
		}

		public abstract void MoveUpdate();

		protected void SetAnimeSpeed(float speed)
		{
			switch (_agentSpeeder.mode)
			{
			case AgentSpeeder.Mode.Walk:
				speed = 0f;
				break;
			case AgentSpeeder.Mode.Run:
				speed = 1f;
				break;
			}
			animator.SetFloat(hashSpeed, speed);
		}

		protected virtual void Awake()
		{
			_agentSpeeder = GetComponent<AgentSpeeder>();
		}

		protected virtual IEnumerator Start()
		{
			IDisposable idis = null;
			state.Scan(delegate(State prev, State current)
			{
				prev.Release();
				idis.Dispose();
				idis = null;
				return current;
			}).Subscribe(delegate(State s)
			{
				Observable.FromCoroutine(s.Initialize).TakeUntilDestroy(this).Subscribe(delegate
				{
					idis = (from __ in this.UpdateAsObservable()
						where !isStop && agent.isOnNavMesh
						select __).Subscribe(delegate
					{
						s.Update();
					});
				});
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable()
				where agent.enabled && agent.isOnNavMesh
				select _).Subscribe(delegate
			{
				agent.isStopped = isNavStoped;
			});
			(from _ in this.UpdateAsObservable().TakeUntilDestroy(actScene)
				select actScene.regulate.HasFlag(ActionScene.Regulate.Move)).DistinctUntilChanged().Subscribe(delegate(bool level)
			{
				if (level)
				{
					if (regulateLevel == -1)
					{
						regulateLevel = 0;
					}
				}
				else if (regulateLevel == 0)
				{
					regulateLevel = -1;
					isStop = false;
				}
			}).AddTo(disposables);
			(from _ in this.UpdateAsObservable()
				where regulateLevel != -1
				where animator.runtimeAnimatorController != null && animator.gameObject.activeInHierarchy
				where isMoveState
				select _).Subscribe(delegate
			{
				isStop = true;
				MoveUpdate();
			}).AddTo(disposables);
			yield return new WaitWhile(() => animator == null);
			(from _ in this.UpdateAsObservable()
				where isStoped && !isStop
				select velocity.magnitude into speed
				where !Mathf.Approximately(speed, 0f)
				select speed).DistinctUntilChanged().Subscribe(delegate(float speed)
			{
				isStoped = false;
				_animParamSpeed.Value = speed;
			});
			(from active in (from _ in this.UpdateAsObservable().TakeUntilDestroy(animator)
					select animator.gameObject.activeInHierarchy).DistinctUntilChanged()
				where active
				select active).Subscribe(delegate
			{
				float value = _animParamSpeed.Value;
				isParamReg = true;
				_animParamSpeed.Value = value + 1f;
				isParamReg = false;
				_animParamSpeed.Value = value;
			}).AddTo(disposables);
		}

		protected virtual void OnDestroy()
		{
			disposables.Dispose();
		}
	}
}
