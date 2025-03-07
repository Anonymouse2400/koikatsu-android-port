using UnityEngine;
using UnityEngine.EventSystems;

public class NoControlCameraTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	private BaseCameraControl camCtrl;

	private bool over;

	private void Start()
	{
		if (null == camCtrl)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if ((bool)gameObject)
			{
				camCtrl = gameObject.GetComponent<BaseCameraControl>();
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
		if (Input.GetMouseButtonUp(0))
		{
			if ((bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => false;
			}
		}
		else if (Input.GetMouseButtonDown(0) && over && (bool)camCtrl)
		{
			camCtrl.NoCtrlCondition = () => true;
		}
	}
}
