using UnityEngine;
using UnityEngine.EventSystems;

public class bl_MiniMapDragArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler, IEventSystemHandler
{
	private bl_MiniMap MiniMap;

	private Vector2 origin;

	private Vector2 direction;

	private Vector2 smoothDirection;

	private bool touched;

	private int pointerID;

	private Texture2D cursorIcon;

	private void Awake()
	{
		MiniMap = base.transform.root.GetComponentInChildren<bl_MiniMap>();
		direction = Vector2.zero;
		touched = false;
		cursorIcon = MiniMap.DragCursorIcon;
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (!(MiniMap == null) && MiniMap.CanDragMiniMap && !touched)
		{
			touched = true;
			pointerID = data.pointerId;
			origin = data.position;
			Cursor.SetCursor(cursorIcon, MiniMap.HotSpot, CursorMode.ForceSoftware);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		if (!(MiniMap == null) && MiniMap.CanDragMiniMap && data.pointerId == pointerID)
		{
			Vector2 position = data.position;
			Vector2 vector = position - origin;
			direction = vector * Time.deltaTime;
			MiniMap.SetDragPosition(direction);
			origin = data.position;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (!(MiniMap == null) && MiniMap.CanDragMiniMap && data.pointerId == pointerID)
		{
			direction = Vector2.zero;
			touched = false;
			Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
		}
	}
}
