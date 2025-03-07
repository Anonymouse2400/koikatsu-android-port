using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_X_Ring_Graph : WMG_GUI_Functions
{
	public UnityEngine.Object ringGraphPrefab;

	public bool onlyRandomizeData;

	private List<WMG_Ring_Graph> ringGraphs = new List<WMG_Ring_Graph>();

	private void Start()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(ringGraphPrefab) as GameObject;
		changeSpriteParent(gameObject, base.gameObject);
		changeSpritePositionTo(gameObject, new Vector3(-230f, 175f));
		WMG_Ring_Graph component = gameObject.GetComponent<WMG_Ring_Graph>();
		component.Init();
		component.values.Add(180f);
		component.values.Add(335f);
		component.leftRightPadding = new Vector2(60f, 60f);
		component.topBotPadding = new Vector2(50f, 0f);
		component.innerRadiusPercentage = 0.3f;
		component.ringPointWidthFactor = 20f;
		changeSpriteSize(gameObject, 420, 350);
		ringGraphs.Add(component);
		GameObject gameObject2 = UnityEngine.Object.Instantiate(ringGraphPrefab) as GameObject;
		changeSpriteParent(gameObject2, base.gameObject);
		changeSpritePositionTo(gameObject2, new Vector3(200f, 115f));
		WMG_Ring_Graph component2 = gameObject2.GetComponent<WMG_Ring_Graph>();
		component2.Init();
		component2.degrees = 180f;
		component2.leftRightPadding = new Vector2(60f, 60f);
		component2.topBotPadding = new Vector2(50f, -120f);
		component2.innerRadiusPercentage = 0.3f;
		component2.ringPointWidthFactor = 20f;
		changeSpriteSize(gameObject2, 420, 230);
		ringGraphs.Add(component2);
		GameObject gameObject3 = UnityEngine.Object.Instantiate(ringGraphPrefab) as GameObject;
		changeSpriteParent(gameObject3, base.gameObject);
		changeSpritePositionTo(gameObject3, new Vector3(-230f, -180f));
		WMG_Ring_Graph component3 = gameObject3.GetComponent<WMG_Ring_Graph>();
		component3.Init();
		component3.degrees = 0f;
		component3.leftRightPadding = new Vector2(60f, 60f);
		component3.topBotPadding = new Vector2(50f, 50f);
		component3.innerRadiusPercentage = 0.3f;
		component3.ringPointWidthFactor = 20f;
		changeSpriteSize(gameObject3, 370, 350);
		ringGraphs.Add(component3);
		GameObject gameObject4 = UnityEngine.Object.Instantiate(ringGraphPrefab) as GameObject;
		changeSpriteParent(gameObject4, base.gameObject);
		changeSpritePositionTo(gameObject4, new Vector3(200f, -180f));
		WMG_Ring_Graph component4 = gameObject4.GetComponent<WMG_Ring_Graph>();
		component4.degrees = 0f;
		component4.Init();
		component4.leftRightPadding = new Vector2(60f, 60f);
		component4.topBotPadding = new Vector2(50f, 50f);
		component4.innerRadiusPercentage = 0.3f;
		component4.ringPointWidthFactor = 20f;
		component4.bandMode = false;
		changeSpriteSize(gameObject4, 370, 350);
		ringGraphs.Add(component4);
	}

	public void randomize()
	{
		for (int i = 0; i < ringGraphs.Count; i++)
		{
			int numPoints = ringGraphs[i].values.Count;
			if (!onlyRandomizeData)
			{
				numPoints = UnityEngine.Random.Range(1, 6);
			}
			ringGraphs[i].values.SetList(ringGraphs[i].GenRandomList(numPoints, ringGraphs[i].minValue, ringGraphs[i].maxValue));
			if (!onlyRandomizeData)
			{
				ringGraphs[i].bandMode = 1 == UnityEngine.Random.Range(0, 2);
				if (i == 3)
				{
					ringGraphs[i].degrees = UnityEngine.Random.Range(0, 180);
					ringGraphs[i].topBotPadding = new Vector2((0f - ringGraphs[i].outerRadius) * (1f - Mathf.Cos(ringGraphs[i].degrees / 2f * ((float)Math.PI / 180f))) + 50f, (0f - ringGraphs[i].outerRadius) * (1f - Mathf.Cos(ringGraphs[i].degrees / 2f * ((float)Math.PI / 180f))) + 50f);
				}
			}
		}
	}

	public void dataOnlyChanged()
	{
		onlyRandomizeData = !onlyRandomizeData;
	}
}
