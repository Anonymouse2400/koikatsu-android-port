using UnityEngine;
using UnityEngine.EventSystems;

public class UI_DrawTimer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	[SerializeField]
	private CanvasGroup cgSpace;

	private bool isOver;

	private float timeCnt;

	private float drawTime = 2f;

	private float fadeTime = 0.3f;

	public void Setup(float _drawTime, float _fadeTime)
	{
		drawTime = _drawTime;
		fadeTime = _fadeTime;
		timeCnt = drawTime;
		cgSpace.alpha = 1f;
	}

	public void OnPointerEnter(PointerEventData ped)
	{
		isOver = true;
	}

	public void OnPointerExit(PointerEventData ped)
	{
		isOver = false;
	}

	private void Update()
	{
		if (isOver)
		{
			timeCnt = drawTime;
			cgSpace.alpha = 1f;
		}
		timeCnt = Mathf.Max(0f, timeCnt - Time.deltaTime);
		if (timeCnt < fadeTime)
		{
			float alpha = Mathf.InverseLerp(0f, fadeTime, timeCnt);
			cgSpace.alpha = alpha;
		}
	}
}
