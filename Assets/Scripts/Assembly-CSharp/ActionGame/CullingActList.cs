using Illusion.Elements.Culling;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ActionGame
{
	public class CullingActList<T> : CullingList<T> where T : Transform
	{
		public Transform absoluteTarget;

		public CullingActList(Camera camera, float[] distances, int activeFalseLevel = int.MaxValue)
			: base(camera, distances, activeFalseLevel)
		{
		}

		protected override void OnChange(CullingGroupEvent ev)
		{
			T val = base[ev.index];
			GameObject gameObject = val.gameObject;
			if (gameObject.transform == absoluteTarget)
			{
				ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
				{
					recieve.CullingGroupEvent(ev);
				});
				return;
			}
			bool flag = ev.isVisible;
			if (flag && ev.currentDistance >= activeFalseLevel)
			{
				flag = false;
			}
			if (!flag)
			{
				ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
				{
					recieve.CullingGroupEventChangeFalse(ev);
				});
			}
			else
			{
				ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
				{
					recieve.CullingGroupEventChangeTrue(ev);
				});
			}
			ExecuteEvents.Execute(gameObject, null, delegate(ICullingListRecieveMessage recieve, BaseEventData y)
			{
				recieve.CullingGroupEvent(ev);
			});
		}
	}
}
