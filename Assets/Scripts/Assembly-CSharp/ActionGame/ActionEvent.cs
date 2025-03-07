using System.Collections.Generic;
using System.Linq;
using ActionGame.MapSound;
using Illusion.Game;
using UnityEngine;

namespace ActionGame
{
	public class ActionEvent : StateMachineEvent
	{
		[SerializeField]
		private EventDataTable _eventTable = new EventDataTable();

		private Queue<int> _playingIDQueue = new Queue<int>();

		private Dictionary<int, Queue<AudioSource>> _audioSources = new Dictionary<int, Queue<AudioSource>>();

		private float _stateTime;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (EventData @event in _eventTable.Events)
			{
				@event.Execute = false;
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			foreach (EventData @event in _eventTable.Events)
			{
				@event.Execute = false;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float t = Mathf.Repeat(stateInfo.normalizedTime, 1f);
			if (animator.speed > 0f)
			{
				_stateTime = Mathf.Lerp(0f, stateInfo.length, t);
			}
			foreach (EventData @event in _eventTable.Events)
			{
				if (@event.Time > _stateTime && @event.Execute)
				{
					@event.Execute = false;
				}
			}
			foreach (EventData event2 in _eventTable.Events)
			{
				if (!(event2.Time < _stateTime) || event2.Execute)
				{
					continue;
				}
				event2.Execute = true;
				switch (event2.KeyType)
				{
				case KeyType.Play:
					if (Utils.Sound.SEPlayCall != null && base.Base != null)
					{
						if (event2.ClipIDArray == null || event2.ClipIDArray.Count() == 0)
						{
							return;
						}
						int arg = event2.ClipIDArray.ElementAt(Random.Range(0, event2.ClipIDArray.Count()));
						Utils.Sound.SEPlayCall(base.Base, arg, false, event2.RolloffThreshold);
					}
					break;
				case KeyType.PlayLoop:
				{
					if (Utils.Sound.SEPlayCall == null || !(base.Base != null))
					{
						break;
					}
					if (event2.ClipIDArray == null || event2.ClipIDArray.Count() == 0)
					{
						return;
					}
					int num = event2.ClipIDArray.ElementAt(Random.Range(0, event2.ClipIDArray.Count()));
					AudioSource audioSource = Utils.Sound.SEPlayCall(base.Base, num, true, event2.RolloffThreshold);
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
					if (Utils.Sound.SEStopCall != null)
					{
						if (_playingIDQueue.Count == 0)
						{
							return;
						}
						int key = _playingIDQueue.Dequeue();
						Queue<AudioSource> value;
						if (!_audioSources.TryGetValue(key, out value) || value.Count == 0)
						{
							return;
						}
						AudioSource obj = value.Dequeue();
						Utils.Sound.SEStopCall(obj);
					}
					break;
				}
			}
		}
	}
}
