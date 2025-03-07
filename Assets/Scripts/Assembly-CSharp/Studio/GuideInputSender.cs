using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class GuideInputSender : MonoBehaviour, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		[SerializeField]
		private GuideInput guideInput;

		[SerializeField]
		private int index;

		public void OnDeselect(BaseEventData eventData)
		{
			if ((bool)guideInput)
			{
				guideInput.selectIndex = -1;
			}
		}

		public void OnSelect(BaseEventData eventData)
		{
			if ((bool)guideInput)
			{
				guideInput.selectIndex = index;
			}
		}
	}
}
