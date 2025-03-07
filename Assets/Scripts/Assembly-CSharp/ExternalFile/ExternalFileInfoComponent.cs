using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace ExternalFile
{
	public class ExternalFileInfoComponent : MonoBehaviour
	{
		public ExternalFileInfo info;

		public Toggle tgl;

		public RawImage imgThumb;

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
