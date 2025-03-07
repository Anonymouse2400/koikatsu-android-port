using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class EmblemSelectInfoComponent : MonoBehaviour
{
	public EmblemSelectInfo info;

	public Image img;

	public Toggle tgl;

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
