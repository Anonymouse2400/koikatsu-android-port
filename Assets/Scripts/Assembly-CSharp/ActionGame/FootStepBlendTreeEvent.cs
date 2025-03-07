using Illusion.Game;
using UnityEngine;

namespace ActionGame
{
	public class FootStepBlendTreeEvent : FootStepEvent
	{
		[SerializeField]
		private string _parameterName;

		[SerializeField]
		private Threshold _walkThreshold = default(Threshold);

		[SerializeField]
		private Threshold _runThreshold = default(Threshold);

		[SerializeField]
		private Threshold _crouchThreshold = default(Threshold);

		[SerializeField]
		private float _walkSpeed;

		[SerializeField]
		private float _runSpeed;

		[SerializeField]
		private float _crouchSpeed;

		[SerializeField]
		private EventData[] _walkEvents;

		[SerializeField]
		private EventData[] _runEvents;

		[SerializeField]
		private EventData[] _crouchEvents;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventData[] walkEvents = _walkEvents;
			foreach (EventData eventData in walkEvents)
			{
				eventData.Execute = false;
			}
			EventData[] runEvents = _runEvents;
			foreach (EventData eventData2 in runEvents)
			{
				eventData2.Execute = false;
			}
			EventData[] crouchEvents = _crouchEvents;
			foreach (EventData eventData3 in crouchEvents)
			{
				eventData3.Execute = false;
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			float @float = animator.GetFloat(_parameterName);
			float t = Mathf.Repeat(stateInfo.normalizedTime, 1f);
			if (animator.speed > 0f)
			{
				_stateTime = Mathf.Lerp(0f, stateInfo.length, t);
			}
			EventData[] walkEvents = _walkEvents;
			foreach (EventData eventData in walkEvents)
			{
				if (eventData.Time > _stateTime && eventData.Execute)
				{
					eventData.Execute = false;
				}
			}
			EventData[] runEvents = _runEvents;
			foreach (EventData eventData2 in runEvents)
			{
				if (eventData2.Time > _stateTime && eventData2.Execute)
				{
					eventData2.Execute = false;
				}
			}
			EventData[] crouchEvents = _crouchEvents;
			foreach (EventData eventData3 in crouchEvents)
			{
				if (eventData3.Time > _stateTime && eventData3.Execute)
				{
					eventData3.Execute = false;
				}
			}
			if (_walkThreshold.InRange(@float))
			{
				EventData[] walkEvents2 = _walkEvents;
				foreach (EventData eventData4 in walkEvents2)
				{
					if (eventData4.Time < _stateTime && !eventData4.Execute)
					{
						eventData4.Execute = true;
						if (Utils.Sound.FootStepPlayCall != null && base.Base != null)
						{
							Utils.Sound.FootStepPlayCall(base.CharaID, base.Base);
						}
					}
				}
			}
			else if (_runThreshold.InRange(@float))
			{
				EventData[] runEvents2 = _runEvents;
				foreach (EventData eventData5 in runEvents2)
				{
					if (eventData5.Time < _stateTime && !eventData5.Execute)
					{
						eventData5.Execute = true;
						if (Utils.Sound.FootStepPlayCall != null && base.Base != null)
						{
							Utils.Sound.FootStepPlayCall(base.CharaID, base.Base);
						}
					}
				}
			}
			else
			{
				if (!_crouchThreshold.InRange(@float))
				{
					return;
				}
				EventData[] crouchEvents2 = _crouchEvents;
				foreach (EventData eventData6 in crouchEvents2)
				{
					if (eventData6.Time < _stateTime && !eventData6.Execute)
					{
						eventData6.Execute = true;
						if (Utils.Sound.FootStepPlayCall != null && base.Base != null)
						{
							Utils.Sound.FootStepPlayCall(base.CharaID, base.Base);
						}
					}
				}
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			EventData[] walkEvents = _walkEvents;
			foreach (EventData eventData in walkEvents)
			{
				eventData.Execute = false;
			}
			EventData[] runEvents = _runEvents;
			foreach (EventData eventData2 in runEvents)
			{
				eventData2.Execute = false;
			}
			EventData[] crouchEvents = _crouchEvents;
			foreach (EventData eventData3 in crouchEvents)
			{
				eventData3.Execute = false;
			}
		}
	}
}
