using Illusion.CustomAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Illusion.Component.UI
{
	public class SelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		[NotEditable]
		[SerializeField]
		private bool _isSelect;

		[NotEditable]
		[SerializeField]
		private bool _isFocus;

		public bool isSelectFocus { get; set; }

		public bool isSelect
		{
			get
			{
				return _isSelect;
			}
		}

		public bool isFocus
		{
			get
			{
				return _isFocus;
			}
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			_isSelect = true;
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			if (!isSelectFocus || !_isFocus)
			{
				_isSelect = false;
			}
		}

		public virtual void OnSelect(BaseEventData eventData)
		{
			_isFocus = true;
		}

		public virtual void OnDeselect(BaseEventData eventData)
		{
			_isFocus = false;
		}

		protected virtual void OnEnable()
		{
			EventSystem current = EventSystem.current;
			if (current != null && base.gameObject == current.currentSelectedGameObject)
			{
				_isFocus = true;
			}
		}

		protected virtual void OnDisable()
		{
			EventSystem current = EventSystem.current;
			if (current != null && base.gameObject == current.currentSelectedGameObject)
			{
				current.SetSelectedGameObject(null);
			}
			_isFocus = false;
			_isSelect = false;
		}
	}
}
