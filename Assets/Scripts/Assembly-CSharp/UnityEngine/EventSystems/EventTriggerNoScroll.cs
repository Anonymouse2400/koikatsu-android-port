using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace UnityEngine.EventSystems
{
	[AddComponentMenu("Event/Event Trigger NoScroll")]
	public class EventTriggerNoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler, ICancelHandler, IEventSystemHandler
	{
		[Serializable]
		public class TriggerEvent : UnityEvent<BaseEventData>
		{
		}

		[Serializable]
		public class Entry
		{
			public EventTriggerType eventID = EventTriggerType.PointerClick;

			public TriggerEvent callback = new TriggerEvent();
		}

		[SerializeField]
		private List<Entry> m_Delegates;

		public List<Entry> triggers
		{
			get
			{
				if (m_Delegates == null)
				{
					m_Delegates = new List<Entry>();
				}
				return m_Delegates;
			}
			set
			{
				m_Delegates = value;
			}
		}

		protected EventTriggerNoScroll()
		{
		}

		private void Execute(EventTriggerType id, BaseEventData eventData)
		{
			int i = 0;
			for (int count = triggers.Count; i < count; i++)
			{
				Entry entry = triggers[i];
				if (entry.eventID == id && entry.callback != null)
				{
					entry.callback.Invoke(eventData);
				}
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerEnter, eventData);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerExit, eventData);
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.Drag, eventData);
		}

		public virtual void OnDrop(PointerEventData eventData)
		{
			Execute(EventTriggerType.Drop, eventData);
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerDown, eventData);
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerUp, eventData);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerClick, eventData);
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			Execute(EventTriggerType.Select, eventData);
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			Execute(EventTriggerType.Deselect, eventData);
		}

		public virtual void OnMove(AxisEventData eventData)
		{
			Execute(EventTriggerType.Move, eventData);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			Execute(EventTriggerType.UpdateSelected, eventData);
		}

		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.InitializePotentialDrag, eventData);
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.BeginDrag, eventData);
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			Execute(EventTriggerType.EndDrag, eventData);
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			Execute(EventTriggerType.Submit, eventData);
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			Execute(EventTriggerType.Cancel, eventData);
		}
	}
}
