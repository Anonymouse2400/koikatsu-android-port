using UnityEngine;

namespace ActionGame.MapSound
{
	public class StateMachineEvent : StateMachineBehaviour, IStateMachineEvent
	{
		[SerializeField]
		private int _charaID = -1;

		[SerializeField]
		private Transform _base;

		public int CharaID
		{
			get
			{
				return _charaID;
			}
			set
			{
				_charaID = value;
			}
		}

		public Transform Base
		{
			get
			{
				return _base;
			}
			set
			{
				_base = value;
			}
		}
	}
}
