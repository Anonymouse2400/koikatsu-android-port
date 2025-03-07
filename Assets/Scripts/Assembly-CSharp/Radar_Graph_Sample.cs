using System.Collections.Generic;
using UnityEngine;

internal class Radar_Graph_Sample : MonoBehaviour
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
		List<float> data = list;
		graph.randomData = false;
		graph.numPoints = 8;
		graph.radarMinVal = 0f;
		graph.radarMaxVal = 100f;
		graph.numGrids = 5;
		List<Vector2> list2 = graph.GenRadar(data, graph.offset.x, graph.offset.y, graph.degreeOffset);
		WMG_Series wMG_Series = graph.addSeries();
		wMG_Series.pointValues.SetList(list2);
		wMG_Series.hidePoints = true;
		wMG_Series.connectFirstToLast = true;
		wMG_Series.lineColor = Color.blue;
	}

	private void Update()
	{
	}
}
