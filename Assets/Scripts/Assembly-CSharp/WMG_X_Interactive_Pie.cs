using DG.Tweening;
using UnityEngine;

public class WMG_X_Interactive_Pie : MonoBehaviour
{
	public Object pieGraphPrefab;

	private WMG_Pie_Graph graph;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(pieGraphPrefab) as GameObject;
		gameObject.transform.SetParent(base.transform, false);
		graph = gameObject.GetComponent<WMG_Pie_Graph>();
		graph.Init();
		graph.interactivityEnabled = true;
		graph.useDoughnut = true;
		graph.doughnutPercentage = 0.5f;
		graph.explodeLength = 0f;
		graph.WMG_Pie_Slice_Click += myPieSliceClickFunction;
		graph.WMG_Pie_Slice_MouseEnter += myPieSliceHoverFunction;
		graph.WMG_Pie_Legend_Entry_Click += myPieLegendEntryClickFunction;
	}

	private void myPieLegendEntryClickFunction(WMG_Pie_Graph pieGraph, WMG_Legend_Entry legendEntry)
	{
	}

	private void myPieSliceClickFunction(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice)
	{
	}

	private void myPieSliceHoverFunction(WMG_Pie_Graph pieGraph, WMG_Pie_Graph_Slice aSlice, bool hover)
	{
		if (hover)
		{
			Vector3 positionFromExplode = graph.getPositionFromExplode(aSlice, 30f);
			WMG_Anim.animPosition(aSlice.gameObject, 1f, Ease.OutQuad, positionFromExplode);
		}
		else
		{
			Vector3 positionFromExplode2 = graph.getPositionFromExplode(aSlice, 0f);
			WMG_Anim.animPosition(aSlice.gameObject, 1f, Ease.OutQuad, positionFromExplode2);
		}
	}
}
