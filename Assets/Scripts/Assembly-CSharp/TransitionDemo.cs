using UnityEngine;
using UnityEngine.UI;

public class TransitionDemo : MonoBehaviour
{
	public EMTransition transition1;

	public EMTransition transition2;

	public Button button;

	private float delay = 0.5f;

	private bool isCloseScene = true;

	public void OnTransitionStart()
	{
		if ((bool)button && isCloseScene)
		{
			button.gameObject.SetActive(false);
		}
	}

	public void OnTransitionComplete()
	{
		if ((bool)button)
		{
			if (isCloseScene)
			{
				isCloseScene = false;
				Invoke("OnStartAnimation", delay);
			}
			else
			{
				button.gameObject.SetActive(true);
				isCloseScene = true;
			}
		}
	}

	public void OnStartAnimation()
	{
		transition1.Play();
		transition2.Play();
	}
}
