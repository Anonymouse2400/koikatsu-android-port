using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Studio
{
	public class MapDragButton : MonoBehaviour, IDragHandler, IBeginDragHandler, IInitializePotentialDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
	{
		[SerializeField]
		private Button button;

		public Action onBeginDragFunc;

		public Action onDragFunc;

		public Action onEndDragFunc;

		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			Singleton<Studio>.Instance.cameraCtrl.isCursorLock = false;
			if (Singleton<GameCursor>.IsInstance())
			{
				Singleton<GameCursor>.Instance.SetCursorLock(true);
			}
			if ((bool)button)
			{
				button.transition = Selectable.Transition.None;
			}
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (onBeginDragFunc != null)
			{
				onBeginDragFunc();
			}
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (onDragFunc != null)
			{
				onDragFunc();
			}
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			Singleton<Studio>.Instance.cameraCtrl.isCursorLock = true;
			if (Singleton<GameCursor>.IsInstance())
			{
				Singleton<GameCursor>.Instance.SetCursorLock(false);
			}
			if ((bool)button)
			{
				button.transition = Selectable.Transition.ColorTint;
			}
			if (onEndDragFunc != null)
			{
				onEndDragFunc();
			}
		}

		public void OnPointerDown(PointerEventData eventData)
		{
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			Singleton<Studio>.Instance.cameraCtrl.isCursorLock = true;
			if (Singleton<GameCursor>.IsInstance())
			{
				Singleton<GameCursor>.Instance.SetCursorLock(false);
			}
			if ((bool)button)
			{
				button.transition = Selectable.Transition.ColorTint;
			}
		}
	}
}
