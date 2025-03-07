using UnityEngine;

namespace StrayTech
{
	public interface ITriggerGate
	{
		bool IsActive { get; }

		void TriggerWasEntered(Collider other);

		bool IsTriggerBlocked();
	}
}
