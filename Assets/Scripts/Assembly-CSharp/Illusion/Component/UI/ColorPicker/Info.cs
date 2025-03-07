using UnityEngine;
using UnityEngine.EventSystems;

namespace Illusion.Component.UI.ColorPicker
{
	public class Info : MouseButtonCheck
	{
		[SerializeField]
		private Canvas canvas;

		private RectTransform myRt;

		public bool isOn { get; private set; }

		public Vector2 imagePos { get; private set; }

		public Vector2 imageRate { get; private set; }

		private void Start()
		{
			if (canvas == null)
			{
				canvas = SearchCanvas(base.transform);
			}
			if (!(canvas == null))
			{
				myRt = GetComponent<RectTransform>();
				onPointerDown.AddListener(delegate(PointerEventData data)
				{
					isOn = true;
					SetImagePosition(data.position);
				});
				onPointerUp.AddListener(delegate(PointerEventData data)
				{
					isOn = false;
					SetImagePosition(data.position);
				});
				onBeginDrag.AddListener(delegate(PointerEventData data)
				{
					SetImagePosition(data.position);
				});
				onDrag.AddListener(delegate(PointerEventData data)
				{
					SetImagePosition(data.position);
				});
				onEndDrag.AddListener(delegate(PointerEventData data)
				{
					SetImagePosition(data.position);
				});
			}
		}

		private static Canvas SearchCanvas(Transform transform)
		{
			Transform transform2 = transform;
			do
			{
				Canvas component = transform2.GetComponent<Canvas>();
				if (component != null)
				{
					return component;
				}
				transform2 = transform2.parent;
			}
			while (transform2 != null);
			return null;
		}

		private void SetImagePosition(Vector2 cursorPos)
		{
			Vector2 localPoint = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(myRt, cursorPos, canvas.worldCamera, out localPoint);
			RectTransform rectTransform = myRt;
			Vector2 vector = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
			float value = localPoint.x / rectTransform.localScale.x;
			float value2 = localPoint.y / rectTransform.localScale.y;
			imagePos = new Vector2(Mathf.Clamp(value, 0f, vector.x), Mathf.Clamp(value2, 0f, vector.y));
			imageRate = new Vector2(Mathf.InverseLerp(0f, vector.x, value), Mathf.InverseLerp(0f, vector.y, value2));
		}
	}
}
