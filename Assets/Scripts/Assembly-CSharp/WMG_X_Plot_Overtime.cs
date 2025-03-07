using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using UnityEngine;

public class WMG_X_Plot_Overtime : MonoBehaviour
{
	public Object emptyGraphPrefab;

	public bool plotOnStart;

	[SerializeField]
	private bool _plottingData;

	public float plotIntervalSeconds;

	public float plotAnimationSeconds;

	private Ease plotEaseType = Ease.OutQuad;

	public float xInterval;

	public bool useAreaShading;

	public bool useComputeShader;

	public bool blinkCurrentPoint;

	public float blinkAnimDuration;

	private float blinkScale = 2f;

	public bool moveXaxisMinimum;

	public Object indicatorPrefab;

	public int indicatorNumDecimals;

	private List<WMG_Change_Obj> changeObjs = new List<WMG_Change_Obj>();

	private WMG_Change_Obj plottingDataC = new WMG_Change_Obj();

	private WMG_Axis_Graph graph;

	private WMG_Series series1;

	private GameObject graphOverlay;

	private GameObject indicatorGO;

	private NumberFormatInfo tooltipNumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;

	private NumberFormatInfo yAxisNumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;

	private NumberFormatInfo seriesDataLabelsNumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;

	private NumberFormatInfo indicatorLabelNumberFormatInfo = new CultureInfo("en-US", false).NumberFormat;

	private float addPointAnimTimeline;

	private Tween blinkingTween;

	public bool plottingData
	{
		get
		{
			return _plottingData;
		}
		set
		{
			if (_plottingData != value)
			{
				_plottingData = value;
				plottingDataC.Changed();
			}
		}
	}

	private void Start()
	{
		changeObjs.Add(plottingDataC);
		GameObject gameObject = Object.Instantiate(emptyGraphPrefab) as GameObject;
		gameObject.transform.SetParent(base.transform, false);
		graph = gameObject.GetComponent<WMG_Axis_Graph>();
		graph.legend.hideLegend = true;
		graph.stretchToParent(gameObject);
		graphOverlay = new GameObject();
		graphOverlay.AddComponent<RectTransform>();
		graphOverlay.name = "Graph Overlay";
		graphOverlay.transform.SetParent(gameObject.transform, false);
		indicatorGO = Object.Instantiate(indicatorPrefab) as GameObject;
		indicatorGO.transform.SetParent(graphOverlay.transform, false);
		indicatorGO.SetActive(false);
		graph.GraphBackgroundChanged += UpdateIndicatorSize;
		graph.paddingLeftRight = new Vector2(65f, 60f);
		graph.paddingTopBottom = new Vector2(40f, 40f);
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.SetLabelsUsingMaxMin = true;
		graph.autoAnimationsEnabled = false;
		graph.xAxis.hideLabels = true;
		graph.xAxis.hideTicks = true;
		graph.xAxis.hideGrid = true;
		graph.yAxis.AxisNumTicks = 5;
		graph.yAxis.hideTicks = true;
		graph.axisWidth = 1;
		graph.yAxis.MaxAutoGrow = true;
		graph.yAxis.MinAutoGrow = true;
		series1 = graph.addSeries();
		series1.pointColor = Color.red;
		series1.lineColor = Color.green;
		series1.lineScale = 0.5f;
		series1.pointWidthHeight = 8f;
		graph.changeSpriteColor(graph.graphBackground, Color.black);
		if (useAreaShading)
		{
			series1.areaShadingType = WMG_Series.areaShadingTypes.Gradient;
			series1.areaShadingAxisValue = graph.yAxis.AxisMinValue;
			series1.areaShadingColor = new Color(16f / 51f, 20f / 51f, 0.23529412f, 1f);
			series1.areaShadingUsesComputeShader = useComputeShader;
		}
		graph.tooltipDisplaySeriesName = false;
		graph.theTooltip.tooltipLabeler = customTooltipLabeler;
		graph.yAxis.axisLabelLabeler = customYAxisLabelLabeler;
		series1.seriesDataLabeler = customSeriesDataLabeler;
		plottingDataC.OnChange += PlottingDataChanged;
		if (plotOnStart)
		{
			plottingData = true;
		}
	}

	private void PlottingDataChanged()
	{
		if (plottingData)
		{
			StartCoroutine(plotData());
		}
	}

	public IEnumerator plotData()
	{
		while (true)
		{
			yield return new WaitForSeconds(plotIntervalSeconds);
			if (!plottingData)
			{
				break;
			}
			animateAddPointFromEnd(new Vector2((series1.pointValues.Count != 0) ? (series1.pointValues[series1.pointValues.Count - 1].x + xInterval) : 0f, Random.Range(graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue * 1.2f)), plotAnimationSeconds);
			if (blinkCurrentPoint)
			{
				blinkCurrentPointAnimation();
			}
		}
	}

