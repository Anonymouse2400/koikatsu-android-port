using System;
using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Illusion.Component.UI
{
	public class MouseButtonCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		[Flags]
		public enum ButtonType
		{
			Left = 1,
			Right = 2,
			Center = 4
		}

		[Flags]
		public enum EventType
		{
			PointerDown = 1,
			PointerUp = 2,
			BeginDrag = 4,
			Drag = 8,
			EndDrag = 0x10
		}

		[Serializable]
		public class Callback : UnityEvent<PointerEventData>
		{
		}

		private enum Key
		{
			Down = 0,
			Hold = 1,
			Up = 2
		}

		[EnumMask]
		public ButtonType buttonType = Utils.Enum<ButtonType>.Everything;

		[EnumMask]
		public EventType eventType = Utils.Enum<EventType>.Everything;

		public Callback onPointerDown = new Callback();

		public Callback onPointerUp = new Callback();

		public Callback onBeginDrag = new Callback();

		public Callback onDrag = new Callback();

		public Callback onEndDrag = new Callback();

		public bool isLeft
		{
			get
			{
				return Utils.Enum<ButtonType>.Normalize(buttonType).HasFlag(ButtonType.Left);
			}
			set
			{
				SetButtonType(value, ButtonType.Left);
			}
		}

		public bool isRight
		{
			get
			{
				return Utils.Enum<ButtonType>.Normalize(buttonType).HasFlag(ButtonType.Right);
			}
			set
			{
				SetButtonType(value, ButtonType.Right);
			}
		}

		public bool isCenter
		{
			get
			{
				return Utils.Enum<ButtonType>.Normalize(buttonType).HasFlag(ButtonType.Center);
			}
			set
			{
				SetButtonType(value, ButtonType.Center);
			}
		}

		public bool isOnPointerDown
		{
			get
			{
				return Utils.Enum<EventType>.Normalize(eventType).HasFlag(EventType.PointerDown);
			}
			set
			{
				SetEventType(value, EventType.PointerDown);
			}
		}

		public bool isOnPointerUp
		{
			get
			{
				return Utils.Enum<EventType>.Normalize(eventType).HasFlag(EventType.PointerUp);
			}
			set
			{
				SetEventType(value, EventType.PointerUp);
			}
		}

		public bool isOnBeginDrag
		{
			get
			{
				return Utils.Enum<EventType>.Normalize(eventType).HasFlag(EventType.BeginDrag);
			}
			set
			{
				SetEventType(value, EventType.BeginDrag);
			}
		}

		public bool isOnDrag
		{
			get
			{
				return Utils.Enum<EventType>.Normalize(eventType).HasFlag(EventType.Drag);
			}
			set
			{
				SetEventType(value, EventType.Drag);
			}
		}

		public bool isOnEndDrag
		{
			get
			{
				return Utils.Enum<EventType>.Normalize(eventType).HasFlag(EventType.EndDrag);
			}
			set
			{
				SetEventType(value, EventType.EndDrag);
			}
		}

		private int[] Indexeser
		{
			get
			{
				return new int[3]
				{
					(!isLeft) ? (-1) : 0,
					isRight ? 1 : (-1),
					(!isCenter) ? (-1) : 2
				}.Where((int i) => i != -1).ToArray();
			}
		}

		public virtual void OnPointerDown(PointerEventData data)
		{
			if (isOnPointerDown && Indexeser.Any((int i) => Check(i)[0]))
			{
				onPointerDown.Invoke(data);
			}
		}

		public virtual void OnPointerUp(PointerEventData data)
		{
			if (isOnPointerUp && Indexeser.Any((int i) => Check(i)[2]))
			{
				onPointerUp.Invoke(data);
			}
		}

		public virtual void OnBeginDrag(PointerEventData data)
		{
			if (isOnBeginDrag && Indexeser.Any((int i) => Check(i)[1]))
			{
				onBeginDrag.Invoke(data);
			}
		}

		public virtual void OnDrag(PointerEventData data)
		{
			if (isOnDrag && Indexeser.Any((int i) => Check(i)[1]))
			{
				onDrag.Invoke(data);
			}
		}

		public virtual void OnEndDrag(PointerEventData data)
		{
			if (isOnEndDrag && Indexeser.Any((int i) => Check(i)[2]))
			{
				onEndDrag.Invoke(data);
			}
		}

		private void SetButtonType(bool isOn, ButtonType type)
		{
			buttonType = Utils.Enum<ButtonType>.Normalize(buttonType);
			buttonType = (ButtonType)((!isOn) ? buttonType.Sub(type) : buttonType.Add(type));
		}

		private void SetEventType(bool isOn, EventType type)
		{
			eventType = Utils.Enum<EventType>.Normalize(eventType);
			eventType = (EventType)((!isOn) ? eventType.Sub(type) : eventType.Add(type));
		}

		private static bool[] Check(int i)
		{
			return new bool[3]
			{
				Input.GetMouseButtonDown(i),
				Input.GetMouseButton(i),
				Input.GetMouseButtonUp(i)
			};
		}
	}
}
