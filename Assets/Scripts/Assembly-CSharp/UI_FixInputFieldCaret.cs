using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_FixInputFieldCaret : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public float correctY;

	public void OnSelect(BaseEventData eventData)
	{
		InputField component = base.gameObject.GetComponent<InputField>();
		if (null != component)
		{
			RectTransform rectTransform = null;
			if ((bool)component.textComponent)
			{
				rectTransform = component.textComponent.transform as RectTransform;
			}
			RectTransform rectTransform2 = (RectTransform)base.transform.Find(base.gameObject.name + " Input Caret");
			if (rectTransform != null && rectTransform2 != null)
			{
				Vector2 anchoredPosition = rectTransform.anchoredPosition;
				anchoredPosition.y += correctY;
				rectTransform2.anchoredPosition = anchoredPosition;
			}
		}
	}
}
