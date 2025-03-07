using UnityEngine;
using UnityEngine.EventSystems;

namespace Studio
{
	public class CanvasSortOrder : MonoBehaviour, IPointerDownHandler, IEventSystemHandler
	{
		[SerializeField]
		private Canvas m_Canvas;

		public void OnPointerDown(PointerEventData eventData)
		{
			SortCanvas.select = m_Canvas;
		}
	}
}
