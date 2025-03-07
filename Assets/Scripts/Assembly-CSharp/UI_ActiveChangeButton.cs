using UnityEngine;

public class UI_ActiveChangeButton : MonoBehaviour
{
	public GameObject[] obj;

	public void ChangeActive()
	{
		if (obj != null)
		{
			GameObject[] array = obj;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(!gameObject.activeSelf);
			}
		}
	}

	public void ChangeActiveSelect(bool value)
	{
		if (obj != null)
		{
			GameObject[] array = obj;
			foreach (GameObject gameObject in array)
			{
				gameObject.SetActive(value);
			}
		}
	}
}
