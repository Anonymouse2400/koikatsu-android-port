using Illusion.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class UI_ToggleOnOff : MonoBehaviour
{
	public Image[] imgOn;

	public Image[] imgOff;

	public GameObject[] objOn;

	public GameObject[] objOff;

	public CanvasGroup[] cgOn;

	public CanvasGroup[] cgOff;

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
		if (imgOn != null)
		{
			Image[] array = imgOn;
			foreach (Image image in array)
			{
				if (null != image)
				{
					image.enabled = check;
				}
			}
		}
		if (imgOff != null)
		{
			Image[] array2 = imgOff;
			foreach (Image image2 in array2)
			{
				if (null != image2)
				{
					image2.enabled = !check;
				}
			}
		}
		if (objOn != null)
		{
			GameObject[] array3 = objOn;
			foreach (GameObject gameObject in array3)
			{
				if (null != gameObject)
				{
					gameObject.SetActiveIfDifferent(check);
				}
			}
		}
		if (objOff != null)
		{
			GameObject[] array4 = objOff;
			foreach (GameObject gameObject2 in array4)
			{
				if (null != gameObject2)
				{
					gameObject2.SetActiveIfDifferent(!check);
				}
			}
		}
		if (cgOn != null)
		{
			CanvasGroup[] array5 = cgOn;
			foreach (CanvasGroup canvasGroup in array5)
			{
				if (null != canvasGroup)
				{
					canvasGroup.Enable(check);
				}
			}
		}
		if (cgOff == null)
		{
			return;
		}
		CanvasGroup[] array6 = cgOff;
		foreach (CanvasGroup canvasGroup2 in array6)
		{
			if (null != canvasGroup2)
			{
				canvasGroup2.Enable(!check);
			}
		}
	}
}
