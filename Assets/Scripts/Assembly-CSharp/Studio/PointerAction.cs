using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Studio
{
	public class PointerAction : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IEventSystemHandler
	{
		public List<UnityAction> listClickAction = new List<UnityAction>();

		public List<UnityAction> listDownAction = new List<UnityAction>();

		public List<UnityAction> listEnterAction = new List<UnityAction>();

		public List<UnityAction> listExitAction = new List<UnityAction>();

		public List<UnityAction> listUpAction = new List<UnityAction>();

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			foreach (UnityAction item in listClickAction)
			{
				item();
			}
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			foreach (UnityAction item in listDownAction)
			{
				item();
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			foreach (UnityAction item in listEnterAction)
			{
				item();
			}
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			foreach (UnityAction item in listExitAction)
			{
				item();
			}
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			foreach (UnityAction item in listUpAction)
			{
				item();
			}
		}
	}
}
