using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DragWindow : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IEventSystemHandler
{
	public RectTransform rtDrag;

	public RectTransform rtMove;

	public RectTransform rtCanvas;

	private Canvas canvas;

	private CanvasScaler cscaler;

	private Vector2 dragStartPosBackup = Vector2.zero;

	private CameraControl camCtrl;

	private void Start()
	{
		if (null == rtMove)
		{
			rtMove = base.transform as RectTransform;
		}
		if (null == rtDrag)
		{
			rtDrag = rtMove;
		}
		if (null == rtCanvas)
		{
			SearchCanvas();
		}
		if (null != rtCanvas && null == canvas)
		{
			canvas = rtCanvas.GetComponent<Canvas>();
			if ((bool)canvas)
			{
				cscaler = rtCanvas.GetComponent<CanvasScaler>();
			}
		}
		if (camCtrl == null && (bool)Camera.main)
		{
			camCtrl = Camera.main.GetComponent<CameraControl>();
		}
	}

	private void SearchCanvas()
	{
		GameObject gameObject = base.gameObject;
		while (true)
		{
			canvas = gameObject.GetComponent<Canvas>();
			if ((bool)canvas)
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

	private float GetScreenRate()
	{
		float num = Screen.width;
		float num2 = Screen.height;
		Vector2 one = Vector2.one;
		one.x = num / cscaler.referenceResolution.x;
		one.y = num2 / cscaler.referenceResolution.y;
		return one.x * (1f - cscaler.matchWidthOrHeight) + one.y * cscaler.matchWidthOrHeight;
	}

	private void CalcDragPosOverlay(PointerEventData ped)
	{
		Vector2 anchoredPosition = ped.position - dragStartPosBackup;
		anchoredPosition.x /= rtCanvas.localScale.x;
		anchoredPosition.y /= rtCanvas.localScale.y;
		float num = ((rtDrag.rect.size.x != rtDrag.sizeDelta.x) ? (rtDrag.rect.size.x - rtDrag.sizeDelta.x) : rtDrag.sizeDelta.x);
		float num2 = ((rtDrag.rect.size.y != rtDrag.sizeDelta.y) ? (rtDrag.rect.size.y - rtDrag.sizeDelta.y) : rtDrag.sizeDelta.y);
		anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0f, (float)Screen.width / rtCanvas.localScale.x - num);
		anchoredPosition.y = 0f - Mathf.Clamp(0f - anchoredPosition.y, 0f, (float)Screen.height / rtCanvas.localScale.y - num2);
		float num3 = ((rtMove.rect.size.x != rtMove.sizeDelta.x) ? (rtMove.rect.size.x - rtMove.sizeDelta.x) : rtMove.sizeDelta.x);
		float num4 = ((rtMove.rect.size.y != rtMove.sizeDelta.y) ? (rtMove.rect.size.y - rtMove.sizeDelta.y) : rtMove.sizeDelta.y);
		anchoredPosition.x += num3 * rtMove.pivot.x;
		anchoredPosition.y += num4 * (rtMove.pivot.y - 1f);
		rtMove.anchoredPosition = anchoredPosition;
	}

	private void CalcDragPosScreenSpace(PointerEventData ped)
	{
		Vector2 anchoredPosition = ped.position - dragStartPosBackup;
		float screenRate = GetScreenRate();
		anchoredPosition.x /= screenRate;
		anchoredPosition.y /= screenRate;
		float num = ((rtDrag.rect.size.x != rtDrag.sizeDelta.x) ? (rtDrag.rect.size.x - rtDrag.sizeDelta.x) : rtDrag.sizeDelta.x);
		float num2 = ((rtDrag.rect.size.y != rtDrag.sizeDelta.y) ? (rtDrag.rect.size.y - rtDrag.sizeDelta.y) : rtDrag.sizeDelta.y);
		anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0f, (float)Screen.width / screenRate - num);
		anchoredPosition.y = 0f - Mathf.Clamp(0f - anchoredPosition.y, 0f, (float)Screen.height / screenRate - num2);
		float num3 = ((rtMove.rect.size.x != rtMove.sizeDelta.x) ? (rtMove.rect.size.x - rtMove.sizeDelta.x) : rtMove.sizeDelta.x);
		float num4 = ((rtMove.rect.size.y != rtMove.sizeDelta.y) ? (rtMove.rect.size.y - rtMove.sizeDelta.y) : rtMove.sizeDelta.y);
		anchoredPosition.x += num3 * rtMove.pivot.x;
		anchoredPosition.y += num4 * (rtMove.pivot.y - 1f);
		rtMove.anchoredPosition = anchoredPosition;
	}

	private void SetClickPosOverlay(PointerEventData ped)
	{
		Vector2 zero = Vector2.zero;
		float num = ((rtMove.rect.size.x != rtMove.sizeDelta.x) ? (rtMove.rect.size.x - rtMove.sizeDelta.x) : rtMove.sizeDelta.x);
		float num2 = ((rtMove.rect.size.y != rtMove.sizeDelta.y) ? (rtMove.rect.size.y - rtMove.sizeDelta.y) : rtMove.sizeDelta.y);
		zero.x = (rtMove.anchoredPosition.x - num * rtMove.pivot.x) * rtCanvas.localScale.x;
		zero.y = (rtMove.anchoredPosition.y - num2 * (rtMove.pivot.y - 1f)) * rtCanvas.localScale.y;
		dragStartPosBackup = ped.position - zero;
	}

	private void SetClickPosScreenSpace(PointerEventData ped)
	{
		float screenRate = GetScreenRate();
		Vector2 zero = Vector2.zero;
		float num = ((rtMove.rect.size.x != rtMove.sizeDelta.x) ? (rtMove.rect.size.x - rtMove.sizeDelta.x) : rtMove.sizeDelta.x);
		float num2 = ((rtMove.rect.size.y != rtMove.sizeDelta.y) ? (rtMove.rect.size.y - rtMove.sizeDelta.y) : rtMove.sizeDelta.y);
		zero.x = (rtMove.anchoredPosition.x - num * rtMove.pivot.x) * screenRate;
		zero.y = (rtMove.anchoredPosition.y - num2 * (rtMove.pivot.y - 1f)) * screenRate;
		dragStartPosBackup = ped.position - zero;
	}

	public void OnPointerDown(PointerEventData ped)
	{
		switch (canvas.renderMode)
		{
		case RenderMode.ScreenSpaceCamera:
			SetClickPosScreenSpace(ped);
			break;
		case RenderMode.ScreenSpaceOverlay:
			SetClickPosOverlay(ped);
			break;
		}
		if ((bool)camCtrl)
		{
			camCtrl.NoCtrlCondition = () => true;
		}
	}

	public void OnBeginDrag(PointerEventData ped)
	{
		switch (canvas.renderMode)
		{
		case RenderMode.WorldSpace:
			break;
		case RenderMode.ScreenSpaceCamera:
			CalcDragPosScreenSpace(ped);
			break;
		case RenderMode.ScreenSpaceOverlay:
			CalcDragPosOverlay(ped);
			break;
		}
	}

	public void OnDrag(PointerEventData ped)
	{
		switch (canvas.renderMode)
		{
		case RenderMode.WorldSpace:
			break;
		case RenderMode.ScreenSpaceCamera:
			CalcDragPosScreenSpace(ped);
			break;
		case RenderMode.ScreenSpaceOverlay:
			CalcDragPosOverlay(ped);
			break;
		}
	}

	public void OnEndDrag(PointerEventData ped)
	{
		if ((bool)camCtrl)
		{
			camCtrl.NoCtrlCondition = () => false;
		}
	}

	public void OnPointerUp(PointerEventData ped)
	{
		if ((bool)camCtrl)
		{
			camCtrl.NoCtrlCondition = () => false;
		}
	}
}
