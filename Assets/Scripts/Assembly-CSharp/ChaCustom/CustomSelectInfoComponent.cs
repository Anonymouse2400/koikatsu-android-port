using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ChaCustom
{
	public class CustomSelectInfoComponent : MonoBehaviour
	{
		public CustomSelectInfo info;

		public Image img;

		public Toggle tgl;

		public GameObject objNew;

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
