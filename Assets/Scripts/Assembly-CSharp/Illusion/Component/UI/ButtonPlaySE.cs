using Illusion.Game;
using UnityEngine.EventSystems;

namespace Illusion.Component.UI
{
	public class ButtonPlaySE : MouseButtonCheck, IPointerClickHandler, IEventSystemHandler
	{
		public enum Type
		{
			Click = 0,
			Down = 1,
			Up = 2
		}

		public Type _Type;

		public SystemSE se = SystemSE.ok_s;

		private void PlaySE()
		{
			Illusion.Game.Utils.Sound.Play(se);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_Type == Type.Click)
			{
				PlaySE();
			}
		}

		private void Start()
		{
			MouseButtonCheck component = GetComponent<MouseButtonCheck>();
			component.onPointerDown.AddListener(delegate
			{
				if (_Type == Type.Down)
				{
					PlaySE();
				}
			});
			component.onPointerUp.AddListener(delegate
			{
				if (_Type == Type.Up)
				{
					PlaySE();
				}
			});
		}
	}
}
