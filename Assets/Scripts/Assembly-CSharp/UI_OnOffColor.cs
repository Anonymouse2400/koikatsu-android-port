using UnityEngine;
using UnityEngine.UI;

public class UI_OnOffColor : MonoBehaviour
{
	public Image[] images;

	public Color onColor = Color.white;

	public Color offColor = Color.white;

	private void Start()
	{
		Toggle component = GetComponent<Toggle>();
		if ((bool)component)
		{
			OnChange(component.isOn);
		}
	}

	public void OnChange(bool check)
	{
		if (images != null)
		{
			Color color = ((!check) ? offColor : onColor);
			Image[] array = images;
			foreach (Image image in array)
			{
				image.color = color;
			}
		}
	}
}
