using Illusion.Game;
using UnityEngine.EventSystems;

namespace Illusion.Component.UI
{
	public class SelectUIPlaySE : SelectUI
	{
		public override void OnPointerEnter(PointerEventData eventData)
		{
			base.OnPointerEnter(eventData);
			Illusion.Game.Utils.Sound.Play(SystemSE.sel);
		}
	}
}
