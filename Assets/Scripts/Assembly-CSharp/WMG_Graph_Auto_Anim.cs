using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WMG_Graph_Auto_Anim : MonoBehaviour
{
	public WMG_Axis_Graph theGraph;

	public void subscribeToEvents(bool val)
	{
		for (int i = 0; i < theGraph.lineSeries.Count; i++)
		{
			if (theGraph.activeInHierarchy(theGraph.lineSeries[i]))
			{
				WMG_Series component = theGraph.lineSeries[i].GetComponent<WMG_Series>();
				if (val)
				{
					component.SeriesDataChanged += SeriesDataChangedMethod;
				}
				else
				{
					component.SeriesDataChanged -= SeriesDataChangedMethod;
				}
			}
		}
	}

	public void addSeriesForAutoAnim(WMG_Series aSeries)
	{
		aSeries.SeriesDataChanged += SeriesDataChangedMethod;
	}

	private void SeriesDataChangedMethod(WMG_Series aSeries)
	{
		List<GameObject> points = aSeries.getPoints();
		for (int i = 0; i < points.Count; i++)
		{
			if (aSeries.seriesIsLine)
			{
				GameObject go = points[i];
				string text = aSeries.GetHashCode() + "autoAnim" + i;
				bool isLast = i == points.Count - 1;
				if (aSeries.currentlyAnimating)
				{
					DOTween.Kill(text);
					animateLinkCallback(aSeries, go, isLast);
				}
				WMG_Anim.animPositionCallbacks(go, theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, new Vector3(aSeries.AfterPositions()[i].x, aSeries.AfterPositions()[i].y), delegate
				{
					animateLinkCallback(aSeries, go, isLast);
				}, delegate
				{
					animateLinkCallbackEnd(aSeries, isLast);
				}, text);
			}
			else
			{
				Vector2 changeSpritePositionTo = theGraph.getChangeSpritePositionTo(points[i], new Vector2(aSeries.AfterPositions()[i].x, aSeries.AfterPositions()[i].y));
				WMG_Anim.animPosition(points[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, new Vector3(changeSpritePositionTo.x, changeSpritePositionTo.y));
				WMG_Anim.animSize(points[i], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, new Vector2(aSeries.AfterWidths()[i], aSeries.AfterHeights()[i]));
			}
		}
		List<GameObject> dataLabels = aSeries.getDataLabels();
		for (int j = 0; j < dataLabels.Count; j++)
		{
			if (aSeries.seriesIsLine)
			{
				float x = aSeries.dataLabelsOffset.x;
				float y = aSeries.dataLabelsOffset.y;
				Vector2 vector = theGraph.getChangeSpritePositionTo(dataLabels[j], new Vector2(x, y));
				vector = new Vector2(vector.x + aSeries.AfterPositions()[j].x, vector.y + aSeries.AfterPositions()[j].y);
				vector = ((theGraph.orientationType != 0) ? new Vector2(vector.x + (float)aSeries.AfterHeights()[j] / 2f, vector.y) : new Vector2(vector.x, vector.y + (float)aSeries.AfterHeights()[j] / 2f));
				WMG_Anim.animPosition(dataLabels[j], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, new Vector3(vector.x, vector.y));
				continue;
			}
			float y2 = aSeries.dataLabelsOffset.y + aSeries.AfterPositions()[j].y + theGraph.barWidth / 2f;
			float x2 = aSeries.dataLabelsOffset.x + aSeries.AfterPositions()[j].x + (float)aSeries.AfterWidths()[j];
			if (aSeries.getBarIsNegative(j))
			{
				x2 = 0f - aSeries.dataLabelsOffset.x - (float)aSeries.AfterWidths()[j] + (float)Mathf.RoundToInt((theGraph.barAxisValue - theGraph.xAxis.AxisMinValue) / (theGraph.xAxis.AxisMaxValue - theGraph.xAxis.AxisMinValue) * theGraph.xAxisLength);
			}
			if (theGraph.orientationType == WMG_Axis_Graph.orientationTypes.vertical)
			{
				y2 = aSeries.dataLabelsOffset.y + aSeries.AfterPositions()[j].y + (float)aSeries.AfterHeights()[j];
				x2 = aSeries.dataLabelsOffset.x + aSeries.AfterPositions()[j].x + theGraph.barWidth / 2f;
				if (aSeries.getBarIsNegative(j))
				{
					y2 = 0f - aSeries.dataLabelsOffset.y - (float)aSeries.AfterHeights()[j] + (float)Mathf.RoundToInt((theGraph.barAxisValue - aSeries.yAxis.AxisMinValue) / (aSeries.yAxis.AxisMaxValue - aSeries.yAxis.AxisMinValue) * theGraph.yAxisLength);
				}
			}
			Vector2 changeSpritePositionTo2 = theGraph.getChangeSpritePositionTo(dataLabels[j], new Vector2(x2, y2));
			WMG_Anim.animPosition(dataLabels[j], theGraph.autoAnimationsDuration, theGraph.autoAnimationsEasetype, new Vector3(changeSpritePositionTo2.x, changeSpritePositionTo2.y));
		}
		if (!aSeries.currentlyAnimating)
		{
			aSeries.currentlyAnimating = true;
		}
	}

	private void animateLinkCallback(WMG_Series aSeries, GameObject aGO, bool isLast)
	{
		WMG_Node component = aGO.GetComponent<WMG_Node>();
		if (component.links.Count != 0)
		{
			WMG_Link component2 = component.links[component.links.Count - 1].GetComponent<WMG_Link>();
			component2.Reposition();
		}
		if (isLast)
		{
			aSeries.updateAreaShading(null);
		}
		if (aSeries.connectFirstToLast)
		{
			component = aSeries.getPoints()[0].GetComponent<WMG_Node>();
			WMG_Link component3 = component.links[0].GetComponent<WMG_Link>();
			component3.Reposition();
		}
	}

	private void animateLinkCallbackEnd(WMG_Series aSeries, bool isLast)
	{
		aSeries.RepositionLines();
		if (isLast)
		{
			aSeries.updateAreaShading(null);
		}
		aSeries.currentlyAnimating = false;
	}
}
