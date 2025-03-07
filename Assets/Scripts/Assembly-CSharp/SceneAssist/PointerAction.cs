using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace SceneAssist
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
			for (int i = 0; i < listClickAction.Count; i++)
			{
				listClickAction[i]();
			}
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			for (int i = 0; i < listDownAction.Count; i++)
			{
				listDownAction[i]();
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			for (int i = 0; i < listEnterAction.Count; i++)
			{
				listEnterAction[i]();
			}
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			for (int i = 0; i < listExitAction.Count; i++)
			{
				listExitAction[i]();
			}
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			for (int i = 0; i < listUpAction.Count; i++)
			{
				listUpAction[i]();
			}
		}
	}
}
