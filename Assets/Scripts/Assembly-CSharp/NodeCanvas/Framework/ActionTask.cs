using System;
using System.Collections;
using ParadoxNotion.Services;
using UnityEngine;

namespace NodeCanvas.Framework
{
	public abstract class ActionTask<T> : ActionTask where T : Component
	{
		public sealed override Type agentType
		{
			get
			{
				return typeof(T);
			}
		}

		public new T agent
		{
			get
			{
				return base.agent as T;
			}
		}
	}
	public abstract class ActionTask : Task
	{
		[NonSerialized]
		private Status status = Status.Resting;

		[NonSerialized]
		private float startedTime;

		[NonSerialized]
		private float pausedTime;

		[NonSerialized]
		private bool latch;

		[NonSerialized]
		private bool _isPaused;

		public float elapsedTime
		{
			get
			{
				if (isPaused)
				{
					return pausedTime - startedTime;
				}
				if (isRunning)
				{
					return Time.time - startedTime;
				}
				return 0f;
			}
		}

		public bool isRunning
		{
			get
			{
				return status == Status.Running;
			}
		}

		public bool isPaused
		{
			get
			{
				return _isPaused;
			}
			private set
			{
				_isPaused = value;
			}
		}

		public void ExecuteAction(Component agent, Action<bool> callback)
		{
			ExecuteAction(agent, null, callback);
		}

		public void ExecuteAction(Component agent, IBlackboard blackboard, Action<bool> callback)
		{
			if (!isRunning)
			{
				MonoManager.current.StartCoroutine(ActionUpdater(agent, blackboard, callback));
			}
		}

		private IEnumerator ActionUpdater(Component agent, IBlackboard blackboard, Action<bool> callback)
		{
			while (ExecuteAction(agent, blackboard) == Status.Running)
			{
				yield return null;
			}
			if (callback != null)
			{
				callback(status == Status.Success);
			}
		}

		public Status ExecuteAction(Component agent, IBlackboard blackboard)
		{
			if (!base.isActive)
			{
				return Status.Failure;
			}
			if (isPaused)
			{
				startedTime += Time.time - pausedTime;
				isPaused = false;
			}
			if (status == Status.Running)
			{
				OnUpdate();
				latch = false;
				return status;
			}
			if (latch)
			{
				latch = false;
				return status;
			}
			if (!Set(agent, blackboard))
			{
				return Status.Failure;
			}
			startedTime = Time.time;
			status = Status.Running;
			OnExecute();
			if (status == Status.Running)
			{
				OnUpdate();
			}
			latch = false;
			return status;
		}

		public void EndAction()
		{
			EndAction(true);
		}

		public void EndAction(bool? success)
		{
			latch = (success.HasValue ? true : false);
			if (status == Status.Running)
			{
				isPaused = false;
				status = ((success == true) ? Status.Success : Status.Failure);
				OnStop();
			}
		}

		public void PauseAction()
		{
			if (status == Status.Running)
			{
				pausedTime = Time.time;
				isPaused = true;
				OnPause();
			}
		}

		protected virtual void OnExecute()
		{
		}

		protected virtual void OnUpdate()
		{
		}

		protected virtual void OnStop()
		{
		}

		protected virtual void OnPause()
		{
		}
	}
}
