using UnityEngine;

public class RepeatButton : MonoBehaviour
{
	public float interval = 0.25f;

	private float nextClick;

	private bool isPressed;

	public bool IsPress
	{
		get
		{
			return isPressed;
		}
	}

	private void OnPress(bool isPress)
	{
		isPressed = isPress;
		nextClick = Time.realtimeSinceStartup + interval;
	}

	private void Update()
	{
		if (isPressed && Time.realtimeSinceStartup < nextClick)
		{
			nextClick = Time.realtimeSinceStartup + interval;
		}
	}
}
