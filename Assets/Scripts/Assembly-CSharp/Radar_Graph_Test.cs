using System.Collections.Generic;
using DG.Tweening;
using Illusion.Game;
using UnityEngine;

internal class Radar_Graph_Test : MonoBehaviour
{
	[SerializeField]
	private WMG_Radar_Graph graph;

	private void Start()
	{
		List<float> list = new List<float>();
		list.Add(100f);
		list.Add(60f);
		list.Add(60f);
		list.Add(50f);
		list.Add(50f);
		list.Add(30f);
		list.Add(5f);
		list.Add(15f);
		List<float> list2 = list;
		string[] labels = new string[9] { "あいうえお", "かきくけこ", "さしすせそ", "たちつてと", "なにぬねの", "はひふへほ", "まみむめも", "やゆよ", "らりるれろ" };
		Utils.RadarGraph.SetGraph(graph, list2.Count, 0f, 100f, 5);
		Utils.RadarGraph.SetLabel(graph, labels);
		WMG_Series series = graph.addSeries();
		Utils.RadarGraph.Offset(graph, list2, series, null);
		Color cyan = Color.cyan;
		Utils.RadarGraph.SetSeries(series, false, cyan);
		cyan.a = 0.5f;
		int num = 0;
		Utils.RadarGraph.CreateFill(graph, series, cyan, "FillObj_" + num++.ToString("00"));
		Utils.RadarGraph.CreateLine(graph, series, Color.white, new Vector2(0f, 10f), new Vector2(280f, 260f), "LineObj");
		List<Vector3> seriesScaleVectors = graph.getSeriesScaleVectors(true, -1f, 0f);
		List<Vector3> seriesScaleVectors2 = graph.getSeriesScaleVectors(true, -1f, 1f);
		List<Vector3> seriesScaleVectors3 = graph.getSeriesScaleVectors(false, 0f, 0f);
		List<Vector3> seriesScaleVectors4 = graph.getSeriesScaleVectors(false, 1f, 1f);
		float duration = 3f;
		Ease anEaseType = Ease.OutQuad;
		graph.changeAllLinePivots(WMG_Text_Functions.WMGpivotTypes.Center);
		graph.animScaleAllAtOnce(false, duration, 0f, anEaseType, seriesScaleVectors, seriesScaleVectors2);
		graph.animScaleAllAtOnce(true, duration, 0f, anEaseType, seriesScaleVectors3, seriesScaleVectors4);
	}

	private void Update()
	{
	}
}
