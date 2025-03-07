using System;
using System.Collections.Generic;
using UnityEngine;

public class WMG_Random_Graph : WMG_Graph_Manager
{
	public UnityEngine.Object nodePrefab;

	public UnityEngine.Object linkPrefab;

	public int numNodes;

	public float minAngle;

	public float minAngleRange;

	public float maxAngleRange;

	public int minRandomNumberNeighbors;

	public int maxRandomNumberNeighbors;

	public float minRandomLinkLength;

	public float maxRandomLinkLength;

	public bool centerPropogate;

	public bool noLinkIntersection;

	public bool noNodeIntersection;

	public float noNodeIntersectionRadiusPadding;

	public int maxNeighborAttempts;

	public bool noLinkNodeIntersection;

	public float noLinkNodeIntersectionRadiusPadding;

	public bool createOnStart;

	public bool debugRandomGraph;

	private void Awake()
	{
		if (createOnStart)
		{
			GenerateGraph();
		}
	}

	public List<GameObject> GenerateGraph()
	{
		GameObject gameObject = CreateNode(nodePrefab, null);
		WMG_Node component = gameObject.GetComponent<WMG_Node>();
		return GenerateGraphFromNode(component);
	}

	public List<GameObject> GenerateGraphFromNode(WMG_Node fromNode)
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(fromNode.gameObject);
		GameObject[] array = new GameObject[numNodes];
		bool[] array2 = new bool[numNodes];
		GameObject gameObject = fromNode.gameObject;
		int num = 0;
		int num2 = 0;
		int num3 = base.NodesParent.Count - 1;
		array[num] = gameObject;
		while (base.NodesParent.Count - num3 < numNodes)
		{
			WMG_Node component = array[num].GetComponent<WMG_Node>();
			int num4 = UnityEngine.Random.Range(minRandomNumberNeighbors, maxRandomNumberNeighbors);
			if (debugRandomGraph)
			{
			}
			for (int i = 0; i < num4; i++)
			{
				int num5 = 0;
				while (num5 < maxNeighborAttempts)
				{
					float num6 = UnityEngine.Random.Range(minAngleRange, maxAngleRange);
					float num7 = UnityEngine.Random.Range(minRandomLinkLength, maxRandomLinkLength);
					bool flag = false;
					if (debugRandomGraph)
					{
					}
					if (minAngle > 0f)
					{
						for (int j = 0; j < component.numLinks; j++)
						{
							float num8 = Mathf.Abs(component.linkAngles[j] - num6);
							if (num8 > 180f)
							{
								num8 = Mathf.Abs(num8 - 360f);
							}
							if (num8 < minAngle)
							{
								flag = true;
								break;
							}
						}
					}
					if (flag)
					{
						if (debugRandomGraph)
						{
						}
						num5++;
						continue;
					}
					if (noLinkIntersection)
					{
						float p1y = component.transform.localPosition.y + (num7 + component.radius) * Mathf.Sin((float)Math.PI / 180f * num6);
						float p1x = component.transform.localPosition.x + (num7 + component.radius) * Mathf.Cos((float)Math.PI / 180f * num6);
						float p2y = component.transform.localPosition.y + component.radius * Mathf.Sin((float)Math.PI / 180f * num6);
						float p2x = component.transform.localPosition.x + component.radius * Mathf.Cos((float)Math.PI / 180f * num6);
						foreach (GameObject item in base.LinksParent)
						{
							WMG_Link component2 = item.GetComponent<WMG_Link>();
							if (component2.id == -1)
							{
								continue;
							}
							WMG_Node component3 = component2.fromNode.GetComponent<WMG_Node>();
							WMG_Node component4 = component2.toNode.GetComponent<WMG_Node>();
							float y = component3.transform.localPosition.y;
							float x = component3.transform.localPosition.x;
							float y2 = component4.transform.localPosition.y;
							float x2 = component4.transform.localPosition.x;
							if (!WMG_Util.LineSegmentsIntersect(p1x, p1y, p2x, p2y, x, y, x2, y2))
							{
								continue;
							}
							if (debugRandomGraph)
							{
							}
							flag = true;
							break;
						}
					}
					if (flag)
					{
						num5++;
						continue;
					}
					if (noNodeIntersection)
					{
						float num9 = component.transform.localPosition.y + num7 * Mathf.Sin((float)Math.PI / 180f * num6);
						float num10 = component.transform.localPosition.x + num7 * Mathf.Cos((float)Math.PI / 180f * num6);
						foreach (GameObject item2 in base.NodesParent)
						{
							WMG_Node component5 = item2.GetComponent<WMG_Node>();
							if (component5.id == -1 || !(Mathf.Pow(num10 - item2.transform.localPosition.x, 2f) + Mathf.Pow(num9 - item2.transform.localPosition.y, 2f) <= Mathf.Pow(2f * (component.radius + noNodeIntersectionRadiusPadding), 2f)))
							{
								continue;
							}
							if (debugRandomGraph)
							{
							}
							flag = true;
							break;
						}
					}
					if (flag)
					{
						num5++;
						continue;
					}
					if (noLinkNodeIntersection)
					{
						float y3 = component.transform.localPosition.y + (num7 + component.radius) * Mathf.Sin((float)Math.PI / 180f * num6);
						float x3 = component.transform.localPosition.x + (num7 + component.radius) * Mathf.Cos((float)Math.PI / 180f * num6);
						float y4 = component.transform.localPosition.y + component.radius * Mathf.Sin((float)Math.PI / 180f * num6);
						float x4 = component.transform.localPosition.x + component.radius * Mathf.Cos((float)Math.PI / 180f * num6);
						foreach (GameObject item3 in base.NodesParent)
						{
							WMG_Node component6 = item3.GetComponent<WMG_Node>();
							if (component.id == component6.id || !WMG_Util.LineIntersectsCircle(x3, y3, x4, y4, item3.transform.localPosition.x, item3.transform.localPosition.y, component6.radius + noLinkNodeIntersectionRadiusPadding))
							{
								continue;
							}
							if (debugRandomGraph)
							{
							}
							flag = true;
							break;
						}
					}
					if (flag)
					{
						num5++;
						continue;
					}
					if (noLinkNodeIntersection)
					{
						float y5 = component.transform.localPosition.y + (num7 + 2f * component.radius) * Mathf.Sin((float)Math.PI / 180f * num6);
						float x5 = component.transform.localPosition.x + (num7 + 2f * component.radius) * Mathf.Cos((float)Math.PI / 180f * num6);
						foreach (GameObject item4 in base.LinksParent)
						{
							WMG_Link component7 = item4.GetComponent<WMG_Link>();
							if (component7.id == -1)
							{
								continue;
							}
							WMG_Node component8 = component7.fromNode.GetComponent<WMG_Node>();
							WMG_Node component9 = component7.toNode.GetComponent<WMG_Node>();
							float y6 = component8.transform.localPosition.y;
							float x6 = component8.transform.localPosition.x;
							float y7 = component9.transform.localPosition.y;
							float x7 = component9.transform.localPosition.x;
							if (!WMG_Util.LineIntersectsCircle(x6, y6, x7, y7, x5, y5, component.radius + noLinkNodeIntersectionRadiusPadding))
							{
								continue;
							}
							if (debugRandomGraph)
							{
							}
							flag = true;
							break;
						}
					}
					if (flag)
					{
						num5++;
						continue;
					}
					gameObject = CreateNode(nodePrefab, fromNode.transform.parent.gameObject);
					list.Add(gameObject);
					array[base.NodesParent.Count - num3 - 1] = gameObject;
					float num11 = Mathf.Cos((float)Math.PI / 180f * num6) * num7;
					float num12 = Mathf.Sin((float)Math.PI / 180f * num6) * num7;
					gameObject.transform.localPosition = new Vector3(component.transform.localPosition.x + num11, component.transform.localPosition.y + num12, 0f);
					list.Add(CreateLink(component, gameObject, linkPrefab, null));
					break;
				}
				if (base.NodesParent.Count - num3 == numNodes)
				{
					break;
				}
			}
			array2[num] = true;
			num2++;
			if (centerPropogate)
			{
				num++;
			}
			else
			{
				int num13 = base.NodesParent.Count - num3 - num2;
				if (num13 > 0)
				{
					int[] array3 = new int[num13];
					int num14 = 0;
					for (int k = 0; k < numNodes; k++)
					{
						if (!array2[k] && k < base.NodesParent.Count - num3)
						{
							array3[num14] = k;
							num14++;
						}
					}
					num = array3[UnityEngine.Random.Range(0, num14 - 1)];
				}
			}
			if (base.NodesParent.Count - num3 == num2)
			{
				break;
			}
		}
		return list;
	}
}
