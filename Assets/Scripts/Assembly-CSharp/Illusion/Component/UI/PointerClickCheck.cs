using System;
using System.Collections.Generic;
using System.Linq;
using Illusion.CustomAttributes;
using Illusion.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Illusion.Component.UI
{
	public class PointerClickCheck : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[Flags]
		public enum ButtonType
		{
			Left = 1,
			Right = 2,
			Center = 4
		}

		[Serializable]
		public class Callback : UnityEvent<PointerEventData>
		{
		}

		[EnumMask]
		public ButtonType buttonType = Utils.Enum<ButtonType>.Everything;

		public Callback onPointerClick = new Callback();

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

		public virtual void OnPointerClick(PointerEventData data)
		{
			IEnumerable<int> source = new int[3]
			{
				(!isLeft) ? (-1) : 0,
				isRight ? 1 : (-1),
				(!isCenter) ? (-1) : 2
			}.Where((int i) => i != -1);
			if (source.Any(Input.GetMouseButtonUp))
			{
				onPointerClick.Invoke(data);
			}
		}

		private void SetButtonType(bool isOn, ButtonType type)
		{
			buttonType = Utils.Enum<ButtonType>.Normalize(buttonType);
			buttonType = (ButtonType)((!isOn) ? buttonType.Sub(type) : buttonType.Add(type));
		}
	}
}
