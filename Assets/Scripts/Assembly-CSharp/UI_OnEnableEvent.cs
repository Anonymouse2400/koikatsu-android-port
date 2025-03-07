using UnityEngine;
using UnityEngine.Events;

public class UI_OnEnableEvent : MonoBehaviour
{
	public UnityEvent _event;

	public void OnEnable()
	{
		_event.Invoke();
	}
}
