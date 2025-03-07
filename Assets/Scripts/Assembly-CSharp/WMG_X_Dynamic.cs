using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WMG_X_Dynamic : MonoBehaviour
{
	public GameObject graphPrefab;

	public WMG_Axis_Graph graph;

	public bool performTests;

	public bool noTestDelay;

	public float testInterval;

	public float testGroupInterval = 2f;

	public Ease easeType;

	public GameObject realTimePrefab;

	private GameObject realTimeObj;

	private float animDuration;

	private WaitForSeconds waitTime;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(graphPrefab);
		graph = gameObject.GetComponent<WMG_Axis_Graph>();
		graph.changeSpriteParent(gameObject, base.gameObject);
		graph.changeSpritePositionTo(gameObject, Vector3.zero);
		graph.graphTitleOffset = new Vector2(0f, 60f);
		graph.autoAnimationsDuration = testInterval - 0.1f;
		waitTime = new WaitForSeconds(testInterval);
		animDuration = testInterval - 0.1f;
		if (animDuration < 0f)
		{
			animDuration = 0f;
		}
		if (performTests)
		{
			StartCoroutine(startTests());
		}
	}

	private void Update()
	{
	}

	private IEnumerator startTests()
	{
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Animation Function Tests";
		StartCoroutine(animationFunctionTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 12f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Auto Animation Tests";
		StartCoroutine(autoAnimationTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 15f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Graph Type and Orientation Tests";
		StartCoroutine(graphTypeAndOrientationTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 13f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Data Labels Tests";
		StartCoroutine(dataLabelsTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 9f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Series Tests";
		StartCoroutine(seriesTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 24f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Grouping / Null Tests";
		StartCoroutine(groupingTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 6f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Axes Tests";
		StartCoroutine(axesTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 13f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Axes Tests - Bar";
		graph.axisWidth = 2;
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval);
		}
		StartCoroutine(axesTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 13f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Axes Tests - Bar - Horizontal";
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval);
		}
		StartCoroutine(axesTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 13f);
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		graph.axisWidth = 4;
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Add / Delete Series Tests";
		StartCoroutine(addDeleteTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 11f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Add / Delete Series Tests - Bar";
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval);
		}
		StartCoroutine(addDeleteTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 11f);
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Legend Tests";
		StartCoroutine(legendTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 7f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Hide / Show Tests";
		StartCoroutine(hideShowTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 12f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Grids / Ticks Tests";
		StartCoroutine(gridsTicksTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 4f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Resize Tests";
		StartCoroutine(sizeTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 3f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Resize Tests - Resize Content";
		graph.resizeEnabled = true;
		graph.resizeProperties = (WMG_Axis_Graph.ResizeProperties)(-1);
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval);
		}
		StartCoroutine(sizeTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 3f);
		}
		graph.resizeEnabled = false;
		graph.resizeProperties = (WMG_Axis_Graph.ResizeProperties)0;
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Dynamic Data Population via Reflection";
		StartCoroutine(dynamicDataPopulationViaReflectionTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(testInterval * 8f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Real-time Tests";
		StartCoroutine(realTimeTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(10f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Axis Auto Grow / Shrink Tests";
		StartCoroutine(axisAutoGrowShrinkTests());
		if (!noTestDelay)
		{
			yield return new WaitForSeconds(23f);
		}
		yield return new WaitForSeconds(testGroupInterval);
		graph.graphTitleString = "Demo Tests Completed Successfully :)";
	}

	private IEnumerator autofitTests()
	{
		string s1 = "Short";
		string s2 = "Medium length";
		string s3 = "This is a lonnnnnnnnnnnnng string";
		graph.yAxis.SetLabelsUsingMaxMin = false;
		graph.paddingTopBottom = new Vector2(40f, 60f);
		graph.paddingLeftRight = new Vector2(60f, 40f);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.axisLabels.SetList(new List<string> { s1, s1, s1 });
		graph.xAxis.axisLabels.SetList(new List<string> { s1, s1, s1, s1 });
		graph.autoFitLabels = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.axisLabels.SetList(new List<string> { s1, s1, s3 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.axisLabels.SetList(new List<string> { s3, s1, s3 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.axisLabels.SetList(new List<string> { s1, s1, s1 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.axisLabels.SetList(new List<string> { s1, s2, s2, s1 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.axisLabels.SetList(new List<string> { s1, s2, s2, s3 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.axisLabels.SetList(new List<string> { s1, s1, s1, s1 });
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.hideLegend = false;
		graph.yAxis.SetLabelsUsingMaxMin = true;
		graph.paddingTopBottom = new Vector2(40f, 70f);
		graph.paddingLeftRight = new Vector2(45f, 40f);
		graph.xAxis.axisLabels.SetList(new List<string> { "Q1 '15", "Q2 '15", "Q3 '15", "Q4 '15" });
		graph.autoFitLabels = false;
	}

	private IEnumerator groupingTests()
	{
		List<string> xLabels = new List<string>(graph.xAxis.axisLabels);
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		Vector2 p1 = s1.pointValues[3];
		Vector2 p2 = s1.pointValues[6];
		Vector2 p3 = s1.pointValues[9];
		graph.useGroups = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.RemoveAt(3);
		s1.pointValues.RemoveAt(5);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues[9] = new Vector2(0f - s1.pointValues[9].x, s1.pointValues[9].y);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
		graph.xAxis.AxisNumTicks = graph.groups.Count;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		graph.xAxis.AxisNumTicks = 2;
		WMG_Anim.animFloat(() => graph.xAxis.AxisLabelRotation, delegate(float x)
		{
			graph.xAxis.AxisLabelRotation = x;
		}, animDuration, 60f);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues[3] = p1;
		s1.pointValues[6] = p2;
		s1.pointValues[9] = p3;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks_center;
		graph.useGroups = false;
		graph.xAxis.AxisNumTicks = 5;
		graph.xAxis.AxisLabelRotation = 0f;
		graph.xAxis.axisLabels.SetList(xLabels);
	}

	private IEnumerator seriesTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		List<Vector2> s1Data = s1.pointValues.list;
		List<Vector2> s2Data = s2.pointValues.list;
		Color s1PointColor = s1.pointColor;
		Color s2PointColor = s2.pointColor;
		Vector2 origSize = graph.getSpriteSize(graph.gameObject);
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x * 2f, origSize.y * 2f));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointWidthHeight = 15f;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointPrefab = 1;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointPrefab = 0;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.linkPrefab = 1;
		s1.lineScale = 1f;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animFloat(() => s1.linePadding, delegate(float x)
		{
			s1.linePadding = x;
		}, animDuration, -15f);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.linkPrefab = 0;
		s1.lineScale = 0.5f;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		List<Color> pointColors = new List<Color>();
		for (int i = 0; i < s1.pointValues.Count; i++)
		{
			pointColors.Add(new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f));
		}
		s1.usePointColors = true;
		s1.pointColors.SetList(pointColors);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.usePointColors = false;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.UseXDistBetweenToSpace = false;
		graph.xAxis.AxisMaxValue = graph.yAxis.AxisMaxValue * (graph.xAxisLength / graph.yAxisLength);
		graph.xAxis.SetLabelsUsingMaxMin = true;
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.numDecimalsAxisLabels = 1;
		s1.pointValues.SetList(graph.GenCircular(s1.pointValues.Count, graph.xAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f - 2f));
		s1.connectFirstToLast = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(graph.GenCircular(3, graph.xAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f - 2f));
		graph.autoAnimationsEnabled = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(graph.GenCircular2(3, graph.xAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f - 2f, 90f));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = false;
		s1.pointValues.SetList(graph.GenCircular(50, graph.xAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f, graph.yAxis.AxisMaxValue / 2f - 2f));
		s1.linePadding = 0f;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.hidePoints = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.lineColor = Color.green;
		WMG_Anim.animFloat(() => s1.lineScale, delegate(float x)
		{
			s1.lineScale = x;
		}, animDuration, 2f);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animFloat(() => s1.lineScale, delegate(float x)
		{
			s1.lineScale = x;
		}, animDuration, 0.5f);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.hideLines = true;
		s1.hidePoints = false;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(graph.GenRandomXY(50, graph.xAxis.AxisMinValue, graph.xAxis.AxisMaxValue, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = true;
		s1.pointColor = Color.green;
		s1.pointValues.SetList(graph.GenRandomXY(50, graph.xAxis.AxisMinValue, graph.xAxis.AxisMaxValue, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = false;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.lineColor = Color.white;
		s1.pointColor = s1PointColor;
		s1.hideLines = false;
		s1.pointValues.SetList(s1Data);
		s1.connectFirstToLast = false;
		s1.UseXDistBetweenToSpace = true;
		s1.pointWidthHeight = 10f;
		addSeriesWithRandomData();
		graph.lineSeries[1].GetComponent<WMG_Series>().pointValues.SetList(s2Data);
		graph.lineSeries[1].GetComponent<WMG_Series>().pointColor = s2PointColor;
		graph.lineSeries[1].GetComponent<WMG_Series>().pointPrefab = 1;
		graph.xAxis.SetLabelsUsingMaxMin = false;
		graph.xAxis.axisLabels.SetList(new List<string> { "Q1 '15", "Q2 '15", "Q3 '15", "Q4 '15" });
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks_center;
		graph.xAxis.numDecimalsAxisLabels = 0;
		graph.xAxis.AxisMaxValue = 100f;
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x, origSize.y));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
	}

	private IEnumerator autoAnimationTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		List<Vector2> s1Data = s1.pointValues.list;
		List<Vector2> s2Data = s2.pointValues.list;
		graph.autoAnimationsEnabled = true;
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		List<Vector2> s1Data2 = new List<Vector2>(s1Data);
		s1Data2[6] = new Vector2(s1Data2[6].x, s1Data2[6].y + 5f);
		s1.pointValues.SetList(s1Data2);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(s1Data);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(graph.GenRandomY(s1Data.Count, 0f, s1Data.Count - 1, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(s1Data);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsDuration = 2f * testInterval - 0.1f;
		s1.pointValues.SetList(graph.GenRandomY(s1Data.Count, 0f, s1Data.Count - 1, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		s2.pointValues.SetList(graph.GenRandomY(s2Data.Count, 0f, s2Data.Count - 1, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.pointValues.SetList(graph.GenRandomY(s1Data.Count, 0f, s1Data.Count - 1, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsDuration = testInterval - 0.1f;
		s1.pointValues.SetList(s1Data);
		s2.pointValues.SetList(s2Data);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = false;
	}

	private IEnumerator animationFunctionTests()
	{
		List<Vector3> beforeScaleLine = graph.getSeriesScaleVectors(true, -1f, 0f);
		List<Vector3> afterScaleLine = graph.getSeriesScaleVectors(true, -1f, 1f);
		List<Vector3> beforeScalePoint = graph.getSeriesScaleVectors(false, 0f, 0f);
		List<Vector3> afterScalePoint = graph.getSeriesScaleVectors(false, 1f, 1f);
		List<Vector3> beforeScaleBar = ((graph.orientationType != 0) ? graph.getSeriesScaleVectors(false, 0f, 1f) : graph.getSeriesScaleVectors(false, 1f, 0f));
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Center);
		graph.animScaleAllAtOnce(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleAllAtOnce(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Top);
		graph.animScaleAllAtOnce(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleAllAtOnce(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Bottom);
		graph.animScaleAllAtOnce(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleAllAtOnce(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Center);
		graph.animScaleBySeries(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleBySeries(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Top);
		graph.animScaleBySeries(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleBySeries(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Bottom);
		graph.animScaleBySeries(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine);
		graph.animScaleBySeries(true, animDuration, 0f, easeType, beforeScalePoint, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Center);
		graph.animScaleOneByOne(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine, 2);
		graph.animScaleOneByOne(true, animDuration / 2f, animDuration / 2f, easeType, beforeScalePoint, afterScalePoint, 2);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Top);
		graph.animScaleOneByOne(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine, 0);
		graph.animScaleOneByOne(true, animDuration / 2f, animDuration / 2f, easeType, beforeScalePoint, afterScalePoint, 0);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Bottom);
		graph.animScaleOneByOne(false, animDuration, 0f, easeType, beforeScaleLine, afterScaleLine, 1);
		graph.animScaleOneByOne(true, animDuration / 2f, animDuration / 2f, easeType, beforeScalePoint, afterScalePoint, 1);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Center);
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.animScaleAllAtOnce(true, animDuration, 0f, easeType, beforeScaleBar, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.animScaleBySeries(true, animDuration, 0f, easeType, beforeScaleBar, afterScalePoint);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.animScaleOneByOne(true, animDuration, 0f, easeType, beforeScaleBar, afterScalePoint, 0);
	}

	private IEnumerator dynamicDataPopulationViaReflectionTests()
	{
		WMG_Data_Source ds = base.gameObject.AddComponent<WMG_Data_Source>();
		ds.dataSourceType = WMG_Data_Source.WMG_DataSourceTypes.Multiple_Objects_Single_Variable;
		List<Vector2> randomData = graph.GenRandomY(graph.groups.Count, 1f, graph.groups.Count, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue);
		List<GameObject> dataProviders = new List<GameObject>();
		for (int i = 0; i < graph.groups.Count; i++)
		{
			GameObject gameObject = new GameObject();
			dataProviders.Add(gameObject);
			WMG_X_Data_Provider wMG_X_Data_Provider = gameObject.AddComponent<WMG_X_Data_Provider>();
			wMG_X_Data_Provider.vec1 = randomData[i];
			ds.addDataProviderToList(wMG_X_Data_Provider);
		}
		ds.setVariableName("vec1");
		ds.variableType = WMG_Data_Source.WMG_VariableTypes.Field;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		List<Vector2> s1Data = s1.pointValues.list;
		s1.pointValuesDataSource = ds;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		randomData = graph.GenRandomY(graph.groups.Count, 1f, graph.groups.Count, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue);
		for (int j = 0; j < graph.groups.Count; j++)
		{
			dataProviders[j].GetComponent<WMG_X_Data_Provider>().vec1 = randomData[j];
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = true;
		randomData = graph.GenRandomY(graph.groups.Count, 1f, graph.groups.Count, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue);
		for (int k = 0; k < graph.groups.Count; k++)
		{
			dataProviders[k].GetComponent<WMG_X_Data_Provider>().vec1 = randomData[k];
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.autoAnimationsEnabled = false;
		s1.pointValuesDataSource = null;
		s1.pointValues.SetList(s1Data);
	}

	private IEnumerator realTimeTests()
	{
		WMG_Data_Source ds1 = graph.lineSeries[0].AddComponent<WMG_Data_Source>();
		WMG_Data_Source ds2 = graph.lineSeries[1].AddComponent<WMG_Data_Source>();
		ds1.dataSourceType = WMG_Data_Source.WMG_DataSourceTypes.Single_Object_Single_Variable;
		ds2.dataSourceType = WMG_Data_Source.WMG_DataSourceTypes.Single_Object_Single_Variable;
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		realTimeObj = Object.Instantiate(realTimePrefab);
		graph.changeSpriteParent(realTimeObj, base.gameObject);
		ds1.setDataProvider(realTimeObj.transform);
		ds2.setDataProvider(realTimeObj.transform);
		ds1.setVariableName("localPosition.x");
		ds2.setVariableName("localPosition.y");
		s1.realTimeDataSource = ds1;
		s2.realTimeDataSource = ds2;
		graph.xAxis.AxisMaxValue = 0f;
		graph.xAxis.AxisMaxValue = 5f;
		graph.yAxis.AxisMinValue = -200f;
		graph.yAxis.AxisMaxValue = 200f;
		s1.seriesName = "Hex X";
		s2.seriesName = "Hex Y";
		s1.UseXDistBetweenToSpace = false;
		s2.UseXDistBetweenToSpace = false;
		graph.xAxis.SetLabelsUsingMaxMin = true;
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.numDecimalsAxisLabels = 1;
		s1.StartRealTimeUpdate();
		s2.StartRealTimeUpdate();
		WMG_Anim.animPosition(realTimeObj, 3f, Ease.Linear, new Vector3(200f, -150f, 0f));
		yield return new WaitForSeconds(4f);
		WMG_Anim.animPosition(realTimeObj, 1f, Ease.Linear, new Vector3(-150f, 100f, 0f));
		yield return new WaitForSeconds(3f);
		WMG_Anim.animPosition(realTimeObj, 1f, Ease.Linear, new Vector3(-125f, 75f, 0f));
		yield return new WaitForSeconds(3f);
		s1.StopRealTimeUpdate();
		s2.StopRealTimeUpdate();
	}

	private IEnumerator axisAutoGrowShrinkTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		s1.ResumeRealTimeUpdate();
		s2.ResumeRealTimeUpdate();
		yield return new WaitForSeconds(1f);
		graph.graphTitleString = "Axis Auto Grow / Shrink - Disabled";
		WMG_Anim.animPosition(realTimeObj, 1f, Ease.Linear, new Vector3(-125f, 300f, 0f));
		yield return new WaitForSeconds(2f);
		WMG_Anim.animPosition(realTimeObj, 1f, Ease.Linear, new Vector3(-125f, 75f, 0f));
		yield return new WaitForSeconds(6f);
		graph.graphTitleString = "Axis Auto Grow / Shrink - Enabled";
		graph.yAxis.MaxAutoGrow = true;
		graph.yAxis.MinAutoGrow = true;
		graph.yAxis.MaxAutoShrink = true;
		graph.yAxis.MinAutoShrink = true;
		graph.autoShrinkAtPercent = 0.6f;
		graph.autoGrowAndShrinkByPercent = 0.2f;
		WMG_Anim.animPosition(realTimeObj, 2f, Ease.Linear, new Vector3(-125f, 350f, 0f));
		yield return new WaitForSeconds(3f);
		WMG_Anim.animPosition(realTimeObj, 2f, Ease.Linear, new Vector3(-125f, 75f, 0f));
		yield return new WaitForSeconds(3f);
		WMG_Anim.animPosition(realTimeObj, 2f, Ease.Linear, new Vector3(-5f, 5f, 0f));
		yield return new WaitForSeconds(8f);
		s1.StopRealTimeUpdate();
		s2.StopRealTimeUpdate();
	}

	private IEnumerator hideShowTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		graph.legend.hideLegend = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.hideLabels = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.hideLabels = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.hideTicks = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.hideTicks = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.hideGrid = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.yAxis.hideGrid = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.SetActive(graph.xAxis.AxisObj, false);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.SetActive(graph.yAxis.AxisObj, false);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.hidePoints = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s2.hideLines = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.hideLines = true;
		s2.hidePoints = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.hideLegend = false;
		graph.xAxis.hideLabels = false;
		graph.yAxis.hideLabels = false;
		graph.xAxis.hideTicks = false;
		graph.yAxis.hideTicks = false;
		graph.xAxis.hideGrid = false;
		graph.yAxis.hideGrid = false;
		graph.SetActive(graph.xAxis.AxisObj, true);
		graph.SetActive(graph.yAxis.AxisObj, true);
		s1.hideLines = false;
		s2.hideLines = false;
		s1.hidePoints = false;
		s2.hidePoints = false;
	}

	private IEnumerator gridsTicksTests()
	{
		List<string> xLabels = new List<string>(graph.xAxis.axisLabels);
		WMG_Anim.animInt(() => graph.yAxis.AxisNumTicks, delegate(int x)
		{
			graph.yAxis.AxisNumTicks = x;
		}, animDuration, 11);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks;
		graph.xAxis.SetLabelsUsingMaxMin = true;
		WMG_Anim.animInt(() => graph.xAxis.AxisNumTicks, delegate(int x)
		{
			graph.xAxis.AxisNumTicks = x;
		}, animDuration, 11);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animInt(() => graph.yAxis.AxisNumTicks, delegate(int x)
		{
			graph.yAxis.AxisNumTicks = x;
		}, animDuration, 3);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animInt(() => graph.xAxis.AxisNumTicks, delegate(int x)
		{
			graph.xAxis.AxisNumTicks = x;
		}, animDuration, 5);
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.xAxis.LabelType = WMG_Axis.labelTypes.ticks_center;
		graph.xAxis.SetLabelsUsingMaxMin = false;
		graph.xAxis.axisLabels.SetList(xLabels);
	}

	private IEnumerator sizeTests()
	{
		Vector2 origSize = graph.getSpriteSize(graph.gameObject);
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x * 2f, origSize.y * 2f));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x * 2f, origSize.y));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x, origSize.y * 2f));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		WMG_Anim.animSize(graph.gameObject, animDuration, Ease.Linear, new Vector2(origSize.x, origSize.y));
	}

	private IEnumerator legendTests()
	{
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
		graph.legend.oppositeSideLegend = true;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
		graph.legend.oppositeSideLegend = false;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		addSeriesWithRandomData();
		addSeriesWithRandomData();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.legendType = WMG_Legend.legendTypes.Right;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		graph.deleteSeries();
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.legend.legendType = WMG_Legend.legendTypes.Bottom;
	}

	private IEnumerator dataLabelsTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		s1.dataLabelsEnabled = true;
		s2.dataLabelsEnabled = true;
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.combo;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line_stacked;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.combo;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		s1.dataLabelsEnabled = false;
		s2.dataLabelsEnabled = false;
	}

	private IEnumerator graphTypeAndOrientationTests()
	{
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.combo;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line_stacked;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked_percent;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.horizontal;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_side;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.combo;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line_stacked;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.bar_stacked_percent;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.graphType = WMG_Axis_Graph.graphTypes.line;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.orientationType = WMG_Axis_Graph.orientationTypes.vertical;
	}

	private IEnumerator axesTests()
	{
		graph.axesType = WMG_Axis_Graph.axesTypes.I_II;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.II;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.II_III;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.III;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.III_IV;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.IV;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.I_IV;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.CENTER;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.I;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_X;
		graph.xAxis.AxisUseNonTickPercent = true;
		WMG_Anim.animVec2(() => graph.theOrigin, delegate(Vector2 x)
		{
			graph.theOrigin = x;
		}, animDuration, new Vector2(graph.theOrigin.x, graph.yAxis.AxisMaxValue));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN_Y;
		graph.yAxis.AxisUseNonTickPercent = true;
		WMG_Anim.animVec2(() => graph.theOrigin, delegate(Vector2 x)
		{
			graph.theOrigin = x;
		}, animDuration, new Vector2(graph.xAxis.AxisMaxValue, graph.theOrigin.y));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.AUTO_ORIGIN;
		WMG_Anim.animVec2(() => graph.theOrigin, delegate(Vector2 x)
		{
			graph.theOrigin = x;
		}, animDuration, new Vector2(graph.xAxis.AxisMaxValue / 4f, graph.yAxis.AxisMaxValue / 2f));
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.axesType = WMG_Axis_Graph.axesTypes.I;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
	}

	private IEnumerator addDeleteTests()
	{
		WMG_Series s1 = graph.lineSeries[0].GetComponent<WMG_Series>();
		WMG_Series s2 = graph.lineSeries[1].GetComponent<WMG_Series>();
		List<Vector2> s1Data = s1.pointValues.list;
		List<Vector2> s2Data = s2.pointValues.list;
		Color s1PointColor = s1.pointColor;
		Color s2PointColor = s2.pointColor;
		float barWidth = graph.barWidth;
		addSeriesWithRandomData();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		graph.deleteSeries();
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		graph.lineSeries[0].GetComponent<WMG_Series>().pointValues.SetList(s1Data);
		graph.lineSeries[0].GetComponent<WMG_Series>().pointColor = s1PointColor;
		if (!noTestDelay)
		{
			yield return waitTime;
		}
		addSeriesWithRandomData();
		graph.lineSeries[1].GetComponent<WMG_Series>().pointValues.SetList(s2Data);
		graph.lineSeries[1].GetComponent<WMG_Series>().pointColor = s2PointColor;
		graph.lineSeries[1].GetComponent<WMG_Series>().pointPrefab = 1;
		graph.barWidth = barWidth;
	}

	private void addSeriesWithRandomData()
	{
		WMG_Series wMG_Series = graph.addSeries();
		wMG_Series.UseXDistBetweenToSpace = true;
		wMG_Series.lineScale = 0.5f;
		wMG_Series.pointColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
		wMG_Series.seriesName = "Series " + graph.lineSeries.Count;
		wMG_Series.pointValues.SetList(graph.GenRandomY(graph.groups.Count, 1f, graph.groups.Count, graph.yAxis.AxisMinValue, graph.yAxis.AxisMaxValue));
		wMG_Series.setOriginalPropertyValues();
	}
}
