using UnityEngine;
using UnityEngine.UI;

public class WMG_Text_Functions : MonoBehaviour
{
	public enum WMGpivotTypes
	{
		Bottom = 0,
		BottomLeft = 1,
		BottomRight = 2,
		Center = 3,
		Left = 4,
		Right = 5,
		Top = 6,
		TopLeft = 7,
		TopRight = 8
	}

	public void changeLabelText(GameObject obj, string aText)
	{
		Text component = obj.GetComponent<Text>();
		component.text = aText;
	}

	public void changeLabelFontSize(GameObject obj, int newFontSize)
	{
		Text component = obj.GetComponent<Text>();
		component.fontSize = newFontSize;
	}

	public Vector2 getTextSize(GameObject obj)
	{
		Text component = obj.GetComponent<Text>();
		return new Vector2(component.preferredWidth, component.preferredHeight);
	}

	public void changeSpritePivot(GameObject obj, WMGpivotTypes theType)
	{
		RectTransform component = obj.GetComponent<RectTransform>();
		Text component2 = obj.GetComponent<Text>();
		if (component == null)
		{
			return;
		}
		switch (theType)
		{
		case WMGpivotTypes.Bottom:
			component.pivot = new Vector2(0.5f, 0f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.LowerCenter;
			}
			break;
		case WMGpivotTypes.BottomLeft:
			component.pivot = new Vector2(0f, 0f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.LowerLeft;
			}
			break;
		case WMGpivotTypes.BottomRight:
			component.pivot = new Vector2(1f, 0f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.LowerRight;
			}
			break;
		case WMGpivotTypes.Center:
			component.pivot = new Vector2(0.5f, 0.5f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.MiddleCenter;
			}
			break;
		case WMGpivotTypes.Left:
			component.pivot = new Vector2(0f, 0.5f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.MiddleLeft;
			}
			break;
		case WMGpivotTypes.Right:
			component.pivot = new Vector2(1f, 0.5f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.MiddleRight;
			}
			break;
		case WMGpivotTypes.Top:
			component.pivot = new Vector2(0.5f, 1f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.UpperCenter;
			}
			break;
		case WMGpivotTypes.TopLeft:
			component.pivot = new Vector2(0f, 1f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.UpperLeft;
			}
			break;
		case WMGpivotTypes.TopRight:
			component.pivot = new Vector2(1f, 1f);
			if (component2 != null)
			{
				component2.alignment = TextAnchor.UpperRight;
			}
			break;
		}
	}

	public void changeLabelColor(GameObject obj, Color newColor)
	{
		Text component = obj.GetComponent<Text>();
		component.color = newColor;
	}

	public void changeLabelFontStyle(GameObject obj, FontStyle newFontStyle)
	{
		Text component = obj.GetComponent<Text>();
		component.fontStyle = newFontStyle;
	}

	public void changeLabelFont(GameObject obj, Font newFont)
	{
		Text component = obj.GetComponent<Text>();
		component.font = newFont;
	}
}
