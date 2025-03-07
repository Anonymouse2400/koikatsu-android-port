using UnityEngine;
using UnityEngine.UI;

public class SpriteClickChangeCtrl : MonoBehaviour
{
	public Sprite on;

	public Sprite off;

	private Image image;

	private void Start()
	{
		image = base.gameObject.GetComponent<Image>();
	}

	public void OnChangeValue(bool _isOn)
	{
		if (!(image == null) && !(on == null) && !(off == null))
		{
			image.sprite = ((!_isOn) ? off : on);
		}
	}
}
