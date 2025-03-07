using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemMessage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
{
	private bool _UpdateSelected;

	public void OnPointerEnter(PointerEventData eventData)
	{
		MonoBehaviour.print("OnPointerEnter : " + eventData);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		MonoBehaviour.print("OnPointerExit : " + eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		MonoBehaviour.print("OnPointerDown : " + eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		MonoBehaviour.print("OnPointerUp : " + eventData);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		MonoBehaviour.print("OnPointerClick : " + eventData);
	}

	public void OnInitializePotentialDrag(PointerEventData eventData)
	{
		MonoBehaviour.print("OnInitializePotentialDrag : " + eventData);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		MonoBehaviour.print("OnBeginDrag : " + eventData);
	}

	public void OnDrag(PointerEventData eventData)
	{
		MonoBehaviour.print("OnDrag : " + eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		MonoBehaviour.print("OnEndDrag : " + eventData);
	}

	public void OnDrop(PointerEventData eventData)
	{
		MonoBehaviour.print("OnDrop : " + eventData);
	}

	public void OnScroll(PointerEventData eventData)
	{
		MonoBehaviour.print("OnScroll : " + eventData);
	}

	public void OnUpdateSelected(BaseEventData eventData)
	{
		if (!_UpdateSelected)
		{
			MonoBehaviour.print("OnUpdateSelected : " + eventData);
			_UpdateSelected = true;
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		MonoBehaviour.print("OnSelect : " + eventData);
		_UpdateSelected = false;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		MonoBehaviour.print("OnDeselect : " + eventData);
	}

	public void OnMove(AxisEventData eventData)
	{
		MonoBehaviour.print("OnMove : " + eventData);
	}

	public void OnSubmit(BaseEventData eventData)
	{
		MonoBehaviour.print("OnSubmit : " + eventData);
	}

	public void OnCancel(BaseEventData eventData)
	{
		MonoBehaviour.print("OnCancel : " + eventData);
	}
}
