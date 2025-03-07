using UnityEngine;
using UnityEngine.EventSystems;

public class UI_NoControlCamera : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private CameraControl_Ver2 camCtrl;

	private bool over;

	private void Start()
	{
		if (null == camCtrl)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if ((bool)gameObject)
			{
				camCtrl = gameObject.GetComponent<CameraControl_Ver2>();
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		over = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		over = false;
	}

	public void Update()
	{
		if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			if ((bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => false;
			}
		}
		else if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && over && (bool)camCtrl)
		{
			camCtrl.NoCtrlCondition = () => true;
		}
	}
}
