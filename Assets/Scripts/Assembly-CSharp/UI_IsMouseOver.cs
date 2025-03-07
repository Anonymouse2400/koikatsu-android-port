using UnityEngine;
using UnityEngine.EventSystems;

public class UI_IsMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private bool _isOver;

	public bool isOver
	{
		get
		{
			return _isOver;
		}
	}

	private void Start()
	{
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		_isOver = true;
	}

	public void OnPointerExit(PointerEventData ped)
	{
		_isOver = false;
	}
}
