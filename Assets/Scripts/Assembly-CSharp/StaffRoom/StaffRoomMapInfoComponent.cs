using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace StaffRoom
{
	public class StaffRoomMapInfoComponent : MonoBehaviour
	{
		public MapFileInfo info;

		public Toggle tgl;

		public Image imgThumb;

		public void Disable(bool disable)
		{
			if ((bool)tgl)
			{
				tgl.interactable = !disable;
			}
		}

		public void Disvisible(bool disvisible)
		{
			base.gameObject.SetActiveIfDifferent(!disvisible);
		}
	}
}
