using Studio;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler, IEventSystemHandler
{
	[SerializeField]
	protected Canvas m_Canvas;

	protected RectTransform m_RectCanvas;

	protected RectTransform m_RectTransform;

	protected Rect rectArea = default(Rect);

	protected Vector2 vecRate = Vector2.one;

	protected Canvas canvas
	{
		get
		{
			if (m_Canvas == null)
			{
				m_Canvas = GetComponentInParent<Canvas>();
			}
			return m_Canvas;
		}
	}

	protected RectTransform rectCanvas
	{
		get
		{
			if (m_RectCanvas == null)
			{
				m_RectCanvas = canvas.transform as RectTransform;
			}
			return m_RectCanvas;
		}
	}

	protected RectTransform rectTransform
	{
		get
		{
			if (m_RectTransform == null)
			{
				m_RectTransform = base.transform as RectTransform;
			}
			return m_RectTransform;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		SortCanvas.select = canvas;
		Rect pixelRect = canvas.pixelRect;
		Vector2 sizeDelta = rectCanvas.sizeDelta;
		Vector2 sizeDelta2 = rectTransform.sizeDelta;
		Vector2 anchorMax = rectTransform.anchorMax;
		Vector2 pivot = rectTransform.pivot;
		rectArea.Set(sizeDelta.x * anchorMax.x + sizeDelta2.x * pivot.x, 0f - sizeDelta.y * anchorMax.y + sizeDelta2.y * pivot.y, sizeDelta.x - sizeDelta2.x, sizeDelta.y - sizeDelta2.y);
		vecRate.x = pixelRect.width / sizeDelta.x;
		vecRate.y = pixelRect.height / sizeDelta.y;
	}

	public void OnDrag(PointerEventData eventData)
	{
		Vector2 delta = eventData.delta;
		delta.x /= vecRate.x;
		delta.y /= vecRate.y;
		delta += rectTransform.anchoredPosition;
		rectTransform.anchoredPosition = Rect.NormalizedToPoint(rectArea, Rect.PointToNormalized(rectArea, delta));
	}

	public void OnEndDrag(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		SortCanvas.select = canvas;
	}
}
