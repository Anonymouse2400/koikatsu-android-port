using UnityEngine;
using UnityEngine.UI;

public class ToggleActiveChange : MonoBehaviour
{
	private Toggle toggle;

	public GameObject[] activeChangeObjs;

	public bool IsOn
	{
		get
		{
			return (bool)toggle && toggle.isOn;
		}
		set
		{
			if ((bool)toggle)
			{
				toggle.isOn = value;
			}
		}
	}

	private void Start()
	{
		toggle = GetComponent<Toggle>();
		ActiveChanger();
	}

	public void ActiveChanger(GameObject go)
	{
		if (go != null)
		{
			go.SetActive(toggle.isOn);
		}
	}

	public void ActiveChanger()
	{
		if (toggle == null)
		{
			toggle = GetComponent<Toggle>();
		}
		GameObject[] array = activeChangeObjs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(toggle.isOn);
		}
	}

	public void ActiveChanger(bool value)
	{
		GameObject[] array = activeChangeObjs;
		foreach (GameObject gameObject in array)
		{
			gameObject.SetActive(value);
		}
	}

	public void OnValueChanged(bool value)
	{
		ActiveChanger(value);
	}
}
