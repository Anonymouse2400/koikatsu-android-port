using System.Collections.Generic;
using UnityEngine;

public class WMG_X_Tutorial_1 : MonoBehaviour
{
	public GameObject emptyGraphPrefab;

	public WMG_Axis_Graph graph;

	public WMG_Series series1;

	public List<Vector2> series1Data;

	public bool useData2;

	public List<string> series1Data2;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(emptyGraphPrefab);
		gameObject.transform.SetParent(base.transform, false);
		graph = gameObject.GetComponent<WMG_Axis_Graph>();
		series1 = graph.addSeries();
		graph.xAxis.AxisMaxValue = 5f;
		if (useData2)
		{
			List<string> list = new List<string>();
			List<Vector2> list2 = new List<Vector2>();
			for (int i = 0; i < series1Data2.Count; i++)
			{
				string[] array = series1Data2[i].Split(',');
				list.Add(array[0]);
				if (!string.IsNullOrEmpty(array[1]))
				{
					float y = float.Parse(array[1]);
					list2.Add(new Vector2(i + 1, y));
				}
			}
			graph.groups.SetList(list);
			graph.useGroups = true;
			graph.xAxis.LabelType = WMG_Axis.labelTypes.groups;
			graph.xAxis.AxisNumTicks = list.Count;
			series1.seriesName = "Fruit Data";
			series1.UseXDistBetweenToSpace = true;
			series1.pointValues.SetList(list2);
		}
		else
		{
			series1.pointValues.SetList(series1Data);
		}
	}
}
