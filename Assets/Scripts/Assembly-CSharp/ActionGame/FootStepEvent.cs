using System;
using ActionGame.MapSound;
using Illusion.Game;
using UnityEngine;

namespace ActionGame
{
	public class FootStepEvent : StateMachineEvent
	{
		[Serializable]
		public class EventData
		{
			[SerializeField]
			private float _time;

			[HideInInspector]
			[SerializeField]
			private bool _execute;

			public float Time
			{
				get
				{
					return _time;
				}
			}

			public bool Execute
			{
				get
				{
					return _execute;
				}
				set
				{
					_execute = value;
				}
			}
		}

		[Serializable]
		public struct Threshold
		{
			public float min;

			public float max;

			public Threshold(float minValue, float maxValue)
			{
				min = minValue;
				max = maxValue;
			}

			public float Evaluate(float t)
			{
				return Mathf.Lerp(min, max, t);
			}

			public bool InRange(float value)
			{
				return value >= min && value <= max;
			}
		}

		[SerializeField]
		private EventData[] _events;

		protected float _stateTime;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventData[] events = _events;
			foreach (EventData eventData in events)
			{
				eventData.Execute = false;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float t = Mathf.Repeat(stateInfo.normalizedTime, 1f);
			if (animator.speed > 0f)
			{
				_stateTime = Mathf.Lerp(0f, stateInfo.length, t);
			}
			EventData[] events = _events;
			foreach (EventData eventData in events)
			{
				if (eventData.Time > _stateTime && eventData.Execute)
				{
					eventData.Execute = false;
				}
			}
			EventData[] events2 = _events;
			foreach (EventData eventData2 in events2)
			{
				if (eventData2.Time < _stateTime && !eventData2.Execute)
				{
					eventData2.Execute = true;
					if (Utils.Sound.FootStepPlayCall != null && base.Base != null)
					{
						Utils.Sound.FootStepPlayCall(base.CharaID, base.Base);
					}
				}
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventData[] events = _events;
			foreach (EventData eventData in events)
			{
				eventData.Execute = false;
			}
		}
	}
}