	private void animateAddPointFromEnd(Vector2 pointVec, float animDuration)
	{
		if (series1.pointValues.Count == 0)
		{
			series1.pointValues.Add(pointVec);
			indicatorGO.SetActive(true);
			graph.Refresh();
			updateIndicator();
			return;
		}
		series1.pointValues.Add(series1.pointValues[series1.pointValues.Count - 1]);
		if (pointVec.x > graph.xAxis.AxisMaxValue)
		{
			addPointAnimTimeline = 0f;
			Vector2 oldEnd = new Vector2(series1.pointValues[series1.pointValues.Count - 1].x, series1.pointValues[series1.pointValues.Count - 1].y);
			Vector2 newStart = new Vector2(series1.pointValues[1].x, series1.pointValues[1].y);
			Vector2 oldStart = new Vector2(series1.pointValues[0].x, series1.pointValues[0].y);
			WMG_Anim.animFloatCallbacks(() => addPointAnimTimeline, delegate(float x)
			{
				addPointAnimTimeline = x;
			}, animDuration, 1f, delegate
			{
				onUpdateAnimateAddPoint(pointVec, oldEnd, newStart, oldStart);
			}, delegate
			{
				onCompleteAnimateAddPoint();
			}, plotEaseType);
		}
		else
		{
			WMG_Anim.animVec2CallbackU(() => series1.pointValues[series1.pointValues.Count - 1], delegate(Vector2 x)
			{
				series1.pointValues[series1.pointValues.Count - 1] = x;
			}, animDuration, pointVec, delegate
			{
				updateIndicator();
			}, plotEaseType);
		}
	}

	private void blinkCurrentPointAnimation(bool fromOnCompleteAnimateAdd = false)
	{
		graph.Refresh();
		WMG_Node component = series1.getLastPoint().GetComponent<WMG_Node>();
		string text = series1.GetHashCode() + "blinkingPointAnim";
		DOTween.Kill(text);
		blinkingTween = component.objectToScale.transform.DOScale(new Vector3(blinkScale, blinkScale, blinkScale), blinkAnimDuration).SetEase(plotEaseType).SetUpdate(false)
			.SetId(text)
			.SetLoops(-1, LoopType.Yoyo);
		if (series1.pointValues.Count > 1)
		{
			WMG_Node component2 = series1.getPoints()[series1.getPoints().Count - 2].GetComponent<WMG_Node>();
			if (fromOnCompleteAnimateAdd)
			{
				blinkingTween.Goto(blinkAnimDuration * component2.objectToScale.transform.localScale.x / blinkScale, true);
			}
			component2.objectToScale.transform.localScale = Vector3.one;
		}
	}

	private void updateIndicator()
	{
		if (series1.getPoints().Count != 0)
		{
			WMG_Node component = series1.getLastPoint().GetComponent<WMG_Node>();
			graph.changeSpritePositionToY(indicatorGO, component.transform.localPosition.y);
			Vector2 nodeValue = series1.getNodeValue(component);
			indicatorLabelNumberFormatInfo.CurrencyDecimalDigits = indicatorNumDecimals;
			string aText = nodeValue.y.ToString("C", indicatorLabelNumberFormatInfo);
			graph.changeLabelText(indicatorGO.transform.GetChild(0).GetChild(0).gameObject, aText);
		}
	}

	private void onUpdateAnimateAddPoint(Vector2 newEnd, Vector2 oldEnd, Vector2 newStart, Vector2 oldStart)
	{
		series1.pointValues[series1.pointValues.Count - 1] = WMG_Util.RemapVec2(addPointAnimTimeline, 0f, 1f, oldEnd, newEnd);
		graph.xAxis.AxisMaxValue = WMG_Util.RemapFloat(addPointAnimTimeline, 0f, 1f, oldEnd.x, newEnd.x);
		updateIndicator();
		if (moveXaxisMinimum)
		{
			series1.pointValues[0] = WMG_Util.RemapVec2(addPointAnimTimeline, 0f, 1f, oldStart, newStart);
			graph.xAxis.AxisMinValue = WMG_Util.RemapFloat(addPointAnimTimeline, 0f, 1f, oldStart.x, newStart.x);
		}
	}

	private void onCompleteAnimateAddPoint()
	{
		if (moveXaxisMinimum)
		{
			series1.pointValues.RemoveAt(0);
			blinkCurrentPointAnimation(true);
		}
	}

	private string customTooltipLabeler(WMG_Series aSeries, WMG_Node aNode)
	{
		Vector2 nodeValue = aSeries.getNodeValue(aNode);
		tooltipNumberFormatInfo.CurrencyDecimalDigits = aSeries.theGraph.tooltipNumberDecimals;
		string text = nodeValue.y.ToString("C", tooltipNumberFormatInfo);
		if (aSeries.theGraph.tooltipDisplaySeriesName)
		{
			text = aSeries.seriesName + ": " + text;
		}
		return text;
	}

	private string customYAxisLabelLabeler(WMG_Axis axis, int labelIndex)
	{
		float num = axis.AxisMinValue + (float)labelIndex * (axis.AxisMaxValue - axis.AxisMinValue) / (float)(axis.axisLabels.Count - 1);
		yAxisNumberFormatInfo.CurrencyDecimalDigits = axis.numDecimalsAxisLabels;
		return num.ToString("C", yAxisNumberFormatInfo);
	}

	private string customSeriesDataLabeler(WMG_Series series, float val)
	{
		seriesDataLabelsNumberFormatInfo.CurrencyDecimalDigits = series.dataLabelsNumDecimals;
		return val.ToString("C", seriesDataLabelsNumberFormatInfo);
	}

	private void UpdateIndicatorSize(WMG_Axis_Graph aGraph)
	{
		aGraph.changeSpritePositionTo(graphOverlay, aGraph.graphBackground.transform.parent.transform.localPosition);
		float num = aGraph.getSpriteWidth(aGraph.graphBackground) - aGraph.paddingLeftRight[0] - aGraph.paddingLeftRight[1];
		aGraph.changeSpriteSize(indicatorGO, Mathf.RoundToInt(num), 2);
		aGraph.changeSpritePositionToX(indicatorGO, num / 2f);
	}
}
