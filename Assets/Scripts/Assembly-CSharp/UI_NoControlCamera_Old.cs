using UnityEngine;

public class UI_NoControlCamera_Old : MonoBehaviour
{
	public RectTransform rtCanvas;

	private CameraControl camCtrl;

	private void Start()
	{
		SearchCanvas();
		if (null == camCtrl && (bool)Camera.main)
		{
			camCtrl = Camera.main.GetComponent<CameraControl>();
		}
	}

	private void SearchCanvas()
	{
		GameObject gameObject = base.gameObject;
		while (true)
		{
			Canvas component = gameObject.GetComponent<Canvas>();
			if ((bool)component)
			{
				rtCanvas = gameObject.transform as RectTransform;
				break;
			}
			if (null == gameObject.transform.parent)
			{
				break;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
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
		else
		{
			if ((!Input.GetMouseButtonDown(0) && !Input.GetMouseButtonDown(1)) || null == rtCanvas)
			{
				return;
			}
			RectTransform rectTransform = base.transform as RectTransform;
			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;
			if (rectTransform.position.x <= x && x <= rectTransform.position.x + rectTransform.sizeDelta.x * rtCanvas.localScale.x && rectTransform.position.y >= y && y >= rectTransform.position.y - rectTransform.sizeDelta.y * rtCanvas.localScale.y && (bool)camCtrl)
			{
				camCtrl.NoCtrlCondition = () => true;
			}
		}
	}
}
