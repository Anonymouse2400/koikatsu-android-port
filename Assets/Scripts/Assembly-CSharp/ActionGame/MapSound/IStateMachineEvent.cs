using UnityEngine;

namespace ActionGame.MapSound
{
	public interface IStateMachineEvent
	{
		int CharaID { get; set; }

		Transform Base { get; set; }
	}
}
