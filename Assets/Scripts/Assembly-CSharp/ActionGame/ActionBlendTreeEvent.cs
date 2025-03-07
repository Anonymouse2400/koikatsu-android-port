using System.Collections.Generic;
using System.Linq;
using ActionGame.MapSound;
using Illusion.Game;
using UnityEngine;

namespace ActionGame
{
	public class ActionBlendTreeEvent : StateMachineEvent
	{
		[SerializeField]
		private bool _ignoredParameter;

		[SerializeField]
		private string _parameterName;

		[SerializeField]
		private Threshold[] _thresholds = new Threshold[0];

		[SerializeField]
		private float[] _values = new float[0];

		[SerializeField]
		private EventDataTable[] _eventsLookup = new EventDataTable[0];

		private Queue<int> _playingIDQueue = new Queue<int>();

		private Dictionary<int, Queue<AudioSource>> _audioSources = new Dictionary<int, Queue<AudioSource>>();

		private float _stateTime;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventDataTable[] eventsLookup = _eventsLookup;
			foreach (EventDataTable eventDataTable in eventsLookup)
			{
				foreach (EventData @event in eventDataTable.Events)
				{
					@event.Execute = false;
				}
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float t = Mathf.Repeat(stateInfo.normalizedTime, 1f);
			if (animator.speed > 0f)
			{
				_stateTime = Mathf.Lerp(0f, stateInfo.length, t);
			}
			EventDataTable[] eventsLookup = _eventsLookup;
			foreach (EventDataTable eventDataTable in eventsLookup)
			{
				foreach (EventData @event in eventDataTable.Events)
				{
					if (@event.Time > _stateTime && @event.Execute)
					{
						@event.Execute = false;
					}
				}
			}
			if (!_ignoredParameter)
			{
				if (_parameterName.IsNullOrEmpty())
				{
					return;
				}
				if (animator.parameters.IsNullOrEmpty())
				{
					DoStateUpdate(0);
				}
				else
				{
					if (!animator.parameters.Any((AnimatorControllerParameter x) => x.name == _parameterName))
					{
						return;
					}
					float @float = animator.GetFloat(_parameterName);
					for (int j = 0; j < _eventsLookup.Length; j++)
					{
						Threshold threshold = _thresholds[j];
						if (threshold.IsRange(@float))
						{
							DoStateUpdate(j);
						}
					}
				}
			}
			else
			{
				DoStateUpdate(0);
			}
		}

		private void DoStateUpdate(int index)
		{
			EventDataTable eventDataTable = _eventsLookup[index];
			foreach (EventData @event in eventDataTable.Events)
			{
				Invoke(@event);
			}
		}

		private void Invoke(EventData e)
		{
			if (!(e.Time < _stateTime) || e.Execute)
			{
				return;
			}
			e.Execute = true;
			switch (e.KeyType)
			{
			case KeyType.Play:
				if (Utils.Sound.SEPlayCall != null && e.ClipIDArray != null && e.ClipIDArray.Count() != 0)
				{
					int arg = e.ClipIDArray.ElementAt(Random.Range(0, e.ClipIDArray.Count()));
					Utils.Sound.SEPlayCall(base.Base, arg, false, e.RolloffThreshold);
				}
				break;
			case KeyType.PlayLoop:
			{
				if (Utils.Sound.SEPlayCall == null || e.ClipIDArray == null || e.ClipIDArray.Count() == 0)
				{
					break;
				}
				int num = e.ClipIDArray.ElementAt(Random.Range(0, e.ClipIDArray.Count()));
				AudioSource audioSource = Utils.Sound.SEPlayCall(base.Base, num, true, e.RolloffThreshold);
				if (audioSource != null)
				{
					Queue<AudioSource> value2;
					if (!_audioSources.TryGetValue(num, out value2))
					{
						Queue<AudioSource> queue = new Queue<AudioSource>();
						_audioSources[num] = queue;
						value2 = queue;
					}
					value2.Enqueue(audioSource);
					_playingIDQueue.Enqueue(num);
				}
				break;
			}
			case KeyType.Stop:
				if (Utils.Sound.SEStopCall != null && _playingIDQueue.Count != 0)
				{
					int key = _playingIDQueue.Dequeue();
					Queue<AudioSource> value;
					if (_audioSources.TryGetValue(key, out value) && value.Count != 0)
					{
						AudioSource obj = value.Dequeue();
						Utils.Sound.SEStopCall(obj);
					}
				}
				break;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventDataTable[] eventsLookup = _eventsLookup;
			foreach (EventDataTable eventDataTable in eventsLookup)
			{
				foreach (EventData @event in eventDataTable.Events)
				{
					@event.Execute = false;
				}
			}
		}
	}
}
