using System.Collections.Generic;
using UnityEngine;

public class WMG_X_Simple_Pie : MonoBehaviour
{
	public GameObject emptyPiePrefab;

	public WMG_Pie_Graph pieGraph;

	public List<float> testData;

	public List<string> testStrings;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(emptyPiePrefab);
		gameObject.transform.SetParent(base.transform, false);
		pieGraph = gameObject.GetComponent<WMG_Pie_Graph>();
		pieGraph.Init();
		pieGraph.sliceValues.SetList(testData);
		pieGraph.sliceLabels.SetList(testStrings);
	}
}
