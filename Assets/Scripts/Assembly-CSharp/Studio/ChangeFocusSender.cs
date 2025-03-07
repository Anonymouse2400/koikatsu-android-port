using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class ChangeFocusSender : MonoBehaviour, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		[SerializeField]
		private ChangeFocus changeFocus;

		[SerializeField]
		private int index;

		public void OnDeselect(BaseEventData eventData)
		{
			if ((bool)changeFocus)
			{
				changeFocus.select = -1;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if ((bool)changeFocus)
			{
				changeFocus.select = index;
			}
		}
	}
}
