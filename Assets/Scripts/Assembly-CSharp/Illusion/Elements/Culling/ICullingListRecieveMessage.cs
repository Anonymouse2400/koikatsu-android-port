using UnityEngine;
using UnityEngine.EventSystems;

namespace Illusion.Elements.Culling
{
	public interface ICullingListRecieveMessage : IEventSystemHandler
	{
		void CullingGroupEvent(CullingGroupEvent ev);

		void CullingGroupEventChangeTrue(CullingGroupEvent ev);

		void CullingGroupEventChangeFalse(CullingGroupEvent ev);
	}
}
