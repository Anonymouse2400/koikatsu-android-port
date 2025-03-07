using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WMG_Graph_Tooltip : WMG_GUI_Functions
{
	public delegate string TooltipLabeler(WMG_Series series, WMG_Node node);

	public TooltipLabeler tooltipLabeler;

	public WMG_Axis_Graph theGraph;

	private Canvas _canvas;

	private GameObject currentObj;

	private CanvasGroup _cg;

	private void Start()
	{
		_canvas = theGraph.toolTipPanel.GetComponent<Graphic>().canvas;
		_cg = theGraph.toolTipPanel.GetComponent<CanvasGroup>();
		if (!_cg)
		{
			_cg = theGraph.toolTipPanel.AddComponent<CanvasGroup>();
		}
	}

	private void Update()
	{
		if (theGraph.tooltipEnabled && !isTooltipObjectNull() && getControlVisibility(theGraph.toolTipPanel))
		{
			if ((bool)currentObj && !activeInHierarchy(currentObj))
			{
				MouseExitCommon(currentObj);
			}
			else
			{
				repositionTooltip();
			}
		}
	}

	public void subscribeToEvents(bool val)
	{
		if (val)
		{
			theGraph.WMG_MouseEnter += TooltipNodeMouseEnter;
			theGraph.WMG_MouseEnter_Leg += TooltipLegendNodeMouseEnter;
			theGraph.WMG_Link_MouseEnter_Leg += TooltipLegendLinkMouseEnter;
			tooltipLabeler = defaultTooltipLabeler;
		}
		else
		{
			theGraph.WMG_MouseEnter -= TooltipNodeMouseEnter;
			theGraph.WMG_MouseEnter_Leg -= TooltipLegendNodeMouseEnter;
			theGraph.WMG_Link_MouseEnter_Leg -= TooltipLegendLinkMouseEnter;
		}
	}

	private bool isTooltipObjectNull()
	{
		if (theGraph.toolTipPanel == null)
		{
			return true;
		}
		if (theGraph.toolTipLabel == null)
		{
			return true;
		}
		return false;
	}

	private void repositionTooltip()
	{
		Vector3 worldPoint;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(theGraph.toolTipPanel.GetComponent<RectTransform>(), new Vector2(Input.mousePosition.x, Input.mousePosition.y), (_canvas.renderMode != 0) ? _canvas.worldCamera : null, out worldPoint);
		float x = theGraph.tooltipOffset.x;
		float y = theGraph.tooltipOffset.y;
		theGraph.toolTipPanel.transform.localPosition = theGraph.toolTipPanel.transform.parent.InverseTransformPoint(worldPoint) + new Vector3(x, y + getSpriteHeight(theGraph.toolTipPanel) / 2f, 0f);
		EnsureTooltipStaysOnScreen(worldPoint, x, y);
	}

	private void EnsureTooltipStaysOnScreen(Vector3 position, float offsetX, float offsetY)
	{
		Vector3 position2 = theGraph.toolTipPanel.transform.position;
		offsetX *= _canvas.transform.localScale.x;
		offsetY *= _canvas.transform.localScale.y;
		Vector3[] array = new Vector3[4];
		((RectTransform)theGraph.toolTipPanel.transform).GetWorldCorners(array);
		float num = array[2].x - array[0].x;
		float num2 = array[1].y - array[0].y;
		Vector3 worldPoint;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.GetComponent<RectTransform>(), new Vector2(0f, 0f), (_canvas.renderMode != 0) ? _canvas.worldCamera : null, out worldPoint);
		Vector3 worldPoint2;
		RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvas.GetComponent<RectTransform>(), new Vector2(Screen.width, Screen.height), (_canvas.renderMode != 0) ? _canvas.worldCamera : null, out worldPoint2);
		float num3 = position.x + offsetX + num - (worldPoint2.x - worldPoint.x);
		if (num3 > worldPoint.x)
		{
			position2 = new Vector3(position.x - num3 + worldPoint.x + offsetX, position2.y, position2.z);
		}
		else
		{
			num3 = position.x + offsetX;
			if (num3 < worldPoint.x)
			{
				position2 = new Vector3(position.x - num3 + worldPoint.x + offsetX, position2.y, position2.z);
			}
		}
		float num4 = position.y + offsetY + num2 - (worldPoint2.y - worldPoint.y);
		if (num4 > worldPoint.y)
		{
			position2 = new Vector3(position2.x, position.y - num4 + worldPoint.y + offsetY + num2 / 2f, position2.z);
		}
		else
		{
			num4 = position.y + offsetY;
			if (num4 < worldPoint.y)
			{
				position2 = new Vector3(position2.x, position.y - num4 + worldPoint.y + offsetY + num2 / 2f, position2.z);
			}
		}
		theGraph.toolTipPanel.transform.position = position2;
	}

	private string defaultTooltipLabeler(WMG_Series aSeries, WMG_Node aNode)
	{
		Vector2 nodeValue = aSeries.getNodeValue(aNode);
		float num = Mathf.Pow(10f, aSeries.theGraph.tooltipNumberDecimals);
		string text = (Mathf.Round(nodeValue.x * num) / num).ToString();
		string text2 = (Mathf.Round(nodeValue.y * num) / num).ToString();
		string text3 = ((!aSeries.seriesIsLine) ? text2 : ("(" + text + ", " + text2 + ")"));
		if (aSeries.theGraph.tooltipDisplaySeriesName)
		{
			text3 = aSeries.seriesName + ": " + text3;
		}
		return text3;
	}

	private IEnumerator delayedTooltipActive()
	{
		showControl(theGraph.toolTipPanel);
		bringSpriteToFront(theGraph.toolTipPanel);
		_cg.alpha = 0f;
		yield return new WaitForEndOfFrame();
		_cg.alpha = 1f;
	}

	private void MouseEnterCommon(string textToSet, GameObject objToAnimate, Vector3 animTo)
	{
		currentObj = objToAnimate;
		changeLabelText(theGraph.toolTipLabel, textToSet);
		changeSpriteWidth(theGraph.toolTipPanel, Mathf.RoundToInt(getSpriteWidth(theGraph.toolTipLabel)) + 24);
		repositionTooltip();
		StartCoroutine(delayedTooltipActive());
		performTooltipAnimation(objToAnimate.transform, animTo);
	}

	private void MouseExitCommon(GameObject objToAnimate)
	{
		hideControl(theGraph.toolTipPanel);
		sendSpriteToBack(theGraph.toolTipPanel);
		performTooltipAnimation(objToAnimate.transform, Vector3.one);
	}

	private void TooltipNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state)
	{
		if (isTooltipObjectNull())
		{
			return;
		}
		if (state)
		{
			Vector3 animTo = new Vector3(2f, 2f, 1f);
			if (!aSeries.seriesIsLine)
			{
				animTo = ((theGraph.orientationType != 0) ? new Vector3(1.1f, 1f, 1f) : new Vector3(1f, 1.1f, 1f));
			}
			MouseEnterCommon(tooltipLabeler(aSeries, aNode), aNode.gameObject, animTo);
		}
		else
		{
			MouseExitCommon(aNode.gameObject);
		}
	}

	private void TooltipLegendNodeMouseEnter(WMG_Series aSeries, WMG_Node aNode, bool state)
	{
		if (!isTooltipObjectNull())
		{
			if (state)
			{
				MouseEnterCommon(aSeries.seriesName, aNode.gameObject, new Vector3(2f, 2f, 1f));
			}
			else
			{
				MouseExitCommon(aNode.gameObject);
			}
		}
	}

	private void TooltipLegendLinkMouseEnter(WMG_Series aSeries, WMG_Link aLink, bool state)
	{
		if (!isTooltipObjectNull() && aSeries.hidePoints)
		{
			if (state)
			{
				MouseEnterCommon(aSeries.seriesName, aLink.gameObject, new Vector3(2f, 1.05f, 1f));
			}
			else
			{
				MouseExitCommon(aLink.gameObject);
			}
		}
	}

	private void performTooltipAnimation(Transform trans, Vector3 newScale)
	{
		if (theGraph.tooltipAnimationsEnabled)
		{
			WMG_Anim.animScale(trans.gameObject, theGraph.tooltipAnimationsDuration, theGraph.tooltipAnimationsEasetype, newScale, 0f);
		}
	}
}
