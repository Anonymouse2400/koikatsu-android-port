using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace UGUI_AssistLibrary
{
	public class UIAL_EventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IEventSystemHandler
	{
		public class TriggerEvent : UnityEvent<BaseEventData>
		{
		}

		[Flags]
		public enum ButtonType
		{
			Left = 1,
			Right = 2,
			Center = 4
		}

		public class Entry
		{
			public ButtonType buttonType = ButtonType.Left | ButtonType.Right | ButtonType.Center;

			public EventTriggerType eventID = EventTriggerType.PointerClick;

			public TriggerEvent callback = new TriggerEvent();
		}

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

		protected UIAL_EventTrigger()
		{
		}

		private void Execute(EventTriggerType id, BaseEventData eventData)
		{
			int i = 0;
			for (int count = triggers.Count; i < count; i++)
			{
				Entry entry = triggers[i];
				if (entry.eventID == id && entry.callback != null && (id != EventTriggerType.PointerClick || isClick(entry.buttonType)))
				{
					entry.callback.Invoke(eventData);
				}
			}
		}

		private bool isClick(ButtonType type)
		{
			if ((type & ButtonType.Left) != 0 && Input.GetMouseButtonUp(0))
			{
				return true;
			}
			if ((type & ButtonType.Right) != 0 && Input.GetMouseButtonUp(1))
			{
				return true;
			}
			if ((type & ButtonType.Center) != 0 && Input.GetMouseButtonUp(2))
			{
				return true;
			}
			return false;
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerEnter, eventData);
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerExit, eventData);
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			Execute(EventTriggerType.PointerClick, eventData);
		}
	}
}
